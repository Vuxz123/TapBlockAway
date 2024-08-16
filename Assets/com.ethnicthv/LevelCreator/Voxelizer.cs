using System.Collections.Generic;
using UnityEngine;

namespace com.ethnicthv.LevelCreator
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Voxelizer: MonoBehaviour
    {
        [Header("Setup")]
        public float halfSize = 0.5f;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        
        [Header("Voxelizer Properties")]
        public List<Vector3Int> generatedGridPoints = new List<Vector3Int>();
        public Vector3 localOrigin;
        public int sizeX;
        public int sizeY;
        public int sizeZ;
        
        public List<Vector3Int> gridPoints
        {
            get => generatedGridPoints;
            set => generatedGridPoints = value;
        }

        public Vector3 PointToPosition(Vector3Int point)
        {
            float size = halfSize * 2f;
            Vector3 pos = new Vector3(halfSize + point.x * size, halfSize + point.y * size, halfSize + point.z * size);
            return localOrigin + transform.TransformPoint(pos);
        }
        
        public void VoxelizeMesh()
        {
            var bounds = meshCollider.bounds;
            var minExtents = bounds.center - bounds.extents;
            var count = bounds.extents / halfSize;

            var xMax = Mathf.CeilToInt(count.x);
            var yMax = Mathf.CeilToInt(count.y);
            var zMax = Mathf.CeilToInt(count.z);
            
            sizeX = xMax;
            sizeY = yMax;
            sizeZ = zMax;

            gridPoints.Clear();
            localOrigin = transform.InverseTransformPoint(minExtents);

            for (var x = 0; x < xMax; ++x)
            {
                for (var z = 0; z < zMax; ++z)
                {
                    for (var y = 0; y < yMax; ++y)
                    {
                        var pos = PointToPosition(new Vector3Int(x, y, z));
                        if (Physics.CheckBox(pos, new Vector3(halfSize, halfSize, halfSize)))
                        {
                            gridPoints.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }
        }
    }
    
}