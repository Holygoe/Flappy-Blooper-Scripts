using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FlappyBlooper
{
    public class SoundMaster : Singelton<SoundMaster>, ISingeltonInitializeHandler
    {
        private const string PREFS_SOUNDS_MUTE = "SoundsMute";
        private const string PARAM_SOUNDS_VOLUME = "SoundsVolume";

        public AudioMixer audioMixer;
        public AudioSource uiAudioSource;
        public AudioClipBundle defaultButtonClick;

        private StoredBool mute;
        public static bool Mute { get => Instance.mute.Value; set => Instance.SetVolume(Instance.mute.Value = value); }

        public void Initialize()
        {
            mute = new StoredBool(PREFS_SOUNDS_MUTE, false);
        }

        private void Start()
        {
            SetVolume(Mute);
        }

        public static void PlayOneShot(AudioClipBundle bundle, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                audioSource = Instance.uiAudioSource;
            }

            audioSource.volume = bundle.Volume;
            audioSource.PlayOneShot(bundle.AudioClip);
        }

        private void SetVolume(bool mute)
        {
            if (mute)
            {
                Instance.audioMixer.SetFloat(PARAM_SOUNDS_VOLUME, -80);
            }
            else
            {
                Instance.audioMixer.SetFloat(PARAM_SOUNDS_VOLUME, 0);
            }
        }
    }
}