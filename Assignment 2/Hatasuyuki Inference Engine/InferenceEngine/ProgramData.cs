using System;
using System.Collections.Generic;
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

            string[] splitSentences = Regex.Split(tell, @"[\s.,;&=>~]+");

            foreach (string s in splitSentences)
            {
                if (s != "" && !symbols.Contains(s))
                    symbols.Add(s.Trim());
            }
            return symbols;
        }

        public List<string> ConvertIntoOperators(string tell, bool allowDuplicates = false)
        {
            List<string> operators = new List<string>();
            string allSymbols = "\\Q";
            foreach (string symbol in ConvertIntoSymbols(tell))
                allSymbols += symbol;
            allSymbols += "\\E";

            string[] splitSentences = Regex.Split(tell, @"[\s" + allSymbols + "]+");

            foreach (string s in splitSentences)
            {
                if (s != "" && !operators.Contains(s))
                    if (!allowDuplicates)
                        operators.Add(s.Trim());
            }
            return operators;
        }




        #endregion



        public string Tell { get => _tell; set => _tell = value; }
        public string Ask { get => _ask; set => _ask = value; }
        public List<string> Sentences { get => _sentences; set => _sentences = value; }
        public List<string> Symbols { get => _symbols; set => _symbols = value; }

        public abstract string Execute();
    }

    


    

    public class TT : KnowledgeBase
    {
        // We need a 2D matrix, hence, list of dicts?:

        private Dictionary<string, List<bool>> _truthTable;

        private Dictionary<string, OperationFunction> _operations;


        public TT(string ask, string tell) : base(ask, tell)
        {
            _operations = new Dictionary<string, OperationFunction>();
            _operations.Add("&", new OperationAnd());
            _operations.Add("||", new OperationOr());
            _operations.Add("=>", new OperationImplies());
        }

        public bool IterativelyParseOperator(ref List<string> operators, ref List<string> symbols, bool key, int index)
        {
            // Sym: B C
            // Opr: & & ~

            if (symbols.Count > 2)
            {
                string localSymbol = symbols[0];
                
                string localOperator = operators[0];
                symbols.RemoveAt(0);
                operators.RemoveAt(0);

                return _operations[localOperator].Evaluate(IterativelyParseOperator(ref operators, ref symbols, key, index),
                    _truthTable[localSymbol][index]) ;
            }
            else
            {
                // This means that we're on the final element, return this value
                return _truthTable[symbols[0]][index];
            }


            return false;
        }


        public override string Execute()
        {
            Sentences = ConvertIntoSentences(Tell);
            Symbols = ConvertIntoSymbols(Tell);

            _truthTable = new Dictionary<string, List<bool>>();


            /* ----------------- Truth Table (Symbols) Generation ----------------- */
            int numberOfRows = (int)Math.Pow(2, Symbols.Count);

            for (int i = 0; i < Symbols.Count; i++)
            {
                List<bool> symbolCol = new List<bool>();
                int index = 0;
                int stepOverRows = (int)(numberOfRows / (Math.Pow(2,i+1)));
                bool currentPlacement = true;
                for (int j = 0; j < numberOfRows; j++)
                {
                    symbolCol.Add(currentPlacement);
                    index++;
                    if (index>= stepOverRows)
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

                List<string> localSymbols = ConvertIntoSymbols(sentence);


                string[] splitSentence = sentence.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                if (splitSentence.Length > 1) 
                {
                    string LHS = splitSentence[0].Trim(); // This may contain some sort of operator
                    string RHS = splitSentence[1].Trim(); // This is always one symbol

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
                            //Assuming 1 operator for now

                            //bool localLHS = _operations[LHSOperators[0]].Evaluate(_truthTable[LHSSymbols[0]][i], _truthTable[LHSSymbols[1]][i]);

                            //string localSymbol = LHSSymbols[0];
                            //LHSSymbols.RemoveAt(0); // Remove first symbol as it's placed inside the parser
                            //bool localLHS = IterativelyParseOperator(ref LHSOperators, ref LHSSymbols, _truthTable[localSymbol][i], i);

                            Queue<string> symbolsQueue = new Queue<string>(LHSSymbols);
                            Queue<string> operatorsQueue = new Queue<string>(LHSOperators);

                            bool resolvedValue;
                            resolvedValue = _truthTable[symbolsQueue.Dequeue()][i];
                            while (symbolsQueue.Count > 0)
                            {
                                resolvedValue = _operations[operatorsQueue.Dequeue()].Evaluate(
                                    resolvedValue, _truthTable[symbolsQueue.Dequeue()][i]);
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
                
            }

            /* ----------------- Truth Table (Ask) Generation ----------------- */

            // Now that the entire truth table is complete, we need to check for every model that:
            //  = ASK || (foreach prop is true)

            int foundModels = 0;
            for (int i = 0; i < numberOfRows; i++)
            {
                bool isSatisfied = true;
                foreach(string sentence in Sentences)
                {
                    isSatisfied = isSatisfied && _truthTable[sentence][i];
                }

                if (isSatisfied && _truthTable[Ask][i])
                {
                    // We have found a model that satisfies the sentences AND the ASK:
                    foundModels++;
                    Console.WriteLine($"Model {i} found to satisfy, Truth is as follows:");
                    foreach (string symbol in Symbols)
                    {
                        Console.Write($"{symbol} : {_truthTable[symbol][i]}, ");
                    }
                    Console.Write("and therefore: ");
                    foreach (string sentence in Sentences)
                    {
                        Console.Write($"{sentence} : {_truthTable[sentence][i]}, ");
                    }
                    Console.WriteLine();
                }

            }
            if (foundModels > 0)
                return "Yes, " + foundModels;
            else
                return "No";
                

        }
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
