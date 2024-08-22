using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.LevelCreator;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace com.ethnicthv.Editor.LevelCreator
{
    [CustomEditor(typeof(PlayableMapGenerator))]
    public class PlayerMapGenEditor : UnityEditor.Editor
    {
        private List<CubeController> _cubes = new List<CubeController>();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mapGen = (PlayableMapGenerator) target;
            if (GUILayout.Button("Generate Map"))
            {
                EditorApplication.isPlaying = true;
                
                mapGen.Generate();
            }
            if (GUILayout.Button("Save Map Data"))
            {
                mapGen.SaveMapData();
        
                EditorApplication.isPlaying = false;
            }
            if (GUILayout.Button("Update Addressable"))
            {
                AddressableAssetSettings.BuildPlayerContent(out var result);
                var success = string.IsNullOrEmpty(result.Error);
        
                if (!success)
                {
                    Debug.LogError("Addressables build error encountered: " + result.Error);
                }
                else
                {
                    Debug.Log("Addressables build completed successfully.");
                }
            }
        }
    }
}