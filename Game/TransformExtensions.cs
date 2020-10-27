using UnityEngine;

namespace FlappyBlooper
{
    public static class TransformExtensions
    {
        public static void Clear(this Transform transform)
        {
            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}