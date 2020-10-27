using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlappyBlooper
{
    public class SingeltonMaster : MonoBehaviour
    {
        public List<SingeltonBase> singeltons;

        private void Awake()
        {
            singeltons = new List<SingeltonBase>();

            foreach(SingeltonBase singelton in Resources.FindObjectsOfTypeAll<SingeltonBase>())
            {
                if (singelton.hideFlags == HideFlags.NotEditable || singelton.hideFlags == HideFlags.HideAndDontSave)
                {
                    continue;
                }

#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(singelton))
                {
                    continue;
                }
#endif

                if (singelton.Exist)
                {
                    continue;
                }

                singelton.InitializeInstance();
                singeltons.Add(singelton);
            }
        }
    }
}