# Game Map
## Introduction
The Map System of the game is a 1D array of int that stores the information of the cubes in the game. A map is stored as a json file in the `Assets/Resources/Maps` folder.

__( IMPORTANT: Remember to mark the file as Addressable in the Unity Editor)__
## Structure
A map json file is structured as follows:
```json
{
    "size": 10,
    "map": []
}
```
- `size`: The size of the map. The map is a square, so the size is the number of cubes in a row.
- `map`: The 1D array of int that stores the information of the cubes. The array is read row by row. Map size is `size * size * size.`

### Map Values
- '-1' : Empty space
- '0' : Up
- '1' : Down
- '2' : Left
- '3' : Right
- '4' : Forward
- '5' : Backward

## Map Operations
### Get Actual Position
- From the index of a cube in the map:
```csharp
public Vector3 GetActualPosition(int index) {
    int a = size / 2;
    int x = index % size - a;
    int y = (index / size) % size - a;
    int z = index / (size * size) - a;
    return new Vector3(x, y, z);
}
```
- Loop through the map:
```csharp
var a = mapJson.size/2;

CubeManager.instance.ResetCubeCache(); // Note: reset the cube cache
            
for (int x = 0; x < mapJson.size; x++) {
    for (int y = 0; y < mapJson.size; y++) {
        for (int z = 0; z < mapJson.size; z++) {
            int index = x + y * mapJson.size + z * mapJson.size * mapJson.size;
            int value = mapJson.map[index];
            if (value == -1) return; // Note: skip empty space
            
            CubeDirection direction = (CubeDirection)mapJson.map[index];
            
            int posX = x - a;
            int posY = y - a;
            int posZ = z - a;
        }
    }
}
```