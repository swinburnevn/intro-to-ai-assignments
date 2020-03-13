using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace tutorial_2
{




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

            _data.Agent


            while (!_data.Finished)
            {
                _model.Run();
                _view.Draw();
                Thread.Sleep(1000);

            }
        }
    }



    /*
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
    */
}
