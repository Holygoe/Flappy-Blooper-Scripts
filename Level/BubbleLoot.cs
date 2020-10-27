using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class BubbleLoot : Loot
    {
        protected override void Apply()
        {
            Player.Instance.StartBubbling();
        }

        protected override int GetEmitParticlesNumber()
        {
            return 1;
        }
    }
}