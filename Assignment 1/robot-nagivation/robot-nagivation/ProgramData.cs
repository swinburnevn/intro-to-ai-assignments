using System;
using System.Collections.Generic;
using System.Text;

namespace robot_nagivation
{
    public class ProgramData
    {
        private IAgent _agent;
        private Map _map;
        private bool _finished;
        private List<AgentActions> _agentDecisions;

        

        public ProgramData()
        {
            _finished = false;
            _agentDecisions = new List<AgentActions>();
        }

        public IAgent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public List<AgentActions> AgentDecisions { get => _agentDecisions; set => _agentDecisions = value; }
        public bool Finished { get => _finished; set => _finished = value; }
        
    }


}
