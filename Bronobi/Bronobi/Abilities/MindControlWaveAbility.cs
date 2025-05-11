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

            MindControlWave forceWave = new GameObject("MindControlWave").AddComponent<MindControlWave>();
            forceWave.transform.position = owner.transform.position;
            forceWave.Setup(waveTexture, controlTime);
            DirectionEnum directionEnum = DirectionEnum.Any;
            if (owner.right || owner.transform.localScale.x > 0f)
            {
                directionEnum = DirectionEnum.Right;
            }
            else if (owner.left || owner.transform.localScale.x < 0f)
            {
                directionEnum = DirectionEnum.Left;
            }
            // The visual are shit for some reason
            forceWave.Setup(owner.playerNum, owner, directionEnum/*OwnerDirection == 1 ? DirectionEnum.Right : DirectionEnum.Left*/);

            if (owner as Bronobi)
            {
                (owner as Bronobi).mindControlforceWave = forceWave;
            }
        }
    }
}
