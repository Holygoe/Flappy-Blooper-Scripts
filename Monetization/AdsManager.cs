using GoogleMobileAds.Api;
using System.Collections.Generic;

namespace FlappyBlooper
{
    public class AdsManager : Singelton<AdsManager>, ISingeltonInitializeHandler
    {
#if UNITY_ANDROID
        private readonly Dictionary<VideoAdTag, string> _videoAdIds = new Dictionary<VideoAdTag, string>
        {
            { VideoAdTag.Store, "ca-app-pub-9097002331946088/8017403274" },
            { VideoAdTag.Offer, "ca-app-pub-9097002331946088/8017403274" },
        };
#elif UNITY_IPHONE
        private readonly Dictionary<AdTag, string> adIds = new Dictionary<AdTag, string>
        {
            { AdTag.Default, "ca-app-pub-3940256099942544/5224354917" },
            { AdTag.Offer, "ca-app-pub-3940256099942544/5224354917" },
        };
#endif
        
        private static Dictionary<VideoAdTag, VideoAd> _videoAds;

        void ISingeltonInitializeHandler.Initialize()
        {
            MobileAds.Initialize(initStatus => { });
            _videoAds = new Dictionary<VideoAdTag, VideoAd>();
            
            foreach (var videoAd in transform.GetComponentsInChildren<VideoAd>())
            {
                _videoAds.Add(videoAd.AdTag, videoAd);
                videoAd.Initialize(_videoAdIds[videoAd.AdTag]);
            }
        }

        public static VideoAd GetVideoAd(VideoAdTag tag)
        {
            return _videoAds[tag];
        }
    }

    public enum VideoAdTag
    {
        Store,
        Offer
    }
}