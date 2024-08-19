using com.ethnicthv.Game.Map;
using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor.Map
{
    [CustomEditor(typeof(MapManager))]
    public class MapManagerEditor : UnityEditor.Editor
    {
        private string _mapPath = "Assets/com.ethnicthv/R/Map/map1.json";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mapManager = (MapManager) target;
            _mapPath = EditorGUILayout.TextField("Map Path", _mapPath);
            if (mapManager.isMapLoaded)
            {
                EditorGUILayout.LabelField("Map Loaded");
                if (GUILayout.Button("Show Map"))
                {
                    mapManager.ShowMap();
                }
            }
            if (GUILayout.Button("Load Map"))
            {
                mapManager.LoadMap(_mapPath);
            }
        }
    }
}