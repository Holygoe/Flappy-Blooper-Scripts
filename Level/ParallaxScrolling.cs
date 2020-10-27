using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class ParallaxScrolling : MonoBehaviour
    {
        public float scrollFactor = 1;
        public float tileSize = 102.4f;
        public int queue = 0;
        public float startYOffset = 0;
        public bool ignoresLevelStatus;
        private float extremXPosition;

        public void Start()
        {
            extremXPosition = -tileSize;
            Vector3 position = transform.position;
            position.x = Random.Range(-tileSize, 0);
            transform.position = position;
        }

        private void Update()
        {
            if (ignoresLevelStatus || Level.Scroll)
            {
                float xPosition = transform.position.x - Time.deltaTime * LevelFormation.ScrollSpeed * scrollFactor;

                if (xPosition < extremXPosition)
                {
                    xPosition += tileSize;
                }

                transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
            }
        }
    }
}