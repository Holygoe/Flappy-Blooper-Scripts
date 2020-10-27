using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DimmerHandler : Singelton<DimmerHandler>
    {
        private const float dimmingRate = 4f;

        private CanvasGroup canvas;
        private bool waiting = false;
        private int openedPopups = 0;

        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            canvas.alpha = 0;
        }

        private void Update()
        {
            float targetAlpha = openedPopups > 0 ? 1 : 0;
            float currentAlpha = canvas.alpha;

            if (currentAlpha != targetAlpha)
            {
                float alpha = Mathf.MoveTowards(currentAlpha, targetAlpha, dimmingRate * Time.unscaledDeltaTime);
                canvas.alpha = alpha;
            }
            else
            {
                waiting = false;

                if (openedPopups <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public static void Switch(bool switchOn)
        {
            if (Instance == null)
            {
                return;
            }

            Instance.waiting = true;
            Instance.openedPopups += switchOn ? 1 : -1;

            if (switchOn && !Instance.gameObject.activeSelf)
            {
                Instance.gameObject.SetActive(true);
            }
        }

        public IEnumerator WaitingAsync()
        {
            while (waiting)
            {
                yield return null;
            }
        }

    }
}