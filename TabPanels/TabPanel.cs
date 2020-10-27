using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class TabPanel : MonoBehaviour
    {
        public Button selectButton;
        public MonoBehaviour tabPanelOnOpened;

        public void Initiate(TabPanelController controller, int index)
        {
            if (selectButton)
            {
                selectButton.onClick.AddListener(() => controller.SelectTab(index));
            }
        }

        public void SetActive(bool value)
        {
            if (selectButton)
            {
                selectButton.interactable = !value;
            }

            gameObject.SetActive(value);

            if (value && tabPanelOnOpened is IWhenTabPanelIsOpened panel)
            {
                panel.WhenTabPanelIsOpened();
            }
        }
    }
}