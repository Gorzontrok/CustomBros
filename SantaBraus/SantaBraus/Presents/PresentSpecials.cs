using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SantaBrausMod.Presents
{
    public class GrenadePresent : IPresentSpecial
    {
        public Material Icon { get; set; }

        protected Grenade _grenade;

        public GrenadePresent(Material icon, Grenade grenade)
        {
            Icon = icon;
            _grenade = grenade;
        }

        public void Unwrap(TestVanDammeAnim character)
        {
            if (character.NotAs<SantaBraus>())
            {
                return;
            }
            SantaBraus santaBraus = character as SantaBraus;
            santaBraus.AddSpecialPresent(this);
        }

        public virtual bool UseSpecial(TestVanDammeAnim character)
        {
            character.PlayThrowLightSound(0.4f);
            Common.TrowGrenade(character, _grenade);
            return false;
        }
    }

    public class GrenadeHologramPresent : GrenadePresent, IDamageEvent
    {
        private GrenadeHologram _thrownGrenade;

        public GrenadeHologramPresent(Material icon, Grenade grenade) : base(icon, grenade)
        { }

        private void TeleportToGrenade(TestVanDammeAnim character)
        {
            character.SetXY(_thrownGrenade.X, _thrownGrenade.Y);
            character.xIBlast = (character.yIBlast = (character.xI = (character.yI = 0f)));
            character.PlaySpecial2Sound(0.4f, 1f);
            _thrownGrenade.Death();
            character.SetInvulnerable(0.1f, true, true);
        }

        public override bool UseSpecial(TestVanDammeAnim character)
        {
            if (_thrownGrenade == null)
            {
                character.PlayThrowLightSound(0.4f);
                _thrownGrenade = Common.TrowGrenade(character, _grenade) as GrenadeHologram;
                return true;
            }
            TeleportToGrenade(character);

            return false;
        }

        public bool DamageEvent(TestVanDammeAnim character, int damage, DamageType damageType, float xI, float yI, int direction, MonoBehaviour damageSender, float hitX, float hitY)
        {
            if (_thrownGrenade != null)
            {
                TeleportToGrenade(character);
                return true;
            }
            return false;
        }
    }
}
