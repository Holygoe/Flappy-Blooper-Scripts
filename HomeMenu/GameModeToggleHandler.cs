using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class GameModeToggleHandler : MonoBehaviour
    {
        public Localize captionText;
        public Button ratingButton;
        public Button storyButton;

        private GameMode gameMode;

        private void Start()
        {
            gameMode = Game.Mode;

            ratingButton.onClick.AddListener(() => Game.Mode = GameMode.Story);
            storyButton.onClick.AddListener(() => Game.Mode = GameMode.Rating);

            UpdateToggle();
        }

        private void Update()
        {
            if (gameMode != Game.Mode)
            {
                gameMode = Game.Mode;
                UpdateToggle();
            }
        }

        private void UpdateToggle()
        {
            switch (gameMode)
            {
                case GameMode.Rating:
                    captionText.QuickConnect(LocalizeKey.RATING_MODE);
                    break;
                case GameMode.Story:
                    captionText.QuickConnect(LocalizeKey.STORY_MODE);
                    break;
            }

            ratingButton.gameObject.SetActive(Game.Mode == GameMode.Rating);
            storyButton.gameObject.SetActive(Game.Mode == GameMode.Story);
        }
    }
}