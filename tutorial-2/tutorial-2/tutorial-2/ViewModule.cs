using System;
using System.Collections.Generic;
using System.Text;

namespace tutorial_2
{
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

            _consoleDisplayValuePairs.Add(SquareStatus.Clean, '.');
            _consoleDisplayValuePairs.Add(SquareStatus.Dirty, ':');
            _consoleDisplayValuePairs.Add(SquareStatus.Null, '#');
        }
        public void Draw()
        {

            Console.Clear();
            Console.WriteLine("+-----------------------------------------+");

            for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
            {
                Console.Write("  |");

                for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
                {

                    if ((_data.Map.AgentPos[0] == x) && (_data.Map.AgentPos[1] == y))
                    {
                        Console.Write("A");
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                    Console.Write(_consoleDisplayValuePairs[_data.Map.MapMatrix[x, y]] + "|");


                }

                Console.WriteLine();
            }

            Console.WriteLine("+-----------------------------------------+");


            if (_data.AgentDecisions.Count > 0)
            {
                Console.WriteLine(" Agent has decided: " + _data.AgentDecisions[_data.AgentDecisions.Count - 1]);
                Console.WriteLine(" Agent Score: " + _data.Lifetime);
                Console.WriteLine(" Agent Position: x: " + _data.Map.AgentPos[0] + ", y:" + +_data.Map.AgentPos[1]);
            }



        }
    }
}
