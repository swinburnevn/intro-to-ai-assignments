using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace tutorial_2
{


    public enum AgentActions
    {
        Left,
        Right,
        Up,
        Down,
        Clean,
        Standby
    }

    public enum SquareStatus
    {
        Clean,
        Dirty,
        Null
    }

    public enum SimpleState
    {
        SquareIsDirty,
        InSquareA,
        InSquareB
    }

    // Manipulates game data
    public interface IModel
    {
        public void Run();
    }

    public interface IAgent
    {
        public AgentActions next(Percepts percepts);
    }

    public class Percepts
    {
        private SquareStatus _currentSquareStatus;
        private int[] _currentPosition;
        private SquareStatus[,] _map;
        private int _lifetime;

        public SquareStatus CurrentSquareStatus { get => _currentSquareStatus; set => _currentSquareStatus = value; }
        public int[] CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public SquareStatus[,] Map { get => _map; set => _map = value; }
        public int Lifetime { get => _lifetime; set => _lifetime = value; }
    }

    public class GameModel : IModel
    {
        public void Run()
        {
            throw new NotImplementedException();
        }
    }



    public class ReflexAgent : IAgent
    {
        // A rulebook, state->action rules.
        private Dictionary<SimpleState, AgentActions> /* unlimited */ _rulebook;

        public AgentActions next(Percepts percepts)
        {
            SimpleState state = InterpretInput(percepts);
            return _rulebook[state];        }


        public SimpleState InterpretInput(Percepts percepts)
        {
            throw new NotImplementedException();
        }

        public Dictionary<SimpleState, AgentActions> Rulebook { set => _rulebook = value; }

    }





    public class ProgramData
    {
        private IAgent _agent;
        private Map _map;

        private int _lifetime;
        private bool _finished = false;

        List<AgentActions> _agentDecisions;

        public void Initilize(Map map)
        {
            _agent = new ReflexAgent();
            _map = map;
            _agentDecisions = new List<AgentActions>();
            _lifetime = 1000;
        }


        public IAgent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public bool Finished { get => _finished; set => _finished = value; }
        public List<AgentActions> AgentDecisions { get => _agentDecisions; set => _agentDecisions = value; }
        public int Lifetime { get => _lifetime; set => _lifetime = value; }
    }

}
