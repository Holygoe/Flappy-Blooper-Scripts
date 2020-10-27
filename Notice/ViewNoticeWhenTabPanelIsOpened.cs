using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class ViewNoticeWhenTabPanelIsOpened : MonoBehaviour, IWhenTabPanelIsOpened
    {
        public NoticeTag noticeTag;

        public void WhenTabPanelIsOpened()
        {
            NoticeMaster.GetNotice(noticeTag).View();
        }
    }
}