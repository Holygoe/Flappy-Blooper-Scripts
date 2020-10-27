using System;
using System.Collections.Generic;
using System.Linq;
using TinyLocalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Character", menuName = "Game / Character")]
    public class Character : UniqueItem, ITaggedName
    {
        public const int MaxEvolvesCount = 5;
#pragma warning disable CS0649
        [SerializeField] private CharacterTag tag;
        [SerializeField] private Achievement highscoreAchievement;
        [SerializeField] private Achievement levelAchievement;
        [SerializeField] private BodyPart[] bodyParts;
#pragma warning restore CS0649
        
        public Achievement HighscoreAchievement => highscoreAchievement;
        //public Achievement LevelAchievement => levelAchievement;
        public IEnumerable<BodyPart> BodyParts => bodyParts;
        public Enum TaggedName => tag;
        public static Consumable ItemsToEvolve => ConsumableTag.MagicCrystals.ToConsumable();
        private CharacterData Data => CharacterData.FindData(Game.GameData.characterDataset, tag);
        public int Level => Data.level;
        public int Evolution => Data.evolution;
        public int EvolutionCost => 1 + GetEvolutionCost(Level - 1);
        //public bool AbleToPayForBoosting => EvolutionCost <= ConsumableTag.MagicCrystals.ToConsumable().Stock;
        protected override float IconScale => 2;
        public override bool Selected => tag.ToString() == Game.GameData.selectedCharacter;
        public override bool Purchased => Data.purchased;
        public List<string> SelectedAccessories => Data.usedAccessories;
        public static int TotalLevel => AssetTag.Characters.ToAsset().GetItems<Character>()
            .Where(character => character.Purchased).Sum(character => character.Level);

        protected override AssetTag AssetTag => AssetTag.Characters;

        public int LifeNumber
        {
            get
            {
                if (Level < 8)
                {
                    return Level / 2 + 1;
                }

                return 5;
            }
        }

        public int HealthPotionNumber
        {
            get
            {
                if (Level < 9)
                {
                    return (Level - 1) / 2 + 1;
                }

                return 5;
            }
        }

        public static Character Current
        {
            get
            {
                if (Enum.TryParse(Game.GameData.selectedCharacter, out CharacterTag tag)) return tag.ToCharacter();
                
                var character = CharacterTag.Blooper.ToCharacter();
                character.Select(true);
                return character;
            }
        }

        public static event EventHandler OnGotLevelUp;

        protected override void Select()
        {
            var characterData = Data;
            if (!characterData.purchased) return;
            Game.GameData.selectedCharacter = tag.ToString();
            Player.UpdatePlayer(true);
            AssetTag.Characters.ToAsset().ItemWasChanged(this);
        }

        protected override void Obtain()
        {
            var data = Data;
            if (data.purchased) return;
            data.purchased = true;
            AssetTag.Characters.ToAsset().ItemWasChanged(this);
        }

        public bool TryToEvolve(bool saveGame = true)
        {
            if (!ConsumableTag.MagicCrystals.ToConsumable().TryToRemove(EvolutionCost, false)) return false;
            Data.evolution++;
            
            if (Data.evolution > MaxEvolvesCount)
            {
                Data.evolution = 0;
                Data.level++;
                levelAchievement.UpdateProgress(Data.level, saveGame);
                Player.UpdatePlayer();
                OnGotLevelUp?.Invoke(this, EventArgs.Empty);
            }

            Player.UpdatePlayer();
            if (saveGame) DataManager.SaveData();
            return true;
        }

        private static int GetEvolutionCost(int level)
        {
            if (level > 0)
            {
                return level + GetEvolutionCost(level - 1);
            }

            return 0;
        }
    }

    [Serializable]
    public struct BodyPart
    {
#pragma warning disable CS0649
        [SerializeField] private BodyPartTag tag;
        [SerializeField] private Sprite sprite;
#pragma warning restore CS0649

        public BodyPartTag Tag => tag;
        public Sprite Sprite => sprite;
    }
    
    public enum BodyPartTag
    {
        Body,
        Head,
        Face,
        RightEar,
        LeftEar,
        RightWing,
        LeftWing,
        Tail,
        RightFrontPaw,
        LeftFrontPaw,
        RightHindPaw,
        LeftHindPaw,
        Feature,
        Soul
    }

    public static class CharacterTagExtensions
    {
        public static Character ToCharacter(this CharacterTag characterTag)
        {
            return AssetTag.Characters.ToAsset().GetItem<Character>(characterTag);
        }
    }
    
    public enum CharacterTag
    {
        Blooper,
        Jeff,
        Loona,
        ZombieAl,
        TigerJoey,
        TigerChandler
    }
}