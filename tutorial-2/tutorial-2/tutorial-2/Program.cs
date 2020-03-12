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
    public interface IAgentStrategy
    {
        abstract AgentActions next(Percepts percepts);

    }

    public class AgentRandomStrategy : IAgentStrategy
    {
        public AgentActions next(Percepts percepts)
        {
            Array possibleActions = Enum.GetValues(typeof(AgentActions));
            return (AgentActions)possibleActions.GetValue(new Random().Next(0, possibleActions.Length));
        }
    }

    public class Percepts
    {
        SquareStatus _currentSquareStatus;
        int[] _currentPosition;
        SquareStatus[,] _map;
        int _lifetime;
        /*
        public Percepts(Map map)
        {
            _currentPosition = map.AgentPos;
            _map = map.MapMatrix;
            _currentSquareStatus = map.MapMatrix[map.AgentPos[0], map.AgentPos[1]];
            _lifetime = 1000;
        }
        */

        public SquareStatus CurrentSquareStatus { get => _currentSquareStatus; set => _currentSquareStatus = value; }
        public int[] CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public SquareStatus[,] Map { get => _map; set => _map = value; }
        public int Lifetime { get => _lifetime; set => _lifetime = value; }
    }

    public interface IAgent
    {
        public AgentActions next(Percepts percepts);
        public void SetAgentStrategy(IAgentStrategy strategy);
    }


    public class CleaningAgent: IAgent
    {
        private IAgentStrategy _agentStrategy;

        public AgentActions next(Percepts percepts)
        {
            return AgentStategy.next(percepts);
        }

        public void SetAgentStrategy(IAgentStrategy strategy)
        {
            _agentStrategy = strategy;
        }

        public IAgentStrategy AgentStategy { get => _agentStrategy; set => _agentStrategy = value; }
    }


    public enum SquareStatus
    {
        Clean,
        Dirty,
        Null
    }
    public class Map
    {
        private int[] _size;
        private int[] _agentPos;
        private SquareStatus[,] _map;



        public Map(int[] size)
        {
            _size = size;
            _map = new SquareStatus[size[0], size[1]];
            _agentPos = new int[2] { 0, 0 };

        }

        public Map(int x, int y)
            : this (new int[] { x, y })
        {
        }


        public int[] Size { get => _size; }
        public SquareStatus[,] MapMatrix { get => _map; set => _map = value; }
        public int[] AgentPos { get => _agentPos; set => _agentPos = value; }
    }

    public interface IView
    {
        abstract void Draw();
    }

    public class ConsoleView : IView
    {

        private ProgramData _data;

        private Dictionary<SquareStatus, Char> _consoleDisplayValuePairs = new Dictionary<SquareStatus, Char>();


        public ConsoleView(ref ProgramData data)
        {
            _data = data;

            _consoleDisplayValuePairs.Add(SquareStatus.Clean, '.');
            _consoleDisplayValuePairs.Add(SquareStatus.Dirty, ':');
            _consoleDisplayValuePairs.Add(SquareStatus.Null, '#');
        }
        public void Draw()
        {

            Console.Clear();
            Console.WriteLine("+-----------------------------------------+");

            for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
            {
                Console.Write("  |");

                for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
                {

                    if ((_data.Map.AgentPos[0] == x) && (_data.Map.AgentPos[1] == y))
                    {
                        Console.Write("A");
                    } 
                    else
                    {
                        Console.Write(" ");
                    }

                    Console.Write(_consoleDisplayValuePairs[_data.Map.MapMatrix[x, y]] + "|");

                    
                }

                Console.WriteLine();
            }

            Console.WriteLine("+-----------------------------------------+");


            if (_data.AgentDecisions.Count  > 0)
            {
                Console.WriteLine(" Agent has decided: " + _data.AgentDecisions[_data.AgentDecisions.Count - 1]);
                Console.WriteLine(" Agent Score: " + _data.Lifetime);
                Console.WriteLine(" Agent Position: x: " + _data.Map.AgentPos[0] + ", y:" + +_data.Map.AgentPos[1]);
            }
            


        }
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
            _agent = new CleaningAgent();
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


    public interface IMapParser
    {
        public Map ReadMapFromFile(string filename);
    }

    public class MapParser : IMapParser
    {
        public Map ReadMapFromFile(string filename)
        {
            StreamReader _reader = new StreamReader(filename);

            string line = _reader.ReadLine();               // read first line: map size
            string[] sizes = Regex.Split(line, @"\D+");     // Returns string[4] with sizes in 1 and 2
            Map _readMap = new Map(int.Parse(sizes[1]), int.Parse(sizes[2]));


            line = _reader.ReadLine();                      // read next line: agent pos
            string[] agentPos = Regex.Split(line, @"\D+");  // Returns string[4] with pos in 1 and 2
            _readMap.AgentPos = new int[] { int.Parse(agentPos[1]), int.Parse(agentPos[2]) };

            // rest of lines are in the form [x, y] status
            while (!_reader.EndOfStream)
            {
                line = _reader.ReadLine();

                //failsafe
                if (line == "")
                    break;

                string[] location = Regex.Split(line, @"\D+");
                string status = Regex.Match(line, @"\b(clean|dirty|null)\b").Value;

                _readMap.MapMatrix[int.Parse(location[1]), int.Parse(location[2])] 
                    = (SquareStatus)Enum.Parse(typeof(SquareStatus), status, true);
            }
            _reader.Close();
            return _readMap;

        }
    }


    // Manipulates game data
    public interface IModel
    {
        public void Run();
    }

    public class GameModel : IModel
    {
        ProgramData _data;
        public GameModel(ref ProgramData data)
        {
            _data = data;
        }

        public Percepts NewPercept()
        {
            return new Percepts()
            {
                CurrentPosition = _data.Map.AgentPos,
                Map = _data.Map.MapMatrix,
                CurrentSquareStatus = _data.Map.MapMatrix[_data.Map.AgentPos[0], _data.Map.AgentPos[1]],
                Lifetime = _data.Lifetime
            };
        }
        public void Run()
        {
            //Update agent percepts
            Percepts currentPercepts = NewPercept();

            //Decide on what to do
            AgentActions currentDecision = _data.Agent.next(currentPercepts);
            _data.AgentDecisions.Add(currentDecision);


            //Update World + Percepts
            switch (currentDecision)
            {
                case AgentActions.Down:
                    MoveAgent(currentDecision);
                    break;

                case AgentActions.Up:
                    MoveAgent(currentDecision);
                    break;

                case AgentActions.Left:
                    MoveAgent(currentDecision);
                    break;

                case AgentActions.Right:
                    MoveAgent(currentDecision);
                    break;

                case AgentActions.Clean:
                    CleanCurrentTile();
                    break;

            }


            _data.Lifetime--;


        }

        public void CleanCurrentTile()
        {
            if (_data.Map.MapMatrix[_data.Map.AgentPos[0], _data.Map.AgentPos[1]] == SquareStatus.Dirty)
            {
                _data.Map.MapMatrix[_data.Map.AgentPos[0], _data.Map.AgentPos[1]] = SquareStatus.Clean;
                _data.Lifetime++;
            }
            
        }

        public bool WithinMapBorders(int[] loc)
        {
            if ((0 <= loc[0]) && (loc[0] <= _data.Map.Size[0] - 1))
            {
                if ((0 <= loc[1]) && (loc[1] <= _data.Map.Size[1] - 1))
                {
                    return true;
                }
            }

            return false;
        }

        public void MoveAgent(AgentActions action)
        {
            int[] newPos;
            switch (action)
            {
                //Down is +VE, matrix notation
                case AgentActions.Down:
                    newPos = new int[] { _data.Map.AgentPos[0], _data.Map.AgentPos[1] + 1 };
                    if (WithinMapBorders(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Up:
                    newPos = new int[] { _data.Map.AgentPos[0], _data.Map.AgentPos[1] - 1 };
                    if (WithinMapBorders(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Left:
                    newPos = new int[] { _data.Map.AgentPos[0] - 1, _data.Map.AgentPos[1] };
                    if (WithinMapBorders(newPos))
                        _data.Map.AgentPos = newPos;
                    break;

                case AgentActions.Right:
                    newPos = new int[] { _data.Map.AgentPos[0] + 1, _data.Map.AgentPos[1] };
                    if (WithinMapBorders(newPos))
                        _data.Map.AgentPos = newPos;
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {       
            ProgramData _data = new ProgramData();
            IView _view = new ConsoleView(ref _data);
            IModel _model = new GameModel(ref _data);
            IMapParser _mapParser = new MapParser();

            Map _map = _mapParser.ReadMapFromFile("basic-map.txt");
            _data.Initilize(_map);

            _data.Agent.SetAgentStrategy(new AgentRandomStrategy());

            while(!_data.Finished)
            {
                _model.Run();
                _view.Draw();
                Thread.Sleep(1000);
                
            }
        }
    }
}
