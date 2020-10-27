using UnityEngine;

namespace FlappyBlooper
{
    public class Barrier : MonoBehaviour
    {
        private const float MinGapSize = 26f;
        private const float MaxGapSize = 35f;
        private const float TopGapLimit = 20f;
        private const float BottomGapLimit = 25f;
        private const float BottomLevelPosition = -LevelFormation.LevelHeight / 2;
        private const float TopLevelPosition = LevelFormation.LevelHeight / 2;


#pragma warning disable CS0649
        [SerializeField] private Transform topHead;
        [SerializeField] private Transform topBody;
        [SerializeField] private Transform bottomHead;
        [SerializeField] private Transform bottomBody;
#pragma warning restore CS0649

        private static float _gapSize;
        private SoundHandler _soundHandler;

        public static void UpdateDifficulty(int difficult)
        {
            const float step = (MaxGapSize - MinGapSize) / (Stage.MaxDifficulty + 1);
            _gapSize = MinGapSize + step * (Stage.MaxDifficulty + 1 - difficult);
        }

        private void Awake()
        {
            _soundHandler = GetComponent<SoundHandler>();
            var gapPosition = Random.Range(BottomLevelPosition + BottomGapLimit + _gapSize / 2, TopLevelPosition - TopGapLimit - _gapSize / 2);
            var halfGapHeight = _gapSize / 2;
            var bottomBarrierHeight = gapPosition - halfGapHeight - BottomLevelPosition;
            var topBarrierHeight = TopLevelPosition - gapPosition - halfGapHeight;

            //Top Head
            var topHeadSize = topHead.GetComponent<SpriteRenderer>().size;
            topHead.localPosition = new Vector3(0, gapPosition + halfGapHeight + topHeadSize.y / 2);

            //Bottom Head
            var bottomHeadSize = bottomHead.GetComponent<SpriteRenderer>().size;
            bottomHead.localPosition = new Vector3(0, gapPosition - halfGapHeight - bottomHeadSize.y / 2);

            //Top Body
            var topBodySprite = topBody.GetComponent<SpriteRenderer>();
            topBody.localPosition = new Vector3(0, TopLevelPosition - topBarrierHeight + topHeadSize.y);
            topBodySprite.size = new Vector2(topBodySprite.size.x, topBarrierHeight - topHeadSize.y);
            var topBodyCollider = topBody.GetComponent<BoxCollider2D>();
            var topBodySpriteSize = topBodySprite.size;
            topBodyCollider.size = topBodySpriteSize;
            topBodyCollider.offset = new Vector2(topBodyCollider.offset.x, -topBodySpriteSize.y / 2);

            //Bottom Body
            var bottomBodySprite = bottomBody.GetComponent<SpriteRenderer>();
            bottomBody.localPosition = new Vector3(0, BottomLevelPosition + bottomBarrierHeight - bottomHeadSize.y);
            bottomBodySprite.size = new Vector2(bottomBodySprite.size.x, bottomBarrierHeight - bottomHeadSize.y);
            var bottomBodyCollider = bottomBody.GetComponent<BoxCollider2D>();
            var bottomBodySpriteSize = bottomBodySprite.size;
            bottomBodyCollider.size = bottomBodySpriteSize;
            bottomBodyCollider.offset = new Vector2(bottomBodyCollider.offset.x, -bottomBodySpriteSize.y / 2);
        }

        private void Update()
        {
            if (!Level.Scroll) return;
            var thisTransform = transform;
            var position = thisTransform.position;
            var itIsToTheRightOfThePlayer = position.x > Player.Instance.transform.position.x;

            position += Vector3.left * (LevelFormation.ScrollSpeed * Time.deltaTime);
            thisTransform.position = position;

            if (itIsToTheRightOfThePlayer && transform.position.x <= Player.Instance.transform.position.x)
            {
                Level.CountPassedBarrier();
                _soundHandler.PlayOneShot();
            }

            if (transform.position.x < LevelFormation.DestroyXPosition) Destroy(gameObject);
        }
    }
}