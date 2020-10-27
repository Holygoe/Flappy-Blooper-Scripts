using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper {
    public class InteractableButtonUpdater : MonoBehaviour
    {
        public Color disabledColor;
        public Sprite disabledSprite;
        public Image mainImage;
        public Graphic[] graphics;
        private Color[] graphicColors;
        private Sprite mainSprite;
        private Button button;
        private bool interactable;
    
    private void Awake()
        {
            button = GetComponent<Button>();
            interactable = button.interactable;
            graphicColors = new Color[graphics.Length];
            mainSprite = mainImage.sprite;

            int length = graphics.Length;
            for (int i = 0; i < length; i++)
            {
                graphicColors[i] = graphics[i].color;
            }

            UpdateCanvas();
        }

        private void Update()
        {
            if (interactable != button.interactable)
            {
                interactable = button.interactable;
                UpdateCanvas();
            }
        }

        private void UpdateCanvas()
        {
            mainImage.sprite = interactable ? mainSprite : disabledSprite;
            int length = graphics.Length;
            for (int i = 0; i < length; i++)
            {
                graphics[i].color = interactable ? graphicColors[i] : disabledColor;
            }
        }
    }
}