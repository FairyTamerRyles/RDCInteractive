using System;
using System.Collections.Generic;
using UnityEngine;

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
    private double transitionLength = 5.0;
    private double transitionProgress;
    private AudioSource transitioningTo;

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
    }

    public void Play(int i = 0) {
        Stop();
        audioSources[i].volume = 1;
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

    void FixedUpdate() {
        if (transitioning) {
            transitionProgress += 0.02;

            if (transitionProgress >= transitionLength) {
                playing.volume = 0;
                transitioningTo.volume = 1;
                playing = transitioningTo;
                transitioningTo = null;
                transitioning = false;
            } else {
                playing.volume = (float)(1 - (transitionProgress / transitionLength));
                transitioningTo.volume = (float)(1 - playing.volume);
            }
        }
    }
}
