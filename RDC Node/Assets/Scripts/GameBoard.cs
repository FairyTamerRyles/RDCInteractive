using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class GameBoard
{

    private float rnd;
    private const int boardSize = 11;
    private const int numResources = 4;

    private bool tradeMadeThisTurn = false;

    private int[] player1Resources;
    private int[] player2Resources;

    private GamePiece[,] gameBoard;

    private Player currentPlayer;

    private List<Move> moveQueue;

    public enum Player
    {
        None = 0,
        Player1 = 1,
        Player2 = 2
    }

    public enum PieceType
    {
        Tile = 0,
        Branch = 1,
        Node = 2
    }

    public enum ResourceType
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        None = 4
    }

    public enum MoveType
    {
        Trade = 1,
        PlaceNode = 2,
        PlaceBranch = 3,
        StartMove = 4
    }

    public struct Coordinate
    {
        public int x;
        public int y;
    }

    public class GamePiece
    {
        public Coordinate coord;
        public Player player;
        public PieceType pieceType;
        //public GamePiece[] adjacentPieces = new GamePiece[8];

        public GamePiece(Coordinate c, PieceType pt)
        {
            player = Player.None;
            coord = c;
            pieceType = pt;
        }
    }

    public class Tile : GamePiece
    {
        public ResourceType ResourceType;
        public int maxLoad;

        public Tile(ResourceType r, int max): base(new Coordinate() {x = 0, y = 0}, PieceType.Tile)
        {
            ResourceType = r;
            maxLoad = max;
            player = Player.None;
        }
    }

    public class Move
    {
        public int[] resourceChange;
        public Player player;
        public Coordinate coord;
        public MoveType moveType;

        public Move(int[] rChange, Player p, Coordinate c, MoveType m)
        {
            resourceChange = rChange;
            player = p;
            coord = c;
            moveType = m;
        }
    }

    //TODO: Try to make this const later
    private List<Coordinate> tileIndexes = new List<Coordinate> 
    {
        new Coordinate{x = 1, y = 5}, 
        new Coordinate{x = 3, y = 3}, 
        new Coordinate{x = 3, y = 5}, 
        new Coordinate{x = 3, y = 7}, 
        new Coordinate{x = 5, y = 1}, 
        new Coordinate{x = 5, y = 3}, 
        new Coordinate{x = 5, y = 5}, 
        new Coordinate{x = 5, y = 7}, 
        new Coordinate{x = 5, y = 9}, 
        new Coordinate{x = 7, y = 3}, 
        new Coordinate{x = 7, y = 5}, 
        new Coordinate{x = 7, y = 7}, 
        new Coordinate{x = 9, y = 5}
    };

    public List<Tile> GameTiles = new List<Tile> 
    {
        new Tile(ResourceType.Red, 1), 
        new Tile(ResourceType.Red, 2), 
        new Tile(ResourceType.Red, 3),
        new Tile(ResourceType.Blue, 1),
        new Tile(ResourceType.Blue, 2),
        new Tile(ResourceType.Blue, 3),
        new Tile(ResourceType.Green, 1),
        new Tile(ResourceType.Green, 2),
        new Tile(ResourceType.Green, 3),
        new Tile(ResourceType.Yellow, 1),
        new Tile(ResourceType.Yellow, 2),
        new Tile(ResourceType.Yellow, 3),
        new Tile(ResourceType.None, -1)
    };

    public GameBoard()
    {
        gameBoard = generateBoard();
        player1Resources = new int[4] {0, 0, 0, 0};
        player2Resources = new int[4] {0, 0, 0, 0};
        currentPlayer = Player.Player1;
        moveQueue = new List<Move>();
    }

    public int getScore(Player p)
    {
        return numberOfNodes(p) + numberCapturedTiles(p) + longestNetwork(p);
    }

    public int[] getResources(Player p)
    {
        if(p == Player.Player1)
        {
            return player1Resources;
        }
        else if(p == Player.Player2)
        {
            return player2Resources;
        }
        return null;
    }

    public Player getCurrentPlayer()
    {
        return currentPlayer;
    }

    public GamePiece[,] getGameBoard()
    {
        return gameBoard;
    }

    //could find a more efficient way to do this
    public Player playerWithLargestNetwork()
    {
        if(longestNetwork(Player.Player1) == 2)
        {
            return Player.Player1;
        }
        else if(longestNetwork(Player.Player2) == 2)
        {
            return Player.Player2;
        }
        else
        {
            return Player.None;
        }
    }

    public void placeNode(Coordinate coord)
    {
        Move m = new Move(new int[]{0,0,-2,-2}, currentPlayer, coord, MoveType.PlaceNode);
        makeMove(m);
    }

    public void placeBranch(Coordinate coord)
    {
        Move m = new Move(new int[]{-1,-1,0,0}, currentPlayer, coord, MoveType.PlaceBranch);
        makeMove(m);
    }

    public void makeTrade(int[] resourceChange)
    {
        Move m = new Move(resourceChange, currentPlayer, new Coordinate{x = 0, y = 0}, MoveType.Trade);
        makeMove(m);
    }

    private void makeMove(Move m)
    {
        if(isValidMove(m))
        {
            applyResourceChange(m);
            moveQueue.Add(m);
            if(m.moveType != MoveType.Trade)
            {
                gameBoard[m.coord.x, m.coord.y].player = m.player;
            }
            else if(m.moveType == MoveType.Trade && !tradeMadeThisTurn)
            {
                tradeMadeThisTurn = true;
            }
        }
    }

    public void endTurn()
    {
        checkForCapturedTiles();
        moveQueue.Clear();
        tradeMadeThisTurn = false;

        if(checkForWin() != Player.None)
        {
            //TODO: Something in case of a win
        }
        else
        {
            if(currentPlayer == Player.Player1)
            { 
                currentPlayer = Player.Player2;
            }
            else
            {
                 currentPlayer = Player.Player1;
            }
            distributeResources(currentPlayer);
        }
    }

    private void distributeResources(Player p)
    {
        int numNodesAroundTile = 0;
        int numPlayerNodesForTile = 0;

        //goes though each tile and distributes resources to the player
        for(int i = 0; i < tileIndexes.Count; ++i)
        {
            //top left
            if(!pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y - 1}, Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y - 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //top right
            if(!pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y + 1}, Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x - 1, y = tileIndexes[i].y + 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //bottom right
            if(!pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y - 1}, Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y - 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            //bottom left
            if(!pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y + 1}, Player.None))
            {
                numNodesAroundTile++;
                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate { x = tileIndexes[i].x + 1, y = tileIndexes[i].y + 1}, p))
                {
                    numPlayerNodesForTile++;
                }
            }

            if((pieceAtCoordinateIsOwnedByPlayer(tileIndexes[i], Player.None) 
                || pieceAtCoordinateIsOwnedByPlayer(tileIndexes[i], p)) 
                && ((Tile)gameBoard[tileIndexes[i].x, tileIndexes[i].y]).ResourceType != ResourceType.None)
            {
                if(numNodesAroundTile <= ((Tile)gameBoard[tileIndexes[i].x, tileIndexes[i].y]).maxLoad || pieceAtCoordinateIsOwnedByPlayer(tileIndexes[i], p))
                {
                    if(p == Player.Player1)
                    {
                        player1Resources[(int)((Tile)gameBoard[tileIndexes[i].x, tileIndexes[i].y]).ResourceType] += numPlayerNodesForTile;
                    }
                    else
                    {
                         player2Resources[(int)((Tile)gameBoard[tileIndexes[i].x, tileIndexes[i].y]).ResourceType] += numPlayerNodesForTile;
                    }
                }
            }

            numNodesAroundTile = 0;
            numPlayerNodesForTile = 0;
        }
    }

    public Player checkForWin()
    {
        if(getScore(Player.Player1) >= 10)
        {
            return Player.Player1;
        }
        else if (getScore(Player.Player2) >= 10)
        {
            return Player.Player1;
        }
        return Player.None;
    }

    public bool isValidMove(Move m)
    {
        if(m.moveType == MoveType.StartMove)
        {
            //TODO: Have start moves in proper order, can currently be placed on tiles
            if(pieceAtCoordinateIsOwnedByPlayer(m.coord, Player.None))
            {
                return true;
            }
        }
        //checks if move is a trade
        else if(m.moveType == MoveType.Trade)
        {
            //checks if 3 resources are used and one is gained
            int spent = 0, gained = 0;
            for(int i = 0; i < player1Resources.Length; ++i)
            {
                if(m.resourceChange[i] >= 0)
                {
                    gained += m.resourceChange[i];
                }
                else
                {
                    spent += m.resourceChange[i];
                }
            }
            if(gained == 1 && spent == -3)
            {
                //checks if player has enough resources and no trade has been made yet
                if(!tradeMadeThisTurn && playerHasResources(m.resourceChange, m.player))
                {
                    return true;
                }
            }
            return false;
        }
        //checks if move is placing a node
        else if(m.moveType == MoveType.PlaceNode)
        {
            //checks if move is in bounds and if the node/branch is unoccupied
            if(pieceAtCoordinateIsOwnedByPlayer(m.coord, Player.None))
            {
                //checks to ensure nodes are only placed at appropriate coordinates
                if(isNode(m.coord))
                {
                    //checks if there is an adjacent piece owned by the player
                    if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 1, y = m.coord.y}, m.player) 
                        || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 1, y = m.coord.y}, m.player)
                        || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y - 1}, m.player)
                        || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y + 1}, m.player))
                        {
                            //checks if player has necessary resources
                            if(playerHasResources(m.resourceChange, m.player))
                            {
                                //No extra checking required for captured zone. As long as no branches have been illegally placed, 
                                //a node cannot have been placed in a captured zone
                                return true;
                            }
                        }
                }
            }
            return false;
        }
        //Checks if move is placing a branch
        else if(m.moveType == MoveType.PlaceBranch)
        {
            //checks if move is in bounds and if the node/branch is unoccupied
            if(pieceAtCoordinateIsOwnedByPlayer(m.coord, Player.None))
            {
                //checks to ensure branches are only placed on appropriate coordinates
                if(isHorizontalBranch(m.coord) || isVerticalBranch(m.coord))
                {
                    //checks if there is an adjacent branch owned by the player
                    if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 1, y = m.coord.y - 1}, m.player) 
                    || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 1, y = m.coord.y + 1}, m.player)
                    || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 1, y = m.coord.y - 1}, m.player) 
                    || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 1, y = m.coord.y + 1}, m.player)
                    || (isHorizontalBranch(m.coord) 
                            && (pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y + 2}, m.player)
                            || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y - 2}, m.player)))
                    || (isVerticalBranch(m.coord) 
                            && (pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 2, y = m.coord.y}, m.player)
                            || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 2, y = m.coord.y}, m.player))))
                    {
                        //checks if player has appropriate resources - NOTE: This does not currently check if the resources spent for a node or branch is correct
                        if(playerHasResources(m.resourceChange, m.player))
                        {
                            //checks if branch is in an area captured by the opponent
                            if(isHorizontalBranch(m.coord))
                            {
                                //Checks if tile above or below belongs to opponent
                                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 1, y = m.coord.y}, Player.None) 
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x - 1, y = m.coord.y}, m.player)
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 1, y = m.coord.y}, Player.None) 
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x + 1, y = m.coord.y}, m.player))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                //checks if tile left or right belongs to opponent
                                if(pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y - 1}, Player.None) 
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y - 1}, m.player)
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y + 1}, Player.None) 
                                || pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = m.coord.x, y = m.coord.y + 1}, m.player))
                                {
                                    return true;
                                } 
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private int numberOfNodes(Player p)
    {
        int playerNodes = 0;
        for(int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            for(int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                if(gameBoard[row,col] != null && gameBoard[row,col].pieceType == PieceType.Node && gameBoard[row,col].player == p)
                {
                    ++playerNodes;
                }
            }
        }

        return playerNodes;
    }

    private void checkForCapturedTiles()
    {
        //TODO: This function should find and mark captured tiles
    }

    private int numberCapturedTiles(Player p)
    {
        int capturedTiles = 0;
        Coordinate current = new Coordinate{x = 0, y =0};

        for(int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            current.x = row;
            for(int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                current.y = col;
                if(pieceAtCoordinateIsOwnedByPlayer(current, p) && gameBoard[row,col].pieceType == PieceType.Tile)
                {
                    ++capturedTiles;
                }
            }
        }

        return capturedTiles;
    }

    //This function is slightly nonintuitive. It takes a player and returns 0 or 2 based on 
    //the number of points they get related to the longest network
    private int longestNetwork(Player p)
    {
        int p1Branches = 0;
        int p2Branches = 0;
        Player playerWithLargestNetwork = Player.None;

        Coordinate p1StartCoord = new Coordinate {x = 0, y = 0};
        Coordinate p2StartCoord = new Coordinate {x = 0, y = 0};

        Coordinate current = new Coordinate{x = 0, y = 0};

        //counts total branches for each player and gets a coordinate to start spanning from each player
        for(int row = 0; row < boardSize; ++row)
        {
            for(int col = 0; col < boardSize; ++col)
            {
                current.x = row;
                current.y = col;

                if(isInBounds(current) && gameBoard[row,col].pieceType == PieceType.Branch)
                {
                    if(pieceAtCoordinateIsOwnedByPlayer(current, Player.Player1))
                    {
                        p1Branches++;
                        if(p1StartCoord.x == 0 && p1StartCoord.y == 0)
                        {
                            p1StartCoord = new Coordinate {x = row, y = col};
                        }
                    }
                    else if (pieceAtCoordinateIsOwnedByPlayer(current, Player.Player2))
                    {
                        p2Branches++;
                        if(p2StartCoord.x == 0 && p2StartCoord.y == 0)
                        {
                            p2StartCoord = new Coordinate {x = row, y = col};
                        }
                    }
                }
            }
        }

        //For easier algorithm, the nearest node to the branch is used
        if(isHorizontalBranch(p1StartCoord))
        {
            p1StartCoord.y--;
        }
        else
        {
            p1StartCoord.x--;
        }

        if(isHorizontalBranch(p2StartCoord))
        {
            p2StartCoord.y--;
        }
        else
        {
            p2StartCoord.x--;
        }

        int p1LargestNetwork = networkTraverse(p1StartCoord, Player.Player1);
        int p2LargestNetwork = networkTraverse(p2StartCoord, Player.Player2);

        if(p1Branches - p1LargestNetwork > p1LargestNetwork)
        {
            p1LargestNetwork = p1Branches - p1LargestNetwork;
        }

        if(p2Branches - p2LargestNetwork > p2LargestNetwork)
        {
            p2LargestNetwork = p2Branches - p2LargestNetwork;
        }

        if(p1LargestNetwork > p2LargestNetwork)
        {
            playerWithLargestNetwork = Player.Player1;
        }
        else if(p2LargestNetwork > p1LargestNetwork)
        {
            playerWithLargestNetwork = Player.Player2;
        }

        if(p == playerWithLargestNetwork)
        {
            return 2;
        }
        return 0;
    }

    private void applyResourceChange(Move m)
    {
        for(int i = 0; i < numResources; ++i)
        {
            if(m.player == Player.Player1)
            {
                player1Resources[i] += m.resourceChange[i];
            }
            else
            {
                player2Resources[i] += m.resourceChange[i];
            }
        }
    }

    //This function takes a starting node coordinate and traverses through the branch network, counting up the number of branches
    private int networkTraverse(Coordinate startCoord, Player p)
    {
        Stack<Coordinate> coordStack = new Stack<Coordinate>();
        bool [,] breadCrumbs = new bool[boardSize, boardSize];
        Coordinate currentCoord;
        int networkSize = 0;

        for (int i = 0; i < boardSize; ++i)
        {
            for (int j = 0; j < boardSize; ++j)
            {
                breadCrumbs [i,j] = false;
            }
        }

        if(isInBounds(startCoord))
        {
            coordStack.Push(startCoord);
        }

        while(coordStack.Count != 0)
        {
            currentCoord = coordStack.Pop();
            breadCrumbs[currentCoord.x, currentCoord.y] = true;

            
            //above
            if(isInBounds(new Coordinate{x = currentCoord.x - 2, y = currentCoord.y}) 
                && pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = currentCoord.x - 1, y = currentCoord.y}, p) 
                && !breadCrumbs[currentCoord.x - 2, currentCoord.y])
            {
                coordStack.Push(new Coordinate{x = currentCoord.x - 2, y = currentCoord.y});
                ++networkSize;
            }

            //below
            if(isInBounds(new Coordinate{x = currentCoord.x + 2, y = currentCoord.y}) 
                && pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = currentCoord.x + 1, y = currentCoord.y}, p) 
                && !breadCrumbs[currentCoord.x + 2, currentCoord.y])
            {
                coordStack.Push(new Coordinate{x = currentCoord.x + 2, y = currentCoord.y});
                ++networkSize;
            }

            //left
            if(isInBounds(new Coordinate{x = currentCoord.x, y = currentCoord.y - 2}) 
                && pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = currentCoord.x, y = currentCoord.y - 1}, p) 
                && !breadCrumbs[currentCoord.x, currentCoord.y - 2])
            {
                coordStack.Push(new Coordinate{x = currentCoord.x, y = currentCoord.y - 2});
                ++networkSize;
            }

            //right
            if(isInBounds(new Coordinate{x = currentCoord.x, y = currentCoord.y + 2}) 
                && pieceAtCoordinateIsOwnedByPlayer(new Coordinate{x = currentCoord.x, y = currentCoord.y + 1}, p) 
                && !breadCrumbs[currentCoord.x, currentCoord.y + 2])
            {
                coordStack.Push(new Coordinate{x = currentCoord.x, y = currentCoord.y + 2});
                ++networkSize;
            }
        }

        return networkSize;
    }

    private GamePiece[,] generateBoard()
    {
        GamePiece[,] newBoard = new GamePiece[boardSize, boardSize];

        //Randomizes order of the tiles
        List<bool> selectedPieces = new List<bool> {true, true, true, true, true, true, true, true, true, true, true, true, true};
        List<GamePiece> randomizedGameTiles = new List<GamePiece>();
        while(selectedPieces.Contains(true))
        {
            rnd = Random.Range(0.0f, 13.0f);
            int newIndex = (int)(Floor(rnd));
            if(selectedPieces[newIndex])
            {
                randomizedGameTiles.Add(GameTiles[newIndex]);
                selectedPieces[newIndex] = false;
            }

        }

        //Initialize board by looping over the tile positions and placing the randomized tiles in them
        //and then surrounding them with new GamePieces
        PieceType pt;
        for(int i = 0; i < tileIndexes.Count; ++i)
        {
            newBoard[tileIndexes[i].x, tileIndexes[i].y] = randomizedGameTiles[i];
            newBoard[tileIndexes[i].x, tileIndexes[i].y].coord = tileIndexes[i];

            for(int row = tileIndexes[i].x - 1; row <= tileIndexes[i].x + 1; ++row)
            {
                for(int col = tileIndexes[i].y - 1; col <= tileIndexes[i].y + 1; ++col)
                {
                    if(newBoard[row,col] == null)
                    {
                        if(col % 2 == 0 && row % 2 == 0)
                        {
                            pt = PieceType.Node;
                            newBoard[row,col] = new GamePiece(new Coordinate{x = row, y = col}, pt);
                        }
                        else
                        {
                            pt = PieceType.Branch;
                            newBoard[row,col] = new GamePiece(new Coordinate{x = row, y = col}, pt);
                        }
                    }
                }
            }
        }
        return newBoard;
    }

    private GamePiece[,] generateBoard(string boardSeed)
    {
        //TODO: this function should take a seed and create a board based upon it
        return new GamePiece[11, 11];
    }

    //checks if a given coordinate is a valid coordinate with a GamePiece on the board
    private bool isInBounds(Coordinate c)
    {
        if(c.x < boardSize && c.x >= 0 && c.y >= 0 && c.y < boardSize && gameBoard[c.x, c.y] != null)
        {
            return true;
        }
        return false;
    }

    //checks if a given coordinate is in bounds and has a piece owned by the given player
    private bool pieceAtCoordinateIsOwnedByPlayer(Coordinate c, Player p)
    {
        if(isInBounds(c) && gameBoard[c.x,c.y].player == p)
        {
            return true;
        }
        return false;
    }

    //checks if player has enough resources to make a move
    private bool playerHasResources(int[] moveCost, Player p)
    {       
        for(int i = 0; i < player1Resources.Length; ++i)
        {
            if((p == Player.Player1 && moveCost[i] + player1Resources[i] < 0) 
                || (p == Player.Player2 && moveCost[i] + player2Resources[i] < 0))
            {
                return false;
            }
        }
        return true;
    }

    private bool isHorizontalBranch(Coordinate c)
    {
        if(isInBounds(c) && c.x % 2 == 0 && c.y % 2 == 1)
        {
            return true;
        }
        return false;
    }

    private bool isVerticalBranch(Coordinate c)
    {
        if(isInBounds(c) && c.x % 2 == 1 && c.y % 2 == 0)
        {
            return true;
        }
        return false;
    }

    private bool isNode(Coordinate c)
    {
        if(isInBounds(c) && c.x % 2 == 0 && c.y % 2 == 0)
        {
            return true;
        }
        return false;
    }

    public void logGameBoard()
    {
        for(int i = 0; i < gameBoard.GetLength(0); ++i)
        {
            string newLine = "";
            for(int j = 0; j < gameBoard.GetLength(1); ++j)
            {
                if(gameBoard[i,j] == null)
                {
                    newLine += "0    ";
                }
                else 
                {
                    switch (gameBoard[i,j].pieceType)
                    {
                        case PieceType.Node:
                        {
                            newLine += "N";
                            break;
                        }

                        case PieceType.Branch:
                        {
                            newLine += "B";
                            break;
                        }

                        case PieceType.Tile:
                        {
                            newLine += "T";
                            break;
                        }
                    }

                    switch (gameBoard[i,j].player)
                    {
                        case Player.Player1:
                        {
                            newLine += "1";
                            break;
                        }

                        case Player.Player2:
                        {
                            newLine += "2";
                            break;
                        }

                        case Player.None:
                        {
                            newLine += "N";
                            break;
                        }
                    }

                    if(gameBoard[i,j].pieceType == PieceType.Tile)
                    {
                        switch(((Tile)gameBoard[i,j]).ResourceType)
                        {
                            case ResourceType.Red:
                            {
                                newLine += "R";
                                break;
                            }

                            case ResourceType.Blue:
                            {
                                newLine += "B";
                                break;
                            }

                            case ResourceType.Green:
                            {
                                newLine += "G";
                                break;
                            }

                            case ResourceType.Yellow:
                            {
                                newLine += "Y";
                                break;
                            }

                            case ResourceType.None:
                            {
                                newLine += "N";
                                break;
                            }
                        }
                        newLine += ((Tile)gameBoard[i,j]).maxLoad;
                        newLine += " ";
                    }
                    else
                    {
                        newLine += "   ";
                    }
                }
            }
            Debug.Log(newLine);
        }
        Debug.Log(player1Resources[0] + player1Resources[1] + player1Resources[2] + player1Resources[3]);
        Debug.Log(player2Resources[0] + player2Resources[1] + player2Resources[2] + player2Resources[3]);
        Debug.Log(getScore(Player.Player1));
        Debug.Log(getScore(Player.Player2));
    }
}
