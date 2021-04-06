//Code written by Riley Judd
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Math;

//All functions will default to void. Go back and fix as more information presents itself
public class AI
{
    const float WIN = 1000000000.0f;
    const float LOSE = -1000000000.0f;
    //TODO: Make Gamepiece class; most likely in GameCore
    public GameBoard AIGameBoard;
    public GameBoard.Player opponent;
    public GameBoard.Player self;
    private int strat;
    public float[] hw;
    public int hwLength;


    //public MonteCarloTree Freederick;
    public struct moveResult
    {
        public GameBoard.GamePiece[,] board;
        public int[] Player1Pieces;
        public int[] Player2Pieces;
        public moveResult(GameBoard gb)
        {
            board = gb.gameBoard;
            Player1Pieces = gb.getResources(GameBoard.Player.Player1);
            Player2Pieces = gb.getResources(GameBoard.Player.Player2);
        }
    }

    public struct CapTileChecker
    {
        public List<GameBoard.Tile> tileStack;
        public bool isCaptured;

        public CapTileChecker(List<GameBoard.Tile> tileStack, bool isCaptured)
        {
            this.tileStack = tileStack;
            this.isCaptured = isCaptured;
        }
    }

    public struct minimaxBoard
    {
        public GameBoard board;
        public float score;

        public minimaxBoard(GameBoard b, float s)
        {
            board = b;
            score = s;
        }
    }
    
    private List<GameBoard.Coordinate> branchIndexes = new List<GameBoard.Coordinate>
    {
        new GameBoard.Coordinate{x = 0, y = 5},
        new GameBoard.Coordinate{x = 1, y = 4},
        new GameBoard.Coordinate{x = 1, y = 6},
        new GameBoard.Coordinate{x = 2, y = 3},
        new GameBoard.Coordinate{x = 2, y = 5},
        new GameBoard.Coordinate{x = 2, y = 7},
        new GameBoard.Coordinate{x = 3, y = 2},
        new GameBoard.Coordinate{x = 3, y = 4},
        new GameBoard.Coordinate{x = 3, y = 6},
        new GameBoard.Coordinate{x = 3, y = 8},
        new GameBoard.Coordinate{x = 4, y = 1},
        new GameBoard.Coordinate{x = 4, y = 3},
        new GameBoard.Coordinate{x = 4, y = 5},
        new GameBoard.Coordinate{x = 4, y = 7},
        new GameBoard.Coordinate{x = 4, y = 9},
        new GameBoard.Coordinate{x = 5, y = 0},
        new GameBoard.Coordinate{x = 5, y = 2},
        new GameBoard.Coordinate{x = 5, y = 4},
        new GameBoard.Coordinate{x = 5, y = 6},
        new GameBoard.Coordinate{x = 5, y = 8},
        new GameBoard.Coordinate{x = 5, y = 10},
        new GameBoard.Coordinate{x = 6, y = 1},
        new GameBoard.Coordinate{x = 6, y = 3},
        new GameBoard.Coordinate{x = 6, y = 5},
        new GameBoard.Coordinate{x = 6, y = 7},
        new GameBoard.Coordinate{x = 6, y = 9},
        new GameBoard.Coordinate{x = 7, y = 2},
        new GameBoard.Coordinate{x = 7, y = 4},
        new GameBoard.Coordinate{x = 7, y = 6},
        new GameBoard.Coordinate{x = 7, y = 8},
        new GameBoard.Coordinate{x = 8, y = 3},
        new GameBoard.Coordinate{x = 8, y = 5},
        new GameBoard.Coordinate{x = 8, y = 7},
        new GameBoard.Coordinate{x = 9, y = 4},
        new GameBoard.Coordinate{x = 9, y = 6},
        new GameBoard.Coordinate{x = 10, y = 5}
    };

    private List<GameBoard.Coordinate> nodeIndexes = new List<GameBoard.Coordinate>
    {
        new GameBoard.Coordinate{x = 0, y = 4},
        new GameBoard.Coordinate{x = 0, y = 6},
        new GameBoard.Coordinate{x = 2, y = 2},
        new GameBoard.Coordinate{x = 2, y = 4},
        new GameBoard.Coordinate{x = 2, y = 6},
        new GameBoard.Coordinate{x = 2, y = 8},
        new GameBoard.Coordinate{x = 4, y = 0},
        new GameBoard.Coordinate{x = 4, y = 2},
        new GameBoard.Coordinate{x = 4, y = 4},
        new GameBoard.Coordinate{x = 4, y = 6},
        new GameBoard.Coordinate{x = 4, y = 8},
        new GameBoard.Coordinate{x = 4, y = 10},
        new GameBoard.Coordinate{x = 6, y = 0},
        new GameBoard.Coordinate{x = 6, y = 2},
        new GameBoard.Coordinate{x = 6, y = 4},
        new GameBoard.Coordinate{x = 6, y = 6},
        new GameBoard.Coordinate{x = 6, y = 8},
        new GameBoard.Coordinate{x = 6, y = 10},
        new GameBoard.Coordinate{x = 8, y = 2},
        new GameBoard.Coordinate{x = 8, y = 4},
        new GameBoard.Coordinate{x = 8, y = 6},
        new GameBoard.Coordinate{x = 8, y = 8},
        //Matthew was here
        new GameBoard.Coordinate{x = 10, y = 4},
        new GameBoard.Coordinate{x = 10, y = 6}
    };
    
    private List<GameBoard.Coordinate> tileIndexes = new List<GameBoard.Coordinate>
    {
        new GameBoard.Coordinate{x = 1, y = 5},
        new GameBoard.Coordinate{x = 3, y = 3},
        new GameBoard.Coordinate{x = 3, y = 5},
        new GameBoard.Coordinate{x = 3, y = 7},
        new GameBoard.Coordinate{x = 5, y = 1},
        new GameBoard.Coordinate{x = 5, y = 3},
        new GameBoard.Coordinate{x = 5, y = 5},
        new GameBoard.Coordinate{x = 5, y = 7},
        new GameBoard.Coordinate{x = 5, y = 9},
        new GameBoard.Coordinate{x = 7, y = 3},
        new GameBoard.Coordinate{x = 7, y = 5},
        new GameBoard.Coordinate{x = 7, y = 7},
        new GameBoard.Coordinate{x = 9, y = 5}
    };

    public List<GameBoard.Coordinate> copyBranchCoords(List<GameBoard.Coordinate> listToCopy)
    {
        List<GameBoard.Coordinate> copyList = new List<GameBoard.Coordinate>();
        foreach (GameBoard.Coordinate c in listToCopy)
        {
            copyList.Add(new GameBoard.Coordinate {x = c.x, y = c.y});
        }
        return copyList;
    }

    public List<GameBoard.Coordinate> copyBranchCoords(List<GameBoard.Coordinate> listToCopy, GameBoard.Coordinate coordToIgnore)
    {
        List<GameBoard.Coordinate> copyList = new List<GameBoard.Coordinate>();
        foreach (GameBoard.Coordinate c in listToCopy)
        {
            if(c.x != coordToIgnore.x && c.y != coordToIgnore.y)
            {
                copyList.Add(new GameBoard.Coordinate{x = c.x, y = c.y});
            }
        }
        return copyList;
    }

    public List<GameBoard.Coordinate> copyNodeCoords(List<GameBoard.Coordinate> listToCopy)
    {
        List<GameBoard.Coordinate> copyList = new List<GameBoard.Coordinate>();
        foreach (GameBoard.Coordinate c in listToCopy)
        {
            copyList.Add(new GameBoard.Coordinate{x = c.x, y = c.y});
        }
        return copyList;
    }

    public List<GameBoard.Coordinate> copyNodeCoords(List<GameBoard.Coordinate> listToCopy, GameBoard.Coordinate coordToIgnore)
    {
        List<GameBoard.Coordinate> copyList = new List<GameBoard.Coordinate>();
        foreach (GameBoard.Coordinate c in listToCopy)
        {
            if(c.x != coordToIgnore.x && c.y != coordToIgnore.y)
            {
                copyList.Add(new GameBoard.Coordinate{x = c.x, y = c.y});
            }
        }
        return copyList;
    }

    public List<GameBoard> getPossibleMoves(GameBoard gBoard)
    {
        int[] resourcePool = new int[]{-1, -1, -1, -1};
        if(gBoard.getCurrentPlayer() == GameBoard.Player.Player1)
        {
            resourcePool[0] = gBoard.getResources(GameBoard.Player.Player1)[0];
            resourcePool[1] = gBoard.getResources(GameBoard.Player.Player1)[1];
            resourcePool[2] = gBoard.getResources(GameBoard.Player.Player1)[2];
            resourcePool[3] = gBoard.getResources(GameBoard.Player.Player1)[3];
        }
        else
        {
            resourcePool[0] = gBoard.getResources(GameBoard.Player.Player2)[0];
            resourcePool[1] = gBoard.getResources(GameBoard.Player.Player2)[1];
            resourcePool[2] = gBoard.getResources(GameBoard.Player.Player2)[2];
            resourcePool[3] = gBoard.getResources(GameBoard.Player.Player2)[3];
        }
        //reduce the resources
        resourcePool = reduceResources(resourcePool);

        //see if there is a single resource you can completely trade. If so, only search possible trades in that reality
        bool oneLargeResource = false;
        int largestLargeResource = 0;
        for (int i = 0; i < 4; i++)
        {
            if(resourcePool[i] - 3 >= 1 && resourcePool[i] - 3 > largestLargeResource)
            {
                largestLargeResource = i;
                oneLargeResource = true;
            }
        }
        if(oneLargeResource)
        {
            for (int i = 0; i < 4; i++)
            {
                if(resourcePool[i] != largestLargeResource)
                {
                    resourcePool[i] = 0;
                }
            }
        }
        List<GameBoard> allTradedBoards = getPossibleTrades(gBoard, resourcePool);
        List<GameBoard> allBranchesAndNodes = new List<GameBoard>();
        List<GameBoard> allPossibleOptions = new List<GameBoard>();
        List<GameBoard.Coordinate> branchCoords = copyBranchCoords(branchIndexes);
        List<GameBoard.Coordinate> nodeCoords = copyNodeCoords(nodeIndexes);
        
        //First check for trades, splitting the options into newPieceTrades, and noPieceTrades
        List<int[]> newPieceTrades = new List<int[]>();
        List<int[]> noPieceTrades = new List<int[]>();
        List<int[]> possibleTrades = getPossibleTradeLists(gBoard, resourcePool);
        foreach (int[] possibleTrade in possibleTrades)
        {
            int[] newResourcePool = new int[]{resourcePool[0] + possibleTrade[0], resourcePool[1] + possibleTrade[1], resourcePool[2] + possibleTrade[2], resourcePool[3] + possibleTrade[3]};
            if ((newResourcePool[0] < 1 || newResourcePool[1] < 1) && (newResourcePool[2] < 2 || newResourcePool[3] < 2))
            {
                //this trade does not impact building this turn. Add it to noPieceTrades
                noPieceTrades.Add(possibleTrade);
            }
            else if (newResourcePool[0] > 0 && newResourcePool[1] > 0)
            {
                //this trade gained a new piece. Add it to newPieceTrades
                newPieceTrades.Add(possibleTrade);
            }
        }

        //start with the newPieceTrades. if there are any, apply them, and then get possible branches and nodes
        if(newPieceTrades.Count != 0)
        {
            List<GameBoard> tradedNewPieceBoards = appliedTrades(gBoard, newPieceTrades);
            List<GameBoard> newPieceBranches = new List<GameBoard>();
            foreach (GameBoard g in tradedNewPieceBoards)
            {
                List<GameBoard> possibleBranches = getPossibleBranches(g, branchCoords, false);
                if (gBoard.getSetupCounter() > 4 && possibleBranches.Count == 0)
                {
                    possibleBranches.Add(new GameBoard(g));
                }
                foreach (GameBoard branchBoard in possibleBranches)
                {
                    newPieceBranches.Add(branchBoard);
                }
            }
            foreach (GameBoard g in newPieceBranches)
            {
                List<GameBoard> possibleNodes = getPossibleNodes(g, nodeCoords, false);
                if (gBoard.getSetupCounter() > 4 && possibleNodes.Count == 0)
                {
                    possibleNodes.Add(new GameBoard(g));
                }
                foreach (GameBoard nodeBoard in possibleNodes)
                {
                    allPossibleOptions.Add(nodeBoard);
                }
            }
        }

        List<GameBoard> noPieceBranches = getPossibleBranches(gBoard, branchCoords, false);
        List<GameBoard> noPieceBranchesAndNodes = new List<GameBoard>();
        if (gBoard.getSetupCounter() > 4 && noPieceBranches.Count == 0)
        {
            noPieceBranches.Add(new GameBoard(gBoard));
        }
        foreach (GameBoard g in noPieceBranches)
        {
            List<GameBoard> possibleNodes = getPossibleNodes(g, nodeCoords, false);
            if (gBoard.getSetupCounter() > 4 && possibleNodes.Count == 0)
            {
                possibleNodes.Add(new GameBoard(g));
            }
            foreach (GameBoard nodeBoard in possibleNodes)
            {
                noPieceBranchesAndNodes.Add(nodeBoard);
            }
        }
        List<GameBoard> noPieceOptions = appliedTrades(noPieceBranchesAndNodes, noPieceTrades);
        foreach(GameBoard g in noPieceOptions)
        {
            allPossibleOptions.Add(g);
        }
        Debug.Log("All possible moves: " + allPossibleOptions.Count);
        return allPossibleOptions;
    }

    int[] reduceResources(int[] resources)
    {
        int[]reducedResources = new int[]{-1, -1, -1, -1};
        //first reduce branch resources
        int reducedBranchResources = resources[0] - resources[1];
        if(reducedBranchResources == 0)
        {
            reducedResources[0] = 0;
            reducedResources[1] = 0;
        }
        else if (reducedBranchResources < 0)
        {
            //blue was bigger than red
            reducedResources[0] = 0;
            reducedResources[1] = reducedBranchResources * -1;
        }
        else
        {
            //red was bigger than blue
            reducedResources[0] = reducedBranchResources;
            reducedResources[1] = 0;
        }

        //now reduce the node resources
        reducedResources[2] = resources[2];
        reducedResources[3] = resources[3];
        while(reducedResources[2] > 1 && reducedResources[3] > 1)
        {
            reducedResources[2] -= 2;
            reducedResources[3] -= 2;
        }
        Debug.Log("Reduced Resources: " + reducedResources[0] + " " + reducedResources[1] + " " + reducedResources[2] + " " + reducedResources[3]);
        return reducedResources;
    }

    private bool isTradable(int[] moveCost, int[] resourcePool)
    {
        for(int i = 0; i < resourcePool.Length; ++i)
        {
            if(moveCost[i] + resourcePool[i] < 0)
            {
                return false;
            }
        }
        return true;
    }

    List<int[]> getPossibleTradeLists(GameBoard gBoard, int[] resourcePool)
    {

        //TODO: Add stipulations for possible trades 
        List<int[]> possibleTrades = new List<int[]>();
        List<GameBoard> tradedBoards = new List<GameBoard>();
        possibleTrades.Add(new int[]{0, 0, 0, 0});
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
                    if(isTradable(testTrade, resourcePool))
                    {
                        possibleTrades.Add(testTrade);
                    }
                }
            }
        }
        return possibleTrades;
    }

    List<GameBoard> appliedTrades(GameBoard board, List<int[]> tradesToBeMade)
    {
        List<GameBoard> tradedBoards = new List<GameBoard>();
        foreach (int[] trade in tradesToBeMade)
        {
            GameBoard g = new GameBoard(board);
            g.makeTrade(trade);
            tradedBoards.Add(g);
        }
        return tradedBoards;
    }

    List<GameBoard> appliedTrades(List<GameBoard> boards, List<int[]> tradesToBeMade)
    {
        List<GameBoard> tradedBoards = new List<GameBoard>();
        foreach (GameBoard board in boards)
        {
            foreach (int[] trade in tradesToBeMade)
            {
                GameBoard g = new GameBoard(board);
                g.makeTrade(trade);
                tradedBoards.Add(g);
            }
        }
        return tradedBoards;
    }

    List<GameBoard> getPossibleTrades(GameBoard gBoard, int[] resourcePool)
    {

        //TODO: Add stipulations for possible trades 
        List<int[]> possibleTrades = new List<int[]>();
        List<GameBoard> tradedBoards = new List<GameBoard>();
        possibleTrades.Add(new int[]{0, 0, 0, 0});
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
                    if(isTradable(testTrade, resourcePool))
                    {
                        possibleTrades.Add(testTrade);
                    }
                }
            }
        }
        foreach (int[] trade in possibleTrades)
        {
            GameBoard g = new GameBoard(gBoard);
            g.makeTrade(trade);
            tradedBoards.Add(g);
        }
        return tradedBoards;
    }

    List<GameBoard> getPossibleNodes(GameBoard gBoard, List<GameBoard.Coordinate> unvisitedCoords, bool haveRanOnce)
    {
        if (haveRanOnce)
        {
            //the function has gotten past the initial recursive call, therefore univsitedCoords is our list of possible coords
            //this means that there is no need to check if it is possible again
            List<GameBoard> possibleNodes = new List<GameBoard>();
            List<GameBoard.Coordinate> abridgedNodeCoords = new List<GameBoard.Coordinate>();
            if(unvisitedCoords.Count != 0)
            {
                foreach (GameBoard.Coordinate coord in unvisitedCoords)
                {
                    abridgedNodeCoords = copyNodeCoords(unvisitedCoords, coord);
                    GameBoard g = new GameBoard(gBoard);
                    g.placePiece(coord);
                    possibleNodes.Add(g);
                    possibleNodes = possibleNodes.Concat(getPossibleNodes(g, abridgedNodeCoords, true)).ToList();
                }
            }
            return possibleNodes;
        }
        else
        {
            List<GameBoard.Coordinate> possibleNodeCoords = new List<GameBoard.Coordinate>();
            List<GameBoard> possibleNodes = new List<GameBoard>();
            if(unvisitedCoords.Count != 0)
            {
                foreach (GameBoard.Coordinate coord in unvisitedCoords)
                {
                    GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = coord.x, y = coord.y};
                    if(gBoard.isValidMove(coord))
                    {
                        possibleNodeCoords.Add(testMove);
                    }
                }

                List<GameBoard.Coordinate> abridgedNodeCoords = copyNodeCoords(possibleNodeCoords);
                foreach (GameBoard.Coordinate coord in possibleNodeCoords)
                {
                    abridgedNodeCoords = copyNodeCoords(abridgedNodeCoords, coord);
                    GameBoard g = new GameBoard(gBoard);
                    g.placePiece(coord);
                    possibleNodes.Add(g);
                    possibleNodes = possibleNodes.Concat(getPossibleNodes(g, abridgedNodeCoords, true)).ToList();
                }
            }
            return possibleNodes;
        }
    }
    
    List<GameBoard> getPossibleBranches(GameBoard gBoard, List<GameBoard.Coordinate> unvisitedCoords, bool haveRanOnce)
    {
        List<GameBoard.Coordinate> possibleInitialBranchCoords = new List<GameBoard.Coordinate>();
        List<GameBoard> possibleBranches = new List<GameBoard>();
        if (unvisitedCoords.Count != 0)
        {
            foreach (GameBoard.Coordinate coord in unvisitedCoords)
            {
                GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = coord.x, y = coord.y};
                if(gBoard.isValidMove(coord))
                {
                    possibleInitialBranchCoords.Add(testMove);
                }
            }

            List<GameBoard.Coordinate> abridgedBranchCoords = copyBranchCoords(possibleInitialBranchCoords);
            foreach (GameBoard.Coordinate coord in possibleInitialBranchCoords)
            {
                abridgedBranchCoords = copyBranchCoords(abridgedBranchCoords, coord);
                GameBoard g = new GameBoard(gBoard);
                g.placePiece(coord);
                possibleBranches.Add(g);
                abridgedBranchCoords = addNewCoordinates(g, coord, abridgedBranchCoords);
                possibleBranches = possibleBranches.Concat(getPossibleBranches(g, abridgedBranchCoords, true)).ToList();
            }
        }
        return possibleBranches;
    }

    List<GameBoard.Coordinate> addNewCoordinates(GameBoard g, GameBoard.Coordinate newCoord, List<GameBoard.Coordinate> abridgedCoords)
    {
        
        List<GameBoard.Coordinate> potentialNewbies = new List<GameBoard.Coordinate>();
        if (g.isHorizontalBranch(newCoord))
        {
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x - 1, y = newCoord.y - 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x + 1, y = newCoord.y - 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x, y = newCoord.y - 2});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x - 1, y = newCoord.y + 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x + 1, y = newCoord.y + 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x, y = newCoord.y + 2});
        }
        else if (g.isVerticalBranch(newCoord))
        {
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x - 1, y = newCoord.y - 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x - 1, y = newCoord.y + 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x - 2, y = newCoord.y});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x + 1, y = newCoord.y - 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x + 1, y = newCoord.y + 1});
            potentialNewbies.Add(new GameBoard.Coordinate{x = newCoord.x + 2, y = newCoord.y});
        }
        foreach (GameBoard.Coordinate c in potentialNewbies)
        {
            bool included = false;
            foreach (GameBoard.Coordinate ac in abridgedCoords)
            {
                if (c.x == ac.x && c.y == ac.y ||
                    (c.x >= 0 && c.x < 11) ||
                    (c.y >= 0 && c.y < 11))
                {
                    included = true;
                }
            }
            if(!included)
            {
                if (g.isValidMove(c))
                {
                    abridgedCoords.Add(c);
                }
            }
        }
        return abridgedCoords;
    }

    List<GameBoard.Coordinate> tilesTouched(GameBoard board, GameBoard.Player player)
    {
        List<GameBoard.Coordinate> tilesTouched = new List<GameBoard.Coordinate>();
        foreach (GameBoard.Coordinate nodeCoord in nodeIndexes)
        {
            if(pieceAtCoordinateIsOwnedByPlayer(board, nodeCoord, player))
            {
                List<GameBoard.Coordinate> tiles = new List<GameBoard.Coordinate> {
                    new GameBoard.Coordinate{x = nodeCoord.x + 1, y = nodeCoord.y + 1},
                    new GameBoard.Coordinate{x = nodeCoord.x + 1, y = nodeCoord.y - 1},
                    new GameBoard.Coordinate{x = nodeCoord.x - 1, y = nodeCoord.y + 1},
                    new GameBoard.Coordinate{x = nodeCoord.x - 1, y = nodeCoord.y - 1}
                };
                foreach (GameBoard.Coordinate tileCoord in tiles)
                {
                    if(isInBounds(board, tileCoord) && !tilesTouched.Contains(tileCoord))
                    {
                        tilesTouched.Add(tileCoord);
                    }
                }
            }
        }
        return tilesTouched;
    }

    int helpfulTileTouchCount(GameBoard board, GameBoard.Player player)
    {
        int httc = 0;
        List<GameBoard.Coordinate> touchedTiles = tilesTouched(board, player);
        foreach (GameBoard.Coordinate tileCoord in touchedTiles)
        {
            if(!board.overloadedTiles().Contains(board.gameBoard[tileCoord.x, tileCoord.y]) &&
            ((GameBoard.Tile)board.gameBoard[tileCoord.x, tileCoord.y]).resourceType != GameBoard.ResourceType.None &&
            (board.gameBoard[tileCoord.x, tileCoord.y].player == GameBoard.Player.None || board.gameBoard[tileCoord.x, tileCoord.y].player == player))
            {
                httc++;
            }
        }
        return httc;
    }

    int touhedTileStability(GameBoard board, GameBoard.Player player)
    {
        int tts = 0;
        List<GameBoard.Coordinate> touchedTiles = tilesTouched(board, player);


        return tts;
    }

    public minimaxBoard greedyFreederick(GameBoard position)
    {
        minimaxBoard hvalue = new minimaxBoard(position, Mathf.NegativeInfinity);
        List<GameBoard> boards = getPossibleMoves(position);
        List<minimaxBoard> legalMoves = new List<minimaxBoard>();
        foreach (GameBoard b in boards)
            {
                legalMoves.Add(new minimaxBoard(b, 0));
            }
        foreach (minimaxBoard child in legalMoves)
        {
            GameBoard endedBoard = new GameBoard(child.board);
            endedBoard.endTurn();
            float challenger = applyHeuristic(endedBoard);
            //Debug.Log("challenger score is: " + challenger);
            if(challenger > hvalue.score)
            {
                Debug.Log("challenger " + challenger + " beat score " + hvalue.score);
                hvalue.board = child.board;
                hvalue.score = challenger;
            }
        }
        return hvalue;
    }


    public float applyHeuristic(GameBoard board)
    {
        switch(strat)
        {
            case 1:
            {
                return standardHeuristic(board);
            }
            case 2:
            {
                return roadStrat(board);
            }
            case 3:
            {
                return nukeStrat(board);
            }
            case 4:
            {
                return maddening(board);
            }
        }
        return standardHeuristic(board);
    }

    public float standardHeuristic(GameBoard board)
    {
        float heuristicResult = 1;
        if(board.getScore(opponent) >= 10)
        {
            heuristicResult = LOSE;
        }
        else if (board.getScore(self) >= 10)
        {
            heuristicResult = WIN;
        }
        else
        {
            heuristicResult = (board.getScore(self) - board.getScore(opponent)) + (branches(board, self) - branches(board, opponent)) + (resourcePotential(board, self) - resourcePotential(board, opponent));
        }
        return heuristicResult;
    }

    public float roadStrat(GameBoard board)
    {
        float heuristicResult = 1;
        if(board.getScore(opponent) >= 10)
        {
            heuristicResult = LOSE;
        }
        else if (board.getScore(self) >= 10)
        {
            heuristicResult = WIN;
        }
        else
        {
            heuristicResult = (board.getScore(self) - board.getScore(opponent)) + 5 * (branches(board, self) - branches(board, opponent)) + (resourcePotential(board, self) - resourcePotential(board, opponent));
        }
        return heuristicResult;
    }

    public float nukeStrat(GameBoard board)
    {
        float heuristicResult = 1;
        if(board.getScore(opponent) >= 10)
        {
            heuristicResult = LOSE;
        }
        else if (board.getScore(self) >= 10)
        {
            heuristicResult = WIN;
        }
        else
        {
            heuristicResult = (board.getScore(self) - board.getScore(opponent)) + (branches(board, self) - branches(board, opponent)) + 5 * (resourcePotential(board, self) - resourcePotential(board, opponent));
        }
        return heuristicResult;
    }

    public float maddening(GameBoard board)
    {
        float heuristicResult = 1;
        if(board.getScore(opponent) >= 10)
        {
            heuristicResult = LOSE;
        }
        else if (board.getScore(self) >= 10)
        {
            heuristicResult = WIN;
        }
        else
        {
            heuristicResult = hw[0] * (board.getScore(self) - board.getScore(opponent)) + hw[1] * (branches(board, self) - branches(board, opponent)) + hw[2] * (resourcePotential(board, self) - resourcePotential(board, opponent));
        }
        return heuristicResult;
    }

    private float branches(GameBoard board, GameBoard.Player p)
    {
        float branchesValue = 0;
        GameBoard.Coordinate current = new GameBoard.Coordinate{x = 0, y = 0};

        //counts total branches for each player and gets a coordinate to start spanning from each player
        for(int row = 0; row < GameBoard.boardSize; ++row)
        {
            for(int col = 0; col < GameBoard.boardSize; ++col)
            {
                current.x = row;
                current.y = col;

                if(isInBounds(board, current) && board.gameBoard[row,col].pieceType == GameBoard.PieceType.Branch)
                {
                    if(pieceAtCoordinateIsOwnedByPlayer(board, current, p))
                    {
                        switch(Mathf.Abs(5 - current.x) + Mathf.Abs(5 - current.y))
                        {
                            case 1:
                                branchesValue += hw[3];
                                break;
                            case 3:
                                branchesValue += hw[4];
                                break;
                            case 5:
                                branchesValue += hw[5];
                                break;
                            case 7:
                                branchesValue += hw[6];
                                break;
                            case 9:
                                branchesValue += hw[7];
                                break;
                        }
                    }
                }
            }
        }
        return branchesValue;
    }

    private bool pieceAtCoordinateIsOwnedByPlayer(GameBoard b, GameBoard.Coordinate c, GameBoard.Player p)
    {
        if(isInBounds(b, c) && b.gameBoard[c.x,c.y].player == p)
        {
            return true;
        }
        return false;
    }

    private bool isInBounds(GameBoard board, GameBoard.Coordinate c)
    {
        if(c.x < 11 && c.x >= 0 && c.y >= 0 && c.y < 11 && board.gameBoard[c.x, c.y] != null)
        {
            return true;
        }
        return false;
    }

    private int[] potentialResources(GameBoard b, GameBoard.Player p)
    {
        int numNodesAroundTile = 0;
        int numPlayerNodesForTile = 0;
        int[] incomingResources = new int[4];

        //goes though each tile and distributes resources to the player
        for(int i = 0; i < tileIndexes.Count; ++i)
        {
            //top left
            if(!pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y - 1}, GameBoard.Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y - 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //top right
            if(!pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y + 1}, GameBoard.Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y + 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //bottom right
            if(!pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y - 1}, GameBoard.Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y - 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //bottom left
            if(!pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y + 1}, GameBoard.Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(b, new GameBoard.Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y + 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            if((pieceAtCoordinateIsOwnedByPlayer(b, tileIndexes[i], GameBoard.Player.None)
                || pieceAtCoordinateIsOwnedByPlayer(b, tileIndexes[i], p))
                && ((GameBoard.Tile)b.gameBoard[tileIndexes[i].x, tileIndexes[i].y]).resourceType != GameBoard.ResourceType.None)
            {
                if(numNodesAroundTile <= ((GameBoard.Tile)b.gameBoard[tileIndexes[i].x, tileIndexes[i].y]).maxLoad || pieceAtCoordinateIsOwnedByPlayer(b, tileIndexes[i], p))
                {
                    incomingResources[(int)((GameBoard.Tile)b.gameBoard[tileIndexes[i].x, tileIndexes[i].y]).resourceType] += numPlayerNodesForTile;
                }
            }
            numNodesAroundTile = 0;
            numPlayerNodesForTile = 0;
        }
        return incomingResources;
    }

    public float resourcePotential(GameBoard board, GameBoard.Player player)
    {
        int[] incomingResources = potentialResources(board, player);
        int noResources = 0;
        foreach (int r in incomingResources)
        {
            if (r == 0)
            {
                noResources++;
            }
        }
        float resourcePotential = hw[8] * incomingResources[0] + hw[9] * incomingResources[1] + hw[10] * incomingResources[2] + hw[11] * incomingResources[3] - hw[12] * (3 * noResources);
        return resourcePotential;
    }

    public void pickStrat(bool isMaddening)
    {
        if(isMaddening)
        {
            strat = 4;
        }
        else
        {
            float rnd = Random.Range(0.0f, 3.0f);
            strat = (int)(Floor(rnd));
        }
    }

    public AI(GameBoard.Player o, GameBoard firstBoard, bool isMaddening)
    {
        AIGameBoard = new GameBoard(firstBoard);
        opponent = o;
        pickStrat(isMaddening);
        hw = new float[]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        /*hw = new float[]{
            0.1068829f,
            -0.6683943f,
            0.7714462f,
            0.8588469f,
            -0.4448601f,
            0.6133263f,
            -0.8766624f,
            -0.6385219f,
            -0.7896829f,
            -0.2846443f,
            -0.4512476f,
            0.4773977f,
            0.3198857f
        };*/
        if (o == GameBoard.Player.Player1)
        {
            self = GameBoard.Player.Player2;
        }
        else
        {
            self = GameBoard.Player.Player1;
        }
    }

    public GameBoard makeMove(GameBoard gBoard)
    {
        System.DateTime time = System.DateTime.Now;
        minimaxBoard chosenBoard = greedyFreederick(gBoard);
        Debug.Log("Selection of move from Freederick took " + (System.DateTime.Now - time));
        Debug.Log("Value of the chosen position: " + chosenBoard.score);
        return chosenBoard.board;
    }


    public void setCapturedTiles(List<GameBoard.Tile> noncapturedTiles, GameBoard.Player player)
    {
        while (noncapturedTiles.Any())
        {
            //make sure that the tile is not owned. If it is owned, there is no need to search it.
            if (noncapturedTiles[0].player == GameBoard.Player.None && !noncapturedTiles[0].quartered)
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
                        }
                    }
                } else
                {
                    foreach (GameBoard.Tile tile in checkResults.tileStack)
                    {
                        if(noncapturedTiles.Contains(tile))
                        {
                            noncapturedTiles.Remove(tile);
                        }
                    }
                }
            }
            else
            {
                noncapturedTiles.Remove(noncapturedTiles[0]);
            }
        }
    }

    public CapTileChecker checkIfCaptured(GameBoard board, GameBoard.Tile currentTile, CapTileChecker checkedTiles, GameBoard.Player player)
    {
        //Debug.Log("Now checking Tile: " + currentTile.coord.x + " - " + currentTile.coord.y);
        //first Check for any insta-fails on the surrounding branches/tiles
        if (currentTile.quartered)
        {
            //the tile is dead, and impossible to capture. Mission failed.
            if(!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles;
        }
        else if (isOpponents(board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y], player) || isOpponents(board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y], player) ||
                isOpponents(board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1], player) || isOpponents(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1], player))
        {
            //Opponent branch found. Mission failed.
            //Debug.Log("Opponent Branch found around Tile " + currentTile.coord.x + " - " + currentTile.coord.y);
            if (!checkedTiles.tileStack.Contains(currentTile))
            {
                checkedTiles.tileStack.Add(currentTile);
            }
            checkedTiles.isCaptured = false;
            return checkedTiles;
        }
        else if ((board.gameBoard[currentTile.coord.x - 1, currentTile.coord.y].player == GameBoard.Player.None && 
                (!inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x - 2, y = currentTile.coord.y}) || isOpponents(board.gameBoard[currentTile.coord.x - 2, currentTile.coord.y], player))) ||
                (board.gameBoard[currentTile.coord.x + 1, currentTile.coord.y].player == GameBoard.Player.None && 
                (!inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x + 2, y = currentTile.coord.y}) || isOpponents(board.gameBoard[currentTile.coord.x + 2, currentTile.coord.y], player))) ||
                (board.gameBoard[currentTile.coord.x, currentTile.coord.y - 1].player == GameBoard.Player.None &&
                (!inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x, y = currentTile.coord.y - 2}) || isOpponents(board.gameBoard[currentTile.coord.x, currentTile.coord.y - 2], player))) ||
                (board.gameBoard[currentTile.coord.x, currentTile.coord.y + 1].player == GameBoard.Player.None &&
                (!inBoundsTile(board, new GameBoard.Coordinate{x = currentTile.coord.x, y = currentTile.coord.y + 2}) || isOpponents(board.gameBoard[currentTile.coord.x, currentTile.coord.y + 2], player))))
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
    bool isOpponents (GameBoard.GamePiece piece, GameBoard.Player player)
    {
        if (piece.player == player || piece.player == GameBoard.Player.None)
        {
            return false;
        }
        else
        {
            return true;
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



    // Start is called before the first frame update
    void Start()
    {
        
    }
}
