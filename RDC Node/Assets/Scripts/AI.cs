
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//All functions will default to void. Go back and fix as more information presents itself
public class AI : MonoBehaviour
{
    const float WIN = 1000000f;
    const float LOSE = -1000000f;
    //TODO: Make Gamepiece class; most likely in GameCore
    public GameBoard AIGameBoard;
    

    private class Vicki
    {
        private GameBoard[,] GameBoard;
        private bool isExpert;
        private float[] stratChoices;
        private float currentStrat;

        public void getStrat()
        {
            //stuffs
        }
        public void setStrat(float newStrat)
        {
            //stuffs
        }
    }

    public AI()
    {
        AIGameBoard = new GameBoard();
    }

    public class AIMove
    {
        //IDK Man
    }

    //based on the pseudoCode found in the negamax Wikipedia
    //currently only returns the value, MUST RETURN THE BOARD AS WELL
    float negamax(GameBoard position, float depth, float alpha, float beta, float player)
    {
        //first check if the game is over so you don't call all the heuristic searches
        float hvalue = gameOver() * player;
        if (hvalue == WIN || hvalue == LOSE)
        {
            return hvalue; //also need board position
        } 
        else if (depth == 0)
        {
            hvalue = heuristic() * player;
            return hvalue; //also need board position
        }
        float evaluation = 0;
        GameBoard[] legalMoves = getPossibleMoves(position, player);
        float baseValue = -Mathf.Infinity; //negative infinity
        foreach (GameBoard childBoard in legalMoves)
        {
            evaluation = Mathf.Max(baseValue, -negamax(childBoard, depth - 1, -beta, -alpha, -player));
            alpha = Mathf.Max(alpha, baseValue);
            if (alpha >= beta)
            {
                break;
            }
        }
        return evaluation;
    }
    public float gameOver()
    {
        float end = 0;
        return end;
        //determine if the game has been won;
    }
    public float heuristic()
    {
        float heuristicResult = 0;
        return heuristicResult;
    }
    GameBoard[] getPossibleMoves(GameBoard board, float player)
    {
        GameBoard gameBoard = new GameBoard();
        GameBoard[] result = new GameBoard[5];
        result[0] = gameBoard;
        return result;
    }
    void getPossibleNodes()
    {

        //ya
    }
    void getPossibleBranches()
    {

        //ya
    }
    void getPossibleCapturedTiles()
    {

        //ya
    }
    void getPotentialResources()
    {

    }
    void getResourceSpread()
    {

    }



    // Start is called before the first frame update
    void Start()
    {
        
    }
}
