using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.ethnicthv.Game.Map
{
    public class MapManager : MonoBehaviour
    {
        private static Camera _camera;
        
        public bool isMapLoaded { get; private set; }
        
        public void LoadMap(string part)
        {
            isMapLoaded = false; // Note: re set the map loaded flag
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

            for (var x = 0; x < mapJson.size; x++)
            {
                for (var y = 0; y < mapJson.size; y++)
                {
                    for (var z = 0; z < mapJson.size; z++)
                    {
                        var index = x + y * mapJson.size + z * mapJson.size * mapJson.size;
                        if (index >= mapJson.map.Length) continue;
                        var cube = (CubeDirection) mapJson.map[index];
                        var posX = x-a;
                        var posY = y-a;
                        var posZ = z-a;
                        CubeManager.instance.CreateCube(posX,posY,posZ,cube);
                    }
                }
            }

            #endregion

            #region Camera Setup

            GamePlayManager.instance.cameraDistance = mapJson.size*2.2f;
            _camera!.transform.position = new Vector3(0,0,-mapJson.size*2.2f);
            // set fog 
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.white;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.01f;
            // set fog distance
            RenderSettings.fogStartDistance = a +7;
            #endregion

            #region Cube Manager Setup
            
            CubeManager.instance.boundX = a + 10;
            CubeManager.instance.boundY = a + 10;
            CubeManager.instance.boundZ = a + 10;

            #endregion
            
            isMapLoaded = true; // Note: set the map loaded flag
        }

        private void Start()
        {
            _camera = Camera.main;
        }
    }
}
