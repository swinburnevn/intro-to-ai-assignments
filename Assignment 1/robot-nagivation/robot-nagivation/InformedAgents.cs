using System;
using System.Collections.Generic;
using System.Text;


using SFML.System;


namespace robot_nagivation
{
    public class AStarAgent : Agent
    {
        /*  Depth-First Search Specific Items  */
        private Stack<Node<TileType>> _nodeQueue;
        private HashSet<Node<TileType>> _searchedNodes;

        public AStarAgent()
        {
            _nodeQueue = new Stack<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _nodeQueue.Push(AgentData.RootNode);
        }

        public override int HeuristicFunction(Vector2i Pos, Percepts percepts)
        {

            int manhattanDistance = 0;

            // Manhanttan Distance to the FIRST goal

            Vector2i vectorDistance = percepts.GoalPositions[0] - Pos;
            manhattanDistance = Math.Abs(vectorDistance.X + vectorDistance.Y);

            return manhattanDistance;
        }

        public override int CostFunction(Vector2i Pos, Percepts percepts)
        {
            int stepsTaken = 0;

            //somehow return the amount of steps taken to get here?

            return 0;


        }


        public override AgentActions next(Percepts percepts)
        {


                
            switch (State)
            {
                case AgentState.Searching:

                    if (_nodeQueue.Count > 0)
                    {
                        Node<TileType> currentNode = _nodeQueue.Pop();

                        if (IsGoalNode(currentNode))
                        {
                            State = AgentState.Moving;

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode);
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                            State = AgentState.Moving;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.SearchedNodes.Add(currentNode);

                        AgentData.PosToSearch.Add(currentNode.Pos);
                        AgentData.SearchedPos.Add(currentNode.Pos);
                        _searchedNodes.Add(currentNode);


                        List<Node<TileType>> bestNodes = new List<Node<TileType>>();
                        int lowestCostFound = 50000; // some high cost or cost of first child node.

                        foreach (Node<TileType> subnode in SearchSurroundingNodes(currentNode, percepts))
                        {
                            if (!_searchedNodes.Contains(subnode))
                                if (!AgentData.SearchedPos.Contains(subnode.Pos))
                                {

                                    if (subnode.Cost <= lowestCostFound)
                                    {
                                        lowestCostFound = subnode.Cost;
                                        bestNodes.Add(subnode);
                                    }
                                }
                        }

                        foreach (Node<TileType> potentialNode in bestNodes)
                        {
                            if (potentialNode.Cost <= lowestCostFound)
                            {
                                _nodeQueue.Push(potentialNode);
                            }
                        }



                    }

                    return AgentActions.Search;


                case AgentState.Moving:

                    AgentData.Path.Add(percepts.AgentPos);

                    if (AgentData.DeterminedMoveSet.Count > 0)
                        return AgentData.DeterminedMoveSet.Dequeue();
                    State = AgentState.Finished;
                    break;

                default:
                    break;

            }

            return AgentActions.Idle;
        }
    }

    public class GreedyFirstAgent : Agent
    {
        /*  Breadth-First Search Specific Items  */
        private Queue<Node<TileType>> _nodeQueue;
        private HashSet<Node<TileType>> _searchedNodes;

        public GreedyFirstAgent()
        {
            _nodeQueue = new Queue<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _nodeQueue.Enqueue(AgentData.RootNode);
        }

        public override int HeuristicFunction(Vector2i Pos, Percepts percepts)
        {

            int manhattanDistance = 0;

            // Manhanttan Distance to the FIRST goal

            Vector2i vectorDistance = percepts.GoalPositions[0] - Pos;
            manhattanDistance = Math.Abs(vectorDistance.X + vectorDistance.Y);

            return manhattanDistance;
        }

        public override AgentActions next(Percepts percepts)
        {
            switch (State)
            {
                case AgentState.Searching:

                    if (_nodeQueue.Count > 0)
                    {
                        Node<TileType> currentNode = _nodeQueue.Dequeue();

                        if (IsGoalNode(currentNode))
                        {
                            State = AgentState.Moving;

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode);
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                            State = AgentState.Moving;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.SearchedNodes.Add(currentNode);


                        List<Node<TileType>> bestNodes = new List<Node<TileType>>();
                        int lowestCostFound = 50000; // some high cost or cost of first child node.

                        foreach (Node<TileType> subnode in SearchSurroundingNodes(currentNode, percepts))
                        {
                            if (!_searchedNodes.Contains(subnode))
                                if (!AgentData.SearchedPos.Contains(subnode.Pos))
                                {

                                    if (subnode.Cost <= lowestCostFound)
                                     {
                                        lowestCostFound = subnode.Cost;
                                        bestNodes.Add(subnode);
                                    }


                                    _searchedNodes.Add(subnode);
                                    AgentData.PosToSearch.Add(subnode.Pos);
                                    AgentData.SearchedPos.Add(subnode.Pos);
                                }
                        }

                        foreach (Node<TileType> potentialNode in bestNodes)
                        {
                            if (potentialNode.Cost <= lowestCostFound)
                            {
                                _nodeQueue.Enqueue(potentialNode);
                            }
                        }



                    }

                    return AgentActions.Search;


                case AgentState.Moving:

                    AgentData.Path.Add(percepts.AgentPos);

                    if (AgentData.DeterminedMoveSet.Count > 0)
                        return AgentData.DeterminedMoveSet.Dequeue();
                    State = AgentState.Finished;
                    break;

                default:
                    break;

            }

            return AgentActions.Idle;
        }
    }
}
