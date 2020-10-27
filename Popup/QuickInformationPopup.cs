using TinyLocalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class QuickInformationPopup : Singelton<QuickInformationPopup>
    { 
#pragma warning disable CS0649
        [SerializeField] private Localize text;
#pragma warning restore CS0649

        public static void Open(Item item)
        {
            if (Instance is null || item is null) return;
            Instance.text.QuickConnect(item.InformationKey == "" ? LocalizeKey.NO_INFORMATION : item.InformationKey);
            Instance.gameObject.SetActive(true);
        }

        public static void Close()
        {
            Instance.gameObject.SetActive(false);
        }
    }
}