using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "New Bundle", menuName ="Game / Audio Clip Bundle")]
    public class AudioClipBundle : ScriptableObject
    {
#pragma warning disable CS0649
        [Range(min: 0, max: 1)] [SerializeField]
        private float volume = 1;
        [SerializeField] private AudioClip[] clips;
#pragma warning restore CS0649

        public AudioClip AudioClip => clips[Random.Range(0, clips.Length)];
        public float Volume => volume;
    }
}