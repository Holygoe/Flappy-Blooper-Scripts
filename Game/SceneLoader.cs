using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlappyBlooper
{
    public class SceneLoader : Singelton<SceneLoader>
    {
#pragma warning disable CS0649
        [SerializeField] private GameLoaderDimmer fastDimmer;
        [SerializeField] private GameLoaderDimmer slowDimmer;
#pragma warning restore CS0649
        
        private static bool _busy;
        private static bool _hasSceneLoadingConflict;
        private static GameScene _conflictedLoadedScene;
        
        public static GameScene PivotScene { get; private set; }

        public static event EventHandler OnSceneLoaded;

        private void Start()
        {
            PivotScene = GameScene.HomeMenu;
            
            var isAnySceneLoaded = false;

            foreach (GameScene scene in Enum.GetValues(typeof(GameScene)))
            {
                if (!scene.IsLoaded()) continue;
                isAnySceneLoaded = true;
                SceneManager.SetActiveScene(scene.GetScene());
            }

            if (!isAnySceneLoaded)
            {
                StartCoroutine(LoadSceneAsync(GameScene.HomeMenu));
            }

            DataManager.OnGameDataUpdated += GameDataUpdated;
        }

        private static void GameDataUpdated(object sender, GameDataUpdatedEventArgs e)
        {
            LoadScene(e.NeedToResolve ? GameScene.Resolving : GameScene.HomeMenu, true);
        }
        
        private static string GetLoadedScene()
        {
            foreach (GameScene scene in Enum.GetValues(typeof(GameScene)))
            {
                if (scene.IsLoaded())
                {
                    return scene.ToStringName();
                }
            }

            return string.Empty;
        }

        public static void LoadScene(GameScene scene, bool slowLoad = false)
        {
            if (_busy)
            {
                _hasSceneLoadingConflict = true;
                _conflictedLoadedScene = scene;
            }
            else
            {
                Instance.StartCoroutine(Instance.LoadSceneAsync(scene, slowLoad));
            }
        }

        private IEnumerator LoadSceneAsync(GameScene scene, bool slowLoad = false)
        {
            _busy = true;

            if (PopupWindow.Opened)
            {
                PopupWindow.Opened.Close();
            }

            var dimmer = slowLoad ? slowDimmer : fastDimmer;

            dimmer.Switch(true);
            yield return StartCoroutine(dimmer.WaitForSwitchAsync());

            var loadedScene = GetLoadedScene();
            if (loadedScene != string.Empty)
            {
                yield return SceneManager.UnloadSceneAsync(loadedScene);
            }

            Time.timeScale = 1;

            if (slowLoad)
            {
                dimmer.StartWaitingForTap();
                yield return StartCoroutine(dimmer.WaitForTapAsync());
            }

            yield return SceneManager.LoadSceneAsync(scene.ToStringName(), LoadSceneMode.Additive);

            if (scene.IsPivot())
            {
                PivotScene = scene;
            }
            
            if (_hasSceneLoadingConflict)
            {
                _hasSceneLoadingConflict = false;
                StartCoroutine(LoadSceneAsync(_conflictedLoadedScene, true));
                yield break;
            }
            
            dimmer.Switch(false);
            yield return StartCoroutine(dimmer.WaitForSwitchAsync());
            OnSceneLoaded?.Invoke(null, EventArgs.Empty);
            _busy = false;
        }
    }

    public static class GameSceneExtensions
    {
        public static bool IsPivot(this GameScene scene)
        {
            return scene == GameScene.HomeMenu || scene == GameScene.Story;
        }

        public static string ToStringName(this GameScene scene)
        {
            return (scene == GameScene.Pivot) ? SceneLoader.PivotScene.ToString() : scene.ToString();
        }
        
        public static Scene GetScene(this GameScene scene)
        {
            return SceneManager.GetSceneByName(scene.ToStringName());
        }

        public static bool IsLoaded(this GameScene scene)
        {
            return scene.GetScene().isLoaded;
        }
    }
    
    public enum GameScene
    {
        HomeMenu,
        Level,
        Characters,
        Store,
        Settings,
        Items,
        Resolving,
        Rewards,
        Story,
        Pivot
    }
}