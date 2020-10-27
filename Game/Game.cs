using System;
using System.Collections;
using UnityEngine;

namespace FlappyBlooper
{
    public class Game : Singelton<Game>
    {
        private const string PrefsGameMode = "GameMode";
        private const string PrefsCurrentChapter = "CurrentChapter";
        private const string PrefsCurrentLevel = "CurrentLevel";
        
        private StoredChapterTag _currentChapterTag;
        private StoredInt _currentLevelIndex;
        private StoredInt _mode;
        private DataManager _dataManager;

        public static GameMode Mode
        {
            get => (GameMode) Instance._mode.Value;
            set => Instance._mode.Value = (int) value;
        }

        
        public static Chapter CurrentChapter => CurrentChapterTag.ToChapter();
        public static Stage CurrentStage => CurrentChapter.GetStage(CurrentStageIndex);
        public static int Highscore => GameData.highscore;
        public static GameData GameData => Instance._dataManager.GameData;
        public static int CurrentStageIndex
        {
            get => Instance._currentLevelIndex.Value;
            set => Instance._currentLevelIndex.Value = value;
        }

        public static ChapterTag CurrentChapterTag
        {
            get => Instance._currentChapterTag.Value;
            set
            {
                Instance._currentChapterTag.Value = value;
                CurrentStageIndex = 0;
            }
        }

        public static event EventHandler OnItIsNewDay;

        private void Awake()
        {
            Debug.Log("Game awake");
            _dataManager = new DataManager();
            _dataManager.LoadLocalData();
            _mode = new StoredInt(PrefsGameMode, 0);
            _currentChapterTag = new StoredChapterTag(PrefsCurrentChapter, ChapterTag.DazeMountain);
            _currentLevelIndex = new StoredInt(PrefsCurrentLevel, 0);
            StartCoroutine(CheckForItIsNewDayAsync());
            
            SocialHandler.OnUserStatusChanged += (sender, e) =>
            {
                if (SocialHandler.SignedIn) DataManager.LoadData();
            };
        }

        private IEnumerator CheckForItIsNewDayAsync()
        {
            yield return new WaitForSecondsRealtime(3);
            if (GameData.LastOpenTheGameDate.Date == DateTime.Now.Date) yield break;
            GameData.daysInTheGameCount++;
            GameData.greenGooCollectedTodayCount = 0;
            GameData.LastOpenTheGameDate = DateTime.Now.Date;
            AchievementTag.DaysInGame.ToAchievement().UpdateProgress(GameData.daysInTheGameCount, false);
            DataManager.SaveData();
            OnItIsNewDay?.Invoke(this, EventArgs.Empty);
        }

        public static bool UpdateHighscore(int value, bool saveGame = true)
        {
            if (value <= GameData.highscore) return false;
            GameData.highscore = value;
            if (saveGame) DataManager.SaveData();
            return true;
        }
        
        public static void SaveCurrentStageProgress(int newProgress, bool saveGame)
        {
            var chapterData = ChapterData.FindData(GameData.chapterDataset, CurrentChapterTag);
            var stageData = chapterData.stages[CurrentStageIndex];
            if (stageData.progress >= newProgress) return;
            stageData.progress = newProgress;
            if (saveGame) DataManager.SaveData();
        }
    }
}