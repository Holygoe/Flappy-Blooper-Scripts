using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class ResolvingGameData : MonoBehaviour
    {
        public Button cloudButton;
        public Button localButton;

        private void Awake()
        {
            cloudButton.onClick.AddListener(() => DataManager.ResolveGameData(true));
            localButton.onClick.AddListener(() => DataManager.ResolveGameData(false));
        }
    }
}