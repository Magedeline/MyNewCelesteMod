namespace DesoloZantas.Core.Core;

public class PlayerSprite : Sprite
{
    // Animation Constants grouped in a static class for organization
    public new static class Animations
    {
        public const string IDLE = "idle";
        public const string SHAKING = "shaking";
        public const string FRONT_EDGE = "edge";
        public const string LOOK_UP = "lookUp";
        public const string WALK = "walk";
        public const string RUN_SLOW = "runSlow";
        public const string RUN_FAST = "runFast";
        public const string RUN_WIND = "runWind";
        public const string RUN_STUMBLE = "runStumble";
        public const string JUMP_SLOW = "jumpSlow";
        public const string FALL_SLOW = "fallSlow";
        public const string FALL = "fall";
        public const string JUMP_FAST = "jumpFast";
        public const string FALL_FAST = "fallFast";
        public const string FALL_BIG = "bigFall";
        public const string LAND_IN_POSE = "fallPose";
        public const string TIRED = "tired";
        public const string TIRED_STILL = "tiredStill";
        public const string WALL_SLIDE = "wallslide";
        public const string CLIMB_UP = "climbUp";
        public const string CLIMB_DOWN = "climbDown";
        public const string CLIMB_LOOK_BACK_START = "climbLookBackStart";
        public const string CLIMB_LOOK_BACK = "climbLookBack";
        public const string DANGLING = "dangling";
        public const string DUCK = "duck";
        public const string DASH = "dash";
        public const string SLEEP = "sleep";
        public const string SLEEPING = "asleep";
        public const string FLIP = "flip";
        public const string SKID = "skid";
        public const string DREAM_DASH_IN = "dreamDashIn";
        public const string DREAM_DASH_LOOP = "dreamDashLoop";
        public const string DREAM_DASH_OUT = "dreamDashOut";
        public const string SWIM_IDLE = "swimIdle";
        public const string SWIM_UP = "swimUp";
        public const string SWIM_DOWN = "swimDown";
        public const string START_STAR_FLY = "startStarFly";
        public const string STAR_FLY = "starFly";
        public const string STAR_MORPH = "starMorph";
        public const string IDLE_CARRY = "idle_carry";
        public const string RUN_CARRY = "runSlow_carry";
        public const string JUMP_CARRY = "jumpSlow_carry";
        public const string FALL_CARRY = "fallSlow_carry";
        public const string PICK_UP = "pickup";
        public const string THROW = "throw";
        public const string LAUNCH = "launch";
        public const string LAUNCH_RECOVER = "launchRecover";
        public const string TENTACLE_GRAB = "tentacle/grab";
        public const string SIT_DOWN = "sitDown";
        public const string WAKE_UP = "wakeUp";
        public const string HALF_WAKE_UP = "halfWakeUp";
        public const string LEVEL_UP = "levelUp/levelUp";
        public const string SWEAT_IDLE = "sweat/idle";
        public const string SWEAT_STILL = "sweat/still";
        public const string SWEAT_JUMP = "sweat/jump";
        public const string SWEAT_CLIMB = "sweat/climb";
        public const string SWEAT_DANGER = "sweat/danger";
        public const string PUSH = "push";
        public const string SPIN = "spin";
        public const string FAINT = "faint";
        public const string HUG = "hug";
        public const string EDGE_BACK = "edge_back";
        public const string BANGS = "bangs";
        public const string DEAD_DOWN = "deaddown";
        public const string DEAD_SIDE = "deadside";
        public const string DEAD_UP = "deadup";
        public const string DEATH_H = "death_h";
        public const string IDLE_A = "idleA";
        public const string IDLE_B = "idleB";
        public const string IDLE_C = "idleC";
        public const string START_STAR_FLY_WHITE = "startStarFlyWhite";
        public const string WALK_CARRY_THEO = "walk_carry_theo";
        public const string CARRY_THEO_COLLAPSE = "carrytheo_collapse";
    }

    public static readonly Dictionary<string, PlayerAnimMetadata>
        FRAME_METADATA = new(StringComparer.OrdinalIgnoreCase);

    public int HairCount = 4;
    public static string LookUp;
    public static string Run;
    public static string Walk;
    public static string Jump;
    public static string Fall;
    public static string Dash;
    public static string Climb;
    public static string Swim;
    public static string Sleep;
    public static string Flip;
    public static string Skid;
    public static string Pickup;
    public static string Throw;
    public static string Launch;
    public static string TentacleGrab;
    public static string SitDown;
    public static string WakeUp;
    public static string HalfWakeUp;
    public static string LevelUp;
    public static string SweatIdle;
    public static string SweatStill;
    public static string SweatJump;
    public static string SweatClimb;
    public static string SweatDanger;
    public static string Push;
    public static string Spin;
    public static string Faint;
    public static string Hug;
    public static string EdgeBack;
    public static string Bangs;
    public static string DeadDown;
    public static string DeadSide;
    public static string DeadUp;
    public static string DeathH;
    public static string IdleA;
    public static string IdleB;
    public static string IdleC;
    public static string LaunchRecover;
    public static string StartStarFlyWhite;
    public static string WalkCarryTheo;
    public static string CarryTheoCollapse;
    public static string DreamDashIn;
    public static string DreamDashLoop;
    public static string DreamDashOut;
    public static string IdleCarry;
    public static string RunCarry;
    public static string JumpCarry;
    public static string FallCarry;
    public static string ClimbUp;
    public static string ClimbDown;
    public static string ClimbLookBackStart;
    public static string ClimbLookBack;
    public static string Dangling;
    public static string WallSlide;
    public static string Tired;
    public static string TiredStill;
    public static string SleepAsleep;
    public static string StartStarFly;
    public static string StarFly;
    public static string StarMorph;
    public static string Shaking;
    public static string FrontEdge;
    public static string LandInPose;
    public static string RunSlow;
    public static string RunFast;
    public static string RunWind;
    public static string RunStumble;
    public static string JumpSlow;
    public static string FallSlow;
    public static string FallFast;
    public static string FallBig;
    public static string JumpFast;
    public static string SleepSleep;
    public static string Duck;
    public static string FlipAnim;
    public static string SkidAnim;
    public static string PickupAnim;

    public PlayerSprite(PlayerSpriteMode mode)
        : base(null, null)
    {
        Mode = mode;
        var modeToIdMap = new Dictionary<PlayerSpriteMode, string>
        {
            { PlayerSpriteMode.Chara, "chara" },
            { PlayerSpriteMode.Kirby, "kirby" },
            { PlayerSpriteMode.Ralsei, "ralsei" },
            { PlayerSpriteMode.Madeline, "player" },
            { PlayerSpriteMode.Madelinealt, "madeline" },
            { PlayerSpriteMode.MadelineNoBackpack, "player_no_backpack" },
            { PlayerSpriteMode.Badeline, "badeline" },
            { PlayerSpriteMode.MadelineAsBadeline, "player_badeline" },
            { PlayerSpriteMode.Playback, "player_playback" },
            { PlayerSpriteMode.Default, "player" },
            { PlayerSpriteMode.Starsie, "starsie" },
            // New characters from the request
            { PlayerSpriteMode.Rick, "rick" },
            { PlayerSpriteMode.Kine, "kine" },
            { PlayerSpriteMode.Coo, "coo" },
            { PlayerSpriteMode.BandanaWaddleDee, "bandana_waddle_dee" },
            { PlayerSpriteMode.KingDDD, "king_ddd" },
            { PlayerSpriteMode.MetaKnight, "meta_knight" },
            { PlayerSpriteMode.Adeline, "adeline" },
            { PlayerSpriteMode.Clover, "clover" },
            { PlayerSpriteMode.Melody, "melody" },
            { PlayerSpriteMode.Batty, "batty" },
            { PlayerSpriteMode.Emily, "emily" },
            { PlayerSpriteMode.Cody, "cody" },
            { PlayerSpriteMode.Odin, "odin" },
            { PlayerSpriteMode.Charlo, "charlo" },
            { PlayerSpriteMode.Frisk, "frisk" },
            { PlayerSpriteMode.Magolor, "magolor" },
            { PlayerSpriteMode.SusieHaltmann, "susie_haltmann" },
            { PlayerSpriteMode.Ness, "ness" },
            { PlayerSpriteMode.Taranza, "taranza" },
            { PlayerSpriteMode.Gooey, "gooey" },
            { PlayerSpriteMode.Squeaker, "squeaker" },
            { PlayerSpriteMode.DarkMetaKnight, "dark_meta_knight" },
            { PlayerSpriteMode.Marx, "marx" },
            { PlayerSpriteMode.FranZalea, "fran_zalea" },  // Three mage sisters
            { PlayerSpriteMode.FlambergeZalea, "flamberge_zalea" },
            { PlayerSpriteMode.HynesZalea, "hynes_zalea" },
            { PlayerSpriteMode.Asriel, "asriel" },
            { PlayerSpriteMode.KirbyClassic, "kirby_classic" }
        };

        if (modeToIdMap.TryGetValue(mode, out string spriteName))
        {
            try
            {
                GFX.SpriteBank.CreateOn(this, spriteName);
            }
            catch (Exception)
            {
                // Fallback to player sprite if the custom sprite doesn't exist
                GFX.SpriteBank.CreateOn(this, "player");
            }
        }
        else
        {
            // Default fallback
            GFX.SpriteBank.CreateOn(this, "player");
        }
    }

    public PlayerSpriteMode Mode { get; private set; }

    public Vector2 HairOffset =>
        Texture != null && FRAME_METADATA.TryGetValue(Texture.AtlasPath, out var playerAnimMetadata)
            ? playerAnimMetadata.HairOffset
            : Vector2.Zero;

    public float CarryYOffset =>
        Texture != null && FRAME_METADATA.TryGetValue(Texture.AtlasPath, out var playerAnimMetadata)
            ? playerAnimMetadata.CarryYOffset * Scale.Y
            : 0.0f;

    public int HairFrame => Texture != null && FRAME_METADATA.TryGetValue(Texture.AtlasPath, out var playerAnimMetadata)
        ? playerAnimMetadata.Frame
        : 0;

    public bool HasHair => Texture != null &&
                           FRAME_METADATA.TryGetValue(Texture.AtlasPath, out var playerAnimMetadata) &&
                           playerAnimMetadata.HasHair;

    public bool Running
    {
        get
        {
            if (LastAnimationID == null)
                return false;
            return LastAnimationID == Animations.FLIP || LastAnimationID.StartsWith("run");
        }
    }

    public bool DreamDashing
    {
        get => LastAnimationID != null && LastAnimationID.StartsWith("dreamDash");
        set
        {
            if (value)
                Play(Animations.DREAM_DASH_IN);
            else if (LastAnimationID == Animations.DREAM_DASH_IN || LastAnimationID == Animations.DREAM_DASH_LOOP)
                Play(Animations.DREAM_DASH_OUT);
        }
    }

    public static string Idle { get; set; }

    public override void Render()
    {
        var renderPosition = RenderPosition;
        RenderPosition = RenderPosition.Floor();
        base.Render();
        RenderPosition = renderPosition;
    }

    internal void Play(object animationId)
    {
        if (animationId is string animName)
        {
            base.Play(animName);
        }
        else if (animationId != null)
        {
            base.Play(animationId.ToString());
        }
    }

    public static explicit operator PlayerSprite(global::Celeste.PlayerSprite v)
    {
        if (v == null) throw new ArgumentNullException(nameof(v));

        var mode = v.Mode switch
        {
            global::Celeste.PlayerSpriteMode.Madeline => PlayerSpriteMode.Madeline,
            global::Celeste.PlayerSpriteMode.MadelineNoBackpack => PlayerSpriteMode.MadelineNoBackpack,
            global::Celeste.PlayerSpriteMode.Badeline => PlayerSpriteMode.Badeline,
            global::Celeste.PlayerSpriteMode.MadelineAsBadeline => PlayerSpriteMode.MadelineAsBadeline,
            global::Celeste.PlayerSpriteMode.Playback => PlayerSpriteMode.Playback,
            _ => PlayerSpriteMode.Default
        };

        return new PlayerSprite(mode);
    }
}




