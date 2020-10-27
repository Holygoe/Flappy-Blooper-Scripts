using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class PausePopup : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private ToggleSwitch music;
        [SerializeField] private ToggleSwitch sounds;
#pragma warning restore CS0649

        private PopupWindow _popup;

        private void Awake()
        {
            _popup = GetComponent<PopupWindow>();
            resumeButton.onClick.AddListener(() => { _popup.Close(); Level.CountdownAfterPause.Start(); });
            restartButton.onClick.AddListener(() => { Level.RecordProgress(); SceneLoader.LoadScene(GameScene.Level); });
            music.IsOn = !MusicMaster.Mute;
            music.onClick.AddListener(value => MusicMaster.Mute = !value);
            sounds.IsOn = !SoundMaster.Mute;
            sounds.onClick.AddListener(value => SoundMaster.Mute = !value);
        }

        private void OnEnable()
        {
            if (Level.State != LevelState.Playing)
            {
                gameObject.SetActive(false);
                return;
            }

            Level.State = LevelState.Paused;
        }
    }
}