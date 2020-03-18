using System;
using System.Collections.Generic;
using System.Text;

namespace robot_nagivation
{

    public enum TileType
    {
        Empty,
        Wall,
        Start,
        Goal
    }

    public enum AgentActions
    {

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
        private int[] _agentPos;
        private TileType[,] _mapMatrix;

        public TileType[,] MapMatrix { get => _mapMatrix; set => _mapMatrix = value; }
        public int[] AgentPos { get => _agentPos; set => _agentPos = value; }
    }

    /*
     * Used to produce a map, e.g. File Parser implementation takes a file input and 
     *  uses it to return the map.
     */
    public interface MapParser
    {

    }
    public class MapFileParser : MapParser
    {
    }
}
