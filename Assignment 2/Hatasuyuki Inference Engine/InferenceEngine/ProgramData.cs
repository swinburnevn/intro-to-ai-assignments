using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace InferenceEngine
{
    // Every KB must have this contract
    public abstract class KnowledgeBase
    {
        // Knowledgebase Inputs
        private string _tell;
        private string _ask;

        private KnowledgeBaseType type;

        public KnowledgeBase(string ask, string tell)
        {
            _ask = ask.Trim();
            _tell = tell.Trim();
        }

        public abstract bool Entails(string goal);

        public string Tell { get => _tell; set => _tell = value; }
        public string Ask { get => _ask; set => _ask = value; }
        public KnowledgeBaseType Type { get => type; set => type = value; }

        public abstract string Execute();
    }
}
