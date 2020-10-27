using System;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class Assets : Singelton<Assets>, ISingeltonInitializeHandler
    {
#pragma warning disable CS0649

        [Header("Sprites")]
        public Sprite emptySprite;

        public Sprite deadFaceSprite;
        public Sprite[] achievementIconBgs;
        public Sprite starOn;
        public Sprite starOff;

        [Header("Tagged Assets")]
        [SerializeField] private TaggedAsset assetHolder;

        [Header("Prefabs")]
        
        [SerializeField] private ProductCard productForCurrencyCard;

        [SerializeField] private ProductCard productForMoneyCard;
        [SerializeField] private ProductCard productFoWatchingAd;
        [SerializeField] private ItemCard itemCard;

        public Transform lifeIconPrefab;
        public Transform healthPotionIconPrefab;
        public StageButton levelButtonPrefab;

        [Header("Audio Clip Bundles")]
        public AudioClipBundle offerUpdated;

#pragma warning restore CS0649

        private SpriteLibraryAsset _spriteLibraryAsset;

        public static SpriteLibraryAsset SpriteLibraryAsset => Instance._spriteLibraryAsset; 
        public static ProductCard ProductForCurrencyCard => Instance.productForCurrencyCard;
        public static ProductCard ProductForMoneyCard => Instance.productForMoneyCard;
        public static ProductCard ProductFoWatchingAd => Instance.productFoWatchingAd;
        public static ItemCard ItemCard => Instance.itemCard;

        public void Initialize()
        {
            Debug.Log("Assets initialize");

            assetHolder.Initialize();
            foreach (var taggedAsset in assetHolder.GetItems<TaggedAsset>())
            {
                taggedAsset.Initialize();
            }

            _spriteLibraryAsset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
            SpriteLibraryAssetCreator.CompleteCharacters(ref _spriteLibraryAsset, 
                GetAsset(AssetTag.Characters).GetItems<Character>(), emptySprite, deadFaceSprite);
            SpriteLibraryAssetCreator.CompleteAccessories(ref _spriteLibraryAsset,
                GetAsset(AssetTag.Accessories).GetItems<Accessory>(), emptySprite);

            //Purpose of icon backgrounds for achievements
            var length = GetAsset(AssetTag.Achievements).Length;
            var spriteCount = achievementIconBgs.Length;
            var achievementArray =  GetAsset(AssetTag.Achievements).GetItems<Achievement>();
            for (var i = 0; i < length; i++)
            {
                var index = i % spriteCount;
                achievementArray[i].IconBg = achievementIconBgs[index];
            }
        }

        public static TaggedAsset GetAsset(AssetTag assetName)
        {
            return Instance.assetHolder.GetItem<TaggedAsset>(assetName);
        }
    }
}