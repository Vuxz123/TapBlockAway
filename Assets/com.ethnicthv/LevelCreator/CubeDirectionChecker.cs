using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.ethnicthv.Game.Cube;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.ethnicthv.LevelCreator
{
    public class CubeDirectionChecker : MonoBehaviour
    {
        [SerializeField] private CubeController cubeController;

        private int _mapSize;
        
        private static CubeController[] _nextCubes = new CubeController[15];
        
        private List<CubeDirection> _directions = new()
        {
            CubeDirection.Up,
            CubeDirection.Down,
            CubeDirection.Left,
            CubeDirection.Right,
            CubeDirection.Forward,
            CubeDirection.Backward
        };
        
        readonly Stack<CubeController> _stack = new();
        
        bool[] _checked;
        
        public IEnumerator SetupPlayableDirection(int mapSize)
        {
            Array.Clear(_nextCubes, 0 , _nextCubes.Length);
            _mapSize = mapSize;
            _checked = new bool[mapSize * mapSize * mapSize];
            recheck:
            Array.Fill(_checked, false);
            var t = CheckDirectionAsync(cubeController.direction);
            yield return t.Yield();
            Debug.Log($"Cube {cubeController.key} direction {cubeController.direction} is {t.Result}");
            if(!t.Result) yield break;
            yield return new WaitForSeconds(0.01f);
            
            // get Random direction different from current direction
            _directions.Remove(cubeController.direction);
            if (_directions.Count == 0)
            {
                goto end;
            }

            var direction = _directions[Random.Range(0, _directions.Count)];
            cubeController.direction = direction;
            goto recheck;
            end: ;
        }
        
        public bool CheckDirection(CubeDirection direction, (int,int,int) rootKey)
        {
            {
                var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cubeController.key, direction, _nextCubes);
                for (var i = 0; i < c; i++)
                {
                    var controller = _nextCubes[i];
                    _stack.Push(controller);
                }
            }
            while (_stack.TryPop(out var cube) )
            {
                if (cube.key == rootKey) return true;
                var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cubeController.key, direction, _nextCubes);
                for (var i = 0; i < c; i++)
                {
                    var controller = _nextCubes[i];
                    _stack.Push(controller);
                }
            }
            return false;
        }
        private Task<bool> CheckDirectionAsync(CubeDirection direction)
        {
            return Task.Run(() =>
            {
                {
                    var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cubeController.key, direction, _nextCubes);
                    Debug.Log("Number of Next Cubes: " + c);
                    var temp = "";
                    for (var i = 0; i < c; i++)
                    {
                        temp += _nextCubes[i].key + " ";
                        var controller = _nextCubes[i];
                        _stack.Push(controller);
                    }
                    Debug.Log($"Next Cubes: {temp}");
                    PrintStack();
                    Debug.Log("Start Checking " + cubeController.key);
                }
                while (_stack.TryPop(out var cube) )
                {
                    Debug.Log($"{cubeController.key} - Checking Cube: {cube.key} - stack: {_stack.Count}");
                    PrintStack();
                    if (cube.key == cubeController.key) return Task.FromResult(true);
                    var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cube.key, cube.direction, _nextCubes);
                    Debug.Log("Number of Next Cubes: " + c);
                    var temp = "";
                    for (var i = 0; i < c; i++)
                    {
                        temp += _nextCubes[i].key + " ";
                        
                        var controller = _nextCubes[i];
                        
                        if (controller.key == cubeController.key) return Task.FromResult(true);
                        
                        var posX = controller.key.Item1 + _mapSize / 2;
                        var posY = controller.key.Item2 + _mapSize / 2;
                        var posZ = controller.key.Item3 + _mapSize / 2;
                        var index = posX + posY * _mapSize + posZ * _mapSize * _mapSize;
                        if (_checked[index]) continue;
                        _checked[index] = true;
                        _stack.Push(controller);
                    }
                    Debug.Log($"Next Cubes: {temp}");
                }
                return Task.FromResult(false);
            });
        }

        private void PrintStack()
        {
            var temp = _stack.Aggregate("", (current, cube) => current + (cube.key + " "));
            Debug.Log($"Stack: {temp}");
        }
    }
}