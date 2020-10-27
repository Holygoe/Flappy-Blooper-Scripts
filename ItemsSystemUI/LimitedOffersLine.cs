using System;
using System.Collections;
using System.Collections.Generic;
using TinyLocalization;
using TMPro;
using UnityEngine;

namespace FlappyBlooper
{
    public class LimitedOffersLine : MonoBehaviour
    {
#pragma warning disable CS0649        
        [SerializeField] private LimitedOfferMaster offerMaster;
        [SerializeField] private TextMeshProUGUI offersUpdateText;
        [SerializeField] private Transform cardsTransform;
#pragma warning restore CS0649
        
        private Animator _offersUpdateTextAnimator;
        private string _offersWillBeUpdated;
        private string _inLessThanAMinute;
        private string _minuteAbridged;
        private string _hourAbridged;
        private string _inTheTime;

        private void Awake()
        {
            _offersWillBeUpdated = LocalizationManager.GetTranslation(LocalizeKey.OffersWillBeUpdated) as string;
            _inLessThanAMinute = LocalizationManager.GetTranslation(LocalizeKey.InLessThanAMinute) as string;
            _minuteAbridged = LocalizationManager.GetTranslation(LocalizeKey.MinuteAbridged) as string;
            _hourAbridged = LocalizationManager.GetTranslation(LocalizeKey.HourAbridged) as string;
            _inTheTime = LocalizationManager.GetTranslation(LocalizeKey.InTheTime) as string;
        }

        private void OnEnable()
        {
            _offersUpdateTextAnimator = offersUpdateText.GetComponent<Animator>();
            StartCoroutine(UpdateLineAsync());
        }

        private void UpdateOffersUpdateText(TimeSpan restTime)
        {
            if (!offersUpdateText) return;
            var minutes = restTime.Minutes;
            var hours = restTime.Hours;
            var text = _offersWillBeUpdated;

            if (minutes < 1 && hours < 1)
            {
                text += $" {_inLessThanAMinute}";
            }
            else
            {
                text += $" {_inTheTime}";
                if (hours > 0) text += $" {hours} {_hourAbridged}";
                text += $" {minutes} {_minuteAbridged}";
            }

            offersUpdateText.text = text;
        }

        private IEnumerator UpdateLineAsync()
        {
            cardsTransform.Clear();
            var cards = new List<ProductCard>();
            var offerUpdated = offerMaster.TryToUpdateOffers();
            var offers = offerMaster.GetOffers();

            foreach (var offer in offers)
            {
                var card = Instantiate(offer.GetCardPrefab(), cardsTransform);
                card.Initialize(offer);
                cards.Add(card);
            }

            UpdateOffersUpdateText(offerMaster.TimeLeftUntilUpdate);
            if (!offerUpdated) yield break;
            var audioClip = Assets.Instance.offerUpdated;
            SoundMaster.PlayOneShot(audioClip);
            yield return new WaitForSecondsRealtime(0.1f);

            foreach (var card in cards)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                card.Woop();
            }

            yield return new WaitForSecondsRealtime(0.1f);
            if (_offersUpdateTextAnimator) _offersUpdateTextAnimator.SetTrigger(Triggers.Woop);
        }
    }
}