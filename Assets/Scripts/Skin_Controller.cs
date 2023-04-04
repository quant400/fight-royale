using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skin_Controller : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _meshRenderer;
    [SerializeField] private PlayerBehaviour _player;

    private string currentSkin;
    private bool _isLocalPlayer;
    
    
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

            _meshRenderer[i].gameObject.name = newMeshRenderers[i].gameObject.name;
        }
    }
}
