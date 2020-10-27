using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FlappyBlooper
{

    public class MusicMaster : Singelton<MusicMaster>, ISingeltonInitializeHandler
    {
        private const float SpeedOfVolumeChanging = 2;
        private const float SnapshotTransitionTime = 0.2f;
        private const string PrefsMusicMute = "MusicMute";

#pragma warning disable CS0649
        [SerializeField] private AudioMixerSnapshot unpausedSnapshot;
        [SerializeField] private AudioMixerSnapshot pausedSnapshot;
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip levelMusic;
        [SerializeField] private AudioClip stageComplete;
#pragma warning restore CS0649
        

        private AudioSource _audioSource;
        private static Dictionary<Music, AudioClip> _musics;
        private bool _isItPlaying;
        private StoredBool _mute;
        public static bool Mute { get => Instance._mute.Value; set { Instance._mute.Value = value; Instance._audioSource.mute = value; } }
        
        void ISingeltonInitializeHandler.Initialize()
        {
            _mute = new StoredBool(PrefsMusicMute, false);
            
            _musics = new Dictionary<Music, AudioClip>
            {
                { Music.MainMenu, mainMenuMusic },
                { Music.Level, levelMusic },
                { Music.StageComplete, stageComplete },
            };
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.mute = Mute;
        }

        private IEnumerator StartPlayingAsync(Music musicTag)
        {
            if (_audioSource.isPlaying)
            {
                yield return StartCoroutine(StopPlayingAsync());
            }

            _audioSource.volume = 0;
            _isItPlaying = true;
            _audioSource.clip = _musics[musicTag];
            _audioSource.Play();

            do
            {
                yield return null;
                _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, 1, SpeedOfVolumeChanging * Time.deltaTime);
            }
            while (_isItPlaying && _audioSource.volume < 1);
        }

        private IEnumerator StopPlayingAsync()
        {
            _isItPlaying = false;

            do
            {
                yield return null;
                _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, 0, SpeedOfVolumeChanging * Time.deltaTime);
            }
            while (!_isItPlaying && _audioSource.volume > 0);

            if (!_isItPlaying)
            {
                Instance._audioSource.Stop();
            }
        }

        public static void Play(Music musicTag)
        {
            if (Instance._audioSource.isPlaying && Instance._audioSource.clip == _musics[musicTag])
            {
                return;
            }

            Instance.StartCoroutine(Instance.StartPlayingAsync(musicTag));
        }

        public static void Stop()
        {
            Instance.StartCoroutine(Instance.StopPlayingAsync());
        }

        public static void Lowpass(bool value)
        {
            if (value)
            {
                Instance.pausedSnapshot.TransitionTo(SnapshotTransitionTime);
            }
            else
            {
                Instance.unpausedSnapshot.TransitionTo(SnapshotTransitionTime);
            }
        }
    }

    public static class MusicExtensions
    {
        public static void Play(this Music music)
        {
            MusicMaster.Play(music);
        }
    }
    
    public enum Music { MainMenu, Level, StageComplete }
}