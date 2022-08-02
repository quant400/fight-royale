using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CFC.Serializable;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttributes : MonoBehaviour
{
    public Account fighter => Data_Manager.Instance.selectedAccount;
    [SerializeField] private PlayerBehaviour _player;
    [SerializeField] private FighterCategory _category;
    public FighterCategory category => GetCategory();
    private float currentHealth => _player.pHealth;
    private bool isBlocking => _player.pIsBlocking;

    private const int _baseHealth = 100;
    private const float _baseCriticalDamage = 20.0f;
    private float _basePunchAtk => GetBaseValue("attack");
    private float _baseKickAtk => GetBaseValue("attack");
    private float _baseBlockDef => GetBaseValue("defense");
    private float _baseTechnique => GetBaseValue("technique");

    [SerializeField] private List<PowerUp> activePowerUps = new List<PowerUp>();

    private FighterCategory GetCategory()
    {
        if (_category == null)
        {
            const string key = "edition";
            var att = GetAttribute(key);
            var allFighterCategory = Resources.LoadAll("FighterCategory", typeof(FighterCategory)).Cast<FighterCategory>().ToList();
            _category = allFighterCategory.FirstOrDefault(f => f.name.ToLower().Equals(att.value.ToLower()));
            Debug.Log(_category);
            SetAtkSpeed(_category.Speed/100 + 1);
            return _category;
        }
        
        return _category;
    }

    private void SetAtkSpeed(float speed)
    {
        if (!_player.isLocalPlayer) return;
        Debug.Log(speed);
        _player.anim.SetFloat("AtkSpeed", speed);
    }

    private float GetBaseValue(string key)
    {
        var baseValue = GetAttribute(key).value;
        return float.Parse(baseValue);
    }

    private Attribute2 GetAttribute(string key)
    {
        return fighter.attributes.FirstOrDefault(a => a.trait_type.Equals(key));
    }

    private float CriticalChangeDamage(float damage)
    {
        if (Random.Range(0, 100) < (_baseTechnique + 1.0f))
        {
            return ImproveAttackPower(damage);
        }
        else
        {
            var criticalDamage = ImproveAttackPower(damage + (damage * (_baseCriticalDamage / 100)));
            return criticalDamage;
        }
    }

    private float ImproveAttackPower(float damage)
    {
        var finalDamage = damage;

        activePowerUps.ForEach(p =>
        {
            if (p.Attribute.Code == CodeAttributePower.ATK)
            {
                finalDamage = damage + (damage * p.Attribute.Value);
            }
        });
        return Mathf.Clamp(finalDamage, 1, 100);
    }

    public float PunchDamage()
    {
        return CriticalChangeDamage(Mathf.Clamp((_basePunchAtk / 10), 0, _basePunchAtk) + (((_basePunchAtk / 10) * (category.Punch) / 100)));
    }

    public float KickDamage()
    {
        return CriticalChangeDamage(Mathf.Clamp((_baseKickAtk / 10), 0, _baseKickAtk) + (((_baseKickAtk / 10) * (category.Kick) / 100)));
    }

    public float Block(float damage)
    {
        var trueDamage = Mathf.Clamp(damage - _baseBlockDef, 1, 50);
        return trueDamage;
    }

    public float Speed(float currentSpeed)
    {
        return currentSpeed + ((currentSpeed * category.Speed) / 100);
    }

    public void OnPowerUp(PowerUp power)
    {
        if (power == null) return;

        switch (power.Attribute.Code)
        {
            case CodeAttributePower.HEAL:
                _player.CmdAddHealth(_baseHealth * power.Attribute.Value);
                break;
            case CodeAttributePower.SPEED:
                StartCoroutine(AddPower(power, (result) =>
                {
                    if (result)
                    {
                        SetAtkSpeed((_category.Speed/100+1) * (power.Attribute.Value + 1));
                    }
                    else
                    {
                        SetAtkSpeed((_category.Speed/100+1));
                    }
                }));
                break;
            default:
                StartCoroutine(AddPower(power));
                break;
        }

    }

    private IEnumerator AddPower(PowerUp power, Action<bool> powerUpAction = null)
    {
        activePowerUps.Add(power);
        powerUpAction?.Invoke(true);
        yield return new WaitForSeconds(power.Attribute.Duration);
        powerUpAction?.Invoke(false);
        activePowerUps.Remove(power);

    }

}
