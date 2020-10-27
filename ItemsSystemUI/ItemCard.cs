using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class ItemCard : Card
    {
#pragma warning disable CS0649
        [SerializeField] private Button selectButton;
        [SerializeField] private Button deselectButton;
        [SerializeField] private Transform selectedLabel;
        [SerializeField] private TextMeshProUGUI stockText;
#pragma warning restore CS0649

        private Item _item;
        
        protected override Item Item => _item;

        public void Initialize(Item item)
        {
            _item = item;
            gameObject.SetActive(true);

            if (item is UniqueItem uniqueItem)
            {
                selectButton.onClick.AddListener(() => uniqueItem.Select(true));
            }

            if (item is IDeselectableItem deselectableItem)
            {
                deselectButton.onClick.AddListener(() => deselectableItem.Deselect());
            }
        }

        private void Awake()
        {
            if (_item is null) gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            if (_item is null) return;
            base.OnEnable();
            _item.OnWasCollected += ItemWasCollected;
        }

        private void OnDisable()
        {
            if (_item is null) return;
            _item.OnWasCollected -= ItemWasCollected;
        }

        private void ItemWasCollected(object sender, EventArgs e)
        {
            Woop();
        }

        protected override void UpdateCard()
        {
            base.UpdateCard();

            var selectButtonActive = false;
            var deselectButtonActive = false;
            var selectedLabelActive = false;

            if (_item is UniqueItem uniqueItem)
            {
                selectButtonActive = !uniqueItem.Selected;

                if (uniqueItem is IDeselectableItem)
                {
                    deselectButtonActive = uniqueItem.Selected;
                }
                else
                {
                    selectedLabelActive = uniqueItem.Selected;
                }
            }
            
            selectButton.gameObject.SetActive(selectButtonActive);
            deselectButton.gameObject.SetActive(deselectButtonActive);
            selectedLabel.gameObject.SetActive(selectedLabelActive);

            var stockTextActive = false;

            if (_item is CountableItem countableItem)
            {
                stockTextActive = true;
                stockText.text = countableItem.Stock.ToString();
            }

            stockText.gameObject.SetActive(stockTextActive);
        }
    }
}