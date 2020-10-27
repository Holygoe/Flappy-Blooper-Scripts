using UnityEngine;

namespace FlappyBlooper
{
    public abstract class UniqueItem : Item
    {
#pragma warning disable CS0649
        [SerializeField] private Product product;
#pragma warning restore CS0649

        public abstract bool Selected { get; }
        public abstract bool Purchased { get; }

        public Product Product => product;
        public override bool Available => Purchased;

        public void Obtain(bool saveGame)
        {
            Obtain();
            if (saveGame) DataManager.SaveData();
        }

        public void Select(bool saveGame)
        {
            Select();
            if (saveGame) DataManager.SaveData();
        }

        protected abstract void Select();
        protected abstract void Obtain();
    }
}