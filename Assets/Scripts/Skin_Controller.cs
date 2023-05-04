using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Skin_Controller : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _meshRenderer;
    [SerializeField] private PlayerBehaviour _player;

    private string[] wearablesWorn;
    private string BeltsModelsPath = "WearableModels/Belts";

    private string GlassesModelsPath = "WearableModels/Glasses";

    private string GlovesModelsPath = "WearableModels/Gloves";

    private string ShoesModelsPath = "WearableModels/Shoes";

    private string ShortsModelsPath = "WearableModels/Shorts";

    private string currentSkin;
    private bool _isLocalPlayer;

    private Transform rootBone;
    
    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _isLocalPlayer = _player == null || _player.isLocalPlayer;
        _meshRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void OnDisable()
    {
        if (_isLocalPlayer)
        {
            Character_Manager.Instance?.OnCharacterChanged.RemoveListener(SetUpSkin);
        }

    }

    void Start()
    {
       

        if (_isLocalPlayer)
        {
            Character_Manager.Instance?.OnCharacterChanged.AddListener(SetUpSkin);
        }
    }

    public void SetUpSkin()
    {
        ChangeSkin(Character_Manager.Instance.GetCurrentCharacter);
    }
    
    public void SetUpSkin(string skinName)
    {
        var currentCharacter = Character_Manager.Instance.GetCharacters.FirstOrDefault(
            auxChar => auxChar.Name.ToLower().Equals(skinName.ToLower()));
        /*if (GetComponent<PlayerBehaviour>().isLocalPlayer)
            wearablesWorn = gameplayView.instance.equipedWearables;
        else if(!GetComponent<PlayerBehaviour>().isLocalPlayer)
            wearablesWorn = GameManager.Instance.wearablesWorn[GetComponent<PlayerBehaviour>().netIdentity];*/
        if (GetComponent<PlayerBehaviour>().isLocalPlayer)
            wearablesWorn = gameplayView.instance.equipedWearables.Split(',');
        else if(!GetComponent<PlayerBehaviour>().isLocalPlayer)
            wearablesWorn = GetComponent<PlayerBehaviour>().pWearables.Split(',');
        Debug.Log((wearablesWorn[0], wearablesWorn[1]));

        if (currentCharacter != null)
        {
            _player.pSkin = currentCharacter.Name;
            ChangeSkin(currentCharacter);
        }
    }

    public void ChangeSkin(Character character)
    {
        if (character == null) return;
        //_meshRenderer.material.mainTexture = character.Texture;
        UpdateMeshRenderer(character.Asset.GetComponentsInChildren<SkinnedMeshRenderer>());
    }

    public void UpdateMeshRenderer (SkinnedMeshRenderer[] newMeshRenderers)
    {
        for (int i = 0; i < newMeshRenderers.Length; i++)
        {
            // update mesh
            //_meshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
            if (newMeshRenderers[i].sharedMaterials.Length > 1)
            {
                _meshRenderer[i].sharedMaterials = newMeshRenderers[i].sharedMaterials;

            }
            else
            {
                _meshRenderer[i].material.mainTexture = newMeshRenderers[i].sharedMaterial.mainTexture;
            }


            _meshRenderer[i].sharedMesh = newMeshRenderers[i].sharedMesh;

            Transform[] childrens = transform.GetComponentsInChildren<Transform>(true);

            // sort bones.
            Transform[] bones = new Transform[newMeshRenderers[i].bones.Length];
            for (int boneOrder = 0; boneOrder < newMeshRenderers[i].bones.Length; boneOrder++)
            {
                bones[boneOrder] = Array.Find<Transform>(childrens, c => c.name == newMeshRenderers[i].bones[boneOrder].name);
            }
            _meshRenderer[i].bones = bones;

            rootBone = _meshRenderer[i].rootBone;

            _meshRenderer[i].gameObject.name = newMeshRenderers[i].gameObject.name;
        }

        GameObject modelToInstantiate = null;

        int childIndex;

        GameObject wearable;

        SkinnedMeshRenderer spawnedSkinnedMeshRenderer;

        foreach (string wearableWorn in wearablesWorn)
        {
            Debug.Log("Wearables Worn");

            var x = wearableWorn.Split('_');

            Debug.Log("WearableModels/" + x[0]);
            modelToInstantiate = Resources.Load(Path.Combine("WearableModels/" + x[0], x[1])) as GameObject;

            GameObject instantiatedWearable = Instantiate(modelToInstantiate);

            childIndex = GetIndex(x[0]);

            if (childIndex != -1)
            {
                wearable = instantiatedWearable.transform.GetChild(1).gameObject;

                spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

                spawnedSkinnedMeshRenderer.bones = gameObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>().bones;
                spawnedSkinnedMeshRenderer.rootBone = rootBone;

                wearable.transform.parent = gameObject.transform;
                Destroy(gameObject.transform.GetChild(childIndex).transform.gameObject);
                Destroy(instantiatedWearable);
                wearable.transform.SetSiblingIndex(childIndex);
            }
        }
    }

    private int GetIndex(string wearableType)
    {
        switch (wearableType)
        {
            case "Gloves":
                return 8;
            case "Shorts":
                return 12;
        }

        return -1;
    }

}
