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
        private Queue<string> _agenda;
        private List<string> _knowledgeBase;

        private string _solution;

        public List<string> Facts { get => _facts; set => _facts = value; }
        public List<string> Clauses { get => _clauses; set => _clauses = value; }
        public List<string> KnowledgeBase { get => _knowledgeBase; set => _knowledgeBase = value; }
        public Queue<string> Agenda { get => _agenda; set => _agenda = value; }
        public string Solution { get => _solution; }

        public BackwardChaining(string ask, string tell) : base(ask, tell)
        {
            Type = KnowledgeBaseType.BC;

            _facts = new List<string>();         // Initial facts of the system
            _clauses = new List<string>();       // sentences to process
            _agenda = new Queue<string>();       // list of symbols to process
            _knowledgeBase = new List<string>(); // list of known facts
        }
        
        // Returns the symbols on the LHS of the sentence 
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
            // Initialise DB by enqueing starting proposition and adding in required
            //  Sentences and logic
            Initialise();

            while (_agenda.Count >= 1)
            {
                // Taking the first element of the queue
                string query = _agenda.Dequeue();
                _knowledgeBase.Add(query);

                // ... if it is not entailed by our current facts
                if (!_facts.Contains(query))
                {
                    // We want to make a list of any additional clauses to confirm
                    List<string> clausesToConfirm = new List<string>();

                    // Checking all known clauses
                    foreach(string clause in _clauses)
                    {
                        // to see if it is concluded by the query
                        if (Concludes(clause, query))
                        {
                            List<string> premiseSymbols = GetPremiseSymbols(clause);
                            foreach (string symbol in premiseSymbols)
                            {
                                //... we add the premise symbols to clauses to confirm
                                clausesToConfirm.Add(symbol);
                            }
                        }
                    }

                    // if there are no additonal clauses to evaluate, we have determined that the premise (ask)
                    //  is unreachable and therefore is false.
                    if (clausesToConfirm.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        // Otherwise we want to confirm each of the clauses, so we enqueue them
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

        // Initialises the Knowledgebase to the given TELL
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
            _solution = output;
            return output;
        }
    }
    

}
