using System;
using System.Collections;
using System.Collections.Generic;
using TinyLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Currency", menuName = "Game / Currency")]
    public class Currency : CountableItem, ITaggedName
    {
#pragma warning disable CS0649
        [SerializeField] private CurrencyTag tag;
#pragma warning restore CS0649
        
        private CurrencyData Data => CurrencyData.FindData(Game.GameData.currencyDataset, tag);
        protected override AssetTag AssetTag => AssetTag.Currencies;

        public override int Stock
        {
            get => Data.stock;
            protected set => Data.stock = value;
        }

        public Enum TaggedName => tag;
    }
    
    public static class CurrencyTagExtensions
    {
        public static Currency ToCurrency(this CurrencyTag currencyTag)
        {
            return AssetTag.Currencies.ToAsset().GetItem<Currency>(currencyTag);
        }
    }
    
    public enum CurrencyTag
    {
        Cookies,
        GoldCoins
    }
}