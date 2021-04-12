using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class LoadingController : MonoBehaviour
{
    public GameObject loader;

    void Start()
    {
        float rnd = Random.Range(0.0f, 20.0f);
        int color = (int)(Floor(rnd));
        if(color % 2 == 0)
        {
            loader.GetComponent<Animator>().SetBool("Orange", true);
        }
    }

    // Start is called before the first frame update
    public void startGame()
    {
        GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
    }
}
