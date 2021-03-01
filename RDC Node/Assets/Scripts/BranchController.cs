using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BranchController : MonoBehaviour
{
    public GameObject controller;

    void Start()
    {

    }

    // Start is called before the first frame update
    void OnPointerEnter()
    {
        GameBoard.Coordinate nodeCoord = parseTag();
        if(!controller.GetComponent<GameController>().getGameBoard().isValidMove(nodeCoord))
        {
            //don't hover
        }
    }

    private GameBoard.Coordinate parseTag()
    {
        string pieceTag = this.tag;
        string[] coordinates = pieceTag.Split('.');
        GameBoard.Coordinate spriteCoord = new GameBoard.Coordinate{x = int.Parse(coordinates[0]), y = int.Parse(coordinates[1])};
        return spriteCoord;
    }
}
