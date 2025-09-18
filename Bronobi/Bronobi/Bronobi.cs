using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;
using UnityEngine;
using System;
using BroMakerLib.Loggers;

namespace BronobiMod
{
    [HeroPreset("Bronobi", HeroType.Blade)]
    public class Bronobi : SwordHero
    {
        public BronobiForceWave forceWave;
        public MindControlWave mindControlforceWave;
        protected Texture2D _waveTexture;
        protected float _controlTime = 10f;

        public BronobiGhost Ghost;
        protected Texture2D ghostSprite;
        protected Vector3 _ghostSpawnOffset = new Vector3(0, 16);

        protected Texture originalGunSprite;
        protected Texture2D gunGrabSprite;
        protected Vector2 grabDistance =  new Vector2(5, 2);
        protected Vector3 _grabOffset = new Vector3(1.5f, 0.10f, 0);
        protected Mook _grabbedMook;
        protected float _grabbedMookControlTime = 5f;
        protected int _grabbedOriginalNum;
        protected float _grabBossTime = 1f;
        protected bool _grabbedBoss = false;

        protected Vector2Int _specialAnimationPosition = new Vector2Int(0, 9);
        protected float _specialAnimationRate = 0.0434f;

        protected AudioClip _forcePush;

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

            if (_grabbedMook as AlienClimber)
            {
                _grabbedMook.As<AlienClimber>().ForceClimbing(false, false, false);
            }
            //_grabbedMook.Panic(true);
        }

        public void UngrabMook(bool setPlayernum = true)
        {
            if (_grabbedMook == null)
                return;

           // _grabbedMook.transform.parent = null;
            if (setPlayernum && (_grabbedMook.NotAs<MookArmouredGuy>() || _grabbedMook.As<MookArmouredGuy>().pilotUnit.NotAs<BroBase>()))
            {
                _grabbedMook.playerNum = _grabbedOriginalNum;
                _grabbedMook.firingPlayerNum = _grabbedOriginalNum;
            }

            gunSprite.SetTexture(originalGunSprite);
            _grabbedMook = null;

            _grabbedBoss = false;
        }

        public override void UIOptions()
        {

            Config.UI();
        }

        protected override void Awake()
        {
            meleeType = MeleeType.Custom;
            base.Awake();

            ghostSprite = ResourcesController.GetTexture(Info.path, "Bronobi_Ghost_anim.png");
            try
            {
                // Load audios
                soundHolder.attackSounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(Info.path, "saber_swing_1.wav"),
                    ResourcesController.GetAudioClip(Info.path, "saber_swing_2.wav")
                };
                soundHolder.hitSounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(Info.path, "saber_hit_0t.wav"),
                    ResourcesController.GetAudioClip(Info.path, "saber_hit_1t.wav"),
                    ResourcesController.GetAudioClip(Info.path, "saber_hit_2t.wav"),
                    ResourcesController.GetAudioClip(Info.path, "saber_hit_3t.wav")
                };
                soundHolder.special2Sounds = soundHolder.hitSounds;
                soundHolder.defendSounds = soundHolder.hitSounds;
                soundHolder.special4Sounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(Info.path, "saber_hit_bullet.wav")
                };
                soundHolder.specialAttackSounds = new AudioClip[]
                {
                    ResourcesController.GetAudioClip(Info.path, "force_1.wav"),
                    ResourcesController.GetAudioClip(Info.path, "force_2.wav"),
                    ResourcesController.GetAudioClip(Info.path, "force_3.wav")
                };
                _forcePush = ResourcesController.GetAudioClip(Info.path, "force_push.wav");
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
            }

            if (_grabbedBoss && (_grabBossTime -= Time.deltaTime) < 0)
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
            _grabbedMook.ForceFaceDirection(Direction);
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
            if (Ghost != null)
                return;
            try
            {
                Ghost = BronobiGhost.CreateAGhost(ghostSprite, X, Y, Renderer.material, gameObject.layer);
                Ghost.transform.parent = transform.parent;
                Ghost.transform.localPosition = this.transform.localPosition + _ghostSpawnOffset;
            }
            catch(Exception ex)
            {
                BMLogger.ExceptionLog(ex);
            }
        }

        #region Melee
        protected override void PressHighFiveMelee(bool forceHighFive = false)
        {
            if (_grabbedMook != null)
            {
                if (!_grabbedBoss && (_grabbedMook.NotAs<MookArmouredGuy>() || _grabbedMook.As<MookArmouredGuy>().pilotUnit.NotAs<BroBase>()))
                {
                    SetupThrownMookVelocity(out float xI, out float yI);
                    _grabbedMook.isBeingThrown = true;
                    _grabbedMook.xI = xI;
                    _grabbedMook.yI = yI;
                    _grabbedMook.SetBackFlyingFrame(xI, yI);
                    _grabbedMook.transform.parent = base.transform.parent;
                    _grabbedMook.Reenable();
                    _grabbedMook.StartFallingScream();
                    _grabbedMook.EvaluateIsJumping();
                    _grabbedMook.ThrowMook(false, playerNum);
                    sound.PlayAudioClip(_forcePush, XY, 0.7f);
                }
                UngrabMook();
                return;
            }
            base.PressHighFiveMelee(forceHighFive);
        }
        protected override void StartMelee()
        {
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
                if (moook != null && moook.gameObject.activeSelf && moook.playerNum < 0)
                {
                    SetupGrabedUnit(moook);
                    gunSprite.SetTexture(gunGrabSprite);
                    if (BroMakerUtilities.IsBoss(moook))
                    {
                        _grabbedBoss = true;
                        _grabBossTime = 1f;
                    }

                    return;
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
                    //base.ActivateGun();
                    break;
            }
        }
        #endregion

        protected override void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            if (_grabbedMook)
            {
                if (!_grabbedBoss && (_grabbedMook.NotAs<MookArmouredGuy>() || _grabbedMook.As<MookArmouredGuy>().pilotUnit.NotAs<BroBase>()))
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
                if (!_grabbedMook.As<Satan>() && !_grabbedBoss && (_grabbedMook.NotAs<MookArmouredGuy>() || _grabbedMook.As<MookArmouredGuy>().pilotUnit.NotAs<BroBase>()))
                {
                    ControlMook(_grabbedMook, _grabbedOriginalNum);
                    UngrabMook(false);
                }
                else
                    UngrabMook(true);
                return;
            }

            if (SpecialAmmo > 0)
            {
                PlaySpecialAttackSound(0.8f);
                SpecialAmmo--;
                if (IsMine)
                {
                    TriggerBroSpecialEvent();
                    mindControlforceWave = MindControlWave.CreateMindControleWave(this, _waveTexture, _controlTime);
                }
                pressSpecialFacingDirection = 0;
                return;
            }
            HeroController.FlashSpecialAmmo(playerNum);
            ActivateGun();
        }

        protected override void CalculateMovement()
        {
            if (usingSpecial && SpecialAmmo > 0)
            {
                xI = 0;
                yI = 0;
                return;
            }
            base.CalculateMovement();
        }

        protected override void AnimateSpecial()
        {
            SetSpriteOffset(0f, 0f);
            DeactivateGun();
            frameRate = _specialAnimationRate;
            int row = _specialAnimationPosition.x + Mathf.Clamp(base.frame, 0, 8);
            int col = _specialAnimationPosition.y;
            sprite.SetLowerLeftPixel((float)(row * this.spritePixelWidth), (float)(this.spritePixelHeight * col));
            if (frame == 6)
            {
                UseSpecial();
            }
            if (frame >= 8)
            {
                frame = 0;
                usingSpecial = (usingPockettedSpecial = false);
                ActivateGun();
                ChangeFrame();
            }
        }

        protected virtual void ControlMook(Mook mook, int mookNum)
        {
            MindControlEffect controlEffect = mook.gameObject.AddComponent<MindControlEffect>();
            controlEffect.Setup(this, mookNum, _controlTime);
        }
    }
}
