using System;
using TinyLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public abstract class Item : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField] private string nameKey;
        [SerializeField] private string informationKey;
        [SerializeField] private Sprite icon;
#pragma warning restore CS0649

        protected abstract float IconScale { get; }
        public abstract bool Available { get; }
        protected abstract AssetTag AssetTag { get; }
        public string Name => LocalizationManager.GetTranslation(nameKey) as string;
        public string NameKey => nameKey;
        public string InformationKey => informationKey;
        public Sprite Icon => icon;

        public event EventHandler OnWasCollected;
        
        public void UpdateIcon(Image image)
        {
            if (image is null) return;
            image.sprite = icon;
            image.transform.localScale = IconScale * Vector3.one;
        }

        public void WasCollected()
        {
            OnWasCollected?.Invoke(this, EventArgs.Empty);
        }
    }
}