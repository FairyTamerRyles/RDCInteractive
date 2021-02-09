
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//All functions will default to void. Go back and fix as more information presents itself
public class AI
{
    const float WIN = 1000000f;
    const float LOSE = -1000000f;
    //TODO: Make Gamepiece class; most likely in GameCore
    public GameBoard AIGameBoard;
    
    public struct capTileChecker
    {
        List<GameBoard.Tile> tileStack;
        bool isCaptured;

        public capTileChecker(List<GameBoard.Tile> tileStack, bool isCaptured)
        {
            this.tileStack = tileStack;
            this.isCaptured = isCaptured;
        }
    }

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
    List<GameBoard.Tile> setCapturedTiles(List<GameBoard.Tile> noncapturedTiles)
    {
        foreach (GameBoard.Tile tile in noncapturedTiles)
        {

        }
    }
    capTileChecker checkIfCaptured(GameBoard.Tile currentTile, capTileChecker checkedTiles, int player)
    {
        //first Check for any insta fails on the surrounding branches/tiles
        if ((AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player != player && AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player != 0) ||
            (AIGameBoard.gameBoard[currentTile.coord.x + 1][currentTile.coord.y].player != player && AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player != 0) ||
            (AIGameBoard.gameBoard[currentTile.coord.x][currentTile.coord.y - 1].player != player && AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player != 0) ||
            (AIGameBoard.gameBoard[currentTile.coord.x][currentTile.coord.y + 1].player != player && AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player != 0))  
        {
            //Opponent branch found. Mission failed.
            if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles;
        } else if ((AIGameBoard.gameBoard[currentTile.coord.x - 1][currentTile.coord.y].player == 0 &&!inBoundsTile(new GameBoard.Coordinate() {x = currentTile.coord.x - 2, y = currentTile.coord.y})) ||
                    (AIGameBoard.gameBoard[currentTile.coord.x + 1][currentTile.coord.y].player == 0 &&!inBoundsTile(new GameBoard.Coordinate() {x = currentTile.coord.x + 2, y = currentTile.coord.y}))
                    (AIGameBoard.gameBoard[currentTile.coord.x][currentTile.coord.y - 1].player == 0 &&!inBoundsTile(new GameBoard.Coordinate() {x = currentTile.coord.x, y = currentTile.coord.y - 2}))
                    (AIGameBoard.gameBoard[currentTile.coord.x][currentTile.coord.y + 1].player == 0 &&!inBoundsTile(new GameBoard.Coordinate() {x = currentTile.coord.x, y = currentTile.coord.y + 2})))
         {
             //The branch is empty and there are no potential tiles in its direction. Mission failed.
             if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles;
         }
         //we didn't immediately fail. Time to start doing some actual searching

    }
    bool inBounds (GameBoard.Coordinate c)
    {
        if (c.x < 0 || c.x > 10 || c.y < 0 || c.y > 10)
        {
            return false;
        } else if (AIGameBoard.gameBoard[c.x][c.y] == null)
        {
            //if this doesn't work try the is operator
            return false;
        }
        return true;
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
