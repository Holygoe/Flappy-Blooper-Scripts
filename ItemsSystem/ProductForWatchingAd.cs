using UnityEngine;

namespace FlappyBlooper
{
    [CreateAssetMenu(fileName = "Product For Watching Ad", menuName = "Game / Product For Watching Ad")]
    public class ProductForWatchingAd : Product
    {
#pragma warning disable CS0649
        [SerializeField] private VideoAdTag adTag;
#pragma warning restore CS0649
        
        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public VideoAdTag AdTag
        {
            set => adTag = value;
        }

        public VideoAd VideoAd => AdsManager.GetVideoAd(adTag);

        protected override ProductCard CardPrefab => Assets.ProductFoWatchingAd;

        protected override void Purchase(PurchaseOfferCallback callback)
        {
            VideoAd.WatchAd((rewardEarned) =>
            {
                if (rewardEarned)
                {
                    CollectProduct(callback);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }
    }
}