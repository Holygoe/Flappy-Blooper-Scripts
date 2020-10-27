using System;
using System.Collections.Generic;

namespace FlappyBlooper
{
    public class TagValuePair<TEnum, TValue, TPair>
        where TEnum : Enum
        where TPair : TagValuePair<TEnum, TValue, TPair>, new()
    {
        public TEnum tag;
        public TValue value;

        public void CompletePair(TEnum tag)
        {
            this.tag = tag;
            this.value = default;
        }

        public static List<TPair> BuildList()
        {
            List<TPair> list = new List<TPair>();

            foreach (TEnum tag in Enum.GetValues(typeof(TEnum)))
            {
                TPair pair = new TPair();
                pair.CompletePair(tag);
                list.Add(pair);
            }

            return list;
        }

        public static TPair Find(List<TPair> list, TEnum tag)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (Equals(list[i].tag, tag))
                {
                    return list[i];
                }
            }

            TPair pair = new TPair();
            pair.CompletePair(tag);
            list.Add(pair);
            return pair;
        }
    }
}
