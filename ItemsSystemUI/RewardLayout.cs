using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class RewardLayout : MonoBehaviour
    {
        private static readonly int ClaimRewardTrigger = Animator.StringToHash("ClaimReward");
        
#pragma warning disable CS0649
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Transform rewardedLabel;
        [SerializeField] private Animator animator;
#pragma warning restore CS0649

        public void UpdateLayout(ItemTarget itemTarget, bool rewarded)
        {
            itemTarget.Item.UpdateIcon(itemIcon);
            countText.text = itemTarget.Count;
            rewardedLabel.gameObject.SetActive(rewarded);
        }

        public void ClaimReward()
        {
            rewardedLabel.gameObject.SetActive(true);
            animator.SetTrigger(ClaimRewardTrigger);
        }
    }
}