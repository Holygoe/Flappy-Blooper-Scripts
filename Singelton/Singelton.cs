using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public abstract class Singelton<T> : SingeltonBase where T : Component
    {
        public static T Instance { get; private set; }

        public sealed override void InitializeInstance()
        {
            Instance = this as T;
            (Instance as ISingeltonInitializeHandler)?.Initialize();
        }

        public sealed override bool Exist
        {
            get
            {
                if (Instance)
                {
                    if (Instance != this)
                    {
                        throw new System.Exception("Find more than one instance of " + typeof(T));
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}