using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Cube.CubeSkin;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CubeSkinDatabase cubeSkinDatabase;
    public CubeController cubeController;
    
    // Start is called before the first frame update
    void Start()
    {
        cubeController.SetSkin(cubeSkinDatabase.GetSkin(1));
        cubeController.cubeColor = Color.red;
        cubeController.cubeAlpha = 0f;
    }
}
