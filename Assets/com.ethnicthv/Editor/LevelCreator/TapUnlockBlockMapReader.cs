using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace com.ethnicthv.Editor.LevelCreator
{
    public class TapUnlockBlockMapReader : EditorWindow
    {
        private string _directory = "";

        private int _startLevel;

        private string _mapCategory = "Normal";

        private int _numMapsRead;

        [MenuItem("Window/Map Reader/Tap Unlock Block Map Reader")]
        public static void ShowWindow()
        {
            GetWindow<TapUnlockBlockMapReader>("Tap Unlock Block Map Reader");
        }

        public void OnGUI()
        {
            // Text field with help text and hint
            GUILayout.Label("Tap Unlock Block Map Reader", EditorStyles.boldLabel);

            _directory = EditorGUILayout.TextField("Directory", _directory);
            GUILayout.Label("Select the directory where the map files are located", EditorStyles.helpBox);

            _startLevel = EditorGUILayout.IntField("Start Level", _startLevel);
            GUILayout.Label("The level number to start from", EditorStyles.helpBox);

            _mapCategory = EditorGUILayout.TextField("Map Category", _mapCategory);
            GUILayout.Label("The category of the map", EditorStyles.helpBox);

            if (GUILayout.Button("Read Map Files"))
            {
                Debug.Log("Reading Map Files");
                ReadMapFiles();
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
                    Debug.Log("Addressables build completed successfully.");
                }
            }

            GUILayout.Label("Number of Maps Read: " + _numMapsRead);
        }

        private void ReadMapFiles()
        {
            // Read all files in the directory
            var files = System.IO.Directory.GetFiles(_directory);
            var level = _startLevel;
            foreach (var file in files)
            {
                // check if the file is a asset file
                if (file.Contains(".meta")) continue;
                Debug.Log("Reading file: " + file);
                // Read the file
                var mapData = System.IO.File.ReadAllText(file);
                ReadMap(level, mapData);
                level++;
            }
        }

        private void ReadMap(int level, string input)
        {
            // separate the input into lines
            var lines = input.Split('\n');
            // Find line with "Blocks:"
            var blocksIndex = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains("Blocks:")) continue;
                blocksIndex = i;
                break;
            }

            var blockList = new List<(int, int, int)>();

            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var minZ = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var maxZ = int.MinValue;

            // Read the blocks
            for (var i = blocksIndex + 1; i < lines.Length; i++)
            {
                if (!lines[i].Contains("- Position:")) continue;
                // get the position in {x: x, y: y, z: z} format
                var pos = lines[i].Split('}')[0].Split("{")[1].Split(",");
                var x = int.Parse(pos[0].Split(":")[1].Trim());
                var y = int.Parse(pos[1].Split(":")[1].Trim());
                var z = int.Parse(pos[2].Split(":")[1].Trim());

                // update the min and max values
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (z > maxZ) maxZ = z;

                blockList.Add((x, y, z));
            }

            var mapSize = Mathf.Max(maxX - minX, maxY - minY, maxZ - minZ) + 1;
            var map = new Game.Map.Map
            {
                size = mapSize,
                map = new int[mapSize * mapSize * mapSize]
            };

            // Fill the map with -1
            Array.Fill(map.map, -1);

            blockList.ForEach(block =>
            {
                var x = block.Item1 - minX;
                var y = block.Item2 - minY;
                var z = block.Item3 - minZ;
                var index = x + y * mapSize + z * mapSize * mapSize;
                map.map[index] = 0;
            });

            // Save the map
            var json = JsonUtility.ToJson(map);
            
            // check if the directory exists
            if (!System.IO.Directory.Exists("Assets/com.ethnicthv/R/Map" + "/" + _mapCategory))
            {
                System.IO.Directory.CreateDirectory("Assets/com.ethnicthv/R/Map" + "/" + _mapCategory);
            }
            
            var path = "Assets/com.ethnicthv/R/Map" + "/" + _mapCategory + "/" + level + ".json";
            
            System.IO.File.WriteAllText(path, json);

            // Increment the number of maps read
            _numMapsRead++;

            // Log the map read
            Debug.Log("Map Read: " + level);
        }
    }
}