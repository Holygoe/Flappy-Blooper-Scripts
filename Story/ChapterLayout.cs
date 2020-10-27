using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyLocalization;

namespace FlappyBlooper
{
    public class ChapterLayout : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ChapterTag chapterTag;
#pragma warning restore CS0649

        private Localize _captionText;
        private Transform _stagesTransform;

        private void Awake()
        {
            _captionText = transform.Find("Caption").GetComponent<Localize>();
            _stagesTransform = transform.Find("Stages");
        }

        private void OnEnable()
        {
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            var chapter = chapterTag.ToChapter();
            _captionText.QuickConnect(chapter.NameKey);
            _stagesTransform.Clear();
            var prefab = Assets.Instance.levelButtonPrefab;
            prefab.ChapterTag = (ChapterTag)chapter.TaggedName;
            
            for (var i = 0; i < chapter.StagesCount; i++)
            {
                prefab.Index = i;
                Instantiate(prefab, _stagesTransform);
            }
        }
    }
}