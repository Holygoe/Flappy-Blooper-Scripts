using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper.OptionsScene
{
    public class SocialPanel : MonoBehaviour
    {
        public Button mainButton;
        public GameObject whileSignedInIcon;
        public GameObject whileSignedOutIcon;
        public GameObject waitingIcon;
        public Button achievementsButton;
        public Button leaderboardButton;
        public PopupWindow socialPopup;

        private void Start()
        {
            mainButton.onClick.AddListener(() => socialPopup.gameObject.SetActive(true));
            leaderboardButton.onClick.AddListener(() => Social.ShowLeaderboardUI());
            UpdateCanvas();
        }

        private void OnEnable()
        {
            SocialHandler.OnUserStatusChanged += SocialHandler_OnUserStatusChanged;
            SocialHandler.OnTryingToSignIn += SocialHandler_OnTryingToSignIn;
        }

        private void OnDisable()
        {
            SocialHandler.OnUserStatusChanged -= SocialHandler_OnUserStatusChanged;
            SocialHandler.OnTryingToSignIn -= SocialHandler_OnTryingToSignIn;
        }

        private void SocialHandler_OnTryingToSignIn(object sender, System.EventArgs e)
        {
            UpdateCanvas();
        }

        private void SocialHandler_OnUserStatusChanged(object sender, System.EventArgs e)
        {
            UpdateCanvas();
        }

        private void UpdateCanvas()
        {
            mainButton.interactable = !SocialHandler.TryingToSignIn;
            waitingIcon.SetActive(SocialHandler.TryingToSignIn);
            whileSignedInIcon.SetActive(!SocialHandler.TryingToSignIn && SocialHandler.SignedIn);
            whileSignedOutIcon.SetActive(!SocialHandler.TryingToSignIn && !SocialHandler.SignedIn);
            achievementsButton.interactable = false;
            leaderboardButton.interactable = SocialHandler.SignedIn;
        }
    }
}