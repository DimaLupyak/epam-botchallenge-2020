using ICanCode.Api;

namespace ICanCode.Client
{
    public partial class YourSolver
    {
        private bool ShouldFireToBottom()
        {
            var me = Board.HeroPosition;

            if (Board.IsLaserBarrierAt(me.ShiftBottom()) && Static.PerkCooldownUnstopableLaser == 0)
            {
                return false;
            }

            //Farm
            if (Static.Farm == 1 && Board.IsAt(me.ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 1
            if (Board.IsAt(me.ShiftBottom().ShiftRight(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftBottom().ShiftRight().ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 2
            if (Board.IsAt(me.ShiftBottom().ShiftLeft(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftBottom().ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump to me
            if (Board.IsAt(me.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftBottom().ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump or go over me
            if (Board.IsAt(me, Element.ROBO_OTHER, Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftTop(), Element.ROBO_OTHER, Element.ROBO_OTHER))
            {
                return true;
            }
            
            //Afk
            if (Board.IsAt(me.ShiftBottom(), Element.ROBO_OTHER) && 
                goldCollected < 5 &&
                PrevBoard.IsAt(me.ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsAt(me.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER) && 
                PrevBoard.IsAt(me.ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsLaserBarrierAt(me.ShiftBottom().ShiftBottom()) &&
                Board.IsAt(me.ShiftBottom().ShiftBottom().ShiftBottom(), Element.ROBO_OTHER) && 
                PrevBoard.IsAt(me.ShiftBottom().ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Zombie near
            return Board.IsAt(me.ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftBottom().ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftBottom().ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftBottom().ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE);
        }

        private bool ShouldFireToTop()
        {
            var me = Board.HeroPosition;

            if (Board.IsLaserBarrierAt(me.ShiftTop()) && Static.PerkCooldownUnstopableLaser == 0)
            {
                return false;
            }

            //Farm
            if (Static.Farm == 1 && Board.IsAt(me.ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 1
            if (Board.IsAt(me.ShiftTop().ShiftRight(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftTop().ShiftRight().ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 2
            if (Board.IsAt(me.ShiftTop().ShiftLeft(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftTop().ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump to me
            if (Board.IsAt(me.ShiftTop().ShiftTop(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftTop().ShiftTop().ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump or go over me
            if (Board.IsAt(me, Element.ROBO_OTHER, Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftBottom(), Element.ROBO_OTHER, Element.ROBO_OTHER))
            {
                return true;
            }

            //Afk
            if (Board.IsAt(me.ShiftTop(), Element.ROBO_OTHER) &&
                goldCollected < 5 &&
                PrevBoard.IsAt(me.ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsAt(me.ShiftTop().ShiftTop(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftTop().ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsLaserBarrierAt(me.ShiftTop().ShiftTop()) &&
                Board.IsAt(me.ShiftTop().ShiftTop().ShiftTop(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftTop().ShiftTop().ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Zombie near
            return Board.IsAt(me.ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftTop().ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftTop().ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftTop().ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE);
        }

        private bool ShouldFireToRight()
        {
            var me = Board.HeroPosition;

            if (Board.IsLaserBarrierAt(me.ShiftRight()) && Static.PerkCooldownUnstopableLaser == 0)
            {
                return false;
            }

            //Farm
            if (Static.Farm == 1 && Board.IsAt(me.ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 1
            if (Board.IsAt(me.ShiftRight().ShiftTop(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftRight().ShiftTop().ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 2
            if (Board.IsAt(me.ShiftRight().ShiftBottom(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftRight().ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump to me
            if (Board.IsAt(me.ShiftRight().ShiftRight(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftRight().ShiftRight().ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump or go over me
            if (Board.IsAt(me, Element.ROBO_OTHER, Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftLeft(), Element.ROBO_OTHER, Element.ROBO_OTHER))
            {
                return true;
            }

            //Afk
            if (Board.IsAt(me.ShiftRight(), Element.ROBO_OTHER) &&
                goldCollected < 5 &&
                PrevBoard.IsAt(me.ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsAt(me.ShiftRight().ShiftRight(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftRight().ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsLaserBarrierAt(me.ShiftRight().ShiftRight()) &&
                Board.IsAt(me.ShiftRight().ShiftRight().ShiftRight(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftRight().ShiftRight().ShiftRight(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Zombie near
            return Board.IsAt(me.ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftRight().ShiftRight(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftRight().ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftRight().ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE);
        }


        private bool ShouldFireToLeft()
        {
            var me = Board.HeroPosition;

            if (Board.IsLaserBarrierAt(me.ShiftLeft()) && Static.PerkCooldownUnstopableLaser == 0)
            {
                return false;
            }

            //Farm
            if (Static.Farm == 1 && Board.IsAt(me.ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 1
            if (Board.IsAt(me.ShiftLeft().ShiftTop(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftLeft().ShiftTop().ShiftTop(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump from side 2
            if (Board.IsAt(me.ShiftLeft().ShiftBottom(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftLeft().ShiftBottom().ShiftBottom(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump to me
            if (Board.IsAt(me.ShiftLeft().ShiftLeft(), Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftLeft().ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Other Hero Jump or go over me
            if (Board.IsAt(me, Element.ROBO_OTHER, Element.ROBO_OTHER_FLYING) &&
                PrevBoard.IsAt(me.ShiftRight(), Element.ROBO_OTHER, Element.ROBO_OTHER))
            {
                return true;
            }

            //Afk
            if (Board.IsAt(me.ShiftLeft(), Element.ROBO_OTHER) &&
                goldCollected < 5 &&
                PrevBoard.IsAt(me.ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsAt(me.ShiftLeft().ShiftLeft(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }
            if (Board.IsLaserBarrierAt(me.ShiftLeft().ShiftLeft()) &&
                Board.IsAt(me.ShiftLeft().ShiftLeft().ShiftLeft(), Element.ROBO_OTHER) &&
                PrevBoard.IsAt(me.ShiftLeft().ShiftLeft().ShiftLeft(), Element.ROBO_OTHER))
            {
                return true;
            }

            //Zombie near
            return Board.IsAt(me.ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftLeft().ShiftLeft(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftLeft().ShiftTop(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE) ||
                   Board.IsAt(me.ShiftLeft().ShiftBottom(), Element.MALE_ZOMBIE, Element.FEMALE_ZOMBIE);
        }
    }
}