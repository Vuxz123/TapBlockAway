using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.ethnicthv.Game.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private int numberOfCubesShowedPerFrame = 10;
        
        public bool isMapLoaded { get; private set; }
        public bool isMapShowed { get; private set; }
        
        // <-- cache -->
        private PriorityQueue<CubeController, float> _cubeQueue;
        private List<CubeController> _cubes;
        // <-- end -->

        private void Awake()
        {
            _cubeQueue = new PriorityQueue<CubeController, float>(new CubeComparer());
        }

        public void LoadMap(string part)
        {
#if UNITY_EDITOR
            Debug.Log("Loading Map: " + part);
#endif
            isMapLoaded = false; // Note: re set the map loaded flag
            isMapShowed = false; // Note: re set the map showed flag
            StopAllCoroutines();
            var mapText = Addressables.LoadAssetAsync<TextAsset>(part);
            StartCoroutine(BeginBuildMap(mapText));
        }

        private IEnumerator BeginBuildMap(AsyncOperationHandle<TextAsset> op)
        {
            if (CubeManager.instance.HideAllCubes())
            {
                yield return new WaitForSeconds(1);
            }
            yield return new WaitUntil(() => op.IsDone); //Note: wait for the operation to complete
            
            var mapText = op.Result.text; 
            
            Addressables.Release(op); //Note: release the operation handle
            
            var mapJson = JsonUtility.FromJson<Map>(mapText);
            
            GamePlayManager.instance.mapSize = mapJson.size;
            
            var a = mapJson.size/2;

            #region Cube Creation
            
            CubeManager.instance.ResetCubeCache(); // Note: reset the cube cache
            
            for (var x = 0; x < mapJson.size; x++)
            {
                for (var y = 0; y < mapJson.size; y++)
                {
                    for (var z = 0; z < mapJson.size; z++)
                    {
                        var index = x + y * mapJson.size + z * mapJson.size * mapJson.size;
                        if (index >= mapJson.map.Length) continue;
                        var pointData = mapJson.map[index];
                        if (pointData == -1) continue; // Note: -1 means no cube (null)
                        var cube = (CubeDirection) pointData;
                        var posX = x-a;
                        var posY = y-a;
                        var posZ = z-a;
                        
                        //calculate the distance from the center
                        var distance = Vector3.Distance(Vector3.zero, new Vector3(posX, posY, posZ));
                        _cubeQueue.Enqueue(CubeManager.instance.PrepareCube(posX, posY, posZ, cube), distance);
                    }
                }
            }

            #endregion

            #region Camera Setup
            
            var cameraController = GamePlayManager.instance.cameraController;
            
            cameraController.maxCameraDist = - mapJson.size*1.2f;
            cameraController.minCameraDist = - mapJson.size*3f;
            cameraController.cameraDist = - mapJson.size*2.2f;
            
            cameraController.cameraShiftX = !mapJson.shiftX;
            cameraController.cameraShiftY = !mapJson.shiftY;
            cameraController.cameraShiftZ = !mapJson.shiftZ;
            
            cameraController.cameraRoot.rotation = Quaternion.Euler(30, 45, 0);
            
            // set fog 
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.white;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.01f;
            #endregion

            #region Cube Manager Setup
            
            CubeManager.instance.bound = a + 10;

            #endregion

            #region Self Setup

            numberOfCubesShowedPerFrame = _cubeQueue.Count / 10;
            
            #endregion
            
            isMapLoaded = true; // Note: set the map loaded flag
#if UNITY_EDITOR
            Debug.Log("Map Loaded");
#endif
        }
        
        public void ShowMap(Action onShowed = null)
        {
            Debug.Log("Show Map");
            StartCoroutine(ShowMapCoroutine(onShowed));
        }
        
        private IEnumerator ShowMapCoroutine(Action onShowed = null)
        {
            var count = 0;
            
            if (_cubeQueue.Count <= 0) goto list;
            
            // Note: Use a Queue to show the cubes
            _cubes = new List<CubeController>(_cubeQueue.Count);
            
            while (_cubeQueue.Count > 0)
            {
                count++;
                var cube = _cubeQueue.Dequeue();
                cube.Appear();
                _cubes.Add(cube);
                if (count < numberOfCubesShowedPerFrame) continue;
                count = 0;
                yield return new WaitForSeconds(0.001f);
            }

            goto end;
            
            // Note: Use a List to show the cubes (this is for the reshowing the cubes)
            list:
            if (_cubes == null)
            {
                throw new Exception("Cubes list for reshowing is null!!!");
            }
            
            foreach (var t in _cubes)
            {
                t.ReStart();
                t.Appear();
                if (count < numberOfCubesShowedPerFrame) continue;
                count = 0;
                yield return new WaitForSeconds(0.001f);
            }
            
            end:
            isMapShowed = true;
            onShowed?.Invoke();
        }
        
        public void HideMap(Action onHided = null)
        {
            Debug.Log("Hide Map");
            StartCoroutine(HideMapCoroutine(onHided));
        }

        private IEnumerator HideMapCoroutine(Action onHided = null)
        {
            Debug.Log("Hiding Map 1");
            if (_cubes == null)
            {
                Debug.LogError("You cannot hide the map if the map is not shown!!!");
                yield break;
            }
            Debug.Log("Hiding Map 2 - " + _cubes.Count);
            var count = 0;
            for (int i = _cubes.Count - 1; i >= 0; i--)
            {
                count++;
                _cubes[i].Disappear();
                Debug.Log($"Cube {i} Disappear");
                if (count < numberOfCubesShowedPerFrame) continue;
                count = 0;
                yield return new WaitForSeconds(0.001f);
            }
            
            isMapShowed = false;
            onHided?.Invoke();
        }

        private class CubeComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
