//Code written by Riley Judd
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
    
    public struct CapTileChecker
    {
        public List<GameBoard.Tile> tileStack;
        public bool isCaptured;

        public CapTileChecker(List<GameBoard.Tile> tileStack, bool isCaptured)
        {
            this.tileStack = tileStack;
            this.isCaptured = isCaptured;
        }
    

    public AI()
    {
        AIGameBoard = new GameBoard();
    }

    public class AIMove
    {
        //IDK Man
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
    public void setCapturedTiles(List<GameBoard.Tile> noncapturedTiles, GameBoard.Player player)
    {
        while (noncapturedTiles.Any())
        {
            CapTileChecker checkResults = checkIfCaptured (AIGameBoard, noncapturedTiles[0], new CapTileChecker(new List<GameBoard.Tile>(), false), player);
            if (checkResults.isCaptured)
            {
                foreach (GameBoard.Tile tile in checkResults.tileStack)
                {
                    tile.player = player;
                    if(noncapturedTiles.Contains(tile))
                    {
                        noncapturedTiles.Remove(tile);
                        Debug.Log(tile.coord.x + " - " + tile.coord.y + " has been set to captured and removed from the captured list.");
                    }
                }
            } else
            {
                foreach (GameBoard.Tile tile in checkResults.tileStack)
                {
                    if(noncapturedTiles.Contains(tile))
                    {
                        noncapturedTiles.Remove(tile);
                        Debug.Log(tile.coord.x + " - " + tile.coord.y + " has been removed from the captured list.");
                    }
                }
            }
        }
    }
    public CapTileChecker checkIfCaptured(GameBoard board, GameBoard.Tile currentTile, CapTileChecker checkedTiles, GameBoard.Player player)
    {
        //Debug.Log("Now checking Tile: " + currentTile.coord.x + " - " + currentTile.coord.y);
        //first Check for any insta-fails on the surrounding branches/tiles
        if ((board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y].player != GameBoard.Player.None && board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y].player != player) ||
            (board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y].player != GameBoard.Player.None && board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y].player != player) ||
            (board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1].player != GameBoard.Player.None && board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1].player != player) ||
            (board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1].player != GameBoard.Player.None && board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1].player != player))  
        {
            //Opponent branch found. Mission failed.
            //Debug.Log("Opponent Branch found around Tile " + currentTile.coord.x + " - " + currentTile.coord.y);
            if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles;
        } else if ((board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y].player == GameBoard.Player.None && !inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x - 2, y = currentTile.coord.y})) ||
                    (board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y].player == GameBoard.Player.None && !inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x + 2, y = currentTile.coord.y})) ||
                    (board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1].player == GameBoard.Player.None && !inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x, y = currentTile.coord.y - 2})) ||
                    (board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1].player == GameBoard.Player.None && !inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x, y = currentTile.coord.y + 2})))
         {
             //Debug.Log("There was an empty branch with no tile on the other side around Tile " + currentTile.coord.x + " - " + currentTile.coord.y);
             //The branch is empty and there are no potential tiles in its direction. Mission failed.
             if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles; 
         }
         //This is gonna be real painful. The branch is either yours and you just move on or you wait for a recursive return and determine what happens based on the result.
         else if (isYourBranch(board, currentTile, player, "up") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x - 2, currentTile.coord.y]))
        {
            //Debug.Log("Up passed");
            if (isYourBranch(board, currentTile, player, "left") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y - 2]))
            {
                //Debug.Log("Left passed");
                if (isYourBranch(board, currentTile, player, "right") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2]))
                {
                    //Debug.Log("Right passed");
                    if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                    {
                        //Debug.Log("Down passed. Tile Captured.");
                        //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                        if (!checkedTiles.tileStack.Contains(currentTile))
                        {
                            checkedTiles.tileStack.Add(currentTile);
                        }
                        checkedTiles.isCaptured = true;
                        return checkedTiles;
                    } else
                    {
                        //down was empty. Start the recursion
                        if (!checkedTiles.tileStack.Contains(currentTile))
                        {
                            checkedTiles.tileStack.Add(currentTile);
                        }
                        CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                        //this is the final case. It doesn't matter if it returns true or false. Whatever the result is will be the result
                        return recursiveTileResults_Down;
                    }
                } else
                {
                    //right was empty. Start the recursion.
                    if (!checkedTiles.tileStack.Contains(currentTile))
                    {
                        checkedTiles.tileStack.Add(currentTile);
                    }
                    CapTileChecker recursiveTileResults_Right = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2], checkedTiles, player);
                    if (recursiveTileResults_Right.isCaptured == true)
                    {
                        //it passed. Update checkedTiles and move on to next branch;
                        checkedTiles = recursiveTileResults_Right;
                        if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                        {
                            //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            checkedTiles.isCaptured = true;
                            return checkedTiles;
                        } else
                        {
                            //down was empty. Start the recursion.
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                            //this is the final case. It doesn't matter if it returns true or false. Whatever the result is the result
                            return recursiveTileResults_Down;
                        }
                    } else
                    {
                        //it failed. return the failed recursiveTileResults
                        return recursiveTileResults_Right;
                    }
                }
            } else
            {
                //left was empty. Start the recursion.
                if (!checkedTiles.tileStack.Contains(currentTile))
                {
                    checkedTiles.tileStack.Add(currentTile);
                }
                CapTileChecker recursiveTileResults_Left = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y - 2], checkedTiles, player);
                if (recursiveTileResults_Left.isCaptured == true)
                {
                    //it passed. Update checkedTiles and move to the next branch.
                    checkedTiles = recursiveTileResults_Left;
                    if (isYourBranch(board, currentTile, player, "right") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2]))
                    {
                        if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                        {
                            //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            checkedTiles.isCaptured = true;
                            return checkedTiles;
                        } else
                        {
                            //down was empty. Start the recursion
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                            //this is the final case. It doesn't matter if it returns true or false. Whatever the result is will be the result
                            return recursiveTileResults_Down;
                        }
                    } else
                    {
                        //right was empty. Start the recursion.
                        if (!checkedTiles.tileStack.Contains(currentTile))
                        {
                            checkedTiles.tileStack.Add(currentTile);
                        }
                        CapTileChecker recursiveTileResults_Right = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2], checkedTiles, player);
                        if (recursiveTileResults_Right.isCaptured == true)
                        {
                            //it passed. Update checkedTiles and move on to next branch;
                            checkedTiles = recursiveTileResults_Right;
                            if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                            {
                                //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                checkedTiles.isCaptured = true;
                                return checkedTiles;
                            } else
                            {
                                //down was empty. Start the recursion.
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                                //this is the final case. It doesn't matter if it returns true or false. Whatever the result is the result
                                return recursiveTileResults_Down;
                            }
                        } else
                        {
                            //it failed. return the failed recursiveTileResults
                            return recursiveTileResults_Right;
                        }
                    }
                } else
                {
                    //it failed. Return the failed recursiveTileResults
                    return recursiveTileResults_Left;
                }
            }
        } else
        {
            //up was empty. Start the recursion.
            if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            CapTileChecker recursiveTileResults_Up = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x - 2, currentTile.coord.y], checkedTiles, player);
            if (recursiveTileResults_Up.isCaptured == true)
            {
                //it passed. Update checkedTiles and move to the next branch.
                checkedTiles = recursiveTileResults_Up;
                if (isYourBranch(board, currentTile, player, "left") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y - 2]))
                {
                    if (isYourBranch(board, currentTile, player, "right") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2]))
                    {
                        if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                        {
                            //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            checkedTiles.isCaptured = true;
                            return checkedTiles;
                        } else
                        {
                            //down was empty. Start the recursion
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                            //this is the final case. It doesn't matter if it returns true or false. Whatever the result is will be the result
                            return recursiveTileResults_Down;
                        }
                    } else
                    {
                        //right was empty. Start the recursion.
                        if (!checkedTiles.tileStack.Contains(currentTile))
                        {
                            checkedTiles.tileStack.Add(currentTile);
                        }
                        CapTileChecker recursiveTileResults_Right = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2], checkedTiles, player);
                        if (recursiveTileResults_Right.isCaptured == true)
                        {
                            //it passed. Update checkedTiles and move on to next branch;
                            checkedTiles = recursiveTileResults_Right;
                            if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                            {
                                //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                checkedTiles.isCaptured = true;
                                return checkedTiles;
                            } else
                            {
                                //down was empty. Start the recursion.
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                                //this is the final case. It doesn't matter if it returns true or false. Whatever the result is the result
                                return recursiveTileResults_Down;
                            }
                        } else
                        {
                            //it failed. return the failed recursiveTileResults
                            return recursiveTileResults_Right;
                        }
                    }
                } else
                {
                    //left was empty. Start the recursion.
                    if (!checkedTiles.tileStack.Contains(currentTile))
                    {
                        checkedTiles.tileStack.Add(currentTile);
                    }
                    CapTileChecker recursiveTileResults_Left = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y - 2], checkedTiles, player);
                    if (recursiveTileResults_Left.isCaptured == true)
                    {
                        //it passed. Update checkedTiles and move to the next branch.
                        checkedTiles = recursiveTileResults_Left;
                        if (isYourBranch(board, currentTile, player, "right") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2]))
                        {
                            if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                            {
                                //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                checkedTiles.isCaptured = true;
                                return checkedTiles;
                            } else
                            {
                                //down was empty. Start the recursion
                                if (!checkedTiles.tileStack.Contains(currentTile))
                                {
                                    checkedTiles.tileStack.Add(currentTile);
                                }
                                CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                                //this is the final case. It doesn't matter if it returns true or false. Whatever the result is will be the result
                                return recursiveTileResults_Down;
                            }
                        } else
                        {
                            //right was empty. Start the recursion.
                            if (!checkedTiles.tileStack.Contains(currentTile))
                            {
                                checkedTiles.tileStack.Add(currentTile);
                            }
                            CapTileChecker recursiveTileResults_Right = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2], checkedTiles, player);
                            if (recursiveTileResults_Right.isCaptured == true)
                            {
                                //it passed. Update checkedTiles and move on to next branch;
                                checkedTiles = recursiveTileResults_Right;
                                if(isYourBranch(board, currentTile, player, "down") || checkedTiles.tileStack.Contains(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y]))
                                {
                                    //down was your branch. The tile is captured. Add yourself to checkedTiles, set it to true, and return it.
                                    if (!checkedTiles.tileStack.Contains(currentTile))
                                    {
                                        checkedTiles.tileStack.Add(currentTile);
                                    }
                                    checkedTiles.isCaptured = true;
                                    return checkedTiles;
                                } else
                                {
                                    //down was empty. Start the recursion.
                                    if (!checkedTiles.tileStack.Contains(currentTile))
                                    {
                                        checkedTiles.tileStack.Add(currentTile);
                                    }
                                    CapTileChecker recursiveTileResults_Down = checkIfCaptured(board, (GameBoard.Tile)board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], checkedTiles, player);
                                    //this is the final case. It doesn't matter if it returns true or false. Whatever the result is the result
                                    return recursiveTileResults_Down; 
                                }
                            } else
                            {
                                //it failed. return the failed recursiveTileResults
                                return recursiveTileResults_Right;
                            }
                        }
                    } else
                    {
                        //it failed. Return the failed recursiveTileResults
                        return recursiveTileResults_Left;
                    }
                }
            } else
            {
                //it failed. return the failed recursiveTileResults
                return recursiveTileResults_Up;
            }
        }
    }
    bool isYourBranch (GameBoard board, GameBoard.Tile currentTile, GameBoard.Player player, string direction) 
    {
        switch (direction)
        {
            case "up":
                if (board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y].player == player) 
                {
                    return true;
                } else 
                {
                    return false;
                }
            case "down":
                if (board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y].player == player)
                {
                    return true;
                } else 
                {
                    return false;
                }
            case "left":
                if (board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1].player == player)
                {
                    return true;
                } else 
                {
                    return false;
                }
            case "right":
                if (board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1].player == player)
                {
                    return true;
                } else 
                {
                    return false;
                }
        }
        //base case; THIS SHOULD NEVER BE CALLED
        return false;
    }
    bool inBoundsTile (GameBoard board, GameBoard.Coordinate c)
    {
        if (c.x < 0 || c.x > 10 || c.y < 0 || c.y > 10)
        {
            return false;
        } else if (board.gameBoard[c.x, c.y] == null)
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
