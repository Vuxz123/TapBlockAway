using com.ethnicthv.LevelCreator;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.ethnicthv.Editor.LevelCreator
{
    public class PositionMapsCorrector : EditorWindow
    {
        private string directory = "";
        
        [MenuItem("Window/Position Maps Corrector")]
        public static void ShowWindow()
        {
            GetWindow<PositionMapsCorrector>("Position Maps Corrector");
        }
        
        public void OnGUI()
        {
            GUILayout.Label("Position Maps Corrector", EditorStyles.boldLabel);
            directory = EditorGUILayout.TextField("Directory", directory);
            GUILayout.Label("Select the directory where the map files are located", EditorStyles.helpBox);
            
            if (SceneManager.GetActiveScene().name != "LevelGenerator")
            {
                GUILayout.Label("Please open LevelCreator scene", EditorStyles.boldLabel);
                GUILayout.Label("Open in [Tools/Open Level Creator]", EditorStyles.helpBox);
                return;
            }
            
            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("Please enter play mode to correct maps position", EditorStyles.boldLabel);
            }
            else if (directory == "")
            {
                GUILayout.Label("Please enter a directory", EditorStyles.boldLabel);
            }
            else
            {
                if (GUILayout.Button("Correct Maps Position"))
                {
                    var positionCorrector = FindObjectOfType<PositionCorrector>();
                    // get all files in the directory
                    var files = System.IO.Directory.GetFiles(directory);
                    // filter out non-json files
                    files = System.Array.FindAll(files, s => s.EndsWith(".json"));
                    // replace / with \ in the path
                    for (var i = 0; i < files.Length; i++)
                    {
                        files[i] = files[i].Replace("\\", "/");
                    }
                    positionCorrector.CorrectMapsPosition(files);
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