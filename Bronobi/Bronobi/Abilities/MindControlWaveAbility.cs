using BroMakerLib.Abilities;
using BroMakerLib.Attributes;
using UnityEngine;

namespace BronobiMod.Abilities
{
    [AbilityPreset("MindControlWave")]
    public class MindControlWaveAbility : CharacterAbility
    {
        public Texture2D waveTexture;
        public override void All(string method, params object[] objects)
        {
            if (method == "Update" || method == "Start" || method == "Awake")
                return;

            var forceWave = new GameObject("MindControlWave").AddComponent<MindControlWave>();
            forceWave.transform.position = owner.transform.position;
            forceWave.Setup(waveTexture);
            forceWave.Setup(owner.playerNum, owner, OwnerDirection == 1 ? DirectionEnum.Right : DirectionEnum.Left);
        }
    }
}
