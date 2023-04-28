using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using CFC;
using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TPFightingController : MonoBehaviour
{
    private PlayerBehaviour _player; 
    private ThirdPersonController _tpController => _player._tpControler;
    private Animator _anim => _player.anim;
    private CFCInputs _inputs=> _player._cfcInputs;   
    private PlayerAttributes _attributes => _player._pAttributes;
    
    [SerializeField] private BoxCollider hitCollider;

    private bool isAttack;
    private bool isHitted;
    private bool isRunningAttack = false;
    private bool isPunch;
    private bool isBlocking;
    private bool toggleAttack = true; //true = punch, false = kick

    [SerializeField]private bool m_comboPossible = false;
    [SerializeField]private int m_PunchComboStep = 0;
    [SerializeField]private int m_KickComboStep = 0;
    [SerializeField] private LayerMask playersMask;
    
    private bool _isCarrying => _carryingObject != null;
    [Header("Actions")]
    [SerializeField] private LayerMask _throwableMask;
    [SerializeField] public Transform _throwTargetTransform;
    public Throwable_BehaviorV2 _carryingObject;
    [SerializeField] private NetworkTransformChild _carryTransformChild;

    [SerializeField]
    GameObject punchEffect, kickEffect;
    [SerializeField] Transform lHand, rHand, lFoot, rFoot;
    CameraShake cam;
    [SerializeField]
    Cinemachine.CinemachineImpulseSource s;
    [SerializeField] private Image _damageEffect;
    Coroutine routineHolder;

    public bool isAction = false;
    
    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        cam = GameObject.FindGameObjectWithTag("PlayerCam").GetComponent<CameraShake>();
    }

    private float _stepOffset = 0.4f;

    void Update()
    {
        if (_player.isServer || !_player.isLocalPlayer) return;

        _player._cControler.stepOffset = _player._tpControler.Grounded ? _stepOffset : 0.1f;
        
        _tpController.canMove = (!isAttack && !isBlocking) || isRunningAttack;
        
        if (!isAttack && _player._tpControler.Grounded)
        {
            isBlocking = _inputs.block;
            _player.CmdBlocking(isBlocking);
            _player.pIsBlocking = isBlocking;
            _anim.SetBool("Block", isBlocking);
        }
        
        if (_inputs.attack && _player._tpControler.Grounded && !isBlocking && !isHitted)
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

        if (_inputs.action && _player._tpControler.Grounded && !isBlocking && !isHitted && _anim.GetLayerWeight(_anim.GetLayerIndex("UpperBody"))==0)
        {
            isAction = true;

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
            _tpController.canMove = true;
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

        if (_player.isLocalPlayer)
            _player.CmdAnimationPickUp(true);

        if (!_isCarrying) 
        {
            _anim.Play("Hit");
            isHitted = true;
        }
        HitEffect();
    }

    public void ReturnFromHit() 
    {
        if(_player.isLocalPlayer)
            _player.CmdAnimationPickUp(false);

        isHitted = false;
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
                PlayPunchEffect();
                DealDamage(_attributes.PunchDamage(), collider);
            }
            else
            {
                PlayKickAudio();
                PlayKickEffect();
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
        if (!_player.isLocalPlayer ||_player.isServer) return;
        
        var colliders = Physics.OverlapBox(hitCollider.transform.position, hitCollider.size/2, hitCollider.transform.rotation, _throwableMask);

        var item = colliders.Select(aux => aux.GetComponent<Throwable_BehaviorV2>()).FirstOrDefault();

        if (item != null)
        {
            if (item.HasCarrier)
            {
                _player.CmdStealObject(item.carrierNetIdentity);
                //item.carrierNetIdentity.GetComponent<PlayerBehaviour>()._tpFightingControler.StolenObject();
            }
            PrePickUp(item);
        }
    }

    public void PrePickUp(Throwable_BehaviorV2 item)
    {
        _anim.Play("PickUp");
        //_anim.SetLayerWeight(_anim.GetLayerIndex("UpperBody"), 1);

        _player.CmdAnimationPickUp(true);
        _carryingObject = item;
    }

    public void PickUp() 
    {
        if (!_player.isLocalPlayer || _player.isServer) return;

        _player.CmdPickUp(_carryingObject.netIdentity);
        _carryingObject.PickUp(_player.netIdentity, _throwTargetTransform);

        isAction = false;
    }   

    public void PreThrow()
    {
        _anim.Play("Throw");
    }

    private void Throw()
    {
        if (!_player.isLocalPlayer || _player.isServer) return;

        _carryingObject.Throw();
        _player.CmdThrow(_carryingObject.netIdentity);
        //_anim.SetLayerWeight(_anim.GetLayerIndex("UpperBody"), 0);
        _player.CmdAnimationPickUp(false);

        _carryingObject = null;

        isAction = false;
    }

    public void StolenObject() 
    {
        if (_carryingObject == null) return;

        if(!_player.isServer) _player.CmdAnimationPickUp(false);
        _carryingObject.ResetChair();
        _carryingObject.ResetCollision(_player);

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

    public void PlayPunchEffect()
    {
        GameObject temp;
        if (_player._cfcInputs.move.magnitude == 0)
        {
            switch (m_PunchComboStep)
            {
                case 1:
                    temp=GameObject.Instantiate(punchEffect, lHand.position, lHand.rotation);
                    break;
                case 2:
                    temp = GameObject.Instantiate(punchEffect, rHand.position,rHand.rotation);
                    break;
                case 3:
                    temp = GameObject.Instantiate(punchEffect, rHand.position, rHand.rotation);
                    break;
                default:
                    temp = GameObject.Instantiate(punchEffect, rHand.position, rHand.rotation);
                    break;
            }
        }

        else
            temp = GameObject.Instantiate(punchEffect, rHand.position, rHand.rotation);
        temp.transform.LookAt(cam.transform);
        temp.transform.localScale = new Vector3(10, 10, 10);
    }

    public void PlayKickEffect()
    {
        GameObject temp;
        if (_player._cfcInputs.move.magnitude == 0)
        {
            switch (m_KickComboStep)
            {
                case 1:
                    temp=GameObject.Instantiate(kickEffect, rFoot.position,rFoot.rotation);
                    break;
                case 2:
                    temp = GameObject.Instantiate(kickEffect, lFoot.position, lFoot.rotation);
                    break;
                case 3:
                    temp = GameObject.Instantiate(kickEffect, lFoot.position, lFoot.rotation);
                    break;
                default:
                    temp = GameObject.Instantiate(kickEffect, rFoot.position, rFoot.rotation);
                    break;
            }
        }

        else
            temp = GameObject.Instantiate(kickEffect, rFoot.position, rFoot.rotation);
        temp.transform.LookAt(cam.transform);
        temp.transform.localScale = new Vector3(10, 10, 10);
    }

    public void HitEffect()
    {
        if (_player.isLocalPlayer)
        {
            cam.Shake(s);
            EnableDamageEffect(1f);
            GameObject temp = GameObject.Instantiate(punchEffect, transform.position + new Vector3(0, 1, 0), transform.rotation);
            temp.transform.LookAt(cam.transform);
            temp.transform.localScale = new Vector3(10, 10, 10);
        }
        
    }

    public void EnableDamageEffect(float duration)
    {
        if(routineHolder!=null)
            StopCoroutine(routineHolder);
        DOTween.Kill(_damageEffect);
        if(_damageEffect.color.a<0.5f)
        {
            _damageEffect.color =new Vector4(_damageEffect.color.r, _damageEffect.color.g, _damageEffect.color.b, 0.5f);
        }
        else
        {
            _damageEffect.color = new Vector4(_damageEffect.color.r, _damageEffect.color.g, _damageEffect.color.b, _damageEffect.color.a + 0.25f);

        }
       routineHolder= StartCoroutine(DisableDamageEffect(duration));
    }

    IEnumerator DisableDamageEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        //_damageEffect.color = new Vector4(_damageEffect.color.r, _damageEffect.color.g, _damageEffect.color.b, 0);
        _damageEffect.DOFade(0f, 1f);

    }
}
