using BroMakerLib.Abilities;
using BroMakerLib.Attributes;
using UnityEngine;

namespace BronobiMod.Abilities
{
    [AbilityPreset(nameof(AutoReflect))]
    public class AutoReflect : CharacterAbility
    {
        public Vector2 positionOffset = new Vector2();
        public Vector2 radius = new Vector2 (1, 1);
        public float fRadius = 16f;
        public float deflectForce = 200f;

        public Vector2 Position
        {
            get
            {
                return new Vector2(owner.X, owner.Y) + positionOffset;
            }
        }

        public override void All(string method, params object[] objects)
        {
            if (method == "Update" || method == "Start" || method == "Awake")
                return;

            Map.DeflectProjectiles(owner, owner.playerNum,
                fRadius,
                owner.X + Mathf.Sign(owner.transform.localScale.x) * positionOffset.x, // X
                owner.Y + positionOffset.y, // y
                Mathf.Sign(owner.transform.localScale.x) * deflectForce, // xI
                false
                );
        }



        public void ReverseProjectile(Projectile projectile)
        {
            projectile.playerNum = owner.playerNum;
            projectile.firedBy = owner;
            projectile.xI = projectile.xI * -1f;
            projectile.yI = projectile.yI * -1f;
        }

        public bool CanReverseProjectile(Projectile projectile)
        {
            if (owner.playerNum == projectile.playerNum)
                return false;

            float distanceX = Position.x - projectile.X;
            if (Mathf.Abs(distanceX) > radius.x)
                return false;

            float distanceY = Position.y - projectile.Y;
            if (Mathf.Abs(distanceY) > radius.y)
                return false;

            return true;
        }
    }
}
