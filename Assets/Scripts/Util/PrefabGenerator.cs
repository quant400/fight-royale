
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if true && UNITY_EDITOR
public class PrefabGenerator : MonoBehaviour
{

    private static Transform targetTransform;

    private static List<GameObject> _gameObjectList;

    private static List<GameObject> _prefabList;

    private static string path = "Assets/Resources/Environment/Academy/";
    
    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
        [MenuItem("Red/Create Prefabs")]
        static void CreatePrefab()
        {
            try
            {
                //targetTransform = GameObject.Find("RedScene").transform;
                if(Selection.activeTransform != null)
                    targetTransform = Selection.activeTransform;
                else
                    Debug.Log("Objeto não selecionado");
            
                _gameObjectList = new List<GameObject>();
                _prefabList = new List<GameObject>();
            
                // Keep track of the currently selected GameObject(s)
                AddGameobjectsToList(targetTransform);
    
                // Loop through every GameObject in the array above
                foreach (GameObject gameObject in _gameObjectList)
                {
                    if (_prefabList.Select(auxPrefab => auxPrefab.name).Contains(gameObject.name.Split('.').First()))
                    {
                        ReplaceGameObject(gameObject);
                    }
                    else
                    {
                        CreatePrefab(gameObject);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        

        private static void ReplaceGameObject(GameObject oldGameObject)
        {
            var oldName = oldGameObject.name.Split('.').First();
            
            GameObject newGameObject = _prefabList.FirstOrDefault(auxPrefab => auxPrefab.name == oldName);

            if (newGameObject == null) return;

            newGameObject = PrefabUtility.InstantiatePrefab(newGameObject as GameObject,targetTransform) as GameObject;

            //Setup Position
            newGameObject.transform.position = oldGameObject.transform.position;
            newGameObject.transform.rotation = oldGameObject.transform.rotation;
            newGameObject.transform.SetSiblingIndex(oldGameObject.transform.GetSiblingIndex());
            
            DestroyImmediate(oldGameObject);
        }

        private static void CreatePrefab(GameObject gameObject)
        {
            var goName = gameObject.name.Split('.').First();

            // Set the path as within the Assets folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = path + goName + ".prefab";
            
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
    
            // Make sure the file name is unique, in case an existing Prefab has the same name.
            
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            
    
            // Create the new Prefab.
            _prefabList.Add(PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction));

            gameObject.name = goName;
        }

        private static void AddGameobjectsToList(Transform auxTransform)
        {
            foreach (Transform item in auxTransform)
            {
                _gameObjectList.Add(item.gameObject);
            }
        }
}

#endif