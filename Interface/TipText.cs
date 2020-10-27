using TinyLocalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class TipText : MonoBehaviour
    {
        private const int TipsCount = 11;
        private Localize _localize;

        private static string RandomKey => $"TIP_{Random.Range(0, TipsCount)}";

        private void Awake()
        {
            _localize = GetComponent<Localize>();
        }

        private void OnEnable()
        {
            _localize.QuickConnect(RandomKey);
        }
    }
}