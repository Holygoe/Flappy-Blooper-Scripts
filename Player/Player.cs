using EZCameraShake;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBlooper
{
    public class Player : Singelton<Player>
    {
        private const int FlapAmount = 90;
        private const float AngularVelocity = 10f;
        private const float AfterHitInvulnerableTime = 3f;
        private const float AfterHitDazedTime = 0.3f;
        private const float BubblingTime = 4f;
        private const float AfterBubblingTime = 2f;
        private const float FlappingInterval = 0.1f;

        [FormerlySerializedAs("blibkinMaterial")] public Material blinkingMaterial;
        public Material defaultMaterial;
        public Animator animator;
        [FormerlySerializedAs("hitVFXPrefab")] public Transform hitVfxPrefab;
        public SpriteRendererArray renderers;
        public PlayerLook look;
        public Bubble bubble;

        private Rigidbody2D _rb;
        private State _state;
        private SoundHandler _flapSoundHandler;

        private int _maxLifeNumber;
        private int _maxHealthPotionNumber;
        private int _lifeNumber;
        private int _healthPotionNumber;
        private float _flapTime;
        private float _invulnerableTime;
        private bool _bubbled;
        private int _accuracy;
        private bool _interactable;
        private static readonly int FlapTrigger = Animator.StringToHash("Flap");
        private static readonly int WaitTrigger = Animator.StringToHash("Wait");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");

        public event EventHandler OnDied;
        public event EventHandler OnRanOutOfLives;
        public event EventHandler OnStartedPlaying;
        public static event EventHandler OnPlayerUpdated;

        public static int Accuracy => Instance._accuracy;
        
        public static int LivesNumber => Instance._lifeNumber;

        public static int HealthPotionsNumber
        {
            get => Instance._healthPotionNumber;
            private set
            {
                var consumablesNumber = ConsumableTag.HealthPotions.ToConsumable().Stock;
                Instance._healthPotionNumber = value > consumablesNumber ? consumablesNumber : value;
            }
        }

        private void Awake()
        {
            Character character = Character.Current;
            _maxLifeNumber = character.LifeNumber;
            _maxHealthPotionNumber = character.HealthPotionNumber;
            _state = State.Awaiting;
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Static;
            _flapSoundHandler = GetComponent<SoundHandler>();
            _accuracy = 3;
            _interactable = true;

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            _lifeNumber = _maxLifeNumber;
            HealthPotionsNumber = _maxHealthPotionNumber;
        }

        private void OnEnable()
        {
            Level.OnLevelStateChanged += Level_OnStateChanged;
        }

        private void OnDisable()
        {
            Level.OnLevelStateChanged -= Level_OnStateChanged;
        }

        private void Level_OnStateChanged(object sender, EventArgs e)
        {
            if (Level.State == LevelState.Paused || Level.State == LevelState.GameOver)
            {
                _rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }

            _interactable = Level.State == LevelState.Playing || Level.State == LevelState.StageComplete;
        }

        private void Update()
        {   
            switch (_state)
            {
                case State.Awaiting:
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    {
                        OnStartedPlaying?.Invoke(this, EventArgs.Empty);
                        _state = State.Playing;
                        _rb.bodyType = RigidbodyType2D.Dynamic;
                        Flap();
                    }
                    break;

                case State.Playing:
                case State.Invulnerable:
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && _interactable)
                    {
                        Flap();
                    }
                    PlaygroundControl();
                    break;

                case State.Dazed:
                    if (_rb.bodyType == RigidbodyType2D.Dynamic)
                    {
                        PlaygroundControl();
                    }
                    break;
            }

            float z = Mathf.LerpAngle(transform.eulerAngles.z, _rb.velocity.y * 0.15f, Time.deltaTime * AngularVelocity);
            transform.eulerAngles = new Vector3(0, 0, z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_state == State.Dazed)
            {
                return;
            }

            switch (collision.tag)
            {
                case "Enemy":

                    if (_state == State.Invulnerable)
                    {
                        break;
                    }
                    StartCoroutine(HitAsync(transform.position));
                    break;

                case "PickUpItem":
                    var loot = collision.gameObject.GetComponent<Loot>();
                    if (loot != null && !loot.Collected)
                    {
                        loot.PickUp();
                    }
                    break;
            }
        }

        public static void UpdatePlayer(bool entryTrigger = false)
        {
            if (Instance)
            {
                Instance.look.UpdateLook();
            }

            if (entryTrigger)
            {
                Instance.animator.SetTrigger(Triggers.Entry);
            }

            OnPlayerUpdated?.Invoke(null, EventArgs.Empty);
        }

        public void TouchPlayer()
        {
            Instance.animator.SetTrigger(Triggers.Touch);
        }

        private void Flap()
        {
            if (Time.time < _flapTime)
            {
                return;
            }

            _flapTime = Time.time + FlappingInterval;

            if (_state != State.Dazed)
            {
                animator.SetTrigger(FlapTrigger);
            }
            _rb.velocity = Vector2.up * FlapAmount;
            _flapSoundHandler.PlayOneShot();
        }

        private void PlaygroundControl()
        {
            if (transform.position.y > Level.RealCameraSize)
            {
                _rb.MovePosition(new Vector2(transform.position.x, Level.RealCameraSize));
                _rb.velocity = Vector2.zero;
            }
            else if (transform.position.y < -Level.RealCameraSize + 5)
            {
                Flap();
            }
        }

        private IEnumerator InvulnerableCountdownAsync()
        {
            do
            {
                yield return null;
                _invulnerableTime -= Time.deltaTime;
            }
            while (_invulnerableTime >= 0);
            
            StopInvulnerable();

            if (_bubbled)
            {
                StopBubbling();
            }
        }

        private void StopInvulnerable()
        {
            _state = State.Playing;
            renderers.ChangeMaterial(defaultMaterial);
        }

        private void StartInvulnerable(float time)
        {
            look.UpdateFace(false);
            _state = State.Invulnerable;
            animator.SetTrigger(WaitTrigger);
            renderers.ChangeMaterial(blinkingMaterial);
            if (_invulnerableTime > 0)
            {
                _invulnerableTime = _invulnerableTime > time ? _invulnerableTime : time;
            }
            else
            {
                _invulnerableTime = time;
                StartCoroutine(InvulnerableCountdownAsync());
            }
        }

        private void Daze()
        {
            look.UpdateFace(true);
            _state = State.Dazed;
            animator.SetTrigger(HitTrigger);
        }

        private IEnumerator HitAsync(Vector3 hitPosition)
        {
            CameraShaker.Instance.ShakeOnce(15, 5, 0.2f, 0.5f);
            LevelFormation.InstantiateScrolledObject(hitVfxPrefab, hitPosition, Quaternion.identity);

            if (_lifeNumber > 0)
            {
                if (_accuracy > 2)
                {
                    _accuracy = 2;
                }

                _lifeNumber--;
                Daze();
                yield return new WaitForSeconds(AfterHitDazedTime);
                StartInvulnerable(AfterHitInvulnerableTime);
            }
            else
            {
                _accuracy = 1;
                Daze();
                OnRanOutOfLives?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerator ContinueToPlayLevelAsync(bool useGreenGoo)
        {
            if (useGreenGoo)
            {
                ConsumableTag.GreenGoo.ToConsumable().TryToRemove(1, true);
            }
            else
            {
                HealthPotionsNumber--;
                ConsumableTag.HealthPotions.ToConsumable().TryToRemove(1, true);
            }

            Daze();
            yield return new WaitForSeconds(AfterHitDazedTime);
            StartInvulnerable(AfterHitInvulnerableTime);
        }

        public static void Die()
        {
            Instance.Daze();
            Instance.OnDied?.Invoke(Instance, EventArgs.Empty);
        }

        public void StartBubbling()
        {
            bubble.Enable();
            _bubbled = true;
            LevelFormation.BoostScrolling(true);
            StartInvulnerable(BubblingTime);
            
        }

        private void StopBubbling()
        {
            LevelFormation.BoostScrolling(false);
            _bubbled = false;
            bubble.Animator.SetTrigger(Triggers.Burst);
            StartInvulnerable(AfterBubblingTime);
        }

        private enum State { Awaiting, Playing, Dazed, Invulnerable };
    }
}