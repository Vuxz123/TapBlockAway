using System.Collections.Generic;
using UnityEngine;

namespace com.ethnicthv.Game.Cube
{
    public static class CubeUtil
    {
        public static List<CubeController> GetCubeOn(int mapSize, (int, int, int) posStart, CubeDirection direction)
        {
            // get the cube that on the direction of the cube
            var (x, y, z) = posStart;
            var currentVector = new Vector3(x, y, z);
            var directionVector = DirectionMapping[(int)direction];

            var start = GetMovingValue(x, y, z, direction);

            var step = SignMap[(int)direction];
            var max = mapSize / 2 * step + step;

            var cubes = new List<CubeController>();
            
            for (var i = start + step; i != max; i += step)
            {
                currentVector += directionVector;
                var cube = CubeManager.instance.GetCube(currentVector);
                if (!cube) continue;
                cubes.Add(cube);
            }

            return cubes;
        }
        
        public static int GetCubeOnNonAlloc(int mapSize, (int, int, int) posStart, CubeDirection direction, CubeController[] returnArray)
        {
            // get the cube that on the direction of the cube
            var (x, y, z) = posStart;
            var currentVector = new Vector3(x, y, z);
            var directionVector = DirectionMapping[(int)direction];

            var start = GetMovingValue(x, y, z, direction);

            var step = SignMap[(int)direction];
            var max = mapSize / 2 * step + step;

            var count = 0;
            
            for (var i = start + step; i != max; i += step)
            {
                currentVector += directionVector;
                var cube = CubeManager.instance.GetCube(currentVector);
                if (!cube) continue;
                returnArray[count] = cube;
                count++;
            }

            return count;
        }

        public static int GetMovingValue(int x, int y, int z, CubeDirection direction)
        {
            return direction switch
            {
                CubeDirection.Up => y,
                CubeDirection.Down => y,
                CubeDirection.Left => x,
                CubeDirection.Right => x,
                CubeDirection.Forward => z,
                CubeDirection.Backward => z,
                _ => 0
            };
        }

        public static readonly Dictionary<int, Vector3> DirectionMapping = new Dictionary<int, Vector3>
        {
            { 0, Vector3.up },
            { 1, Vector3.down },
            { 2, Vector3.left },
            { 3, Vector3.right },
            { 4, Vector3.forward },
            { 5, Vector3.back }
        };

        public static readonly Dictionary<int, int> SignMap = new Dictionary<int, int>
        {
            { 0, 1 },
            { 1, -1 },
            { 2, -1 },
            { 3, 1 },
            { 4, 1 },
            { 5, -1 }
        };
    }
}