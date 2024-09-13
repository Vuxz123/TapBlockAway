using System;
using UnityEngine;

namespace com.ethnicthv.Game.Cube.CubeSkin
{
    [Serializable]
    public class Skin
    {
        public string name;
        public Mesh mesh;
        public Mesh arrowMesh;
        public Material material;
        public Material arrowMaterial;
        public float scale = 1;
    }
}