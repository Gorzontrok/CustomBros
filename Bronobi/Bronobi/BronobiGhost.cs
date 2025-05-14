using System;
using System.Collections.Generic;
using System.Linq;
using RocketLib;
using UnityEngine;

namespace BronobiMod
{
    public enum GhostState { Starting, Idle, Gift, Leaving}
    public enum GhostGift { Life, Special, FlexPower, Pig, Perk}
    public class BronobiGhost : MonoBehaviour
    {
        const int spawnAnimEndCol = 7;
        public Texture texture;

        public float X;
        public float Y;

        private GhostState state = GhostState.Starting;
        private int row = 0;
        private int col = 1;
        private int spriteSize = 32;
        private float fps = 1f;
        private float time;
        private float giftRange = 10f;

        private int _giftedPlayer;

        public static BronobiGhost CreateAGhost(Texture2D sprite, float X, float Y)
        {
            GameObject gameObject = new GameObject("BronobiGhost", new Type[] { typeof(MeshRenderer), typeof(SpriteSM), typeof(BronobiGhost) });
            BronobiGhost ghost = gameObject.GetComponent<BronobiGhost>();
            ghost.Setup(X, Y);
            ghost.Sprite.SetTexture(sprite);
            ghost.Sprite.UpdateUVs();

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
            }

            if (state == GhostState.Idle)
            {
                _giftedPlayer = HeroController.GetActualNearestPlayer(X, Y, giftRange, giftRange);
                if (_giftedPlayer == -1)
                    return;
                state = GhostState.Gift;
                ForceAnimate();
            }


            // Sin animation
            /*
            Vector3 pos = transform.localPosition;
            pos.y = Mathf.Sin(Time.deltaTime) + Y;
            transform.localPosition = pos;
            //*/
        }

        private void Awake()
        {
            Sprite = gameObject.GetComponent<SpriteSM>();
            time = fps;
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
            Sprite.SetLowerLeftPixel(col * spriteSize, row * spriteSize);
            if (col == 21)
            {
                // Gift
                int r = UnityEngine.Random.Range(0, 2);
                switch(r)
                {
                    case 2:
                        HeroController.AddLife(_giftedPlayer);
                        break;
                    default:
                        HeroController.SetSpecialAmmo(_giftedPlayer, HeroController.players[_giftedPlayer].chara)
                }
            }
            if (col == 23)
            {
                //Die?
            }
        }

        void Gift()
        {
            List<GhostGift> gifts = (GhostGift[])Enum.GetValues(typeof(GhostGift)).TOList
                ;
            TestVanDammeAnim bro = HeroController.players[_giftedPlayer].character;
            if (bro.SpecialAmmo >= bro.originalSpecialAmmo)
                gifts.remom

        }

        void AnimateLeaving()
        { }
    }
}
