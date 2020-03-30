using System;
using System.Collections.Generic;
using System.Text;

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
        private AgentState _state;      // Determines what the agent is currently doing

        public virtual void Initialise(Percepts percepts)
        {
            _agentData = new AgentData();
            _state = AgentState.Searching;

            AgentData.RootNode = new Node<TileType>(TileType.Start, null, percepts.AgentPos);
            AgentData.RootNode.IsOnPath = true;
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

        public virtual int HeuristicFunction(Vector2i Pos, Percepts percepts)
        {
            return 0;
        }
        protected virtual List<Node<TileType>> SearchSurroundingNodes(Node<TileType> currentNode, Percepts percepts)
        {
            Vector2i[] searchNodes = new Vector2i[4]; // 4 surrounding nodes
            List<Node<TileType>> foundNodes = new List<Node<TileType>>();

            // Priorities: Goes UP before LEFT then DOWN then RIGHT
            searchNodes[3] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y - 1);
            searchNodes[2] = new Vector2i(currentNode.Pos.X - 1, currentNode.Pos.Y);
            searchNodes[1] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y + 1);
            searchNodes[0] = new Vector2i(currentNode.Pos.X + 1, currentNode.Pos.Y);
            

            foreach (Vector2i newPos in searchNodes)
            {
                if (WithinMap(newPos, percepts))
                {
                    Node<TileType> newNode = new Node<TileType>(
                        percepts.MapMatrix[newPos.X, newPos.Y], currentNode, newPos);
                    newNode.Cost = HeuristicFunction(newPos, percepts);
                    foundNodes.Add(newNode);
                    currentNode.Children.Add(newNode);
                }
                    
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
                currentNode.IsOnPath = true;
                nodePath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            nodePath.Add(start);
            nodePath.Reverse();
            return nodePath;
        }

        /*
        public List<Vector2i> DetermineAgentPosPath(List<Node<TileType>> nodePath)
        {
            List<Vector2i> posPath = new List<Vector2i>();
            foreach (Node<TileType> node in nodePath)
            {
                posPath.Add(node.Pos);
            }

            return posPath;
        }
        */

        #endregion


        #region Agent Properties

        public AgentData AgentData { get => _agentData; set => _agentData = value; }
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

    public class DepthFirstAgent : Agent
    {

        /*  Depth-First Search Specific Items  */
        private Stack<Node<TileType>> _nodeStack;
        private HashSet<Node<TileType>> _searchedNodes;

        public DepthFirstAgent()
        {
            _nodeStack = new Stack<Node<TileType>>();
            _searchedNodes = new HashSet<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            _nodeStack.Push(AgentData.RootNode);
        }

        public override AgentActions next(Percepts percepts)
        {

            switch (State)
            {
                case AgentState.Searching:

                    if (_nodeStack.Count > 0)
                    {
                        Node<TileType> currentNode = _nodeStack.Pop();

                        if (IsGoalNode(currentNode))
                        {
                            State = AgentState.Moving;

                            AgentData.NodePath = DetermineAgentPath(AgentData.RootNode, currentNode);
                            AgentData.DeterminedMoveSet = DetermineMoveSet();
                            State = AgentState.Moving;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.PosToSearch.Add(currentNode.Pos);
                        AgentData.SearchedPos.Add(currentNode.Pos);
                        _searchedNodes.Add(currentNode);
                        AgentData.SearchedNodes.Add(currentNode);

                        foreach (Node<TileType> subnode in SearchSurroundingNodes(currentNode, percepts))
                        {
                            if (!_searchedNodes.Contains(subnode))
                                if (!AgentData.SearchedPos.Contains(subnode.Pos))
                                {
                                    _nodeStack.Push(subnode);

                                    
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
                            //AgentData.Path = DetermineAgentPosPath(AgentData.NodePath);
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


