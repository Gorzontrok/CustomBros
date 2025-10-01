using System;
using SantaBrausMod.Presents;
using System.Collections.Generic;
using BroMakerLib.Loggers;
using BroMakerLib;

namespace SantaBrausMod
{
    public class PresentsBag
    {
        public List<IPresent> Presents { get; private set; }

        protected TestVanDammeAnim _character;

        public void AddPresent(IPresent present)
        {
            Presents.Add(present);
        }

        public void SpawnWrappedPresent()
        {
            WrappedPresent wrappedPresent = WrappedPresent.CreateWrappedPresent(_character, _character.X, _character.Y, _character.playerNum, _character.gameObject.layer);
            if (wrappedPresent == null)
                return;
            wrappedPresent.AssignPresent(GetRandomPresent());
            wrappedPresent.Launch(_character.X, _character.Y, (UnityEngine.Random.value > 0.5f ? -0.5f : 0.5f), 1f);
        }

        public IPresent GetRandomPresent()
        {
            return Presents.RandomElement();
        }

        public void Initialize(TestVanDammeAnim character)
        {
            _character = character;
            try
            {
                Presents = new List<IPresent>()
                {
                    // Grenades
                   //SpecialPresents.HeroGrenade, SpecialPresents.FireGrenade, SpecialPresents.HolyWaterBottle, SpecialPresents.StickyGrenade,
                   //SpecialPresents.FlashGrenade, SpecialPresents.FrozeGrenade, SpecialPresents.GrenadeColJamesBroddock, SpecialPresents.Molotov,
                   SpecialPresents.TankSummoner, SpecialPresents.Martini, SpecialPresents.Hologram,
                    // Pocketteds
                   // SpecialPresents.Airstrike, SpecialPresents.MechDrop, SpecialPresents.AlienPheromones,

                    // Guns
                    GunPresents.AssaultRifle,
                    GunPresents.RocketLauncher

                };

               
                //
            }
            catch (Exception ex)
            {
                BMLogger.ExceptionLog(ex);
            }
        }
    }
}
