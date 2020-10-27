using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class StageButton : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ChapterTag chapterTag;
        [SerializeField] private int index;
#pragma warning restore CS0649
        
        private Transform _locked;
        private Transform _unlocked;
        private TextMeshProUGUI _numberText;
        private StarsArray _stars;
        private Button _button;

        public static event EventHandler OnAnyButtonClicked;
        
        public ChapterTag ChapterTag
        {
            set => chapterTag = value;
        }
        
        public int Index
        {
            set => index = value;
        }
        
        private void Awake()
        {
            var background = transform.Find("Background");
            _locked = background.Find("Locked");
            _unlocked = background.Find("Unlocked");
            _numberText = _unlocked.Find("Number").GetComponent<TextMeshProUGUI>();
            _stars = GetComponent<StarsArray>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            var stage = chapterTag.ToChapter().GetStageInfo(index);
            _numberText.text = stage.Number;

            _locked.gameObject.SetActive(!stage.Unlocked);
            _unlocked.gameObject.SetActive(stage.Unlocked);
            _button.interactable = stage.Unlocked;

            if (stage.Unlocked)
            {
                _stars.UpdateProgress(stage.Progress);
            }
        }

        private void OnButtonClicked()
        {
            Game.CurrentChapterTag = chapterTag;
            Game.CurrentStageIndex = index;
            StagePopup.Open();
            OnAnyButtonClicked?.Invoke(null, EventArgs.Empty);
        }
    }
}