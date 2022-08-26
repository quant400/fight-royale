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
    private bool isRunningAttack = false;
    private bool isPunch;
    private bool isBlocking;
    private bool toggleAttack = true; //true = punch, false = kick

    [SerializeField]private bool m_comboPossible = false;
    [SerializeField]private int m_PunchComboStep = 0;
    [SerializeField]private int m_KickComboStep = 0;
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

    private float _stepOffset = 0.4f;

    void Update()
    {
        if (_player.isServer) return;

        _player._cControler.stepOffset = _player._tpControler.Grounded ? _stepOffset : 0.1f;
        
        _tpController.enabled = (!isAttack && !isBlocking) || isRunningAttack;
        
        if (!isAttack && _player._tpControler.Grounded)
        {
            isBlocking = _inputs.block;
            _player.CmdBlocking(isBlocking);
            _player.pIsBlocking = isBlocking;
            _anim.SetBool("Block", isBlocking);
        }
        
        if (_inputs.attack && _player._tpControler.Grounded && !isBlocking)
        {
            if (_isCarrying)
                PreThrow();
            else if(toggleAttack)
                Punch();
            else
                Kick();
        }
        
        
        _inputs.attack = false;


        if (_inputs.toggleAttack)
        {
            toggleAttack = !toggleAttack;
            _inputs.toggleAttack = false;
        }

        if (_inputs.action && _player._tpControler.Grounded && !isBlocking)
        {
            
            if (!_isCarrying)
                CheckAction();
            else
                PreThrow();
        }
        
        _inputs.action = false;

    }
    
    public void Punch()
    {
        if (!_tpController.gameObject) return;
			
        if (m_PunchComboStep == 0)
        {
            if (_player._cfcInputs.move.magnitude != 0)
            {
                isAttack = true;
                isPunch = true;
                PlayAttackAnimation("Running Punch");
                isRunningAttack = true;
                return;
            }
            else
            {
                isAttack = true;
                isPunch = true;
                PlayAttackAnimation("Punch 1");
                m_PunchComboStep = 1;
                return;
            }
        }
			
        if (m_comboPossible)
        {
            m_comboPossible = false;
            m_PunchComboStep += 1;
        }
    }
    
    public void Kick()
    {
        if (!_tpController.gameObject) return;
			
        if (m_KickComboStep == 0)
        {
            if (_player._cfcInputs.move.magnitude != 0)
            {
                isAttack = true;
                isPunch = false;
                PlayAttackAnimation("Running Kick");
                isRunningAttack = true;
                return;
            }
            else
            {
                isAttack = true;
                isPunch = false;
                PlayAttackAnimation("Kick 1");
                m_KickComboStep = 1;
                return;
            }
        }
			
        if (m_comboPossible)
        {
            m_comboPossible = false;
            m_KickComboStep += 1;
        }
    }

    private void PlayAttackAnimation(string animName)
    {
        _anim.Play(animName);
    }

    public void PunchCombo()
    {
        switch (m_PunchComboStep)
        {
            case 2:
                PlayAttackAnimation("Punch 2");
                break;
            case 3:
                PlayAttackAnimation("Punch 3");
                break;
        }
    }
    
    public void KickCombo()
    {
        switch (m_KickComboStep)
        {
            case 2:
                PlayAttackAnimation("Kick 2");
                break;
        }
    }

    public void ComboReset()
    {
        isAttack = false;
        isRunningAttack = false;
        isPunch = false;
        m_comboPossible = false;
        m_PunchComboStep = 0;
        m_KickComboStep = 0;
        
        if(_player.isLocalPlayer)
            _tpController.enabled = true;
    }
    
    public void ComboPossible()
    {
        m_comboPossible = true;
    }
    
    public void KickOld()
    {
        if (!_tpController.Grounded) return;

        if (isAttack) return;

        isPunch = false;
        isAttack = true;

        PlayAttackAnimation("Kick");
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
    
    public void Win()
    {
        //_anim.Play("Win");
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
            _player.CmdOnDamage(_player.netIdentity, target.netIdentity, damage);
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
        return;
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

    public void PreCarry(ThrowableBehavior throwable)
    {
        _anim.Play("PickUp");
        _anim.SetLayerWeight(_anim.GetLayerIndex("UpperBody"), 1);
        _throwableBehavior = throwable;
    }

    private ThrowableBehavior _throwableBehavior;

    public void Carry()
    {
        _isCarrying = true;
        _carryingObject = _throwableBehavior;
        //_carryTransformChild.target = throwable.transform;
        _carryingObject.Carry(_player.netIdentity,_throwTargetTransform);
        _throwableBehavior = null;
    }

    public void RemoveCarryAssignment()
    {
        _player.CmdThrow(_carryingObject.netIdentity);
    }

    public void PreThrow()
    {
        _anim.Play("Throw");
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
        
        SFX_Manager.Play($"Punch {(m_PunchComboStep==0?1:m_PunchComboStep)}");
    }
    
    public void PlayKickAudio()
    {
        SFX_Manager.Play("Kick");
    }
}
