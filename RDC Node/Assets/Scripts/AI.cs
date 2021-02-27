//Code written by Riley Judd
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Math;
using static System.DateTime;

//All functions will default to void. Go back and fix as more information presents itself
public class AI
{
    const float WIN = 1000.0f;
    const float LOSE = -1000.0f;
    //TODO: Make Gamepiece class; most likely in GameCore
    public GameBoard AIGameBoard;
    public GameBoard.Player opponent;
    public MonteCarloTree Freederick;
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

    public class GameState : AI
    {
        public GameBoard unendedState;
        public GameBoard actualState;
        public GameBoard.Player largestNetworkOwner;
        public int P1Score;
        public int P2Score;

        public GameState(GameBoard board)
        {
            unendedState = board;
            GameBoard newBoard = new GameBoard(board);
            newBoard.endTurn();
            actualState = newBoard;
            largestNetworkOwner = actualState.playerWithLargestNetwork();
            P1Score = actualState.getScore(GameBoard.Player.Player1);
            P2Score = actualState.getScore(GameBoard.Player.Player2);
        }
    }

    public class TreeNode
    {
        public GameState state;
        public int visits;
        public float wins;
        public TreeNode parentTreeNode;
        public List<TreeNode> childrenTreeNodes;
        public TreeNode()
        {
            state = null;
            visits = 0;
            wins = 0.0f;
            parentTreeNode = null;
            childrenTreeNodes = new List<TreeNode>();
        }
        public TreeNode(GameState s)
        {
            state = s;
            visits = 0;
            wins = 0.0f;
            parentTreeNode = null;
            childrenTreeNodes = new List<TreeNode>();
        }
        public TreeNode(GameState s, TreeNode parent)
        {
            state = s;
            visits = 0;
            wins = 0f;
            parentTreeNode = parent;
            childrenTreeNodes = new List<TreeNode>();
        }
        public TreeNode getRandomChildTreeNode()
        {
            int randomIndex = (int)Round(Random.Range(0.0f, (float)childrenTreeNodes.Count));
            return childrenTreeNodes[randomIndex];
        }
    }

    public class MonteCarloTree
    {
        public TreeNode root;
        int level;
        GameBoard.Player opponent;

        public MonteCarloTree(GameBoard.Player o, GameBoard firstBoard)
        {
            GameState rootState = new GameState(firstBoard);
            root = new TreeNode(rootState);
            expandTree(root);
            level = 1;
            opponent = o;
        }
        public bool updateRoot(GameBoard gBoard)
        {
            foreach (TreeNode child in root.childrenTreeNodes)
            {
                bool isNewRoot = true;
                TreeNode newRoot = child;
                Debug.Log("About to test for the new root. newRoot's state is");
                Debug.Log(newRoot.state);
                if (child.state.P1Score == gBoard.getScore(GameBoard.Player.Player1) &&
                    child.state.P2Score == gBoard.getScore(GameBoard.Player.Player2) &&
                    child.state.largestNetworkOwner == gBoard.playerWithLargestNetwork())
                {
                    int[] actualP1Resources = gBoard.getResources(GameBoard.Player.Player1);
                    int[] childP1Resources = child.state.actualState.getResources(GameBoard.Player.Player1);
                    for (int i = 0; i < actualP1Resources.Length; i++)
                    {
                        if (actualP1Resources[i] != childP1Resources[i])
                        {
                            isNewRoot = false;
                        }
                    }
                    if (isNewRoot)
                    {
                        int[] actualP2Resources = gBoard.getResources(GameBoard.Player.Player2);
                        int[] childP2Resources = child.state.actualState.getResources(GameBoard.Player.Player2);
                        for (int i = 0; i < actualP2Resources.Length; i++)
                        {
                            if (actualP2Resources[i] != childP2Resources[i])
                            {
                                isNewRoot = false;
                            }
                        }
                    }
                    Debug.Log("All Tests Passed");
                }
                else
                {
                    isNewRoot = false;
                }
                if (isNewRoot)
                {
                    root = newRoot;
                    Debug.Log("New Root Selected: ");
                    Debug.Log(newRoot.state);
                    return true;
                }
            }
            Debug.Log("No newRoot was found");
            return false;
        }

        public TreeNode selectMove()
        {
            int timePassed = 0;
            while(timePassed < 5)
            {
                Debug.Log("selectedMove Root:");
                Debug.Log(root.state);
                TreeNode leafToSimulate = findLeafToSimulate(root);
                if(leafToSimulate.state.actualState.checkForWin() != GameBoard.Player.None) // node isn't a root node
                {
                    expandTree(leafToSimulate);
                }
                if(leafToSimulate.childrenTreeNodes.Count > 0)
                {
                    leafToSimulate = leafToSimulate.getRandomChildTreeNode();
                }
                float rolloutResult = rollout(leafToSimulate.state, 0);
                backPropagate(leafToSimulate, rolloutResult, 0);
                timePassed++;
            }
            TreeNode bestChild = getBestChild();
            root = bestChild;
            root.parentTreeNode = null;
            return bestChild;
        }

        TreeNode findLeafToSimulate(TreeNode root)
        {
            TreeNode node = root;
            Debug.Log(root);
            Debug.Log(root.state);
            while(node.state.actualState.checkForWin() != GameBoard.Player.None || node.visits != 0)
            {
                float maxUCT = Mathf.Infinity;
                TreeNode bestChild = new TreeNode();
                foreach (TreeNode child in node.childrenTreeNodes)
                {
                    int childNodeVisits = child.visits;
                    float childWins = child.wins;
                    int parentVisits = child.parentTreeNode.visits;
                    float UCT = getUCT(parentVisits, childWins, childNodeVisits);
                    if (UCT > maxUCT)
                    {
                        maxUCT = UCT;
                        bestChild = child;
                        node = bestChild;
                    }
                }
            }
            return node;
        }
        TreeNode uctResult()
        {
            return new TreeNode();
        }
        float rollout(GameState simulation, int numCap)
        {
            if(numCap > 10)
            {
                return Mathf.NegativeInfinity;
            }
            if ((simulation.opponent == GameBoard.Player.Player1 && simulation.P1Score >=  10) || (simulation.opponent == GameBoard.Player.Player2 && simulation.P2Score >= 10))
            {
                return Mathf.NegativeInfinity;
            }
            else if ((simulation.opponent == GameBoard.Player.Player1 && simulation.P2Score >=  10) || (simulation.opponent == GameBoard.Player.Player2 && simulation.P1Score >= 10))
            {
                return Mathf.Infinity;
            }
            else
            {
                List<GameBoard> possibleMoves = getPossibleMoves(simulation.actualState);
                if(possibleMoves.Count != 0)
                {
                    int randomIndex = (int)Round(Random.Range(0.0f, (float)possibleMoves.Count));
                    GameBoard chosenMove = possibleMoves[randomIndex];
                    GameState nextState = new GameState(chosenMove);
                    return rollout(nextState, numCap + 1);
                }
                else
                {
                    return Mathf.NegativeInfinity;
                }
            }
        }
        /*float rolloutStrat()
        {
            return new TreeNode();
        }*/
        void backPropagate(TreeNode leaf, float rolloutResult, int numCap)
        {
            TreeNode node = leaf;
            while (node.parentTreeNode != null && numCap < 1000000)
            {
                node.visits++;
                if(node.state.actualState.checkForWin() != node.state.opponent)
                {
                    node.wins += 1.0f;
                }
                numCap++;
                node = node.parentTreeNode;
            }
        }
        TreeNode getBestChild()
        {
            return new TreeNode();
        }
        public float getUCT(int parentVisits, float nodeWinScore, int childVisits)
        {
            if (childVisits == 0)
            {
                return Mathf.Infinity;
            }
            return ((float) nodeWinScore / (float) childVisits) + 1.41f * Mathf.Sqrt(Mathf.Log10(parentVisits) / (float) childVisits);
        }
        void expandTree(TreeNode nodeToExpand)
        {
            List<GameBoard> possibleMoves = getPossibleMoves(nodeToExpand.state.actualState);
            foreach (GameBoard g in possibleMoves)
            {
                GameState state = new GameState(g);
                TreeNode newNode = new TreeNode(state, nodeToExpand);
                nodeToExpand.childrenTreeNodes.Add(newNode);
            }
        }

        public List<GameBoard> getPossibleMoves(GameBoard gBoard)
        {
            List<GameBoard> allTradedBoards = getPossibleTrades(gBoard);
            List<GameBoard> allBranchPlacements = new List<GameBoard>();
            List<GameBoard> allPossibleOptions = new List<GameBoard>();
            foreach (GameBoard g in allTradedBoards)
            {
                List<GameBoard> possibleBranches = getPossibleBranches(g);
                foreach (GameBoard branchBoard in possibleBranches)
                {
                    allBranchPlacements.Add(branchBoard);
                }
            }
            foreach (GameBoard g in allBranchPlacements)
            {
                List<GameBoard> possibleNodes = getPossibleNodes(g);
                foreach (GameBoard nodeBoard in possibleNodes)
                {
                    allPossibleOptions.Add(nodeBoard);
                }
            }
            return allPossibleOptions;
        }

        List<GameBoard> getPossibleTrades(GameBoard gBoard)
        { 
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
                        if(gBoard.isValidTrade(testTrade))
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

        List<GameBoard> getPossibleNodes(GameBoard gBoard)
        {
            List<GameBoard.Coordinate> possibleNodeCoords = new List<GameBoard.Coordinate>();
            List<GameBoard> possibleNodes = new List<GameBoard>();
            for(int i = 0; i < GameBoard.boardSize; ++i)
            {
                for(int j = 0; j < GameBoard.boardSize; ++j)
                {
                    GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = i, y = j};
                    if(gBoard.gameBoard[i,j] != null && gBoard.isNode(testMove) && gBoard.isValidMove(testMove))
                    {
                        possibleNodeCoords.Add(testMove);
                    }
                }
            }
            foreach (GameBoard.Coordinate coord in possibleNodeCoords)
            {
                GameBoard g = new GameBoard(gBoard);
                g.placePiece(coord);
                possibleNodes.Add(g);
            }
            return possibleNodes;
        }
        
        List<GameBoard> getPossibleBranches(GameBoard gBoard)
        {
            List<GameBoard.Coordinate> possibleBranchCoords = new List<GameBoard.Coordinate>();
            List<GameBoard> possibleBranches = new List<GameBoard>();
            for(int i = 0; i < GameBoard.boardSize; ++i)
            {
                for(int j = 0; j < GameBoard.boardSize; ++j)
                {
                    GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = i, y = j};
                    if(gBoard.gameBoard[i,j] != null && gBoard.gameBoard[i,j].pieceType == GameBoard.PieceType.Branch && gBoard.isValidMove(testMove))
                    {
                        possibleBranchCoords.Add(testMove);
                    }
                }
            }
            foreach (GameBoard.Coordinate coord in possibleBranchCoords)
            {
                GameBoard g = new GameBoard(gBoard);
                g.placePiece(coord);
                possibleBranches.Add(g);
            }
            return possibleBranches;
        }
    }

    public AI(GameBoard.Player o, GameBoard firstBoard)
    {
        AIGameBoard = new GameBoard(firstBoard);
        opponent = o;
        Freederick = new MonteCarloTree(o, firstBoard);
    }
    public AI()
    {
        AIGameBoard = new GameBoard();
        opponent = GameBoard.Player.None;
        Freederick = null;
    }

    public GameBoard makeMove(GameBoard gBoard)
    {
        Freederick.updateRoot(gBoard);
        Debug.Log("The cheese is");
        Debug.Log(Freederick.root);
        TreeNode selectedNode = Freederick.selectMove();
        return selectedNode.state.unendedState;
    }

    public float heuristic()
    {
        float heuristicResult = 0;
        return heuristicResult;
    }
    /*GameBoard[] getPossibleMoves(GameBoard board, float player)
    {
        GameBoard gameBoard = new GameBoard();
        GameBoard[] result = new GameBoard[5];
        result[0] = gameBoard;
        return result;
    }*/
    /*public List<GameBoard> getPossiblePiecePlacements(GameBoard gBoard)
    {
        List<GameBoard> possiblePiecePlacements = new List<GameBoard>();
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
        return possiblePiecePlacements;
    }*/
/*
    List<GameBoard> getPossibleTrades(GameBoard gBoard)
    { 
        List<int[]> possibleTrades = new List<int[]>();
        List<GameBoard> tradedBoards = new List<GameBoard>();
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
                    if(AIGameBoard.isValidTrade(testTrade))
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
    void getPossibleNodes(GameBoard gBoard)
    {
        List<GameBoard.Coordinate> possibleNodeCoords = new List<GameBoard.Coordinate>();
        List<GameBoard> possibleNodes = new List<GameBoard>();
        for(int i = 0; i < GameBoard.boardSize; ++i)
        {
            for(int j = 0; j < GameBoard.boardSize; ++j)
            {
                GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = i, y = j};
                if(gBoard.gameBoard[i][j].pieceType == GameBoard.PieceType.node && gameBoard.isValidMove(testMove))
                {
                    possibleNodes.Add(testMove);
                }
            }
        }
        return possibleNodes;
    }
    List<GameBoard> getPossibleBranches(GameBoard gBoard)
    {
        List<GameBoard> possibleBranches = new List<GameBoard>();
        for(int i = 0; i < GameBoard.boardSize; ++i)
        {
            for(int j = 0; j < GameBoard.boardSize; ++j)
            {
                GameBoard.Coordinate testMove = new GameBoard.Coordinate{x = i, y = j};
                if(gBoard.gameBoard[i][j].pieceType == GameBoard.PieceType.branch && gameBoard.isValidMove(testMove))
                {
                    possibleBranches.Add(testMove);
                }
            }
        }
        return possibleBranches;
    }*/
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
