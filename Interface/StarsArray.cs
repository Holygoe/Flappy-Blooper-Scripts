using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class StarsArray : MonoBehaviour
    {
        public Image[] stars;

        public void UpdateProgress(int value)
        {
            int length = stars.Length;

            if (value > length)
            {
                value = length;
            }

            for (int i = 0; i < value; i++)
            {
                stars[i].sprite = Assets.Instance.starOn;
            }

            for (int i = value; i < length; i++)
            {
                stars[i].sprite = Assets.Instance.starOff;
            }
        }
    }
}