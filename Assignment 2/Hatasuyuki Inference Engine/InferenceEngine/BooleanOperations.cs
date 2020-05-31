using System;
using System.Collections.Generic;
using System.Text;

namespace InferenceEngine
{
    public abstract class OperationFunction
    {
        public abstract bool Evaluate(bool a, bool b);
    }

    public class OperationAnd : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            return a && b;
        }
    }

    public class OperationOr : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            return a || b;
        }
    }

    public class OperationImplies : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            return (!a || b);
        }
    }

    public class OperationNand : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            return ! (a && b);
        }
    }

    public class OperationNor : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            return ! (a || b);
        }
    }

    public class OperationInv : OperationFunction
    {
        public override bool Evaluate(bool a, bool b)
        {
            if (b)
                return !a;
            else return a;
        }
    }

}
