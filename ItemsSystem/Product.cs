using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    public abstract class Product : ScriptableObject, IOffer
    {
#pragma warning disable CS0649
        [FormerlySerializedAs("itemContainer")]
        [FormerlySerializedAs("entityTarget")]
        [SerializeField] private ItemTarget itemTarget;
        [SerializeField] private bool limitedOffer;
        [FormerlySerializedAs("offersAvailability")] [SerializeField] private int stockLimitedOffer;
        [SerializeField] private int discount;
#pragma warning restore CS0649

        public int StockLimitedOffer => stockLimitedOffer;
        public int Discount => discount;
        ItemTarget IOffer.ItemContainer => itemTarget;
        protected abstract ProductCard CardPrefab { get; }
        Product IOffer.Product => this;
        bool IOffer.AvailableInStock => itemTarget.AvailableForPurchase;

        void IOffer.Purchase(PurchaseOfferCallback callback)
        {
            Purchase(callback);
        }

        public ProductCard GetCardPrefab()
        {
            return CardPrefab;
        }
        
        protected void CollectProduct(PurchaseOfferCallback callback)
        {
            itemTarget.Collect(false);
            CollectItemPopup.Open(itemTarget, callback);
        }
        
        protected abstract void Purchase(PurchaseOfferCallback callback);
    }
}