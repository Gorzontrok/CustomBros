using BroMakerLib.Loggers;
using RocketLib;
using System;
using System.Collections;
using UnityEngine;

namespace BronobiMod
{
    public class MindControlWave : FlameWallExplosion
    {
        public TestVanDammeAnim Character
        { get => firedBy as  TestVanDammeAnim;}

        [SerializeField] protected float _mindControlTimeInSecond = 2.0f;

        public void Setup(Texture2D wavePointTexture)
        {
            try
            {
                lightExplosion = new GameObject("Bronobi_MindControlWave_Puff", new Type[] { typeof(MeshFilter), typeof(MeshRenderer), typeof(SpriteSM), typeof(Puff) }).GetComponent<Puff>();
                lightExplosion.frameRate = 0.01f;
                lightExplosion.pauseFrame = 12;
                lightExplosion.gameObject.layer = 19;

                if (wavePointTexture != null)
                {
                    Puff matildaPuff = (HeroController.GetHeroPrefab(HeroType.TheBrofessional) as TheBrofessional).matildaTargettingWavePrefab.lightExplosion;
                    Material mat = new Material(matildaPuff.GetComponent<MeshRenderer>().material);
                    MeshRenderer renderer = lightExplosion.GetComponent<MeshRenderer>();
                    mat.mainTexture = wavePointTexture;
                    renderer.material = mat;
                }

                SpriteSM sprite = lightExplosion.GetComponent<SpriteSM>();
                sprite.lowerLeftPixel = new Vector2(0, 64);
                sprite.pixelDimensions = new Vector2(16, 64);
                sprite.plane = SpriteBase.SPRITE_PLANE.XY;
                sprite.width = 16;
                sprite.height = 64;
                sprite.offset = new Vector3(0, 0, -5);
            }
            catch (Exception ex)
            {
                BMLogger.Log($"[{typeof(MindControlWave)}] {ex}");
            }
        }

        protected virtual void Awake()
        {
            assasinateUnits = true; // make sure that 'TryAssassinateUnits(float,float,int,int,int)' is called
            damageDoodads = false;
            knockUnits = false;
            blindUnits = false;

            rotateExplosionSprite = true;

            maxCollumns = 32;
            maxRows = 3;
            totalExplosions = 60;
            explosionRate = 0.04f;
        }

        protected override void TryAssassinateUnits(float x, float y, int xRange, int yRange, int playerNum)
        {
            Mook closestMook = Map.GetNearbyMook((float)xRange, (float)yRange, x, y, (forceDirection == DirectionEnum.Left ? -1 : 1), false);
            if (closestMook != null)
            {
                ControlMook(closestMook);
            }
        }

        protected virtual void ControlMook(Mook mook)
        {
            mook.playerNum = playerNum;
            mook.gameObject.SetOwnerNetworked(Character.Owner);
            mook.SetReviveSource(Character);
            mook.SetIsZombie(true);
        }

        private IEnumerator StopControlMookCoroutine(Mook mook, int previousPlayerNum)
        {
            yield return new WaitForSeconds(_mindControlTimeInSecond);

            mook.playerNum = previousPlayerNum;
            mook.Blind(0.0f);
        }

    }
}