using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using SFML.System;

namespace robot_nagivation
{

    public enum AgentState
    {
        Searching,
        Moving,
        Lost,           // Cannot find path
        Finished,
        Idle
    }

    public abstract class Agent
    {
        private AgentData _agentData;   // Contains all the information used by the agent.
        private Percepts _percepts;     // Determines what the agent can "see".
        private AgentState _state;      // Determines what the agent is currently doing

        public virtual void Initialise(Percepts percepts)
        {
            _agentData = new AgentData();
            _percepts = percepts;
            _state = AgentState.Searching;

            AgentData.RootNode = new Node<TileType>(TileType.Start, null, percepts.AgentPos);

        }
        public abstract AgentActions next(Percepts percepts);


        #region Agent Virtual methods

        protected virtual bool WithinMap(Vector2i pos, Percepts percepts)
        {
            if ((0 <= pos.X) && (pos.X < percepts.MapMatrix.GetLength(0)))
                if ((0 <= pos.Y) && (pos.Y < percepts.MapMatrix.GetLength(1)))
                    if (percepts.MapMatrix[pos.X, pos.Y] != TileType.Wall)
                        return true;
            return false;
        }

        protected virtual List<Node<TileType>> SearchSurroundingNodes(Node<TileType> currentNode, Percepts percepts)
        {
            Vector2i[] searchNodes = new Vector2i[4]; // 4 surrounding nodes
            List<Node<TileType>> foundNodes = new List<Node<TileType>>();

            searchNodes[0] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y + 1);
            searchNodes[1] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y - 1);
            searchNodes[2] = new Vector2i(currentNode.Pos.X + 1, currentNode.Pos.Y);
            searchNodes[3] = new Vector2i(currentNode.Pos.X - 1, currentNode.Pos.Y);

            foreach (Vector2i newNode in searchNodes)
            {
                if (WithinMap(newNode, percepts))
                    foundNodes.Add(new Node<TileType>(
                        percepts.MapMatrix[newNode.X, newNode.Y], currentNode, newNode));
            }
            return foundNodes;
        }

        public virtual bool IsGoalNode(Node<TileType> node)
        {
            return (node.Data == TileType.Goal);
        }

        public virtual bool IsFinished()
        {
            return (_state == AgentState.Finished);
        }

        public Queue<AgentActions> DetermineMoveSet()
        {
            Queue<AgentActions> moveSet= new Queue<AgentActions>();
            for (int i = 0; i < AgentData.NodePath.Count - 1; i++)
            {
                Vector2i direction = AgentData.NodePath[i + 1].Pos - AgentData.NodePath[i].Pos;
                if (direction.X > 0)
                    moveSet.Enqueue(AgentActions.Right);
                if (direction.X < 0)
                    moveSet.Enqueue(AgentActions.Left);
                if (direction.Y > 0)
                    moveSet.Enqueue(AgentActions.Down);
                if (direction.Y < 0)
                    moveSet.Enqueue(AgentActions.Up);
            }

            return moveSet;
        }

        public List<Node<TileType>> DetermineAgentPath(Node<TileType> start, Node<TileType> end)
        {

            Node<TileType> currentNode = end;

            List < Node<TileType>> nodePath = new List<Node<TileType>>();

            while (currentNode != start)
            {
                nodePath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            nodePath.Reverse();
            return nodePath;
        }


        #endregion


        #region Agent Properties

        public AgentData AgentData { get => _agentData; set => _agentData = value; }
        public Percepts Percepts { get => _percepts; }
        public AgentState State { get => _state; set => _state = value; }

        #endregion

    }

    public class RandomAgent : Agent
    {

        public override AgentActions next(Percepts percepts)
        {
            Array possibleActions = Enum.GetValues(typeof(AgentActions));
            return (AgentActions)possibleActions.GetValue(new Random().Next(0, possibleActions.Length));
        }

    }

    public class BreadthFirstAgent : Agent
    {
        /*  Breadth-First Search Specific Items  */
        private Queue<Node<TileType>> _nodeQueue;
        private HashSet<Node<TileType>> _searchedNodes;

        public BreadthFirstAgent()
        {
            _nodeQueue = new Queue<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _nodeQueue = new Queue<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();

            _nodeQueue.Enqueue(AgentData.RootNode);
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

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode );
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

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


