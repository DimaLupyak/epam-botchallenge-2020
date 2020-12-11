using System.Collections.Generic;
using System.Linq;
using ICanCode.Api;
using ICanCode.Client.Models;

namespace ICanCode.Client
{
    public static class BoardExtensions
    {
        public static AimPath GetPath(this Board board, Point pointFrom, Point pointTo, Board prevBoard)
        {
            var path = FindWay.GetPath(board, pointFrom, pointTo, board.GetStepBarriers(pointFrom, prevBoard),
                board.GetJumpBarriers(pointFrom, prevBoard));
            return path;
        }

        public static AimPath GetPathFromHero(this Board board,  Point pointTo, Board prevBoard)
        {
            var path =  FindWay.GetPath(board, board.GetMe(), pointTo, board.GetStepBarriers(prevBoard).Where(x=> x != pointTo).ToList(), board.GetJumpBarriers(prevBoard));
            return path;
        }

        public static bool ShotIsDanger(this Board board, Point point, Board prevBoard)
        {
            var isNearOtherHero = board.GetOtherHeroes()
                .Any(hero => point.GetLengthTo(hero) == 1);

            var isNearOtherHeroNotAfk = board.GetOtherHeroes()
                .Where(hero => point.GetLengthTo(hero) == 1).Any(hero => !prevBoard.GetOtherHeroes().Contains(hero));

            var isMoreThenOneZombie = board.GetZombies()
                .Count(hero => point.GetLengthTo(hero) == 1) > 1;

            var isZombieMoveToMe = board.GetZombies().Any(hero => point.GetLengthTo(hero) == 1) &&
                                   prevBoard.GetZombies().Any(hero => point.GetLengthTo(hero) == 1);

            if (Static.Farm == 1)
            {
                isNearOtherHero = false;
            }

            return board.IsAt(point.ShiftLeft(), Element.LASER_MACHINE_READY_RIGHT, Element.LASER_RIGHT) ||
                   board.IsAt(point.ShiftRight(), Element.LASER_MACHINE_READY_LEFT, Element.LASER_LEFT) ||
                   board.IsAt(point.ShiftTop(), Element.LASER_MACHINE_READY_DOWN, Element.LASER_DOWN) ||
                   board.IsAt(point.ShiftBottom(), Element.LASER_MACHINE_READY_UP, Element.LASER_UP) ||
                   board.IsAt(point, Element.ZOMBIE_START, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   (isNearOtherHero/*&& Static.IsExitOpen && Static.TurnsWithoutFire < Constants.Recharge*/) || isMoreThenOneZombie || isZombieMoveToMe;
        }
        
        public static bool IsInDanger(this Board board, Point point, Board prevBoard)
        {
            return board.IsAt(point.ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.ShotIsDanger(point, prevBoard);
        }

        public static bool IsInDangerNextTurn(this Board board, Point point, Board prevBoard)
        {
            if (!board.IsLaserBarrierAt(point.ShiftLeft()))
            {
                if (board.IsAt(point.ShiftLeft().ShiftLeft(), Element.LASER_MACHINE_READY_RIGHT, Element.LASER_RIGHT))
                {
                    return true;
                }
                if (board.IsAt(point.ShiftLeft().ShiftLeft(), Element.ROBO_OTHER) && !prevBoard.IsAt(point.ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
                {
                    return true;
                }
            }

            if (!board.IsLaserBarrierAt(point.ShiftRight()))
            {
                if (board.IsAt(point.ShiftRight().ShiftRight(), Element.LASER_MACHINE_READY_LEFT, Element.LASER_LEFT))
                {
                    return true;
                }
                if (board.IsAt(point.ShiftRight().ShiftRight(), Element.ROBO_OTHER) && !prevBoard.IsAt(point.ShiftRight().ShiftRight(), Element.ROBO_OTHER))
                {
                    return true;
                }
            }

            if (!board.IsLaserBarrierAt(point.ShiftTop()))
            {
                if (board.IsAt(point.ShiftTop().ShiftTop(), Element.LASER_MACHINE_READY_DOWN, Element.LASER_DOWN))
                {
                    return true;
                }
                if (board.IsAt(point.ShiftTop().ShiftTop(), Element.ROBO_OTHER) && !prevBoard.IsAt(point.ShiftTop().ShiftTop(), Element.ROBO_OTHER))
                {
                    return true;
                }
            }

            if (!board.IsLaserBarrierAt(point.ShiftBottom()))
            {
                if (board.IsAt(point.ShiftBottom().ShiftBottom(), Element.LASER_MACHINE_READY_UP, Element.LASER_UP))
                {
                    return true;
                }
                if (board.IsAt(point.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER) && !prevBoard.IsAt(point.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
                {
                    return true;
                }
            }

            if (board.IsAt(point.ShiftLeft(), Element.LASER_MACHINE_CHARGING_RIGHT) &&
                !prevBoard.IsAt(point.ShiftLeft(), Element.LASER_MACHINE_READY_RIGHT))
            {
                return true;
            }
            if (board.IsAt(point.ShiftRight(), Element.LASER_MACHINE_CHARGING_LEFT) &&
                !prevBoard.IsAt(point.ShiftRight(), Element.LASER_MACHINE_READY_LEFT))
            {
                return true;
            }
            if (board.IsAt(point.ShiftBottom(), Element.LASER_MACHINE_CHARGING_UP) &&
                !prevBoard.IsAt(point.ShiftBottom(), Element.LASER_MACHINE_READY_UP))
            {
                return true;
            }
            if (board.IsAt(point.ShiftTop(), Element.LASER_MACHINE_CHARGING_DOWN) &&
                !prevBoard.IsAt(point.ShiftTop(), Element.LASER_MACHINE_READY_DOWN))
            {
                return true;
            }

            return board.IsAt(point.ShiftLeft().ShiftLeft(),  Element.ROBO_OTHER) ||
                   board.IsAt(point.ShiftRight().ShiftRight(), Element.ROBO_OTHER) ||
                   board.IsAt(point.ShiftTop().ShiftTop(),  Element.ROBO_OTHER) ||
                   board.IsAt(point.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER) ||
                   board.IsAt(point.ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point.ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   board.IsAt(point, Element.ZOMBIE_START, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE);
        }

        public static List<Point> GetStepBarriers(this Board board, Point point, Board prevBoard)
        {
            var stepBarriers = GetCommonBarriers(board, point);

            if (board.IsInDanger(point.ShiftRight(), prevBoard) || board.IsAt(point.ShiftRight(), Element.LASER_LEFT, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE))
            {
                stepBarriers.Add(point.ShiftRight());
            }
            if (board.IsInDanger(point.ShiftLeft(), prevBoard) || board.IsAt(point.ShiftLeft(), Element.LASER_RIGHT, Element.LASER_MACHINE_READY_RIGHT, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE))
            {
                stepBarriers.Add(point.ShiftLeft());
            }
            if (board.IsInDanger(point.ShiftTop(), prevBoard) || board.IsAt(point.ShiftTop(), Element.LASER_DOWN, Element.LASER_MACHINE_READY_DOWN, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE))
            {
                stepBarriers.Add(point.ShiftTop());
            }
            if (board.IsInDanger(point.ShiftBottom(), prevBoard) || board.IsAt(point.ShiftBottom(), Element.LASER_UP, Element.LASER_MACHINE_READY_UP, Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE))
            {
                stepBarriers.Add(point.ShiftBottom());
            }
            return stepBarriers;
        }

        public static List<Point> GetStepBarriers(this Board board, Board prevBoard)
        {
            return board.GetStepBarriers(board.GetMe(), prevBoard);
        }

        public static List<Point> GetJumpBarriers(this Board board, Point point, Board prevBoard)
        {
            var jumpBarriers = GetCommonBarriers(board, point);

            if (board.IsInDangerNextTurn(point.ShiftRight().ShiftRight(), prevBoard) || board.IsWallAt(point.ShiftRight()))
            {
                jumpBarriers.Add(point.ShiftRight().ShiftRight());
            }
            if (board.IsInDangerNextTurn(point.ShiftLeft().ShiftLeft(), prevBoard) || board.IsWallAt(point.ShiftLeft()))
            {
                jumpBarriers.Add(point.ShiftLeft().ShiftLeft());
            }
            if (board.IsInDangerNextTurn(point.ShiftTop().ShiftTop(), prevBoard) || board.IsWallAt(point.ShiftTop()))
            {
                jumpBarriers.Add(point.ShiftTop().ShiftTop());
            }
            if (board.IsInDangerNextTurn(point.ShiftBottom().ShiftBottom(), prevBoard) || board.IsWallAt(point.ShiftBottom()))
            {
                jumpBarriers.Add(point.ShiftBottom().ShiftBottom());
            }
            return jumpBarriers;
        }

        public static List<Point> GetJumpBarriers(this Board board, Board prevBoard)
        {
            return board.GetJumpBarriers(board.GetMe(), prevBoard);
        }

        private static List<Point> GetCommonBarriers(Board board, Point me)
        {
            var stepBarriers = board.Get(Element.HOLE, Element.BOX, Element.ZOMBIE_START);
            stepBarriers.AddRange(board.GetLaserMachines());
            if (!Static.CanExit)
            {
                stepBarriers.AddRange(board.Get(Element.EXIT));
            }

            return stepBarriers;
        }
    }
}
