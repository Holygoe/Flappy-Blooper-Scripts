using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace FlappyBlooper
{
    [Serializable]
    public class GameData
    {
        public const string DataVersionReference = "v.5";
        public List<AccessoryData> accessoryDataset = new List<AccessoryData>();
        public List<AchievementData> achievementDataset = new List<AchievementData>();
        public List<string> achievementsOrder = new List<string>();
        public List<ChapterData> chapterDataset = new List<ChapterData>();
        public List<CharacterData> characterDataset = new List<CharacterData>();
        public List<ConsumableData> consumableDataset = new List<ConsumableData>();
        public List<CurrencyData> currencyDataset = new List<CurrencyData>();
        public string dataVersion;
        public int daysInTheGameCount;
        public int gameProgress;
        public int greenGooCollectedTodayCount;
        public int highscore;
        [SerializeField] private string lastOpenTheGameDate;
        public List<NoticeData> noticeDataset = new List<NoticeData>();
        public List<OfferMasterData> offerMasterDataset = new List<OfferMasterData>();
        [SerializeField] private string saveDate;
        public string selectedCharacter;

        public GameData()
        {
            selectedCharacter = CharacterTag.Blooper.ToString();
            ConsumableData.FindData(consumableDataset, ConsumableTag.HealthPotions).stock = 10;
            ConsumableData.FindData(consumableDataset, ConsumableTag.GreenGoo).stock = 3;
            NormalizeData();
        }

        public DateTime SaveDate
        {
            get => DateTime.TryParse(saveDate, out var value) ? value : DateTime.MinValue;
            set => saveDate = value.ToString(CultureInfo.CurrentCulture);
        }

        public DateTime LastOpenTheGameDate
        {
            get => DateTime.TryParse(lastOpenTheGameDate, out var value) ? value : DateTime.MinValue;
            set => lastOpenTheGameDate = value.ToString(CultureInfo.CurrentCulture);
        }

        public void NormalizeData()
        {
            CurrencyData.NormalizeDataset(currencyDataset);
            CharacterData.NormalizeDataset(characterDataset);
            ConsumableData.NormalizeDataset(consumableDataset);
            AchievementData.NormalizeDataset(achievementDataset);
            OfferMasterData.NormalizeDataset(offerMasterDataset);
            AccessoryData.NormalizeDataset(accessoryDataset);
            NoticeData.NormalizeDataset(noticeDataset);
            ChapterData.NormalizeDataset(chapterDataset);

            CharacterData.FindData(characterDataset, CharacterTag.Blooper).purchased = true;

            if (Enum.TryParse(selectedCharacter, out CharacterTag selectedCharacterTag))
            {
                var selectedCharacterData = CharacterData.FindData(characterDataset, selectedCharacterTag);
                if (!selectedCharacterData.purchased) selectedCharacter = CharacterTag.Blooper.ToString();
            }

            // Delete outdated chapter information.
            if (!Assets.Instance) return;
            foreach (var data in chapterDataset)
            {
                if (!data.TryGetTag(out var tag)) continue;

                var chapter = tag.ToChapter();
                var stagesCount = chapter.StagesCount;
                if (data.stages.Count > stagesCount)
                    data.stages.RemoveRange(stagesCount, data.stages.Count - stagesCount);
            }
        }
    }

    [Serializable]
    public class ConsumableData : TaggedData<ConsumableTag, ConsumableData>
    {
        public int stock;
    }

    [Serializable]
    public class AchievementData : TaggedData<AchievementTag, AchievementData>
    {
        public int claimed;
        public int progress;
    }

    [Serializable]
    public class OfferMasterData : TaggedData<OfferMasterTag, OfferMasterData>
    {
        public LimitedOfferData[] offerDataset;
        [SerializeField] private string updateTime;

        public DateTime UpdateTime
        {
            get => DateTime.TryParse(updateTime, out var value) ? value : DateTime.MinValue;
            set => updateTime = value.ToString(CultureInfo.CurrentCulture);
        }
    }

    [Serializable]
    public class LimitedOfferData
    {
        public int productIndex;
        public int stock;
    }

    [Serializable]
    public class AccessoryData : TaggedData<AccessoryTag, AccessoryData>
    {
        public bool purchased;
    }

    [Serializable]
    public class NoticeData : TaggedData<NoticeTag, NoticeData>
    {
        public bool topical;
        public bool viewed;
    }

    [Serializable]
    public class CurrencyData : TaggedData<CurrencyTag, CurrencyData>
    {
        public int stock;
    }

    [Serializable]
    public class CharacterData : TaggedData<CharacterTag, CharacterData>
    {
        public int evolution;
        public int level = 1;
        public bool purchased;
        public List<string> usedAccessories = new List<string>();
    }

    [Serializable]
    public class ChapterData : TaggedData<ChapterTag, ChapterData>
    {
        public List<StageData> stages = new List<StageData>();
    }

    [Serializable]
    public class StageData
    {
        public int progress;
    }
}