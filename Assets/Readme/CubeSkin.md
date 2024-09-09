# Cube Skin

Cube Skin is a Class type that define what the Cube look. It contains properties like mesh, and material.

## Adding new Cube Skin
To add a new Cube Skin, you open the 'CubeSkinDatabase' instance in the folder 'Assets/com.ethnicthv/Game/Data', and add a new Cube Skin to the list.

A new Cube Skin include a new mesh and a new material to be assigned to the cube.

## Material
The material is a shader type 'Cube' that can be found in the folder `Assets/com.ethnicthv/R/Model/Materials`.

__Important note__: If u want to change the color of the cube, pls add a Color property named 'Color' to the material. This property is essential for the shader to be work with current implementation.

### Texture
If you want to change the texture of the cube, you can create a new material of shader type 'Cube' that can be found in the folder `Assets/com.ethnicthv/R/Model/Materials`. Then you can assign the material to the cube skin.

__Important note__: Every black pixel in the texture will not change color when the cube color is changed.

## Mesh
Current implementation only support one large cube mesh. If you want to use multiple mesh, it will need a refactor in the code (This might include `Skin.cs`, `CubeController.cs`, `Cube.prefab`)

## How current skin works
The cube skin is assigned to the cube in the `CubeController` script. Cube now use a mesh, a material, and a color to display the cube. The Cube Color is the color of the cube. When you use 'SetSkin' method in the `CubeController`, it will assign the mesh, material to the cube, and color which was cache in CubeController will be assigned to the Cube.

So if you want more complex skin with multiple mesh, material, and color, you will need to do it for every cube skin. Not needed part can be disabled by your code (this might take a lot to do). 