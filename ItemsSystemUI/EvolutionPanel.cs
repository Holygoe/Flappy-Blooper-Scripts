using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class EvolutionPanel : MonoBehaviour
    {
        public Button evolveButton;
        public Sprite startSetSprite;
        public Sprite starUnsetSprite;
        private TextMeshProUGUI evolvePriceText;
        private Image evolvePriceIcon;
        private Color priceColor;

        private void Awake()
        {
            evolveButton.onClick.AddListener(Evolve);
            evolvePriceText = evolveButton.transform.Find("Price").Find("Text").GetComponent<TextMeshProUGUI>();
            evolvePriceIcon = evolvePriceText.transform.Find("Icon").GetComponent<Image>();
            priceColor = evolvePriceText.color;
        }

        public void OnEnable()
        {
            UpdatePanel();
            UpdateButton();
            Player.OnPlayerUpdated += Player_OnPlayerUpdated;
            Character.ItemsToEvolve.OnStockChanged += EvolutionConsumable_OnNumberWasChanged;
        }

        private void OnDisable()
        {
            Player.OnPlayerUpdated -= Player_OnPlayerUpdated;
            Character.ItemsToEvolve.OnStockChanged -= EvolutionConsumable_OnNumberWasChanged;
        }

        private void EvolutionConsumable_OnNumberWasChanged(object sender, System.EventArgs e)
        {
            UpdateButton();
        }

        private void Player_OnPlayerUpdated(object sender, System.EventArgs e)
        {
            UpdateButton();
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            int evolution = Character.Current.Evolution;
            Image star = new GameObject("StarSet", typeof(Image)).GetComponent<Image>();
            star.rectTransform.sizeDelta = new Vector2(20, 20);
            star.sprite = startSetSprite;

            for (int i = 0; i < evolution; i++)
            {
                Instantiate(star, transform);
            }

            star.sprite = starUnsetSprite;

            for (int i = evolution; i < Character.MaxEvolvesCount; i++)
            {
                Instantiate(star, transform);
            }

            Destroy(star.gameObject);
        }

        private void UpdateButton()
        {
            Consumable consumable = Character.ItemsToEvolve;
            evolvePriceIcon.sprite = consumable.Icon;

            int cost = Character.Current.EvolutionCost;
            evolvePriceText.text = cost.ToString();
            Color color = cost > consumable.Stock ? Color.red : priceColor;
            evolvePriceText.color = color;
        }

        private void Evolve()
        {
            if (!Character.Current.TryToEvolve())
            {
                InsufficientFundsPopup.Open(Character.ItemsToEvolve);
            }
        }
    }
}