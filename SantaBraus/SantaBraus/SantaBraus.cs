using BroMakerLib.CustomObjects.Bros;
using BroMakerLib;
using UnityEngine;
using UnityEngine.UI;
using System;
using BroMakerLib.Loggers;
using SantaBrausMod.Presents;
using System.Collections.Generic;
using System.Linq;

namespace SantaBrausMod
{
    [HeroPreset("SantaBraus", HeroType.Rambro)]
    public class SantaBraus : CustomHero
    {
        protected GunTextureInfo _originalGun;

        public PresentsBag Bag { get; protected set; }

        protected short _bagSatisfaction = 0;
        protected short _bagSatisfactionRequired = 2;

        protected Stack<IPresentSpecial> _specials;
        protected Queue<Material> _specialsIcons;
        protected IPresentSpecial _specialKeeped = null;

        protected IPresentGun _presentGun = null;
        protected Text _ammoLeftText;
        protected Vector3 anchorPos;

        protected bool _hasHitWithSlice;
        protected bool _hasHitWithWall;
        protected bool _hasPlayedAttackHitSound;
        protected bool _hasStartedfiring;
        protected List<Unit> _alreadyHit = new List<Unit>();

        public override void UIOptions()
        {
            Config.UI();
        }

        protected override void Awake()
        {
            Config.path = DirectoryPath;
            try
            {
                Bag = new PresentsBag();
                Bag.Initialize(this);
                _specials = new Stack<IPresentSpecial>(6);
                _specialsIcons = new Queue<Material>(6);

                _originalGun = new GunTextureInfo()
                {
                    OffsetX = 3
                };
            }
            catch(Exception ex)
            {
                BMLogger.ExceptionLog(ex);
            }

            base.Awake();

        }

        protected override void Start()
        {
            base.Start();

            _originalGun.Texture = gunSprite.GetTexture();
            SetGunTexture(_originalGun, false);

            _ammoLeftText = CanvasController.CreateTextInstance();
            _ammoLeftText.gameObject.SetActive(false);
            _ammoLeftText.color = Color.magenta;
        }

        public override void PickupPockettableAmmo(PockettedSpecialAmmoType ammoType)
        {
            switch (ammoType)
            {
                case PockettedSpecialAmmoType.Airstrike:
                    AddSpecialPresent(SpecialPresents.Airstrike);
                    break;
                case PockettedSpecialAmmoType.AlienPheromones:
                    AddSpecialPresent(SpecialPresents.AlienPheromones);
                    break;
                case PockettedSpecialAmmoType.MechDrop:
                    AddSpecialPresent(SpecialPresents.MechDrop);
                    break;
                case PockettedSpecialAmmoType.Standard:
                    ResetSpecialAmmo();
                    break;
                case PockettedSpecialAmmoType.None:
                case PockettedSpecialAmmoType.Damage:
                case PockettedSpecialAmmoType.Dollars:
                case PockettedSpecialAmmoType.Perk:
                case PockettedSpecialAmmoType.Piggy:
                case PockettedSpecialAmmoType.RemoteControlCar:
                case PockettedSpecialAmmoType.Revive:
                case PockettedSpecialAmmoType.Steroids:
                case PockettedSpecialAmmoType.Timeslow:
                    break;
            }
        }
        protected new PockettedSpecialAmmoType GetPockettedAmmoType()
        {
            return PockettedSpecialAmmoType.None;
        }

        public override void ResetSpecialAmmo()
        {
            if (_presentGun == null || _presentGun as IGunMagazine == null)
                return;
            _presentGun.As<IGunMagazine>().Refill();
            _ammoLeftText.text = _presentGun.As<IGunMagazine>().AmmoLeft.ToString();
        }
        public override bool IsAmmoFull()
        {
            if (_presentGun == null || _presentGun as IGunMagazine == null)
                return true;
            return _presentGun.As<IGunMagazine>().IsAmmoFull();
        }

        public override void RecallBro()
        {
            base.RecallBro();
            Destroy(_ammoLeftText);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(_ammoLeftText);
        }

        public override void Death(float xI, float yI, DamageObject damage)
        {
            base.Death(xI, yI, damage);
            _ammoLeftText.gameObject.SetActive(false);
        }

        protected virtual void SetGunTexture(GunTextureInfo gunInfo, bool changeTexture = true)
        {
            if (changeTexture)
                gunSprite.SetTexture(gunInfo.Texture);
            gunSprite.SetSize(gunInfo.Width, gunInfo.Height);
            gunSprite.SetLowerLeftPixel(gunInfo.LowerLeftPixelX, gunInfo.LowerLeftPixelY);
            gunSpritePixelHeight = gunInfo.Height;
            gunSpritePixelWidth = gunInfo.Width;
            gunSprite.SetPixelDimensions(gunInfo.Width, gunInfo.Height);
            gunSprite.RecalcTexture();
            CurrentGunSpriteOffset = new Vector2(gunInfo.OffsetX, gunInfo.OffsetY);
        }

        public void AddSpecialPresent(IPresentSpecial presentSpecial)
        {
            _specials.Push(presentSpecial);
            _specialsIcons.Enqueue(presentSpecial.Icon);
            SpecialAmmo = _specials.Count;

            // update icons
            try
            {
                BroMakerUtilities.SetSpecialMaterials(playerNum, _specialsIcons.ToList(), CurrentSpecialMaterialOffset, CurrentSpecialMaterialSpacing);
            }
            catch (Exception ex) { BMLogger.ExceptionLog(ex); }
        }

        public void AssignGunPresent(IPresentGun presentGun)
        {
            _presentGun = presentGun;
            // get stats
            if (_presentGun as IGunMagazine == null)
                return;
            _ammoLeftText.gameObject.SetActive(true);
            _ammoLeftText.text = _presentGun.As<IGunMagazine>().AmmoLeft.ToString();
            if (_presentGun.GunAppearance == null)
                return;
            SetGunTexture(_presentGun.GunAppearance);
        }

        public void RemoveCurrentGunPresent()
        {
            _presentGun = null;
            _ammoLeftText.gameObject.SetActive(false);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (_ammoLeftText == null || !_ammoLeftText.isActiveAndEnabled)
                return;
            anchorPos = player.characterUI.GetFieldValue<Vector3>("anchorPos");
            _ammoLeftText.transform.position = anchorPos + Vector3.up * 0.5f + Vector3.back * 32f;
        }

        protected override void RunFiring()
        {
            if (_presentGun != null)
            {
                base.RunFiring();
                return;
            }

            if (gunFrame <= 0)
            {
                base.RunFiring();
            }
        }

        protected override void RunGun()
        {
            if (_presentGun != null)
            {
                base.RunGun();
                return;
            }

            // BroveHeart.RunGunDisarmed();
            if (gunFrame < 0)
                return;
            if (!_hasStartedfiring)
            {
                StartFiring();
            }
            gunCounter += t;
            if (gunCounter > 0.034f)
            {
                gunCounter -= 0.034f;
                gunFrame--;
                if (gunFrame < 0)
                {
                    gunFrame = 0;
                }
                SetGunSprite(gunFrame, 0);
                if (!_hasPlayedAttackHitSound)
                {
                    if (_hasHitWithSlice)
                    {
                        PlayHitSound();
                        _hasPlayedAttackHitSound = true;
                    }
                    if (_hasHitWithWall)
                    {
                        PlayHitSound();
                        _hasPlayedAttackHitSound = true;
                    }
                }
                if (gunFrame >= 3)
                {
                    SwingFistEnemies();
                }
            }
        }

        protected override void UseFire()
        {
            if (_presentGun != null)
            {
                base.UseFire();
                fireDelay = _presentGun.FireDelay;
                return;
            }
            _alreadyHit.Clear();
            _hasHitWithWall = false;
            _hasHitWithSlice = false;
            _hasStartedfiring = false;
            gunFrame = 6;
            gunCounter = 0f;
            SetGunSprite(gunFrame, 0);
            _hasPlayedAttackHitSound = false;
        }

        protected override void StartFiring()
        {
            if (_presentGun != null)
            {
                base.StartFiring();
                return;
            }
            if (_hasStartedfiring || gunFrame <= 0)
            {
                return;
            }

            _hasStartedfiring = true;
            PlayAttackSound();
            SetGunSprite(gunFrame, 0);
            float xIT = transform.localScale.x * 12f;
            ConstrainToFragileBarriers(ref xIT, 16f);
            if (Physics.Raycast(new Vector3(X - Mathf.Sign(transform.localScale.x) * 12f, Y + 5.5f, 0f), 
                new Vector3(transform.localScale.x, 0f, 0f), 
                out raycastHit, 20f, groundLayer) || Physics.Raycast(new Vector3(X - Mathf.Sign(transform.localScale.x) * 12f, Y + 10.5f, 0f), 
                new Vector3(transform.localScale.x, 0f, 0f), 
                out raycastHit, 20f, groundLayer))
            {
                EffectsController.CreateProjectilePopWhiteEffect(X + (width + 4f) * transform.localScale.x, Y + height + 4f);
                bool hitCage = raycastHit.collider.GetComponent<Cage>();
                MapController.Damage_Local(this, raycastHit.collider.gameObject, 1 + (hitCage ? 5 : 0), DamageType.Bullet, xI, 0f, X, Y);
                if (!_hasHitWithWall)
                {
                    SortOfFollow.Shake(0.15f);
                }
                SwingFistEnemies();
                _hasHitWithWall = true;
            }
            else
            {
                _hasHitWithWall = false;
                SwingFistEnemies();
            }
        }

        protected virtual void SwingFistEnemies()
        {
            if (Map.HitUnits(this, playerNum, 5, DamageType.Melee, 12f, X, Y + 6f, transform.localScale.x * 200f, 160f, true, true, true, _alreadyHit, false, false))
            {
                if (!_hasHitWithSlice)
                {
                    SortOfFollow.Shake(0.15f);
                }
                _hasHitWithSlice = true;
            }
            else
            {
                _hasHitWithSlice = false;
            }
        }
        protected override void FireWeapon(float x, float y, float xSpeed, float ySpeed)
        {
            // Use current weapon
            if (_presentGun != null)
            {
                _presentGun.FireWeapon(x, y, xSpeed, ySpeed);
                _ammoLeftText.text = _presentGun.As<IGunMagazine>().AmmoLeft.ToString();
            }
            // base.FireWeapon(x, y, xSpeed, ySpeed);
        }

        public override void ReceiveHeroKillReport(KillData killData)
        {
            base.ReceiveHeroKillReport(killData);
            if (Bag == null)
                return;
            _bagSatisfaction += 1;
            if (killData.gibbed)
                _bagSatisfaction += 1;

            if (_bagSatisfaction == _bagSatisfactionRequired)
            {
                Bag.SpawnWrappedPresent();
                _bagSatisfaction = 0;
            }
            // update a progress bar ?
        }

        public override void Damage(int damage, DamageType damageType, float xI, float yI, int direction, MonoBehaviour damageSender, float hitX, float hitY)
        {
            if (_specialKeeped != null && (_specialKeeped as IDamageEvent) != null)
            {
                bool success = _specialKeeped.As<IDamageEvent>().DamageEvent(this, damage, damageType, xI, yI, direction, damageSender, hitX, hitY);
                if (success)
                    return;
            }
            base.Damage(damage, damageType, xI, yI, direction, damageSender, hitX, hitY);
        }

        protected override void UseSpecial()
        {
            if (_specialKeeped != null)
            {
                if(!_specialKeeped.UseSpecial(this))
                    _specialKeeped = null;
                return;
            }

            if (SpecialAmmo > 0)
            {
                PlaySpecialAttackSound(0.8f);
                SpecialAmmo--;
                if (IsMine)
                {
                    TriggerBroSpecialEvent();
                    // Throw present
                    IPresentSpecial special = _specials.Pop();
                    _specialsIcons.Dequeue();
                    bool keep = special.UseSpecial(this);
                    if (keep)
                    {
                        _specialKeeped = special;
                    }
                }
                pressSpecialFacingDirection = 0;
                return;
            }
            HeroController.FlashSpecialAmmo(playerNum);
            ActivateGun();
        }
    }
}
