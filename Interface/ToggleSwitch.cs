using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class ToggleSwitch : MonoBehaviour
    {
        public Button onButton;
        public Button offButton;
        private bool isOn;

        public ToggleSwitchEvent onClick = new ToggleSwitchEvent();
        public bool IsOn { get => isOn; set { isOn = value; UpdateCanvas(); } }

        private void Awake()
        {
            onButton.onClick.AddListener(() => IsOn = false);
            offButton.onClick.AddListener(() => IsOn = true);
        }

        private void Start()
        {
            UpdateCanvas();
        }

        private void UpdateCanvas()
        {
            onClick.Invoke(isOn);
            onButton.gameObject.SetActive(isOn);
            offButton.gameObject.SetActive(!isOn);
        }

        public class ToggleSwitchEvent : UnityEvent<bool> { }
    }
}