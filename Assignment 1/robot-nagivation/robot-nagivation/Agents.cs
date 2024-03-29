﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using C5;

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
        private string _name = "You shouldn't be seeing this";


        private int _agentDelay = 0;
        public virtual void Initialise(Percepts percepts)
        {
            _agentData = new AgentData();
            _state = AgentState.Searching;

            AgentData.RootNode = new Node<TileType>(TileType.Start, null, percepts.AgentPos);
            AgentData.RootNode.IsOnPath = true;
            AgentData.SearchedPos.Add(AgentData.RootNode.Pos);
        }
        public abstract AgentActions next(Percepts percepts);


        #region Agent Virtual methods

        public virtual void UpdateInternalHeap(Queue<Node<TileType>> list)
        {
            AgentData.InternalHeap = new List<Node<TileType>>();
            foreach (Node<TileType> node in list)
            {
                AgentData.InternalHeap.Add(node);
            }
        }

        public virtual void UpdateInternalHeap(Stack<Node<TileType>> list)
        {
            AgentData.InternalHeap = new List<Node<TileType>>();
            foreach (Node<TileType> node in list)
            {
                AgentData.InternalHeap.Add(node);
            }
        }

        public virtual void UpdateInternalHeap(List<Node<TileType>> list)
        {
            AgentData.InternalHeap = new List<Node<TileType>>(list);
        }

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

        public virtual int CostFunction(Vector2i Pos, Percepts percepts)
        {
            return 0;
        }

        protected virtual List<Node<TileType>> SearchSurroundingNodes(Node<TileType> currentNode, Percepts percepts)
        {
            Vector2i[] searchNodes = new Vector2i[4]; // 4 surrounding nodes
            List<Node<TileType>> foundNodes = new List<Node<TileType>>();

            // Priorities: Goes UP before LEFT then DOWN then RIGHT
            searchNodes[0] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y - 1);
            searchNodes[1] = new Vector2i(currentNode.Pos.X - 1, currentNode.Pos.Y);
            searchNodes[2] = new Vector2i(currentNode.Pos.X, currentNode.Pos.Y + 1);
            searchNodes[3] = new Vector2i(currentNode.Pos.X + 1, currentNode.Pos.Y);
            


            for (int i = 0; i < searchNodes.Length; i++)
            {
                Vector2i newPos = searchNodes[i];
                if (WithinMap(newPos, percepts))
                {
                    if (!AgentData.SearchedPos.Contains(newPos))
                    {
                        Node<TileType> newNode = new Node<TileType>(
                                                percepts.MapMatrix[newPos.X, newPos.Y], currentNode, newPos);
                        newNode.Cost = HeuristicFunction(newPos, percepts) + CostFunction(newPos, percepts);
                        if (AgentData.DirectionalMovementCost.Count != 0)
                        {
                            newNode.Cost += AgentData.DirectionalMovementCost[newPos - currentNode.Pos];
                        }
                        foundNodes.Add(newNode);
                        currentNode.Children.Add(newNode);
                    }
                        
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
            return ((_state == AgentState.Finished) || (_state == AgentState.Lost));
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

        #endregion


        #region Agent Properties

        public AgentData AgentData { get => _agentData; set => _agentData = value; }
        public AgentState State { get => _state; set => _state = value; }
        public int AgentDelay { get => _agentDelay; set => _agentDelay = value; }
        public string Name { get => _name; set => _name = value; }

        #endregion

    }

    public class RandomAgent : Agent
    {

        public override void Initialise(Percepts percepts)
        {
            Name = "Random Agent";

            base.Initialise(percepts);
        }

        public override AgentActions next(Percepts percepts)
        {
            Array possibleActions = Enum.GetValues(typeof(AgentActions));
            return (AgentActions)possibleActions.GetValue(new Random().Next(0, possibleActions.Length));
            
        }

    }

    public class NodeComparer : IComparer<Node<TileType>>
    {
        public int Compare([AllowNull] Node<TileType> x, [AllowNull] Node<TileType> y)
        {
            return x.Cost - y.Cost;
        }
    }

    public class UniformCostAgent : Agent
    {
        private IntervalHeap<Node<TileType>> _priorityQueue;

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            Name = "Uniform Cost Agent";

            _priorityQueue = new IntervalHeap<Node<TileType>>(new NodeComparer());
            _priorityQueue.Add(AgentData.RootNode);
        }

        public override int CostFunction(Vector2i Pos, Percepts percepts)
        {

            // Get the current latest node
            // GBFS should only expand one node at a time...


            if (AgentData.SearchedNodes.Count < 1)
            {
                return 1;
            }

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

        public void UpdateInternalHeap(IntervalHeap<Node<TileType>> priorityQueue)
        {
            AgentData.InternalHeap = new List<Node<TileType>>();
            foreach (Node<TileType> node in priorityQueue)
            {
                AgentData.InternalHeap.Add(node);
            }
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

           UpdateInternalHeap(_priorityQueue);

            switch (State)
            {
                case AgentState.Searching:

                    if (_priorityQueue.Count > 0)
                    {
                        Node<TileType> currentNode = _priorityQueue.FindMin();
                        _priorityQueue.DeleteMin();

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

                        AgentData.PosToSearch.Add(currentNode.Pos); // frontier nodes, yellow.
                        AgentData.SearchedPos.Add(currentNode.Pos); // important: searched positions


                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);


                        // The order of the surrounding nodes is in:
                        //  Up -> Left -> Down -> Right
                        // However, since we're pushing it into the stack in that order,
                        //  the first one that's popped is the "right" one. let's flip the order here...
                        for (int i = surroundingNodes.Count - 1; i >= 0; i--)
                        {
                            Node<TileType> subnode = surroundingNodes[i];
                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {
                                _priorityQueue.Add(subnode);

                                AgentData.SearchedNodes.Add(subnode);   // for the node tree, searched nodes.
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

    public class IterativeDDFSAgent : DepthFirstAgent
    {
        private int _targetDepth = 1;
        private int _currentDepth = 0;

        private int _leadingDepth;

        public override void Initialise(Percepts percepts)
        {
            base.Initialise(percepts);

            Name = "Iterative Deepening DFS Agent";

        }

        public int DepthFunction(Node<TileType> currentNode)
        {

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

            

            UpdateInternalHeap(_nodeStack);

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
                            AgentData.PosToSearch.Clear();
                            break;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.PosToSearch.Add(currentNode.Pos); // frontier nodes, yellow.
                        AgentData.SearchedPos.Add(currentNode.Pos); // important: searched positions


                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);

                        // The order of the surrounding nodes is in:
                        //  Up -> Left -> Down -> Right
                        // However, since we're pushing it into the stack in that order,
                        //  the first one that's popped is the "right" one. let's flip the order here...

                        

                        for (int i = surroundingNodes.Count - 1; i >= 0; i--)
                        {
                            

                            Node<TileType> subnode = surroundingNodes[i];
                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {
                                _currentDepth = DepthFunction(subnode);

                                if (_currentDepth > _leadingDepth)
                                    _leadingDepth = _currentDepth;
                                if (_currentDepth <  _targetDepth)
                                {
                                    _nodeStack.Push(subnode);

                                    AgentData.SearchedNodes.Add(subnode);   // for the node tree, searched nodes.
                                }
                                
                                
                            }
                        }
                    }
                    else
                    {
                        if (_leadingDepth < _targetDepth)
                        {
                            State = AgentState.Lost;
                            return AgentActions.Lost;
                        }


                        _targetDepth++;
                        _nodeStack.Push(AgentData.RootNode);
                        AgentData.RootNode.Children.Clear();
                        AgentData.SearchedNodes.Clear();
                        AgentData.SearchedPos.Clear();


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

    public class DepthFirstAgent : Agent
    {

        /*  Depth-First Search Specific Items  */
        protected Stack<Node<TileType>> _nodeStack;

        public DepthFirstAgent()
        {
            _nodeStack = new Stack<Node<TileType>>();
        }


        public override void Initialise(Percepts percepts)
        {
            Name = "Depth-First Agent";

            base.Initialise(percepts);

            _nodeStack.Push(AgentData.RootNode);
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

            UpdateInternalHeap(_nodeStack);

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
                            AgentData.PosToSearch.Clear();
                            break;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.PosToSearch.Add(currentNode.Pos); // frontier nodes, yellow.
                        AgentData.SearchedPos.Add(currentNode.Pos); // important: searched positions
                        

                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);


                        // The order of the surrounding nodes is in:
                        //  Up -> Left -> Down -> Right
                        // However, since we're pushing it into the stack in that order,
                        //  the first one that's popped is the "right" one. let's flip the order here...
                        for (int i = surroundingNodes.Count - 1; i >= 0; i--)
                        {
                            Node<TileType> subnode = surroundingNodes[i];
                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {
                                _nodeStack.Push(subnode);

                                AgentData.SearchedNodes.Add(subnode);   // for the node tree, searched nodes.
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

    public class BreadthFirstAgent : Agent
    {
        /*  Breadth-First Search Specific Items  */
        private Queue<Node<TileType>> _nodeQueue;

        public BreadthFirstAgent()
        {
            _nodeQueue = new Queue<Node<TileType>>();
        }

        public override void Initialise(Percepts percepts)
        {
            Name = "Breadth-First Agent";

            base.Initialise(percepts);

            _nodeQueue.Enqueue(AgentData.RootNode);
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

            UpdateInternalHeap(_nodeQueue);

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
                            AgentData.PosToSearch.Clear();
                            break;
                        }

                        AgentData.PosToSearch = new List<Vector2i>();

                        AgentData.PosToSearch.Add(currentNode.Pos);


                        List<Node<TileType>> surroundingNodes = SearchSurroundingNodes(currentNode, percepts);

                        for (int i = 0; i < surroundingNodes.Count; i++)
                        {
                            Node<TileType> subnode = surroundingNodes[i];

                            if (!AgentData.SearchedPos.Contains(subnode.Pos))
                            {
                                _nodeQueue.Enqueue(subnode);
                                
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


