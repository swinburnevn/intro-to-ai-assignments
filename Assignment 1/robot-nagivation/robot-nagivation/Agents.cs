using System;
using System.Collections.Generic;
using System.Text;

namespace robot_nagivation
{
    public interface IAgent
    {
        public AgentActions next(Percepts percepts);
    }

    public class RandomAgent : IAgent
    {
        public AgentActions next(Percepts percepts)
        {
            Array possibleActions = Enum.GetValues(typeof(AgentActions));
            return (AgentActions)possibleActions.GetValue(new Random().Next(0, possibleActions.Length));
        }
    }




}


