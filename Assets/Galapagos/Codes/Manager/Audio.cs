using Unity.VisualScripting;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio instance;

    [Header("#BGM")]
    public AudioClip bgmclip;
    public float bgmvolume;
    AudioSource bgmplayer;

    [Header("#SFX")]
    public AudioClip[] sfxclips;
    public float sfxvolume;
    public int channels;
    AudioSource[] sfxplayers;
    int channelindex;

    public enum sfx { Dead, Hit, Levelup=3, Lose, Melee, Range=7, Select, Win}


    private void Awake()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        //배경음 플레이커초기화
        GameObject bgmObject = new GameObject("bgmplayer");
        bgmObject.transform.parent = transform;
        bgmplayer = bgmObject.AddComponent<AudioSource>();
        bgmplayer.playOnAwake = false;
        bgmplayer.loop = true;
        bgmplayer.volume = bgmvolume;
        bgmplayer.clip = bgmclip;
        //효과음
        GameObject sfxObject = new GameObject("sfxplayer");
        sfxObject.transform.parent = transform;
        sfxplayers = new AudioSource[channels];

        for (int index = 0; index < sfxplayers.Length; index++)
        {
            sfxplayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxplayers[index].playOnAwake = false ;
            sfxplayers[index].volume = sfxvolume;
        }
    }

    public void  playsfx(sfx sfx)
    {
        for (int index = 0; index < sfxplayers.Length; index++) {
            int loopindex = (index + channelindex) % sfxplayers.Length;

            if (sfxplayers[loopindex].isPlaying)
                continue;

        channelindex = loopindex;
        sfxplayers[loopindex].clip = sfxclips[(int)sfx];
        sfxplayers[loopindex].Play();
        break;
        }

    }
    public void PlaySelectSFX()
    {
       // Audio.instance.playsfx(Audio.sfx.Select); 다른스크립트에서 쓸때
        playsfx(sfx.Select);
    }

    public void PlayWinSFX()
    {
        playsfx(sfx.Win);
    }
    public void PlayBGM()
    {
        if (bgmplayer != null && bgmclip != null)
        {
            bgmplayer.clip = bgmclip;
            bgmplayer.volume = bgmvolume;
            bgmplayer.loop = true;
            bgmplayer.Play();
        }
    }


    public void SetBGMVolume(float volume)
    {
       // Debug.Log("BGM Volume changed to: " + volume);
        bgmvolume = volume/4;
        if (bgmplayer != null)
            bgmplayer.volume = volume/4;
    }

    public void SetSFXVolume(float volume)
    {
        sfxvolume = volume;
        if (sfxplayers != null)
        {
            foreach (AudioSource player in sfxplayers)
            {
                player.volume = volume;
            }
        }
    }



}
