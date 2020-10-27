using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class InsufficientFundsPopup : Singelton<InsufficientFundsPopup>
    {
#pragma warning disable CS0649
        [SerializeField] private Image icon;
#pragma warning restore CS0649

        public static void Open(CountableItem countableItem)
        {
            if (Instance is null) return;
            countableItem.UpdateIcon(Instance.icon);
            Instance.gameObject.SetActive(true);
        }
    }
}