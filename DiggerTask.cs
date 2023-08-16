using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Digger;
using System.IO;

namespace Digger
{
    class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return -13523;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    class Player : ICreature
    {
        public int X;
        public int Y;
        public CreatureCommand Act(int x, int y)
        {
            
            var position = new CreatureCommand() { DeltaX = 0, DeltaY = 0 };

            switch (WASD())
            {
                case Keys.Up:
                    if (y - 1 < 0 || Game.Map[x, y - 1] is Sack)
                        break;
                    if(Game.Map[x,y-1]is Gold)
                        Game.Scores += 10;       
                    position.DeltaY--;
                    break;
                case Keys.Down:
                    if (y + 1 >= Game.MapHeight||Game.Map[x,y+1] is Sack)
                        break;
                    if (Game.Map[x, y + 1] is Gold)
                        Game.Scores += 10;
                    position.DeltaY++;
                    break;
                case Keys.Left:
                    if (x - 1 < 0 || Game.Map[x-1, y] is Sack)
                        break;
                    if (Game.Map[x-1, y] is Gold)
                        Game.Scores += 10;
                    position.DeltaX--;
                    break;
                case Keys.Right:
                    if (x + 1 >= Game.MapWidth || Game.Map[x+1,y] is Sack)
                        break;
                    if (Game.Map[x+1, y] is Gold)
                        Game.Scores += 10;
                    position.DeltaX++;
                    break;
            }
            return position;

        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack || conflictedObject is Monster)
                return true;
            return false;
        }

        public int GetDrawingPriority()
        {
            return -106346;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }

        public Keys WASD()
        {
            return Game.KeyPressed;
        }
    }

    class Sack : ICreature
    {
        public int X;
        public int Y;
        public int count;
        public CreatureCommand Act(int x, int y)
        {

            if (y + 1 < Game.MapHeight && Game.Map[x, y + 1] is null)
            {
                count++;
                return new CreatureCommand { DeltaX = 0, DeltaY = 1 };

            }
            else if (y + 1 < Game.MapHeight && count >= 1 && (Game.Map[x, y + 1] is Player || Game.Map[x, y + 1] is Monster))
            {
                count++;
                return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
            }
            else if (count > 1)
            {
                return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
            }
            else
            {
                count = 0;
                return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            
            return false;
        }

        public int GetDrawingPriority()
        {
            return -241;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    class Gold : ICreature
    {
        public int X;
        public int Y;
        public CreatureCommand Act(int x, int y)
        {
            
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {

            if (conflictedObject is Player||conflictedObject is Monster)
                return true;
            return false;
        }

        public int GetDrawingPriority()
        {
            return 3190;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    class Monster : ICreature
    {

        public bool IsPlayerOnMap(int[] coordinate)
        {
            if(coordinate[0]<0||coordinate[1]<0)
             return false;
            return true;
        }
        public int[] PlayerCoordinate()
        { 
            for (int i = 0; i < Game.MapHeight; i++)
            {
                for (int j = 0; j < Game.MapWidth; j++)
                {
                    if (Game.Map[j, i] is Player)
                        return new int[] { j, i };
                }
            }
            return new int[]{-1,-1};
        }
        public CreatureCommand Act(int x, int y)
        {
            var playerCoordinate = PlayerCoordinate();
            if (IsPlayerOnMap(playerCoordinate))
            {
                if (playerCoordinate[0] != x)
                {
                    if (x - 1 >= 0 && (Game.Map[x - 1, y] is null || Game.Map[x - 1, y] is Gold || Game.Map[x - 1, y] is Player))
                    {
                        return new CreatureCommand { DeltaX = -1, DeltaY = 0 };
                    }
                    if (x + 1 < Game.MapWidth && (Game.Map[x + 1, y] is null || Game.Map[x + 1, y] is Gold || Game.Map[x + 1, y] is Player))
                    {
                        return new CreatureCommand { DeltaX = 1, DeltaY = 0 };
                    }
                }
                else if (playerCoordinate[1] != y)
                {
                    if (y - 1 >= 0 && (Game.Map[x, y - 1] is null || Game.Map[x, y - 1] is Gold || Game.Map[x, y - 1] is Player))
                    {
                        return new CreatureCommand { DeltaX = 0, DeltaY = -1 };
                    }
                    if (y + 1 < Game.MapWidth && (Game.Map[x, y + 1] is null || Game.Map[x, y + 1] is Gold || Game.Map[x, y + 1] is Player))
                    {
                        return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
                    }
                }
                else { return new CreatureCommand { DeltaX = 0, DeltaY = 0 }; }
            }
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
            
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack || conflictedObject is Monster)
                return true;
            return false;
        }

        public int GetDrawingPriority()
        {
            return -1241251;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }
    }
}
