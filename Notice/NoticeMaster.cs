using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class NoticeMaster : Singelton<NoticeMaster>
    {
        private Dictionary<NoticeTag, Notice> _notices;

        private void Awake()
        {
            _notices = new Dictionary<NoticeTag, Notice>
            {
                { NoticeTag.Achievements, new AchievementsNotice() },
                { NoticeTag.StoreOffers, new StoreOfferNotice() },
                { NoticeTag.Cloud, new CloudNotice() },
            };

            // Achievement
            Achievement.OnAnyAchievementUpdated += (sender, e) => _notices[NoticeTag.Achievements].UpdateStatus();

            // StoreOffer
            StartCoroutine(RareUpdateAsync());

            // Social
            SocialHandler.OnUserStatusChanged += (sender, e) => _notices[NoticeTag.Cloud].UpdateStatus();
        }

        private void Start()
        {
            _notices[NoticeTag.Cloud].UpdateStatus();
        }

        private IEnumerator RareUpdateAsync()
        {
            while (true)
            {
                if (LimitedOfferMaster.IsOfferLineOutdated(OfferMasterTag.Store))
                {
                    _notices[NoticeTag.StoreOffers].ResetNotice();
                }
                yield return new WaitForSecondsRealtime(30);
            }
        }

        public static Notice GetNotice(NoticeTag tag)
        {
            return Instance._notices[tag];
        }

        public static bool GetStatus(NoticeTag tag)
        {
            return Instance._notices[tag].Status;
        }
    }
}