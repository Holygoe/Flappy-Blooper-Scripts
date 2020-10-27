using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class SettingsManager : MonoBehaviour
    {
        public ToggleSwitch debugInfo;
        public TextMeshProUGUI version;
        public ToggleSwitch music;
        public ToggleSwitch sounds;
        public Button resetProgress;
        public Button debugLog;
        public PopupWindow resetProgressPopup;
        
        private void Start()
        {
            version.text = $"v. {Application.version}";
            debugInfo.IsOn = DebugPanel.IsOn;
            debugInfo.onClick.AddListener(value => DebugPanel.IsOn = value);
            music.IsOn = !MusicMaster.Mute;
            music.onClick.AddListener(value => MusicMaster.Mute = !value);
            sounds.IsOn = !SoundMaster.Mute;
            sounds.onClick.AddListener(value => SoundMaster.Mute = !value);
            debugLog.onClick.AddListener(DebugPanel.OpenDebugLog);
            
            resetProgress.onClick.AddListener(() =>
            {
                DataManager.ResetGameData();
                resetProgressPopup.Open();
            });
        }
    }
}