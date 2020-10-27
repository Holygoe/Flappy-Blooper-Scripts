using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyLocalization;
using TMPro;

namespace FlappyBlooper
{
    public class TotalLevelText : MonoBehaviour
    {
        private Localize _text;

        private void Awake()
        {
            _text = GetComponent<Localize>();
        }

        private void OnEnable()
        {
            UpdateText();
            Player.OnPlayerUpdated += OnTotalLevelUpdated;
            AssetTag.Characters.ToAsset().OnItemChanged += OnTotalLevelUpdated;
        }

        private void OnDisable()
        {
            Player.OnPlayerUpdated -= OnTotalLevelUpdated;
            AssetTag.Characters.ToAsset().OnItemChanged -= OnTotalLevelUpdated;
        }

        private void OnTotalLevelUpdated(object sender, System.EventArgs e)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _text.SetStringParameter("value", Character.TotalLevel.ToString());
        }
    }
}