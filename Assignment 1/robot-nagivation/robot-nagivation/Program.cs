using System;
using System.Collections.Generic;

namespace robot_nagivation
{
    public class Program
    {
        static int Main(string[] args)
        {

            // Process CLI
            //  Arguments in the form nav.exe  [filename] [method] [optional: output file] [optional:gui] [optional: agentdelay]
            //                                      0          1          2                     3

            if (args.Length < 2)
            {
                throw new Exception("Not enough arguments: Input is taken as 'nav.exe' 'search'/'gui' [filename] [bfs/dfs/greedy/etc]");
            }

            // Create modules and assign connections
            ProgramData _data = new ProgramData();
            IView _view = new ConsoleOutput(ref _data);
            IModel _model = new GameModel(ref _data);
            IMapParser _mapParser = new MapParser();

            string _outputFile = "101624964-output.txt";
           

            // Slight pause for GUI, so windows can be re-arranged.
            bool startRequested = false;

            // Defaults to console view, if program specifically calls for GUI, it's then used.
            if (args.Length >= 3 )
            {
                if (args[2] != null)
                {
                    _view = new SFMLView(ref _data);
                    _data.DisplayMode = true;
                }
            }
                
                


            //Map _map = _mapParser.ReadMapFromFile("RobotNav-test.txt");
            //Map _map = _mapParser.ReadMapFromFile("robot-nav-map4.txt");

            try
            {
                Map _map = _mapParser.ReadMapFromFile(args[0]);

                _data.Map = _map;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Map could not be initialised, is the file incorrectly formatted? {e}");
                return 1; // Map not initialised, try checking existence of map or reformatting it (most likely out of bounds?)
                
            }


            // Define type of agent, insert into data.

            Dictionary<string, Agent> _agentsByIdentifier = new Dictionary<string, Agent>();
            _agentsByIdentifier.Add("bfs", new BreadthFirstAgent());
            _agentsByIdentifier.Add("dfs", new DepthFirstAgent());
            _agentsByIdentifier.Add("gbfs", new GreedyFirstAgent());
            _agentsByIdentifier.Add("astar", new AStarAgent());
            _agentsByIdentifier.Add("iddfs", new IterativeDDFSAgent());
            _agentsByIdentifier.Add("ucs", new UniformCostAgent());

            try
            {
                _data.Agent = _agentsByIdentifier[args[1]];
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not find method/agent in dictionary, error returned: '{e}'");
                return 2; //Agent could not be initialised, check docs for accepted methods
            }
            
            _model.Initialise();
            int count = 0;


            if (args.Length >= 4)
                _data.Agent.AgentData.AgentDelay = int.Parse(args[3]);

            // if there's a delay, wait at the start.
            if (_data.DisplayMode)
            {
                while (!startRequested || _data.WindowRequestClosed)
                {
                    count++;
                    _view.Draw();
                    System.Threading.Thread.Sleep(5);
                    if (count > 300)
                        startRequested = true;
                }
            }
            

            while (!_data.Finished)
            {
                _model.Run();
                _view.Draw();
                System.Threading.Thread.Sleep(10);

            }
            // Write to file, open streamwriter without append (overwrite)
            System.IO.StreamWriter _file = new System.IO.StreamWriter(_outputFile, false);
            string line;
            // Write output to console, write output to file.
            line = $"{_outputFile} '{args[1]}: {_data.Agent.Name}' {_data.Agent.AgentData.SearchedPos.Count}";
            Console.WriteLine(line);
            _file.WriteLine(line);

            foreach (AgentActions action in _data.AgentDecisions)
            {
                if (action == AgentActions.Lost)
                {
                    line = "No solution found";
                    Console.WriteLine(line);
                    _file.WriteLine(line);
                    break;
                }
                line = $"{action}; ";
                Console.Write(line);
                _file.Write(line);
            }
            _file.Close();

            

            



            // Pause if there's a delay, this means that the user wants to observe
            if (_data.DisplayMode)
            {
                while(!_data.WindowRequestClosed)
                {
                    _model.Run();
                    _view.Draw();
                    System.Threading.Thread.Sleep(10);
                }
            }

            Console.WriteLine();

            return 0;


        }
    }
}
