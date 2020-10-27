using System;
using System.Linq;

namespace FlappyBlooper
{
    public abstract class Notice
    {
        private bool _lastStatus;

        protected Notice()
        {
            _lastStatus = Status;
        }

        private NoticeData NoticeData => NoticeData.FindData(Game.GameData.noticeDataset, Tag);
        public bool Status => NoticeData.topical && !NoticeData.viewed;
        protected abstract NoticeTag Tag { get; }
        protected abstract bool CurrentTopical { get; }
        
        public static event EventHandler OnAnyNoticeStatusChanged;

        public void View(bool saveGame = true)
        {
            NoticeData.viewed = true;
            OnAnyNoticeStatusChanged?.Invoke(null, EventArgs.Empty);
            if (saveGame) DataManager.SaveData();
        }

        public void ResetNotice(bool saveGame = true)
        {
            var noticeData = NoticeData;
            noticeData.viewed = false;
            noticeData.topical = CurrentTopical;
            OnAnyNoticeStatusChanged?.Invoke(null, EventArgs.Empty);
            if (saveGame) DataManager.SaveData();
        }

        public void UpdateStatus()
        {
            var noticeData = NoticeData;
            var changed = false;
            var currentTopical = CurrentTopical;

            if (currentTopical && !noticeData.topical)
            {
                noticeData.viewed = false;
                changed = true;
            }

            if (noticeData.topical != currentTopical)
            {
                noticeData.topical = currentTopical;
                changed = true;
            }

            if (changed) DataManager.SaveData();
            var currentStatus = Status;
            if (_lastStatus == currentStatus) return;
            _lastStatus = currentStatus;
            OnAnyNoticeStatusChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    public enum NoticeTag
    {
        Achievements,
        StoreOffers,
        Cloud
    }

    public class AchievementsNotice : Notice
    {
        protected override NoticeTag Tag => NoticeTag.Achievements;

        protected override bool CurrentTopical
        {
            get
            {
                return AssetTag.Achievements.ToAsset().GetItems<Achievement>().Aggregate(false,
                    (current, achievement) => current | (achievement.Status == AchievementStatus.Topical));
            }
        }
    }

    public class StoreOfferNotice : Notice
    {
        protected override NoticeTag Tag => NoticeTag.StoreOffers;

        protected override bool CurrentTopical => true;
    }
    
    public class CloudNotice : Notice
    {
        protected override NoticeTag Tag => NoticeTag.Cloud;

        protected override bool CurrentTopical => !SocialHandler.SignedIn;
    }
}