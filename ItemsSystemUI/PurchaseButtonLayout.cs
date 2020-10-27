using UnityEngine;

namespace FlappyBlooper
{
    public abstract class PurchaseButtonLayout : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private RudeButton button;
#pragma warning restore CS0649

        protected RudeButton Button => button;
        
        public abstract Product Product { set; }

        public abstract void UpdateButton();
    }
}