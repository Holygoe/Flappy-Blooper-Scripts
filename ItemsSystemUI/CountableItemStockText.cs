using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    public class CountableItemStockText : MonoBehaviour
    {
#pragma warning disable CS0649
        [FormerlySerializedAs("entity")] [SerializeField]
        private CountableItem countableItem;
#pragma warning restore CS0649
        
        private Animator _animator;
        private int _value;
        private TextMeshProUGUI _valueText;

        private void Awake()
        {
            _valueText = GetComponent<TextMeshProUGUI>();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _value = countableItem.Stock;
            _valueText.text = _value.ToString();
            countableItem.OnStockChanged += CountableItemStockChanged;
        }

        private void OnDisable()
        {
            countableItem.OnStockChanged += CountableItemStockChanged;
        }

        private void CountableItemStockChanged(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            var amountInStock = countableItem.Stock;
            if (_value == amountInStock) return;
            _value = amountInStock;
            _valueText.text = _value.ToString();
            if (_animator) _animator.SetTrigger(Triggers.Woop);
        }
    }
}