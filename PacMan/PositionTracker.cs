using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class PositionTracker
    {
        Dictionary<int, string> ghostPositions = new Dictionary<int, string>();


        public void putGhostPosition(object o, int[] coordinates)
        {
            if(ghostPositions.ContainsValue(coordinates[0]+"_"+coordinates[1]))
            {
                return;
            }
            ghostPositions[o.GetHashCode()] = coordinates[0]+"_"+coordinates[1];
        }

        public bool isGhostPosition(int[] coordinates)
        {
            return ghostPositions.ContainsValue(coordinates[0]+"_"+coordinates[1]);
        }

    }
}
