using System;
using System.Linq;
using CFC.Serializable;
using UnityEngine;

public class Data_Manager : MonoBehaviour
{
    public static Data_Manager Instance;

    public string accountId;
    public string contractId;

    public Account selectedAccount => GetSelectedAccount();
    private Account _selectedAccount;

    [SerializeField] private RootAccount userAccount;

    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
    }

    private void SetAccount(RootAccount tempAccount)
    {
        if (userAccount.accounts.Count == 0)
        {
            userAccount = tempAccount;
        }
        else
        {
            Debug.Log("Unable to change account");
        }
    }

    public RootAccount GetAccount()
    {
        return userAccount;
    }

    public void ResetAccount()
    {
        userAccount = null;
    }

    public void StartAccount(string json, Action success, Action<string> error)
    {
        Debug.Log("Starting the account");
        try
        {
            Debug.Log(json);

            var tempAccounts = JsonUtility.FromJson<RootAccount>(json);

            if (tempAccounts.accounts.Count > 0)
            {
                SetAccount(tempAccounts);

                Character_Manager.Instance.StartCharacter(tempAccounts.accounts);

                success();
            }
            else
            {
                error("You don't own any characters visit \n  (https://app.cryptofightclub.io/mint) \n to acquire");

            }
        }
        catch (System.Exception e)
        {
            error(e.Message);
        }

    }


    private Account GetSelectedAccount()
    {
        if (_selectedAccount == null)
        {

            _selectedAccount = userAccount.accounts
                .FirstOrDefault(a => a.name.ToLower().Equals(Character_Manager.Instance.GetCurrentCharacter.Name.ToLower()));
            return _selectedAccount;
        }

        return _selectedAccount;
    }
}


