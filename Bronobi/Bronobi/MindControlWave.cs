using BroMakerLib.Loggers;
using System;
using UnityEngine;

namespace BronobiMod
{
    public class MindControlWave : MatildaTargettingWave
    {
        private static Puff _forceWavePuff;

        public TestVanDammeAnim Character
        { get => firedBy as  TestVanDammeAnim;}

        protected float _controlTime = 2.0f;

        public static Puff GetForceWavePuff(Texture2D texture)
        {
            if (_forceWavePuff != null)
                return _forceWavePuff;

            GameObject go = new GameObject("Bronobi_MindControlWave_Puff", new Type[] { typeof(MeshFilter), typeof(MeshRenderer), typeof(SpriteSM) });

            Puff wavePuff = (HeroController.GetHeroPrefab(HeroType.TheBrofessional) as TheBrofessional).matildaTargettingWavePrefab.lightExplosion;

            SpriteSM sprite = go.GetComponent<SpriteSM>();
            sprite.Copy(wavePuff.GetComponent<SpriteSM>());

            _forceWavePuff = go.AddComponent<Puff>();

            _forceWavePuff.frameRate = wavePuff.frameRate;
            _forceWavePuff.pauseFrame = wavePuff.pauseFrame;
            _forceWavePuff.gameObject.layer = wavePuff.gameObject.layer;
            _forceWavePuff.spriteSize = wavePuff.spriteSize;
            _forceWavePuff.frames = wavePuff.frames;
            _forceWavePuff.rows = wavePuff.rows;
            _forceWavePuff.loopStartFrame = wavePuff.loopStartFrame;
            _forceWavePuff.loopEndFrame = wavePuff.loopEndFrame;
            _forceWavePuff.numLoops = wavePuff.numLoops;
            _forceWavePuff.requiresGroundBelow = wavePuff.requiresGroundBelow;
            _forceWavePuff.pauseFrame = wavePuff.pauseFrame;
            _forceWavePuff.pauseTime = wavePuff.pauseTime;
            _forceWavePuff.useGravity = wavePuff.useGravity;
            _forceWavePuff.gravityM = wavePuff.gravityM;
            _forceWavePuff.correctRotation = wavePuff.correctRotation;
            _forceWavePuff.useLightingMultiplier = wavePuff.useLightingMultiplier;

            // Change texture
            Material mat = new Material(wavePuff.GetComponent<MeshRenderer>().material);
            MeshRenderer renderer = _forceWavePuff.GetComponent<MeshRenderer>();
            mat.mainTexture = texture;
            if (texture != null)
            {
                renderer.material = mat;
            }
            return _forceWavePuff;
        }


        public void Setup(Texture2D wavePointTexture, float controlTime)
        {
            try
            {
                _controlTime = controlTime;

                lightExplosion = GetForceWavePuff(wavePointTexture);

                MatildaTargettingWave wave = (HeroController.GetHeroPrefab(HeroType.TheBrofessional) as TheBrofessional).matildaTargettingWavePrefab;
                flashBangSoundHolder = wave.flashBangSoundHolder;
            }
            catch (Exception ex)
            {
                BMLogger.Log($"[{typeof(MindControlWave)}] {ex}");
            }
        }

        protected virtual void Awake()
        {
            assasinateUnits = true; // make sure that 'TryAssassinateUnits(float,float,int,int,int)' is called
            //damageDoodads = false;
            knockUnits = false;
            blindUnits = false;

            rotateExplosionSprite = true;

           // maxCollumns = 32;
            //maxRows = 3;
            totalExplosions = 52;
            explosionRate = 0.04f;
        }

        protected override void TryAssassinateUnits(float x, float y, int xRange, int yRange, int playerNum)
        {
            Mook closestMook = Map.GetNearbyMook((float)xRange, (float)yRange, x, y, (forceDirection == DirectionEnum.Left ? -1 : 1), false);
            if (closestMook != null)
            {
                MindControlEffect controlEffect = closestMook.gameObject.AddComponent<MindControlEffect>();
                controlEffect.Setup(Character, closestMook.playerNum, _controlTime);
            }
        }
    }
}