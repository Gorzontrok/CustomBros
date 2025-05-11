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
            mook.playerNum = mindController.playerNum;
            mook.firingPlayerNum = mindController.playerNum;

            _controlTime = controlTime;
        }

        protected virtual void RunEffect()
        {
            try
            {
                EffectsController.CreateShrapnelBlindStar(mook.X + UnityEngine.Random.value * 2f - 1f, mook.Y + 6f + mook.height * 1.4f,
                    2f, 2f, 1f,
                    0f, 20f, mook.transform);

                Puff puff = EffectsController.CreateEffect(EffectsController.instance.revivedZombiePassivePrefab, mook.X, mook.Y, mook.transform.position.z, 0f, Vector3.zero);
                if (puff != null)
                {
                    puff.SetColor(Color.white);
                    puff.transform.parent = transform;
                }
            }
            catch (Exception e)
            {
                BMLogger.ExceptionLog(e);
            }
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
