using System;
using System.Collections.Generic;
using System.Linq;
using BroMakerLib;
using UnityEngine;

namespace SantaBrausMod.Presents
{
    public interface IPresent
    {
        void Unwrap(TestVanDammeAnim character);
    }

    public interface IPresentGun : IPresent
    {
        Projectile Projectile { get; }
        GunTextureInfo GunAppearance { get; }
        float FireDelay { get; }

        void FireWeapon(float x, float y, float xSpeed, float ySpeed);
    }

    public interface IPresentSpecial : IPresent
    {
        Material Icon {  get; }
        bool UseSpecial(TestVanDammeAnim character);
    }

    public interface IGunMagazine
    {
        int AmmoLeft { get; }

        void Refill();
        bool IsAmmoFull();
    }

    public interface IDamageEvent
    {
        bool DamageEvent(TestVanDammeAnim character, int damage, DamageType damageType, float xI, float yI, int direction, MonoBehaviour damageSender, float hitX, float hitY);
    }

    public static class SpecialPresents
    {
        // Grenades
        public static GrenadePresent HeroGrenade = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:Rambro"), HeroController.GetHeroPrefab(HeroType.Rambro).specialGrenade);
        public static GrenadePresent FireGrenade = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:BABroracus"), HeroController.GetHeroPrefab(HeroType.BaBroracus).specialGrenade);
        public static GrenadePresent HolyWaterBottle = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:Broffy"), HeroController.GetHeroPrefab(HeroType.Broffy).specialGrenade);
        public static GrenadePresent StickyGrenade = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:BroneyRoss"), HeroController.GetHeroPrefab(HeroType.BroneyRoss).specialGrenade);
        public static GrenadePresent FlashGrenade = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:BroHard"), HeroController.GetHeroPrefab(HeroType.BroHard).specialGrenade);
        public static GrenadePresent FrozeGrenade = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:DemolitionBroSpecialAmmoIcon"), HeroController.GetHeroPrefab(HeroType.DemolitionBro).specialGrenade);
        public static GrenadePresent GrenadeColJamesBroddock = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:ColJamesBroddock"), HeroController.GetHeroPrefab(HeroType.ColJamesBroddock).specialGrenade);
        public static GrenadePresent Molotov = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:DirtyHarry"), HeroController.GetHeroPrefab(HeroType.DirtyHarry).specialGrenade);
        public static GrenadePresent TankSummoner = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:TankBroSpecialAmmo"), HeroController.GetHeroPrefab(HeroType.TankBro).specialGrenade);
        public static GrenadePresent Martini = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:007MartiniGlass"), HeroController.GetHeroPrefab(HeroType.DoubleBroSeven).As<DoubleBroSeven>().martiniGlass);
        public static GrenadePresent Hologram = new GrenadeHologramPresent(ResourcesController.GetMaterial("sharedtextures:SnakeBroskin"), HeroController.GetHeroPrefab(HeroType.SnakeBroSkin).specialGrenade);
        // Pocketteds
        public static GrenadePresent Airstrike = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:BrodelWalker"), ProjectileController.GetAirstrikeGrenadePrefab());
        public static GrenadePresent AlienPheromones = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:Alien Pheromones"), ProjectileController.GetAlienPheromoneGrenadePrefab());
        public static GrenadePresent MechDrop = new GrenadePresent(ResourcesController.GetMaterial("sharedtextures:MechDrop"), ProjectileController.GetMechDropGrenadePrefab());

    }

    public static class GunPresents
    {
        public static GunTextureInfo AssaultRifleTexInfo = new GunTextureInfo()
        {
            Texture = ResourcesController.GetTexture(Config.path, "assault_rifle_anim.png"),
        };
        public static GunTextureInfo RocketLauncherTexInfo = new GunTextureInfo()
        {
            Texture = ResourcesController.GetTexture(Config.path, "rocket_launcher_anim.png"),
        };

        public static readonly BasicGun AssaultRifle = new BasicGun(HeroController.GetHeroPrefab(HeroType.Rambro).projectile,
                    30, 0.1f, // maxAmmo, fireDelay
                    HeroController.GetHeroPrefab(HeroType.Rambro).As<Rambro>().bulletShell, AssaultRifleTexInfo);
        public static readonly BasicGun RocketLauncher = new BasicGun(HeroController.GetHeroPrefab(HeroType.Brommando).projectile,
                    8, 0.6f, // maxAmmo, fireDelay
                    null, RocketLauncherTexInfo);

                   

    }

    public static class Common
    {
        public static Grenade TrowGrenade(TestVanDammeAnim character, Grenade grenade)
        {
            if (character == null || grenade == null) 
                return null;
            Grenade result = null;

            if (character.down && character.IsOnGround() && character.GetBool("ducking"))
            {
                result = ProjectileController.SpawnGrenadeOverNetwork(grenade, character, 
                    character.X + Mathf.Sign(character.transform.localScale.x) * 6f, character.Y + 3f, // x, y
                    0.001f, 0.011f, // radius, force
                    Mathf.Sign(character.transform.localScale.x) * 30f, 70f, // xI, yI 
                    character.playerNum
                    );
            }
            else
            {
                result = ProjectileController.SpawnGrenadeOverNetwork(grenade, character, 
                    character.X + Mathf.Sign(character.transform.localScale.x) * 8f, character.Y + 8f, // x, y
                    0.001f, 0.011f, // radius, force
                    Mathf.Sign(character.transform.localScale.x) * 200f, 150f, // xI, yI 
                    character.playerNum
                    );
            }
            return result;
        }
    }

   
}
