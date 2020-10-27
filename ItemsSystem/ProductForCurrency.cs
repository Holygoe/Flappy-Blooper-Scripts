using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Product", menuName = "Game / For Currency")]
    public class ProductForCurrency : Product
    {
#pragma warning disable CS0649
        [FormerlySerializedAs("currency")] [SerializeField]
        private CurrencyTag currencyTag;
        [SerializeField] private int price;
#pragma warning restore CS0649

        public Currency Currency => currencyTag.ToCurrency();
        public string Price => price.ToString();
        public bool HaveSufficientFunds => Currency.Stock >= price;

        protected override ProductCard CardPrefab => Assets.ProductForCurrencyCard;

        protected override void Purchase(PurchaseOfferCallback callback)
        {
            var success = Currency.TryToRemove(price, false);

            if (success)
            {
                CollectProduct(callback);
            }
            else
            {
                InsufficientFundsPopup.Open(Currency);
                callback?.Invoke(false);
            }
        }
    }
}