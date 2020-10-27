using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class TabPanelController : MonoBehaviour
    {
        public TabPanel[] panels;
        private int indexOfOpenedPanel;

        private void Awake()
        {
            indexOfOpenedPanel = 0;
            
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Initiate(this, i);
            }
        }

        private void OnEnable()
        {
            UpdatePanel();
        }

        public void SelectTab(int index)
        {
            indexOfOpenedPanel = index;
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(i == indexOfOpenedPanel);
            }
        }
    }
}