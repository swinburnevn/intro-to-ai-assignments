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
    }

    public class RandomAgent : IAgent
    {
        public List<Vector2> GetSearchedNodes()
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



    public class BreadthFirstAgent : SearchingAgent
    {
        private Queue<Vector2> _nodeQueue;
        private HashSet<Vector2> _searchedNodes;

        private List<Vector2> _listSearchedNodes;

        public BreadthFirstAgent()
        {
            _nodeQueue = new Queue<Vector2>();
            _searchedNodes = new HashSet<Vector2>();
            _listSearchedNodes = new List<Vector2>();
        }

        public override List<Vector2> GetSearchedNodes()
        {
            return _listSearchedNodes;
        }

        public override void Initialise(Percepts percepts)
        {
            _nodeQueue.Enqueue(percepts.AgentPos);
        }

        public override AgentActions next(Percepts percepts)
        {
            if (_nodeQueue.Count > 0)
            {
                Vector2 currentNode = _nodeQueue.Dequeue();
                if (IsGoalNode(currentNode, percepts))
                {
                    Console.WriteLine("at goal node need event handler of some sort");
                }



                foreach (Vector2 subnode in SearchSurroundingNodes(currentNode, percepts))
                {
                    if (!_searchedNodes.Contains(subnode))
                    {
                        _nodeQueue.Enqueue(subnode);
                        _searchedNodes.Add(subnode);
                        _listSearchedNodes.Add(subnode);
                    }
                }
            }
            

            return AgentActions.Search;
        }

    }



}


