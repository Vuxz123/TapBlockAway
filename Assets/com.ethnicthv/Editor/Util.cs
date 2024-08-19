using UnityEditor;

namespace com.ethnicthv.Editor
{
    public class Util : UnityEditor.Editor
    {
        [MenuItem("Tools/Open Level Creator")]
        public static void OpenLevelCreator()
        {
            //Open Scene
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/LevelGenerator.unity");
        }
        
        [MenuItem("Tools/Open Game")]
        public static void OpenGame()
        {
            //Open Scene
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GamePlay.unity");
        }
    }
}