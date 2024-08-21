using com.ethnicthv.LevelCreator;
using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor.LevelCreator
{
    [CustomEditor(typeof(PositionCorrector))]
    public class PositionCorrectorEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var positionCorrector = (PositionCorrector) target;
            
            if (GUILayout.Button("Load Map"))
            {
                positionCorrector.LoadMap();
            }
            
            if (GUILayout.Button("Correct Position"))
            {
                positionCorrector.CorrectPosition();
            }
            
            if (GUILayout.Button("Save Map"))
            {
                positionCorrector.SaveMap();
            }
            
            if (GUILayout.Button("Update Addressable"))
            {
                positionCorrector.UpdateAddressable();
            }
        }
    }
}