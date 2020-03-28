using System;
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
        private Queue<AgentActions> _determinedMoveSet;      // Path the agent takes. Not altered after it is found.
        private List<Vector2i> _searchedPos;                // Searched positions
        private List<Vector2i> _posToSearch;                // Currently searching positions
        private List<Vector2i> _path;                       // List of positions used to get to the final 

        private Node<TileType> _rootNode;                   // The starting node

        private List<Node<TileType>> _nodePath;             // List of states required to get to end state.

        public int Steps { get => _determinedMoveSet.Count; }
        public Queue<AgentActions> DeterminedMoveSet { get => _determinedMoveSet; set => _determinedMoveSet = value; }
        public List<Vector2i> SearchedPos { get => _searchedPos; set => _searchedPos = value; }
        public List<Vector2i> PosToSearch { get => _posToSearch; set => _posToSearch = value; }
        public List<Node<TileType>> NodePath { get => _nodePath; set => _nodePath = value; }
        public Node<TileType> RootNode { get => _rootNode; set => _rootNode = value; }
        public List<Vector2i> Path { get => _path; set => _path = value; }

        public AgentData()
        {
            _determinedMoveSet = new Queue<AgentActions>();
            _searchedPos = new List<Vector2i>();
            _posToSearch = new List<Vector2i>();
            _path = new List<Vector2i>();
        }

    }
    public class ProgramData
    {
        private Agent _agent;
        private Map _map;
        private bool _finished;
        private List<AgentActions> _agentDecisions;

        public ProgramData()
        {
            _finished = false;
            _agentDecisions = new List<AgentActions>();
        }

        public Agent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public List<AgentActions> AgentDecisions { get => _agentDecisions; set => _agentDecisions = value; }
        public bool Finished { get => _finished; set => _finished = value; }
    }


}
