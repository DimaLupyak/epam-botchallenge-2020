namespace ICanCode.Api
{
    public static class Static
    {
        public static bool CanExit { get; set; } = true;
        public static bool IsExitOpen { get; set; } = true;
        public static int TurnsWithoutAim { get; set; } = 0;
        public static int TurnsWithoutFire { get; set; } = 0;
        public static int PerkCooldownDeathRay { get; set; } = 0;
        public static int PerkCooldownUnlimitedFire { get; set; } = 0;
        public static int PerkCooldownUnstopableLaser { get; set; } = 0;
        public static int Farm { get; set; } = 0;
    }
}