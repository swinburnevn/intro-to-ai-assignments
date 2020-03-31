using System;

namespace robot_nagivation
{
    public class Program
    {
        static void Main(string[] args)
        {

            
            // Create modules and assign connections

            ProgramData _data = new ProgramData();
            IView _view = new SFMLView(ref _data);
            IModel _model = new GameModel(ref _data);
            IMapParser _mapParser = new MapParser();

            bool startRequested = false;

            // Read in the map

            //Map _map = _mapParser.ReadMapFromFile("RobotNav-test.txt");
            Map _map = _mapParser.ReadMapFromFile("robot-nav-map2.txt");
            if (_map == null)
            {
                throw new Exception("Map could not be initialised");
            }

            _data.Map = _map;


            // Define type of agent, insert into data.
            /*
            switch (args[2])
            {
                case "random":
                    
                    break;

                case "astar":
                    break;

                default:
                    //return some sort of error
                    
                    break;
            }
            */

            _data.Agent = new AStarAgent();
            _model.Initialise();


            int count = 0;

            while(!startRequested)
            {
                count++;
                _view.Draw();
                System.Threading.Thread.Sleep(10);
                if (count > 300)
                    startRequested = true;
            }

            while (!_data.WindowRequestClosed)
            {
                _model.Run();
                _view.Draw();
                System.Threading.Thread.Sleep(10);

            }


            Console.Read();





        }
    }
}
