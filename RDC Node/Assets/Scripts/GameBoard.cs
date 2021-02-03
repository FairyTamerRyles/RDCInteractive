using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    const int numResources = 4;

    int[] player1Resources = new int[numResources];
    int[] player2Resources = new int[numResources];

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
        public GamePiece[] adjacentPieces = new GamePiece[8];
    }

    public class Tile : GamePiece
    {
        public ResourceType ResourceType;
        public int maxLoad;

        public Tile(ResourceType r, int max)
        {
            pieceType = PieceType.Tile;
            ResourceType = r;
            maxLoad = max;
            player = Player.None;
            coord = new Coordinate() {x = 0, y = 0};
        }
    }

    public class Move
    {
        int[] resourceChange;
        Player player;

        public Move(int[] rChange, Player p)
        {
            resourceChange = rChange;
            player = p;
        }
    }

    public class PlacePiece : Move
    {
        Coordinate coord;
        public PlacePiece(int[] rChange, Player p, Coordinate c) : base(rChange, p)
        {
            coord = c;
        }
    }

    public Tile[] GameTiles = new Tile[] 
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
        new Tile(ResourceType.None, 0)
    };

    public int getScore(Player p)
    {
        return numberOfNodes(p) + numberCapturedTiles(p) + longestNetwork(p);
    }

    private int numberOfNodes(Player p)
    {
        return 0;
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
        return true;
    }
}
