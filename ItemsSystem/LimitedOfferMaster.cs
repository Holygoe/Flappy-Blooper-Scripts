using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "New OfferLine", menuName = "Game / Offer Line")]
    public class LimitedOfferMaster : ScriptableObject
    {
#pragma warning disable CS0649        
        [SerializeField] private OfferMasterTag tag;
        [SerializeField] private int lineLength;
        [SerializeField] private int updateRate;
        [SerializeField] private Product[] products;
#pragma warning restore CS0649

        private OfferMasterData Data => OfferMasterData.FindData(Game.GameData.offerMasterDataset, tag);
        public TimeSpan TimeLeftUntilUpdate => Data.UpdateTime - DateTime.Now;
        private bool IsOutdated => Data.UpdateTime < DateTime.Now;

        public IEnumerable<IOffer> GetOffers()
        {
            return Data.offerDataset.Select((offerData, offerIndex) =>
                new LimitedOffer(products[offerData.productIndex], this, offerIndex));
        }

        public LimitedOfferData GetOfferData(int offerIndex)
        {
            return Data.offerDataset[offerIndex];
        }

        public bool TryToUpdateOffers()
        {
            var data = Data;
            if (!IsOutdated) return false;

            data.UpdateTime = DateTime.Now.AddMinutes(updateRate);
            data.offerDataset = new LimitedOfferData[lineLength];
            var usedIndexes = new List<int>();
            var possibleOffersLength = products.Length;

            for (var i = 0; i < lineLength; i++)
            {
                var index = GetUniqueRandomIndex(possibleOffersLength, ref usedIndexes);
                
                data.offerDataset[i] = new LimitedOfferData
                {
                    productIndex = index,
                    stock = products[i].StockLimitedOffer
                };
            }

            DataManager.SaveData();
            return true;
        }

        private static int GetUniqueRandomIndex(int max, ref List<int> usedValues)
        {
            if (max <= usedValues.Count) return Random.Range(0, max);
            usedValues.Sort();
            var value = Random.Range(0, max - usedValues.Count);

            foreach (var unused in usedValues.Where(index => value >= index))
            {
                value++;
            }

            usedValues.Add(value);
            return value;
        }

        public static bool IsOfferLineOutdated(OfferMasterTag tag)
        {
            var data = OfferMasterData.FindData(Game.GameData.offerMasterDataset, tag);
            return data.UpdateTime < DateTime.Now;
        }
    }

    public enum OfferMasterTag
    {
        Store,
        Level
    }
}