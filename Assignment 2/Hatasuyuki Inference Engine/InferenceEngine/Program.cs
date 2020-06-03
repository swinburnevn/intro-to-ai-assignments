using System;
using System.Collections.Generic;
using SFML.System;

/// <summary>
/// Hatsuyuki Inference Engine
///   A3 makes a comeback! Please refer to documentation for more details.
///   
/// For Introduction to Artificial Intelligence 2020 (Year of the Big Sick)
///   Author: 101624964 | mikanwolfe@nekox.net
/// </summary>

namespace InferenceEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Usage: iengine method filename
            //  Methods: TT / FC / BC

            // New Usage: 
            // iengine method filename (MODE: GUI/BASE)

            //if (args.Length < 2)
            //    throw new Exception("Not enough arguments! Usage: iengine [Method] [Filename] [Mode]");

            // Create modules and assign connections
            TextParser parser = new TextParser("test_HornKB.txt");

            Console.WriteLine($"ASK: {parser.Ask} | TELL: {parser.Tell}");

            KnowledgeBase KB = new ForwardChaining(parser.Ask, parser.Tell);

            Console.WriteLine($"KB Output: {KB.Execute()}");
        }
    }
}
