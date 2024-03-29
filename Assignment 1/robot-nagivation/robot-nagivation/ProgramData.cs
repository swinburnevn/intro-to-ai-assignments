﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using SFML.System;

namespace robot_nagivation
{


    /*
     * Used to pass information between the agent and the program,
     *  This is the empty data object that the agent manipulates ot acheive the goal
     *  
     * Requesting this allows us to understand what the agent is "thinking" and to visualise
     *  the search pattern of the agent.
     *  
     *  A key difference here is POS vs NODE
     *   A pos is a Vector2i representing a position with no additional information
     *   A Node is the agent's association with that state -- whether it be parent node, actual pos, type...
     *   
     *   In other words, Node is a possible state of an agent.
     * 
     */
    public class AgentData
    {
        private Queue<AgentActions> _determinedMoveSet;     // Path the agent takes. Not altered after it is found.
        private List<Vector2i> _searchedPos;                // Searched positions
        private List<Vector2i> _posToSearch;                // Currently searching positions
        private List<Vector2i> _path;                       // List of positions used to get to the final 

        private Node<TileType> _rootNode;                   // The starting node

        private List<Node<TileType>> _internalHeap;         // Agent internal list, whether stack or Queue

        private List<Node<TileType>> _nodePath;             // List of states required to get to end state.
        private List<Node<TileType>> _searchedNodes;             // List of states required to get to end state.

        private Dictionary<Vector2i, int> _directionalMovementCost;

        private int _agentDelay = 0;

        public int Steps { get => _determinedMoveSet.Count; }
        public Queue<AgentActions> DeterminedMoveSet { get => _determinedMoveSet; set => _determinedMoveSet = value; }
        public List<Vector2i> SearchedPos { get => _searchedPos; set => _searchedPos = value; }
        public List<Vector2i> PosToSearch { get => _posToSearch; set => _posToSearch = value; }
        public List<Node<TileType>> NodePath { get => _nodePath; set => _nodePath = value; }
        public Node<TileType> RootNode { get => _rootNode; set => _rootNode = value; }
        public List<Vector2i> Path { get => _path; set => _path = value; }
        public List<Node<TileType>> SearchedNodes { get => _searchedNodes; set => _searchedNodes = value; }
        public List<Node<TileType>> InternalHeap { get => _internalHeap; set => _internalHeap = value; }
        public int AgentDelay { get => _agentDelay; set => _agentDelay = value; }
        public Dictionary<Vector2i, int> DirectionalMovementCost { get => _directionalMovementCost; set => _directionalMovementCost = value; }

        public AgentData()
        {
            _determinedMoveSet = new Queue<AgentActions>();
            _searchedPos = new List<Vector2i>();
            _posToSearch = new List<Vector2i>();
            _searchedNodes = new List<Node<TileType>>();
            _path = new List<Vector2i>();
            _internalHeap = new List<Node<TileType>>();
            _directionalMovementCost = new Dictionary<Vector2i, int>();
        }

    }
    public class ProgramData
    {
        private Agent _agent;
        private Map _map;
        private bool _finished;
        private bool _windowRequestClosed;
        private List<AgentActions> _agentDecisions;

        string _outputName;

        private bool _displayMode = false;

        public ProgramData()
        {
            _finished = false;
            _windowRequestClosed = false;
            _agentDecisions = new List<AgentActions>();
        }

        public Agent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public List<AgentActions> AgentDecisions { get => _agentDecisions; set => _agentDecisions = value; }
        public bool Finished { get => _finished; set => _finished = value; }
        public bool WindowRequestClosed { get => _windowRequestClosed; set => _windowRequestClosed = value; }
        public bool DisplayMode { get => _displayMode; set => _displayMode = value; }
        public string OutputName { get => _outputName; set => _outputName = value; }
    }


}
