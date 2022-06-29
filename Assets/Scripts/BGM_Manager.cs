using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Manager : MonoBehaviour
{

    public static BGM_Manager Instance;

    private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _songsList;
    [SerializeField] private int indexSong = 0;


    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            _audioSource = this.gameObject.GetComponent<AudioSource>();
        }
    }


    public void PlaySong()
    {
        if(_audioSource.clip == null)
        {
            _audioSource.clip = _songsList[indexSong];
        }
        _audioSource.Play();
    }

    private void SetSong(AudioClip audioClip)
    {
        _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void Next()
    {
        indexSong++;

        if (indexSong > (_songsList.Count - 1)) 
        {
            indexSong = 0;
        }

        
        SetSong(_songsList[indexSong]);
        
    }

    public void Previous()
    {
        indexSong--;
        if (indexSong < 0)
        {
            indexSong = (_songsList.Count - 1);
        }
        
        SetSong(_songsList[indexSong]);
       
    }

    public string GetNameSong()
    {
            return _songsList[indexSong].name.ToString();
    }


}
