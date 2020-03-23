using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Numerics;

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

            int _posPointer = 0;
            for (int i = _data.AgentDecisions.Count; (i > _data.AgentDecisions.Count - 15) && (i >= 1); i--)
            {
                
                _window.Draw(new Text(i +  " : " +_data.AgentDecisions[i-1].ToString(), _font, 16)
                {
                    Position = new Vector2f(740, 160 + _posPointer * 22),
                    FillColor = new Color(252, 187, 116)

                });
                _posPointer++;
            }

           

            

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

            for (int i = 0; i < _data.AgentPositions.Count - 1; i++)
            {
                Vector2 prev = _data.AgentPositions[i];
                Vector2 curr = _data.AgentPositions[i + 1];

                Vector2f A = new Vector2f(
                    85 + (spacing.X) + prev.X * (boxSize.X + spacing.X) + boxSize.X / 2,
                    85 + (spacing.Y) + prev.Y * (boxSize.Y + spacing.Y)
                    );

                Vector2f B = new Vector2f(
                    85 + (spacing.X) + curr.X * (boxSize.X + spacing.X) + boxSize.X / 2,
                    85 + (spacing.Y) + curr.Y * (boxSize.Y + spacing.Y)
                    );
                Vector2f AB = B - A;
                float magnitude = (float)Math.Sqrt(AB.X * AB.X + AB.Y * AB.Y);
                float rotation = (float)(Math.Atan(AB.Y / AB.X) * 180 / Math.PI);
                if (AB.X < 0)
                    rotation += 180;


                RectangleShape rectLine = new RectangleShape()
                {
                    Position = A,
                    Size = new Vector2f(magnitude, 3),
                    Rotation = rotation,
                    FillColor = new Color(
                        (byte)(255 * i / _data.AgentPositions.Count),
                        0,
                       (byte)(255 - 255 * i / _data.AgentPositions.Count)
                    )

                };

                _window.Draw(rectLine);

            }

            foreach (Vector2 node in _data.SearchedNodes)
            {
                _window.Draw(new CircleShape()
                {
                    Position = new Vector2f(
                            85 + (spacing.X) + node.X * (boxSize.X + spacing.X) + boxSize.X / 2 - 10,
                            60 + (spacing.Y) + node.Y * (boxSize.Y + spacing.Y) + boxSize.Y / 2 - 5) ,
                    Radius = 10,
                    FillColor = new Color(150, 150, 150)
                });
            }

            foreach (Vector2 node in _data.FrontierNodes)
            {
                _window.Draw(new CircleShape()
                {
                    Position = new Vector2f(
                            85 + (spacing.X) + node.X * (boxSize.X + spacing.X) + boxSize.X / 2 - 10,
                            60 + (spacing.Y) + node.Y * (boxSize.Y + spacing.Y) + boxSize.Y / 2 - 5),
                    Radius = 10,
                    FillColor = new Color(255, 201, 14)
                });
            }

            foreach (Vector2 node in _data.Path)
            {
                _window.Draw(new CircleShape()
                {
                    Position = new Vector2f(
                            85 + (spacing.X) + node.X * (boxSize.X + spacing.X) + boxSize.X / 2 - 10,
                            60 + (spacing.Y) + node.Y * (boxSize.Y + spacing.Y) + boxSize.Y / 2 - 5),
                    Radius = 5,
                    FillColor = new Color(255, 255, 255)
                });
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
