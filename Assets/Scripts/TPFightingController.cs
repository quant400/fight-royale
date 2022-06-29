using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CFC;
using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TPFightingController : MonoBehaviour
{
    private PlayerBehaviour _player; 
    private ThirdPersonController _tpController => _player._tpControler;
    private Animator _anim => _player.anim;
    private CFCInputs _inputs=> _player._cfcInputs;   
    private PlayerAttributes _attributes => _player._pAttributes;
    
    [SerializeField] private BoxCollider hitCollider;

    private bool isAttack;
    private bool isPunch;
    private bool isBlocking;

    [SerializeField]private bool m_comboPossible = false;
    [SerializeField]private int m_comboStep = 0;
    [SerializeField] private LayerMask playersMask;
    
    [Header("Actions")]
    private bool _isCarrying = false;
    [SerializeField] private LayerMask _throwableMask;
    [SerializeField] private Transform _throwTargetTransform;
    private ThrowableBehavior _carryingObject;
    [SerializeField] private NetworkTransformChild _carryTransformChild;
    
    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (_player.isServer) return;
        
        _tpController.enabled = !isAttack && !isBlocking;
        
        if (!isAttack)
        {
            isBlocking = _inputs.block;
            _player.CmdBlocking(isBlocking);
            _player.pIsBlocking = isBlocking;
            _anim.SetBool("Block", isBlocking);
        }
        
        if (isBlocking) return;
        
        if (_inputs.punch)
        {
            Punch();
            _inputs.punch = false;
        }


        if (_inputs.kick)
        {
            Kick();
            _inputs.kick = false;
        }

        if (_inputs.action)
        {
            _inputs.action = false;
            
            if (!_isCarrying)
                CheckAction();
            else
                Throw();
        }

    }
    
    public void Punch()
    {
        if (!_tpController.gameObject) return;
			
        if (m_comboStep == 0)
        {
            isAttack = true;
            isPunch = true;
            _anim.Play("Punch 1");
            m_comboStep = 1;
            return;
        }
			
        if (m_comboPossible)
        {
            m_comboPossible = false;
            m_comboStep += 1;
        }
    }
    
    public void Combo()
    {
        switch (m_comboStep)
        {
            case 2:
                _anim.Play("Punch 2");
                break;
            case 3:
                _anim.Play("Punch 3");
                break;
        }
    }

    public void ComboReset()
    {
        isAttack = false;
        isPunch = false;
        m_comboPossible = false;
        m_comboStep = 0;
        
        if(_player.isLocalPlayer)
            _tpController.enabled = true;
    }
    
    public void ComboPossible()
    {
        m_comboPossible = true;
    }
    
    public void Kick()
    {
        if (!_tpController.Grounded) return;

        if (isAttack) return;
			
        isAttack = true;

        _anim.Play("Kick");
    }

    public void TakeDamage()
    {
        Debug.Log("TakeDamage");
        _anim.Play("Hit");
        isAttack = true;
    }
    
    public void BlockDamage()
    {
        _anim.Play("Block Hit");
    }
    
    public void Die()
    {
        _anim.Play("Death");
    }

    
    public void CheckHit()
    {
        if (!_player.isLocalPlayer ||_player.isServer) return;
        
        var colliders = Physics.OverlapBox(hitCollider.transform.position, hitCollider.size, hitCollider.transform.rotation, playersMask);
			
        foreach (var collider in colliders.Select(player => player.GetComponent<PlayerBehaviour>()))
        {
            if (collider.isLocalPlayer || collider == _player) continue;

            if (isPunch)
            {
                PlayPunchAudio();
                DealDamage(_attributes.PunchDamage(), collider);
            }
            else
            {
                PlayKickAudio();
                DealDamage(_attributes.KickDamage(), collider);
            }
            
            Debug.Log($"Hit: {collider.pName}");
        }
    }

    public void DealDamage(float damage, PlayerBehaviour target)
    {
            _player.CmdOnDamage(target.netIdentity, damage);
            Debug.Log($"DealDamage: {_player.pName} deal {damage} damage on {target.pName}");
    }

    public void CheckCall()
    {
        if (SceneManager.GetSceneByName("AgoraHomeCFC").isLoaded) return;
			
        var colliders = Physics.OverlapBox(hitCollider.transform.position, hitCollider.size*3, hitCollider.transform.rotation, playersMask);
			
        foreach (var collider in colliders.Select(player => player.GetComponent<PlayerBehaviour>()))
        {
            if (collider.isLocalPlayer || collider == _player) continue;

            var callId = _player.pName + collider.pName;
            
            //Debug.Log("Network Manager: "+CFCNetworkManager.Instance);
            //Debug.Log("Agora: "+CFCNetworkManager.Instance.agora);
            
            //CFCNetworkManager.Instance.agora?.onJoin(false, callId);
            //collider.DoCall(callId);
            break;
        }
    }
    
    public void CheckAction()
    {
        if (!_player.isLocalPlayer ||_player.isServer) return;
        
        var colliders = Physics.OverlapBox(hitCollider.transform.position, hitCollider.size/2, hitCollider.transform.rotation, _throwableMask);

        var throwable = colliders.Select(aux => aux.GetComponent<ThrowableBehavior>()).FirstOrDefault();

        if (throwable != null)
        {
            AssignToCarry(throwable);
        }
    }

    public void AssignToCarry(ThrowableBehavior throwable)
    {
        _player.CmdCarry(throwable.netIdentity);
    }

    public void Carry(ThrowableBehavior throwable)
    {
        _isCarrying = true;
        _carryingObject = throwable;
        //_carryTransformChild.target = throwable.transform;
        _carryingObject.Carry(_throwTargetTransform);
        _anim.SetLayerWeight(_anim.GetLayerIndex("UpperBody"), 1);
    }

    public void RemoveCarryAssignment()
    {
        _player.CmdThrow(_carryingObject.netIdentity);
    }
    public void Throw()
    {
        _isCarrying = false;
        //_carryTransformChild.target = null;
        _carryingObject.Throw(_player.transform);
        _anim.SetLayerWeight(_anim.GetLayerIndex("UpperBody"), 0);
        _carryingObject = null;
    }

    public void PlayPunchAudio()
    {
        SFX_Manager.Play($"Punch {m_comboStep}");
    }
    
    public void PlayKickAudio()
    {
        SFX_Manager.Play("Kick");
    }
}
