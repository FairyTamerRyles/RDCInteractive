using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class LoadingController : MonoBehaviour
{
    // Start is called before the first frame update
    public void startGame()
    {
        GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
    }
}
