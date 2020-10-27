using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class RudeButton : Button
    {
#pragma warning disable CS0649
        [SerializeField] private Sprite disabledSprite;
        [SerializeField] private Color disabledColor = new Color(0.2f, 0.14f, 0.06f);
        [SerializeField] private Graphic[] graphics;
#pragma warning restore CS0649

        private bool _initialized;
        private Color[] _normalGraphicColors;
        private Sprite _normalSprite;
        private Image _targetImage;

        // ReSharper disable once InconsistentNaming
        public bool Interactable
        {
            set
            {
                interactable = value;
                InteractableChanged(value);
            }
        }

        protected override void Awake()
        {
            Initialize();
            Interactable = interactable;
        }

        private void Initialize()
        {
            if (_initialized) return;
            _targetImage = targetGraphic as Image;
            if (_targetImage == null) throw new Exception("Target mast be image");
            _normalSprite = _targetImage.sprite;
            _normalGraphicColors = new Color[graphics.Count()];
            
            for (var i = 0; i < graphics.Length; i++)
            {
                _normalGraphicColors[i] = graphics[i].color;
            }
            
            _initialized = true;
        }

        private void InteractableChanged(bool value)
        {
            if (!_initialized) Initialize();

            _targetImage.sprite = value ? _normalSprite : disabledSprite;

            if (value)
            {
                for (var i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = _normalGraphicColors[i];
                }
            }
            else
            {
                foreach (var t in graphics)
                {
                    t.color = disabledColor;
                }
            }
        }
    }
}