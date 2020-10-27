using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyLocalization;

namespace FlappyBlooper
{
    public class CharacterLevel : MonoBehaviour
    {
        private Localize level;

        private void Awake()
        {
            level = GetComponent<Localize>();
        }

        private void OnEnable()
        {
            UpdateLevel();
            Player.OnPlayerUpdated += Player_OnPlayerUpdated;
        }

        private void OnDisable()
        {
            Player.OnPlayerUpdated -= Player_OnPlayerUpdated;
        }

        private void Player_OnPlayerUpdated(object sender, System.EventArgs e)
        {
            UpdateLevel();
        }

        private void UpdateLevel()
        {
            level.SetStringParameter("value", Character.Current.Level.ToString());
        }
    }
}