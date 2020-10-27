using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlappyBlooper
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameLoaderDimmer : MonoBehaviour, IPointerDownHandler
    {
        private const float WaitingTimeOfTapping = 8f;
        
        public float dimmingRate = 4f;

        private CanvasGroup _canvas;
        private bool _toggle;
        private bool _waitingForLoad;
        private bool _waitingForTap;
        private float _targetAlpha;
        private float _tapCountdown;

        private Transform _waitingIcon;
        private Transform _tapToContinue;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
            _waitingIcon = transform.Find("WaitingIcon");
            _tapToContinue = transform.Find("TapToContinue");
        }

        private void OnEnable()
        {
            _canvas.alpha = 0;
        }

        private void Update()
        {
            var currentAlpha = _canvas.alpha;

            if (currentAlpha != _targetAlpha)
            {
                var alpha = Mathf.MoveTowards(currentAlpha, _targetAlpha, dimmingRate * Time.unscaledDeltaTime);
                _canvas.alpha = alpha;
            }
            else
            {
                _waitingForLoad = false;
                if (!_toggle) gameObject.SetActive(false);
            }

            if (_tapCountdown > 0)
                _tapCountdown -= Time.fixedUnscaledDeltaTime;
            else
                _waitingForTap = false;
        }

        public void Switch(bool toggle)
        {
            if (_toggle == toggle) return;

            _waitingForLoad = true;
            _toggle = toggle;
            _targetAlpha = toggle ? 1 : 0;

            if (!toggle) return;
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            SetActiveWaitingIcon(true);
        }

        public void StartWaitingForTap()
        {
            _tapCountdown = WaitingTimeOfTapping;
            _waitingForTap = true;

            SetActiveWaitingIcon(false);
        }

        public IEnumerator WaitForTapAsync()
        {
            while (_waitingForTap)
            {
                yield return null;
            }
        }

        public IEnumerator WaitForSwitchAsync()
        {
            while (_waitingForLoad)
            {
                yield return null;
            }

            SetActiveWaitingIcon(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _waitingForTap = false;
        }

        private void SetActiveWaitingIcon(bool value)
        {
            if (_waitingIcon) _waitingIcon.gameObject.SetActive(value);
            if (_tapToContinue) _tapToContinue.gameObject.SetActive(!value);
        }
    }
}