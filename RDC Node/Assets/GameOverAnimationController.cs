using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverAnimationController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject gameOverCanvas;
    public Image resultImage;
    public Text resultText;
    public GameObject gameOverBlock;

    public void updateGameOverCanvas()
    {
        gameOverCanvas.GetComponent<Canvas>().sortingLayerName = "Menus";
        int winner = gameOverBlock.GetComponent<Animator>().GetInteger("winner");
        int humanPlayer = gameOverBlock.GetComponent<Animator>().GetInteger("humanPlayer");
        if(winner == 1)
        {
            resultImage.color = new Color(255, 141, 66);
        }
        else
        {
            resultImage.color = new Color(165, 77, 231);
        }

        if(humanPlayer == winner)
        {
            resultText.text = "You won";
        }
        else
        {
            resultText.text = "You lost";
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
