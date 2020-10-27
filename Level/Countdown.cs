using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class Countdown : MonoBehaviour
    {
        public int initialValue;
        public bool autoClose;
        public UnityEvent onWasOver;
        public TextMeshProUGUI value;
        public Animator animator;
        public SoundHandler soundHandler;

        public bool IsCounting { get; private set; }

        public void Start()
        {
            if (!IsCounting)
            {
                IsCounting = true;

                if (gameObject.activeSelf)
                {
                    StartCoroutine(CountingDownAsync());
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }

        private void OnEnable()
        {
            StartCoroutine(CountingDownAsync());
        }

        private IEnumerator CountingDownAsync()
        {
            int counter = initialValue;

            do
            {
                soundHandler?.PlayOneShot();
                value.text = counter.ToString();
                animator.SetTrigger(Triggers.Woop);
                yield return new WaitForSecondsRealtime(1);
                counter--;
            }
            while (counter >= 0 && gameObject.activeSelf);

            onWasOver.Invoke();

            IsCounting = false;

            if (autoClose)
            {
                gameObject.SetActive(false);
            }
        }
    }
}