﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] audioSources;
    public AudioClip[] audioClips;
    public String[] audioNames;

    private AudioSource[] sfxSources;
    public AudioClip[] sfxClips;
    public String[] sfxNames;

    private AudioSource playing;

    private bool transitioning;
    private double transitionLength = 1;
    private double transitionProgress;
    private AudioSource transitioningTo;

    private float MusicVolume;
    private float SFXVolume;

    public Slider MusicSlider;

    public Slider SFXSlider;

    public void Start()
    {


        audioSources = new AudioSource[audioClips.Length];
        sfxSources = new AudioSource[sfxClips.Length];

        for (int i = 0; i < audioClips.Length; ++i) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[i];
            audioSource.volume = 0;
            audioSource.loop = true;

            audioSources[i] = audioSource;
        }

        foreach (AudioSource audioSource in audioSources) {
            audioSource.Play();
        }

        for (int i = 0; i < sfxClips.Length; ++i) {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.clip = sfxClips[i];
            sfxSource.volume = 100;
            sfxSource.loop = false;

            sfxSources[i] = sfxSource;
        }

        if(PlayerPrefs.HasKey("Music"))
        {
            MusicVolume = PlayerPrefs.GetFloat("Music");
            ChangeSoundMasterVolume(MusicVolume);
        }
        else
        {
            PlayerPrefs.SetFloat("Music", 0.4f);
            MusicVolume = PlayerPrefs.GetFloat("Music");
            ChangeSoundMasterVolume(MusicVolume);
        }
        if(PlayerPrefs.HasKey("SFX"))
        {
            SFXVolume = PlayerPrefs.GetFloat("SFX");
            ChangeSFXMasterVolume(SFXVolume);
        }
        else
        {
            PlayerPrefs.SetFloat("SFX", 0.8f);
            SFXVolume = PlayerPrefs.GetFloat("SFX");
            ChangeSFXMasterVolume(SFXVolume);
        }
        PlayerPrefs.Save();
        MusicSlider.value = MusicVolume;
        SFXSlider.value = SFXVolume;
        Play("BamGoozledMenu");
    }

    public float getMusicVolume()
    {
        return MusicVolume;
    }

    public float getSFXVolume()
    {
        return SFXVolume;
    }

    public void Play(int i = 0) {
        Stop();
        audioSources[i].volume = MusicVolume;
        playing = audioSources[i];
    }

    public void Play(String name) {
        int index = 0;

        for (int i = 0; i < audioNames.Length; ++i) {
            if (audioNames[i] == name) index = i;
        }

        Play(index);
    }

    public void PlaySFX(int i = 0) {
        sfxSources[i].Play();
    }

    public void PlaySFX(String name) {
        int index = 0;

        for (int i = 0; i < sfxNames.Length; ++i) {
            if (sfxNames[i] == name) index = i;
        }

        PlaySFX(index);
    }

    public void StopSFX(int i = 0) {
        sfxSources[i].Stop();
    }

    public void StopSFX(String name) {
        int index = 0;

        for (int i = 0; i < sfxNames.Length; ++i) {
            if (sfxNames[i] == name) index = i;
        }

        StopSFX(index);
    }

    public void ToggleLoopSFX(int i = 0) {
        sfxSources[i].loop = !sfxSources[i].loop;
    }

    public void ToggleLoopSFX(String name) {
        int index = 0;

        for (int i = 0; i < sfxNames.Length; ++i) {
            if (sfxNames[i] == name) index = i;
        }

        ToggleLoopSFX(index);
    }

    public void Stop() {
        foreach (AudioSource audioSource in audioSources) {
            audioSource.volume = 0;
        }

        playing = null;
    }

    public void Transition(int i = 0) {
        transitionProgress = 0.0;
        transitioningTo = audioSources[i];
        transitioning = true;
    }

    public void Transition(String name) {
        int index = 0;

        for (int i = 0; i < audioNames.Length; ++i) {
            if (audioNames[i] == name) index = i;
        }

        Transition(index);
    }

    public void TransitionToSilence() {
        transitionProgress = 0.0;
        transitioningTo = null;
        transitioning = true;
    }

    void FixedUpdate() {
        if (transitioning) {
            transitionProgress += 0.02;

            if (transitionProgress >= transitionLength) {
                if (playing != null) playing.volume = 0;
                if (transitioningTo != null) transitioningTo.volume = MusicVolume;
                playing = transitioningTo;
                transitioningTo = null;
                transitioning = false;
            } else {
                float playingVolume = (float)(MusicVolume - (transitionProgress / transitionLength));
                if(playingVolume < 0)
                {
                    playingVolume = 0;
                }
                if (playing != null) playing.volume = playingVolume;
                if (transitioningTo != null) transitioningTo.volume = (float)(MusicVolume - playingVolume);
            }
        }
    }

    public void ChangeMasterVolume(System.Single volume) {
        ChangeSoundMasterVolume(volume);
        ChangeSFXMasterVolume(volume);
    }

    public void ChangeSoundMasterVolume(System.Single volume) {
        foreach (AudioSource audioSource in audioSources) {
            if(MusicSlider.value == 0)
            {
                MusicSlider.value = .01f;
            }
            if(audioSource.volume != 0)
            {
                audioSource.volume = MusicSlider.value;
            }
        }
        MusicVolume = volume;
        PlayerPrefs.SetFloat("Music", MusicVolume);
        PlayerPrefs.Save();
    }

    public void ChangeSoundMasterVolume() {
        foreach (AudioSource audioSource in audioSources) {
            if(MusicSlider.value == 0)
            {
                MusicSlider.value = .01f;
            }
            if(audioSource.volume != 0)
            {
                audioSource.volume = MusicSlider.value;
            }
        }
        MusicVolume = MusicSlider.value;
        PlayerPrefs.SetFloat("Music", MusicVolume);
        PlayerPrefs.Save();
    }

    public void ChangeSoundMasterVolume_temp(float volume) {
        foreach (AudioSource audioSource in audioSources) {
            if(MusicSlider.value == 0)
            {
                MusicSlider.value = .01f;
            }
            if(audioSource.volume != 0)
            {
                audioSource.volume = volume;
            }
        }
    }

    public void ChangeSFXMasterVolume() {
        foreach (AudioSource sfxSource in sfxSources) {
            sfxSource.volume = SFXSlider.value;
        }
        SFXVolume = SFXSlider.value;
        PlayerPrefs.SetFloat("SFX", SFXVolume);
        PlayerPrefs.Save();
        int rSound = (int)(Floor(UnityEngine.Random.Range(1.0f, 3.0f)));
        PlaySFX("SlimeSpawn" + rSound);
    }

    public void ChangeSFXMasterVolume(System.Single volume) {
        foreach (AudioSource sfxSource in sfxSources) {
            sfxSource.volume = volume;
        }
        SFXVolume = volume;
        PlayerPrefs.SetFloat("SFX", SFXVolume);
        PlayerPrefs.Save();
        int rSound = (int)(Floor(UnityEngine.Random.Range(1.0f, 3.0f)));
    }

    public void ChangeSoundVolume(int index, float volume) {
        audioSources[index].volume = volume;
    }

    public void ChangeSoundVolume(String name, float volume) {
       int index = 0;

        for (int i = 0; i < audioNames.Length; ++i) {
            if (audioNames[i] == name) index = i;
        }

        ChangeSoundVolume(index, volume);
    }

    public void ChangeSFXVolume(int index, float volume) {
        sfxSources[index].volume = volume;
    }

    public void ChangeSFXVolume(String name, float volume) {
        int index = 0;

        for (int i = 0; i < sfxNames.Length; ++i) {
            if (sfxNames[i] == name) index = i;
        }

        ChangeSFXVolume(index, volume);
    }

    public void playClick()
    {
        PlaySFX("MarkerCap");
    }
    public void playThud()
    {
        PlaySFX("MarkerOnDryErase");
    }
}