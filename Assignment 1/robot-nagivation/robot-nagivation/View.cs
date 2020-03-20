using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SFML.Graphics;
using SFML.System;

namespace robot_nagivation
{
    public interface IView
    {
        abstract void Draw();
    }

    public class SFMLView : IView
    {
        private ProgramData _data;

        private RenderWindow _window;

        private Font _font;

        private Vector2i _windowSize = new Vector2i(1024, 576);

        public SFMLView(ref ProgramData data)
        {
            _data = data;


            _font = new Font("express.ttf");

            _window = new RenderWindow(
                 new SFML.Window.VideoMode(1024, 576),
                 "SFML: Robot Nagivation");

            _window.KeyPressed += Window_KeyPressed;

        }

        public void SetWindowSize(int width, int height)
        {
            _windowSize = new Vector2i(width, height);
        }
        
        
        public void Draw()
        {
            // Process Events
            _window.DispatchEvents();

            _window.Clear(new Color(61,71,78));

            

            _window.Draw(new Text("Agent Actions", _font, 35)
            {
                Position = new Vector2f(720,90),
                FillColor = new Color(190, 190, 190)
                
            });

            //Left Bar
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(60, 600),
                FillColor = new Color(27, 27, 40)
            });

            // Left Big Box
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(95, 70),
                Size = new Vector2f(570, 430),
                FillColor = new Color(40, 40, 40)
            });
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(85, 60),
                Size = new Vector2f(570, 430),
                FillColor = new Color(100, 100, 110)
            });


            // Right Box
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(730, 160),
                Size = new Vector2f(240, 340),
                FillColor = new Color(40, 40, 40)
            });
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(720, 150),
                Size = new Vector2f(240, 340),
                FillColor = new Color(100, 100, 110)
            });

            // Bottom debug box
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(95, 520),
                Size = new Vector2f(870, 40),
                FillColor = new Color(26, 26, 28)
            });
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(105, 530),
                Size = new Vector2f(860, 30),
                FillColor = new Color(40, 40, 40)
            });

            Vector2f boxSize = new Vector2f(
                    (570 - 100) / _data.Map.MapMatrix.GetLength(0),
                    (430 - 100) / _data.Map.MapMatrix.GetLength(1)
                );

            Vector2f spacing = new Vector2f(
                100 / _data.Map.MapMatrix.GetLength(0),
                100 / (_data.Map.MapMatrix.GetLength(1)+1));

            for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
                {

                    _window.Draw(new RectangleShape()
                    {
                        Position = new Vector2f(
                            85 + (spacing.X) + x * (boxSize.X + spacing.X),
                            60 + (spacing.Y) + y * (boxSize.Y + spacing.Y)),
                        Size = boxSize,
                        FillColor = new Color(50, 50, 52)
                    });

                    switch(_data.Map.MapMatrix[x,y])
                    {
                        case TileType.Wall:

                            _window.Draw(new RectangleShape()
                            {
                                Position = new Vector2f(
                                    85 + (spacing.X) + x * (boxSize.X + spacing.X) + 5,
                                    60 + (spacing.Y) + y * (boxSize.Y + spacing.Y) + 5),
                                Size = new Vector2f(boxSize.X - 10, boxSize.Y - 10),
                                FillColor = new Color(180, 180, 180)
                            });
                            break;

                        case TileType.Goal:

                            _window.Draw(new RectangleShape()
                            {
                                Position = new Vector2f(
                                    85 + (spacing.X) + x * (boxSize.X + spacing.X) + 5,
                                    60 + (spacing.Y) + y * (boxSize.Y + spacing.Y) + 5),
                                Size = new Vector2f(boxSize.X - 10, boxSize.Y - 10),
                                FillColor = new Color(111, 221, 102)
                            });
                            break;

                        case TileType.Start:

                            _window.Draw(new RectangleShape()
                            {
                                Position = new Vector2f(
                                    85 + (spacing.X) + x * (boxSize.X + spacing.X) + 5,
                                    60 + (spacing.Y) + y * (boxSize.Y + spacing.Y) + 5),
                                Size = new Vector2f(boxSize.X - 10, boxSize.Y - 10),
                                FillColor = new Color(221, 108, 102)
                            });
                            break;
                    }

                    if ((_data.Map.AgentPos.X == x) && (_data.Map.AgentPos.Y == y))
                    {
                        _window.Draw(new RectangleShape()
                        {
                            Position = new Vector2f(
                                    85 + (spacing.X) + x * (boxSize.X + spacing.X) + 15,
                                    60 + (spacing.Y) + y * (boxSize.Y + spacing.Y) + 15),
                            Size = new Vector2f(boxSize.X - 30, boxSize.Y - 30),
                            FillColor = new Color(242, 242, 112)
                        });
                    }

                }
            }



            _window.Display();
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
                _data.Finished = true;
            }
        }
    }

        public class ConsoleView : IView
    {
        
        private ProgramData _data;

        private Dictionary<TileType, Char> _consoleDisplayValuePairs = new Dictionary<TileType, Char>();


        public ConsoleView(ref ProgramData data)
        {
            _data = data;

            _consoleDisplayValuePairs.Add(TileType.Empty, '.');
            _consoleDisplayValuePairs.Add(TileType.Wall,  '#');
            _consoleDisplayValuePairs.Add(TileType.Start, 'S');
            _consoleDisplayValuePairs.Add(TileType.Goal,  'G');
        }
        public void Draw()
        {
            
            Console.Clear();
            
            Console.WriteLine("  +-----------------------------------------+");
            for (int y = 0; y < _data.Map.MapMatrix.GetLength(1); y++)
            {
                Console.Write("  |");

                for (int x = 0; x < _data.Map.MapMatrix.GetLength(0); x++)
                {

                    if ((_data.Map.AgentPos.X == x) && (_data.Map.AgentPos.Y == y))
                    {
                        Console.Write("A");
                    }
                    else
                    {
                        Console.Write(_consoleDisplayValuePairs[_data.Map.MapMatrix[x, y]]);
                    }


                }

                Console.WriteLine("|");
            }

            Console.WriteLine("  +-----------------------------------------+");

            Thread.Sleep(10);
           
            if (_data.AgentDecisions.Count > 0)
            {
                //Console.WriteLine(" Agent has decided: " + _data.AgentDecisions[_data.AgentDecisions.Count - 1]);
                //Console.WriteLine(" Agent Score: " + _data.Lifetime);
                //Console.WriteLine(" Agent Position: x: " + _data.Map.AgentPos[0] + ", y:" + +_data.Map.AgentPos[1]);
            }
            


        }
    }
}
