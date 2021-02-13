using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    private AI testAI;
    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameBoard();
        testAI = new AI();

        GameBoard.GamePiece[,] testFullEnclosure = {{null, null, null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 0, y = 4}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 0, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 0, y = 6}, GameBoard.PieceType.Node), null, null, null, null},
                                                    {null, null, null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 1, y = 4}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Red, 1), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 1, y = 6}, GameBoard.PieceType.Branch), null, null, null, null},
                                                    {null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 2}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 3}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 4}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 6}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 7}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 2, y = 8}, GameBoard.PieceType.Node), null, null},
                                                    {null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 3, y = 2}, GameBoard.PieceType.Branch),  new GameBoard.Tile(GameBoard.ResourceType.Red, 2), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 3, y = 4}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Blue, 3), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 3, y = 6}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Green, 3), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 3, y = 8}, GameBoard.PieceType.Branch), null, null},
                                                    {new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 0}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 1}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 2}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 3}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 4}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 6}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 7}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 8}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 9}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 4, y = 10}, GameBoard.PieceType.Node)},
                                                    {new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 0}, GameBoard.PieceType.Branch),  new GameBoard.Tile(GameBoard.ResourceType.Red, 3), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 2}, GameBoard.PieceType.Branch),  new GameBoard.Tile(GameBoard.ResourceType.Green, 1), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 4}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Yellow, 1), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 6}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Yellow, 3), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 8}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.None, -1), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 5, y = 8}, GameBoard.PieceType.Branch)},
                                                    {new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 0}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 1}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 2}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 3}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 4}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 6}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 7}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 8}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 9}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 6, y = 10}, GameBoard.PieceType.Node)},
                                                    {null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 7, y = 2}, GameBoard.PieceType.Branch),  new GameBoard.Tile(GameBoard.ResourceType.Blue, 1), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 7, y = 4}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Green, 2), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 7, y = 6}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Yellow, 2), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 7, y = 8}, GameBoard.PieceType.Branch), null, null},
                                                    {null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 2}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 3}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 4}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 6}, GameBoard.PieceType.Node), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 7}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 8, y = 8}, GameBoard.PieceType.Node), null, null},
                                                    {null, null, null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 9, y = 4}, GameBoard.PieceType.Branch), new GameBoard.Tile(GameBoard.ResourceType.Blue, 2), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 9, y = 6}, GameBoard.PieceType.Branch), null, null, null, null},
                                                    {null, null, null, null, new GameBoard.GamePiece(new GameBoard.Coordinate{x = 10, y = 4}, GameBoard.PieceType.Node),  new GameBoard.GamePiece(new GameBoard.Coordinate{x = 10, y = 5}, GameBoard.PieceType.Branch), new GameBoard.GamePiece(new GameBoard.Coordinate{x = 10, y = 6}, GameBoard.PieceType.Node), null, null, null, null}};
        testFullEnclosure[1, 5].coord.x = 1;
        testFullEnclosure[1, 5].coord.y = 5;
        testFullEnclosure[3, 5].coord.x = 3;
        testFullEnclosure[3, 5].coord.y = 5;
        testFullEnclosure[3, 3].coord.x = 3;
        testFullEnclosure[3, 3].coord.y = 3;
        testFullEnclosure[3, 7].coord.x = 3;
        testFullEnclosure[3, 7].coord.y = 7;
        testFullEnclosure[5, 1].coord.x = 5;
        testFullEnclosure[5, 1].coord.y = 1;
        testFullEnclosure[5, 3].coord.x = 5;
        testFullEnclosure[5, 3].coord.y = 3;
        testFullEnclosure[5, 5].coord.x = 5;
        testFullEnclosure[5, 5].coord.y = 5;
        testFullEnclosure[5, 7].coord.x = 5;
        testFullEnclosure[5, 7].coord.y = 7;
        testFullEnclosure[5, 9].coord.x = 5;
        testFullEnclosure[5, 9].coord.y = 9;
        testFullEnclosure[7, 3].coord.x = 7;
        testFullEnclosure[7, 3].coord.y = 3;
        testFullEnclosure[7, 5].coord.x = 7;
        testFullEnclosure[7, 5].coord.y = 5;
        testFullEnclosure[7, 7].coord.x = 7;
        testFullEnclosure[7, 7].coord.y = 7;
        testFullEnclosure[9, 5].coord.x = 9;
        testFullEnclosure[9, 5].coord.y = 5;

        testAI.AIGameBoard.gameBoard = testFullEnclosure;

        //branches around 5 - 1
        testFullEnclosure[5, 0].player = GameBoard.Player.Player1;
        testFullEnclosure[4, 1].player = GameBoard.Player.Player1;
        testFullEnclosure[6, 1].player = GameBoard.Player.Player1;
        //branches around 3 - 3
        testFullEnclosure[2, 3].player = GameBoard.Player.Player1;
        testFullEnclosure[3, 2].player = GameBoard.Player.Player1;
        testFullEnclosure[3, 4].player = GameBoard.Player.Player1;
        //branches around 5 - 3
        testFullEnclosure[5, 4].player = GameBoard.Player.Player1;
        //branches around 7 - 3
        testFullEnclosure[7, 2].player = GameBoard.Player.Player1;
        testFullEnclosure[8, 3].player = GameBoard.Player.Player1;
        //branches around 7 - 5
        testFullEnclosure[6, 5].player = GameBoard.Player.Player1;
        testFullEnclosure[8, 5].player = GameBoard.Player.Player1;
        //branches around 7 - 7
        testFullEnclosure[7, 8].player = GameBoard.Player.Player1;
        testFullEnclosure[8, 7].player = GameBoard.Player.Player1;
        //branches around 5 - 7
        testFullEnclosure[5, 6].player = GameBoard.Player.Player1;
        testFullEnclosure[5, 8].player = GameBoard.Player.Player1;
        //branches around 3 - 7
        testFullEnclosure[2, 7].player = GameBoard.Player.Player1;
        testFullEnclosure[3, 8].player = GameBoard.Player.Player1;
        //branches around 3 - 5
        testFullEnclosure[3, 4].player = GameBoard.Player.Player1;
        testFullEnclosure[2, 5].player = GameBoard.Player.Player1;
        //branches around 5 - 5
        testFullEnclosure[5, 4].player = GameBoard.Player.Player1;
        testFullEnclosure[5, 6].player = GameBoard.Player.Player1;
        testFullEnclosure[6, 5].player = GameBoard.Player.Player1;

        //testAI.setCapturedTiles(testAI.AIGameBoard.GameTiles, GameBoard.Player.Player1);

        /*foreach (GameBoard.Tile tile in test1.tileStack)
        {
            Debug.Log(tile.coord.x + " - " + tile.coord.y);
        }
        Debug.Log(test1.isCaptured);*/
    }
}
