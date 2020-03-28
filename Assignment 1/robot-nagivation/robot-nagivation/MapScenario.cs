using System;
using System.Collections.Generic;
using System.Text;

namespace robot_nagivation
{

    public class MapNode : StateData
    {
        public override int GetCost()
        {
            throw new NotImplementedException();
        }
    }
    public class MapScenario : Scenario<MapNode>
    {
        public override List<MapNode> DeterminePossibleMoves(State<MapNode> state)
        {
            throw new NotImplementedException();
        }

        public override bool isSolved(MapNode data)
        {
            throw new NotImplementedException();
        }
    }
}
