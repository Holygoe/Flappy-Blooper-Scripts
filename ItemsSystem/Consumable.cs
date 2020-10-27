using System;
using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Game / Consumable")]
    public class Consumable : CountableItem, ITaggedName
    {
#pragma warning disable CS0649
        [SerializeField] private ConsumableTag tag;
#pragma warning restore CS0649
        
        public Enum TaggedName => tag;
        private ConsumableData Data => ConsumableData.FindData(Game.GameData.consumableDataset,tag);
        protected override AssetTag AssetTag => AssetTag.Consumables;

        public override int Stock
        {
            get => Data.stock;
            protected set => Data.stock = value;
        }

        public static Consumable GetConsumable(ConsumableTag consumableTag)
        {
            return AssetTag.Consumables.ToAsset().GetItem<Consumable>(consumableTag);
        }
    }

    public static class ConsumableTagExtensions
    {
        public static Consumable ToConsumable(this ConsumableTag consumableTag)
        {
            return Consumable.GetConsumable(consumableTag);
        }
    }

    public enum ConsumableTag
    {
        HealthPotions,
        GreenGoo,
        MagicCrystals
    }
}
