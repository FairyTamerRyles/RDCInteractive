using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAnimationController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject gameOverCanvas;

    public void enableGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
