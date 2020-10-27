using System;
using TinyLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class AchievementCard : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Achievement achievement;
        [SerializeField] private Image icon;
        [SerializeField] private Image iconBg;
        [SerializeField] private Localize captionText;
        [SerializeField] private Localize descriptionText;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardAmountText;
        [SerializeField] private Image progressFiller;
        [SerializeField] private Button claimButton;
        [SerializeField] private Transform starsTransform;
        [SerializeField] private Sprite starOn;
        [SerializeField] private Sprite starOff;
#pragma warning restore CS0649
        
        public Achievement Achievement
        {
            set => achievement = value;
        }

        private void Awake()
        {
            claimButton.onClick.AddListener(ClaimReward);
        }

        private void OnEnable()
        {
            UpdateCard();
        }

        private void UpdateCard()
        {
            if (achievement == null) return;

            var status = achievement.Status;

            if (!achievement.TryToGetNextThreshold(out var nextReward)) return;

            icon.sprite = achievement.Icon;
            iconBg.sprite = achievement.IconBg;
            captionText.QuickConnect(achievement.NameKey);
            descriptionText.QuickConnect(achievement.DescriptionKey);
            rewardIcon.sprite = achievement.RewardItem.Icon;
            rewardAmountText.text = "+" + nextReward.RewardItemsCount;
            claimButton.interactable = status == AchievementStatus.Topical;

            float current = achievement.Progress;
            float min = achievement.PreviousThreshold;
            float max = nextReward.Value;
            progressFiller.fillAmount = (current - min) / (max - min);

            UpdateStars();
        }

        private void ClaimReward()
        {
            if (!achievement.TryToGetNextThreshold(out var nextReward)) throw new Exception();
            
            CollectItemPopup.Open(new ItemTarget(achievement.RewardItem, nextReward.RewardItemsCount));
            achievement.ClaimReward();
            UpdateCard();
        }

        private void UpdateStars()
        {
            starsTransform.Clear();

            var claimed = achievement.Claimed;
            var maxProgress = achievement.Thresholds.Length;
            var star = new GameObject("StarSet", typeof(Image)).GetComponent<Image>();
            star.rectTransform.sizeDelta = new Vector2(15, 15);
            star.sprite = starOn;

            for (var i = 0; i < claimed; i++)
            {
                Instantiate(star, starsTransform);
            }

            star.sprite = starOff;

            for (var i = claimed; i < maxProgress; i++)
            {
                Instantiate(star, starsTransform);
            }

            Destroy(star.gameObject);
        }
    }
}