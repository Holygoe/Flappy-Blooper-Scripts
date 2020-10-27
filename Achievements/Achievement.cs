using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Game / Achievement")]
    public class Achievement : ScriptableObject, ITaggedName
    {
#pragma warning disable CS0649
        [SerializeField] private AchievementTag tag;
        [SerializeField] private string descriptionKey;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite iconBg;
        [SerializeField] private string nameKey;
        [FormerlySerializedAs("copyRewardFrom")] [SerializeField]
        private Achievement parentAchievement;
        [SerializeField] private CountableItem rewardItem;
        [FormerlySerializedAs("rewards")] [SerializeField]
        private AchievementThreshold[] thresholds;
#pragma warning restore CS0649

        private AchievementData Data => AchievementData.FindData(Game.GameData.achievementDataset, tag);
        private int MaxRewardIndex => Thresholds.Length - 1;
        public int Progress => Data.progress;
        public int Claimed => Data.claimed;
        public string NameKey => nameKey;
        public string DescriptionKey => descriptionKey;
        public Sprite Icon => icon;

        public Sprite IconBg
        {
            get => iconBg;
            set => iconBg = value;
        }

        public CountableItem RewardItem
        {
            get
            {
                if (!parentAchievement) return rewardItem;

                if (!parentAchievement.rewardItem)
                {
                    throw new ArgumentNullException(nameof(parentAchievement),
                        $"Parent achievement must have not null {nameof(rewardItem)}");
                }

                return parentAchievement.rewardItem;
            }
        }

        public AchievementThreshold[] Thresholds
        {
            get
            {
                if (!parentAchievement) return thresholds;

                if (parentAchievement.thresholds?.Length == 0)
                {
                    throw new ArgumentNullException(nameof(parentAchievement),
                        $"Parent achievement must have not null {nameof(thresholds)}");
                }

                return parentAchievement.thresholds;
            }
        }

        public int PreviousThreshold
        {
            get
            {
                if (Data.claimed == 0) return 0;

                var i = Data.claimed - 1 > MaxRewardIndex ? MaxRewardIndex : Data.claimed - 1;
                return Thresholds[i].Value;
            }
        }

        public AchievementStatus Status
        {
            get
            {
                if (Data.claimed >= Thresholds.Length) return AchievementStatus.Completed;
                if (Data.progress == 0) return AchievementStatus.Empty;
                return Thresholds[Data.claimed].Value <= Progress
                    ? AchievementStatus.Topical
                    : AchievementStatus.InProgress;
            }
        }

        public Enum TaggedName => tag;

        public static event EventHandler OnAnyAchievementUpdated;
        public static event EventHandler OnAnyAchievementCompleted;

        public void UpdateProgress(int value, bool saveGame = true)
        {
            if (value <= Data.progress) return;

            Data.progress = value;
            OnAnyAchievementUpdated?.Invoke(this, EventArgs.Empty);

            if (saveGame) DataManager.SaveData();
        }

        public void IncreaseProgress(int value, bool saveGame = true)
        {
            if (Status == AchievementStatus.Completed) return;
            Data.progress += value;
            OnAnyAchievementUpdated?.Invoke(this, EventArgs.Empty);
            if (saveGame) DataManager.SaveData();
        }

        public bool TryToGetNextThreshold(out AchievementThreshold threshold)
        {
            var index = Data.claimed > MaxRewardIndex ? MaxRewardIndex : Data.claimed;

            if (Thresholds != null && Thresholds.Length > 0)
            {
                threshold = Thresholds[index];
                return true;
            }

            threshold = default;
            return false;
        }

        public void ClaimReward(bool saveGame = true)
        {
            if (Status != AchievementStatus.Topical) return;

            if (TryToGetNextThreshold(out var reward)) RewardItem.Add(reward.RewardItemsCount, true, false);

            Data.claimed++;
            OnAnyAchievementUpdated?.Invoke(this, EventArgs.Empty);

            if (Status == AchievementStatus.Completed) OnAnyAchievementCompleted?.Invoke(this, EventArgs.Empty);

            if (saveGame) DataManager.SaveData();
        }
    }

    [Serializable]
    public struct AchievementThreshold
    {
#pragma warning disable CS0649
        [FormerlySerializedAs("threshold")] [FormerlySerializedAs("treashold")] [SerializeField]
        private int value;
        [FormerlySerializedAs("amount")] [FormerlySerializedAs("rewardItemsCount")] [SerializeField]
        private int itemsAmount;
#pragma warning restore CS0649

        public int Value => value;
        public int RewardItemsCount => itemsAmount;
    }

    public enum AchievementStatus
    {
        InProgress,
        Topical,
        Completed,
        Empty
    }

    public static class AchievementTagExtensions
    {
        public static Achievement ToAchievement(this AchievementTag tag)
        {
            return AssetTag.Achievements.ToAsset().GetItem<Achievement>(tag);
        }
    }

    public enum AchievementTag
    {
        Mileage,
        BlooperHighscore,
        JeffHighscore,
        LoonaHighscore,
        ZombieAlHighscore,
        BlooperLevel,
        JeffLevel,
        LoonaLevel,
        ZombieAlLevel,
        DaysInGame,
        TigersJoeyHighscore,
        TigersJoeyLevel,
        TigerChandlerHighscore,
        TigerChandlerLevel
    }
}