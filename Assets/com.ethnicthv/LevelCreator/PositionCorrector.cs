using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Map;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.ethnicthv.LevelCreator
{
    public class PositionCorrector : MonoBehaviour
    {
        [SerializeField] private string mapPath = "Assets/com.ethnicthv/R/Map/map1.json";
        
        // <-- state -->
        private bool _isMapLoaded;
        private bool _isMapCorrected;
        private bool _isMapSaved;
        // <-- end -->
        
        // <-- cache -->
        private readonly Dictionary<(int, int, int), int> cubes = new();
        private Map _oldMap;
        private Map _newMap;
        // <-- end -->
        
        public void CorrectMapsPosition(string[] mapPaths)
        {
            StartCoroutine(CorrectMapsPositionCoroutine(mapPaths));
        }
        
        private IEnumerator CorrectMapsPositionCoroutine(string[] mapPaths)
        {
            var c = 0;
            foreach (var path in mapPaths)
            {
                mapPath = path;
                yield return LoadMapCoroutine();
                yield return CorrectPositionCoroutine();
                yield return SaveMapCoroutine();
                yield return new WaitUntil(() => _isMapCorrected && _isMapSaved && _isMapLoaded);
                c++;
            }
            Debug.Log("Corrected " + c + " maps");
        }
        
        private IEnumerator LoadMapCoroutine()
        {
            cubes.Clear(); //Note: clear the previous cube cache
            Debug.Log("Load Map: " + mapPath);
            var op = Addressables.LoadAssetAsync<TextAsset>(mapPath);
            yield return new WaitUntil(() => op.IsDone);
            if (op.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load map: " + mapPath);
                yield break;
            }
            var mapText = op.Result.text;
            Addressables.Release(op);
            _oldMap = JsonUtility.FromJson<Map>(mapText);
            _newMap = new Map
            {
                size = _oldMap.size,
                map = new int[_oldMap.size * _oldMap.size * _oldMap.size]
            };
            
            Array.Fill(_newMap.map, -1);
            
            _isMapLoaded = true;
            
        }
        
        public void LoadMap()
        {
            // Note: reset Corrector state
            _isMapLoaded = false;
            _isMapCorrected = false;
            _isMapSaved = false;
            
            // Note: start loading map
            Debug.Log("Load Map: " + mapPath);
            StopAllCoroutines();
            StartCoroutine(LoadMapCoroutine());
        }
        
        public void CorrectPositionWhenMapLoaded()
        {
            StartCoroutine(CorrectPositionCoroutine());
        }

        private IEnumerator CorrectPositionCoroutine()
        {
            yield return new WaitUntil(() => _isMapLoaded);
            CorrectPosition();
        }
        
        public void CorrectPosition()
        {
            Debug.Log("Correct Position!..");
            var a = _oldMap.size/2;

            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var maxZ = int.MinValue;
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var minZ = int.MaxValue;
            
            for (var x = 0; x < _oldMap.size; x++)
            {
                for (var y = 0; y < _oldMap.size; y++)
                {
                    for (var z = 0; z < _oldMap.size; z++)
                    {
                        var index = x + y * _oldMap.size + z * _oldMap.size * _oldMap.size;
                        if (index >= _oldMap.map.Length) continue;
                        var pointData = _oldMap.map[index];
                        if (pointData == -1) continue; // Note: -1 means no cube (null)
                        var posX = x-a;
                        var posY = y-a;
                        var posZ = z-a;
                        
                        if (posX > maxX) maxX = posX;
                        if (posY > maxY) maxY = posY;
                        if (posZ > maxZ) maxZ = posZ;
                        if (posX < minX) minX = posX;
                        if (posY < minY) minY = posY;
                        if (posZ < minZ) minZ = posZ;
                        
                        cubes.Add((posX, posY, posZ), pointData);
                    }
                }
            }

            var xGap = (Math.Abs(maxX) - Math.Abs(minX));
            var yGap = (Math.Abs(maxY) - Math.Abs(minY));
            var zGap = (Math.Abs(maxZ) - Math.Abs(minZ));
            
            _newMap.shiftX = xGap % 2 == 0;
            _newMap.shiftY = yGap % 2 == 0;
            _newMap.shiftZ = zGap % 2 == 0;
            
            var offsetX = xGap / 2;
            var offsetY = yGap / 2;
            var offsetZ = zGap / 2;
            
            foreach (var (key, value) in cubes)
            {
                var x = key.Item1 - offsetX + a;
                var y = key.Item2 - offsetY + a;
                var z = key.Item3 - offsetZ + a;
                _newMap.map[x + y * _oldMap.size + z * _oldMap.size * _oldMap.size] = value;
            }
            
            _isMapCorrected = true;
        }
        
        public void SaveMapWhenMapCorrected()
        {
            StartCoroutine(SaveMapCoroutine());
        }
        
        private IEnumerator SaveMapCoroutine()
        {
            yield return new WaitUntil(() => _isMapCorrected);
            SaveMap();
        }
        
        public void SaveMap()
        {
            Debug.Log("Save Map: " + mapPath);
            var newMapText = JsonUtility.ToJson(_newMap);
            
            System.IO.File.WriteAllText(mapPath, newMapText);
            
            _isMapSaved = true;
        }
        
        public void UpdateAddressable()
        {
            Debug.Log("Update Addressable");
            
            
        }
        
    }
}