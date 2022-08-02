using System;
using System.Collections.Generic;
using System.Linq;
using CFC.Serializable;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Data_Manager : MonoBehaviour
{
    public static Data_Manager Instance;

    public string accountId;
    public string contractId;

    public int currentNftId;

    public Account selectedAccount => GetSelectedAccount();
    private Account _selectedAccount;
    [SerializeField] private RootAccount userAccount;

    //Leaderboard
    public CFC.Serializable.Leaderboard.RootLeaderboard leaderboard_AllTime;
    public CFC.Serializable.Leaderboard.RootLeaderboard leaderboard_Daily;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
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
            //Debug.Log(json);

            var tempAccounts = JsonUtility.FromJson<RootAccount>(json);

            tempAccounts.accounts = tempAccounts.accounts.OrderBy(aux => aux.name).ToList();

            List<string> names = new List<string>();

            foreach (var account in tempAccounts.accounts)
            {
                account.name = account.name.Replace(".", "")
                    .Replace("-"," ")
                    .Replace("'","");
                names.Add(account.name);
            }
            
            //System.IO.File.WriteAllText(Application.dataPath+"/data names.txt", string.Join("\n",names));

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
        if (_selectedAccount == null || true)
        {
            _selectedAccount = userAccount.accounts
                .FirstOrDefault(a => a.name.ToLower().Equals(Character_Manager.Instance.GetCurrentCharacter.Name.ToLower()));
        }

        currentNftId = _selectedAccount.id;
        
        return _selectedAccount;
    }
    
    

}


