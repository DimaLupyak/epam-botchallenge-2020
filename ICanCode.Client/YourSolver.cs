using ICanCode.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICanCode.Client.Models;

namespace ICanCode.Client
{
    /// <summary>
    /// This is ICanCode client demo.
    /// </summary>
    public partial class YourSolver : AbstractSolver
    {
        private List<Point> prevBestPath;
        
        public YourSolver(string server)
            : base(server)
        {
        }
        /// <summary>
        /// Calls each move to make decision what to do (next move)
        /// </summary>
        public override Command WhatToDo(Board board)
        {
            var me = board.GetMe();

            if (Static.TurnsWithoutAim == Constants.MaxTurnCountWithoutAim)
            {
                Static.TurnsWithoutAim = 0;
                return Command.Reset();
            }

            if (board.GetExits().Contains(me))
            {
                return Command.DoNothing();
            }

            if (!board.IsMeAlive())
            {
                HandleDie();
                return Command.DoNothing();
            }

            if (Static.PerkCooldownDeathRay > 0)
                Static.PerkCooldownDeathRay--;
            if (Static.PerkCooldownUnlimitedFire > 0)
                Static.PerkCooldownUnlimitedFire--;
            if (Static.PerkCooldownUnstopableLaser > 0)
                Static.PerkCooldownUnstopableLaser--;

            foreach (var point in board.GetStepBarriers(PrevBoard))
            {
                board.AddOnLayer4(point.X, point.Y, 's');
            }
            foreach (var point in board.GetJumpBarriers(PrevBoard))
            {
                board.AddOnLayer5(point.X, point.Y, 'j');
            }

            if (board.IsAt(me, Element.ROBO_FLYING))
            {
                if (prevBestPath != null)
                {
                    foreach (var pathPoint in prevBestPath)
                    {
                        board.AddOnLayer4(pathPoint.X, pathPoint.Y, 't');
                    }
                    board.AddOnLayer4(prevBestPath.Last().X, prevBestPath.Last().Y, 'a');
                }
                return Command.DoNothing();
            }

            if (CanFire(board, me))
            {
                if (Static.PerkCooldownDeathRay > 0)
                {
                    if (board.GetOtherHeroes().Any(hero => hero.Y == me.Y && hero.X > me.X && hero.GetLengthTo(me) < 10) ||
                        board.GetZombies().Any(hero => hero.Y == me.Y && hero.X > me.X && hero.GetLengthTo(me) < 10))
                    {
                        
                        board.AddOnLayer2(me.X, me.Y, '~');
                        return Command.Fire(Direction.Right);
                    }
                    if (board.GetOtherHeroes().Any(hero => hero.Y == me.Y && hero.X < me.X && hero.GetLengthTo(me) < 10) ||
                        board.GetZombies().Any(hero => hero.Y == me.Y && hero.X < me.X && hero.GetLengthTo(me) < 10))
                    {
                        board.AddOnLayer2(me.X, me.Y, '~');
                        return Command.Fire(Direction.Left);
                    }
                    if (board.GetOtherHeroes().Any(hero => hero.X == me.X && hero.Y < me.Y && hero.GetLengthTo(me) < 10) ||
                        board.GetZombies().Any(hero => hero.X == me.X && hero.Y < me.Y && hero.GetLengthTo(me) < 10))
                    {
                        board.AddOnLayer2(me.X, me.Y, '~');
                        return Command.Fire(Direction.Up);
                    }
                    if (board.GetOtherHeroes().Any(hero => hero.X == me.X && hero.Y > me.Y && hero.GetLengthTo(me) < 10) ||
                        board.GetZombies().Any(hero => hero.X == me.X && hero.Y > me.Y && hero.GetLengthTo(me) < 10))
                    {
                        board.AddOnLayer2(me.X, me.Y, '~');
                        return Command.Fire(Direction.Down);
                    }
                }
                if (ShouldFireToLeft())
                {
                    board.AddOnLayer2(me.X, me.Y, '~');
                    return Command.Fire(Direction.Left);
                }
                if (ShouldFireToRight())
                {
                    board.AddOnLayer2(me.X, me.Y, '~');
                    return Command.Fire(Direction.Right);
                }
                if (ShouldFireToTop())
                {
                    board.AddOnLayer2(me.X, me.Y, '~');
                    return Command.Fire(Direction.Up);
                }
                if (ShouldFireToBottom())
                {
                    board.AddOnLayer2(me.X, me.Y, '~');
                    return Command.Fire(Direction.Down);
                }
            }

            Static.CanExit = false;
            List<Point> bestPath = GetTheNearestToHero(GetCornersAims());
            if (board.CurrentLevel < 22 && GetTheNearestToHero(GetExitAims()) != null)
            {
                bestPath = null;
            }

            if (GetTheNearestToHero(GetGoldAims()) != null || goldCollected > 0)
            {
                Static.Farm = 0;
            }
            else if(board.GetOtherHeroes().Any())
            {
                Static.Farm = 1;
            }
            if (Static.Farm == 1)
            {
                bestPath = GetTheNearestToHero(GetFarmAims());
            }
            if (bestPath == null)
            {
                var stuffAims = new List<Point>();
                stuffAims.AddRange(GetGoldAims());
                stuffAims.AddRange(GetPerkAims());
                bestPath = GetTheNearestToHero(stuffAims);
            }
            if (bestPath == null)
            {
                Static.CanExit = true;
                bestPath = GetTheNearestToHero(GetExitAims());
                if (bestPath != null)
                {
                    Static.IsExitOpen = true;
                    boxesMoved = 0;
                }
            }
            if (bestPath == null)
            {
                Static.IsExitOpen = false;
            }
            
            if (bestPath == null && boxesMoved <= 5)
            {
                var boxToPull = GetTheNearestToExit(GetBoxAims());

                if (boxToPull != null)
                {
                    bestPath = boxToPull.Value.Item1;
                    if (bestPath.Count == 1)
                    {
                        boxesMoved++;
                        return Command.Pull(boxToPull.Value.Item2);
                    }
                }
            }
            if (bestPath == null)
            {
                var heroAims = new List<Point>();
                heroAims.AddRange(GetAfkAims());
                heroAims.AddRange(GetHeroesAims());
                heroAims.AddRange(GetZombieAims());
                bestPath = GetTheNearestToHero(heroAims);
            }

            if (bestPath == null)
            {
                Static.TurnsWithoutAim++;
                return Command.DoNothing();
            }
            else
            {
                Static.TurnsWithoutAim = 0;
            }
            bestPath = bestPath.Select(x => x.AbsoluteToRelative(board.Size, board.Offset)).ToList();
            prevBestPath = bestPath;
            foreach (var pathPoint in bestPath)
            {
                board.AddOnLayer4(pathPoint.X, pathPoint.Y, 't');
                var absolute = pathPoint.RelativeToAbsolute(board.Size, board.Offset);
                FullBoard.AddOnLayer4(absolute.X, absolute.Y, 't');
            }
            board.AddOnLayer4(bestPath.Last().X, bestPath.Last().Y, 'a');
            var a = bestPath.Last().RelativeToAbsolute(board.Size, board.Offset);
            FullBoard.AddOnLayer4(a.X, a.Y, 'a');
            

            var nextPosition = bestPath[1];
            

            if (board.IsAt(nextPosition, Element.GOLD))
            {
                goldCollected++;
            }
            if (board.IsAt(nextPosition, Element.EXIT))
            {
                goldCollected = 0;
            }
            if (board.IsAt(nextPosition, Element.DEATH_RAY_PERK))
            {
                Static.PerkCooldownDeathRay = 10;
            }
            if (board.IsAt(nextPosition, Element.UNLIMITED_FIRE_PERK))
            {
                Static.PerkCooldownUnlimitedFire = 10;
            }
            if (board.IsAt(nextPosition, Element.UNSTOPPABLE_LASER_PERK))
            {
                Static.PerkCooldownUnstopableLaser = 10;
            }
            if (me.GetLengthTo(nextPosition) == 1)
            {
                return Command.Go(GetDirection(me, nextPosition));
            }
            return Command.Jump(GetDirection(me, nextPosition));
        }



        private void HandleDie()
        {
            if (prevState != null)
            {
                if (!Directory.Exists("Dies"))
                {
                    Directory.CreateDirectory("Dies");
                }

                using (StreamWriter sw = File.CreateText($@"Dies\{Guid.NewGuid()}.txt"))
                {
                    sw.WriteLine(prevState);
                }
            }

            goldCollected = 0;
        }

        private static Direction GetDirection(Point me, Point nextPosition)
        {
            if (me.X < nextPosition.X)
                return Direction.Right;
            if (me.X > nextPosition.X)
                return Direction.Left;
            if (me.Y < nextPosition.Y)
                return Direction.Down;
            else
                return Direction.Up;
        }

        private List<Point> GetCornersAims()
        {
            var corners = new[]
            {
                new Point(0, 0),
                new Point(0, Constants.FullBoardSize - 1),
                new Point(Constants.FullBoardSize - 1, 0),
                new Point(Constants.FullBoardSize - 1, Constants.FullBoardSize - 1)
            }.ToList();
            return corners;
        }

        private List<Point> GetExitAims()
        {
            var exitAims = FullBoard.GetExits().Where(x => !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)).ToList();
            return exitAims;
        }

        private List<Point> GetGoldAims()
        {
            var notAfk = FullBoard.GetOtherHeroes().Where(x => !PrevFullBoard.GetOtherHeroes().Contains(x)).ToList();
            var aims = FullBoard.GetGold()
                .Where(x => x.GetLengthTo(FullBoard.GetMe()) < Constants.GoldRadius * 2 &&
                               !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT) &&
                               FullBoard.GetPathFromHero(x, PrevFullBoard)?.Length < Constants.GoldRadius && 
                               notAfk.All(othetHero => FullBoard.GetPath(othetHero, x, PrevFullBoard)?.Length >= FullBoard.GetPathFromHero(x, PrevFullBoard)?.Length))
                .ToList();
            return aims;
        }

        private List<Point> GetZombieAims()
        {
            var target = FullBoard.GetZombies().Where(x => x.GetLengthTo(FullBoard.GetMe()) <= 5).ToList();
            var aims = new List<Point>();
            aims.AddRange(target.Select(x => x.ShiftLeft().ShiftLeft()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight().ShiftRight()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftLeft().ShiftLeft()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight().ShiftRight()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            return aims;
        }

        private List<Point> GetPerkAims()
        {
            var aims = FullBoard.GetPerks()
                .Where(x => !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT) &&
                            FullBoard.GetPathFromHero(x, PrevFullBoard)?.Length < 4 )
                .ToList();
            return aims;
        }

        private List<Point> GetBoxAims()
        {
            var boxesAims = FullBoard.GetBoxes();
            return boxesAims;
        }
        private List<Point> GetHeroesAims()
        {
            var target = FullBoard.GetOtherHeroes().Where(x => x.GetLengthTo(FullBoard.GetMe()) <= 10).ToList();
            //target.AddRange(board.GetStarts());
            var aims = new List<Point>();
            aims.AddRange(target.Select(x => x.ShiftLeft().ShiftTop()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight().ShiftTop()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftLeft().ShiftBottom()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight().ShiftBottom()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            return aims;
        }

        private List<Point> GetFarmAims()
        {
            var target = new List<Point>();
            target.AddRange(FullBoard.GetOtherHeroes().Where(x => FullBoard.GetStarts().Any(start => start.GetLengthTo(x) < 2)).ToList());
            target.AddRange(FullBoard.GetStarts());
            var aims = new List<Point>();
            aims.AddRange(target.Select(x => x.ShiftLeft()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftTop()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftBottom()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            return aims;
        }

        private List<Point> GetAfkAims()
        {
            var target = FullBoard.GetOtherHeroes().Where(x => x.GetLengthTo(FullBoard.GetMe()) <= 7 && PrevFullBoard.GetOtherHeroes().Contains(x)).ToList();
            var aims = new List<Point>();
            aims.AddRange(target.Select(x => x.ShiftLeft()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftRight()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftBottom()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));
            aims.AddRange(target.Select(x => x.ShiftTop()).Where(x => !FullBoard.IsHeroBarrierAt(x) && !FullBoard.IsInDanger(x, PrevFullBoard) && !FullBoard.IsAt(x, Element.LASER_DOWN, Element.LASER_LEFT, Element.LASER_UP, Element.LASER_RIGHT)));

            return aims;
        }

        private bool CanFire(Board board, Point me)
        {
            if (Static.PerkCooldownUnlimitedFire == 0)
            {
                return !board.ShotIsDanger(me, PrevBoard) && Static.TurnsWithoutFire >= Constants.Recharge;
            }
            else
            {
                return !board.ShotIsDanger(me, PrevBoard);
            }
        }



        private List<Point> GetTheNearestToHero(IEnumerable<Point> aims)
        {
            AimPath bestPath = null;
            foreach (var aim in aims)
            {
                var path = FullBoard.GetPathFromHero(aim, PrevFullBoard);
                if (path != null && path.Path.Count > 1)
                {
                    if (bestPath == null || path.Length < bestPath.Length)
                    {
                        bestPath = path;
                    }
                }
            }
            return bestPath?.Path;
        }

        private (List<Point>, Direction)? GetTheNearestToExit(IEnumerable<Point> aims)
        {
            var aimPathes = new Dictionary<Point, AimPath>();
            foreach (var point in aims)
            {
                var path = FullBoard.GetPathFromHero(point, PrevFullBoard);
                if (path != null && path.Path.Count > 1)
                {
                    aimPathes[point] = path;
                }
            }
            foreach (var item in aimPathes.OrderBy(x => GetLenghtToExit(FullBoard, x.Key) * 2 + x.Value.Length))
            {
                var aim = item.Key;
                var path = item.Value;

                if (!FullBoard.IsHeroBarrierAt(aim.ShiftLeft()) && !FullBoard.IsAt(aim.ShiftLeft(), Element.START) && !FullBoard.IsInDanger(aim.ShiftLeft(), PrevFullBoard) &&
                    GetLenghtToExit(FullBoard, aim.ShiftLeft()) > GetLenghtToExit(FullBoard, aim) &&
                    !FullBoard.IsHeroBarrierAt(aim.ShiftLeft().ShiftLeft()) && 
                    FullBoard.GetPathFromHero(aim.ShiftLeft().ShiftLeft(), PrevFullBoard) != null)
                {
                    path = FullBoard.GetPathFromHero(aim.ShiftLeft(), PrevFullBoard);
                    if (path != null)
                    {
                        return (path.Path, Direction.Left);
                    }
                }

                if (!FullBoard.IsHeroBarrierAt(aim.ShiftRight()) && !FullBoard.IsAt(aim.ShiftRight(), Element.START) && !FullBoard.IsInDanger(aim.ShiftRight(), PrevFullBoard) &&
                    GetLenghtToExit(FullBoard, aim.ShiftRight()) > GetLenghtToExit(FullBoard, aim) &&
                    !FullBoard.IsHeroBarrierAt(aim.ShiftRight().ShiftRight()) && 
                    FullBoard.GetPathFromHero(aim.ShiftRight().ShiftRight(), PrevFullBoard) != null)
                {
                    path = FullBoard.GetPathFromHero(aim.ShiftRight(), PrevFullBoard);
                    if (path != null)
                    {
                        return (path.Path, Direction.Right);
                    }
                }

                if (!FullBoard.IsHeroBarrierAt(aim.ShiftTop()) && !FullBoard.IsAt(aim.ShiftTop(), Element.START) && !FullBoard.IsInDanger(aim.ShiftTop(), PrevFullBoard) &&
                    GetLenghtToExit(FullBoard, aim.ShiftTop()) > GetLenghtToExit(FullBoard, aim) &&
                    !FullBoard.IsHeroBarrierAt(aim.ShiftTop().ShiftTop()) && 
                    FullBoard.GetPathFromHero(aim.ShiftTop().ShiftTop(), PrevFullBoard) != null)
                {
                    path = FullBoard.GetPathFromHero(aim.ShiftTop(), PrevFullBoard);
                    if (path != null)
                    {
                        return (path.Path, Direction.Up);
                    }
                }

                if (!FullBoard.IsHeroBarrierAt(aim.ShiftBottom()) && !FullBoard.IsAt(aim.ShiftBottom(), Element.START) && !FullBoard.IsInDanger(aim.ShiftBottom(), PrevFullBoard) &&
                    GetLenghtToExit(FullBoard, aim.ShiftBottom()) > GetLenghtToExit(FullBoard, aim) &&
                    !FullBoard.IsHeroBarrierAt(aim.ShiftBottom().ShiftBottom()) && 
                    FullBoard.GetPathFromHero(aim.ShiftBottom().ShiftBottom(), PrevFullBoard) != null)
                {
                    path = FullBoard.GetPathFromHero(aim.ShiftBottom(), PrevFullBoard);
                    if (path != null)
                    {
                        return (path.Path, Direction.Down);
                    }
                }

            }
            return null;
        }

        private static int GetLenghtToExit(Board board, Point aim)
        {
            var minLenghtToExit = 999;
            foreach (var exit in board.GetExits())
            {
                var lenghtToExit = aim.GetLengthTo(exit);
                if (lenghtToExit < minLenghtToExit)
                {
                    minLenghtToExit = lenghtToExit;
                }
            }
            return minLenghtToExit;
        }
    }
}
