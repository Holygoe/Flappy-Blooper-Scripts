using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public abstract class SingeltonBase : MonoBehaviour
    {
        public abstract void InitializeInstance();
        public abstract bool Exist { get; }
    }
}