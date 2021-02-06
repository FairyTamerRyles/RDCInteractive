using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Hover : MonoBehaviour
{

    //public Color startColor;
    //public Color mouseOverColor;
    SpriteRenderer sprite;
    bool mouseOver = false;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }


    void OnMouseOver()
    {
        mouseOver = true;
   //     sprite.color = new Color(4, 2, 1, 2);// (Red, Green, Blue, Alpha);
        //GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        mouseOver = false;
     //   sprite.color = new Color(1, 0, 0, 1); // (Red, Green, Blue, Alpha);
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }
}
