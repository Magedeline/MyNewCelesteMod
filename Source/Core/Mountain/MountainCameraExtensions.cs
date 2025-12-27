#nullable enable
namespace DesoloZantas.Core.Core.Mountain
{
    /// <summary>
    /// Extension methods and helpers for working with MountainCamera
    /// </summary>
    public static class MountainCameraExtensions
    {
        /// <summary>
        /// Create a camera looking at a target from a specific position
        /// </summary>
        public static MountainCamera CreateLookAt(Vector3 position, Vector3 target)
        {
            return new MountainCamera(position, target);
        }

        /// <summary>
        /// Create a camera orbiting around a point at a specific distance and angle
        /// </summary>
        public static MountainCamera CreateOrbit(Vector3 center, float distance, float angle, float height)
        {
            Vector3 position = new Vector3(
                center.X + (float)Math.Cos(angle) * distance,
                center.Y + height,
                center.Z + (float)Math.Sin(angle) * distance
            );
            return new MountainCamera(position, center);
        }

        /// <summary>
        /// Interpolate between two camera positions
        /// </summary>
        public static MountainCamera Lerp(MountainCamera from, MountainCamera to, float amount)
        {
            Vector3 position = Vector3.Lerp(from.Position, to.Position, amount);
            Vector3 target = Vector3.Lerp(from.Target, to.Target, amount);
            return new MountainCamera(position, target);
        }

        /// <summary>
        /// Get the distance between camera position and target
        /// </summary>
        public static float GetDistance(this MountainCamera camera)
        {
            return (camera.Target - camera.Position).Length();
        }

        /// <summary>
        /// Get the forward direction of the camera
        /// </summary>
        public static Vector3 GetForward(this MountainCamera camera)
        {
            return Vector3.Normalize(camera.Target - camera.Position);
        }
    }

    /// <summary>
    /// Predefined camera positions for DesoloZantas chapters
    /// </summary>
    public static class DesoloCameraPresets
    {
        // ===========================================
        // DEFAULT OVERWORLD VIEWS
        // ===========================================
        
        /// <summary>Default rotation view for overworld idle</summary>
        public static readonly MountainCamera RotationDefault = new MountainCamera(
            new Vector3(0f, 12f, 24f),
            MountainRenderer.RotateLookAt
        );

        /// <summary>Far away view for wide overworld perspective</summary>
        public static readonly MountainCamera FarView = new MountainCamera(
            new Vector3(0f, 5f, 35f),
            new Vector3(0f, 8f, 0f)
        );

        /// <summary>Close overhead view</summary>
        public static readonly MountainCamera OverheadView = new MountainCamera(
            new Vector3(0f, 25f, 5f),
            new Vector3(0f, 0f, 0f)
        );

        // ===========================================
        // PROLOGUE - HOUSE
        // ===========================================
        
        /// <summary>Prologue: House exterior - cozy distant view</summary>
        public static readonly MountainCamera Prologue_House = new MountainCamera(
            new Vector3(-5f, 6f, 16f),
            new Vector3(-2f, 3f, 0f)
        );

        /// <summary>Prologue: House interior close-up</summary>
        public static readonly MountainCamera Prologue_HouseInterior = new MountainCamera(
            new Vector3(-3f, 4f, 8f),
            new Vector3(-1f, 2f, 0f)
        );

        /// <summary>Prologue: Waking up scene - low angle</summary>
        public static readonly MountainCamera Prologue_Awakening = new MountainCamera(
            new Vector3(-2f, 3f, 12f),
            new Vector3(0f, 4f, 0f)
        );

        // ===========================================
        // CHAPTER 1 - CITY
        // ===========================================
        
        /// <summary>Chapter 1: City skyline overview</summary>
        public static readonly MountainCamera Chapter1_City = new MountainCamera(
            new Vector3(5f, 10f, 15f),
            new Vector3(2f, 5f, 0f)
        );

        /// <summary>Chapter 1: City streets level view</summary>
        public static readonly MountainCamera Chapter1_CityStreets = new MountainCamera(
            new Vector3(8f, 4f, 18f),
            new Vector3(3f, 3f, 0f)
        );

        /// <summary>Chapter 1: City rooftops - high angle</summary>
        public static readonly MountainCamera Chapter1_CityRooftops = new MountainCamera(
            new Vector3(4f, 14f, 12f),
            new Vector3(2f, 6f, 0f)
        );

        /// <summary>Chapter 1: City at night - dramatic angle</summary>
        public static readonly MountainCamera Chapter1_CityNight = new MountainCamera(
            new Vector3(6f, 8f, 20f),
            new Vector3(1f, 4f, -2f)
        );

        // ===========================================
        // CHAPTER 9 - SUMMIT
        // ===========================================
        
        /// <summary>Chapter 9: Summit peak - looking up</summary>
        public static readonly MountainCamera Chapter9_Summit = new MountainCamera(
            new Vector3(0f, 20f, 10f),
            new Vector3(0f, 15f, 0f)
        );

        /// <summary>Chapter 9: Summit base - looking up at mountain</summary>
        public static readonly MountainCamera Chapter9_SummitBase = new MountainCamera(
            new Vector3(2f, 5f, 22f),
            new Vector3(0f, 18f, 0f)
        );

        /// <summary>Chapter 9: Summit climb - side view</summary>
        public static readonly MountainCamera Chapter9_SummitClimb = new MountainCamera(
            new Vector3(-12f, 14f, 8f),
            new Vector3(0f, 12f, 0f)
        );

        /// <summary>Chapter 9: Summit top - triumphant wide view</summary>
        public static readonly MountainCamera Chapter9_SummitTop = new MountainCamera(
            new Vector3(0f, 25f, 15f),
            new Vector3(0f, 20f, 0f)
        );

        /// <summary>Chapter 9: Summit panoramic - 360 view position</summary>
        public static readonly MountainCamera Chapter9_SummitPanoramic = new MountainCamera(
            new Vector3(0f, 22f, 0f),
            new Vector3(10f, 18f, 10f)
        );

        // ===========================================
        // CHAPTER 10 - RUINS
        // ===========================================
        
        /// <summary>Chapter 10: Ruins entrance - mysterious view</summary>
        public static readonly MountainCamera Chapter10_Ruins = new MountainCamera(
            new Vector3(-6f, 8f, 14f),
            new Vector3(-2f, 5f, 0f)
        );

        /// <summary>Chapter 10: Ruins interior - dark depths</summary>
        public static readonly MountainCamera Chapter10_RuinsDepths = new MountainCamera(
            new Vector3(-4f, 3f, 10f),
            new Vector3(-1f, 2f, 0f)
        );

        /// <summary>Chapter 10: Ruins ancient chamber</summary>
        public static readonly MountainCamera Chapter10_RuinsChamber = new MountainCamera(
            new Vector3(-8f, 6f, 8f),
            new Vector3(-3f, 4f, 0f)
        );

        /// <summary>Chapter 10: Ruins collapsed area - dramatic angle</summary>
        public static readonly MountainCamera Chapter10_RuinsCollapsed = new MountainCamera(
            new Vector3(-5f, 10f, 12f),
            new Vector3(-2f, 3f, 0f)
        );

        // ===========================================
        // CASTLE
        // ===========================================
        
        /// <summary>Castle: Grand exterior view</summary>
        public static readonly MountainCamera Castle_Exterior = new MountainCamera(
            new Vector3(10f, 12f, 20f),
            new Vector3(4f, 8f, 0f)
        );

        /// <summary>Castle: Throne room - regal view</summary>
        public static readonly MountainCamera Castle_ThroneRoom = new MountainCamera(
            new Vector3(6f, 6f, 12f),
            new Vector3(3f, 5f, 0f)
        );

        /// <summary>Castle: Tower - high vantage point</summary>
        public static readonly MountainCamera Castle_Tower = new MountainCamera(
            new Vector3(8f, 18f, 10f),
            new Vector3(4f, 10f, 0f)
        );

        /// <summary>Castle: Dungeon - oppressive low angle</summary>
        public static readonly MountainCamera Castle_Dungeon = new MountainCamera(
            new Vector3(5f, 2f, 8f),
            new Vector3(3f, 3f, 0f)
        );

        /// <summary>Castle: Courtyard - open view</summary>
        public static readonly MountainCamera Castle_Courtyard = new MountainCamera(
            new Vector3(7f, 8f, 16f),
            new Vector3(3f, 5f, 0f)
        );

        // ===========================================
        // CHAPTER 15/16 - WELCOME HOME
        // ===========================================
        
        /// <summary>Chapter 15: Welcome Home - nostalgic view</summary>
        public static readonly MountainCamera Chapter15_WelcomeHome = new MountainCamera(
            new Vector3(-4f, 7f, 18f),
            new Vector3(-1f, 4f, 0f)
        );

        /// <summary>Chapter 15: Home interior - warm and close</summary>
        public static readonly MountainCamera Chapter15_HomeInterior = new MountainCamera(
            new Vector3(-2f, 5f, 10f),
            new Vector3(0f, 3f, 0f)
        );

        /// <summary>Chapter 16: Welcome Home continued - emotional view</summary>
        public static readonly MountainCamera Chapter16_WelcomeHome = new MountainCamera(
            new Vector3(-3f, 8f, 16f),
            new Vector3(0f, 5f, 0f)
        );

        /// <summary>Chapter 16: Memory sequence - dreamy angle</summary>
        public static readonly MountainCamera Chapter16_MemorySequence = new MountainCamera(
            new Vector3(0f, 10f, 14f),
            new Vector3(0f, 6f, 2f)
        );

        /// <summary>Chapter 15/16: Family reunion - wide emotional shot</summary>
        public static readonly MountainCamera Chapter15_Reunion = new MountainCamera(
            new Vector3(-5f, 6f, 20f),
            new Vector3(-1f, 4f, 0f)
        );

        // ===========================================
        // EPILOGUE
        // ===========================================
        
        /// <summary>Epilogue: Peaceful conclusion view</summary>
        public static readonly MountainCamera Epilogue_Main = new MountainCamera(
            new Vector3(0f, 8f, 22f),
            new Vector3(0f, 5f, 0f)
        );

        /// <summary>Epilogue: Reflection - calm water view</summary>
        public static readonly MountainCamera Epilogue_Reflection = new MountainCamera(
            new Vector3(2f, 4f, 16f),
            new Vector3(0f, 3f, 0f)
        );

        /// <summary>Epilogue: Sunrise - hopeful angle</summary>
        public static readonly MountainCamera Epilogue_Sunrise = new MountainCamera(
            new Vector3(-3f, 10f, 18f),
            new Vector3(0f, 6f, 5f)
        );

        /// <summary>Epilogue: Final farewell - wide goodbye shot</summary>
        public static readonly MountainCamera Epilogue_Farewell = new MountainCamera(
            new Vector3(0f, 12f, 25f),
            new Vector3(0f, 7f, 0f)
        );

        // ===========================================
        // CHAPTER 18 - CORE
        // ===========================================
        
        /// <summary>Chapter 18: Core entrance - ominous view</summary>
        public static readonly MountainCamera Chapter18_Core = new MountainCamera(
            new Vector3(0f, 8f, 8f),
            new Vector3(0f, 5f, 0f)
        );

        /// <summary>Chapter 18: Core heart - intense close-up</summary>
        public static readonly MountainCamera Chapter18_CoreHeart = new MountainCamera(
            new Vector3(0f, 6f, 5f),
            new Vector3(0f, 4f, 0f)
        );

        /// <summary>Chapter 18: Core hot side - fiery angle</summary>
        public static readonly MountainCamera Chapter18_CoreHot = new MountainCamera(
            new Vector3(4f, 7f, 7f),
            new Vector3(1f, 5f, 0f)
        );

        /// <summary>Chapter 18: Core cold side - frozen angle</summary>
        public static readonly MountainCamera Chapter18_CoreCold = new MountainCamera(
            new Vector3(-4f, 7f, 7f),
            new Vector3(-1f, 5f, 0f)
        );

        /// <summary>Chapter 18: Core collapse - dramatic wide</summary>
        public static readonly MountainCamera Chapter18_CoreCollapse = new MountainCamera(
            new Vector3(0f, 12f, 12f),
            new Vector3(0f, 3f, 0f)
        );

        // ===========================================
        // CHAPTER 19 - MOON
        // ===========================================
        
        /// <summary>Chapter 19: Moon surface - ethereal view</summary>
        public static readonly MountainCamera Chapter19_Moon = new MountainCamera(
            new Vector3(0f, 15f, 18f),
            new Vector3(0f, 12f, 0f)
        );

        /// <summary>Chapter 19: Moon landscape - vast emptiness</summary>
        public static readonly MountainCamera Chapter19_MoonLandscape = new MountainCamera(
            new Vector3(8f, 10f, 25f),
            new Vector3(0f, 8f, 0f)
        );

        /// <summary>Chapter 19: Moon crater - looking down</summary>
        public static readonly MountainCamera Chapter19_MoonCrater = new MountainCamera(
            new Vector3(0f, 20f, 8f),
            new Vector3(0f, 5f, 0f)
        );

        /// <summary>Chapter 19: Earth view from Moon - nostalgic perspective</summary>
        public static readonly MountainCamera Chapter19_EarthView = new MountainCamera(
            new Vector3(-5f, 18f, 15f),
            new Vector3(5f, 10f, -10f)
        );

        /// <summary>Chapter 19: Moon base - sci-fi angle</summary>
        public static readonly MountainCamera Chapter19_MoonBase = new MountainCamera(
            new Vector3(3f, 8f, 14f),
            new Vector3(0f, 6f, 0f)
        );

        /// <summary>Chapter 19: Moon temple - ancient mystery</summary>
        public static readonly MountainCamera Chapter19_MoonTemple = new MountainCamera(
            new Vector3(-6f, 12f, 10f),
            new Vector3(-2f, 8f, 0f)
        );

        // ===========================================
        // CHAPTER 20 - BLACKHOLE
        // ===========================================
        
        /// <summary>Chapter 20: Blackhole approach - dread-inducing view</summary>
        public static readonly MountainCamera Chapter20_Blackhole = new MountainCamera(
            new Vector3(0f, 10f, 20f),
            new Vector3(0f, 0f, 0f)
        );

        /// <summary>Chapter 20: Event horizon - point of no return</summary>
        public static readonly MountainCamera Chapter20_EventHorizon = new MountainCamera(
            new Vector3(0f, 5f, 12f),
            new Vector3(0f, 0f, 0f)
        );

        /// <summary>Chapter 20: Blackhole vortex - spiraling inward</summary>
        public static readonly MountainCamera Chapter20_Vortex = new MountainCamera(
            new Vector3(6f, 8f, 10f),
            new Vector3(0f, 2f, 0f)
        );

        /// <summary>Chapter 20: Singularity - impossibly close</summary>
        public static readonly MountainCamera Chapter20_Singularity = new MountainCamera(
            new Vector3(0f, 3f, 5f),
            new Vector3(0f, 0f, 0f)
        );

        /// <summary>Chapter 20: Time dilation zone - warped perspective</summary>
        public static readonly MountainCamera Chapter20_TimeDilation = new MountainCamera(
            new Vector3(-8f, 6f, 15f),
            new Vector3(2f, 4f, 0f)
        );

        /// <summary>Chapter 20: Beyond the void - transcendent view</summary>
        public static readonly MountainCamera Chapter20_BeyondVoid = new MountainCamera(
            new Vector3(0f, 25f, 0f),
            new Vector3(0f, 0f, 0f)
        );

        /// <summary>Chapter 20: Blackhole escape - desperate angle</summary>
        public static readonly MountainCamera Chapter20_Escape = new MountainCamera(
            new Vector3(0f, 8f, 25f),
            new Vector3(0f, 5f, 0f)
        );

        // ===========================================
        // SPECIAL/TRANSITION VIEWS
        // ===========================================
        
        /// <summary>Nightmare sequence - disorienting angle</summary>
        public static readonly MountainCamera Nightmare = new MountainCamera(
            new Vector3(-8f, 12f, 12f),
            new Vector3(-3f, 7f, 0f)
        );

        /// <summary>Stars/Space - cosmic view</summary>
        public static readonly MountainCamera Stars = new MountainCamera(
            new Vector3(0f, 15f, 20f),
            new Vector3(0f, 10f, 0f)
        );

        /// <summary>Dream sequence - floaty surreal view</summary>
        public static readonly MountainCamera DreamSequence = new MountainCamera(
            new Vector3(0f, 18f, 12f),
            new Vector3(0f, 8f, 3f)
        );

        /// <summary>Boss arena - dramatic confrontation angle</summary>
        public static readonly MountainCamera BossArena = new MountainCamera(
            new Vector3(0f, 10f, 16f),
            new Vector3(0f, 6f, 0f)
        );

        /// <summary>Credits roll - slow pan starting position</summary>
        public static readonly MountainCamera Credits = new MountainCamera(
            new Vector3(-10f, 15f, 20f),
            new Vector3(0f, 8f, 0f)
        );

        // ===========================================
        // UTILITY METHODS
        // ===========================================

        /// <summary>
        /// Get camera preset for a specific chapter area
        /// </summary>
        public static MountainCamera GetChapterCamera(int area)
        {
            return area switch
            {
                0 => Prologue_House,           // Prologue - House
                1 => Chapter1_City,            // Chapter 1 - City
                9 => Chapter9_Summit,          // Chapter 9 - Summit
                10 => Chapter10_Ruins,         // Chapter 10 - Ruins
                15 => Chapter15_WelcomeHome,   // Chapter 15 - Welcome Home
                16 => Chapter16_WelcomeHome,   // Chapter 16 - Welcome Home continued
                17 => Epilogue_Main,           // Epilogue
                18 => Chapter18_Core,          // Chapter 18 - Core
                19 => Chapter19_Moon,          // Chapter 19 - Moon
                20 => Chapter20_Blackhole,     // Chapter 20 - Blackhole
                _ => RotationDefault
            };
        }

        /// <summary>
        /// Get a specific sub-view for a chapter (e.g., different areas within a chapter)
        /// </summary>
        public static MountainCamera GetChapterSubView(int chapter, int subView)
        {
            return (chapter, subView) switch
            {
                // Prologue sub-views
                (0, 0) => Prologue_House,
                (0, 1) => Prologue_HouseInterior,
                (0, 2) => Prologue_Awakening,
                
                // Chapter 1 - City sub-views
                (1, 0) => Chapter1_City,
                (1, 1) => Chapter1_CityStreets,
                (1, 2) => Chapter1_CityRooftops,
                (1, 3) => Chapter1_CityNight,
                
                // Chapter 9 - Summit sub-views
                (9, 0) => Chapter9_Summit,
                (9, 1) => Chapter9_SummitBase,
                (9, 2) => Chapter9_SummitClimb,
                (9, 3) => Chapter9_SummitTop,
                (9, 4) => Chapter9_SummitPanoramic,
                
                // Chapter 10 - Ruins sub-views
                (10, 0) => Chapter10_Ruins,
                (10, 1) => Chapter10_RuinsDepths,
                (10, 2) => Chapter10_RuinsChamber,
                (10, 3) => Chapter10_RuinsCollapsed,
                
                // Castle sub-views
                (11, 0) => Castle_Exterior,
                (11, 1) => Castle_ThroneRoom,
                (11, 2) => Castle_Tower,
                (11, 3) => Castle_Dungeon,
                (11, 4) => Castle_Courtyard,
                
                // Chapter 15 - Welcome Home sub-views
                (15, 0) => Chapter15_WelcomeHome,
                (15, 1) => Chapter15_HomeInterior,
                (15, 2) => Chapter15_Reunion,
                
                // Chapter 16 - Welcome Home continued sub-views
                (16, 0) => Chapter16_WelcomeHome,
                (16, 1) => Chapter16_MemorySequence,
                
                // Epilogue sub-views
                (17, 0) => Epilogue_Main,
                (17, 1) => Epilogue_Reflection,
                (17, 2) => Epilogue_Sunrise,
                (17, 3) => Epilogue_Farewell,
                
                // Chapter 18 - Core sub-views
                (18, 0) => Chapter18_Core,
                (18, 1) => Chapter18_CoreHeart,
                (18, 2) => Chapter18_CoreHot,
                (18, 3) => Chapter18_CoreCold,
                (18, 4) => Chapter18_CoreCollapse,
                
                // Chapter 19 - Moon sub-views
                (19, 0) => Chapter19_Moon,
                (19, 1) => Chapter19_MoonLandscape,
                (19, 2) => Chapter19_MoonCrater,
                (19, 3) => Chapter19_EarthView,
                (19, 4) => Chapter19_MoonBase,
                (19, 5) => Chapter19_MoonTemple,
                
                // Chapter 20 - Blackhole sub-views
                (20, 0) => Chapter20_Blackhole,
                (20, 1) => Chapter20_EventHorizon,
                (20, 2) => Chapter20_Vortex,
                (20, 3) => Chapter20_Singularity,
                (20, 4) => Chapter20_TimeDilation,
                (20, 5) => Chapter20_BeyondVoid,
                (20, 6) => Chapter20_Escape,
                
                _ => RotationDefault
            };
        }

        /// <summary>
        /// Get camera by named preset
        /// </summary>
        public static MountainCamera GetNamedPreset(string name)
        {
            return name.ToLowerInvariant() switch
            {
                // Default views
                "default" or "rotation" => RotationDefault,
                "far" => FarView,
                "overhead" => OverheadView,
                
                // Prologue
                "prologue" or "house" => Prologue_House,
                "house_interior" => Prologue_HouseInterior,
                "awakening" => Prologue_Awakening,
                
                // City
                "city" => Chapter1_City,
                "city_streets" => Chapter1_CityStreets,
                "city_rooftops" => Chapter1_CityRooftops,
                "city_night" => Chapter1_CityNight,
                
                // Summit
                "summit" => Chapter9_Summit,
                "summit_base" => Chapter9_SummitBase,
                "summit_climb" => Chapter9_SummitClimb,
                "summit_top" => Chapter9_SummitTop,
                "summit_panoramic" => Chapter9_SummitPanoramic,
                
                // Ruins
                "ruins" => Chapter10_Ruins,
                "ruins_depths" => Chapter10_RuinsDepths,
                "ruins_chamber" => Chapter10_RuinsChamber,
                "ruins_collapsed" => Chapter10_RuinsCollapsed,
                
                // Castle
                "castle" or "castle_exterior" => Castle_Exterior,
                "throne_room" => Castle_ThroneRoom,
                "tower" => Castle_Tower,
                "dungeon" => Castle_Dungeon,
                "courtyard" => Castle_Courtyard,
                
                // Welcome Home
                "welcome_home" or "home" => Chapter15_WelcomeHome,
                "home_interior" => Chapter15_HomeInterior,
                "reunion" => Chapter15_Reunion,
                "memory" => Chapter16_MemorySequence,
                
                // Epilogue
                "epilogue" => Epilogue_Main,
                "reflection" => Epilogue_Reflection,
                "sunrise" => Epilogue_Sunrise,
                "farewell" => Epilogue_Farewell,
                
                // Core
                "core" => Chapter18_Core,
                "core_heart" => Chapter18_CoreHeart,
                "core_hot" => Chapter18_CoreHot,
                "core_cold" => Chapter18_CoreCold,
                "core_collapse" => Chapter18_CoreCollapse,
                
                // Moon
                "moon" => Chapter19_Moon,
                "moon_landscape" => Chapter19_MoonLandscape,
                "moon_crater" => Chapter19_MoonCrater,
                "earth_view" => Chapter19_EarthView,
                "moon_base" => Chapter19_MoonBase,
                "moon_temple" => Chapter19_MoonTemple,
                
                // Blackhole
                "blackhole" => Chapter20_Blackhole,
                "event_horizon" => Chapter20_EventHorizon,
                "vortex" => Chapter20_Vortex,
                "singularity" => Chapter20_Singularity,
                "time_dilation" => Chapter20_TimeDilation,
                "beyond_void" or "void" => Chapter20_BeyondVoid,
                "escape" => Chapter20_Escape,
                
                // Special
                "nightmare" => Nightmare,
                "stars" or "space" => Stars,
                "dream" => DreamSequence,
                "boss" => BossArena,
                "credits" => Credits,
                
                _ => RotationDefault
            };
        }

        /// <summary>
        /// Create a smooth transition sequence between two presets
        /// Returns an array of interpolated cameras for animation
        /// </summary>
        public static MountainCamera[] CreateTransitionSequence(MountainCamera from, MountainCamera to, int steps = 60)
        {
            MountainCamera[] sequence = new MountainCamera[steps];
            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / (steps - 1);
                float eased = Ease.SineInOut(t);
                sequence[i] = MountainCameraExtensions.Lerp(from, to, eased);
            }
            return sequence;
        }

        /// <summary>
        /// Create a cinematic pan sequence through multiple camera positions
        /// </summary>
        public static MountainCamera[] CreateCinematicPan(MountainCamera[] waypoints, int stepsPerSegment = 30)
        {
            if (waypoints.Length < 2)
                return waypoints;

            int totalSteps = (waypoints.Length - 1) * stepsPerSegment;
            MountainCamera[] sequence = new MountainCamera[totalSteps];

            for (int seg = 0; seg < waypoints.Length - 1; seg++)
            {
                for (int i = 0; i < stepsPerSegment; i++)
                {
                    float t = (float)i / stepsPerSegment;
                    float eased = Ease.SineInOut(t);
                    int index = seg * stepsPerSegment + i;
                    sequence[index] = MountainCameraExtensions.Lerp(waypoints[seg], waypoints[seg + 1], eased);
                }
            }

            return sequence;
        }
    }
}
