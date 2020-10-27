using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class DebugPanel : Singelton<DebugPanel>, ISingeltonInitializeHandler
    {
        private const string PREFS_DEBUG_IS_ON = "DebugIsOn";

        public Text textPrefab;
        public GameObject debugSavingStatus;
        public GameObject debugText;
        public GameObject debugLog;
        public Transform debugLogContent;
        public Button backButton;

        private StoredBool isOn;
        public static bool IsOn { get => Instance.isOn.Value;  set => Instance.UpdateCanvas(Instance.isOn.Value = value); }

        public void Initialize()
        {
            isOn = new StoredBool(PREFS_DEBUG_IS_ON, false);
        }

        private void Start()
        {
            backButton.onClick.AddListener(() => debugLog.SetActive(false));
            UpdateCanvas(IsOn);
        }

        public static void Log(string message)
        {
            if (Instance == null)
            {
                return;
            }

            System.DateTime now = System.DateTime.Now;
            Text text = Instantiate(Instance.textPrefab, Instance.debugText.transform);
            text.text = "[" + now.ToString("HH:mm:ss") + "] " + message;
            Destroy(text.gameObject, 15f);

            text = Instantiate(Instance.textPrefab, Instance.debugLogContent);
            text.text = "[" + now.ToString("HH:mm:ss") + "] " + message;
        }

        private void UpdateCanvas(bool isOn)
        {
            debugSavingStatus.SetActive(isOn);
            debugText.SetActive(isOn);
        }

        public static void OpenDebugLog()
        {
            Instance?.debugLog.SetActive(true);
        }

    }
}