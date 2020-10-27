using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public abstract class Loot : MonoBehaviour
    {
        public ParticleSystem pickUpVFX;
        public Material pickupVfxMaterial;

        public bool Collected { get; private set; }

        protected abstract void Apply();
        protected abstract int GetEmitParticlesNumber();

        public void PickUp()
        {
            Collected = true;
            pickUpVFX.GetComponent<ParticleSystemRenderer>().material = pickupVfxMaterial;
            ParticleSystem clone = LevelFormation.InstantiateScrolledObject(pickUpVFX.transform, transform.position, Quaternion.identity)
                .GetComponent<ParticleSystem>();
            clone.Emit(GetEmitParticlesNumber());
            Apply();
            LevelFormation.DestroyScrolledObject(transform);
        }
    }
}