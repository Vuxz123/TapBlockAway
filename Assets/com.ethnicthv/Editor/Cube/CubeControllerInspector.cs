using com.ethnicthv.Game.Cube;
using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor.Cube
{
    [CustomEditor(typeof(CubeController))]
    public class CubeControllerInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var cubeController = (CubeController) target;
            if (GUILayout.Button("Move"))
            {
                cubeController.Move();
            }
        }
    }
}