using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

namespace RudeBlooper.InAppPurchase
{
    public class InAppPurchaseManager : MonoBehaviour, IStoreListener
    {
        private static IStoreController _storeController;
        private static IExtensionProvider _storeExtensionProvider;
        private static BuyProductCallback _buyProductCallback;
        
        public static bool Initialized => _storeController != null && _storeExtensionProvider != null;

        public static event EventHandler OnInitialized;

        private void Start()
        {
            if (_storeController == null) InitializePurchasing();
        }

        private void InitializePurchasing()
        {
            if (Initialized) return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (InAppProductTag product in typeof(InAppProductTag).GetEnumValues())
            {
                builder.AddProduct(product.ToString(), ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public static Product GetProduct(InAppProductTag productTag)
        {
            return _storeController?.products.WithID(productTag.ToString());
        }

        public static void BuyProduct(InAppProductTag productTag, BuyProductCallback callback)
        {
            if (!Initialized) return;
            var product = GetProduct(productTag);
            if (product is null || !product.availableToPurchase) return;
            _buyProductCallback = callback;
            _storeController.InitiatePurchase(product);
        }

        public void RestorePurchases()
        {
            if (Initialized) return;
            
            if (Application.platform != RuntimePlatform.IPhonePlayer
                && Application.platform != RuntimePlatform.OSXPlayer) return;
            
            var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions(result => {});
        }

        private static IEnumerator InvokeCallbackAsync(bool success)
        {
            yield return new WaitForSeconds(0.8f);
            _buyProductCallback?.Invoke(success);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            StartCoroutine(InvokeCallbackAsync(true));
            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
            OnInitialized?.Invoke(null, EventArgs.Empty);
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            StartCoroutine(InvokeCallbackAsync(false));
        }
    }

    public delegate void BuyProductCallback(bool success);
    
    public enum InAppProductTag
    {
        gold_coins_500,
        gold_coins_1500,
        gold_coins_4500,
        gold_coins_13500,
        gold_coins_40500
    }
}