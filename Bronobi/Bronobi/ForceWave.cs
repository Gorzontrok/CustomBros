using BroMakerLib.Abilities;
using BroMakerLib.Attributes;
using UnityEngine;

namespace BronobiMod
{
    [AbilityPreset("ForceWave")]
    public class ForceWave : CharacterAbility
    {
        public Texture2D forceWaveTexture;
        public override void All()
        {
            var forceWave = new GameObject("BronobiForceWave").AddComponent<BronobiForceWave>();
            forceWave.transform.position = owner.transform.position;
            forceWave.Setup(forceWaveTexture);
            forceWave.Setup(owner.playerNum, owner, DirectionEnum.Any);
        }
    }
}
