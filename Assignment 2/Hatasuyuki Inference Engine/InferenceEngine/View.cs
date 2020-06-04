using System;
using System.Collections.Generic;
using System.Text;

using SFML.System;
using SFML.Graphics;
using System.Globalization;
using System.Xml.Schema;

namespace InferenceEngine
{

    public abstract class View
    {
        private KnowledgeBase _kb;
        public View(ref KnowledgeBase kb)
        {
            _kb = kb;
        }

        public KnowledgeBase KB { get => _kb; set => _kb = value; }

        public abstract void Close();
        public abstract void Draw();


    }

    public enum KnowledgeBaseType { TT, BC, FC }


    public class SFMLView : View
    {
        
        private RenderWindow _window;
        private Vector2i _windowSize = new Vector2i(1024, 576);

        private Font _font;
        private Font _titleFont;

        private bool _isFinished = false;

        

        public SFMLView(ref KnowledgeBase kb) : base(ref kb)
        {
            _font = new Font("express.ttf");
            _titleFont = new Font("VtksHunt.ttf");

            _window = new RenderWindow(
                 new SFML.Window.VideoMode(1024, 576),
                 "Hatsuyuki Inference Engine");

            _window.KeyPressed += Window_KeyPressed;
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {

                window.Close();
                _isFinished = true;
            }
        }

        

        public override void Close()
        {
            
        }

        public override void Draw()
        {
            // Process Events
            _window.DispatchEvents();

            _window.Clear(new Color(61, 71, 78));


            _window.Draw(new Text("Hatsuyuki", _titleFont, 45)
            {
                Position = new Vector2f(715, 55),
                FillColor = new Color(20, 20, 20)

            });
            _window.Draw(new Text("Inference Engine", _font, 35)
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

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(95, 10),
                Size = new Vector2f(870, 40),
                FillColor = new Color(26, 26, 28)
            });
            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(105, 20),
                Size = new Vector2f(860, 30),
                FillColor = new Color(40, 40, 40)
            });

            _window.Draw(new Text($"Tell: {KB.Tell} --  Ask: {KB.Ask}", _font, 16)
            {
                Position = new Vector2f(110, 25),
                FillColor = new Color(252, 187, 116)

            });

            _window.Draw(new Text($"Database Type: {KB.Type}; Press ESC to exit", _font, 16)
            {
                Position = new Vector2f(110, 535),
                FillColor = new Color(252, 187, 116)

            });

            switch(KB.Type)
            {
                case KnowledgeBaseType.TT:
                    TTDraw();
                    break;

                case KnowledgeBaseType.BC:
                    BCDraw();
                    break;

                case KnowledgeBaseType.FC:
                    FCDraw();
                    break;
            }


            _window.Display();


        }

        public void BCDraw()
        {
            BackwardChaining _bckb = (BackwardChaining)KB;

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 90),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 205),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 360),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });


            _window.Draw(new Text("Knowledge Base", _font, 14)
            {
                Position = new Vector2f(110, 70),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Determined Facts", _font, 14)
            {
                Position = new Vector2f(110, 184),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Solution", _font, 14)
            {
                Position = new Vector2f(110, 330),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Agenda", _font, 14)
            {
                Position = new Vector2f(740, 160),
                FillColor = new Color(220, 220, 220)

            });

            List<string> listAgenda = new List<string>(_bckb.Agenda);

            int _posPointer = 0;
            for (int i = listAgenda.Count; (i > listAgenda.Count - 15) && (i >= 1); i--)
            {

                _window.Draw(new Text(i + " : " + listAgenda[i - 1].ToString(), _font, 14)
                {
                    Position = new Vector2f(740, 185 + _posPointer * 22),
                    FillColor = new Color(190, 190, 190)

                });
                _posPointer++;
            }

            _posPointer = 0;
            for (int i = _bckb.KnowledgeBase.Count; (i > _bckb.KnowledgeBase.Count - 15) && (i >= 1); i--)
            {

                _window.Draw(new Text(_bckb.KnowledgeBase[i - 1].ToString(), _font, 12)
                {
                    Position = new Vector2f(130 + (int)(_posPointer / 5) * 120, 100 + (_posPointer % 5) * 14),
                    FillColor = new Color(10, 10, 10)

                });
                _posPointer++;
            }

            _posPointer = 0;

            for (int i = _bckb.Facts.Count; (i > _bckb.Facts.Count - 15) && (i >= 1); i--)
            {

                _window.Draw(new Text(_bckb.KnowledgeBase[i - 1].ToString(), _font, 12)
                {
                    Position = new Vector2f(130 + (int)(_posPointer / 5) * 120, 215 + (_posPointer % 5) * 14),
                    FillColor = new Color(10, 10, 10)

                });
                _posPointer++;
            }

            _window.Draw(new Text(_bckb.Solution, _font, 14)
            {
                Position = new Vector2f(130, 370),
                FillColor = new Color(10, 10, 10)

            });


        }

        public void FCDraw()
        {
            ForwardChaining _fckb = (ForwardChaining)KB;

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 90),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 205),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });

            _window.Draw(new RectangleShape()
            {
                Position = new Vector2f(110, 360),
                Size = new Vector2f(520, 90),
                FillColor = new Color(190, 190, 190)
            });


            _window.Draw(new Text("Knowledge Base", _font, 14)
            {
                Position = new Vector2f(110, 70),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Clauses and their Counts", _font, 14)
            {
                Position = new Vector2f(110, 184),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Solution", _font, 14)
            {
                Position = new Vector2f(110, 330),
                FillColor = new Color(190, 190, 190)

            });

            _window.Draw(new Text("Agenda", _font, 14)
            {
                Position = new Vector2f(740, 160),
                FillColor = new Color(220, 220, 220)

            });

            List<string> listAgenda = new List<string>(_fckb.Agenda);

            int _posPointer = 0;
            for (int i = listAgenda.Count; (i > listAgenda.Count - 15) && (i >= 1); i--)
            {

                _window.Draw(new Text(i + " : " + listAgenda[i-1].ToString(), _font, 14)
                {
                    Position = new Vector2f(740, 185 + _posPointer * 22),
                    FillColor = new Color(190, 190, 190)

                });
                _posPointer++;
            }

            _posPointer = 0;
            for (int i = _fckb.KnowledgeBase.Count; (i > _fckb.KnowledgeBase.Count - 15) && (i >= 1); i--)
            {

                _window.Draw(new Text(_fckb.KnowledgeBase[i - 1].ToString(), _font, 12)
                {
                    Position = new Vector2f(130 + (int)(_posPointer / 5) * 120, 100 + (_posPointer % 5) * 14),
                    FillColor = new Color(10, 10, 10)

                });
                _posPointer++;
            }

            _posPointer = 0;
            foreach (KeyValuePair<string, int> entry in _fckb.ClauseCounts)
            {


                _window.Draw(new Text(entry.Key + "  : " + entry.Value, _font, 12)
                {
                    Position = new Vector2f(130 + (int)(_posPointer / 5) * 120, 215 + (_posPointer % 5) * 14),
                    FillColor = new Color(10, 10, 10)

                });
                _posPointer++;
            }

            _window.Draw(new Text(_fckb.Solution, _font, 14)
            {
                Position = new Vector2f(130, 370),
                FillColor = new Color(10, 10, 10)

            });
        }

        public void TTDraw()
        {
            TruthTable _ttkb = (TruthTable)KB;

            Color truthColor = new Color(69, 175, 32);
            Color falseColor = new Color(231, 71, 21);
            Dictionary<bool, Color> TTColors = new Dictionary<bool, Color>();
            TTColors.Add(true, truthColor);
            TTColors.Add(false, falseColor);

            _window.Draw(new Text("Truth Table Models", _font, 20)
            {
                Position = new Vector2f(110, 70),
                FillColor = new Color(220, 220, 220)
            });

            _window.Draw(new Text("Solution", _font, 20)
            {
                Position = new Vector2f(110, 400),
                FillColor = new Color(220, 220, 220)
            });

            int _posPointer = 0;
            int _xPosPointer = 0;

            foreach (int row in _ttkb.ModelRows)
            {
                _window.Draw(new Text($"Model {row}", _font, 11)
                {
                    Position = new Vector2f(200 + 50 * _xPosPointer, 100),
                    FillColor = new Color(220, 220, 220)
                });
                _xPosPointer++;
            }

            _xPosPointer = 0;
            _posPointer = 0;



            foreach (KeyValuePair<string, List<bool>> entry in _ttkb.TruthTableRef)
            {
                _window.Draw(new Text(entry.Key, _font, 11)
                {
                    Position = new Vector2f(110, 120 + 12 * _posPointer),
                    FillColor = new Color(220, 220, 220)
                });
                _posPointer++;
                _xPosPointer = 0;

                foreach (int row in _ttkb.ModelRows)
                {
                    
                    _window.Draw(new Text(entry.Value[row].ToString(), _font, 11)
                    {
                        Position = new Vector2f(200 + 50 * _xPosPointer, 110 + 12 * _posPointer),
                        FillColor = TTColors[entry.Value[row]]
                    });
                    _xPosPointer++;
                }

            }

            _window.Draw(new Text(_ttkb.Solution, _font, 14)
            {
                Position = new Vector2f(110, 430),
                FillColor = new Color(220, 220, 220)
            });




        }



        public bool IsFinished { get => _isFinished; }
    }
}
