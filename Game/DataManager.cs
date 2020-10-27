using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlappyBlooper
{
    public class DataManager
    {
        private const string DataFileName = "SaveData.json";
        private const int MinTimeBetweenCloudSaves = 10;
        private const int MaxCloudWaitsCount = 6;

        private enum CloudAction { Saving, Loading }

        public GameData GameData { get; private set; }
        private static GameData _resolvingGameData;
        private bool _waitingForTheCloudToSave;
        private bool _waitingForTheCloudToLoad;
        private readonly string _gameDataPath;
        private CloudAction _cloudAction;
        private int _cloudWaitsCount;
        
        private static DataManager Instance { get; set; }
        public static CloudStatus CloudStatus { get; private set; }
        public static bool WaitingForTheCloud { get; private set; }

        public static event EventHandler<GameDataUpdatedEventArgs> OnGameDataUpdated;
        public static event EventHandler OnCloudStatusUpdated;

        public DataManager()
        {
            Instance = this;
            CloudStatus = CloudStatus.CloudUpdating;
            WaitingForTheCloud = false;
            _gameDataPath = Path.Combine(Application.persistentDataPath, DataFileName);
            _waitingForTheCloudToSave = false;
            _waitingForTheCloudToLoad = false;
            _cloudWaitsCount = 0;
        }

        public static void ResetGameData()
        {
            Instance.GameData = new GameData();
            SaveData();
        }

        public static void LoadData()
        {
            if (Social.localUser.authenticated)
            {
                if (CloudStatus == CloudStatus.CloudUpdating)
                {
                    Instance._waitingForTheCloudToLoad = true;
                    if (!WaitingForTheCloud)
                    {
                        Instance.WaitForTheCloudAsync();
                    }
                    return;
                }

                Instance.OpenCloud(CloudAction.Loading);
            }
            else
            {
                Instance.LoadLocalData();
            }
        }

        public static void SaveData()
        {
            Instance.GameData.SaveDate = DateTime.Now;
            Instance.GameData.dataVersion = GameData.DataVersionReference;
            Instance.SaveLocally();

            if (CloudStatus == CloudStatus.CloudUpdating)
            {
                Instance._waitingForTheCloudToSave = true;
                if (!WaitingForTheCloud)
                {
                    Instance.WaitForTheCloudAsync();
                }
                return;
            }

            if (Social.localUser.authenticated)
            {
                Instance.OpenCloud(CloudAction.Saving);
            }
            else
            {
                Instance.UpdateCloudStatus(CloudStatus.CloudDisconnected);
            }
        }

        public void LoadLocalData()
        {
            UpdateCloudStatus(CloudStatus.CloudDisconnected);

            if (File.Exists(_gameDataPath))
            {
                using (var streamReader = File.OpenText(_gameDataPath))
                {
                    var jsonString = streamReader.ReadToEnd();

                    try
                    {
                        var  loadedGameData = JsonUtility.FromJson<GameData>(jsonString);
                        if (loadedGameData.dataVersion != GameData.DataVersionReference)
                        {
                            GameData = new GameData();
                        }
                        else
                        {
                            GameData = loadedGameData;
                            GameData.NormalizeData();
                        }
                    }
                    catch
                    {
                        GameData = new GameData();
                    }
                }
            }
            else
            {
                GameData = new GameData();
            }
            
            OnGameDataUpdated?.Invoke(this, new GameDataUpdatedEventArgs(false));
        }

        private void SaveLocally()
        {
            var jsonString = JsonUtility.ToJson(GameData);

            using (var streamWriter = File.CreateText(_gameDataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        private void OpenCloud(CloudAction cloudAction)
        {
            UpdateCloudStatus(CloudStatus.CloudUpdating);
            _cloudAction = cloudAction;
            var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(DataFileName, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseMostRecentlySaved, CloudWasOpenedCallback);
        }

        private void CloudWasOpenedCallback(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                switch (_cloudAction)
                {
                    case CloudAction.Saving:
                    {
                        var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                        var jsonString = JsonUtility.ToJson(GameData);
                        var gameData = Encoding.ASCII.GetBytes(jsonString);
                        var builder = new SavedGameMetadataUpdate.Builder();
                        var updatedMetadata = builder.Build();
                        savedGameClient.CommitUpdate(metaData, updatedMetadata, gameData, CloudDataWasWrittenCallback);
                        break;
                    }
                    case CloudAction.Loading:
                    {
                        var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                        savedGameClient.ReadBinaryData(metaData, CloudDataWasReadCallback);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                UpdateCloudStatus(CloudStatus.CloudDisconnected);
            }
        }

        private void CloudDataWasWrittenCallback(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            UpdateCloudStatus(status == SavedGameRequestStatus.Success
                ? CloudStatus.CloudUpdated
                : CloudStatus.CloudDisconnected);
        }

        private void CloudDataWasReadCallback(SavedGameRequestStatus status, byte[] data)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                GameData loadedGameData;
                var jsonString = Encoding.ASCII.GetString(data);

                try
                {
                    loadedGameData = JsonUtility.FromJson<GameData>(jsonString);
                    loadedGameData.NormalizeData();
                }
                catch
                {
                    loadedGameData = new GameData();
                }

                if (loadedGameData.dataVersion == GameData.DataVersionReference &&
                    (loadedGameData.gameProgress > GameData.gameProgress ||
                    loadedGameData.gameProgress == GameData.gameProgress && loadedGameData.SaveDate > GameData.SaveDate))
                {   
                    _resolvingGameData = loadedGameData;
                    OnGameDataUpdated?.Invoke(this, new GameDataUpdatedEventArgs(true));
                    UpdateCloudStatus(CloudStatus.CloudUpdated);
                }
                else
                {
                    UpdateCloudStatus(CloudStatus.CloudUpdated);
                    OpenCloud(CloudAction.Saving);
                }
            }
            else
            {
                UpdateCloudStatus(CloudStatus.CloudDisconnected);
            }
        }

        private void UpdateCloudStatus(CloudStatus newStatus)
        {
            if (CloudStatus == newStatus) return;
            CloudStatus = newStatus;
            OnCloudStatusUpdated?.Invoke(this, System.EventArgs.Empty);
        }

        private async void WaitForTheCloudAsync()
        {
            while (true)
            {
                WaitingForTheCloud = true;
                OnCloudStatusUpdated?.Invoke(null, EventArgs.Empty);
                if (_cloudWaitsCount > MaxCloudWaitsCount)
                {
                    _cloudWaitsCount = 0;
                    _waitingForTheCloudToLoad = false;
                    _waitingForTheCloudToSave = false;
                    UpdateCloudStatus(CloudStatus.CloudDisconnected);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(MinTimeBetweenCloudSaves));
                    _cloudWaitsCount++;
                }

                if (CloudStatus == CloudStatus.CloudUpdating)
                {
                    continue;
                }

                if (_waitingForTheCloudToLoad)
                {
                    _waitingForTheCloudToLoad = false;
                    LoadData();

                    if (_waitingForTheCloudToSave)
                    {
                        _cloudWaitsCount = 0;
                        continue;
                    }
                }

                if (_waitingForTheCloudToSave)
                {
                    _waitingForTheCloudToSave = false;
                    SaveData();
                }

                _cloudWaitsCount = 0;
                WaitingForTheCloud = false;
                OnCloudStatusUpdated?.Invoke(null, EventArgs.Empty);
                break;
            }
        }

        public static void ResolveGameData(bool useCloudData)
        {
            if (useCloudData && _resolvingGameData != null)
            {
                Instance.GameData = _resolvingGameData;
            }

            _resolvingGameData = null;
            SaveData();
            OnGameDataUpdated?.Invoke(null, new GameDataUpdatedEventArgs(false));
        }
    }

    public class GameDataUpdatedEventArgs : EventArgs
    {
        public GameDataUpdatedEventArgs(bool needToResolve)
        {
            NeedToResolve = needToResolve;
        }

        public bool NeedToResolve { get; }
    }
    
    public enum CloudStatus
    {
        CloudDisconnected,
        CloudUpdated,
        CloudUpdating
    }
}