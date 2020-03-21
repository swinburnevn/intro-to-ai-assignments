using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace robot_nagivation
{
    public class ProgramData
    {
        private IAgent _agent;
        private Map _map;
        private bool _finished;
        private List<AgentActions> _agentDecisions;
        private List<AgentActions> _foundPath;
        private List<Vector2> _agentPositions;
        private List<Vector2> _searchedNodes;

        private int _steps;

        

        public ProgramData()
        {
            _finished = false;
            _agentDecisions = new List<AgentActions>();
            _agentPositions = new List<Vector2>();
            _searchedNodes = new List<Vector2>();
            _steps = 0;
        }

        public IAgent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public List<AgentActions> AgentDecisions { get => _agentDecisions; set => _agentDecisions = value; }
        public bool Finished { get => _finished; set => _finished = value; }
        public int Steps { get => _steps; set => _steps = value; }
        public List<Vector2> AgentPositions { get => _agentPositions; set => _agentPositions = value; }
        public List<Vector2> SearchedNodes { get => _searchedNodes; set => _searchedNodes = value; }
    }


}
