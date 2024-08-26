using System;
using com.ethnicthv.Game;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Gameplay;
using com.ethnicthv.Game.Map;
using UnityEditor;
using UnityEngine;

namespace com.ethnicthv.Editor.Cube
{
    public class CubeUtilWindow: EditorWindow    
    {
        private static CubeDirection _cubeDirection = CubeDirection.Up;
        
        private static int _x;
        private static int _y;
        private static int _z;

        [MenuItem("Window/Cube Util")]
        public static void ShowWindow()
        {
            GetWindow<CubeUtilWindow>("Cube Util");
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    //check if GamePlay is in the scene
                    FindObjectOfType<GamePlayManager>();
                }
            };
        }

        private void OnGUI()
        {
            GUILayout.Label("Cube Util", EditorStyles.boldLabel);
            
            if (!EditorApplication.isPlaying) return;
            if (!CubeManager.instance)
            {
                //error message
                GUILayout.Label("Please add GamePlay to the scene", EditorStyles.boldLabel);
                return;
            }

            GUILayout.Label("Waiting for map", EditorStyles.boldLabel);
            if (GamePlayManager.instance.mapManager.isMapLoaded)
            {
                GUILayout.Label("Map is loaded", EditorStyles.boldLabel);
                if(GUILayout.Button("Show Map"))
                {
                    GamePlayManager.instance.mapManager.ShowMap();
                }
            }
            
            _x = EditorGUILayout.IntField("X", _x, GUILayout.MinWidth(20));
            _y = EditorGUILayout.IntField("Y", _y, GUILayout.MinWidth(20));
            _z = EditorGUILayout.IntField("Z", _z, GUILayout.MinWidth(20));
            
            _cubeDirection = (CubeDirection)EditorGUILayout.EnumFlagsField("Cube Direction", _cubeDirection);
            
            if (GUILayout.Button("Create Cube"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) CubeManager.instance.DestroyCube(_x, _y, _z);
                CubeManager.instance.CreateCube(_x, _y, _z, _cubeDirection);
            }
            
            if (GUILayout.Button("Destroy Cube"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) CubeManager.instance.DestroyCube(_x, _y, _z);
            }
            
            if (GUILayout.Button("Move Cube"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.Move();
            }

            if (GUILayout.Button("Appear"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.Appear();
            }
            
            if (GUILayout.Button("Disappear"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.Disappear();
            }
            
            if (GUILayout.Button("FadeOut"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.FadeOut();
            }
            
            if (GUILayout.Button("Enable"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.gameObject.SetActive(true);
            }
            
            if (GUILayout.Button("Disable"))
            {
                var cube = CubeManager.instance.GetCube(_x, _y, _z);
                if (cube) cube.gameObject.SetActive(false);
            }
            
            
        }
    }
}