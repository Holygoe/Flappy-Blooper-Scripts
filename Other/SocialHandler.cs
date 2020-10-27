using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class SocialHandler : Singelton<SocialHandler>
    {
        private const string PREFS_SIGNED_IN = "SocialSignedIn";

        private StoredBool signedIn;
        public static bool SignedIn { get => Instance.signedIn.Value; private set => Instance.signedIn.Value = value; }
        public static bool TryingToSignIn { get; private set; }

        public static event System.EventHandler OnUserStatusChanged;
        public static event System.EventHandler OnTryingToSignIn;

        private void Awake()
        {
            signedIn = new StoredBool(PREFS_SIGNED_IN, false);

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();

            if (SignedIn)
            {
                SignInUser();
            }
        }

        public static void SignInUser()
        {
            TryingToSignIn = true;
            OnTryingToSignIn?.Invoke(Instance, System.EventArgs.Empty);
            Social.localUser.Authenticate((bool success) =>
            {
                TryingToSignIn = false;
                SignedIn = success;
                OnUserStatusChanged?.Invoke(Instance, System.EventArgs.Empty);
            });
        }

        public static void SignOutUser()
        {
            SignedIn = false;
            PlayGamesPlatform.Instance.SignOut();
            OnUserStatusChanged?.Invoke(Instance, System.EventArgs.Empty);
        }
    }
}