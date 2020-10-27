using System;
using System.Linq;
using UnityEngine;

namespace FlappyBlooper
{
    public class NoticeHandler : MonoBehaviour
    {
        public NoticeIcon notice;
        public NoticeTag[] tags;

        private void Awake()
        {
            if (notice) return;
            var nTransform = transform.Find("Notice");
            if (!nTransform) nTransform = transform.Find("Background").transform.Find("Notice");
            notice = nTransform.GetComponent<NoticeIcon>();
        }

        private void OnEnable()
        {
            UpdateNotice();
            Notice.OnAnyNoticeStatusChanged += WhenNoticeStatusChanged;
        }

        private void OnDisable()
        {
            Notice.OnAnyNoticeStatusChanged -= WhenNoticeStatusChanged;
        }

        private void WhenNoticeStatusChanged(object sender, EventArgs e)
        {
            UpdateNotice();
        }

        private void UpdateNotice()
        {
            var status = tags.Aggregate(false, (current, noticeTag) => current | NoticeMaster.GetStatus(noticeTag));
            notice.Activate(status);
        }
    }
}