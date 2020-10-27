using System;
using System.Globalization;
using UnityEngine;

namespace FlappyBlooper
{
    public abstract class StoredValueGenesis
    {

    }

    public abstract class StoredValue<T> : StoredValueGenesis
    {
        protected T StoringValue;
        protected readonly string Key;

        public T Value
        {
            get => StoringValue;

            set
            {
                StoringValue = value;
                PlayerPrefsSet(value);
            }
        }

        protected StoredValue(string key, T defaultValue)
        {
            this.Key = key;
            PlayerPrefsGet(defaultValue);
        }

        protected abstract void PlayerPrefsGet(T defaultValue);
        protected abstract void PlayerPrefsSet(T value);
    }

    public class StoredInt : StoredValue<int>
    {
        public StoredInt(string key, int defaultValue) : base(key, defaultValue) { }

        protected override void PlayerPrefsGet(int defaultValue)
        {
            StoringValue = PlayerPrefs.GetInt(Key, defaultValue);
        }

        protected override void PlayerPrefsSet(int value)
        {
            PlayerPrefs.SetInt(Key, value);
        }
    }

    public class StoredBool : StoredValue<bool>
    {
        public StoredBool(string key, bool defaultValue) : base(key, defaultValue) { }

        protected override void PlayerPrefsGet(bool defaultValue)
        {
            StoringValue = PlayerPrefs.GetInt(Key, defaultValue ? 1 : 0) == 1;
        }

        protected override void PlayerPrefsSet(bool value)
        {
            PlayerPrefs.SetInt(Key, value ? 1 : 0);
        }
    }

    public class StoresDateTime : StoredValue<DateTime>
    {
        public StoresDateTime(string key, DateTime defaultValue) : base(key, defaultValue) { }

        protected override void PlayerPrefsGet(DateTime defaultValue)
        {
            StoringValue = DateTime.Parse(PlayerPrefs.GetString(Key, defaultValue.ToString(CultureInfo.CurrentCulture)));
        }

        protected override void PlayerPrefsSet(DateTime value)
        {
            PlayerPrefs.SetString(Key, value.ToString(CultureInfo.CurrentCulture));
        }
    }

    public class StoredChapterTag : StoredValue<ChapterTag>
    {
        public StoredChapterTag(string key, ChapterTag defaultValue) : base(key, defaultValue) { }

        protected override void PlayerPrefsGet(ChapterTag defaultValue)
        {
            StoringValue = Enum.TryParse(PlayerPrefs.GetString(Key, defaultValue.ToString()), out ChapterTag tag)
                ? tag : ChapterTag.DazeMountain;
        }

        protected override void PlayerPrefsSet(ChapterTag value)
        {
            PlayerPrefs.SetString(Key, value.ToString());
        }
    }
}