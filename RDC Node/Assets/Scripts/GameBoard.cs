using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public enum Player
    {
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
        Yellow = 3
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
    }

    public class Tile : GamePiece
    {
        public ResourceType ResourceType;
        public int maxLoad;
    }
}
