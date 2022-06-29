using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class CharacterImporter : MonoBehaviour
{
    #if UNITY_EDITOR
    
    const string _characterResourcePath = "Assets/Resources/Characters";

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
                //Debug.Log($"assetPath: {assetPath}");
 
                var assetImporter = AssetImporter.GetAtPath(assetPath);
 
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
