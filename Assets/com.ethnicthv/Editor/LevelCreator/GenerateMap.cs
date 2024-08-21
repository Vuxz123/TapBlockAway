using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor.LevelCreator
{
    public class GenerateMap : EditorWindow
    {
        private string _directory = "";
        
        private PlayableMapGenerator _mapGen;
        
        [MenuItem("Window/Generate Map")]
        public static void ShowWindow()
        {
            var w = GetWindow<GenerateMap>("Generate Map");
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    w._mapGen = FindObjectOfType<PlayableMapGenerator>();
                }
            };
        }
        
        public void OnGUI()
        {
            // Text field with help text and hint
            GUILayout.Label("Generate Map", EditorStyles.boldLabel);
            
            _directory = EditorGUILayout.TextField("Directory", _directory);
            GUILayout.Label("Select the directory where the map files are located", EditorStyles.helpBox);

            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("Please enter play mode to generate map", EditorStyles.boldLabel);
            }
            else
            {
                if (GUILayout.Button("Generate Map"))
                {
                    // get all files in the directory
                    var files = System.IO.Directory.GetFiles(_directory);
                    foreach (var file in files)
                    {
                        Debug.Log("Reading Map File: " + file);
                        _mapGen.Directory = file;
                        _mapGen.Generate();
                        _mapGen.SaveMapDataWhenDone();
                    }
                }
            }
        }
        
    }
}