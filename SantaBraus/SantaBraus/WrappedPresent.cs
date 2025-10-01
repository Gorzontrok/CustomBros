using BroMakerLib;
using BroMakerLib.Loggers;
using System;
using UnityEngine;

namespace SantaBrausMod.Presents
{
    public class WrappedPresent : Shrapnel
    {
		private static Texture[] _PresentsTex;

		protected MobileSwitch _mobileSwitch;

        protected IPresent _present;

		private static void CreatePresentTex()
		{
			if (_PresentsTex != null)
				return;
			try
			{
				_PresentsTex = new Texture[]
				{
				    ResourcesController.GetTexture(Config.path, "present_1.png")
				};
			}
			catch (Exception e)
			{
				BMLogger.ExceptionLog(e);
			}
		}

        public static WrappedPresent CreateWrappedPresent(MonoBehaviour firedBy, float X, float Y, int playerNum, int layer)
        {
            CreatePresentTex();

            GameObject gameObject = new GameObject("WrappedPresent", new Type[] { typeof(MeshRenderer) });
            gameObject.transform.position = new Vector3(X, Y, 0);
            gameObject.layer = layer;
            gameObject.GetComponent<Renderer>().material = new Material(ResourcesController.GetMaterial("sharedtextures:Grenade"));
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", _PresentsTex.RandomElement());

            // Create Sprite
            SpriteSM sprite = gameObject.AddComponent<SpriteSM>();
            sprite.SetTextureDefaults();

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(16, 16, 64);

            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            // Creat Mobile Switch
            MobileSwitch mobileSwitch = new GameObject("MobileSwitch").AddComponent<MobileSwitch>();
            mobileSwitch.transform.parent = gameObject.transform;
            mobileSwitch.transform.localPosition = Vector3.zero;
            mobileSwitch.affectedGameObject = gameObject;
            mobileSwitch.methodName = "UnwrapPresent";

            // Create Bubble
            GameObject g = new GameObject("Empty Bubble", new Type[] { typeof(MeshRenderer) });
            g.GetComponent<Renderer>().material = new Material(HeroController.GetHeroPrefab(HeroType.Rambro).high5Bubble.GetComponent<Renderer>().material);
            g.GetComponent<Renderer>().material.SetTexture("_mainTex", ResourcesController.GetTexture(Config.path, "present_bubble.png"));

            SpriteSM sprite2 = g.AddComponent<SpriteSM>();
            sprite2.SetPixelDimensions(32, 32);
            sprite2.SetSize(32, 32);
            sprite2.width = 32;
            sprite2.height = 32;
            sprite2.CalcUVs();
            sprite2.UpdateUVs();

            ReactionBubble bubble = g.AddComponent<ReactionBubble>();
            bubble.life = 3;
            bubble.animatedBubble = true;

            bubble.transform.parent = mobileSwitch.transform;
            bubble.transform.localPosition = new Vector3(0.5f, 30f, 0);
            bubble.RefreshYStart();
            mobileSwitch.bubble = bubble;

            WrappedPresent wrappedPresent = gameObject.AddComponent<WrappedPresent>();
            wrappedPresent._mobileSwitch = mobileSwitch;

            return wrappedPresent;
        }

		public void AssignPresent(IPresent present)
		{
			if (_present != null)
				return;
			_present = present;
		}

        protected override void RunLife()
        { }

        protected override void Awake()
		{
			_mobileSwitch = GetComponent<MobileSwitch>();
			spriteWidth = 16;
			spriteHeight = 16;
			lifeM = 9999;
			zOffset = -0.01f;
			fadeUVs = false;

            base.Awake();

            ShouldKillIfNotVisible = false;
			shrink = false;

			// Fix Sprite
            sprite.SetPixelDimensions(16, 16);
            sprite.SetSize(12, 12);
            sprite.SetLowerLeftPixel(0, 16);
            sprite.CalcUVs();
            sprite.UpdateUVs();
        }

        protected override void Start()
        {
			//otherMaterial = mainMaterial;
            base.Start();
			life = 3;
			//otherMaterial = mainMaterial;
		}

		public void UnwrapPresent(Unit activator)
		{
			if (activator.NotAs<TestVanDammeAnim>())
			{
				ForceDeathRPC();
				Destroy(_mobileSwitch);
				return;
			}

			_present.Unwrap(activator as TestVanDammeAnim);
			Destroy(_mobileSwitch);
			ForceDeathRPC();
		}

       /* public override void Death()
		{
			BMLogger.Warning("died");
            // present is destroyed
            _present = null;
			if (!dontMakeEffects)
			{
				MakeEffects();
			}
			DestroyGrenade();
		}*/

      /*  protected override void MakeEffects()
		{
			EffectsController.CreateExplosionRangePop(base.X, base.Y, -1f, this.range * 2f);

			this.PlayDeathSound();
			bool flag;
			Map.DamageDoodads(this.damage, DamageType.Explosion, base.X, base.Y, 0f, 0f, this.range, this.playerNum, out flag, null);
		}*/
    }
}
