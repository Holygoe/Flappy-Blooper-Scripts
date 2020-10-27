using System;
using TinyLocalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class PurchaseForWatchingAdButtonLayout : PurchaseButtonLayout
    {
#pragma warning disable CS0649
        [SerializeField] private Localize getButtonText;
        [HideInInspector] [SerializeField] private ProductForWatchingAd product;
#pragma warning restore CS0649

        private const string WatchAdKey = "WATCH_AD";
        private const string AdNotAvailableKey = "AD_NOT_AVAILABLE";
        
        public override Product Product
        {
            set
            {
                if (value is ProductForWatchingAd productForWatchingAd)
                {
                    product = productForWatchingAd;
                }
                else
                {
                    throw new ArgumentException($"{nameof(Product)} must be {typeof(ProductForWatchingAd)}");
                }
            }
        }

        private void OnEnable()
        {
            product.VideoAd.OnStatusChanged += VideoAdStatusChanged;
        }

        private void OnDisable()
        {
            product.VideoAd.OnStatusChanged -= VideoAdStatusChanged;
        }

        private void VideoAdStatusChanged(object sender, EventArgs e)
        {
            UpdateButton();
        }

        public override void UpdateButton()
        {
            if (product.VideoAd.AdLoaded)
            {
                Button.Interactable = true;
                getButtonText.QuickConnect(WatchAdKey);
            }
            else
            {
                Button.Interactable = false;
                getButtonText.QuickConnect(AdNotAvailableKey);
            }
        }
    }
}