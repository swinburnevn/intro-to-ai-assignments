using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace InferenceEngine
{

    /*
     * Used to pass information between program and views.
     *  A discount MVC model for ease of expansion and flexibiltiy during development
     * 
     *  More notes here
     *  
     *  
     * 
     * 
     */

    public enum ProgramState
    {
        Infering,
        Finished
    }

    /*
     * The Knowledgebase will be held by the Program Data. Why?
     *   Simplicity's sake. That's why.
     *   
     *   Or rather, the object won't be called upon in the data itself, only the model does so, and therefore
     * 
     * Usage: Knowledgebase(ask, tell)
     * And then: Knowledgebase.execute();
     * 
     * Type of KB: TT/FC/BC
     * 
     * 
     */

    // Every KB must have this contract
    public abstract class KnowledgeBase
    {
        // Knowledgebase Inputs
        private string _tell;
        private string _ask;

        private List<string> _sentences;
        private List<string> _symbols;

        public KnowledgeBase(string ask, string tell)
        {
            _ask = ask.Trim();
            _tell = tell.Trim();
            _sentences = new List<string>();
            _symbols = new List<string>();
            
        }


        public abstract bool Entails(string goal);

        #region Inherited Methods

        public List<string> ConvertIntoSentences(string tell)
        {
            List<string> sentences = new List<string>();

            string[] splitSentences = tell.Split(";");
            
            foreach (string s in splitSentences)
            {
                if (s!="")
                    sentences.Add(s.Trim());
            }
            return sentences;
        }

        public List<string> ConvertIntoSymbols(string tell)
        {
            List<string> symbols = new List<string>();

            string[] splitSentences = Regex.Split(tell, @"[\s.,;&=>\\~\\|\\|]+");

            foreach (string s in splitSentences)
            {
                if (s != "" && !symbols.Contains(s))
                    symbols.Add(s.Trim());
            }
            return symbols;
        }       




        #endregion



        public string Tell { get => _tell; set => _tell = value; }
        public string Ask { get => _ask; set => _ask = value; }
        public List<string> Sentences { get => _sentences; set => _sentences = value; }
        public List<string> Symbols { get => _symbols; set => _symbols = value; }

        public abstract string Execute();
    }

    


    

    public class ProgramData
    {
        private KnowledgeBase _kb;


        public ProgramData()
        {
            // Nothing at the moment
        }

        public KnowledgeBase KB { get => _kb; set => _kb = value; }
    }
}
