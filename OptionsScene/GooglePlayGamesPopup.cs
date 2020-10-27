using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper.OptionsScene
{
    public class GooglePlayGamesPopup : MonoBehaviour
    {
        public GameObject whileSignedIn;
        public GameObject whileSignedOut;

        public Button signOutButton;
        public Button okayButton;
        public Button cancelButton;
        public Button signInButton;

        public void Awake()
        {
            signOutButton.onClick.AddListener(() => { SocialHandler.SignOutUser(); gameObject.SetActive(false); });
            okayButton.onClick.AddListener(() => gameObject.SetActive(false));
            cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
            signInButton.onClick.AddListener(() => { SocialHandler.SignInUser(); gameObject.SetActive(false); });
        }

        public void OnEnable()
        {
            whileSignedIn.SetActive(SocialHandler.SignedIn);
            whileSignedOut.SetActive(!SocialHandler.SignedIn);
        }
    }
}