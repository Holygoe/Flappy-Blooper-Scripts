using System;
using System.Linq;
using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Accessory", menuName = "Game / Accessory")]
    public class Accessory : UniqueItem, ITaggedName, IDeselectableItem
    {
#pragma warning disable CS0649
        [SerializeField] private AccessoryTag tag;
        [SerializeField] private AccessoryCategory category;
#pragma warning restore CS0649

        public Enum TaggedName => tag;
        public AccessoryCategory Category => category;
        private AccessoryData Data => AccessoryData.FindData(Game.GameData.accessoryDataset, tag);
        protected override float IconScale => 1;
        public override bool Purchased => Data.purchased;
        public override bool Selected => Character.Current.SelectedAccessories.Any(accessory => accessory == tag.ToString());
        protected override AssetTag AssetTag => AssetTag.Accessories;

        protected override void Obtain()
        {
            var data = Data;
            if (data.purchased) return;
            data.purchased = true;
            AssetTag.Accessories.ToAsset().ItemWasChanged(this);
        }

        public static bool TryToGetSelectedItem(AccessoryCategory category, out Accessory accessory)
        {
            foreach(var tagName in Character.Current.SelectedAccessories)
            {
                if (!Enum.TryParse(tagName, out AccessoryTag accessoryTag)) continue;
                accessory = accessoryTag.ToAccessory();
                if (accessory.category == category)
                {
                    return true;
                }
            }

            accessory = null;
            return false;
        }

        protected override void Select()
        {
            var data = Data;

            if (!data.purchased)
            {
                return;
            }
            
            if (TryToGetSelectedItem(category, out var selectedAccessory))
            {
                selectedAccessory.Deselect(false);
            }

            Character.Current.SelectedAccessories.Add(tag.ToString());
            Player.UpdatePlayer();
            AssetTag.Accessories.ToAsset().ItemWasChanged(this);
        }

        public void Deselect(bool saveGame = true)
        {
            if (Selected)
            {
                Character.Current.SelectedAccessories.Remove(tag.ToString());
            }
            
            Player.UpdatePlayer();
            AssetTag.Accessories.ToAsset().ItemWasChanged(this);

            if (saveGame) DataManager.SaveData();
        }
    }

    public enum AccessoryCategory
    {
        Mask
    }

    public static class AccessoryTagExtensions
    {
        public static Accessory ToAccessory(this AccessoryTag tag)
        {
            return AssetTag.Accessories.ToAsset().GetItem<Accessory>(tag);
        }
    }
    
    public enum AccessoryTag
    {
        PirateEyePatch,
        HockeyMask, 
        DivingMask,
        RitualMask,
        RogueMask
    }
}