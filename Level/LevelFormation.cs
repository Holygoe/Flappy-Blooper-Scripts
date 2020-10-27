using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FlappyBlooper
{
    public class LevelFormation : MonoBehaviour
    {
        private const float NormalScrollSpeed = 20f;
        private const float BoostedScrollSpeed = 40f;
        public const float LevelHeight = 128f;
        public const float DestroyXPosition = -50f;
        private const float SpawnXPosition = 50f;
        private const float SpawningPeriod = 0.9f;
        private const float CookieChance = 0.2f;
        private const float GreenGooChance = 0.01f;
        private const float LootGap = 8f;
        private const float MinFirefliesPeriod = 3f;
        private const float MaxFirefliesPeriod = 6f;
        private const float MaxCrystalSpawnChance = 0.07f;
        private static readonly int[] DifficultyThresholds = {5, 15, 30, 50, 100, 150, 200, 250, 300, 350};
        private static readonly int[] BubbleChanceStepLength = {5, 50, 100};
        private static readonly float[] BubbleChance = {0f, 0.05f, 0.03f, 0.015f}; 

#pragma warning disable CS0649
        [SerializeField] private ItemLoot lootCookiePrefab;
        [SerializeField] private ItemLoot lootMagicCrystalPrefab;
        [SerializeField] private ItemLoot lootGreenGooPrefab;
        [FormerlySerializedAs("lootBubbleFrefab")] [SerializeField]
        private Loot lootBubblePrefab;
        [SerializeField] private Barrier barrierPrefab;
        [SerializeField] private Transform fireflies;
#pragma warning restore CS0649

        private List<Transform> _scrolledObjects;
        private float _spawningCountdown;
        private bool _lootSpawned;
        private int _lootCookieNumber;
        private float _additionalCookieChance;
        private float _firefliesCountdown;
        private bool _boostedScrolling;
        private float _scrollSpeed;
        private int _barriersSpawnedCount;
        private bool _spawning;
        private int _bubbleChanceStep;

        private static LevelFormation Instance => Level.LevelFormation;
        public static float ScrollSpeed => Instance._scrollSpeed;

        private void Awake()
        {
            _spawning = true;
            _scrollSpeed = NormalScrollSpeed;
            _scrolledObjects = new List<Transform>();
            _firefliesCountdown = Random.Range(MinFirefliesPeriod, MaxFirefliesPeriod);
        }

        private void Start()
        {
            _spawningCountdown = SpawningPeriod;
            _lootCookieNumber = Character.TotalLevel / 10 + 1;
            _additionalCookieChance = Character.TotalLevel / 10.0f - _lootCookieNumber + 1;
        }

        private void Update()
        {
            // Регулирование скорости скроллинга.
            _scrollSpeed = Mathf.MoveTowards(_scrollSpeed, 
                _boostedScrolling? BoostedScrollSpeed : NormalScrollSpeed, Time.deltaTime * 15);

            // Размещение объектов уровня.
            if (Level.State == LevelState.Playing && _spawning) SpawningUpdate();

            // Скроллинг уровня.
            if (Level.Scroll) ScrollLevel();
        }

        public static Transform InstantiateScrolledObject(Transform newObject, Vector3 position, Quaternion rotation, float z = 1)
        {
            position.z = z;
            var newTransform = Instantiate(newObject, position, rotation, Instance.transform);
            Instance._scrolledObjects.Add(newTransform);
            return newTransform;
        }

        public static void DestroyScrolledObject(Transform transform)
        {
            Instance._scrolledObjects.Remove(transform);
            Destroy(transform.gameObject);
        }

        private void ScrollLevel()
        {   
            for (var i = 0; i < _scrolledObjects.Count; i++)
            {
                var scrolled = _scrolledObjects[i];
                var position = scrolled.position;
                position += Vector3.left * (_scrollSpeed * Time.deltaTime * position.z);
                scrolled.position = position;

                if (!(scrolled.position.x < DestroyXPosition)) continue;
                DestroyScrolledObject(scrolled);
                i--;
            }
        }

        private void SpawnBarrier()
        {
            Instantiate(barrierPrefab, new Vector3(SpawnXPosition, 0, 0), Quaternion.identity, transform);
        }

        private void SpawnLoot()
        {
            var spawnPositions = new List<Vector3>();

            // Магические кристаллы
            var magicCrystalChance = Mathf.Log((Level.Score - 200) * 0.0001f + 1) * 0.7f;
            magicCrystalChance = magicCrystalChance > MaxCrystalSpawnChance ? MaxCrystalSpawnChance : magicCrystalChance;
            if (Level.Score > 200 && Random.value < magicCrystalChance)
            {
                InstantiateScrolledObject(lootMagicCrystalPrefab.transform, GetRandomSpawnPosition(ref spawnPositions), Quaternion.identity);
            }

            // Печеньки
            if (Level.Score > 20 && Random.value < CookieChance)
            {
                var number = _lootCookieNumber;
                if (Random.value < _additionalCookieChance)
                {
                    number++;
                }
                lootCookiePrefab.number = number;
                InstantiateScrolledObject(lootCookiePrefab.transform, GetRandomSpawnPosition(ref spawnPositions), Quaternion.identity);
            }
            
            if (Game.Mode == GameMode.Story) return;
            
            // Зеленая жижа
            var greenGooChance = GreenGooChance / Mathf.Pow(2, Game.GameData.greenGooCollectedTodayCount);
            if (Random.value < greenGooChance)
            {
                InstantiateScrolledObject(lootGreenGooPrefab.transform, GetRandomSpawnPosition(ref spawnPositions), Quaternion.identity);
            }

            // Шары
            if (BubbleChanceStepLength.Length > _bubbleChanceStep &&
                BubbleChanceStepLength[_bubbleChanceStep] < _barriersSpawnedCount)
            {
                _bubbleChanceStep++;
                Debug.Log($"Chance is changed to {BubbleChance[_bubbleChanceStep]}");
            }
            
            if (Level.Score > 20 && Random.value < BubbleChance[_bubbleChanceStep])
                InstantiateScrolledObject(lootBubblePrefab.transform,
                    GetRandomSpawnPosition(ref spawnPositions), Quaternion.identity);
        }

        private void SpawningUpdate()
        {
            _spawningCountdown -= Time.deltaTime * _scrollSpeed / NormalScrollSpeed;
            if (_spawningCountdown < SpawningPeriod)
            {
                _spawningCountdown += SpawningPeriod;

                if (!_lootSpawned)
                {
                    SpawnLoot();
                    _lootSpawned = true;
                }
                else
                {
                    SpawnBarrier();
                    _lootSpawned = false;
                    _barriersSpawnedCount++;

                    switch (Game.Mode)
                    {
                        case GameMode.Rating when DifficultyThresholds.Length > Level.Difficulty 
                                                  && DifficultyThresholds[Level.Difficulty] < _barriersSpawnedCount:
                            if (Level.Difficulty < Stage.MaxDifficulty)
                            {
                                Level.Difficulty++;
                            }
                            break;
                        case GameMode.Story when _barriersSpawnedCount >= Level.StageLength:
                            _spawning = false;
                            break;
                    }
                }
            }

            _firefliesCountdown -= Time.deltaTime;
            if (_firefliesCountdown >= 0) return;
            _firefliesCountdown += Random.Range(MinFirefliesPeriod, MaxFirefliesPeriod);
            InstantiateScrolledObject(fireflies, 
                new Vector3(SpawnXPosition, fireflies.position.y, 0), Quaternion.identity, z:0.7f);
        }

        private static Vector3 GetRandomSpawnPosition(ref List<Vector3> spawnPositions)
        {
            Vector3 newPosition;

            var count = spawnPositions.Count;
            if (count == 0)
            {
                newPosition = new Vector3(SpawnXPosition,
                    Random.Range(-Level.RealCameraSize * 0.3f, Level.RealCameraSize * 0.3f), 0);
            }
            else
            {
                var sign =  - Mathf.Sign(spawnPositions[0].y);
                var lastPosition = spawnPositions[count - 1];
                newPosition = lastPosition + new Vector3(0, sign, 0) * LootGap;
            }

            spawnPositions.Add(newPosition);
            return newPosition;
        }

        public static void BoostScrolling(bool value)
        {
            Instance._boostedScrolling = value;
        }
    }
}