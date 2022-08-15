using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pathfinder
{
    class AiBotLRTAStar : AiBotBase
    {
        List<Coord2> path;

        //constructor: requires initial position
        public AiBotLRTAStar(int x, int y, int gridSize) : base(x, y)
        {

        }

        //This method takes the path created in the AStar algorithm and stores it within the bot class
        public void setPath(List<Coord2> pathToSet)
        {
            path = pathToSet;
        }


        //This method picks the next position on the grid to move to from the path created
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            if (path.Count > 0)
            {
                SetNextGridPosition(path[0], level);
                path.RemoveAt(0);
            }
        }
    }
  
    class Node
    {
        public Coord2 gridPosition;
        public int vertex;
        public double stateCost;
    }
}
