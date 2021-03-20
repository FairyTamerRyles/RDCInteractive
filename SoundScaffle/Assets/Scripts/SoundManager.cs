using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] audioSources;
    public AudioClip[] audioClips;

    public void Start()
    {
        audioSources = new AudioSource[audioClips.Length];

        for (int i = 0; i < audioClips.Length; ++i) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[i];
            audioSource.volume = 0;
            audioSource.loop = true;
            audioSource.Play();

            audioSources[i] = audioSource;
        }
    }

    public void Play() {
        audioSources[0].volume = 100;
    }

    public void Stop() {
        audioSources[0].volume = 0;
    }
}
