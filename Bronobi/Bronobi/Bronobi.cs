using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;
using UnityEngine;

namespace BronobiMod
{
    [HeroPreset("Bronobi", HeroType.Blade)]
    public class Bronobi : SwordHero
    {
        protected BronobiForceWave _forceWave;

        protected Texture2D gunGrabSprite;
        protected Texture originalGunSprite;
        protected float maxGrabDistance = 20;

        protected GameObject _grabPoint;
        protected Unit _grabbedUnit;


        protected override void Awake()
        {
            base.Awake();

            _grabPoint = new GameObject("GrabPoint");
            _grabPoint.transform.parent = this.transform;
            _grabPoint.transform.localPosition = new Vector3(3, 0, 0);
        }

        protected override void Start()
        {
            base.Start();
            originalGunSprite = gunSprite.GetTexture();
        }

        protected override void SetupThrownMookVelocity(out float XI, out float YI)
        {
            base.SetupThrownMookVelocity(out XI, out YI);
            XI *= 1.2f;
            YI *= 1.2f;
        }

        protected override void StartMeleeCommon()
        {

            if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitInfo, maxGrabDistance, Map.unitLayer))
            {
                GameObject gameobj = hitInfo.rigidbody.gameObject;
                Unit unit = gameobj.GetComponent<Unit>();
                if (unit != null)
                {
                    _grabbedUnit = unit;
                }
            }

            gunSprite.SetTexture(gunGrabSprite);
            doingMelee = true;


        }

        protected override bool CanStartNewMelee()
        {
            return base.CanStartNewMelee();
        }

        protected override void CancelMelee()
        {
            gunSprite.SetTexture(originalGunSprite);
            base.CancelMelee();
        }
    }
}
