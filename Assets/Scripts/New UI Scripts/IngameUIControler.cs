using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class IngameUIControler : MonoBehaviour
{
    public static IngameUIControler instance;
    [SerializeField]
    AudioSource musicAudioSource;
    public bool sfxMuted = false;
    public bool musicMuted = false;
    [SerializeField]
    Button muteSfx, muteMusic, settings, closeSettings, chat;
    Image sfxButtonImage, musicButtonImage, chatButtonImage;

    float defaultMusicVol;

    Sprite regularImage;
    [SerializeField]
    Sprite disableImage;

    [SerializeField]
    GameObject SettingsPanel;
    // for chat 
    [SerializeField]
    GameObject chatObject;
    [SerializeField]
    TMPro.TMP_InputField chatinput;
    bool chatOpen=false;

    [SerializeField]
    Transform playerDisplayHolder;
    [SerializeField]
    GameObject playerDisplayObject;
    public Dictionary<NetworkIdentity,Image> playerMap = new Dictionary<NetworkIdentity, Image>();

    public bool localPlayerSpawned =false;
    int playerNum  = 0;

    //for leaderboard 
    [SerializeField]
    GameObject leaderboard;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
            Destroy(this.gameObject);

        

    }
    private void Start()
    {
        musicAudioSource = BGM_Manager.Instance.GetComponent<AudioSource>();
        defaultMusicVol = musicAudioSource.volume;
        sfxButtonImage = muteSfx.GetComponent<Image>();
        musicButtonImage = muteMusic.GetComponent<Image>();
        chatButtonImage = chat.GetComponent<Image>();
        regularImage = sfxButtonImage.sprite;
        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetString("SFX") == "off")
            {
                sfxMuted = true;
                MuteSFX();
            }

        }
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetString("Music") == "off")
            {
                musicMuted = true;
                MuteMusic();
            }
        }

        muteSfx.onClick.AddListener(MuteSFX);
        muteMusic.onClick.AddListener(MuteMusic);
        settings.onClick.AddListener(OpenSettings);
        closeSettings.onClick.AddListener(CloseSettings);
        chat.onClick.AddListener(ChatToggle);
        Invoke("ActivateTimer", 4f);
    }




    #region Toggle_Buttons

    public void MuteSFX()
    {
        if (PlayerPrefs.HasKey("SFX") && PlayerPrefs.GetString("SFX") == "off")
        {
            //SFX.volume = defaultMusicVol;
            sfxButtonImage.sprite = regularImage;//sfxButtonImage.color = new Color(1f, 1f, 1f, 1f);
            sfxButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            PlayerPrefs.SetString("SFX", "on");
            
        }
        else
        {
            sfxButtonImage.sprite = disableImage;//sfxButtonImage.color = new Color(1f, 1f, 1f, 0.5f);
            sfxButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(0.9450981f, 0.1215686f, 0.172549f, 1f);
            //SFX.volume = 0;
            PlayerPrefs.SetString("SFX", "off");
        }

    }

    public void MuteMusic()
    {
        if (musicAudioSource.volume == 0)
        {
            musicButtonImage.sprite = regularImage; //musicButtonImage.color = new Color(1f, 1f, 1f, 1f);
            musicButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            musicAudioSource.volume = defaultMusicVol;
            PlayerPrefs.SetString("Music", "on");
            if (!musicAudioSource.isPlaying)
                musicAudioSource.Play();
        }
        else
        {
            musicButtonImage.sprite = disableImage;//musicButtonImage.color = new Color(1, 1, 1, 0.5f);
            musicButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(0.9450981f, 0.1215686f, 0.172549f, 1f);
            musicAudioSource.volume = 0;
            PlayerPrefs.SetString("Music", "off");
        }
    }

    void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }


    void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }

    void ChatToggle()
    {
        if (chatOpen)
            CloseChat();
        else
            OpenChat();
    }

    void OpenChat()
    {
        chatOpen = true;
        chatButtonImage.sprite = disableImage;
        chatButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(0.9450981f, 0.1215686f, 0.172549f, 1f);
        chatObject.transform.GetChild(0).gameObject.SetActive(false);
        chatObject.transform.GetChild(1).gameObject.SetActive(true);
        chatObject.transform.GetChild(2).gameObject.SetActive(false);
    }

    void CloseChat()
    {
        chatOpen = false;
        chatButtonImage.sprite = regularImage;
        chatButtonImage.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        chatObject.transform.GetChild(0).gameObject.SetActive(false);
        chatObject.transform.GetChild(1).gameObject.SetActive(false);
        chatObject.transform.GetChild(2).gameObject.SetActive(false);
    }
    #endregion Toggle_Buttons

    #region Player_Display

    public void AddPlayer(NetworkIdentity p, GameObject chtr)
    {
        var obj= Instantiate(playerDisplayObject, playerDisplayHolder);
        playerMap[p] = obj.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        obj.GetComponent<PlayerDisplayScript>().SetChar(p.gameObject.GetComponent<PlayerBehaviour>().pName.Replace(' ','-'), chtr);
        playerNum++;
    }

    public void AddLocalPlayer(NetworkIdentity p)
    {
        var obj = Instantiate(playerDisplayObject, playerDisplayHolder);
        playerMap[p] = obj.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        obj.GetComponent<PlayerDisplayScript>().SetLocalChar(p.gameObject.GetComponent<PlayerBehaviour>().pName.Replace(' ', '-'));
        localPlayerSpawned = true;
    }

    public void UpdatePlayerHealth(NetworkIdentity p,float newHealth)
    {
        if (playerMap.ContainsKey(p))
        {
            playerMap[p].fillAmount = newHealth;
        }
    }

    public int GetPlayerNumber()
    {
        return playerNum;
    }
    #endregion Player_Display

    void ActivateTimer()
    {
        transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
    }


    #region Leaderboard

   
    


    #endregion Leaderboard

}
