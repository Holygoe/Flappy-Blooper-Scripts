using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public abstract class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
#pragma warning disable CS0649
        [SerializeField] private Image icon;
        [SerializeField] protected TextMeshProUGUI title;
        [SerializeField] private Animator woopAnimator;
#pragma warning restore CS0649
        
        private const float TimeToPopupInformation = 0.2f;
        private float _waitingTimeToOpenInformation;
        private bool _isPointerDown;
        
        protected abstract Item Item { get; }

        protected virtual void UpdateCard()
        {
            Item.UpdateIcon(icon);
        }

        private void Update()
        {
            if (!_isPointerDown) return;
            _waitingTimeToOpenInformation += Time.unscaledDeltaTime;

            if (_waitingTimeToOpenInformation > TimeToPopupInformation
                && QuickInformationPopup.Instance && !QuickInformationPopup.Instance.gameObject.activeSelf)
            {
                QuickInformationPopup.Open(Item);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _waitingTimeToOpenInformation = 0;
            _isPointerDown = false;
            QuickInformationPopup.Close();
        }

        public void Woop()
        {
            if (woopAnimator) woopAnimator.SetTrigger(Triggers.Woop);
        }

        protected virtual void OnEnable()
        {
            if (Item is null) return;
            UpdateCard();
        }
    }
}