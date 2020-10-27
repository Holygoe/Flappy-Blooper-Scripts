using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class NoticeIcon : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Activate(bool value)
        {
            if (value)
            {
                gameObject.SetActive(true);
            }
            else if (gameObject.activeSelf)
            {
                animator.SetTrigger(Triggers.Deactivate);
            }
        }

        public void DeactivateViaAnimation()
        {
            gameObject.SetActive(false);
        }
    }
}