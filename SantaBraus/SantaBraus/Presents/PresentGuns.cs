using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SantaBrausMod.Presents
{
    public class GunTextureInfo
    {
        public Texture Texture;
        public int Width = 32;
        public int Height = 32;
        public int LowerLeftPixelX = 0;
        public int LowerLeftPixelY = 32;
        public int OffsetX = 0;
        public int OffsetY = 0;
    }

    public class BasicGun : IPresentGun, IGunMagazine
    {
        public Projectile Projectile { get; set; }
        public TestVanDammeAnim Character { get; protected set; }
        public GunTextureInfo GunAppearance => _gunTextureInfo;
        public float FireDelay { get => _fireDelay; }
        public int AmmoLeft { get; protected set; }

        protected Shrapnel _bulletShell;
        protected float _fireDelay;
        protected GunTextureInfo _gunTextureInfo;
        protected int _ammoMax;

        public BasicGun(Projectile projectile, int maxAmmo, float fireDelay, Shrapnel bulletShell, GunTextureInfo gunSprite) 
        {
            Projectile = projectile;
            _bulletShell = bulletShell;
            _ammoMax = maxAmmo;
            _fireDelay = fireDelay;
            _gunTextureInfo = gunSprite;
        }

        public void Refill()
        {
            AmmoLeft = _ammoMax;
        }

        public bool IsAmmoFull()
        {
            return _ammoMax <= AmmoLeft;
        }

        public void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            if (AmmoLeft <= 0)
            SpawnBulletShell(x, y);

            Character.SetFieldValue("gunFrame", 3);
            Character.CallMethod("SetGunSprite", new object[] { 3, 0 });
            Character.CallMethod("TriggerBroFireEvent");
            EffectsController.CreateMuzzleFlashEffect(x, y, -25f, xSpeed * 0.15f, ySpeed * 0.15f, Character.transform);

            ProjectileController.SpawnProjectileLocally(Projectile, Character, x, y, xSpeed, ySpeed, Character.playerNum);
            AmmoLeft--;

            if (AmmoLeft <= 0)
            {
                TryRemoveGun();
            }
        }

        protected void TryRemoveGun()
        {
            if (Character.NotAs<SantaBraus>())
                return;
            Character.As<SantaBraus>().RemoveCurrentGunPresent();
        }

        public void Unwrap(TestVanDammeAnim character)
        {
            this.Character = character;
            if (character as SantaBraus == null)
                return;
            AmmoLeft = _ammoMax;
            character.As<SantaBraus>().AssignGunPresent(this);
        }

        protected void SpawnBulletShell(float x, float y)
        {
            EffectsController.CreateShrapnel(_bulletShell, 
                x + Character.transform.localScale.x * -15f, y + 3f, // x, y
                1f, 30f, 1f, // radius, force, count
                -Character.transform.localScale.x * 80f, 170f // xI, yI
                );
        }

    }
}
