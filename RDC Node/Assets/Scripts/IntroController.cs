using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    public bool canSkip;
    public GameObject nextSlide;

    public void playQuack()
    {

    }

    public void canSkipNow()
    {
        canSkip = true;
        GameObject.Find("SkipText").GetComponent<Animator>().SetTrigger("canSkip");
    }

    public void vs()
    {
        GameObject.Find("Sci").GetComponent<Animator>().SetBool("StartSlideIn", true);
        GameObject.Find("Yui").GetComponent<Animator>().SetBool("StartSlideIn", true);
        GameObject.Find("Vs").GetComponent<Animator>().SetBool("StartSlideIn", true);
    }

    public void stopAllFadeOut()
    {
        canSkip = false;
        GameObject[] introSlides = GameObject.FindGameObjectsWithTag("Intro");
        foreach(GameObject g in introSlides)
        {
            g.GetComponent<Animator>().SetBool("ToMenu", true);
            g.GetComponent<Animator>().SetBool("StartSlideIn", false);
        }
        GameObject.Find("IntroImage2").GetComponent<Animator>().SetBool("StartSlideIn", false);
    }

    public void startNextSlideIn()
    {
        nextSlide.GetComponent<Animator>().SetBool("StartSlideIn", true);
    }

    public void fadeToMenu()
    {
        GameObject.Find("RDC").GetComponent<Animator>().SetBool("FadeToMenu", true);
    }

    public void killEveryone()
    {
        GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("MainMenu");
    }

    public void blackOutKillScene()
    {
        GameObject.Find("Blackout").GetComponent<Animator>().SetBool("ToMenu", true);
    }

    public void blockPanels()
    {
        GameObject.Find("Darkener").GetComponent<Animator>().SetTrigger("blockPanels");
    }

    // Start is called before the first frame update
    void Start()
    {
        canSkip = false;
        GameObject.Find("SoundManager").GetComponent<SoundManager>().Play("Intro");
        GameObject.Find("SoundManager").GetComponent<SoundManager>().ChangeSoundMasterVolume_temp(0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey && canSkip)
        {
            blackOutKillScene();
        }
    }
}
