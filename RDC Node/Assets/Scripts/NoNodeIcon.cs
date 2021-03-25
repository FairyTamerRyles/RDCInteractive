using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoNodeIcon : MonoBehaviour
{
    public GameObject controller;

    private Animator animator;

    private bool hovered;

    Color mouseOverColor = Color.cyan;

    Color originalColor;

    SpriteRenderer sprite;

    void Start()
    {
        Debug.Log(this.name);
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
        animator = GetComponent<Animator>(); 
    }

    // Start is called before the first frame update
    void OnMouseEnter()
    {
        GameBoard.Coordinate nodeCoord = parseName();
        if(controller.GetComponent<GameController>().getGameBoard().getCurrentPlayer() == controller.GetComponent<GameController>().humanPlayer && controller.GetComponent<GameController>().getGameBoard().isValidMove(nodeCoord))
        {
            Debug.Log("Valid Hover");
            hovered = true;
            animator.SetBool("validHover", true);
        }
    }

    void OnMouseExit()
    {
        if(hovered)
        {
            hovered = false;
            animator.SetBool("validHover", false);
        }
    }

    private GameBoard.Coordinate parseName()
    {
        string pieceName = this.name;
        string[] coordinates = pieceName.Split(',');
        GameBoard.Coordinate spriteCoord = new GameBoard.Coordinate{x = int.Parse(coordinates[0]), y = int.Parse(coordinates[1])};
        return spriteCoord;
    }
}
