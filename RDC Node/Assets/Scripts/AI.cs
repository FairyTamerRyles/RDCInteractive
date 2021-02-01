/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//All functions will default to void. Go back and fix as more information presents itself

public class AI : MonoBehaviour
{
    const WIN = 1000000;
    const LOSE = -1000000;
    //TODO: Make Gamepiece class; most likely in GameCore
    public GameBoard AIGameBoard;
    

    private class Vicki
    {
        private GameBoard[,] GameBoard;
        private bool isExpert;
        private int[] stratChoices;
        private int currentStrat;

        public void getStrat();
        public void setStrat(int newStrat);
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
    void negamax(GameBoard position, float depth, float alpha, int beta, int player)
    {
        //first check if the game is over so you don't call all the heuristic searches
        hvalue = gameOver() * player;
        if (hvalue == WIN || hvalue == LOSE) \
        {
            return hvalue; //also need board position
        } 
        else if (depth == 0)
        {
            hvalue = heuristic() * player;
            return hvalue; //also need board position
        }
        legalMoves = getPossibleMoves(position, player);
        baseValue = -1 / 0; //negative infinity
        foreach (Gameboard childBoard in legalMoves)
        {
            float evaluation = max(value, -negamax(childBoard, depth - 1, -beta, -alpha, -color));
            alpha = max(alpha, value);
            if (alpha >= beta)
            {
                break;
            }
        }
        return evaluation;
    }
    public int gameOver();
    {
        //determine if the game has been won;
    }
    public float heuristic()
    {
        int heuristicResult;
        return heuristicResult;
    }
    GameBoard[] getPossibleMoves(int player)
    {

    }
    void getPossibleNodes()
    {

    }
    void getPossibleBranches()
    {

    }
    void getPossibleCapturedTiles()
    {

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
*/
