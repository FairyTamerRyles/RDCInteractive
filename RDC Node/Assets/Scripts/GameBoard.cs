using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class GameBoard
{

    private float rnd;
    const int boardSize = 11;

    public void Start()
    {
        //Hi
    }

    const int numResources = 4;

    int[] player1Resources;
    int[] player2Resources;

    GamePiece[,] gameBoard;

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
        EndTurn = 0,
        Trade = 1,
        PlaceNode = 2,
        PlaceBranch = 3
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

        public Move(int[] rChange, Player p, Coordinate c)
        {
            resourceChange = rChange;
            player = p;
            coord = c;
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
    }

    public int getScore(Player p)
    {
        return numberOfNodes(p) + numberCapturedTiles(p) + longestNetwork(p);
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

    private int numberCapturedTiles(Player p)
    {
        return 0;
    }

    private int longestNetwork(Player p)
    {
        return 0;
    }

    public void makeMove(Move[] moves)
    {
        
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
        //checks if move is an EndTurn
        if(m.moveType == MoveType.EndTurn)
        {
            return true;
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
                //checks if player has enough resources
                bool enoughResources = true;
                for(int i = 0; i < player1Resources.Length; ++i)
                {
                    if((m.player == Player.Player1 && m.resourceChange[i] + player1Resources[i] < 0) 
                        || (m.player == Player.Player2 && m.resourceChange[i] + player2Resources[i] < 0))
                    {
                        enoughResources = false;
                    }
                }
                if(enoughResources)
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
            if(m.coord.x < boardSize && m.coord.x >= 0 && m.coord.y >= 0 && m.coord.y < boardSize 
                && gameBoard[m.coord.x, m.coord.y] != null && gameBoard[m.coord.x, m.coord.y].player == Player.None)
            {
                //checks to ensure nodes are only placed at appropriate coordinates
                if(m.coord.x % 2 == 0 && m.coord.y % 2 == 0)
                {
                    //checks if there is an adjacent piece owned by the player
                    if((m.coord.x - 1 >= 0 && gameBoard[m.coord.x - 1, m.coord.y].player == m.player) 
                        || (m.coord.x + 1 < gameBoard.GetLength(0) && gameBoard[m.coord.x + 1, m.coord.y].player == m.player)
                        || (m.coord.y - 1 >= 0 && gameBoard[m.coord.x, m.coord.y - 1].player == m.player)
                        || (m.coord.y + 1 < gameBoard.GetLength(1) && gameBoard[m.coord.x, m.coord.y + 1].player == m.player))
                        {
                            //checks if player has appropriate resources - NOTE: This does not currently check if the resources spent for a node or branch is correct
                            bool enoughResources = true;
                            for(int i = 0; i < player1Resources.Length; ++i)
                            {
                                if((m.player == Player.Player1 && m.resourceChange[i] + player1Resources[i] < 0) 
                                   || (m.player == Player.Player2 && m.resourceChange[i] + player2Resources[i] < 0))
                                {
                                    enoughResources = false;
                                }
                            }

                            if(enoughResources)
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
            if(m.coord.x < boardSize && m.coord.x >= 0 && m.coord.y >= 0 && m.coord.y < boardSize 
                && gameBoard[m.coord.x, m.coord.y] != null && gameBoard[m.coord.x, m.coord.y].player == Player.None)
            {
                //checks to ensure branches and nodes are only placed on appropriate coordinates
                if((m.coord.x % 2 == 0 && m.coord.y % 2 == 1) || (m.coord.x % 2 == 1 && m.coord.y % 2 == 0))
                {
                    //checks if there is an adjacent node or 2-away branch owned by the player
                    if((m.coord.x % 2 == 1 && ((m.coord.x - 1 >= 0 && gameBoard[m.coord.x - 1, m.coord.y].player == m.player) || (m.coord.x + 1 < gameBoard.GetLength(0) && gameBoard[m.coord.x + 1, m.coord.y].player == m.player) || (m.coord.x - 2 >= 0 && gameBoard[m.coord.x - 2, m.coord.y].player == m.player) || (m.coord.x + 2 < gameBoard.GetLength(0) && gameBoard[m.coord.x + 2, m.coord.y].player == m.player)))
                        || (m.coord.x % 2 == 0 && ((m.coord.y - 1 >= 0 && gameBoard[m.coord.x, m.coord.y - 1].player == m.player) || (m.coord.y + 1 < gameBoard.GetLength(1) && gameBoard[m.coord.x, m.coord.y + 1].player == m.player) || (m.coord.y - 2 >= 0 && gameBoard[m.coord.x, m.coord.y - 2].player == m.player) || (m.coord.y + 2 < gameBoard.GetLength(1) && gameBoard[m.coord.x, m.coord.y + 2].player == m.player))))
                    {
                        //checks if player has appropriate resources - NOTE: This does not currently check if the resources spent for a node or branch is correct
                        bool enoughResources = true;
                        for(int i = 0; i < player1Resources.Length; ++i)
                        {
                            if((m.player == Player.Player1 && m.resourceChange[i] + player1Resources[i] < 0) 
                                || (m.player == Player.Player2 && m.resourceChange[i] + player2Resources[i] < 0))
                            {
                                enoughResources = false;
                            }
                        }
                        if(enoughResources)
                        {
                            //checks if branch is in an area captured by the opponent
                            if(m.coord.x % 2 == 0)
                            {
                                if(m.coord.x - 1 < 0 || gameBoard[m.coord.x - 1, m.coord.y] == null 
                                || gameBoard[m.coord.x - 1, m.coord.y].player == Player.None || gameBoard[m.coord.x - 1, m.coord.y].player == m.player
                                || m.coord.x + 1 >= gameBoard.GetLength(0) || gameBoard[m.coord.x + 1, m.coord.y] == null
                                || gameBoard[m.coord.x + 1, m.coord.y].player == Player.None || gameBoard[m.coord.x + 1, m.coord.y].player == m.player)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                 if(m.coord.y - 1 < 0 || gameBoard[m.coord.x, m.coord.y - 1] == null 
                                || gameBoard[m.coord.x, m.coord.y - 1].player == Player.None || gameBoard[m.coord.x, m.coord.y - 1].player == m.player
                                || m.coord.y + 1 >= gameBoard.GetLength(1) || gameBoard[m.coord.x, m.coord.y + 1] == null
                                || gameBoard[m.coord.x, m.coord.y + 1].player == Player.None || gameBoard[m.coord.x, m.coord.y + 1].player == m.player)
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

        for(int i = 0; i < 11; ++i)
        {
            for(int j = 0; j < 11; ++j)
            {
                if(newBoard[i,j] != null){ Debug.Log(newBoard[i,j].pieceType);}
                else {Debug.Log("0");}
            }
        }

        return newBoard;
    }

    private GamePiece[,] generateBoard(string boardSeed)
    {
        return new GamePiece[11, 11];
    }
}
