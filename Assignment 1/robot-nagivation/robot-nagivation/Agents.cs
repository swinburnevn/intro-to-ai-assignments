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
        private Queue<AgentActions> _determinedMoveSet; // List of actions agent takes

        public virtual void Initialise(Percepts percepts)
        {
            _agentData = new AgentData();
            _percepts = percepts;
            _state = AgentState.Searching;
            _determinedMoveSet = new Queue<AgentActions>();
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
                        percepts.MapMatrix[newNode.X, newNode.Y], currentNode));
            }
            return foundNodes;
        }

        public virtual bool IsGoalNode(Node<TileType> node)
        {
            return (node.Data == TileType.Goal);
        }

        //Fix!
        public void InterpretCommands()
        {
            _determinedMoveSet = new Queue<AgentActions>();

            for (int i = 0; i < _nodePath.Count - 1; i++)
            {
                Vector2 direction = _nodePath[i + 1] - _nodePath[i];
                if (direction.X > 0)
                    _commandPath.Enqueue(AgentActions.Right);
                if (direction.X < 0)
                    _commandPath.Enqueue(AgentActions.Left);
                if (direction.Y > 0)
                    _commandPath.Enqueue(AgentActions.Down);
                if (direction.Y < 0)
                    _commandPath.Enqueue(AgentActions.Up);
            }
        }

        public void CreateAgentPath(Node<TileType> start, Node<TileType> end)
        {

            Node<TileType> currentNode = end;

            AgentData.NodePath = new List<Node<TileType>>();

            while (currentNode != start)
            {
                AgentData.NodePath.Add(currentNode);
                currentNode = currentNode.Parent;
                
            }
            AgentData.NodePath.Reverse();
        }


        #endregion


        #region Agent Properties

        public AgentData AgentData { get => _agentData; set => _agentData = value; }
        public Percepts Percepts { get => _percepts; }
        public AgentState State { get => _state; set => _state = value; }
        public Queue<AgentActions> DeterminedMoveSet { get => _determinedMoveSet; set => _determinedMoveSet = value; }

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

                            //Backtrack(currentNode, percepts.AgentPos);
                            //InterpretCommands();
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        foreach (Node<TileType> subnode in SearchSurroundingNodes(currentNode, percepts))
                        {
                            if (!_searchedNodes.Contains(subnode))
                            {
                                _nodeQueue.Enqueue(subnode);
                                _searchedNodes.Add(subnode);
                                AgentData.PosToSearch.Add(subnode.Pos);
                                AgentData.SeaarchedPos.Add(subnode.Pos);
                            }


                        }
                    }

                    break;


                case AgentState.Moving:

                    if (DeterminedMoveSet.Count > 0)
                        return DeterminedMoveSet.Dequeue();
                    State = AgentState.Finished;
                    break;

                default:
                    break;

            }

            return AgentActions.Idle;
        }

    }
}


