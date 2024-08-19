using System;
using System.Linq;
using com.ethnicthv.LevelCreator;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace com.ethnicthv.Editor.LevelCreator
{
    [CustomEditor(typeof(Voxelizer))]
    public class VoEditor : UnityEditor.Editor
    {
        
        public string levelName = "map2";
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var voxelizer = (Voxelizer) target;
            if (GUILayout.Button("Voxelize Mesh"))
            {
                Debug.Log("Voxelize Mesh");
                if (!voxelizer.TryGetComponent(out MeshCollider meshCollider) ||
                    !voxelizer.TryGetComponent(out MeshFilter meshFilter)
                   )
                {
                    //error message
                    EditorUtility.DisplayDialog("Error", "MeshCollider or MeshFilter not found", "OK");
                    return;
                }
                voxelizer.VoxelizeMesh();
            }

            levelName = EditorGUILayout.TextField("Level Name", levelName);
            
            if (GUILayout.Button("Save Position Data"))
            {
                Debug.Log("Save Position Data");
                var map = new Game.Map.Map();
                Debug.Log("Map Size: " + voxelizer.size);
                map.size = voxelizer.size;
                var bounds = voxelizer.size/2;
                var maxBounds = voxelizer.size % 2 == 0 ? bounds - 1 : bounds;
                var sizeX = voxelizer.sizeX;
                var sizeY = voxelizer.sizeY;
                var sizeZ = voxelizer.sizeZ;
                
                map.map = new int[map.size * map.size * map.size];
                Array.Fill(map.map, -1);
                
                Debug.Log("Filling Map");
                voxelizer.gridPoints.ForEach((i =>
                {
                    // convert to map position
                    var x = i.x - sizeX/2 + bounds;
                    var y = i.y - sizeY/2 + bounds;
                    var z = i.z - sizeZ/2 + bounds;
                    var index = x + y * map.size + z * map.size * map.size;
                    map.map[index] = 0;
                }));
                Debug.Log($"Map Filled with {voxelizer.gridPoints.Count} grid points");
                
                var json = JsonUtility.ToJson(map);
                
                var path = $"Assets/com.ethnicthv/R/Map/{levelName}.json";
                Debug.Log($"Saving to {path}");
                System.IO.File.WriteAllText(path, json);
                
                AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
                var success = string.IsNullOrEmpty(result.Error);

                if (!success)
                {
                    Debug.LogError("Addressables build error encountered: " + result.Error);
                }
                Debug.Log("Map Saved");
            }
        }

        void OnSceneGUI()
        {
            var voxelizer = target as Voxelizer;

            Handles.color = Color.green;
            var size = voxelizer.halfSize * 2f;

            foreach (var worldPos in voxelizer.gridPoints.Select(gridPoint => voxelizer.PointToPosition(gridPoint)))
            {
                Handles.DrawWireCube(worldPos, new Vector3(size, size, size));
            }

            Handles.color = Color.red;
            if (!voxelizer.TryGetComponent(out MeshCollider meshCollider)) return;
            var bounds = meshCollider.bounds;
            Handles.DrawWireCube(bounds.center, bounds.extents * 2);
        }
    }
}