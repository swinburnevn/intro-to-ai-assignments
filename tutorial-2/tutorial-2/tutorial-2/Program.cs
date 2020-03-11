using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace tutorial_2
{

    public interface IAgentStategy
    {
        abstract void next(Percepts percepts);

    }

    public class Percepts
    {
        // wrapper that contains all the information required
    }

    public interface IAgent
    {
        public abstract void next(Percepts percepts);
    }

    public class CleaningAgent: IAgent
    {
        private IAgentStategy _agentStategy;

        public void next(Percepts percepts)
        {
            AgentStategy.next(percepts);
        }

        public IAgentStategy AgentStategy { get => _agentStategy; set => _agentStategy = value; }
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
        private SquareStatus[,] _map;



        public Map(int[] size)
        {
            _size = size;
            _map = new SquareStatus[size[0], size[1]];



        }


        public int[] Size { get => _size; }
        public SquareStatus[,] MapMatrix { get => _map; set => _map = value; }
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

            _consoleDisplayValuePairs.Add(SquareStatus.Clean, '_');
            _consoleDisplayValuePairs.Add(SquareStatus.Dirty, '*');
            _consoleDisplayValuePairs.Add(SquareStatus.Null, '#');
        }
        public void Draw()
        {
            Console.WriteLine("+-----------------------------------------+");

            for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
            {
                Console.Write("| ");

                for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
                {
                    Console.Write(_consoleDisplayValuePairs[_data.Map.MapMatrix[x, y]]);
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

        public void Initilize(int[] size)
        {
            _agent = new CleaningAgent();
            _map = new Map(size);
        }

        public IAgent Agent { get => _agent; set => _agent = value; }
        public Map Map { get => _map; set => _map = value; }
    }



    class Program
    {
        static void Main(string[] args)
        {
            
            StreamReader _reader = new StreamReader("basic-map.txt");
            ProgramData _data = new ProgramData();
            IView _view = new ConsoleView(ref _data);


            string line = _reader.ReadLine(); // read size of array
            string[] sizes = Regex.Split(line, @"\D+"); // Returns string[4] with sizes in 1 and 2

            //why not create map here and send it? (here being procedural)
            
            _data.Initilize(new int[2] { int.Parse(sizes[1]), int.Parse(sizes[2]) }); // Creates Map


            _view.Draw();





            //while ((line = _reader.ReadLine()) != null)
            //{
            //    string[] items = line.Split(' ');
            //}


        }
    }
}
