using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class SoundHandler : MonoBehaviour
    {
        public bool playOnEnable;
        public AudioClipBundle bundle;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                PlayOneShot();
            }
        }

        public void PlayOneShot()
        {
            SoundMaster.PlayOneShot(bundle, _audioSource);
        }
    }
}