using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class DebuggingMaster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject debuggingObject;
        private StoredBool showDebugging;
        private float counterToDebugging;
        private bool waitingToDebugging;
        private const float TIME_TO_DEBUGGING = 7;

        private void Awake()
        {
            showDebugging = new StoredBool("OptionsSceneShowDebugging", false);
            UpdateDebugging();
        }

        private void Update()
        {
            if (waitingToDebugging)
            {
                counterToDebugging += Time.unscaledDeltaTime;

                if (counterToDebugging > TIME_TO_DEBUGGING)
                {
                    counterToDebugging = 0;
                    showDebugging.Value = !showDebugging.Value;
                    UpdateDebugging();
                }
            }
            else
            {
                counterToDebugging = 0;
            }
        }

        private void UpdateDebugging()
        {
            debuggingObject.SetActive(showDebugging.Value);
            int spacing = showDebugging.Value ? 30 : 40;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            waitingToDebugging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            waitingToDebugging = false;
            counterToDebugging = 0;
        }
    }
}