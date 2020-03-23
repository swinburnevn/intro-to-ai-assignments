using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace robot_nagivation
{
    public interface IAgent
    {
        public AgentActions next(Percepts percepts);
        public void Initialise(Percepts percepts);

        public List<Vector2> GetSearchedNodes();
        public List<Vector2> GetFrontierNodes();
        public List<Vector2> GetPath();
         

    }

    public abstract class SearchingAgent : IAgent
    {
        public abstract void Initialise(Percepts percepts);
        public abstract AgentActions next(Percepts percepts);

        public virtual bool WithinMap(Vector2 pos, Percepts percepts)
        {
            if ((0 <= pos.X) && (pos.X < percepts.Map.GetLength(0)))
                if ((0 <= pos.Y) && (pos.Y < percepts.Map.GetLength(1)))
                    if (percepts.Map[(int)pos.X, (int)pos.Y] != TileType.Wall)
                        return true;
            return false;
        }

        public virtual List<Vector2> SearchSurroundingNodes(Vector2 currentNode, Percepts percepts)
        {
            Vector2[] searchNodes = new Vector2[4]; // 4 surrounding nodes
            List<Vector2> foundNodes = new List<Vector2>();

            searchNodes[0] = new Vector2(currentNode.X, currentNode.Y+1);
            searchNodes[1] = new Vector2(currentNode.X, currentNode.Y-1);
            searchNodes[2] = new Vector2(currentNode.X+1, currentNode.Y);
            searchNodes[3] = new Vector2(currentNode.X-1, currentNode.Y);

            foreach (Vector2 node in searchNodes)
            {
                if (WithinMap(node, percepts))
                    foundNodes.Add(node);
            }
            return foundNodes;
        }

        public virtual bool IsGoalNode(Vector2 node, Percepts percepts)
        {
            if (percepts.Map[(int)node.X, (int)node.Y] == TileType.Goal)
                return true;
            return false;
        }

        public abstract List<Vector2> GetSearchedNodes();
        public abstract List<Vector2> GetFrontierNodes();
        public abstract List<Vector2> GetPath();
    }

    public class RandomAgent : IAgent
    {
        public List<Vector2> GetSearchedNodes()
        {
            return new List<Vector2>();
        }
        public List<Vector2> GetPath()
        {
            return new List<Vector2>();
        }
        public List<Vector2> GetFrontierNodes()
        {
            return new List<Vector2>();
        }

        public void Initialise(Percepts percepts)
        {
        }

        public AgentActions next(Percepts percepts)
        {
            Array possibleActions = Enum.GetValues(typeof(AgentActions));
            return (AgentActions)possibleActions.GetValue(new Random().Next(0, possibleActions.Length));
        }

    }

    public class Node
    {
        Vector2 _pos;
        List<Vector2> _prevNodes;

        public Vector2 Pos { get => _pos; set => _pos = value; }
        public List<Vector2> PrevNodes { get => _prevNodes; set => _prevNodes = value; }
    }

    public class BreadthFirstAgent : SearchingAgent
    {
        private Queue<Vector2> _nodeQueue;
        private HashSet<Vector2> _searchedNodes;

        private List<Vector2> _listSearchedNodes;
        private List<Vector2> _frontierNodes;

        private List<Vector2> _nodePath;
        private Queue<AgentActions> _commandPath;

        private Vector2[,] _internalMap;

        private bool _foundGoal;

        public bool FoundGoal { get => _foundGoal; set => _foundGoal = value; }

        public BreadthFirstAgent()
        {
            _nodeQueue = new Queue<Vector2>();
            _searchedNodes = new HashSet<Vector2>();
            _listSearchedNodes = new List<Vector2>();
            _frontierNodes = new List<Vector2>();
            _nodePath = new List<Vector2>();
            _foundGoal = false;

        }

        public override List<Vector2> GetSearchedNodes()
        {
            return _listSearchedNodes;
        }public override List<Vector2> GetFrontierNodes()
        {
            return _frontierNodes;
        }

        public override void Initialise(Percepts percepts)
        {
            _nodeQueue.Enqueue(percepts.AgentPos);
            _internalMap = new Vector2[percepts.Map.GetLength(0), percepts.Map.GetLength(1)];
        }

        public void InterpretCommands()
        {
            _commandPath = new Queue<AgentActions>();

            for(int i = 0; i < _nodePath.Count - 1; i++)
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

        public void Backtrack(Vector2 start, Vector2 end)
        {
            bool finished = false;
            Vector2 currentNode = start;
            _nodePath.Add(start);
            while (!finished)
            {
                Vector2 nextNode = _internalMap[(int)currentNode.X, (int)currentNode.Y];
                _nodePath.Add(nextNode);
                currentNode = nextNode;

                if (currentNode == end)
                    finished = true;
            }
            _nodePath.Reverse();
        }

        public override AgentActions next(Percepts percepts)
        {
            if (_foundGoal)
            {
                if (_commandPath.Count > 0)
                    return _commandPath.Dequeue();
            } 
            else
            {
                if (_nodeQueue.Count > 0)
                {
                    Vector2 currentNode = _nodeQueue.Dequeue();
                    if (IsGoalNode(currentNode, percepts))
                    {
                        _foundGoal = true;
                        Backtrack(currentNode, percepts.AgentPos);
                        InterpretCommands();
                        Console.WriteLine("Found Goal, setting internal flag to true");
                    }

                    _frontierNodes = new List<Vector2>();


                    foreach (Vector2 subnode in SearchSurroundingNodes(currentNode, percepts))
                    {
                        if (true)
                        {

                            _internalMap[(int)subnode.X, (int)subnode.Y] = currentNode;
                            _nodeQueue.Enqueue(subnode);
                            _searchedNodes.Add(subnode);
                            _listSearchedNodes.Add(subnode);
                            _frontierNodes.Add(subnode);
                        }


                    }
                }

                return AgentActions.Search;

            }
            
            

            return AgentActions.Idle;
        }

        public override List<Vector2> GetPath()
        {
            return _nodePath;
        }
    }



}


