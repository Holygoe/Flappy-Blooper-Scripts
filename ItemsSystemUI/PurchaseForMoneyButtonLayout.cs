using System;
using RudeBlooper.InAppPurchase;
using TinyLocalization;
using TMPro;
using UnityEngine;

namespace FlappyBlooper
{
    public class PurchaseForMoneyButtonLayout : PurchaseButtonLayout
    {
#pragma warning disable CS0649
        [SerializeField] private TextMeshProUGUI priceText;
        [HideInInspector] [SerializeField] private ProductForMoney product;
#pragma warning restore CS0649

        private const string NotAvailableKey = "NOT_AVAILABLE";

        public override Product Product
        {
            set
            {
                if (value is ProductForMoney productForMoney)
                {
                    product = productForMoney;
                }
                else
                {
                    throw new ArgumentException($"{nameof(Product)} must be {typeof(ProductForMoney)}");
                }
            }
        }

        private void OnEnable()
        {
            InAppPurchaseManager.OnInitialized += InAppPurchaseManagerInitialized;
        }

        private void OnDisable()
        {
            InAppPurchaseManager.OnInitialized -= InAppPurchaseManagerInitialized;
        }

        private void InAppPurchaseManagerInitialized(object sender, EventArgs e)
        {
            UpdateButton();
        }

        public override void UpdateButton()
        {
            UnityEngine.Purchasing.Product inAppProduct = null;
            
            if (InAppPurchaseManager.Initialized)
            {
                inAppProduct = InAppPurchaseManager.GetProduct(product.InAppProductTag);
            }
            
            if (inAppProduct is null)
            {
                priceText.text = LocalizationManager.GetTranslation(NotAvailableKey) as string;
                Button.Interactable = false;
            }
            else
            {
                priceText.text = inAppProduct.metadata.localizedPriceString;
                Button.Interactable = true;
            }
        }
    }
}