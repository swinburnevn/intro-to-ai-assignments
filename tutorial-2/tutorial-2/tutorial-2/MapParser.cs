using System;
using System.IO;
using System.Text.RegularExpressions;

namespace tutorial_2
{
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
            : this(new int[] { x, y })
        {
        }


        public int[] Size { get => _size; }
        public SquareStatus[,] MapMatrix { get => _map; set => _map = value; }
        public int[] AgentPos { get => _agentPos; set => _agentPos = value; }
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
}
