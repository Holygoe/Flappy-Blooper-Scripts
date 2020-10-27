using System;
using UnityEngine;

namespace FlappyBlooper
{
    public class StoryLayout : MonoBehaviour
    {
        private const string PrefContentPosition = "StoryLayoutContentPosition";

#pragma warning disable CS0649
        [SerializeField] private Transform content;
#pragma warning restore CS0649

        private StoredInt _contentPosition;

        private void OnEnable()
        {
            StageButton.OnAnyButtonClicked += AnyButtonClicked;
        }

        private void OnDisable()
        {
            StageButton.OnAnyButtonClicked -= AnyButtonClicked;
        }

        private void Start()
        {
            _contentPosition = new StoredInt(PrefContentPosition, 0);
            var position = content.position;
            position.y = _contentPosition.Value;
            content.position = position;
            MusicMaster.Play(Music.MainMenu);
        }

        private void AnyButtonClicked(object sender, EventArgs e)
        {
            _contentPosition.Value = (int) content.position.y;
        }
    }
}