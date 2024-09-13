using UnityEngine;

namespace com.ethnicthv.Game
{
    public static class MaterialExtension
    {
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        public static void SetOpacity( this Material material)
        {
            if (material)
            {
                // Set blend mode to opaque
                material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt(ZWrite, 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                // Set alpha to 1
                var color = material.color;
                color.a = 1.0f;
                material.color = color;
            }
            else
            {
                Debug.LogWarning("Material is null.");
            }
        }
        
        public static void SetTransparent( this Material material)
        {
            if (material)
            {
                // Set blend mode to transparent
                material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt(ZWrite, 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                Debug.LogWarning("Material is null.");
            }
        }
    }
}