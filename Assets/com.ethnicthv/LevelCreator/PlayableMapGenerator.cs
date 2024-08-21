using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Map;
using com.ethnicthv.LevelCreator;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayableMapGenerator : MonoBehaviour
{
    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private string directory = "Assets/com.ethnicthv/R/Map/map1.json";
    [SerializeField] private GameObject cubePrefab;

    public List<CubeController> cubes = new();

    private Map _mapJson;

    private bool _doneGenerating;
    private bool _available;

    public void Generate()
    {
        _available = false;
        _doneGenerating = false;
        foreach (var cubeController in cubes)
        {
            cubeManager.DestroyCube(cubeController.key.Item1, cubeController.key.Item2, cubeController.key.Item3,
                false);
        }

        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        cubes.Clear();
        cubeManager.cubePrefab = cubePrefab;
        Debug.Log("Loading Map: " + directory);
        var operation = Addressables.LoadAssetAsync<TextAsset>(directory);

        yield return new WaitUntil(() => operation.IsDone);
        if (operation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load map");
            yield break;
        }

        Debug.Log("Map Loaded");
        var mapData = operation.Result;

        _mapJson = JsonUtility.FromJson<Map>(mapData.text);

        var a = _mapJson.size / 2;

        Debug.Log("Generating Map");
        Debug.Log("Map Size: " + _mapJson.size);

        for (var x = 0; x < _mapJson.size; x++)
        {
            for (var y = 0; y < _mapJson.size; y++)
            {
                for (var z = 0; z < _mapJson.size; z++)
                {
                    var index = x + y * _mapJson.size + z * _mapJson.size * _mapJson.size;
                    if (index >= _mapJson.map.Length) continue;
                    var pointData = _mapJson.map[index];
                    if (pointData == -1) continue;
                    var posX = x - a;
                    var posY = y - a;
                    var posZ = z - a;
                    // get Random direction
                    var direction = (CubeDirection)Random.Range(0, 6);
                    var cube = cubeManager.PrepareCube(posX, posY, posZ, direction);

                    var cubeDirectionChecker = cube.GetComponent<CubeDirectionChecker>();
                    yield return cubeDirectionChecker.SetupPlayableDirection(_mapJson.size);
                    cube.Appear();
                    cubes.Add(cube);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }

        Debug.Log("Map Generated");
        _doneGenerating = true;
    }

    public void SaveMapData()
    {
        Debug.Log("Save Direction Data");
        var bounds = _mapJson.size / 2;

        cubes.ForEach((cube =>
        {
            // convert to map position
            var x = cube.key.Item1 + bounds;
            var y = cube.key.Item2 + bounds;
            var z = cube.key.Item3 + bounds;
            var index = x + y * _mapJson.size + z * _mapJson.size * _mapJson.size;
            _mapJson.map[index] = (int)cube.direction;
        }));
        Debug.Log($"Map Save with {cubes.Count} cubes");

        var json = JsonUtility.ToJson(_mapJson);

        Debug.Log(json);

        var path = directory;
        Debug.Log($"Saving to {path}");
        System.IO.File.WriteAllText(path, json);

        Debug.Log("Map Saved");
        _available = true;
    }

    public void SaveMapDataWhenDone()
    {
        StartCoroutine(SaveMapDataWhenDoneCoroutine());
    }

    private IEnumerator SaveMapDataWhenDoneCoroutine()
    {
        yield return new WaitUntil(() => _doneGenerating);
        SaveMapData();
    }

    private IEnumerator GenerateMapsCoroutine(string[] files)
    {
        var count = 0;
        foreach (var file in files)
        {
            directory = file;
            Generate();
            yield return SaveMapDataWhenDoneCoroutine();
            yield return new WaitUntil(() => _available);
            count++;
        }
        Debug.Log($"{count} Maps Generated");
    }

    public void GenerateMaps(string[] files)
    {
        StartCoroutine(GenerateMapsCoroutine(files));
    }
}