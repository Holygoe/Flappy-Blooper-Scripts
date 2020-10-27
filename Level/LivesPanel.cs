using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class LivesPanel : MonoBehaviour
    {
        public Transform livesIconPrefab;
        public Transform healthPotionPrefab;
        public Material blinkingSprite;

        private int livesNumber;
        private int healthPotionsNumber;
        private bool starting;

        private void Start()
        {
            starting = true;
            livesNumber = Player.LivesNumber;
            healthPotionsNumber = Player.HealthPotionsNumber;
        }

        private void Update()
        {
            if (livesNumber != Player.LivesNumber || healthPotionsNumber != Player.HealthPotionsNumber || starting)
            {
                Clear();

                starting = false;

                for (int i = 0; i < healthPotionsNumber; i++)
                {
                    Transform clone = Instantiate(healthPotionPrefab, transform);

                    if (i >= Player.HealthPotionsNumber)
                    {
                        clone.Find("Image").GetComponent<Image>().material = blinkingSprite;
                        Destroy(clone.gameObject, 1f);
                    }
                }

                for (int i = 0; i < livesNumber; i++)
                {
                    Transform clone = Instantiate(livesIconPrefab, transform);

                    if (i >= Player.LivesNumber)
                    {
                        clone.GetComponent<Image>().material = blinkingSprite;
                        Destroy(clone.gameObject, 1f);
                    }
                }
                livesNumber = Player.LivesNumber;
                healthPotionsNumber = Player.HealthPotionsNumber;
            }
        }

        public void Clear()
        {
            int iconsCount = transform.childCount;
            for (int i = 0; i < iconsCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}