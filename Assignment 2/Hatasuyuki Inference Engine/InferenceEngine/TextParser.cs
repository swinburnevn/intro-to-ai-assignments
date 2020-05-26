using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace InferenceEngine
{
    /// <summary>
    /// Give parser input file, ask for Parser.Ask() and Parser.Tell()
    /// </summary>
    public class TextParser
    {
        private string _ask;
        private string _tell;
        private string _fileName;
        public TextParser(string fileName)
        {
            _fileName = fileName;
            Parse();
        }

        public void Parse()
        {
            try
            {
                StreamReader _reader = new StreamReader(_fileName);
                string line;

                while (!_reader.EndOfStream)
                {
                    line = _reader.ReadLine();
                    if (line.ToLower() == "tell")
                    {
                        _tell = _reader.ReadLine();
                    } 
                    else // Assumed to be ASK
                    {
                        _ask = _reader.ReadLine();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception returned at file parser: " + e.Message);
            }


        }

        public string Ask { get => _ask; set => _ask = value; }
        public string Tell { get => _tell; set => _tell = value; }
    }
}
