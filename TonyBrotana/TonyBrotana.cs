using BroMakerLib;
using BroMakerLib.CustomObjects.Bros;
using RocketLib;
using UnityEngine;

namespace TonyBrotanaMod
{
    [HeroPreset(nameof(TonyBrotana), HeroType.Rambro)]
    public class TonyBrotana : CustomHero
    {
        public Texture2D RocketTexture
        {
            get
            {
                if (_rocketTexture == null)
                    _rocketTexture = ResourcesController.GetTexture(DirectoryPath, "TonyBrotana_Special.png");
                return _rocketTexture;
            }
        }
        private Texture2D _rocketTexture;

        protected int fireCount = 0;
        protected float rocketDelay = 0.25f;
        protected bool willFireRocket = false;
        protected Projectile rocket;
        protected int bankAccount = 0;

        public void AddMoney(int amount)
        {
            bankAccount += amount;

            if (bankAccount >= 5)
            {
                AddSpecialAmmo();
                bankAccount = 0;
            }
        }

        protected override void Awake()
        {
            rocket = HeroController.GetHeroPrefab(HeroType.Brommando).projectile;
            base.Awake();
        }

        protected override void UseFire()
        {
            base.UseFire();
        }

        protected override void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            base.FireWeapon(x, y, xSpeed, ySpeed);
            if (willFireRocket)
            {
                willFireRocket = false;
                fireDelay = rocketDelay;
                return;
            }
            fireCount++;
            if (fireCount >= 3)
            {
                fireCount = 0;
                fireDelay = rocketDelay;
                willFireRocket = true;
            }

        }

        protected override Projectile SpawnPrimaryProjectile(Projectile projectilePrefab, float x, float y, float xSpeed, float ySpeed)
        {
            if (willFireRocket)
            {
                xIBlast = -transform.localScale.x * 40f;
                Projectile rocketSpawned = ProjectileController.SpawnProjectileLocally(rocket, this, x, y, xSpeed, ySpeed, playerNum);
                rocketSpawned.SetRendererTexture(RocketTexture);
                return rocketSpawned;
            }
            Projectile proj = base.SpawnPrimaryProjectile(projectilePrefab, x, y, xSpeed, ySpeed);
            proj.damage = 3;
            return proj;
        }

    }
}
