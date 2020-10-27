using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class PurchaseForCurrencyButtonLayout : PurchaseButtonLayout
    {
#pragma warning disable CS0649
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private Image priceIcon;
        [HideInInspector] [SerializeField] private ProductForCurrency product;
#pragma warning restore CS0649
        
        public override Product Product
        {
            set
            {
                if (value is ProductForCurrency productForCurrency)
                {
                    product = productForCurrency;
                }
                else
                {
                    throw new ArgumentException($"{nameof(Product)} must be {typeof(ProductForCurrency)}");
                }
            }
        }

        public override void UpdateButton()
        {
            price.text = product.Price;
            price.color = product.HaveSufficientFunds ? Color.white : Color.red;
            priceIcon.sprite = product.Currency.Icon;
        }
    }
}