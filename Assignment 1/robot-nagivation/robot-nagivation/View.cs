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
        private RenderWindow _nodeWindow;

        private View _nodeView;

        private float _nodeViewPos;

        private Font _font;

        private Vector2i _windowSize = new Vector2i(1024, 576);

        public SFMLView(ref ProgramData data)
        {

            _data = data;


            _font = new Font("express.ttf");

            _nodeWindow = new RenderWindow(
                 new SFML.Window.VideoMode(1024, 576),
                 "SFML: Node Display");


            _window = new RenderWindow(
                 new SFML.Window.VideoMode(1024, 576),
                 "SFML: Robot Nagivation");

            _nodeView = new View(new FloatRect(new Vector2f(), new Vector2f(1024, 576)));
            _nodeViewPos = _nodeView.Center.Y;
            


            _window.KeyPressed += Window_KeyPressed;
            _nodeWindow.KeyPressed += Window_KeyPressed;

            _nodeWindow.MouseWheelScrolled += Window_MouseWheeled;

        }

        public void SetWindowSize(int width, int height)
        {
            _windowSize = new Vector2i(width, height);
        }

        public Color HashedPosColor(int x, int y, int seed)
        {
            byte r = (byte)(150 * Math.Sin(x / 3.0f));
            byte g = (byte)(150 * Math.Cos(y / 3.0f));
            byte b = (byte)seed;

            return new Color(r, g, b);

        }

        protected void DrawBackground()
        {
            _window.Draw(new Text("Agent Actions", _font, 35)
            {
                Position = new Vector2f(720, 90),
                FillColor = new Color(190, 190, 190)

            });

            //Left Bar
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(60, 600),
                FillColor = new Color(27, 27, 40)
            });

            //Left Bar
            _nodeWindow.Draw(new RectangleShape()
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(60, 3000),
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
        }
        
        public void DrawNodeDisplay()
        {

            _nodeWindow.Draw(new Text("Node tree", _font, 35)
            {
                Position = new Vector2f(90, 30),
                FillColor = new Color(190, 190, 190)

            });

            _nodeWindow.Draw(new Text(_data.Agent.Name, _font, 15)
            {
                Position = new Vector2f(260, 53),
                FillColor = new Color(200, 200, 200)

            });

            bool finished = false;
            int level = 0;

            int parentXPos = 0;
            int childXPos = 0;

            Queue<Node<TileType>> levelNodeQueue = new Queue<Node<TileType>>();
            Queue<Node<TileType>> nextLevelNodeQueue = new Queue<Node<TileType>>();

            levelNodeQueue.Enqueue(_data.Agent.AgentData.RootNode);

            _nodeWindow.Draw(new CircleShape()
            {
                Position = new Vector2f(
            105,
            80),
                Radius = 5,
                FillColor = new Color(221, 108, 102)
            });

            _nodeWindow.Draw(new Text("(" + _data.Agent.AgentData.RootNode.Pos.X + ", "
                + _data.Agent.AgentData.RootNode.Pos.Y + ")", _font, 10)
            {
                Position = new Vector2f(
                        105 + 15,
                        80),
                FillColor = new Color(220, 220, 220)

            });


            while (!finished)
            {
                parentXPos = 0;
                childXPos = 0;




                while (levelNodeQueue.Count > 0)
                {



                    Node<TileType> currentParentNode = levelNodeQueue.Dequeue();

                    foreach (Node<TileType> child in currentParentNode.Children)
                    {
                        nextLevelNodeQueue.Enqueue(child);




                        // child is child node
                        // current parent node is current parent node, xpos increases with eveyr child, however, we'll need to know the position of the parent node...
                        // draw the line between aprent and child

                        Vertex[] line = new Vertex[] { };

                        if (child.IsOnPath)
                        {
                            if (currentParentNode.IsOnPath)
                            {
                                line = new Vertex[]
                            {
                                new Vertex(new Vector2f(105 + 5 + 80 * parentXPos, 110 + 5 + 30 * (level - 1 )), new Color(250,250,220)),
                                new Vertex(new Vector2f(105 + 5 + 80 * childXPos, 110 + 5 + 30 * level), new Color(250,250,220))
                            };
                            }
                        }
                        else
                        {
                            line = new Vertex[]
                            {
                                new Vertex(new Vector2f(105 + 5 + 80 * parentXPos, 110 + 5 + 30 * (level - 1 )), new Color(150,150,150)),
                                new Vertex(new Vector2f(105 + 5 + 80 * childXPos, 110 + 5 + 30 * level), new Color(150,150,150))
                            };
                        }

                        

                        _nodeWindow.Draw(line, PrimitiveType.Lines);

                        _nodeWindow.Draw(new Text( "(" + child.Pos.X + ", " + child.Pos.Y + ")", _font, 10)
                        {
                            Position = new Vector2f(
                                105 + 15 +  80 * childXPos,
                                110 + 30 * level),
                            FillColor = new Color(220, 220, 220)

                        });

                        if (child.Cost != 0)
                        {
                            _nodeWindow.Draw(new Text("c: " + child.Cost, _font, 10)
                            {
                                Position = new Vector2f(
                                105 + 50 + 80 * childXPos,
                                110 + 30 * level),
                                FillColor = new Color(220, 220, 220)

                            });
                        }
                        

                        CircleShape nodeCircle = new CircleShape()
                        {
                            Position = new Vector2f(
                                105 + 80 * childXPos,
                                110 + 30 * level),
                            Radius = 5,
                            FillColor = HashedPosColor(child.Pos.X, child.Pos.Y, 100)
                        };

                        if (_data.Agent.AgentData.PosToSearch.Count > 0) 
                            if (child.Pos == _data.Agent.AgentData.PosToSearch[0])
                            {
                                nodeCircle.FillColor = new Color(255, 201, 14);
                            }


                        if (child.Data == TileType.Goal)
                            nodeCircle.FillColor = new Color(111, 221, 102);




                        _nodeWindow.Draw(nodeCircle);






                        childXPos++;

                    }

                    parentXPos++;
                    

                }

                // We've just looped through all the stuff in this level, all the children are in the next level
                // move each next level node to the level one and move on

                levelNodeQueue.Clear();
                while (nextLevelNodeQueue.Count > 0)
                {
                    levelNodeQueue.Enqueue(nextLevelNodeQueue.Dequeue());
                }

                nextLevelNodeQueue.Clear();

                if (levelNodeQueue.Count == 0)
                    finished = true;

                level++;

                _nodeWindow.Draw(new Text("INTERNAL", _font, 10)
                {
                    Position = new Vector2f(
                                5,
                                50),
                    FillColor = new Color(220, 220, 220)

                });

                int yPos = 0;

                for (int i = _data.Agent.AgentData.InternalHeap.Count - 1; i >= 0; i-- )
                {
                    Node<TileType> node = _data.Agent.AgentData.InternalHeap[i];
                    CircleShape nodeCircle = new CircleShape()
                    {
                        Position = new Vector2f(
                                10,
                                90 + 30 * yPos),
                        Radius = 5,
                        FillColor = HashedPosColor(node.Pos.X, node.Pos.Y, 100)
                    };


                    _nodeWindow.Draw(nodeCircle);
                    if (node.Cost != 0)
                    {
                        _nodeWindow.Draw(new Text(node.Cost.ToString(), _font, 10)
                        {
                            Position = new Vector2f(
                            12,
                            90 + 30 * yPos),
                            FillColor = new Color(220, 220, 220)

                        });
                    }


                    _nodeWindow.Draw(new Text("(" + node.Pos.X + ", " + node.Pos.Y + ")", _font, 10)
                    {
                        Position = new Vector2f(
                                25,
                                90 + 30 * yPos),
                        FillColor = new Color(220, 220, 220)

                    });
                    yPos++;
                }



            }

        }

        
        public void Draw()
        {
            // Process Events
            _window.DispatchEvents();
            _nodeWindow.DispatchEvents();

            _window.Clear(new Color(61,71,78));
            _nodeWindow.Clear(new Color(61,71,78));


            float _currentViewYPos = _nodeView.Center.Y;
            _nodeView.Move(new Vector2f(0,  ( _nodeViewPos - _currentViewYPos) / 10));
            _nodeWindow.SetView(_nodeView);

            DrawBackground();

            DrawNodeDisplay();

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



            foreach (Node<TileType> node in _data.Agent.AgentData.SearchedNodes)
            {
                _window.Draw(new CircleShape()
                {
                    Position = new Vector2f(
                            85 + (spacing.X) + node.Pos.X * (boxSize.X + spacing.X) + boxSize.X / 2 - 10,
                            60 + (spacing.Y) + node.Pos.Y * (boxSize.Y + spacing.Y) + boxSize.Y / 2 - 5) ,
                    Radius = 10,
                    FillColor = HashedPosColor(node.Pos.X, node.Pos.Y, 100)
                });


                if (node.Cost != 0)
                {
                    _window.Draw(new Text(node.Cost.ToString(), _font, 15)
                    {
                        Position = new Vector2f(
                            85 + 5 + (spacing.X) + node.Pos.X * (boxSize.X + spacing.X) + boxSize.X / 2 - 10,
                            60 + (spacing.Y) + node.Pos.Y * (boxSize.Y + spacing.Y) + boxSize.Y / 2 - 5),
                        FillColor = new Color(220, 220, 220)

                    });
                }
                    

            }

            foreach (Vector2i node in _data.Agent.AgentData.PosToSearch)
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

            /*
            foreach (Vector2i node in _data.AgentData.Path)
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
            */
            /* Draw out the heatmap path */

            for (int i = 0; i < _data.Agent.AgentData.Path.Count - 1; i++)
            {
                Vector2i prev = _data.Agent.AgentData.Path[i];
                Vector2i curr = _data.Agent.AgentData.Path[i + 1];

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
                        (byte)(255 * i / _data.Agent.AgentData.Path.Count),
                        0,
                       (byte)(255 - 255 * i / _data.Agent.AgentData.Path.Count)
                    )

                };

                _window.Draw(rectLine);

            }

            _window.Display();
            _nodeWindow.Display();
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

        private void Window_MouseWheeled(object sender, SFML.Window.MouseWheelScrollEventArgs e)
        {
            var window = (SFML.Window.Window)sender;

            _nodeViewPos += -40 * e.Delta;
            if (_nodeViewPos < 576 / 2)
                _nodeViewPos = 576 / 2;

            //_nodeView.Move(new Vector2f(0, -30 * e.Delta));

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
