using System;
using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName ="New Chapter", menuName ="Game / Chapter")]
    public class Chapter : ScriptableObject, ITaggedName
    {
#pragma warning disable CS0649
        [SerializeField] private ChapterTag tag;
        [SerializeField] private string nameKey;
        [SerializeField] private Stage[] stages;
#pragma warning restore CS0649

        public string NameKey => nameKey;
        public Enum TaggedName => tag;
        public int StagesCount => stages.Length;
        private ChapterData Data => ChapterData.FindData(Game.GameData.chapterDataset, tag);

        public Stage GetStage(int index)
        {
            return stages[index];
        }
        
        public StageInfo GetStageInfo(int index)
        {
            var info = new StageInfo
            {
                Number = (index + 1).ToString()
            };

            var data = Data;

            if (index == 0) info.Unlocked = true;
            else if (index > data.stages.Count || index >= StagesCount) info.Unlocked = false;
            else info.Unlocked = data.stages[index - 1].progress > 0;

            if (index == 0 && data.stages.Count < 1) data.stages.Add(new StageData());
            if (!info.Unlocked) return info;
            if (index >= data.stages.Count) data.stages.Add(new StageData());

            info.StageData = data.stages[index];
            info.Rewards = stages[index].Rewards;

            return info;
        }
    }

    public static class ChapterTagExtensions
    {
        public static Chapter ToChapter(this ChapterTag tag)
        {
            return AssetTag.Chapters.ToAsset().GetItem<Chapter>(tag);
        }
    }

    [Serializable]
    public class Stage
    {
        public const int MaxDifficulty = 9;
        
#pragma warning disable CS0649
        [SerializeField] private int length;
        [SerializeField] private int difficulty;
        [SerializeField] private ItemTarget[] rewards;
#pragma warning restore CS0649
        
        public int Length => length;
        public int Difficulty => difficulty;
        public ItemTarget[] Rewards => rewards;
    }
    
    public enum ChapterTag { DazeMountain }
}