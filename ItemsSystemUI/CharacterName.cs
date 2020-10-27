using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyLocalization;

namespace FlappyBlooper
{
    public class CharacterName : MonoBehaviour
    {
        private Localize _nameLocalize;

        private void Awake()
        {
            _nameLocalize = GetComponent<Localize>();
        }

        private void OnEnable()
        {
            UpdateName();
            Player.OnPlayerUpdated += Player_OnPlayerUpdated;
        }

        private void OnDisable()
        {
            Player.OnPlayerUpdated -= Player_OnPlayerUpdated;
        }

        private void Player_OnPlayerUpdated(object sender, System.EventArgs e)
        {
            UpdateName();
        }

        private void UpdateName()
        {
            _nameLocalize.QuickConnect(Character.Current.NameKey);
        }
    }
}