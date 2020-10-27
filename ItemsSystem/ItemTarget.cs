using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace FlappyBlooper
{
    [Serializable]
    public class ItemTarget
    {
#pragma warning disable CS0649
        [SerializeField] private Item item;
        [FormerlySerializedAs("number")] [SerializeField] private int count;
#pragma warning disable CS0649

        public string Count => count.ToString();
        
        public ItemTarget(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public bool AvailableForPurchase
        {
            get
            {
                if (item is UniqueItem uniqueItem)
                {
                    return !uniqueItem.Purchased;
                }
                
                return true;
            }
        }
        
        public Item Item => item;

        public string Title
        {
            get
            {
                var name = item.Name;
                if (item is CountableItem)
                {
                    name += $" ({count})";
                }
                return name;
            }
        }

        public void Collect(bool addProgress, bool saveGame = true)
        {
            switch (item)
            {
                case UniqueItem uniqueItem:
                    uniqueItem.Obtain(saveGame);
                    break;
                case CountableItem countableItem:
                    countableItem.Add(count, addProgress, saveGame);
                    break;
            }
        }
    }
}