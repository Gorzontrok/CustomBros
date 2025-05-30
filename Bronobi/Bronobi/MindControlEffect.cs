using BroMakerLib;
using BroMakerLib.CustomObjects.Bros;
using BroMakerLib.Loggers;
using HarmonyLib;
using System;
using UnityEngine;

namespace BronobiMod
{
    public class MindControlEffect : MonoBehaviour
    {
        public Mook mook { get; protected set; }

        public TestVanDammeAnim MindController {  get; protected set; }

        public float timeBetwenEffects = 1f;
        protected float _counter;
        protected int _originalPlayerNum;
        protected float _controlTime;

        protected Color _starColor = Color.cyan;

        protected static Texture2D[] _blindStars = null;

        protected void Awake()
        {
            mook = GetComponent<Mook>();
        }


        protected void Update()
        {
            _controlTime -= Time.deltaTime;
            if (mook == null || !mook.IsAlive() || _controlTime <= 0f)
            {
                StopControl();
                Destroy(this);
                return;
            }

            _counter -= Time.deltaTime;
            if (_counter <= 0f)
            {
                RunEffect();
                _counter = timeBetwenEffects;
            }
        }

        public void Setup(TestVanDammeAnim mindController, int originalPlayerNum, float controlTime)
        {
            MindController = mindController;
            _originalPlayerNum = originalPlayerNum;

            mook.GetComponent<PolymorphicAI>().TryLooseSightOfPlayer(mindController.playerNum);
            mook.playerNum = -10;
            mook.firingPlayerNum = mindController.playerNum;

            _controlTime = controlTime;

            if (_blindStars != null)
                return;
            try
            {
                string path = mindController.As<CustomHero>().info.path;
                _blindStars = new Texture2D[3] {
                        ResourcesController.GetTexture(path, "blind_stars_white1.png"),
                        ResourcesController.GetTexture(path, "blind_stars_white2.png"),
                        ResourcesController.GetTexture(path, "blind_stars_white3.png")
                    };
            }
            catch (Exception ex)
            {
                BMLogger.ExceptionLog(ex);
            }
        }

        protected virtual void RunEffect()
        {
            try
            {
                PuffTwoLayer puff2layer = CreateBlindStar(mook.X + UnityEngine.Random.value * 2f - 1f, mook.Y + 6f + mook.height * 1.4f,
                    2f, 2f, 1f,
                    0f, 20f, mook.transform);

                puff2layer.SetColor(_starColor);

               /* Puff puff = EffectsController.CreateEffect(EffectsController.instance.revivedZombiePassivePrefab, mook.X, mook.Y, mook.transform.position.z, 0f, Vector3.zero);
                if (puff != null)
                {
                    puff.SetColor(Color.white);
                    puff.transform.parent = transform;
                }*/
            }
            catch (Exception e)
            {
                BMLogger.ExceptionLog(e);
            }
        }

        protected PuffTwoLayer CreateBlindStar(float x, float y, float radius, float force, float count, float xI, float yI, Transform unitTransform)
        {
            if (EffectsController.instance == null)
                return null;

            int nbSpawned = 0;
            int stunnedStarsCount = 0;
            int choice = 0;
            PuffTwoLayer puffTwoLayer2 = null;
            while ((float)nbSpawned < count)
            {
                stunnedStarsCount += 1 + UnityEngine.Random.Range(0, 2);
                choice = stunnedStarsCount % 3;

                PuffTwoLayer puffTwoLayer;
                if (choice == 0)
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars1Prefab;
                }
                if (choice == 1)
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars2Prefab;
                }
                else
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars3Prefab;
                }

                puffTwoLayer2 = EffectsController.CreateEffect(puffTwoLayer, x, y, 0f, new Vector3(xI, yI, 0f), BloodColor.None);
                if (puffTwoLayer2 != null)
                {
                    puffTwoLayer2.transform.parent = unitTransform;
                    if (_blindStars.IsNotNullOrEmpty())
                    {
                        puffTwoLayer2.spriteLayer1.MeshRenderer.material.mainTexture = _blindStars[choice];
                        puffTwoLayer2.spriteLayer2.MeshRenderer.material.mainTexture = _blindStars[choice];
                    }
                }
                nbSpawned++;
            }
            return puffTwoLayer2;
        }

        public void StopControl()
        {
            mook.playerNum = _originalPlayerNum;
            mook.firingPlayerNum = _originalPlayerNum;
            mook.SetFieldValue("catchFriendlyBullets", false);
        }

        public void CreateBlueStars()
        {
            // voit les sfx plus tard, fait le gameplay d'abord
        }
    }

    [HarmonyPatch(typeof(Mook), "CatchFriendlyBullets")]
    static class CatchFriendlyBullets_Patch
    {
        static bool Prefix(Mook __instance, ref bool __result)
        {
            if (__instance.GetComponent<MindControlEffect>())
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
