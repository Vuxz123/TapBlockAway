using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace com.ethnicthv.Game.Cube
{
    public class CubeManager : MonoBehaviour
    {
        public static CubeManager instance { get; private set; }

        public Transform cubeContainer;

        public GameObject cubePrefab;
        public int cubeMoveDistance = 10;
        public int cubeMoveSpeed = 2;

        public float cubeMoveDuration => cubeMoveDistance / (float)cubeMoveSpeed;

        public int boundX
        {
            get => _bounds.Item1;
            set => _bounds.Item1 = value;
        }

        public int boundY
        {
            get => _bounds.Item2;
            set => _bounds.Item2 = value;
        }

        public int boundZ
        {
            get => _bounds.Item3;
            set => _bounds.Item3 = value;
        }

        private CubePoll _cubePoll;
        private readonly Dictionary<(int, int, int), CubeController> _cubeList = new();
        private (int, int, int) _bounds = (10, 10, 10);

        private void Awake()
        {
            instance = this;
            _cubePoll = new CubePoll(cubeContainer, cubePrefab, (transform1, o) => Instantiate(o, transform1));
        }

        public static bool InBounds(Vector3 pos)
        {
            var (x, y, z) = instance._bounds;
            return pos.x >= -x-0.5f && pos.x <= x+0.5f &&
                   pos.y >= -y-0.5f && pos.y <= y+0.5f &&
                   pos.z >= -z-0.5f && pos.z <= z+0.5f;
        }

        #region GetCube

        [CanBeNull]
        public CubeController GetCube((int, int, int) pos)
        {
            return _cubeList.GetValueOrDefault(pos);
        }

        [CanBeNull]
        public CubeController GetCube(int x, int y, int z)
        {
            var key = (x, y, z);
            return _cubeList.GetValueOrDefault(key);
        }

        [CanBeNull]
        public CubeController GetCube(Vector3 pos)
        {
            var key = ((int)pos.x, (int)pos.y, (int)pos.z);
            return _cubeList.GetValueOrDefault(key);
        }

        #endregion

        public bool CreateCube(int x, int y, int z, CubeDirection direction)
        {
            var key = (x, y, z);
            
            if (_cubeList.ContainsKey(key)) return false;
            
            var cube = _cubePoll.GetCubeObject().GetComponent<CubeController>();
            
            cube.transform.position = new Vector3(x, y, z);
            cube.Setup(key, GetNearbyColor(x, y, z), direction);
            _cubeList.Add(key, cube);
            cube.Appear();
            
            return true;
        }

        public void DestroyCube(int x, int y, int z, bool fade = false)
        {
            var key = (x, y, z);
            
            if (!_cubeList.Remove(key, out var value)) return;

            if (fade)
            {
                value.FadeOut(onComplete: OnComplete); 
                return;
            }
            
            value.Disappear(OnComplete);
            
            return;
            void OnComplete()
            {
                _cubePoll.ReturnCube(value.gameObject);
            }
        }

        #region Utility
        
        private Color[] GetNearbyColor(int x, int y, int z)
        {
            var (nb, count) = GetNearbyCubes(x, y, z);
            var colors = new Color[count];
            
            for (var i = 0; i < count; i++)
            {
                colors[i] = _cubeList[nb[i]].cubeColor;
            }

            return colors;
        }

        private readonly (int, int, int)[] _nb = new (int, int, int)[8];

        private ((int, int, int)[], int) GetNearbyCubes(int x, int y, int z)
        {
            var count = 0;
            foreach (var (dx, dy, dz) in AllDirections)
            {
                var key = (x + dx, y + dy, z + dz);
                if (!_cubeList.ContainsKey(key)) continue;
                _nb[count] = key;
                count++;
            }

            return (_nb, count);
        }

        private static readonly (int, int, int)[] AllDirections =
        {
            (0, 0, 1),
            (0, 0, -1),
            (0, 1, 0),
            (0, -1, 0),
            (1, 0, 0),
            (-1, 0, 0)
        };

        #endregion
    }
    
    public class CubePoll
    {
        private readonly Queue<GameObject> _pool = new();
        private readonly Transform _container;
        private readonly GameObject _prefab;

        private readonly Func<Transform, GameObject, GameObject> _factory; // Note: factory function to create a new cube
        private readonly Action<CubeController> _createCube;
        private readonly Action<CubeController> _destroyCube;
        
        public CubePoll(Transform container, GameObject prefab, Func<Transform, GameObject, GameObject> factory)
        {
            _container = container;
            _prefab = prefab;
            _factory = factory;
        }

        public GameObject GetCubeObject()
        {
            if (_pool.Count == 0)
            {
                return InstantiateCube();
            }

            var cube = _pool.Dequeue();
            return cube;
        }

        public void ReturnCube(GameObject cube)
        {
            _pool.Enqueue(cube);
            Debug.LogError(_pool.Count);
        }

        private GameObject InstantiateCube()
        {
            return _factory(_container,_prefab);
        }
    }
}