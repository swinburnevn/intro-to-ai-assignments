using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace tutorial_2
{

    public enum AgentActions
    {
        Left,
        Right,
        Up,
        Down,
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

        public Percepts(Map map)
        {
            _currentPosition = map.AgentPos;
            _map = map.MapMatrix;
            _currentSquareStatus = map.MapMatrix[map.AgentPos[0], map.AgentPos[1]];
        }

        public SquareStatus CurrentSquareStatus { get => _currentSquareStatus; set => _currentSquareStatus = value; }
        public int[] CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public SquareStatus[,] Map { get => _map; set => _map = value; }
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
            Console.WriteLine("+-----------------------------------------+");

            for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
            {
                Console.Write("  |");

                for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
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

        }
    }

    public class ProgramData
    {
        private IAgent _agent;
        private Map _map;
        private Percepts _percepts;

        public void Initilize(Map map)
        {
            _agent = new CleaningAgent();
            _map = map;
        }


        public IAgent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
        public Percepts Percepts
        {
            get
            {
                return new Percepts(_map);
            }
        }
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

            return _readMap;

        }
    }


    class Program
    {
        static void Main(string[] args)
        {       
            ProgramData _data = new ProgramData();
            IView _view = new ConsoleView(ref _data);
            IMapParser _mapParser = new MapParser();

            Map _map = _mapParser.ReadMapFromFile("basic-map.txt");
            _data.Initilize(_map);

            _data.Agent.SetAgentStrategy(new AgentRandomStrategy());

            _view.Draw();

            Console.WriteLine("Agent Decision: " + _data.Agent.next(_data.Percepts));

            //while ((line = _reader.ReadLine()) != null)
            //{
            //    string[] items = line.Split(' ');
            //}

        }
    }
}
