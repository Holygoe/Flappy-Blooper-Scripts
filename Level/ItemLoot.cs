using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    public class ItemLoot : Loot
    {
        [FormerlySerializedAs("entity")] public CountableItem countableItem;
        public int number;

        protected override void Apply()
        {
            if (countableItem is Consumable consumable && (ConsumableTag)consumable.TaggedName == ConsumableTag.GreenGoo)
            {
                Game.GameData.greenGooCollectedTodayCount++;
            }
            
            countableItem.Add(number, true, true);
        }

        protected override int GetEmitParticlesNumber()
        {
            return number;
        }
    }
}