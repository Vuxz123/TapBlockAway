using System.Linq;
using com.ethnicthv.LevelCreator;
using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor
{
    [CustomEditor(typeof(Voxelizer))]
    public class VoEditor : UnityEditor.Editor
    {
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