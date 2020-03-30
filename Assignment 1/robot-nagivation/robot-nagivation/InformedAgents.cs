using System;
using System.Collections.Generic;
using System.Text;


using SFML.System;


namespace robot_nagivation
{
    public class AStarAgent : Agent
    {
        /*  Depth-First Search Specific Items  */
        private List<Node<TileType>> _frontierList;
        private HashSet<Node<TileType>> _searchedNodes;

        public AStarAgent()
        {
            _frontierList = new List<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _frontierList.Add(AgentData.RootNode);
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

                    if (_frontierList.Count > 0)
                    {
                        Node<TileType> currentNode = _frontierList[_frontierList.Count - 1];
                        int lowestCost = 5000;
                        foreach (Node<TileType> listNode in _frontierList)
                        {
                            if (listNode.Cost < lowestCost)
                            {
                                lowestCost = listNode.Cost;
                                currentNode = listNode;
                            }
                        }

                        

                        if (IsGoalNode(currentNode))
                        {
                            State = AgentState.Moving;

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode);
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                            State = AgentState.Moving;
                            AgentData.PosToSearch.Clear();
                            break;
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
                                _frontierList.Add(potentialNode);
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
        private List<Node<TileType>> _frontierList;

        public GreedyFirstAgent()
        {
            _frontierList = new List<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _frontierList.Add(AgentData.RootNode);
        }

        public override int HeuristicFunction(Vector2i Pos, Percepts percepts)
        {

            int manhattanDistance = 0;

            // Manhanttan Distance to the FIRST goal

            manhattanDistance = Math.Abs(percepts.GoalPositions[0].X - Pos.X) +
                Math.Abs(percepts.GoalPositions[0].Y - Pos.Y);

            return manhattanDistance;
        }

        public override AgentActions next(Percepts percepts)
        {

            AgentDelay--;
            if (AgentDelay < 0)
            {
                AgentDelay = 20;
            }
            else
            {
                return AgentActions.Search;
            }

            UpdateInternalHeap(_frontierList);

            switch (State)
            {
                case AgentState.Searching:

                    if (_frontierList.Count > 0)
                    {
                        Node<TileType> currentNode = _frontierList[_frontierList.Count - 1];
                        int lowestCost = 5000;
                        foreach (Node<TileType> listNode in _frontierList)
                        {
                            if (listNode.Cost < lowestCost)
                            {
                                lowestCost = listNode.Cost;
                                currentNode = listNode;
                            }
                        }

                        _frontierList.Remove(currentNode);

                        if (IsGoalNode(currentNode))
                        {
                            State = AgentState.Moving;

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode);
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                            State = AgentState.Moving;
                            AgentData.PosToSearch.Clear();
                            break;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();
                        AgentData.PosToSearch.Add(currentNode.Pos);
                        AgentData.SearchedNodes.Add(currentNode);


                        List<Node<TileType>> bestNodes = new List<Node<TileType>>();
                        int lowestLocalCost = 50000; // some high cost or cost of first child node.


                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);

                        for (int i = 0; i < surroundingNodes.Count; i++)
                        {
                            Node<TileType> subnode = surroundingNodes[i];

                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {

                                if (subnode.Cost <= lowestLocalCost)
                                {
                                    lowestLocalCost = subnode.Cost;

                                    bestNodes.Add(subnode);
                                }

                                AgentData.SearchedPos.Add(subnode.Pos);
                                AgentData.SearchedNodes.Add(subnode);
                            }
                        }

                        foreach (Node<TileType> potentialNode in bestNodes)
                        {
                            if (potentialNode.Cost <= lowestLocalCost)
                            {
                                //of ALL the expanded nodes, these are the lowest
                                // we can compare with the stack cost?


                                _frontierList.Add(potentialNode);
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
