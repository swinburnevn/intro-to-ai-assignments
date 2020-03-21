using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace robot_nagivation
{
    public class Percepts
    {
        private int[] _currentPosition;
        private TileType[,] _map;
        private Vector2 _agentPos;

       
        public int[] CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public TileType[,] Map { get => _map; set => _map = value; }
        public Vector2 AgentPos { get => _agentPos; set => _agentPos = value; }
    }
    /*
     * Encapsulates the logic of the virtual world
     *  Contains Map, Agents, and provides agent with percepts
     *  
     * 
     */
    public interface IModel
    {
        public void Initialise();
        public void Run();
    }

    public class GameModel : IModel
    {
        private ProgramData _data;
        public GameModel(ref ProgramData data)
        {
            _data = data;
        }

        public void Initialise()
        {
            _data.AgentPositions.Add(_data.Map.AgentPos);
            _data.Agent.Initialise(CreatePercepts());
        }

        public bool WithinMap (Vector2 pos)
        {
            if ((0 <= pos.X) && (pos.X < _data.Map.MapMatrix.GetLength(0)))
                if ((0 <= pos.Y) && (pos.Y < _data.Map.MapMatrix.GetLength(1)))
                    if (_data.Map.MapMatrix[(int)pos.X, (int)pos.Y] != TileType.Wall)
                        return true;
                    
            return false;
        }

        public void MoveAgent(AgentActions direction)
        {
            Vector2 newPos;

            switch (direction)
            {
                case AgentActions.Up:
                    newPos = new Vector2(_data.Map.AgentPos.X, _data.Map.AgentPos.Y - 1);
                    if (WithinMap(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Down:
                    newPos = new Vector2(_data.Map.AgentPos.X, _data.Map.AgentPos.Y + 1);
                    if (WithinMap(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Left:
                    newPos = new Vector2(_data.Map.AgentPos.X - 1, _data.Map.AgentPos.Y);
                    if (WithinMap(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Right:
                    newPos = new Vector2(_data.Map.AgentPos.X + 1, _data.Map.AgentPos.Y);
                    if (WithinMap(newPos))
                        _data.Map.AgentPos = newPos;
                    break;
            }
        }

        public Percepts CreatePercepts()
        {
            Percepts percepts = new Percepts();
            percepts.Map = _data.Map.MapMatrix;
            percepts.AgentPos = _data.Map.AgentPos;
            return percepts;
        }
        public void Run()
        {
            _data.Steps++;


            Percepts percepts = CreatePercepts();
            
            AgentActions agentDecision = _data.Agent.next(percepts);

            _data.AgentDecisions.Add(agentDecision);
            _data.SearchedNodes = _data.Agent.GetSearchedNodes();
            
            MoveAgent(agentDecision);
            Console.WriteLine(agentDecision);

            _data.AgentPositions.Add(_data.Map.AgentPos);

            // Check if agent has reached goal state
            if (_data.Map.MapMatrix[(int)_data.Map.AgentPos.X, (int)_data.Map.AgentPos.Y] == TileType.Goal)
                _data.Finished = true;
            
        }
    }
}
