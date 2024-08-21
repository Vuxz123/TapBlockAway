using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        }
        
        public void OnGUI()
        {
            // Text field with help text and hint
            GUILayout.Label("Generate Map", EditorStyles.boldLabel);
            
            _directory = EditorGUILayout.TextField("Directory", _directory);
            GUILayout.Label("Select the directory where the map files are located", EditorStyles.helpBox);
            
            if (SceneManager.GetActiveScene().name != "LevelGenerator")
            {
                GUILayout.Label("Please open LevelCreator scene", EditorStyles.boldLabel);
                GUILayout.Label("Open in [Tools/Open Level Creator]", EditorStyles.helpBox);
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("Please enter play mode to generate map", EditorStyles.boldLabel);
            }
            else if (_directory == "")
            {
                GUILayout.Label("Please enter a directory", EditorStyles.boldLabel);
            }
            else
            {
                if (GUILayout.Button("Generate Map"))
                {
                    _mapGen = FindObjectOfType<PlayableMapGenerator>();
                    // get all files in the directory
                    var files = System.IO.Directory.GetFiles(_directory);
                    // filter out non-json files
                    files = System.Array.FindAll(files, s => s.EndsWith(".json"));
                    // replace / with \ in the path
                    for (var i = 0; i < files.Length; i++)
                    {
                        files[i] = files[i].Replace("\\", "/");
                    }
                    _mapGen.GenerateMaps(files);
                }
            }
            
            if (GUILayout.Button("Update Addressable"))
            {
                //Mark all file in the directory as addressable
                AddressableAssetSettings.BuildPlayerContent(out var result);
                var success = string.IsNullOrEmpty(result.Error);

                if (!success)
                {
                    Debug.LogError("Addressables build error encountered: " + result.Error);
                }
                else
                {
                    Debug.Log("Addressables build success");
                }
            }
        }
        
    }
}