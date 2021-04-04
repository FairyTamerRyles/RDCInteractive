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
    public GameObject leaveGame;
    public GameObject playAgain;
    public Sprite gameOver_Purple;
    public Sprite gameOver_Orange;

    public void updateGameOverCanvas()
    {
        gameOverCanvas.GetComponent<Canvas>().sortingLayerName = "Menus";
        int winner = gameOverBlock.GetComponent<Animator>().GetInteger("winner");
        int humanPlayer = gameOverBlock.GetComponent<Animator>().GetInteger("humanPlayer");
        if(winner == 1)
        {
            resultImage.sprite = gameOver_Orange;
        }
        else
        {
            resultImage.sprite = gameOver_Purple;
        }

        if(humanPlayer == winner)
        {
            resultText.text = "You won";
        }
        else
        {
            resultText.text = "You lost";
        }
        leaveGame.GetComponent<Button>().interactable = true;
        playAgain.GetComponent<Button>().interactable = true;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
