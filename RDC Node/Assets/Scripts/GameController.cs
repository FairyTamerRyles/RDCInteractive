using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    private AI testAI;

    public Button[] nodeButtons;
    public Button[] branchButtons;
    public Button[] tileButtons;

    //make setGameControllerReference
    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameBoard();
        testAI = new AI();
        testAI.AIGameBoard = gameBoard;
        /*for each(tile in gameBoard.gameTiles)
            find tag that matches coordinate
            find prefab tag that matches resource and limit
            instantiate prefab at said game objects coordinates*/
    }

    public void endTurn()
    {
        gameBoard.endTurn();
    }
}
