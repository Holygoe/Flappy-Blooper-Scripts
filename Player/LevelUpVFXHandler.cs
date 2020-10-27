using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class LevelUpVFXHandler : MonoBehaviour
    {
        public GameObject vfxPrefab;
        public SoundHandler soundHandler;

        private void OnEnable()
        {
            Character.OnGotLevelUp += Character_OnLevelUp;
        }

        private void OnDisable()
        {
            Character.OnGotLevelUp -= Character_OnLevelUp;
        }

        private void Character_OnLevelUp(object sender, System.EventArgs e)
        {
            Instantiate(vfxPrefab, transform);
            soundHandler.PlayOneShot();
        }
    }
}