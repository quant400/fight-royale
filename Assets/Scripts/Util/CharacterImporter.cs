using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class CharacterImporter : MonoBehaviour
{
    #if UNITY_EDITOR
    
    const string _characterResourcePath = "Assets/Resources/CharactersV2";

    [MenuItem("Mix/ImportCharacter")]
    public static void ImportCharacters()
    {
        //Main path
        //string mainPath = EditorUtility.OpenFolderPanel("Select Character Directory", "", "fbx");

        //GetAllCharacter
        //var info = new DirectoryInfo(mainPath);
        //var allCharacters = info.GetFiles();

        var allCharacters = LoadAssetImportersFromSelection();
        
        if (!AssetDatabase.IsValidFolder(_characterResourcePath))
        {
            AssetDatabase.CreateFolder("Assets/Resources","Characters");
        }
        
        foreach (var characterImporter in allCharacters)
        {
            var characterName = GetCharacterName(characterImporter.assetPath);

            if (!AssetDatabase.IsValidFolder(Path.GetDirectoryName(characterImporter.assetPath) + "/" + characterName))
            {
                AssetDatabase.CreateFolder(Path.GetDirectoryName(characterImporter.assetPath), 
                    characterName);
            }

            var newPath =  $"{Path.GetDirectoryName(characterImporter.assetPath)}/{characterName}/{Path.GetFileName(characterImporter.assetPath)}";
            
            AssetDatabase.MoveAsset(characterImporter.assetPath, newPath);
            
            //Pegar caminhos
            var fullPath = Application.dataPath.Replace("Assets", "") + newPath;
            
            //Alterar Escala
            characterImporter.globalScale = 2.5f;
            characterImporter.SaveAndReimport();

            //Extrair e pegar Textura
            characterImporter.ExtractTextures(Path.GetDirectoryName(fullPath));
            AssetDatabase.Refresh();
            var texturesPaths = AssetDatabase.FindAssets("t:Texture", new []{Path.GetDirectoryName(characterImporter.assetPath)});
            var characterTexture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(texturesPaths[0]));

            //Pegar Mesh
            var characterAsset = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);

            //Criar Scriptable
            Character asset = ScriptableObject.CreateInstance<Character>();

            asset.name = characterName;
            asset.Name = characterName;
            asset.Texture = characterTexture;
            asset.Asset = characterAsset;

            AssetDatabase.CreateAsset(asset, $"{_characterResourcePath}/{characterName}.asset");
            Debug.Log($"{characterName} created!");
        }
        
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Mix/ImportCharacterV2")]
    public static void ImportCharactersV2()
    {
        //Main path
        //string mainPath = EditorUtility.OpenFolderPanel("Select Character Directory", "", "fbx");

        //GetAllCharacter
        //var info = new DirectoryInfo(mainPath);
        //var allCharacters = info.GetFiles();

        var allCharacters = LoadAssetImportersFromSelection();
        
        if (!AssetDatabase.IsValidFolder(_characterResourcePath))
        {
            AssetDatabase.CreateFolder("Assets/Resources","Characters");
        }
        
        foreach (var characterImporter in allCharacters)
        {
            var characterName = GetCharacterName(characterImporter.assetPath);

            var newPath =  $"{Path.GetDirectoryName(characterImporter.assetPath)}/{Path.GetFileName(characterImporter.assetPath)}";
            
            //Pegar caminhos
            var fullPath = Application.dataPath.Replace("Assets", "") + newPath;

            //Extrair e pegar Textura
            //characterImporter.ExtractTextures(Path.GetDirectoryName(fullPath));
            AssetDatabase.Refresh();
            
            var texturesPaths = AssetDatabase.FindAssets("t:Texture", new []{Path.GetDirectoryName(characterImporter.assetPath)});
            var characterTexture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(texturesPaths[0]));
            
            //Pegar Mesh
            var characterAsset = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);

            Character asset = AssetDatabase.LoadAssetAtPath<Character>($"{_characterResourcePath}/{characterName}.asset");
            bool createAsset = false;
            if (asset == null)
            {
                 //Criar Scriptable
                 asset = ScriptableObject.CreateInstance<Character>();
                 createAsset = true;
            }
            
            asset.name = characterName;
            asset.Name = characterName;
            asset.Texture = characterTexture;
            asset.Asset = characterAsset;

            if (createAsset)
            {
                AssetDatabase.CreateAsset(asset, $"{_characterResourcePath}/{characterName}.asset");
                Debug.Log($"{characterName} created!");
            }
        }
        
        AssetDatabase.SaveAssets();
    }

    public static string GetCharacterName(string path)
    {
        var characterName = Path.GetFileNameWithoutExtension(path);
        characterName = characterName.Replace("-", " ");
        characterName = characterName.Replace("_", " ");
        characterName = characterName.ToLower();

        return characterName;
    }

    public static List<ModelImporter> LoadAssetImportersFromSelection()
    {
        var modelImporters = new List<ModelImporter>();
        var guids = Selection.assetGUIDs;
        foreach (var g in guids)
        {
            if (g != null)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(g);
                
                var asset = AssetDatabase.FindAssets("t:Model", new []{assetPath}).First();
                //Debug.Log($"assetPath: {assetPath}");
 
                var assetImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(asset));
 
                if (assetImporter is ModelImporter model)
                {
                    modelImporters.Add(model);
                }
            }
        }
        return modelImporters;
    }
#endif

}
