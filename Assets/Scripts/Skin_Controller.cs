using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skin_Controller : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private PlayerBehaviour _player;

    private string currentSkin;
    private bool _isLocalPlayer;
    
    
    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _isLocalPlayer = _player == null || _player.isLocalPlayer;
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
        _meshRenderer.material.mainTexture = character.Texture;
        UpdateMeshRenderer(character.Asset.GetComponentInChildren<SkinnedMeshRenderer>());
    }

    public void UpdateMeshRenderer (SkinnedMeshRenderer newMeshRenderer)
    {
        // update mesh
        _meshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
        
        Transform[] childrens = transform.GetComponentsInChildren<Transform> (true);
        
        // sort bones.
        Transform[] bones = new Transform[newMeshRenderer.bones.Length];
        for (int boneOrder = 0; boneOrder < newMeshRenderer.bones.Length; boneOrder++) {
            bones [boneOrder] = Array.Find<Transform> (childrens, c => c.name == newMeshRenderer.bones [boneOrder].name);
        }
        _meshRenderer.bones = bones;
    }
}
