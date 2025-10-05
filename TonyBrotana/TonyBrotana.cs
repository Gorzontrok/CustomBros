using BroMakerLib;
using BroMakerLib.CustomObjects.Bros;
using RocketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonyBrotanaMod
{
    [HeroPreset(nameof(TonyBrotana), HeroType.Rambro)]
    public class TonyBrotana : CustomHero
    {
        protected int _fireCount = 0;
        protected float _rocketDelay = 0.5f;
        protected bool _willFireRocket = false;
        protected Projectile _rocket;

        protected override void Awake()
        {
            _rocket = HeroController.GetHeroPrefab(HeroType.Brommando).projectile;
            base.Awake();
        }

        protected override void UseFire()
        {
            base.UseFire();
        }

        protected override void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            base.FireWeapon(x, y, xSpeed, ySpeed);
            if (_willFireRocket)
            {
                _willFireRocket = false;
                fireDelay = _rocketDelay;
                return;
            }
            _fireCount++;
            if (_fireCount >= 3)
            {
                _fireCount = 0;
                fireDelay = _rocketDelay;
                _willFireRocket = true;
            }

        }

        protected override Projectile SpawnPrimaryProjectile(Projectile projectilePrefab, float x, float y, float xSpeed, float ySpeed)
        {
            if (_willFireRocket)
            {
                Projectile rocket = ProjectileController.SpawnProjectileLocally(_rocket, this, x, y, xSpeed, ySpeed, base.playerNum);
                rocket.SetRendererTexture(ResourcesController.GetTexture(DirectoryPath, "TonyBrotana_Special.png"));
                return rocket;
            }
            Projectile proj = base.SpawnPrimaryProjectile(projectilePrefab, x, y, xSpeed, ySpeed);
            proj.damage = 3;
            return proj;
        }

    }
}
