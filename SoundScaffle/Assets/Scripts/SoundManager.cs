using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] audioSources;
    public AudioClip[] audioClips;

    private AudioSource playing;

    private bool transitioning;
    private double transitionLength = 5.0;
    private double transitionProgress;
    private AudioSource transitioningTo;

    public void Start()
    {
        audioSources = new AudioSource[audioClips.Length];

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
    }

    public void Play(int i = 0) {
        Stop();
        audioSources[i].volume = 100;
        playing = audioSources[i];
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
        Debug.Log("Transition Began");
    }

    void FixedUpdate() {
        if (transitioning) {
            transitionProgress += 0.02;

            if (transitionProgress >= transitionLength) {
                playing.volume = 0;
                transitioningTo.volume = 100;
                playing = transitioningTo;
                transitioningTo = null;
                transitioning = false;
                Debug.Log("Transition Ended");
            } else {
                playing.volume = 100 - Convert.ToInt32((transitionProgress / transitionLength) * 100);
                transitioningTo.volume = 100 - playing.volume;
            }
        }
    }
}
