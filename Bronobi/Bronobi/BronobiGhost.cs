using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BronobiMod
{
    public class BronobiGhost : MonoBehaviour
    {
        const int spawnAnimEndCol = 7;
        public Texture texture;
        public static BronobiGhost CreateAGhost(Texture2D sprite)
        {
            GameObject gameObject = new GameObject("BronobiGhost", new Type[] { typeof(MeshRenderer), typeof(SpriteSM), typeof(BronobiGhost) });
            BronobiGhost ghost = gameObject.GetComponent<BronobiGhost>();

            ghost.Sprite.SetTexture(sprite);
            ghost.Sprite.UpdateUVs();

            return ghost;
        }

        public SpriteSM Sprite { get; private set; }

        void Awake()
        {
            Sprite = gameObject.GetComponent<SpriteSM>();
        }
    }
}
