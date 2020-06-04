using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace InferenceEngine
{
    public class TruthTable : KnowledgeBase
    {
        private Dictionary<string, List<bool>> _truthTable;

        private List<string> _sentences;
        private List<string> _symbols;
        private List<string> _facts;

        private List<string> _models;
        private List<int> _modelRows;
        private Dictionary<string, OperationFunction> _operations;

        private string _solution;

        public TruthTable(string ask, string tell) : base(ask, tell)
        {

            Type = KnowledgeBaseType.TT;

            _truthTable = new Dictionary<string, List<bool>>();
            _models = new List<string>();
            _facts = new List<string>();
            _modelRows = new List<int>();
            _sentences = new List<string>();
            _symbols = new List<string>();

            _operations = new Dictionary<string, OperationFunction>();
            _operations.Add("&", new OperationAnd());
            _operations.Add("=>", new OperationImplies());
        }

        public List<string> ConvertIntoSentences(string tell)
        {
            List<string> sentences = new List<string>();

            string[] splitSentences = tell.Split(";");

            foreach (string s in splitSentences)
            {
                if (s != "")
                {
                    if (s.Contains("=>"))
                    {
                        sentences.Add(s.Trim());
                    }                   
                    else
                    {
                        _facts.Add(s.Trim());
                    }
                }
                    
            }
            return sentences;
        }

        public List<string> ConvertIntoOperators(string tell, bool allowDuplicates = false)
        {
            List<string> operators = new List<string>();

            foreach (KeyValuePair<string, OperationFunction> entry in _operations)
            {
                if (tell.Contains(entry.Key))
                    operators.Add(entry.Key);
            }

            return operators;
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


        public override bool Entails(string goal)
        {
            _sentences = ConvertIntoSentences(Tell);
            _symbols = ConvertIntoSymbols(Tell);


            /* ----------------- Truth Table (Symbols) Generation ----------------- */
            int numberOfRows = (int)Math.Pow(2, Symbols.Count);

            for (int i = 0; i < Symbols.Count; i++)
            {
                List<bool> symbolCol = new List<bool>();
                int index = 0;
                int stepOverRows = (int)(numberOfRows / (Math.Pow(2, i + 1)));
                bool currentPlacement = true;
                for (int j = 0; j < numberOfRows; j++)
                {
                    symbolCol.Add(currentPlacement);
                    index++;
                    if (index >= stepOverRows)
                    {
                        index = 0;
                        currentPlacement = !currentPlacement;
                    }
                }
                _truthTable.Add(Symbols[i], symbolCol);
            }

            /* ----------------- Truth Table (Sentences) Generation ----------------- */

            foreach (string sentence in Sentences)
            {
                //Evaluate the logic of the sentence
                string[] splitSentence = sentence.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                string LHS = splitSentence[0].Trim(); // This may contain some sort of operator
                string RHS = splitSentence[1].Trim(); // This is always one symbol

                // We can count the number of Symbols on the LHS
                List<string> localSymbols = ConvertIntoSymbols(LHS);

                List<bool> sentenceColumn = new List<bool>();

                for (int i = 0; i < numberOfRows; i++)
                {

                    // We want to evaluate the AND of all symbols in LocalSymbols, 

                    bool resolvedValue = true;
                    foreach (string symbol in localSymbols)
                    {
                        // truthTable [Sentence/Symbol][Row Number]
                        resolvedValue = _truthTable[symbol][i] && resolvedValue;
                    }

                    //Resolved value is the AND of all the LHS symbols
                    bool conclusion = (!resolvedValue || _truthTable[RHS][i]);
                    sentenceColumn.Add(conclusion);

                }
                // Add the new Column into the Truth Table
                _truthTable.Add(sentence, sentenceColumn);

                /*


                if (splitSentence.Length > 1)
                {
                    

                    // LHS is likey to have operator and therefore we need to parse the number of symbols:
                    List<string> LHSSymbols = ConvertIntoSymbols(LHS);
                    int numberOfLHSSymbols = LHSSymbols.Count;
                    List<string> LHSOperators = ConvertIntoOperators(LHS, false);

                    List<bool> sentenceCol = new List<bool>();

                    // Evaluate all numberOfRows rows
                    for (int i = 0; i < numberOfRows; i++)
                    {

                        if (LHSOperators.Count > 0)
                        {
                            Queue<string> symbolsQueue = new Queue<string>(LHSSymbols);
                            Queue<string> operatorsQueue = new Queue<string>(LHSOperators);

                            bool resolvedValue;
                            resolvedValue = _truthTable[symbolsQueue.Dequeue()][i];
                            while (symbolsQueue.Count > 1)
                            {
                                if (operatorsQueue.Count > 1)
                                {
                                    string nextOperator = operatorsQueue.Dequeue();

                                    resolvedValue = _operations[nextOperator].Evaluate(
                                        resolvedValue, _truthTable[symbolsQueue.Dequeue()][i]);
                                }
                            }

                            bool localLHS = resolvedValue;

                            sentenceCol.Add(_operations["=>"].Evaluate(localLHS, _truthTable[RHS][i]));
                        }
                        else
                        {
                            sentenceCol.Add(_operations["=>"].Evaluate(_truthTable[LHS][i], _truthTable[RHS][i]));
                        }

                    }

                    _truthTable.Add(sentence, sentenceCol);
                }
                */
            }
            

            /* ----------------- Truth Table (Ask) Generation ----------------- */

            // Now that the entire truth table is complete, we need to check for every model that:
            //  = ASK || (foreach prop is true)
            for (int i = 0; i < numberOfRows; i++)
            {
                bool isSatisfied = true;
                foreach (string sentence in Sentences)
                {
                    isSatisfied = isSatisfied && _truthTable[sentence][i];
                }

                foreach (string fact in _facts)
                {
                    isSatisfied = isSatisfied && _truthTable[fact][i];
                }

                if (isSatisfied && _truthTable[Ask][i])
                {
                    // We have found a model that satisfies the sentences AND the ASK:
                    string model = "";
                    foreach(string s in Symbols)
                    {
                        model += ($"{s}: {_truthTable[s][i]}; ");
                    }

                    foreach (string s in Sentences)
                    {
                        model += ($"{s}: {_truthTable[s][i]}; ");
                    }
                    _models.Add(model);
                    _modelRows.Add(i);
                }

            }

            return (_models.Count >= 0);
        }

        public override string Execute()
        {
            string output = "";

            if (Entails(Ask))
            {
                output = "YES:" + _models.Count;
            }
            else
            {
                output = "NO";
            }
            _solution = output;
            return output;
        }

        public List<string> Sentences { get => _sentences; set => _sentences = value; }
        public List<string> Symbols { get => _symbols; set => _symbols = value; }

        public string Solution { get => _solution; }
        public Dictionary<string, List<bool>> TruthTableRef { get => _truthTable; set => _truthTable = value; }
        public List<int> ModelRows { get => _modelRows; set => _modelRows = value; }
    }
}