using System;
using System.Collections.Generic;
using System.Text;


using SFML.System;


namespace robot_nagivation
{
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

            


            return 0;
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

                        foreach (Node<TileType> subnode in SearchSurroundingNodes(currentNode, percepts))
                        {
                            if (!_searchedNodes.Contains(subnode))
                                if (!AgentData.SearchedPos.Contains(subnode.Pos))
                                {
                                    _nodeQueue.Enqueue(subnode);
                                    _searchedNodes.Add(subnode);
                                    AgentData.PosToSearch.Add(subnode.Pos);
                                    AgentData.SearchedPos.Add(subnode.Pos);
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
