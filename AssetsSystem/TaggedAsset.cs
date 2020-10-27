using System;
using System.Collections.Generic;
using System.Linq;
using FlappyBlooper;
using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "TaggedAsset", menuName = "Game / Tagged Asset")]
    public class TaggedAsset : ScriptableObject, ITaggedName
    {
#pragma warning disable CS0649
        [SerializeField] private AssetTag tag;
        [SerializeField] private ScriptableObject[] asset;
#pragma warning restore CS0649
        
        private Dictionary<Enum, ScriptableObject> _dictionary;

        public Enum TaggedName => tag;
        public int Length => asset.Length;

        public event EventHandler<ItemChangedEventArgs> OnItemChanged;

        public void Initialize()
        {
            _dictionary = new Dictionary<Enum, ScriptableObject>();

            foreach(var item in asset)
            {
                if (item is ITaggedName taggedItem)
                {
                    _dictionary.Add(taggedItem.TaggedName, item);
                }
            }
        }

        public T[] GetItems<T>() where T : ScriptableObject
        {
            return asset.Cast<T>().ToArray();
        }

        public T GetItem<T>(Enum itemTag) where T : ScriptableObject
        {
            return _dictionary[itemTag] as T;
        }

        public void ItemWasChanged(ScriptableObject item)
        {
            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(item));
        }
    }
}

public class ItemChangedEventArgs : EventArgs
{
    public ItemChangedEventArgs(ScriptableObject item)
    {
        Item = item;
    }

    public ScriptableObject Item { get; }
}

public static class AssetTagExtensions
{
    public static TaggedAsset ToAsset(this AssetTag tag)
    {
        return Assets.GetAsset(tag);
    }
}

public enum AssetTag
{
    None,
    Characters,
    Accessories,
    Consumables,
    Currencies,
    Achievements,
    Chapters
}