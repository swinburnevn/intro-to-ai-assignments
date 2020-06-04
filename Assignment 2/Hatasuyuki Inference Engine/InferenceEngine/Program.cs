using System;
using System.Collections.Generic;
using SFML.System;

/// <summary>
/// Hatsuyuki Inference Engine
/// 
/// Usage: iengine.exe [tt/fc/bc] [filename] [optional: GUI]
/// 
/// For Introduction to Artificial Intelligence 2020 (Year of the Big Sick)
///   Author: 101624964 | mikanwolfe@nekox.net
/// </summary>

/// <example>
/// 
/// Open the program in Forward Chaining Mode without GUI 
///     iengine fc "test_HornKB.txt"
/// 
/// Open the program in Truth Table Mode with GUI
///     iengine tt "test_HornKB.txt" gui
///     
/// Supplying a third parameter, regardless of what it is, will open the GUI.
/// Method case insensitive.
/// 
/// </example>


namespace InferenceEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Usage: iengine method filename
            //  Methods: TT / FC / BC

            // New Usage: 
            // iengine method filename (Any triggers GUI)

            bool guiEnabled = false;


            if (args.Length < 1)
                throw new Exception("Not enough arguments! Usage: iengine [Method] [Filename] [GUI: Yes/No]");

            if (args.Length > 2)
            {
                guiEnabled = true;
            }

            // Create modules and assign connections
            TextParser parser = new TextParser(args[1]);
            
            Console.WriteLine($"ASK: {parser.Ask} | TELL: {parser.Tell}");

            KnowledgeBase KB;
            switch (args[0].ToLower())
            {

                case "bc":
                    KB = new BackwardChaining(parser.Ask, parser.Tell);
                    break;

                case "fc":
                    KB = new ForwardChaining(parser.Ask, parser.Tell);
                    break;

                default:
                    KB = new TruthTable(parser.Ask, parser.Tell);
                    break;
            }

            Console.WriteLine($"KB Output: {KB.Execute()}");

            if (guiEnabled)
            {
                SFMLView _view = new SFMLView(ref KB);

                while (!_view.IsFinished)
                {
                    _view.Draw();
                }
                _view.Close();
            }
        }
    }
}
