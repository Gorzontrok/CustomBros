using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;
using UnityEngine;
using System;
using System.IO;
using BroMakerLib.Loggers;

namespace BronobiMod
{
    [HeroPreset("Bronobi", HeroType.Blade)]
    public class Bronobi : SwordHero
    {
        public BronobiForceWave forceWave;
        public MindControlWave mindControlforceWave;

        protected Texture2D gunGrabSprite;
        protected Texture originalGunSprite;
        protected Vector2 grabDistance =  new Vector2(2, 2);

        protected Vector3 _grabOffset = new Vector3(1.5f, 0.10f, 0);
        protected Mook _grabbedMook;
        protected float _grabbedMookControlTime = 5f;
        protected int _grabbedOriginalNum;

        public void SetupGrabedUnit(Mook mook)
        {
            if (_grabbedMook != null)
                return;

            _grabbedMook = mook;
            //_grabbedMook.transform.parent = transform;
            _grabbedOriginalNum = _grabbedMook.playerNum;
            _grabbedMook.playerNum = playerNum;

            _grabbedMook.X = this.X + _grabOffset.x;
            _grabbedMook.Y = this.Y + _grabOffset.y;
            _grabbedMook.xI = 0f;
            _grabbedMook.yI = 0f;
            _grabbedMook.Panic(true);
        }

        public void UngrabMook(bool setPlayernum = true)
        {
            if (_grabbedMook == null)
                return;

           // _grabbedMook.transform.parent = null;
            if (setPlayernum)
                _grabbedMook.playerNum = _grabbedOriginalNum;

            gunSprite.SetTexture(originalGunSprite);
            _grabbedMook = null;
        }

        public override void PreloadAssets()
        {
            CustomHero.PreloadSounds(info.path, new System.Collections.Generic.List<string> {
                "saber_swing_1.wav", "saber_swing_2.wav",
                "saber_hit_0t.wav","saber_hit_1t.wav","saber_hit_2t.wav","saber_hit_3t.wav",
                "saber_hit_bullet.wav"
            });
        }

       /* protected override void PlayAttackSound()
        {
            //PlayAttackSound(1f);
        }*/

        protected override void Awake()
        {
            meleeType = MeleeType.Custom;
            base.Awake();

            try
            {
                // Load audios
                soundHolder.attackSounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(info.path, "saber_swing_1.wav"),
                    ResourcesController.GetAudioClip(info.path, "saber_swing_2.wav")
                };
                soundHolder.hitSounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(info.path, "saber_hit_0t.wav"),
                    ResourcesController.GetAudioClip(info.path, "saber_hit_1t.wav"),
                    ResourcesController.GetAudioClip(info.path, "saber_hit_2t.wav"),
                    ResourcesController.GetAudioClip(info.path, "saber_hit_3t.wav")
                };
                soundHolder.special2Sounds = soundHolder.hitSounds;
                soundHolder.defendSounds = soundHolder.hitSounds;
                soundHolder.special4Sounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(info.path, "saber_hit_bullet.wav")
                };
            }
            catch(Exception ex)
            {
                BMLogger.Log(ex);
            }
        }

        protected override void Start()
        {
            base.Start();
            originalGunSprite = gunSprite.GetTexture();
        }

        protected override void Update()
        {
            base.Update();
            // Update the grabbed mook position
            if (_grabbedMook == null)
                return;

            if (!_grabbedMook.IsAlive())
            {
                UngrabMook();
                return;
            }

            _grabbedMook.X = this.X + (_grabOffset.x * Direction) * Map.TileSize;
            // try to make it not go through blocks
            /*if (Physics.Raycast(transform.position, Vector3.right * Direction, out RaycastHit hitInfo, this.X + _grabOffset.x * Map.TileSize, Map.fragileLayer))
            {
                if (hitInfo.distance < _grabbedMook.X)
                    _grabbedMook.X = Mathf.Clamp(_grabbedMook.X, this.X, this.X + (hitInfo.distance * Direction) * Map.TileSize);
            }*/

            _grabbedMook.Y = this.Y + _grabOffset.y * Map.TileSize;
            _grabbedMook.xI = 0f;
            _grabbedMook.yI = 0f;
            _grabbedMook.Panic(true);
        }

        protected override void SetupThrownMookVelocity(out float XI, out float YI)
        {
            base.SetupThrownMookVelocity(out XI, out YI);
            XI *= 1.2f;
            YI *= 1.2f;
        }

        public override void Death(float xI, float yI, DamageObject damage)
        {
            UngrabMook();
            base.Death(xI, yI, damage);
            // Spawn Ghost
        }

        #region Melee
        protected override void StartMelee()
        {
            if (_grabbedMook != null)
            {
                SetupThrownMookVelocity(out float xI, out float yI);
                _grabbedMook.xI = xI;
                _grabbedMook.yI = yI;
                _grabbedMook.SetBackFlyingFrame(xI, yI);
                _grabbedMook.transform.parent = base.transform.parent;
                _grabbedMook.Reenable();
                _grabbedMook.StartFallingScream();
                _grabbedMook.EvaluateIsJumping();
                _grabbedMook.ThrowMook(false, playerNum);
                UngrabMook();
                return;
            }

            base.counter = 0f;
            this.currentMeleeType = this.meleeType;
            RaycastHit raycastHit;
            if ((Physics.Raycast(new Vector3(base.X, base.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, this.platformLayer) || Physics.Raycast(new Vector3(base.X + 4f, base.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, this.platformLayer) || Physics.Raycast(new Vector3(base.X - 4f, base.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, this.platformLayer)) && raycastHit.collider.GetComponentInParent<Animal>() != null)
            {
                this.currentMeleeType = BroBase.MeleeType.Knife;
            }
            else
            {
                Mook moook = Map.GetNearbyMook(grabDistance.x * Map.TileSize, grabDistance.y * Map.TileSize, X, Y, Direction, false);
                if (moook != null)
                {
                    SetupGrabedUnit(moook);
                    gunSprite.SetTexture(gunGrabSprite);
                }
            }

            switch (this.currentMeleeType)
            {
                case BroBase.MeleeType.Knife:
                    this.StartKnifeMelee();
                    break;
                case BroBase.MeleeType.Punch:
                case BroBase.MeleeType.JetpackPunch:
                    this.StartPunch();
                    break;
                case BroBase.MeleeType.Disembowel:
                case BroBase.MeleeType.FlipKick:
                case BroBase.MeleeType.Tazer:
                case BroBase.MeleeType.Custom:
                case BroBase.MeleeType.ChuckKick:
                case BroBase.MeleeType.VanDammeKick:
                case BroBase.MeleeType.ChainSaw:
                case BroBase.MeleeType.ThrowingKnife:
                case BroBase.MeleeType.Smash:
                case BroBase.MeleeType.BrobocopPunch:
                case BroBase.MeleeType.PistolWhip:
                case BroBase.MeleeType.HeadButt:
                case BroBase.MeleeType.TeleportStab:
                    this.StartCustomMelee();
                    base.ActivateGun();
                    break;
            }
        }
        protected override void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            if (_grabbedMook)
            {
                _grabbedMook.Gib();
                UngrabMook();
                return;
            }
            base.FireWeapon(x, y, xSpeed, ySpeed);
        }

        protected override void UseSpecial()
        {
            if (_grabbedMook)
            {
                MindControlEffect controlEffect = _grabbedMook.gameObject.AddComponent<MindControlEffect>();
                controlEffect.Setup(this, _grabbedOriginalNum, _grabbedMookControlTime);
                UngrabMook(false);
                return;
            }
            base.UseSpecial();
        }
        #endregion


    }
}
