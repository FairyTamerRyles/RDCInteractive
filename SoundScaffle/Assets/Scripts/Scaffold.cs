using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffold : MonoBehaviour
{
    public SoundManager soundManager;
    private bool flag = false;

    public void Play() {
        soundManager.Play(2);
    }

    public void Stop() {
        soundManager.Stop();
    }

    public void Transition() {
        if (flag) {
            soundManager.Transition(2);
        } else {
            soundManager.Transition(3);
        }

        flag = !flag;
    }
}
