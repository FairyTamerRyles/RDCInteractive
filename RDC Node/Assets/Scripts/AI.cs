//Code written by Riley Judd
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Math;

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

    public class GameState
    {
        public GameBoard unendedState;
        public GameBoard actualState;
        public GameBoard.Player largestNetworkOwner;
        public int P1Score;
        public int P2Score;
        public GameBoard.Player opponent;

        public GameState(GameBoard board, GameBoard.Player o)
        {
            opponent = o;
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
            int randomIndex = (int)Round(Random.Range(0.0f, (float)childrenTreeNodes.Count - 1));
            return childrenTreeNodes[randomIndex];
        }
    }

    public class MonteCarloTree
    {
        public TreeNode root;
        int level;
        GameBoard.Player opponent;

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
            new GameBoard.Coordinate{x = 10, y = 4},
            new GameBoard.Coordinate{x = 10, y = 6}
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

        public MonteCarloTree(GameBoard.Player o, GameBoard firstBoard)
        {
            GameState rootState = new GameState(firstBoard, o);
            root = new TreeNode(rootState);
            expandTree(root);
            level = 1;
            opponent = o;
        }
        public bool updateRoot(GameBoard gBoard)
        {
            System.DateTime time = System.DateTime.Now;
            foreach (TreeNode child in root.childrenTreeNodes)
            {
                bool isNewRoot = true;
                TreeNode newRoot = child;
                if (child.state.actualState.getCurrentPlayer() == gBoard.getCurrentPlayer() &&
                    child.state.P1Score == gBoard.getScore(GameBoard.Player.Player1) &&
                    child.state.P2Score == gBoard.getScore(GameBoard.Player.Player2) &&
                    child.state.largestNetworkOwner == gBoard.playerWithLargestNetwork())
                {
                    //Debug.Log("Player and Score");
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
                        //Debug.Log("Player1 resource");
                        int[] actualP2Resources = gBoard.getResources(GameBoard.Player.Player2);
                        int[] childP2Resources = child.state.actualState.getResources(GameBoard.Player.Player2);
                        for (int i = 0; i < actualP2Resources.Length; i++)
                        {
                            if (actualP2Resources[i] != childP2Resources[i])
                            {
                                isNewRoot = false;
                            }
                        }
                        if(isNewRoot)
                        {
                            //Debug.Log("Player2 resource");
                            for(int i = 0; i < 11; i++)
                            {
                                for(int j = 0; j < 11; j++)
                                {
                                    if(gBoard.gameBoard[i,j] != null)
                                    {
                                        if (gBoard.gameBoard[i,j].player != child.state.actualState.gameBoard[i,j].player ||
                                            gBoard.gameBoard[i,j].pieceType != child.state.actualState.gameBoard[i,j].pieceType)
                                        {
                                            isNewRoot = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    isNewRoot = false;
                }
                if (isNewRoot)
                {
                    //Debug.Log("Board matched");
                    root = newRoot;
                    if (root.childrenTreeNodes.Count == 0 && root.state.actualState.checkForWin() == GameBoard.Player.None)
                    {
                        expandTree(root);
                    }
                    root.parentTreeNode = null;
                    Debug.Log("Update Root time: " + (System.DateTime.Now - time));
                    return true;
                }
            }
            Debug.Log("No newRoot was found");
            if (root.childrenTreeNodes.Count == 0 && root.state.actualState.checkForWin() == GameBoard.Player.None)
            {
                expandTree(root);
            }
            root.parentTreeNode = null;
            Debug.Log("Update Root time: " + (System.DateTime.Now - time));
            return false;
        }

        public TreeNode selectMove()
        {
            int timePassed = 0;
            System.DateTime selectTime = System.DateTime.Now;
            while(timePassed < 3)
            {
                //Debug.Log("Root currently has " + root.childrenTreeNodes.Count + " before finding leaf");
                TreeNode leafToSimulate = findLeafToSimulate(root, 0);
                Debug.Log("LeafSelection took " + (System.DateTime.Now - selectTime));
                selectTime = System.DateTime.Now;
                if(leafToSimulate.state.actualState.checkForWin() == GameBoard.Player.None && leafToSimulate.childrenTreeNodes.Count == 0) // node isn't a root node
                {
                    //Debug.Log("did expand leaf");
                    expandTree(leafToSimulate);
                    Debug.Log("ExpandTree took " + (System.DateTime.Now - selectTime));
                    selectTime = System.DateTime.Now;
                }
                if(leafToSimulate.childrenTreeNodes.Count > 0)
                {
                    leafToSimulate = leafToSimulate.getRandomChildTreeNode();
                }
                selectTime = System.DateTime.Now;
                float rolloutResult = rollout(leafToSimulate.state, 0);
                Debug.Log("Rollout took " + (System.DateTime.Now - selectTime));
                selectTime = System.DateTime.Now;
                backPropagate(leafToSimulate, rolloutResult, 0);
                Debug.Log("backPropogate took " + (System.DateTime.Now - selectTime));
                selectTime = System.DateTime.Now;
                timePassed++;
            }
            TreeNode bestChild = getBestChild(root);
            selectTime = System.DateTime.Now;
            root = bestChild;
            root.parentTreeNode = null;
            if(root.state.actualState.checkForWin() == GameBoard.Player.None && root.childrenTreeNodes.Count == 0) // node isn't a root node
            {
                //Debug.Log("did expand leaf");
                expandTree(root);
            }
            return bestChild;
        }

        TreeNode findLeafToSimulate(TreeNode root, int numCap)
        {
            TreeNode node = root;
            while(numCap < 100 && node.state.actualState.checkForWin() == GameBoard.Player.None && node.visits != 0)
            {
                float maxUCT = Mathf.NegativeInfinity;
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
                numCap++;
            }
            if(numCap >= 100)
            {
                Debug.Log("FindLeafCappedOut");
            }
            return node;
        }
        float rollout(GameState simulation, int numCap)
        {
            if(numCap > 50)
            {
                Debug.Log("Rollout capped Out");
                return Mathf.NegativeInfinity;
            }
            if (simulation.actualState.checkForWin() == opponent)
            {
                Debug.Log("Cap: " + numCap);
                Debug.Log("Simulation lost");
                return Mathf.NegativeInfinity;
            }
            else if (simulation.actualState.checkForWin() != opponent && simulation.actualState.checkForWin() != GameBoard.Player.None)
            {
                Debug.Log("Cap: " + numCap);
                Debug.Log("Simulation won");
                return Mathf.Infinity;
            }
            else
            {
                System.DateTime time = System.DateTime.Now;
                List<GameBoard> possibleMoves = getPossibleMoves(simulation.actualState);
                Debug.Log("PossibleMoves took: " + (System.DateTime.Now - time));
                Debug.Log("PossibleMoves count: " + possibleMoves.Count);
                int randomIndex = (int)Round(Random.Range(0.0f, (float)possibleMoves.Count - 1));
                //Debug.Log("Random: " + randomIndex);
                GameBoard chosenMove = possibleMoves[randomIndex];
                GameState nextState = new GameState(chosenMove, simulation.opponent);
                return rollout(nextState, numCap + 1);
            }
        }
        /*float rolloutStrat()
        {
            return new TreeNode();
        }*/
        void backPropagate(TreeNode leaf, float rolloutResult, int numCap)
        {
            TreeNode node = leaf;
            while (node.parentTreeNode != null)
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
        TreeNode getBestChild(TreeNode root)
        {
            TreeNode bestChild = new TreeNode();
            foreach (TreeNode child in root.childrenTreeNodes)
            {
                if (child.wins > bestChild.wins)
                {
                    bestChild = child;
                }
            }
            Debug.Log("Children: " + root.childrenTreeNodes.Count);
            if(root.childrenTreeNodes.IndexOf(bestChild) == -1)
            {
                bestChild = root.childrenTreeNodes[0];
            }
            return bestChild;
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
                GameState state = new GameState(g, nodeToExpand.state.opponent);
                TreeNode newNode = new TreeNode(state, nodeToExpand);
                nodeToExpand.childrenTreeNodes.Add(newNode);
            }
        }

        public List<GameBoard> getPossibleMoves(GameBoard gBoard)
        {
            List<GameBoard> allTradedBoards = getPossibleTrades(gBoard);
            List<GameBoard> allBranchPlacements = new List<GameBoard>();
            List<GameBoard> allPossibleOptions = new List<GameBoard>();
            List<GameBoard.Coordinate> branchCoords = copyBranchCoords(branchIndexes);
            List<GameBoard.Coordinate> nodeCoords = copyNodeCoords(nodeIndexes);
            Debug.Log("Number of Possible Trades: " + allTradedBoards.Count);
            foreach (GameBoard g in allTradedBoards)
            {
                List<GameBoard> possibleBranches = getPossibleBranches(g, branchCoords);
                if (gBoard.getSetupCounter() > 4)
                {
                    possibleBranches.Add(new GameBoard(g));
                }
                foreach (GameBoard branchBoard in possibleBranches)
                {
                    allBranchPlacements.Add(branchBoard);
                }
            }
            Debug.Log("Number of Possible Branches with Trades: " + allBranchPlacements.Count);
            foreach (GameBoard g in allBranchPlacements)
            {
                List<GameBoard> possibleNodes = getPossibleNodes(g, nodeCoords);
                if (gBoard.getSetupCounter() > 4)
                {
                    possibleNodes.Add(new GameBoard(g));
                }
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

        List<GameBoard> getPossibleNodes(GameBoard gBoard, List<GameBoard.Coordinate> unvisitedCoords)
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

                List<GameBoard.Coordinate> abridgedNodeCoords = new List<GameBoard.Coordinate>();
                foreach (GameBoard.Coordinate coord in possibleNodeCoords)
                {
                    abridgedNodeCoords = copyNodeCoords(possibleNodeCoords, coord);
                    GameBoard g = new GameBoard(gBoard);
                    g.placePiece(coord);
                    possibleNodes.Add(g);
                    possibleNodes = possibleNodes.Concat(getPossibleNodes(g, abridgedNodeCoords)).ToList();
                }
            }
            return possibleNodes;
        }
        
        List<GameBoard> getPossibleBranches(GameBoard gBoard, List<GameBoard.Coordinate> unvisitedCoords)
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

                List<GameBoard.Coordinate> abridgedBranchCoords = new List<GameBoard.Coordinate>();
                foreach (GameBoard.Coordinate coord in possibleInitialBranchCoords)
                {
                    abridgedBranchCoords = copyBranchCoords(possibleInitialBranchCoords, coord);
                    GameBoard g = new GameBoard(gBoard);
                    g.placePiece(coord);
                    possibleBranches.Add(g);
                    abridgedBranchCoords = addNewCoordinates(g, coord, abridgedBranchCoords);
                    possibleBranches = possibleBranches.Concat(getPossibleBranches(g, abridgedBranchCoords)).ToList();
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
                    abridgedCoords.Add(c);
                }
            }
            return abridgedCoords;
        }
    }

    public AI(GameBoard.Player o, GameBoard firstBoard)
    {
        AIGameBoard = new GameBoard(firstBoard);
        opponent = o;
        Freederick = new MonteCarloTree(o, firstBoard);
    }
    /*public AI()
    {
        AIGameBoard = new GameBoard();
        opponent = GameBoard.Player.None;
        Freederick = null;
    }*/

    public GameBoard makeMove(GameBoard gBoard)
    {
        System.DateTime time = System.DateTime.Now;
        Freederick.updateRoot(gBoard);
        Debug.Log("After updating the root, Freedericks children count is " + Freederick.root.childrenTreeNodes.Count);
        TreeNode selectedNode = Freederick.selectMove();
        Debug.Log("Selection of move from Freederick took " + (System.DateTime.Now - time));
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
