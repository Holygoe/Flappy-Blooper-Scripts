using System;
using TinyLocalization;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    public class Level : Singelton<Level>
    {
        public const float RealCameraSize = 50f;
        private const float ContinuePopupDelay = 1f;
        private const int BarrierScoreAmount = 10;
        private const float StageCompletedDelay = 2f;

#pragma warning disable CS0649
        [SerializeField] private PopupWindow continuePopup;
        [SerializeField] private Countdown countdownAfterPause;
        [SerializeField] private GameOverPopup gameOverPopup;
        [SerializeField] private Localize highscoreText;
        [SerializeField] private LevelFormation levelFormation;
        [SerializeField] private GameObject scorePanel;
        [SerializeField] private GameObject tapToFlapText;
        [SerializeField] private AudioClipBundle letsPlay;
        [SerializeField] private AudioSource audioSource;
#pragma warning disable CS0649

        private int _barriersPassed;
        private bool _highscoreAchieved;
        private int _score;
        private bool _scroll;
        private LevelState _state;
        private static int _difficulty;

        public static int Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                Barrier.UpdateDifficulty(value);
            }
        }

        public static int StageLength { get; private set; }
        public static Countdown CountdownAfterPause => Instance.countdownAfterPause;
        public static LevelFormation LevelFormation => Instance.levelFormation;

        public static LevelState State
        {
            get => Instance ? Instance._state : LevelState.Paused;

            set
            {
                Instance._state = value;
                Instance._scroll = value == LevelState.Playing || value == LevelState.StageComplete;
                Time.timeScale = value == LevelState.Paused ? 0 : 1;
                OnLevelStateChanged?.Invoke(Instance, EventArgs.Empty);
            }
        }

        public static int Score => Instance._score;
        public static bool Scroll => Instance != null && Instance._scroll;

        public static event EventHandler OnLevelStateChanged;

        private void Awake()
        {
            _state = LevelState.WaitForStart;
            tapToFlapText.SetActive(true);
            gameOverPopup.Close();
            continuePopup.Close();
            scorePanel.SetActive(true);
            countdownAfterPause.onWasOver.AddListener(() => State = LevelState.Playing);
        }

        private void Start()
        {
            Time.timeScale = 1;
            if (Game.Mode == GameMode.Story) highscoreText.QuickConnect(LocalizeKey.STORY_MODE);
            else highscoreText.SetStringParameter("value", Game.Highscore.ToString());
            MusicMaster.Play(Music.Level);
            StageLength = Game.CurrentStage.Length;
            Difficulty = Game.Mode == GameMode.Story ? Game.CurrentStage.Difficulty : 0;
            audioSource.PlayOneShot(letsPlay.AudioClip, letsPlay.Volume);
        }

        private void OnEnable()
        {
            Player.Instance.OnDied += OverTheGame;
            Player.Instance.OnStartedPlaying += StartToPlay;
            Player.Instance.OnRanOutOfLives += PlayerRanOutOfLives;
        }

        private void OnDisable()
        {
            Player.Instance.OnDied -= OverTheGame;
            Player.Instance.OnStartedPlaying -= StartToPlay;
            Player.Instance.OnRanOutOfLives -= PlayerRanOutOfLives;
        }

        private void OverTheGame(object sender, EventArgs e)
        {
            RecordProgress();
            State = LevelState.GameOver;
            scorePanel.SetActive(false);
            gameOverPopup.Open(_highscoreAchieved);
        }

        private void StartToPlay(object sender, EventArgs e)
        {
            State = LevelState.Playing;
            tapToFlapText.SetActive(false);
        }

        private void PlayerRanOutOfLives(object sender, EventArgs e)
        {
            Invoke(nameof(OpenContinuePopup), ContinuePopupDelay);
        }

        private void OpenContinuePopup()
        {
            continuePopup.Open();
        }

        public static void RecordProgress()
        {
            if (Game.Mode == GameMode.Rating)
            {
                if (Game.UpdateHighscore(Score, false)) Instance._highscoreAchieved = true;
                Character.Current.HighscoreAchievement.UpdateProgress(Score, false);
                
                Social.ReportScore(Score, GPGSIds.leaderboard_rating_mode, success =>
                {
                    if (success) Debug.Log("Score was reported on leaderboard.");
                });
                
            }
            
            AchievementTag.Mileage.ToAchievement().IncreaseProgress(Score, false);
            DataManager.SaveData();
        }

        public void ContinueToPlay(bool useGreenGoo)
        {
            State = LevelState.Playing;
            StartCoroutine(Player.Instance.ContinueToPlayLevelAsync(useGreenGoo));
        }

        public static void CountPassedBarrier()
        {
            Instance._score += BarrierScoreAmount;
            Instance._barriersPassed++;

            if (Game.Mode == GameMode.Rating || Instance._barriersPassed < StageLength) return;

            State = LevelState.StageComplete;
            Instance.Invoke(nameof(FinishStage), StageCompletedDelay);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void FinishStage()
        {
            State = LevelState.GameOver;
            RecordProgress();
            StagePopup.Open(Player.Accuracy);
        }
    }
}