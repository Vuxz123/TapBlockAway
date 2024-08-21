using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.ethnicthv.Game.Cube
{
    public class CubeManager : MonoBehaviour
    {
        public static CubeManager instance { get; private set; }

        public Transform cubeContainer;

        public GameObject cubePrefab;
        public int cubeMoveDistance = 10;
        public int cubeMoveSpeed = 2;
        public LayerMask enableLayerMask = 0;
        public int enableLayer = 0;
        public int disableLayer = 0;

        public float cubeMoveDuration => cubeMoveDistance / (float)cubeMoveSpeed;

        public int bound
        {
            get => _bounds.Item1;
            set => _bounds = (value, value, value);
        }

        private CubePoll _cubePoll;
        private readonly Dictionary<(int, int, int), CubeController> _cubeList = new();
        private (int, int, int) _bounds = (10, 10, 10);
        
        // <-- cache -->
        private int _cubeCount;
        // <-- end -->
        
        public event Action OnAllCubeMoved = () => { };

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
            return GetCube(key);
        }

        [CanBeNull]
        public CubeController GetCube(Vector3 pos)
        {
            var key = ((int)pos.x, (int)pos.y, (int)pos.z);
            return GetCube(key);
        }

        #endregion

        #region Cube Contruction

        public CubeController PrepareCube(int x, int y, int z, CubeDirection direction)
        {
            var key = (x, y, z);
            
            if (_cubeList.TryGetValue(key, out var prepareCube)) return prepareCube;
            
            var cube = _cubePoll.GetCubeObject().GetComponent<CubeController>();
            
            cube.transform.position = new Vector3(x, y, z);
            cube.Setup(key, GetNearbyColor(x, y, z), direction);
            _cubeList.Add(key, cube);
            
            _cubeCount++;
            
            return cube;
        }

        public CubeController CreateCube(int x, int y, int z, CubeDirection direction, bool show = false)
        {
            var cube = PrepareCube(x, y, z, direction);

            if (show)
            {
                cube.Appear();
            }
            
            return cube;
        }

        public void DestroyCube(int x, int y, int z, bool animated = true)
        {
            var key = (x, y, z);
            
            if (!_cubeList.Remove(key, out var value)) return;
            
            if (animated)
            {
                value.Disappear(OnComplete);
                return;
            }
            
            value.gameObject.SetActive(false);
            transform.localScale = Vector3.one;
            OnComplete();
            
            return;
            void OnComplete()
            {
                _cubePoll.ReturnCube(value.gameObject);
            }
        }

        #endregion
        
        public void ResetCubeCache()
        {
            _cubeCount = 0;
            _cubeList.Clear();
        }
        
        public void CallMoveCube()
        {
            _cubeCount--;
        }
        
        public int GetCubeCount()
        {
            return _cubeCount;
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

        public void CallAllCubeMoved()
        {
            OnAllCubeMoved();
        }
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
        }

        private GameObject InstantiateCube()
        {
            var go = _factory(_container, _prefab);
            go.SetActive(false);
            return go;
        }
    }
}