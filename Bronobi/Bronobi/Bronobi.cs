using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;
using UnityEngine;

namespace BronobiMod
{
    [HeroPreset("Bronobi", HeroType.Blade)]
    public class Bronobi : SwordHero
    {
        protected BronobiForceWave forceWave;
        protected Texture2D gunGrabSprite;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void SetupThrownMookVelocity(out float XI, out float YI)
        {
            base.SetupThrownMookVelocity(out XI, out YI);
            XI *= 1.2f;
            YI *= 1.2f;
        }
    }
}
