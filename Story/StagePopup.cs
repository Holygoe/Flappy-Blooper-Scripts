using System.Collections;
using System.Collections.Generic;
using TinyLocalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class StagePopup : Singelton<StagePopup>
    {
#pragma warning disable CS0649
        [SerializeField] private Localize chapterText;
        [SerializeField] private Localize stageText;
        [SerializeField] private StarsArray stars;
        [SerializeField] private RudeButton okayButton;
        [SerializeField] private AudioClipBundle woohSound;
        [SerializeField] private RewardLayout[] rewards;
        [SerializeField] private bool playMusic;
#pragma warning restore CS0649

        private readonly Queue<ItemTarget> _collectedRewards = new Queue<ItemTarget>();

        private void Awake()
        {
            if (okayButton) okayButton.onClick.AddListener(() => 
            {
                CollectItemPopup.Open(_collectedRewards, () =>
                {
                    SceneLoader.LoadScene(GameScene.Pivot, true);
                });
            });
        }

        public static void Open(int progress = -1)
        {
            if (!Instance) return;
            if (Instance.playMusic) Music.StageComplete.Play();
            Instance.gameObject.SetActive(true);
            Instance.UpdateLayout(progress);
        }

        private void UpdateLayout(int newProgress)
        {
            _collectedRewards.Clear();
            var chapter = Game.CurrentChapter;
            chapterText.QuickConnect(chapter.NameKey);
            var stageInfo = chapter.GetStageInfo(Game.CurrentStageIndex);
            stageText.SetStringParameter("value", stageInfo.Number);
            var currentProgress = stageInfo.StageData.progress;
            stars.UpdateProgress(currentProgress > newProgress ? currentProgress : newProgress);

            for (var i = 0; i < 3; i++)
            {
                rewards[i].UpdateLayout(stageInfo.Rewards[i], stageInfo.IsRewarded(i));
            }

            if (newProgress <= currentProgress) return;
            if (okayButton) okayButton.Interactable = false;
            StartCoroutine(ClaimRewardsAsync(currentProgress, newProgress));
        }
        
        private IEnumerator ClaimRewardsAsync(int currentProgress, int newProgress)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            var stage = Game.CurrentStage;

            for (var i = currentProgress; i < newProgress; i++)
            {
                yield return new WaitForSecondsRealtime(0.3f);
                SoundMaster.PlayOneShot(woohSound);
                rewards[i].ClaimReward();
                _collectedRewards.Enqueue(stage.Rewards[i]);
                stage.Rewards[i].Collect(true, false);
            }
            
            Game.SaveCurrentStageProgress(Player.Accuracy, true);
            DataManager.SaveData();
            okayButton.Interactable = true;
        }
    }
}