using BroMakerLib.Abilities;
using BroMakerLib.Attributes;
using UnityEngine;

namespace BronobiMod.Abilities
{
    [AbilityPreset("MindControlWave")]
    public class MindControlWaveAbility : CharacterAbility
    {
        public Texture2D waveTexture;
        public float controlTime = 10f;

        public override void All(string method, params object[] objects)
        {
            if (method == "Update" || method == "Start" || method == "Awake")
                return;

            MindControlWave forceWave = MindControlWave.CreateMindControleWave(owner, waveTexture, controlTime);

            if (owner as Bronobi)
            {
                (owner as Bronobi).mindControlforceWave = forceWave;
            }
        }


    }
}
