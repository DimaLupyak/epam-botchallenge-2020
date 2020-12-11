using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICanCode.Api
{
    [Serializable]
    public class Board : AbstractLayeredBoard
    {
        public Board(string boardString) : base(boardString)
        {
        }

        public Board(int size) : base(size)
        {
        }

        /**
        * @return Checks if your robot is alive.
        */
        public bool IsMeAlive()
        {
            return Get(Layers.LAYER2, Element.ROBO_FALLING, Element.ROBO_LASER).Count == 0;
        }

        public bool IsWallAt(Point p)
        {
            return IsWallAt(p.X, p.Y);
        }

        public bool IsHeroBarrierAt(Point p)
        {
            return IsLaserBarrierAt(p) || IsAt(p, Element.HOLE);
        }
        public bool IsLaserBarrierAt(Point p)
        {
            return IsWallAt(p) ||
                   IsAt(p, Element.BOX,
                       Element.LASER_MACHINE_READY_LEFT,
                       Element.LASER_MACHINE_CHARGING_RIGHT,
                       Element.LASER_MACHINE_READY_UP,
                       Element.LASER_MACHINE_READY_DOWN,
                       Element.LASER_MACHINE_CHARGING_LEFT,
                       Element.LASER_MACHINE_CHARGING_RIGHT,
                       Element.LASER_MACHINE_CHARGING_UP,
                       Element.LASER_MACHINE_CHARGING_DOWN);
        }




        /**
         * @param x X coordinate.
         * @param y Y coordinate.
         * @return Is it possible to go through the cell with {x,y} coordinates.
         */
        public bool IsWallAt(int x, int y)
        {
            return IsAt(Layers.LAYER1, x, y,
                Element.ANGLE_IN_LEFT,
                Element.WALL_FRONT,
                Element.ANGLE_IN_RIGHT,
                Element.WALL_RIGHT,
                Element.ANGLE_BACK_RIGHT,
                Element.WALL_BACK,
                Element.ANGLE_BACK_LEFT,
                Element.WALL_LEFT,
                Element.WALL_BACK_ANGLE_LEFT,
                Element.WALL_BACK_ANGLE_RIGHT,
                Element.ANGLE_OUT_RIGHT,
                Element.ANGLE_OUT_LEFT,
                Element.SPACE);

        }

        public void AddOnLayer4(int x, int y, char c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                Field[Layers.LAYER4, x, y] = c;

        }

        public void AddOnLayer2(int x, int y, char c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                Field[Layers.LAYER2, x, y] = c;
        }

        public void AddOnLayer5(int x, int y, char c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                Field[Layers.LAYER5, x, y] = c;
        }

        public bool IsAt(Point p, Element element)
        {
            return IsAt(p.X, p.Y, element);
        }


        public bool IsAt(Point point, params Element[] elements)
        {
            foreach (var element in elements)
            {
                if (IsAt(point, element))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAt(Point[] points, params Element[] elements)
        {
            foreach (var point in points)
            {
                if (IsAt(point, elements))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAt(int x, int y, Element element)
        {
            return IsAt(Layers.LAYER1, x, y, element) ||
                   IsAt(Layers.LAYER2, x, y, element) ||
                   IsAt(Layers.LAYER3, x, y, element);
        }

        public Point GetMe()
        {
            return HeroPosition;
        }

        public List<Point> GetOtherHeroes()
        {
            List<Point> points = new List<Point>();
            points.AddRange(Get(Layers.LAYER2,
                   Element.ROBO_OTHER));
            points.AddRange(Get(Layers.LAYER3,
                   Element.ROBO_OTHER_FLYING));
            return points;
        }

        /**
        * @return Returns list of coordinates for all visible Exit points.
        */
        public List<Point> GetExits()
        {
            return Get(Layers.LAYER1, Element.EXIT);
        }

        /**
         * @return Returns list of coordinates for all visible Start points.
         */
        public List<Point> GetStarts()
        {
            return Get(Layers.LAYER1, Element.START);
        }

        /**
         * @return Returns list of coordinates for all visible Gold.
         */
        public List<Point> GetGold()
        {
            return Get(Layers.LAYER1, Element.GOLD);
        }

        /**
         * @return Returns list of coordinates for all Zombies (even die).
         */
        public List<Point> GetZombies()
        {
            return Get(Layers.LAYER2,
                    Element.FEMALE_ZOMBIE,
                    Element.MALE_ZOMBIE);
        }

        /**
        * @return Returns list of coordinates for all visible Boxes.
        */
        public List<Point> GetBoxes()
        {
            return Get(Layers.LAYER2,
                    Element.BOX);
        }

        /**
         * @return Returns list of coordinates for all visible Holes.
         */
        public List<Point> GetHoles()
        {
            return Get(Layers.LAYER1,
                    Element.HOLE,
                    Element.ROBO_FALLING,
                    Element.ROBO_OTHER_FALLING);
        }

        /**
        * @return Returns list of coordinates for all visible LaserMachines.
        */
        public List<Point> GetLaserMachines()
        {
            return Get(Layers.LAYER1,
                    Element.LASER_MACHINE_CHARGING_LEFT,
                    Element.LASER_MACHINE_CHARGING_RIGHT,
                    Element.LASER_MACHINE_CHARGING_UP,
                    Element.LASER_MACHINE_CHARGING_DOWN,

                    Element.LASER_MACHINE_READY_LEFT,
                    Element.LASER_MACHINE_READY_RIGHT,
                    Element.LASER_MACHINE_READY_UP,
                    Element.LASER_MACHINE_READY_DOWN);
        }

        /**
         * @return Returns list of coordinates for all visible Lasers.
         */
        public List<Point> GetLasers()
        {
            return Get(Layers.LAYER2,
                    Element.LASER_LEFT,
                    Element.LASER_RIGHT,
                    Element.LASER_UP,
                    Element.LASER_DOWN);
        }

        /**
         * @return Returns list of coordinates for all perks.
         */
        public List<Point> GetPerks()
        {
            return Get(Layers.LAYER1,
                    Element.UNSTOPPABLE_LASER_PERK,
                    Element.DEATH_RAY_PERK,
                    Element.UNLIMITED_FIRE_PERK);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string[] layer1 = boardAsString(Layers.LAYER1).Split('\n');
            string[] layer2 = boardAsString(Layers.LAYER2).Split('\n');
            string[] layer4 = boardAsString(Layers.LAYER4).Split('\n');

            for (int i = 0; i < layer1.Length; ++i)
            {
                int ii = Size - 1 - i;
                string index = (ii < 10 ? " " : "") + ii;
                builder.Append(index + layer1[i]
                        + " " + index + MaskOverlay(layer2[i], layer1[i])
                        + " " + index + MaskOverlay(layer4[i], layer1[i]));

                /*switch (i)
                {
                    case 0:
                        builder.Append(" Robots: " + GetMe() + "," + ListToString(GetOtherHeroes()));
                        break;
                    case 1:
                        builder.Append(" Gold: " + ListToString(GetGold()));
                        break;
                    case 2:
                        builder.Append(" Starts: " + ListToString(GetStarts()));
                        break;
                    case 3:
                        builder.Append(" Exits: " + ListToString(GetExits()));
                        break;
                    case 4:
                        builder.Append(" Boxes: " + ListToString(GetBoxes()));
                        break;
                    case 5:
                        builder.Append(" Holes: " + ListToString(GetHoles()));
                        break;
                    case 6:
                        builder.Append(" LaserMachine: " + ListToString(GetLaserMachines()));
                        break;
                    case 7:
                        builder.Append(" Lasers: " + ListToString(GetLasers()));
                        break;
                    case 8:
                        builder.Append(" Zombies: " + ListToString(GetZombies()));
                        break;
                    case 9:
                        builder.Append(" Perks: " + ListToString(GetPerks()));
                        break;
                }*/

                if (i != layer1.Length - 1)
                {
                    builder.Append("\n");
                }
            }

            return "\n" + builder.ToString() + "\n  ";
        }


        public string MaskOverlay(string source, string mask)
        {
            StringBuilder result = new StringBuilder(source);
            for (int i = 0; i < result.Length; ++i)
            {
                Element el = (Element)mask[i];
                if (IsWall(el))
                {
                    result[i] = (char)el;
                }
            }

            return result.ToString();
        }

        private string ListToString(List<Point> list)
        {
            return string.Join(",", list.ToArray());
        }

        public static bool IsWall(Element el)
        {
            Element[] map = new Element[] {
                    Element.ANGLE_IN_LEFT,
                    Element.WALL_FRONT,
                    Element.ANGLE_IN_RIGHT,
                    Element.WALL_RIGHT,
                    Element.ANGLE_BACK_RIGHT,
                    Element.WALL_BACK,
                    Element.ANGLE_BACK_LEFT,
                    Element.WALL_LEFT,
                    Element.WALL_BACK_ANGLE_LEFT,
                    Element.WALL_BACK_ANGLE_RIGHT,
                    Element.ANGLE_OUT_RIGHT,
                    Element.ANGLE_OUT_LEFT,
                    Element.SPACE };

            return map.Contains(el);
        }
    }
}
