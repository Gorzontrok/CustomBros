using System;
using System.Collections.Generic;
using System.Linq;
using RocketLib;
using RocketLib.Collections;
using UnityEngine;
using World.Generation.MapGenV4;

namespace BronobiMod
{
    public enum GhostState { Starting, Idle, Gift, Leaving}
    public enum GhostGift { Life, Special, FlexPower, Pig, Perk, Pocketted, Drone}
    public class BronobiGhost : MonoBehaviour
    {
        const int spawnAnimEndCol = 7;
        public Texture texture;

        public float X;
        public float Y;

        private GhostState state = GhostState.Starting;
        private int row = 1;
        private int col = 1;
        private int spriteSize = 32;
        private float fps = 0.085f;
        private float time;

        private float giftRange = 30f;
        private int _giftedPlayer;
        private List<GhostGift> _gifts;

        public static BronobiGhost CreateAGhost(Texture2D sprite, float X, float Y, Material mat, int layer)
        {
            GameObject gameObject = new GameObject("BronobiGhost", new Type[] { typeof(MeshRenderer) });
            gameObject.transform.position = new Vector3(X, Y, 0);
            gameObject.layer = layer;
            gameObject.GetComponent<Renderer>().material = new Material(mat);
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", sprite);
            BronobiGhost ghost = gameObject.AddComponent<BronobiGhost>();
            ghost.Setup(X, Y);

            return ghost;
        }

        public void Setup(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public SpriteSM Sprite { get; private set; }

        private void Update()
        {

            time -= Time.deltaTime;
            if (time <  0)
            {
                // next frame
                Animate();
                time = fps;
            }

            if (state == GhostState.Idle)
            {
                int num = HeroController.GetActualNearestPlayer(X, Y, giftRange, giftRange);
                if (num == -1 || !HeroController.PlayerIsAlive(num))
                    return;
                if (DetermineGifts(num))
                {
                    _giftedPlayer = num;
                    state = GhostState.Gift;
                    ForceAnimate();
                }
            }
        }

        private void Awake()
        {
            time = fps;

            Sprite = gameObject.GetOrAddComponent<SpriteSM>();
            Sprite.SetTextureDefaults();
            Sprite.SetPixelDimensions(spriteSize, spriteSize);
            Sprite.SetSize(spriteSize, spriteSize);
            Sprite.SetLowerLeftPixel(0, spriteSize);
        }

        void ForceAnimate()
        {
            time = fps;
            Animate();
        }

        void Animate()
        {
            switch(state)
            {
                case GhostState.Starting:
                    AnimateStarting();
                    break;
                case GhostState.Idle:
                    AnimateIdle();
                    break;
                case GhostState.Gift:
                    AnimateGift();
                    break;
                case GhostState.Leaving:
                    AnimateLeaving();
                    break;
            }
        }

        void AnimateStarting()
        {
            // max frame 7
            ++col;
            Sprite.SetLowerLeftPixel(col * spriteSize, row * spriteSize);
            if (col == 7)
            {
                state = GhostState.Idle;
            }
        }

        void AnimateIdle()
        {
            //Max frame 8
            if (col == 15)
                col = 7;
            else
                ++col;
            Sprite.SetLowerLeftPixel(col * spriteSize, row * spriteSize);
        }

        void AnimateGift()
        {
            // max frame 8
            // gift frame 6
            ++col;
            if (col < 16)
                col = 16;
            Sprite.SetLowerLeftPixel(col * spriteSize, row * spriteSize);
            if (col == 21)
            {
                Gift();
            }
            if (col == 23)
            {
                //Die?
            }
        }

        protected int _nextBroRevive = -1;
        bool DetermineGifts(int playerNum)
        {
            _gifts = new List<GhostGift>();

            TestVanDammeAnim bro = HeroController.players[playerNum].character;
            if (Config.refillAmmos && bro.SpecialAmmo < bro.originalSpecialAmmo)
                _gifts.Add(GhostGift.Special);
            if (Config.giveFlexPower && !bro.player.HasFlexPower())
                _gifts.Add(GhostGift.FlexPower);
            if (Config.givePockettedAmmo && bro.AsBroBase().pockettedSpecialAmmo.IsNullOrEmpty())
                _gifts.Add(GhostGift.Pocketted);
            if (Config.addLife && (/*(_nextBroRevive = GetFirstPlayerDead()) != -1 ||*/ bro.player.Lives < 4))
                _gifts.Add(GhostGift.Life);
            if (Config.useProcGen && ProcGenGameMode.UseProcGenRules)
                _gifts.Add(GhostGift.Perk);
            if (Config.spawnPig && UnityEngine.Random.value > 0.2f)
                _gifts.Add(GhostGift.Pig);
            if (Config.spawnDrone)
                _gifts.Add(GhostGift.Drone);

            return _gifts.IsNotNullOrEmpty();
        }

        int GetFirstPlayerDead()
        {
            if (HeroController.GetPlayersAliveCount() < 1)
                return -1;
            if (HeroController.Instance.playerDeathOrder.IsNullOrEmpty())
                return -1;
            int res = -1;

            while (HeroController.Instance.playerDeathOrder.Count != 0)
            {
                int num = HeroController.Instance.playerDeathOrder[0];
                HeroController.Instance.playerDeathOrder.RemoveAt(0);
                if (HeroController.players[num] != null && HeroController.IsPlayerPlaying(num) && !HeroController.players[num].IsAlive())
                {
                    res = num;
                    break;
                }
            }
            return res;
        }

        void Gift()
        {
            if (_giftedPlayer == -1)
                return;
            if (_gifts.IsNullOrEmpty())
                return;
          /*  if (_nextBroRevive != -1 && HeroController.players[_nextBroRevive].character != null)
            {
                HeroController.players[_nextBroRevive].character.SetWillComebackToLife(0);
            }*/
            TestVanDammeAnim bro = HeroController.players[_giftedPlayer].character;
            int r = UnityEngine.Random.Range(0, _gifts.Count);
            GhostGift gift = _gifts[r];
            switch(gift)
            {
                case GhostGift.Life:
                    bro.player.AddLife();
                    break;
                case GhostGift.Special:
                    bro.AddSpecialAmmo(); break;
                case GhostGift.Pig:
                    // Spawn Pig
                    MapController.SpawnTestVanDamme_Networked(Map.Instance.activeTheme.animals[0].GetComponent<TestVanDammeAnim>(),
                        X, Y, 0f, 0f,
                        tumble: false, useParachuteDelay: false, useParachute: false, onFire: false);
                    break;
                case GhostGift.FlexPower:
                    // Give random FlexPower
                    Pickupable pickup = PickupableController.CreateGoldenPrizePickupable(X, Y, PickupType.None);
                    Registry.RegisterDeterminsiticGameObject(pickup.gameObject);
                    pickup.Launch(X, Y, 0, 0);
                    EffectsController.CreatePuffDisappearRingEffect(X, Y, 0f, 0f);
                    break;
                case GhostGift.Pocketted:
                    // Give random pocketted
                    Pickupable pickup2 = null;
                    switch((PockettedSpecialAmmoType)UnityEngine.Random.Range(2,8))
                    {
                        case PockettedSpecialAmmoType.Timeslow:
                            pickup2 = PickupableController.CreateTimeSlowAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.TimeSlow;
                            break;
                        case PockettedSpecialAmmoType.RemoteControlCar:
                            pickup2 = PickupableController.CreateRCCarAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.RemotecontrolCar;
                            break;
                        case PockettedSpecialAmmoType.Airstrike:
                            pickup2 = PickupableController.CreateAirstrikeAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.Airstrike;
                            break;
                        case PockettedSpecialAmmoType.MechDrop:
                            pickup2 = PickupableController.CreateMechDropAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.MechDrop;
                            break;
                        case PockettedSpecialAmmoType.AlienPheromones:
                            pickup2 = PickupableController.CreateAlienPheromonesAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.AlienPheromones;
                            break;
                        case PockettedSpecialAmmoType.Steroids:
                        default:
                            pickup2 = PickupableController.CreateSteroidsAmmoBox(X, Y);
                            pickup2.pickupType = PickupType.Steroids;
                            break;
                    }
                    pickup2.Launch(X, Y, 0, 100);
                    EffectsController.CreatePuffDisappearRingEffect(X, Y, 0f, 0f);
                    break;
                case GhostGift.Perk:
                    // give random perk
                    break;
                case GhostGift.Drone:
                    bro.CallMethod("SpawnDrone");
                    break;
            }
        }

        void AnimateLeaving()
        { }
    }
}
