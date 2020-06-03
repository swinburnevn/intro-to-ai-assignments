using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace InferenceEngine
{

    public class BackwardChaining : KnowledgeBase
    {

        private List<string> _facts;     // E.g. A, E, B, C
        private List<string> _clauses;   // F & B => Z, C & D => F, A => D
        private Dictionary<string, int> _clauseCounts; // keeps a track of each clauses' fact count
        private Queue<string> _agenda;
        private List<string> _knowledgeBase;

        public BackwardChaining(string ask, string tell) : base(ask, tell)
        {
            _facts = new List<string>(); // This essentially becomes our knowledgebase
            _clauses = new List<string>();
            _clauseCounts = new Dictionary<string, int>();
            _agenda = new Queue<string>();
            _knowledgeBase = new List<string>();
        }

        public List<string> GetPremiseSymbols(string clause)
        {
            string LHS = clause.Split("=>")[0].Trim();
            string[] LHSSymbols = LHS.Split("&");

            List<string> output = new List<string>();

            foreach (string s in LHSSymbols)
            {
                output.Add(s.Trim());
            }
            return output;
        }

        // The backward chaining algorithm, yay!
        public override bool Entails(string goal)
        {
            Initialise();

            while (_agenda.Count >= 1)
            {
                string query = _agenda.Dequeue();
                _knowledgeBase.Add(query);

                if (!_facts.Contains(query))
                {
                    

                    List<string> clausesToConfirm = new List<string>();
                    foreach(string clause in _clauses)
                    {
                        if (Concludes(clause, query))
                        {
                            List<string> premiseSymbols = GetPremiseSymbols(clause);
                            foreach (string symbol in premiseSymbols)
                            {
                                clausesToConfirm.Add(symbol);
                            }
                        }
                    }

                    if (clausesToConfirm.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (string symbol in clausesToConfirm)
                        {
                            if (!_knowledgeBase.Contains(symbol))
                                _agenda.Enqueue(symbol);
                        }
                    }

                }
                
                
            }

            return true;


        }

        public void Initialise() { Initialise(Tell); }
        public void Initialise(string tell)
        {
            _agenda.Enqueue(Ask);
            string[] sentences = tell.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in sentences)
            {
                if (s.Contains("=>"))
                {
                    // S is a Sentence
                    _clauses.Add(s.Trim());

                }
                else
                {
                    // S is a Fact
                    _facts.Add(s.Trim());
                }
            }
        }

        // Returns true if the fact appears on the RHS (Conclusion) of the sentence
        public bool Concludes(string clause, string fact)
        {
            string[] LHS = clause.Split("=>", StringSplitOptions.RemoveEmptyEntries);
            return LHS[1].Contains(fact);
        }


        public override string Execute()
        {
            string output = "";

            if (Entails(Ask))
            {
                output = "YES:";
                for (int i = 0; i < _knowledgeBase.Count; i++)
                {
                    output += " " + _knowledgeBase[i];
                    if (i < _knowledgeBase.Count - 1)
                    {
                        output += ",";
                    }
                }
            }
            else
            {
                output = "NO";
            }

            return output;
        }
    }
    

}
