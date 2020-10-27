using System;
using UnityEngine;

namespace FlappyBlooper
{
    public abstract class CountableItem : Item
    {
#pragma warning disable CS0649
        [SerializeField] private int progressMultiplier;
#pragma warning restore CS0649

        protected override float IconScale => 1;
        public override bool Available => Stock > 0;
        public abstract int Stock { get; protected set; }

        public event EventHandler OnStockChanged;

        public void Add(int value, bool addProgress, bool saveGame)
        {
            Stock += value;

            if (addProgress) Game.GameData.gameProgress += value * progressMultiplier;

            if (saveGame) DataManager.SaveData();
            AssetTag.ToAsset().ItemWasChanged(this);
            OnStockChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool TryToRemove(int value, bool saveGame)
        {
            if (Stock < value) return false;
            Stock -= value;
            if (saveGame) DataManager.SaveData();
            AssetTag.ToAsset().ItemWasChanged(this);
            OnStockChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}