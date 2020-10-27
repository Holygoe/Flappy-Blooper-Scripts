using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class DebugSavingStatusPanel : MonoBehaviour
    {
        private const float FADE_DURATION = 3;

        public GameObject cloudUpdated;
        public GameObject cloudDisconnected;
        public GameObject cloudUpdating;
        public GameObject waiting;

        private float updatedCountdown;

        private void OnEnable()
        {
            UpdatePanel();
            DataManager.OnCloudStatusUpdated += GameDataManager_OnCloudStatusUpdated;
        }

        private void OnDisable()
        {
            DataManager.OnCloudStatusUpdated -= GameDataManager_OnCloudStatusUpdated;
        }

        private void GameDataManager_OnCloudStatusUpdated(object sender, System.EventArgs e)
        {
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            CloudStatus newStatus = DataManager.CloudStatus;
            bool waitingCloud = DataManager.WaitingForTheCloud;

            if (newStatus == CloudStatus.CloudUpdated && !waitingCloud)
            {
                updatedCountdown = FADE_DURATION;
            }

            cloudUpdated.SetActive(newStatus == CloudStatus.CloudUpdated && !waitingCloud);
            waiting.SetActive(newStatus == CloudStatus.CloudUpdated && waitingCloud);
            cloudDisconnected.SetActive(newStatus == CloudStatus.CloudDisconnected);
            cloudUpdating.SetActive(newStatus == CloudStatus.CloudUpdating);
        }

        private void Update()
        {
            if (updatedCountdown > 0)
            {
                updatedCountdown -= Time.unscaledDeltaTime;

                if (updatedCountdown <= 0)
                {
                    cloudUpdated.SetActive(false);
                }
            }
        }
    }
}
