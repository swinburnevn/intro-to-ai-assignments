using System;
using System.Collections.Generic;
using System.Text;

namespace robot_nagivation
{
    public class Percepts
    {
        //private TileType _currentTile;
        private int[] _currentPosition;
        private TileType[,] _map;
        private int _lifetime;

       
        public int[] CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public TileType[,] Map { get => _map; set => _map = value; }
        public int Lifetime { get => _lifetime; set => _lifetime = value; }
    }
    /*
     * Encapsulates the logic of the virtual world
     *  Contains Map, Agents, and provides agent with percepts
     *  
     * 
     */
    public interface IModel
    {
        public void Run();
    }

    public class GameModel : IModel
    {
        private ProgramData _data;
        public GameModel(ref ProgramData data)
        {
            _data = data;
        }
        public void Run()
        {
            



        }
    }
}
