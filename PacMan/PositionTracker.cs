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
            Console.WriteLine("Saving position {0} {1}", coordinates[0], coordinates[1]);
            ghostPositions[o.GetHashCode()] = coordinates[0]+"_"+coordinates[1];
        }

        public bool isGhostPosition(int[] coordinates)
        {
            Console.WriteLine("looking for position {0} {1}", coordinates[0], coordinates[1]);
            return ghostPositions.ContainsValue(coordinates[0]+"_"+coordinates[1]);
        }

    }
}
