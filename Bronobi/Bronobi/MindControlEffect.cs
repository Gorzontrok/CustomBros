using BroMakerLib;
using BroMakerLib.CustomObjects.Bros;
using BroMakerLib.Loggers;
using System;
using UnityEngine;

namespace BronobiMod
{
    public class MindControlEffect : MonoBehaviour
    {
        public Mook mook { get; protected set; }

        public TestVanDammeAnim MindController {  get; protected set; }
        protected int _originalPlayerNum;


        public float timeBetwenEffects = 1f;
        protected float _counter;
        protected float _controlTime;

        protected Color _starColor = Color.cyan;
        protected static Texture2D[] _blindStars = null;

        protected Unit _nearestEnemy = null;
        protected int _rangeEnemySearch = 300;

        protected void Awake()
        {
            mook = GetComponent<Mook>();
        }

        protected void Update()
        {
            _controlTime -= Time.deltaTime;
            if (mook == null || !mook.IsAlive() || _controlTime <= 0f)
            {
                StopControl();
                Destroy(this);
                return;
            }

            _counter -= Time.deltaTime;
            if (_counter <= 0f)
            {
                RunEffect();
                _counter = timeBetwenEffects;
            }

            if (_nearestEnemy == null || !_nearestEnemy.IsAlive())
            {
                if (_nearestEnemy != null && !_nearestEnemy.IsAlive())
                {
                    mook.enemyAI.ClearActionQueue(true);
                    mook.enemyAI.AddAction(EnemyActionType.Laugh, 1f);
                }

                if (mook.NotAs<MookSuicide>() && mook.NotAs<MookSuicideUndead>() && mook.enemyAI.CallMethod<bool>("CanSeeEnemyThisWay", mook.Direction))
                {
                    mook.enemyAI.AddAction(EnemyActionType.Fire, 1f);
                    return;
                }
                else if (mook.NotAs<MookSuicide>() && mook.NotAs<MookSuicideUndead>() && mook.enemyAI.CallMethod<bool>("CanSeeEnemyThisWay", -mook.Direction))
                {
                    mook.ForceFaceDirection(-mook.Direction);
                    mook.enemyAI.AddAction(EnemyActionType.Fire, 1f);
                    return ;
                }

                _nearestEnemy = Map.GetNearestUnit(RocketLib.Collections.Nums.TERRORIST, _rangeEnemySearch, mook.X, mook.Y, false);
                if (_nearestEnemy == null)
                    _nearestEnemy = Map.GetNearestUnit(RocketLib.Collections.Nums.ALIENS, _rangeEnemySearch, mook.X, mook.Y, false);
                if (_nearestEnemy != null)
                {
                    mook.enemyAI.SetMentalState(MentalState.Alerted);
                    mook.enemyAI.AddAction(EnemyActionType.FollowPath, new GridPoint(_nearestEnemy.collumn + (UnityEngine.Random.value > 0.5f ? 2 : -2), _nearestEnemy.row));
                   // mook.enemyAI.AddAction(EnemyActionType.FacePoint, new GridPoint(_nearestEnemy.collumn, _nearestEnemy.row));
                    mook.enemyAI.AddAction(EnemyActionType.Fire, mook.enemyAI.attackTime);
                }
            }
        }

        public void Setup(TestVanDammeAnim mindController, int originalPlayerNum, float controlTime)
        {
            MindController = mindController;
            _originalPlayerNum = originalPlayerNum;

            mook.playerNum = 10;
            mook.firingPlayerNum = 10;
            mook.enemyAI.ClearActionQueue(true);
            mook.enemyAI.SetMentalState(MentalState.Idle);

            _controlTime = controlTime;

            if (_blindStars != null)
                return;
            try
            {
                string path = mindController.As<CustomHero>().info.path;
                _blindStars = new Texture2D[3] {
                        ResourcesController.GetTexture(path, "blind_stars_white1.png"),
                        ResourcesController.GetTexture(path, "blind_stars_white2.png"),
                        ResourcesController.GetTexture(path, "blind_stars_white3.png")
                    };
            }
            catch (Exception ex)
            {
                BMLogger.ExceptionLog(ex);
            }
        }

        protected virtual void RunEffect()
        {
            PuffTwoLayer puff2layer = CreateBlindStar(mook.X + UnityEngine.Random.value * 2f - 1f, mook.Y + 6f + mook.height * 1.4f,
                2f, 2f, 1f,
                0f, 20f, mook.transform);

            puff2layer.SetColor(_starColor);
        }

        protected PuffTwoLayer CreateBlindStar(float x, float y, float radius, float force, float count, float xI, float yI, Transform unitTransform)
        {
            if (EffectsController.instance == null)
                return null;

            int nbSpawned = 0;
            int stunnedStarsCount = 0;
            int choice = 0;
            PuffTwoLayer puffTwoLayer2 = null;
            while ((float)nbSpawned < count)
            {
                stunnedStarsCount += 1 + UnityEngine.Random.Range(0, 2);
                choice = stunnedStarsCount % 3;

                PuffTwoLayer puffTwoLayer;
                if (choice == 0)
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars1Prefab;
                }
                if (choice == 1)
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars2Prefab;
                }
                else
                {
                    puffTwoLayer = EffectsController.instance.stunnedStars3Prefab;
                }

                puffTwoLayer2 = CreatePuffTwoLayer(puffTwoLayer, x, y, 0f, new Vector3(xI, yI, 0f));
                if (puffTwoLayer2 != null)
                {
                    puffTwoLayer2.transform.parent = unitTransform;
                    if (_blindStars.IsNotNullOrEmpty())
                    {
                        puffTwoLayer2.spriteLayer1.MeshRenderer.material.mainTexture = _blindStars[choice];
                        puffTwoLayer2.spriteLayer2.MeshRenderer.material.mainTexture = _blindStars[choice];
                    }
                }
                nbSpawned++;
            }
            return puffTwoLayer2;
        }

        PuffTwoLayer CreatePuffTwoLayer(PuffTwoLayer puffTwoLayer, float x, float y,float delay, Vector3 velocity)
        {
            PuffTwoLayer puff = UnityEngine.Object.Instantiate(puffTwoLayer, new Vector3(x, y, 0), Quaternion.identity);
            if (velocity != Vector3.zero)
            {
                velocity.z = 0f;
                puff.SetVelocity(velocity);
            }
            if (delay > 0f)
            {
                puff.Delay(delay);
            }
            return puff;
        }

        public void StopControl()
        {
            mook.playerNum = _originalPlayerNum;
            mook.firingPlayerNum = _originalPlayerNum;
            mook.enemyAI.ClearActionQueue(true);
            mook.enemyAI.AddAction(EnemyActionType.BecomeIdle, 0.2f);
            mook.enemyAI.AddAction(EnemyActionType.QuestionMark, 1f);
            mook.SetFieldValue("catchFriendlyBullets", false);
        }
    }
}
