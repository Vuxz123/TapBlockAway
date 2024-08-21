using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.ethnicthv.Game.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private int numberOfCubesShowedPerFrame = 10;
        
        public bool isMapLoaded { get; private set; }
        
        // <-- cache -->
        private PriorityQueue<CubeController, float> _cubeQueue;
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
            StopAllCoroutines();
            var mapText = Addressables.LoadAssetAsync<TextAsset>(part);
            StartCoroutine(BeginBuildMap(mapText));
        }

        private IEnumerator BeginBuildMap(AsyncOperationHandle<TextAsset> op)
        {
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
            cameraController.maxCameraDistance = - mapJson.size*1.2f;
            cameraController.minCameraDistance = - mapJson.size*3f;
            cameraController.cameraDist = - mapJson.size*2.2f;
            cameraController.cameraShift = mapJson.size % 2 == 0;
            
            // set fog 
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.white;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.01f;
            #endregion

            #region Cube Manager Setup
            
            CubeManager.instance.bound = a + 10;

            #endregion
            
            isMapLoaded = true; // Note: set the map loaded flag
#if UNITY_EDITOR
            Debug.Log("Map Loaded");
#endif
        }
        
        public void ShowMap()
        {
            StartCoroutine(ShowMapCoroutine());
        }
        
        private IEnumerator ShowMapCoroutine()
        {
            var count = 0;
            while (_cubeQueue.Count > 0)
            {
                count++;
                var cube = _cubeQueue.Dequeue();
                cube.Appear();
                if (count < numberOfCubesShowedPerFrame) continue;
                count = 0;
                yield return new WaitForSeconds(0.001f);
            }
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
