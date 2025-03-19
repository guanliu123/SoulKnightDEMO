using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{
    private AudioSource bkMusic = null;
    public float bkValue = 1;
    public float soundValue = 1;
    private GameObject soundObj = null;
    private List<AudioSource> playingList = new List<AudioSource>();
    private List<AudioSource> emptyList = new List<AudioSource>();

    public AudioManager()
    {
    }

    public override void Init()
    {
        base.Init();
        MonoManager.Instance.AddUpdate100msAction(update);
    }

    private void update()
    {
        for (int i = playingList.Count - 1; i >= 0; i--)
        {
            if (playingList[i] == null)
            {
                playingList.RemoveAt(i);
            }
            else if (!playingList[i].isPlaying)
            {
                emptyList.Add(playingList[i]);
                playingList.RemoveAt(i);
            }
        }
    }

    public void PlayBKMusic(string musciName)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("BKMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }

        LoadManager.Instance.LoadAsync<AudioClip>(ResourcePath.Audio + musciName+".wav", (clip) =>
        {
            if (clip == null) return;
            var c=clip.asset() as AudioClip;
            bkMusic.clip = c;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }

    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }

    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    public void StopBkMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }
    
    public void PlaySound(string audioName, bool isLoop = false)
    {
        var audio = LoadManager.Instance.Load<AudioClip>(ResourcePath.Audio+audioName+".wav");
        PlaySound(audio, isLoop);
    }

    public void PlaySound(AudioClip audio, bool isLoop = false)
    {
        if (!audio) return;
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Audios";
        }
        AudioSource source;
        if(emptyList.Count > 0&&emptyList[0] == null)
        {
            emptyList.Clear();
        }
        if (emptyList.Count <= 0)
        {
            source = soundObj.AddComponent<AudioSource>();
        }
        else
        {
            source = emptyList[0];
            emptyList.RemoveAt(0);
        }
        playingList.Add(source);

        source.clip = audio;
        source.loop = isLoop;
        source.volume = soundValue;
        source.Play();
    }

    /*public async void PlaySound(string audioName, bool isLoop = false, UnityAction<AudioSource> callback = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sounds";
        }

        AudioSource source;
        if (emptyList.Count <= 0) source = soundObj.AddComponent<AudioSource>();
        else
        {
            source = emptyList[0];
            playingList.Add(emptyList[0]);
            emptyList.RemoveAt(0);
        }
        await LoadManager.Instance.LoadAsync<AudioClip>(audioName, result =>
        {
            source.clip = result;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
        }, () => { }, ResourceType.Audio);
        /*source.clip = audio;
        source.loop = isLoop;
        source.volume = soundValue;
        source.Play();#1#
    }*/

    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for (int i = 0; i < playingList.Count; i++)
            playingList[i].volume = value;
    }

    public void StopSound(AudioSource source)
    {
        if (playingList.Contains(source))
        {
            playingList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}