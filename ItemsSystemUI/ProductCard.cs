using System;
using TinyLocalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class ProductCard : Card
    {
#pragma warning disable CS0649
        [SerializeField] private RudeButton purchaseButton;
        [SerializeField] private Transform purchasedLabel;
        [SerializeField] private Transform discountLabel;
        [SerializeField] private Localize discountText;
        [SerializeField] private Localize limitedOfferStock;
#pragma warning restore CS0649

        private IOffer _offer;
        private PurchaseButtonLayout _purchaseButtonLayout;
        protected override Item Item => _offer?.ItemContainer.Item;

        private event EventHandler OnOfferWasPurchased;
        
        private void Awake()
        {
            _purchaseButtonLayout = purchaseButton.GetComponent<PurchaseButtonLayout>();
            if (_offer is null) gameObject.SetActive(false);
        }

        public void Initialize(IOffer offer)
        {
            _offer = offer;
            purchaseButton.onClick.AddListener(() => _offer.Purchase((success) =>
            {
                if (success)
                    OnOfferWasPurchased?.Invoke(this, EventArgs.Empty);
            }));
            _purchaseButtonLayout.Product = _offer.Product;
            gameObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnOfferWasPurchased += ThisOfferWasPurchased;
        }

        private void OnDisable()
        {
            OnOfferWasPurchased -= ThisOfferWasPurchased;
        }

        private void ThisOfferWasPurchased(object sender, EventArgs e)
        {
            UpdateCard();
            Woop();
        }

        protected override void UpdateCard()
        {
            base.UpdateCard();
            title.text = _offer.ItemContainer.Title;
            var availableInStock = _offer.AvailableInStock;
            purchaseButton.gameObject.SetActive(availableInStock);
            purchasedLabel.gameObject.SetActive(!availableInStock);
            var discount = _offer.Product.Discount; 
            discountLabel.gameObject.SetActive(discount > 0);
            discountText.SetStringParameter("value", discount.ToString());
            _purchaseButtonLayout.UpdateButton();

            if (_offer is LimitedOffer limitedOffer && limitedOffer.Stock > 0)
            {
                limitedOfferStock.gameObject.SetActive(true);
                limitedOfferStock.SetStringParameter("value", limitedOffer.Stock.ToString());
            }
            else
            {
                limitedOfferStock.gameObject.SetActive(false);
            }
        }
    }
}