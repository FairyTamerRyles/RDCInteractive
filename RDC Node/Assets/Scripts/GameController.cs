using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameBoard();
        testSomeStuff();
    }

    private void testSomeStuff()
    {
        GameBoard.Move m = new GameBoard.Move(new int[]{0,0,0,0}, GameBoard.Player.Player1, new GameBoard.Coordinate{x = 0, y = 4}, GameBoard.MoveType.StartMove);
        GameBoard.Move m1 = new GameBoard.Move(new int[]{0,0,0,0}, GameBoard.Player.Player1, new GameBoard.Coordinate{x = 1, y = 4}, GameBoard.MoveType.StartMove);
        GameBoard.Move m2 = new GameBoard.Move(new int[]{0,0,0,0}, GameBoard.Player.Player2, new GameBoard.Coordinate{x = 0, y = 4}, GameBoard.MoveType.StartMove);

        GameBoard.Move m3 = new GameBoard.Move(new int[]{0,0,0,0}, GameBoard.Player.Player2, new GameBoard.Coordinate{x = 0, y = 6}, GameBoard.MoveType.StartMove);
        GameBoard.Move m4 = new GameBoard.Move(new int[]{0,0,0,0}, GameBoard.Player.Player2, new GameBoard.Coordinate{x = 1, y = 6}, GameBoard.MoveType.StartMove);
        GameBoard.Move m5 = new GameBoard.Move(new int[]{0, 0,0,0}, GameBoard.Player.Player2, new GameBoard.Coordinate{x = 3, y = 6}, GameBoard.MoveType.PlaceBranch);
        GameBoard.Move m6 = new GameBoard.Move(new int[]{-1, -1,0,0}, GameBoard.Player.Player2, new GameBoard.Coordinate{x = 3, y = 6}, GameBoard.MoveType.EndTurn);

        List<GameBoard.Move> moves = new List<GameBoard.Move>();

        moves.Add(m);
        moves.Add(m1);
        moves.Add(m2);
        moves.Add(m3);
        moves.Add(m4);
        moves.Add(m5);
        moves.Add(m6);

        gameBoard.makeMove(moves);

        gameBoard.logGameBoard();
    }
}
