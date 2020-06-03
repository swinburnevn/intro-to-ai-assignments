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
        Inferring,
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

        

        public KnowledgeBase(string ask, string tell)
        {
            _ask = ask.Trim();
            _tell = tell.Trim();
        }


        public abstract bool Entails(string goal);

        #region Inherited Methods

        



        #endregion



        public string Tell { get => _tell; set => _tell = value; }
        public string Ask { get => _ask; set => _ask = value; }
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
