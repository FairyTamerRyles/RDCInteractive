using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameBoard();
    }
}
