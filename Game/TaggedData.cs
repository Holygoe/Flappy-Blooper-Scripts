using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlappyBlooper
{
    [Serializable]
    public abstract class TaggedData<TTag, TData> where TTag : struct, Enum where TData : TaggedData<TTag, TData>, new()
    {
        [SerializeField] private string tagString;

        public bool TryGetTag(out TTag tag)
        {
            return Enum.TryParse(tagString, out tag);
        }

        public static void NormalizeDataset(List<TData> dataset)
        {
            for (var i = 0; i < dataset.Count; i++)
            {
                if (!int.TryParse(dataset[i].tagString, out _) && dataset[i].TryGetTag(out _)) continue;
                dataset.Remove(dataset[i]);
                i--;
            }
        }

        public static TData FindData(List<TData> dataset, TTag tag)
        {
            var tagString = tag.ToString();
            
            foreach (var data in dataset.Where(data => tagString == data.tagString))
            {
                return data;
            }

            var newData = new TData
            {
                tagString = tag.ToString()
            };
            
            dataset.Add(newData);
            return newData;
        }
    }
}