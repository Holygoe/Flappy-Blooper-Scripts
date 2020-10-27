using TinyLocalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class GameOverPopup : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Image playerImage;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private Localize highscore;
        [SerializeField] private Button retryButton;
#pragma warning restore CS0649

        private void Awake()
        {
            retryButton.onClick.AddListener(() => SceneLoader.LoadScene(GameScene.Level));
        }

        public void Open(bool highscoreAchieved)
        {
            playerImage.sprite = Character.Current.Icon;

            if (highscoreAchieved)
            {
                highscore.QuickConnect(LocalizeKey.NEW_HIGHSCORE);
            }
            else if (Game.Mode == GameMode.Story)
            {
                highscore.QuickConnect(LocalizeKey.STORY_MODE);
            }
            else
            {
                highscore.QuickConnect(LocalizeKey.HIGHSCORE);
                highscore.SetStringParameter("value", Game.Highscore.ToString());
            }

            score.text = Level.Score.ToString();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}