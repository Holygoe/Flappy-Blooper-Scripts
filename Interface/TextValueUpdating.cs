using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class TextValueUpdating : MonoBehaviour
    {
        public enum Tag { Cookies, Score }

        public Tag valueTag;
        public Animator animator;

        private TextMeshProUGUI _text;
        private int _value;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _value = GetNewValue();

            // Инициализируем переменную, иначе будет ругаться
            if (animator == null)
            {
                animator = null;
            }
        }

        private void Update()
        {
            int newValue = GetNewValue();

            if (_value != newValue)
            {
                _value = newValue;

                if (animator)
                {
                    animator.SetTrigger(Triggers.Woop);
                }
            }
            _text.text = _value.ToString();
        }

        private int GetNewValue()
        {
            int value;

            switch (valueTag)
            {
                case Tag.Cookies:
                    value = CurrencyTag.Cookies.ToCurrency().Stock;
                    break;
                case Tag.Score:
                    value = Level.Score;
                    break;
                default:
                    value = 0;
                    break;
            }

            return value;
        }
    }
}
