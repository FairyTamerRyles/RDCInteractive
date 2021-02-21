using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class AdamRandomAI : MonoBehaviour
{
    private float rnd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameBoard gameBoard;

    public AdamRandomAI(GameBoard board)
    {
        gameBoard = board;
    }

    public GameBoard makeRandomAIMove(GameBoard gBoard)
    {
        gameBoard = gBoard;
        while(true)
        {
            List<GameBoard.Coordinate> possiblePiecePlacements = new List<GameBoard.Coordinate>();
            List<int[]> possibleTrades = new List<int[]>();
            
            //finds all possible piece placements and stores them in possiblePiecePlacements
            for(int i = 0; i < GameBoard.boardSize; ++i)
            {
                for(int j = 0; j < GameBoard.boardSize; ++j)
                {
                    GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = i, y = j};
                    if(gameBoard.isValidMove(testMove))
                    {
                        possiblePiecePlacements.Add(testMove);
                    }
                }
            }

            //finds all possible trades and stores them in possibleTrades
            for(int selectedResource = 0; selectedResource < 4; ++selectedResource)
            {
                for(int r1 = 0; r1 <= 3; ++r1)
                {
                    for(int r2 = 0; r2 <= 3 - r1; ++r2)
                    {
                        int r3 = 3 - r1 - r2;
                        int[] resourcesToSpend = new int[]{r1, r2, r3};
                        int[] testTrade = new int[4];
                        int resourcesToSpendIndex = 0;
                        for(int i = 0; i < testTrade.Length; ++i)
                        {
                            if(i == selectedResource)
                            {
                                testTrade[i] = 1;
                            }
                            else
                            {
                                testTrade[i] = resourcesToSpend[resourcesToSpendIndex] * -1;
                                ++resourcesToSpendIndex;
                            }
                        }
                        if(gameBoard.isValidTrade(testTrade))
                        {
                            possibleTrades.Add(testTrade);
                        }
                    }
                }
            }

            //Picks one possible move and applies it
            if(possiblePiecePlacements.Count == 0 && possibleTrades.Count == 0)
            {
                return gameBoard;
            }
            else if(possiblePiecePlacements.Count == 0)
            {
                rnd = Random.Range(0.0f, (float)possibleTrades.Count);
                int selectedMove = (int)(Floor(rnd));
                gameBoard.makeTrade(possibleTrades[selectedMove]);
            }
            else if(possibleTrades.Count == 0)
            {
                rnd = Random.Range(0.0f, (float)possiblePiecePlacements.Count);
                int selectedMove = (int)(Floor(rnd));
                gameBoard.placePiece(possiblePiecePlacements[selectedMove]);
            }
            else
            {
                rnd = Random.Range(0.0f, 1.0f);
                if(rnd <= 0.8f)
                {
                    rnd = Random.Range(0.0f, (float)possiblePiecePlacements.Count);
                    int selectedMove = (int)(Floor(rnd));
                    gameBoard.placePiece(possiblePiecePlacements[selectedMove]);
                }
                else
                {
                    rnd = Random.Range(0.0f, (float)possibleTrades.Count);
                    int selectedMove = (int)(Floor(rnd));
                    gameBoard.makeTrade(possibleTrades[selectedMove]);
                }
            }
        }

        return gameBoard;
    }
}
