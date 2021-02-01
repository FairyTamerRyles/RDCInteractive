using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public enum Player
    {
        None = 0,
        Player1 = 1,
        player2 = 2
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

        public Tile(ResourceType r, int max, Player p = None, Coordinate c = null)
        {
            pieceType = Tile;
            ResourceType = r;
            maxLoad = max;
            player = p;
            coord = c;
        }
    }

    public Tile[] GameTiles = new Tile[] 
    {
        new Tile(Red, 1), 
        new Tile(Red, 2), 
        new Tile(Red, 3),
        new Tile(Blue, 1),
        new Tile(Blue, 2),
        new Tile(Blue, 3),
        new Tile(Green, 1),
        new Tile(Green, 2),
        new Tile(Green, 3),
        new Tile(Yellow, 1),
        new Tile(Yellow, 2),
        new Tile(Yellow, 3),
        new Tile(None, 0)
    };
}
