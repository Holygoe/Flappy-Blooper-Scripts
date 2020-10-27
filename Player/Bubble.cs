using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FlappyBlooper
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDisabled = new UnityEvent();

        public Animator Animator { get; private set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Animator.SetTrigger(Triggers.Bubbling);
        }

        public void Enable()
        {
            if (gameObject.activeSelf)
            {
                Animator.SetTrigger(Triggers.Bubbling);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void Disable()
        {
            onDisabled.Invoke();
            gameObject.SetActive(false); 
        }
    }
}