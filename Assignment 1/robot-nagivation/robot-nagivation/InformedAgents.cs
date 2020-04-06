using System;
using System.Collections.Generic;
using System.Text;


using SFML.System;


namespace robot_nagivation
{

    public class AStarAgent : GreedyFirstAgent
    {

        public override void Initialise(Percepts percepts)
        {
            

            base.Initialise(percepts);

            Name = "A-Star Agent";
        }

        public override int CostFunction(Vector2i Pos, Percepts percepts)
        {

            // Get the current latest node
            // GBFS should only expand one node at a time...
            Node<TileType> currentNode = AgentData.SearchedNodes[AgentData.SearchedNodes.Count - 1];

            // Determine layers of parents
            //  This gives us the "cost" to get up to this point
            int nodeDistance = 0;
            Node<TileType> parentNode = currentNode.Parent;
            while (parentNode != null)
            {
                nodeDistance++;
                parentNode = parentNode.Parent;
            }

            return nodeDistance;

        }


    }

    public class GreedyFirstAgent : Agent
    {
        /*  Geedy-First Search Specific Items  */
        private List<Node<TileType>> _frontierList;

        public GreedyFirstAgent()
        {
            _frontierList = new List<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            Name = "Greedy-Best First Agent";

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
                AgentDelay = AgentData.AgentDelay;
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

                        // This is reversed such that it provides a DFS under all equal circumstances
                        //  this implies in a worst-case scenario, A* behaves as  GBFS
                        for (int i = _frontierList.Count - 1; i >= 0; i--)
                        {
                            Node<TileType> listNode = _frontierList[i];
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

                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);

                        for (int i = 0; i < surroundingNodes.Count; i++)
                        {
                            Node<TileType> subnode = surroundingNodes[i];

                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {
                                _frontierList.Add(subnode);
                                
                                AgentData.SearchedPos.Add(subnode.Pos);
                                AgentData.SearchedNodes.Add(subnode);
                            }
                        }



                    } 
                    else
                    {
                        State = AgentState.Lost;
                        return AgentActions.Lost;
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

            return AgentActions.Search;
        }
    }
}
