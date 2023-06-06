using Photon.Pun;
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
    public string[] colors;
    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _isLocalPlayer = _player == null || _player.GetComponent<PhotonView>().IsMine/*isLocalPlayer*/;
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
        //Debug.LogError("updateMeshRun");
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
        //TODO Suleman: Uncomment Later
        UpdateWearables();

    }

    public void UpdateWearables()
    {
        //TODO Suleman: Uncomment Later
        if (GetComponent<PlayerBehaviour>().photonView.IsMine/*isLocalPlayer*/)
            wearablesWorn = gameplayView.instance.equipedWearables.Split(',');
        else if (!GetComponent<PlayerBehaviour>().photonView.IsMine/*isLocalPlayer*/ /*&& !GetComponent<PlayerBehaviour>().isServer*/)
            wearablesWorn = GetComponent<PlayerBehaviour>().pWearables.Split(',');
        Debug.Log("Update Called: " + wearablesWorn[0]);
        GameObject modelToInstantiate = null;

        transform.GetChild(8).gameObject.SetActive(false);
        transform.GetChild(9).gameObject.SetActive(false);
        transform.GetChild(10).gameObject.SetActive(false);

        int childIndex;

        GameObject wearable;

        SkinnedMeshRenderer spawnedSkinnedMeshRenderer;

        foreach (string wearableWorn in wearablesWorn)
        {
            var x = wearableWorn.Split('_');

            Debug.Log("WearableModels/" + x[0]);
            GameObject instantiatedWearable = Resources.Load(Path.Combine("WearableModels/" + x[0], x[1])) as GameObject;

           // GameObject  = Instantiate(modelToInstantiate);

            childIndex = GetIndex(x[0]);

            if (childIndex != -1)
            {
                wearable = instantiatedWearable.transform.GetChild(1).gameObject;
                foreach (string color in colors)
                {
                    if (transform.GetChild(childIndex).name.Contains(color))
                    {
                        foreach (Transform child in instantiatedWearable.transform)
                        {
                            if (child.name.Contains(color))
                            {
                                wearable = child.gameObject;
                            }
                        }
                    }
                }

                spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

                _meshRenderer[GetIndex(x[0]) - 3].sharedMaterial = spawnedSkinnedMeshRenderer.sharedMaterial;

                if (spawnedSkinnedMeshRenderer.sharedMaterials.Length > 1)
                {

                    _meshRenderer[GetIndex(x[0]) - 3].sharedMaterials = spawnedSkinnedMeshRenderer.sharedMaterials;

                }
                else
                {
                    _meshRenderer[GetIndex(x[0]) - 3].material.mainTexture = spawnedSkinnedMeshRenderer.sharedMaterial.mainTexture;
                }


                _meshRenderer[GetIndex(x[0]) - 3].sharedMesh = spawnedSkinnedMeshRenderer.sharedMesh;
            }

            if(childIndex >= 8)
            {
                transform.GetChild(childIndex).gameObject.SetActive(true);
            }

            //Destroy(instantiatedWearable);
        }

    }


    private int GetIndex(string wearableType)
    {
        switch (wearableType)
        {
            case "Gloves":
                return 3;
            case "Shorts":
                return 5;
            case "Shoes":
                return 6;
            case "Belts":
                return 8;
            case "masks":
                return 9;
            case "glasses":
                return 10;
        }

        return -1;
    }
}
