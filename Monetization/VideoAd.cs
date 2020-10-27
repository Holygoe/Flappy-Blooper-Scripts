using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace FlappyBlooper
{
    public class VideoAd : MonoBehaviour
    {
        private const string PrefsAvailableInStock = "VideoAdAvailableInStock";
        private const string PrefsNextBlockTimeAppointed = "VideoAdNextBlockTimeAppointed";
        private const string PrefsNextBlockTime = "VideoAdТextBlockTime";

#pragma warning disable CS0649
        [SerializeField] private VideoAdTag adTag;
        [SerializeField] private int adsInBlock;
        [SerializeField] private float blockPeriod;
#pragma warning restore CS0649
        
        private RewardedAd _rewardedAd;
        private bool _rewardEarned;
        private StoredInt _availableInStock;
        private StoredBool _nextBlockTimeAppointed;
        private StoresDateTime _nextBlockTime;
        private bool _adLoaded;
        private string _adId;
        private WatchAdCallback _callback;
        private bool _adLoading;
        private bool _isItWaitingForNextBlock;

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public VideoAdTag AdTag => adTag;

        public bool AdLoaded
        {
            get => _adLoaded;
            
            private set
            {
                _adLoaded = value;
                OnStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnStatusChanged;

        public void Initialize(string adId)
        {
            _adId = adId;
            _availableInStock = new StoredInt($"{adTag}_{PrefsAvailableInStock}", adsInBlock);
            _nextBlockTimeAppointed = new StoredBool($"{adTag}_{PrefsNextBlockTimeAppointed}", true);
            _nextBlockTime = new StoresDateTime($"{adTag}_{PrefsNextBlockTime}", DateTime.Now);
            LoadAd();
        }

        private void LoadAd()
        {
            if (AdLoaded || _adLoading) return;
            if (_availableInStock.Value < adsInBlock) StartCoroutine(WaitForNextBlockAsync());
            if (_availableInStock.Value <= 0) return;

            _adLoading = true;
            _rewardedAd = new RewardedAd(_adId);
            
            _rewardedAd.OnAdLoaded += (sender, args) =>
            {
                _adLoading = false;
                AdLoaded = true;
            };
            
            _rewardedAd.OnAdClosed += (sender, args) => StartCoroutine(RewardedAdClosedAsync()); 
            _rewardedAd.OnAdFailedToLoad += (sender, args) => StartCoroutine(RewardedAdFailedToLoadAsync()); 
            _rewardedAd.OnUserEarnedReward += (sender, reward) => _rewardEarned = true;
            var request = new AdRequest.Builder().Build();
            _rewardedAd.LoadAd(request);

#if UNITY_EDITOR
            AdLoaded = true;
            _adLoading = false;
#endif
        }

        private IEnumerator WaitForNextBlockAsync()
        {
            if (!_nextBlockTimeAppointed.Value)
            {
                _nextBlockTimeAppointed.Value = true;
                _nextBlockTime.Value = DateTime.Now.AddMinutes(blockPeriod);
            }
            
            if (_isItWaitingForNextBlock) yield break;
            _isItWaitingForNextBlock = true;
            
            while (DateTime.Now <= _nextBlockTime.Value)
            {
                DebugPanel.Log("Waiting... in stock " + _availableInStock.Value + " Ads...");
                DebugPanel.Log(" ...waiting time: " + _nextBlockTime.Value.ToString(CultureInfo.CurrentCulture) + "...");
                yield return new WaitForSecondsRealtime(15);
            }

            DebugPanel.Log("Ads number updated to " + adsInBlock + ". Stop.");
            _availableInStock.Value = adsInBlock;
            _nextBlockTimeAppointed.Value = false;
            _isItWaitingForNextBlock = false;
            LoadAd();
        }

        public void WatchAd(WatchAdCallback callback)
        {
#if UNITY_EDITOR
            _availableInStock.Value--;
            _rewardedAd = null;
            _callback = callback;
            _rewardEarned = true;
            StartCoroutine(RewardedAdClosedAsync());
#else
            if (_rewardedAd.IsLoaded())
            {
                _availableInStock.Value--;
                _callback = callback;
                _rewardEarned = false;
                _rewardedAd.Show();
            }
            else
            {
                AdLoaded = false;
            }
#endif
        }

        private IEnumerator RewardedAdFailedToLoadAsync()
        {
            _adLoading = false;
            AdLoaded = false;
            yield return new WaitForSecondsRealtime(30f);
            LoadAd();
        }

        private IEnumerator RewardedAdClosedAsync()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            _callback?.Invoke(_rewardEarned);
            AdLoaded = false;
            LoadAd();
        }
    }

    public delegate void WatchAdCallback(bool rewardEarned);
}