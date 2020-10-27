using System;
using UnityEngine;

namespace FlappyBlooper
{
    public class ProductLine : MonoBehaviour
    {
        private Transform _cardsTransform;

#pragma warning disable CS0649
        [SerializeField] private Product[] products;
#pragma warning restore CS0649

        private void Awake()
        {
            _cardsTransform = transform.Find("Cards");
            if (_cardsTransform is null)
                throw new Exception("Cards transform not found.");

            foreach (var product in products)
            {
                if (product is ProductForWatchingAd productForWatchingAd)
                    productForWatchingAd.AdTag = VideoAdTag.Store;
                var card = Instantiate(product.GetCardPrefab(), _cardsTransform);
                card.Initialize(product);
            }
        }
    }
}