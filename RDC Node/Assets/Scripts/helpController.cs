using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpController : MonoBehaviour
{
    public void switchAnimation(int newSlide)
    {
        GameObject.Find("HelpImage").GetComponent<Animator>().SetInteger("slide", newSlide);
    }
    public void resetAnimation()
    {
        GameObject.Find("HelpImage").GetComponent<Animator>().SetInteger("slide", 1);
    }
}

