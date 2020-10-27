using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class ContinuePopup : MonoBehaviour
    {
        private const float BUTTON_ACTIVATION_DELAY = 1;

        public Countdown countdown;
        public Button continueButton;
        public Localize continueText;
        public Button finishButton;
        public Image iconButton;

        private bool useGreenGoo;

        private bool NoHealthPotions => Player.HealthPotionsNumber <= 0 || ConsumableTag.HealthPotions.ToConsumable().Stock <= 0;

        private void Awake()
        {
            continueButton.onClick.AddListener(ClickContinueButton);
            finishButton.onClick.AddListener(ClickFinishButton);
            countdown.onWasOver.AddListener(ClickFinishButton);
        }

        private void OnEnable()
        {
            Level.State = LevelState.Paused;

            continueButton.interactable = false;
            finishButton.interactable = false;

            StartCoroutine(ActivateButtonAsync());

            if (ConsumableTag.GreenGoo.ToConsumable().Stock <= 0 && NoHealthPotions)
            {
                ClickFinishButton();
            }
            else
            {
                if (NoHealthPotions)
                {
                    useGreenGoo = true;
                }

                var consumable = useGreenGoo
                    ? ConsumableTag.GreenGoo.ToConsumable()
                    : ConsumableTag.HealthPotions.ToConsumable();

                iconButton.sprite = consumable.Icon;
                continueText.SetStringParameter("value", consumable.Stock.ToString());
            }
        }

        private IEnumerator ActivateButtonAsync()
        {
            yield return new WaitForSecondsRealtime(BUTTON_ACTIVATION_DELAY);
            continueButton.interactable = true;
            finishButton.interactable = true;
        }

        private void ClickContinueButton()
        {
            gameObject.SetActive(false);
            Level.Instance.ContinueToPlay(useGreenGoo);
        }

        private void ClickFinishButton()
        {
            gameObject.SetActive(false);
            Player.Die();
        }
    }
}