using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Numerics;

using SFML.System;

namespace robot_nagivation
{

    public enum TileType
    {
        Empty,
        Wall,
        Start,
        Goal,
        Agent
    }

    public enum AgentActions
    {
        Search,
        Idle,
        Up,
        Down,
        Left,
        Right
    }


    /*
     * Node<T> represents a potential state of the agent, notably, it 
     *  is a position on the tile map. 
     *  
     *  Though it was originally inteded to be a pure class, for ease of reading and writing,
     *  It will not be.
     * 
     */
    public class Node<T>
    {
        private T _data; // TileType + Pos
        private Node<T> _parent;
        private string _message;
        private Vector2i _pos;

        public Node(T data)
        {
            _data = data;
            _pos = new Vector2i();
        }

        public Node(T data, Node<T> parent)
            : this(data)
        {
            _parent = parent;
        }

        public Node(T data, Node<T> parent, Vector2i pos)
            : this(data, parent)
        {
            _pos = pos;
        }

        public T Data { get => _data; set => _data = value; }
        public Vector2i Pos { get => _pos; set => _pos = value; }
        public Node<T> Parent { get => _parent; set => _parent = value; }
        public string Message { get => _message; set => _message = value; }
    }
    

    /* 
     * This is the "world" that the agent inhabits.
     *  Contains a NxM matrix of TileType enum, which represents the tile occupying that location.
     *  Also contains agent position.
     * 
     */
    /// <summary>
    /// Map class, contains details about the world and agent.
    /// </summary>
    public class Map
    {
        private Vector2i _agentPos;
        private Vector2i _size;
        private TileType[,] _mapMatrix;
        private List<Vector2i> _goalPositions;

        public Map(Vector2i size)
        {
            _size = size;
            _mapMatrix = new TileType[size.X, size.Y];
            
            for (int y = 0; y < _mapMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < _mapMatrix.GetLength(0); x++)
                {
                    _mapMatrix[x, y] = TileType.Empty;
                }
            }

            _agentPos = new Vector2i(0, 0);

            _goalPositions = new List<Vector2i>();

        }

        public Map(int x, int y)
            : this(new Vector2i( x, y ))
        {
        }

        public List<Vector2i> GoalPos { get => _goalPositions; set => _goalPositions = value; }
        public TileType[,] MapMatrix { get => _mapMatrix; set => _mapMatrix = value; }
        public Vector2i AgentPos { get => _agentPos; set => _agentPos = value; }
    }

    /*
     * Used to produce a map, e.g. File Parser implementation takes a file input and 
     *  uses it to return the map.
     */
    public interface IMapParser
    {
        public Map ReadMapFromFile(string filename);
    }

    public class MapParser : IMapParser
    {
        public Map ReadMapFromFile(string filename)
        {
            try
            {
                StreamReader _reader = new StreamReader(filename);

                string line = _reader.ReadLine();               // Map size
                string[] sizes = Regex.Split(line, @"\D+");     // Returns string[4] with sizes in 1 and 2
                Map readInMap = new Map(int.Parse(sizes[2]), int.Parse(sizes[1]));


                line = _reader.ReadLine();                      // Agent pos
                string[] agentPos = Regex.Split(line, @"\D+");
                readInMap.AgentPos = new Vector2i ( int.Parse(agentPos[1]), int.Parse(agentPos[2]) );
                
                readInMap.MapMatrix[(int)readInMap.AgentPos.X, (int)readInMap.AgentPos.Y] = TileType.Start;

                line = _reader.ReadLine();                      // Goal states of agent / Goal pos.
                string[] goalPos = line.Split('|');             // There can be more than one goal.
                
                foreach (string s in goalPos)
                {
                    string[] aGoalPos = Regex.Split(s, @"\D+");
                    Vector2i vectorGoalPos = new Vector2i(int.Parse(aGoalPos[1]), int.Parse(aGoalPos[2]));
                    readInMap.GoalPos.Add(vectorGoalPos);
                    readInMap.MapMatrix[(int)vectorGoalPos.X, (int)vectorGoalPos.Y] = TileType.Goal;
                }

                readInMap.AgentPos = new Vector2i ( int.Parse(agentPos[1]), int.Parse(agentPos[2]) );


                // Rest of lines are in the form (x, y, w, h)
                // All of the lines represent walls.
                while (!_reader.EndOfStream)
                {
                    line = _reader.ReadLine();

                    // Failsafe, in case there are empty newlines.
                    if (line == "")
                        break;

                    string[] wall = Regex.Split(line, @"\D+");
                    int wallX = int.Parse(wall[1]);
                    int wallY = int.Parse(wall[2]);
                    int wallW = int.Parse(wall[3]);
                    int wallH = int.Parse(wall[4]);

                for (int y = wallY; y < wallY + wallH; y++)
                {
                    for (int x = wallX; x < wallX + wallW; x++)
                    {
                        readInMap.MapMatrix[x, y] = TileType.Wall;
                        }
                }

            }

                _reader.Close();
                return readInMap;

            } catch (Exception e)
            {
                Console.WriteLine("Exception returned at file parser: " + e.Message);
                return null;
            }
            
        }
    }
}
