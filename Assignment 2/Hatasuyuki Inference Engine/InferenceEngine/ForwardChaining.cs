using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{

    public class ForwardChaining : KnowledgeBase
    {
        private List<string> _knowledgeBase;     // E.g. A, E, B, C
        private List<string> _clauses;   // F & B => Z, C & D => F, A => D
        private Dictionary<string, int> _clauseCounts; // keeps a track of each clauses' fact count
        private Queue<string> _agenda;

        private string _solution;

        public List<string> KnowledgeBase { get => _knowledgeBase; set => _knowledgeBase = value; }
        public List<string> Clauses { get => _clauses; set => _clauses = value; }
        public Dictionary<string, int> ClauseCounts { get => _clauseCounts; set => _clauseCounts = value; }
        public Queue<string> Agenda { get => _agenda; set => _agenda = value; }
        public string Solution { get => _solution; }

        public ForwardChaining(string ask, string tell) : base(ask, tell)
        {
            Type = KnowledgeBaseType.FC;


            _knowledgeBase = new List<string>(); // This essentially becomes our knowledgebase
            _clauses = new List<string>();
            _clauseCounts = new Dictionary<string, int>();
            _agenda = new Queue<string>();

        }

        public void Initialise() { Initialise(Tell); }
        public void Initialise(string tell)
        {
            string[] sentences = tell.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in sentences)
            {
                if (s.Contains("=>"))
                {
                    // S is a Sentence
                    _clauses.Add(s.Trim());
                    // Split, add the number of facts required in the LHS
                    int LHSCount = s.Split("=>", StringSplitOptions.RemoveEmptyEntries)[0]
                        .Split("&", StringSplitOptions.RemoveEmptyEntries).Count();
                    _clauseCounts.Add(s.Trim(), LHSCount);
                    
                }
                else
                {
                    // S is a Fact
                    _agenda.Enqueue(s.Trim()); // Here we want to add facts to our system to consider
                }
            }
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

        // Return whether or not the statement is entailled
        public override bool Entails(string goal)
        {
            Initialise(); // We want to reset our KB's agendas and set up the sentences and clauses

            // While we still have Facts to evaluate
            while (_agenda.Count > 0)
            {
                string fact = _agenda.Dequeue();
                // We can add the fact to our KB
                if (!_knowledgeBase.Contains(fact))
                {
                    _knowledgeBase.Add(fact);


                    foreach (string s in _clauses)
                    {
                        // We want to remove the count if it's contained in any clause

                        if (ImpliedBy(s, fact))
                        {
                            // Grab the count of the particular clause and remove 1
                            _clauseCounts[s]--;
                            int factCount = _clauseCounts[s];


                            if (factCount <= 0)
                            {
                                // We have satisfied the LHS and therefore the RHS is true
                                string RHS = s.Split("=>", StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                                // Add to agenda and move on
                                _agenda.Enqueue(RHS);
                                // Check for if this is the goal
                                if (RHS == goal)
                                {
                                    _knowledgeBase.Add(RHS);
                                    return true;
                                }
                                    
                            }

                        }

                    }
                }

            }
            // If we cannot find any (reachable) sentence that implies the goal, then it is unreachable.
            return false;
        }


        // Returns true if the fact appears on the LHS (premise) of the sentence
        public bool ImpliedBy(string clause, string fact)
        {
            string[] LHS = clause.Split("=>", StringSplitOptions.RemoveEmptyEntries);
            return LHS[0].Contains(fact);
        }

    }
}
