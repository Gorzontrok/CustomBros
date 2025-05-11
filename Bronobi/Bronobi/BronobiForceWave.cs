using BroMakerLib.Loggers;
using System;
using UnityEngine;

namespace BronobiMod
{
    public class BronobiForceWave : FlameWallExplosion
    {
        public static void CallMethod(TestVanDammeAnim testVanDammeAnim)
        {
            var forceWave = new GameObject("BronobiForceWave", new Type[] { typeof(Transform) }).AddComponent<BronobiForceWave>();
            forceWave.transform.position = testVanDammeAnim.transform.position;
            forceWave.Setup(testVanDammeAnim.playerNum, testVanDammeAnim, DirectionEnum.Any);
        }

        protected override void TryAssassinateUnits(float x, float y, int xRange, int yRange, int playerNum)
        {
            Mook closestMook = Map.GetNearbyMook((float)xRange, (float)yRange, x, y, (forceDirection == DirectionEnum.Left ? -1 : 1), true);
            if(closestMook)
            {
                float XI = Mathf.Sign(firedBy.transform.localScale.x) * 310f + (firedBy as TestVanDammeAnim).xI * 0.2f;
                float YI = 220f + (firedBy as TestVanDammeAnim).yI * 0.3f;
                closestMook.xI = XI;
                closestMook.yI = YI;
                closestMook.SetBackFlyingFrame(XI, YI);
                closestMook.transform.parent = firedBy.transform.parent;
                closestMook.Reenable();
                closestMook.StartFallingScream();
                closestMook.EvaluateIsJumping();
                closestMook.ThrowMook(false, base.playerNum);
                closestMook.Blind(10f);
            }
            Map.DeflectProjectiles(this, base.playerNum, 16f, (firedBy as TestVanDammeAnim).X + Mathf.Sign(base.transform.localScale.x) * 6f, (firedBy as TestVanDammeAnim).Y + 6f, Mathf.Sign(base.transform.localScale.x) * 200f, true);

        }

        void Awake()
        {
            try
            {
                assasinateUnits = true;
                damageDoodads = false;
                damageGround = false;
                damageUnits = false;
                knockUnits = false;
                blindUnits = false;
                maxCollumns = 32;
                maxRows = 3;
                rotateExplosionSprite = true;
                totalExplosions = 60;
                explosionRate = 0.04f;
            }
            catch(Exception ex)
            {
                BMLogger.Log($"[{typeof(BronobiForceWave)}] {ex}");
            }
        }

        public void Setup(Texture2D texture)
        {
            try
            {
                lightExplosion = MindControlWave.GetForceWavePuff(texture);

                MatildaTargettingWave wave = (HeroController.GetHeroPrefab(HeroType.TheBrofessional) as TheBrofessional).matildaTargettingWavePrefab;
                flashBangSoundHolder = wave.flashBangSoundHolder;
            }
            catch (Exception ex)
            {
                BMLogger.Log($"[{typeof(BronobiForceWave)}] {ex}");
            }
        }
    }
}
