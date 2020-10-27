using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlappyBlooper
{
    public class SpriteRendererArray : MonoBehaviour
    {
        private SpriteRenderer[] renderers;

        private void Awake()
        {
            renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        }

        public void ChangeMaterial(Material material)
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.material = material;
            }
        }
    }
}