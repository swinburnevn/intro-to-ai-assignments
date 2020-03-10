using System;
using System.IO;

namespace tutorial_2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            StreamReader reader = new StreamReader("basic-map.txt");
            string line;


            line = reader.ReadLine(); // read size of array
            string size = line.Substring(1, line.Length - 2);


            
            


            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(' ');
            }


        }
    }
}
