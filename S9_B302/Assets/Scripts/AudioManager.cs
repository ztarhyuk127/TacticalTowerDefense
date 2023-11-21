using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mainMixer;
    public Slider[] sliders;


    public float mValue;
    public float bValue;
    public float sValue;

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    public float mainBgmVolume;
    public int bgmChannels;
    AudioSource bgmPlayer;
    AudioSource mainBgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;


    public enum Sfx 
    { 
        attack, buyTower, collectItem, 
        gameClear, gameOver, item, 
        reroll, synergy, waveEndGetCoin 
    }

    private void Awake()
    {
        
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(sliders[0].value) * 20);
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            mValue = PlayerPrefs.GetFloat("masterVolume");
            bValue = PlayerPrefs.GetFloat("bgmVolume");
            sValue = PlayerPrefs.GetFloat("sfxVolume");
        }
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Init();
        InitializeSlider();
    }

    private void Start()
    {
        float savedVol = PlayerPrefs.GetFloat("masterVolume");
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(savedVol) * 20);
        float floatvalue;
        Debug.Log(mainMixer.GetFloat("MasterVolume", out floatvalue));
    }

    void Init()
    {
        // 메인 씬 배경음 플레이어 초기화
        GameObject mainBgmObject = new GameObject("mainBgmPlayer");
        mainBgmObject.transform.parent = transform;
        mainBgmPlayer = mainBgmObject.AddComponent<AudioSource>();
        mainBgmPlayer.playOnAwake = false;
        mainBgmPlayer.loop = true;
        mainBgmPlayer.volume = mainBgmVolume;
        mainBgmPlayer.clip = bgmClips[1];
        mainBgmPlayer.outputAudioMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClips[0];
        bgmPlayer.outputAudioMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
        }

        mainMixer.SetFloat("MasterVolume", Mathf.Log10(sliders[0].value) * 20);
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i =0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying) 
            {
                continue;
            }
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay && !bgmPlayer.isPlaying)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Pause();
        }
    }

    public void PlayMainBgm(bool isPlay)
    {
        if (isPlay && !mainBgmPlayer.isPlaying)
        {
            mainBgmPlayer.Play();
        }
        else
        {
            mainBgmPlayer.Pause();
        }
    }

    public void InitializeSlider()
    {
        for (int i = 0; i < 3; i++)
        {
            sliders[i].minValue = 0.0001f;
            sliders[i].maxValue = 1;
        }
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            sliders[0].value = 1;
            sliders[1].value = bgmVolume;
            sliders[2].value = sfxVolume;
            GameManager.Instance.masterVolume = 1;
            GameManager.Instance.backgroundVolume = bgmVolume;
            GameManager.Instance.sfxVolume = sfxVolume;
        }
        else
        {
            sliders[0].value = PlayerPrefs.GetFloat("masterVolume");
            sliders[1].value = PlayerPrefs.GetFloat("bgmVolume");
            sliders[2].value = PlayerPrefs.GetFloat("sfxVolume");
            mainBgmPlayer.volume = PlayerPrefs.GetFloat("bgmVolume");
            bgmPlayer.volume = PlayerPrefs.GetFloat("bgmVolume");
            for (int i = 0;i < sfxPlayers.Length;i++)
            {
                sfxPlayers[i].volume = PlayerPrefs.GetFloat("sfxPlayer");
            }
            GameManager.Instance.masterVolume = PlayerPrefs.GetFloat("masterVolume");
            GameManager.Instance.backgroundVolume = PlayerPrefs.GetFloat("bgmVolume");
            GameManager.Instance.sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(sliders[0].value) * 20);

    }

    public void MasterVolume()
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(sliders[0].value) * 20);
        GameManager.Instance.masterVolume = sliders[0].value;
    }

    public void BGMVolume()
    {
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(sliders[1].value) * 20);
        bgmPlayer.volume = sliders[1].value;
        mainBgmPlayer.volume = sliders[1].value;
        GameManager.Instance.backgroundVolume = sliders[1].value;
    }

    public void SFXVolume()
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sliders[2].value) * 20);
        for (int i = 0; i < sfxPlayers.Length;i++)
        {
            sfxPlayers[i].volume = sliders[2].value;
        }
        GameManager.Instance.sfxVolume = sliders[2].value;
    }

}
