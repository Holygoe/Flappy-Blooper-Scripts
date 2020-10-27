using UnityEngine;
using RudeBlooper.InAppPurchase;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Product", menuName = "Product/For Money")]
    public class ProductForMoney : Product
    {
#pragma warning disable CS0649
        [FormerlySerializedAs("productTag")] public InAppProductTag inAppProductTag;
#pragma warning restore CS0649

        public InAppProductTag InAppProductTag => inAppProductTag;

        protected override ProductCard CardPrefab => Assets.ProductForMoneyCard;

        protected override void Purchase(PurchaseOfferCallback callback)
        {
            InAppPurchaseManager.BuyProduct(inAppProductTag, success =>
            {
                if (success)
                {
                    CollectProduct(callback);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }
    }
}