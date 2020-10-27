using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class CollectItemPopup : Singelton<CollectItemPopup>
    {
#pragma warning disable CS0649
        [SerializeField] private Button collectButton;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
#pragma warning restore CS0649

        private PurchaseOfferCallback _onPopupClosedCallback;
        private ItemTarget _itemContainer;
        private AudioSource _audioSource;
        
        public static void Open(ItemTarget itemTarget, PurchaseOfferCallback onPopupClosedCallback = null)
        {
            if (Instance is null) return;
            Instance._onPopupClosedCallback = onPopupClosedCallback;
            Instance.UpdatePopup(itemTarget);
        }

        private void UpdatePopup(ItemTarget itemTarget)
        {
            _itemContainer = itemTarget;
            title.text = itemTarget.Title;
            itemTarget.Item.UpdateIcon(icon);
            gameObject.SetActive(true);
            _audioSource.Play();
        }

        public static void Open(Queue<ItemTarget> itemTargets, CollectItemCallback callback)
        {
            if (itemTargets.Count <= 0)
            {
                callback?.Invoke();
                return;
            }
            
            var itemTarget = itemTargets.Dequeue();
            Open(itemTarget, (success) => { Open(itemTargets, callback); });
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            collectButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                _onPopupClosedCallback?.Invoke(true);
                if (_itemContainer?.Item != null)
                {
                    _itemContainer.Item.WasCollected();
                }
            });
        }
    }

    public delegate void CollectItemCallback();
}