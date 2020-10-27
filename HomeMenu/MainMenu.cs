using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class MainMenu : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Button playButton;
#pragma warning restore CS0649

        private void Start()
        {
            playButton.onClick.AddListener(ClickPlay);
            MusicMaster.Play(Music.MainMenu);
        }

        private static void ClickPlay()
        {
            switch (Game.Mode)
            {
                case GameMode.Rating:
                    SceneLoader.LoadScene(GameScene.Level, slowLoad:true);
                    break;
                case GameMode.Story:
                    SceneLoader.LoadScene(GameScene.Story);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
