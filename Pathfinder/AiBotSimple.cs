using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class AiBotSimple : AiBotBase
    {
        Random rnd;
        public AiBotSimple(int x, int y) : base(x, y)
        {
            rnd = new Random();
        }

        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            bool ok = false;
            Coord2 newPos = GridPosition;// = new Coord2();
            while (!ok)
            {
                if (newPos == plr.GridPosition)
                {
                    ok = true;
                }
                else if (newPos.X > plr.GridPosition.X)
                {
                    newPos.X -= 1;
                }
                else if (newPos.X < plr.GridPosition.X)
                {
                    newPos.X += 1;
                }
                else if (newPos.Y > plr.GridPosition.Y)
                {
                    newPos.Y -= 1;
                }
                else if (newPos.Y < plr.GridPosition.Y)
                {
                    newPos.Y += 1;
                }

                //pos = GridPosition;
                //int x = rnd.Next(0, 4);
                //switch (x)
                //{
                //    case (0):
                //        pos.X += 1;
                //        break;
                //    case (1):
                //        pos.X -= 1;
                //        break;
                //    case (2):
                //        pos.Y += 1;
                //        break;
                //    case (3):
                //        pos.Y -= 1;
                //        break;
                //}
                ok = SetNextGridPosition(newPos, level);
            }
        }
    }
}
