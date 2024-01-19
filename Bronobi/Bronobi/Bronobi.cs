using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;

namespace BronobiMod
{
    [HeroPreset("Bronobi", HeroType.Blade)]
    public class Bronobi : SwordHero
    {
        protected BronobiForceWave forceWave;


        protected override void SetupThrownMookVelocity(out float XI, out float YI)
        {
            base.SetupThrownMookVelocity(out XI, out YI);
            XI *= 1.2f;
            YI *= 1.2f;
        }
    }
}
