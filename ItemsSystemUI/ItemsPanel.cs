using System;
using UnityEngine;

namespace FlappyBlooper
{
    public class ItemsPanel : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private AssetTag assetTag;
#pragma warning restore CS0649

        private void OnEnable()
        {
            UpdatePanel();
            assetTag.ToAsset().OnItemChanged += AssetTagItemChanged;
        }

        private void OnDisable()
        {
            assetTag.ToAsset().OnItemChanged -= AssetTagItemChanged;
        }

        private void AssetTagItemChanged(object sender, EventArgs e)
        {
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            transform.Clear();
            var items = assetTag.ToAsset().GetItems<Item>();

            foreach(var item in items)
            {
                if (item.Available)
                {
                    var card = Instantiate(Assets.ItemCard, transform);
                    card.Initialize(item);
                }
                else if (item is UniqueItem uniqueItem)
                {
                    var product = uniqueItem.Product;
                    if (product is null) continue;
                    var card = Instantiate(product.GetCardPrefab(), transform);
                    card.Initialize(product);
                }
            }
        }
    }
}