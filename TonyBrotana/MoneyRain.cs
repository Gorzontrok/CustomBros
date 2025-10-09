using BroMakerLib.Loggers;
using HarmonyLib;
using RocketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TonyBrotanaMod
{
    public class MoneyRain
    {
    }

    [HarmonyPatch(typeof(Mook))]
    static class Mook_Patches
    {
        [HarmonyPatch("Damage")]
        [HarmonyPostfix]
        static void MakeMoneyOnHit(Mook __instance, int damage, DamageType damageType, float xI, float yI, int direction, MonoBehaviour damageSender, float hitX, float hitY)
        {
            if (!(damageSender as TonyBrotana))
                return;

            MookDropMoneyEffect mookDropMoneyEffect = __instance.gameObject.GetComponent<MookDropMoneyEffect>();
            if (mookDropMoneyEffect == null)
            {
                mookDropMoneyEffect = __instance.gameObject.AddComponent<MookDropMoneyEffect>();
                mookDropMoneyEffect.SetFieldValue("moneys", 10);

            }
            if (mookDropMoneyEffect != null)
                mookDropMoneyEffect.TakeDamage(
                    new DamageObject(damage, __instance.GetFieldValue<DamageType>("lastDamageType"), xI, yI, hitX, hitY, damageSender));
            //__instance.SetFieldValue("_mookDamageEffect", mookDropMoneyEffect);
        }

        [HarmonyPatch("Damage")]
        [HarmonyPostfix]
        static void Gib(DamageType damageType, float xI, float yI)
        {

        }
    }

    [HarmonyPatch(typeof(Pickupable))]
    static class Pickupable_Patches
    {
        [HarmonyPatch("Collect")]
        [HarmonyPrefix]
        static void AddMoneyToTony(Pickupable __instance, TestVanDammeAnim hero)
        {
            if (hero.NotAs<TonyBrotana>() || __instance.pickupType != PickupType.Dollars)
                return;

            hero.As<TonyBrotana>().AddMoney(2);
        }
    }
}
