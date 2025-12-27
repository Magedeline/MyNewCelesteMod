namespace DesoloZantas.Core.Core.Player
{
    public enum IngesteStates
    {
        StNormal = 0,
        StClimb = 1,
        StDash = 2,
        StSwim = 3,
        StBoost = 4,
        StRedDash = 5,
        StHitSquash = 6,
        StLaunch = 7,
        StPickup = 8,       
        StDreamDash = 9,
        StSummitLaunch = 10,
        StDummy = 11,
        StIntroWalk = 12,
        StIntroJump = 13,
        StIntroRespawn = 14,
        StIntroWakeUp = 15,
        StBirdDashTutorial = 16,
        StFrozen = 17,
        StReflectionFall = 18,
        StStarFly = 19,
        StTempleFall = 20,
        StCassetteFly = 21,
        StAttract = 22,
        StIntroMoonJump = 23,
        StFlingBird = 24,
        StIntroThinkForABit = 25,
        StSwing = 26,
        StHover = 27,
        StInhale = 28,
        StSpit = 29,
        StDashGroundPound = 30,
        StElectricShock = 31,
        StWarperDash = 32, // New warper dash state
        StKirbyKnight = 33, // New Kirby Knight state
        MaxStates = 34
    }
    
    public class PlayerKirbyStates
    {
        private Monocle.StateMachine stateMachine;

        public PlayerKirbyStates(Monocle.StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        
        // Helper method to check if player is in Kirby state
        public static bool IsKirbyState(int state)
        {
            return state >= (int)IngesteStates.StHover && state < (int)IngesteStates.MaxStates;
        }
    }
}




