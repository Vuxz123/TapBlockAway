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
        
        private readonly CubeController[] _nextCubes = new CubeController[15];
        
        private List<CubeDirection> _directions;

        private readonly Stack<CubeController> _stack = new();

        private bool[] _checked;
        
        public IEnumerator SetupPlayableDirection(int mapSize)
        {
            //Debug.Log("<color=red>Start Checking </color>" + cubeController.key);
            
            Array.Clear(_nextCubes, 0 , _nextCubes.Length);
            _mapSize = mapSize;
            _checked = new bool[mapSize * mapSize * mapSize];
            
            
            recheck:
            
            //Note: setup needed variables
            _stack.Clear();
            _directions = new List<CubeDirection>
            {
                CubeDirection.Up,
                CubeDirection.Down,
                CubeDirection.Left,
                CubeDirection.Right,
                CubeDirection.Forward,
                CubeDirection.Backward
            };
            Array.Fill(_checked, false);
            
            //Debug.Log($"<color=yellow>Start Checking </color>{cubeController.key} - {cubeController.direction}");
            var t = CheckDirectionAsync(cubeController.direction);
            yield return t.Yield();
            
            //Debug.Log($"<color=yellow>End Checking </color>{cubeController.key} - {cubeController.direction} is {t.Result}");
            
            if(!t.Result) yield break;
            yield return new WaitForSeconds(0.01f);
            
            // get Random direction different from current direction
            _directions.Remove(cubeController.direction);
            if (_directions.Count == 0)
            {
                //Debug.LogError("<color=red> No Direction Available </color>");
                goto end;
            }

            var direction = _directions[Random.Range(0, _directions.Count)];
            cubeController.direction = direction;
            goto recheck;
            end: ;
        }
        
        private Task<bool> CheckDirectionAsync(CubeDirection direction)
        {
            return Task.Run(() =>
            {
                {
                    var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cubeController.key, direction, _nextCubes);
                    //Debug.Log("         Number of Next Cubes: " + c);
                    //var temp = "";
                    for (var i = 0; i < c; i++)
                    {
                        //temp += _nextCubes[i].key + " ";
                        var controller = _nextCubes[i];
                        _stack.Push(controller);
                    }
                    //Debug.Log($"        Next Cubes: {temp}");
                    //PrintStack();
                }
                while (_stack.TryPop(out var cube) )
                {
                    
                    //Debug.Log($"<color=green>{cubeController.key} - Checking Cube: {cube.key}</color> - stack: {_stack.Count}");
                    
                    //PrintStack();
                    if (cube.key == cubeController.key)
                    {
                        //Debug.Log("<color=blue>Found Root!!</color>");
                        return Task.FromResult(true);
                    }
                    var c = CubeUtil.GetCubeOnNonAlloc(_mapSize, cube.key, cube.direction, _nextCubes);
                    //Debug.Log("         Number of Next Cubes: " + c);
                    //var temp = "";
                    for (var i = 0; i < c; i++)
                    {
                        //temp += _nextCubes[i].key + " ";
                        
                        var controller = _nextCubes[i];
                        
                        if (controller.key == cubeController.key) return Task.FromResult(true);
                        
                        var posX = controller.key.Item1 + _mapSize / 2;
                        var posY = controller.key.Item2 + _mapSize / 2;
                        var posZ = controller.key.Item3 + _mapSize / 2;
                        var index = posX + posY * _mapSize + posZ * _mapSize * _mapSize;
                        if (_checked[index])
                        {
                            //Debug.Log($"       <color=purple>Already Checked</color> {controller.key}");
                            continue;
                        }
                        //Debug.Log($"        <color=purple>Push</color> {controller.key}");
                        _checked[index] = true;
                        _stack.Push(controller);
                    }
                    //Debug.Log($"        Next Cubes: {temp}");
                }
                return Task.FromResult(false);
            });
        }

        private void PrintStack()
        {
            var temp = _stack.Aggregate("", (current, cube) => current + (cube.key + " "));
            Debug.Log($"        Stack: {temp}");
        }
    }
}