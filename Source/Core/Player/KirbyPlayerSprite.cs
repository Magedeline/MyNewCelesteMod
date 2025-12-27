using System.Xml;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby player sprite with customizable ability hats
    /// Supports various copy abilities with visual indicators
    /// </summary>
    public class KirbyPlayerSprite : Sprite
    {
        // Animation constants for Kirby - matching KirbySprites.xml
        public const string Idle = "idle";
        public const string Walk = "walk";
        public const string Run = "run";
        public const string RunSlow = "runSlow";
        public const string RunFast = "runFast";
        public const string Jump = "jump";
        public const string JumpFast = "jumpFast";
        public const string JumpSlow = "jumpSlow";
        public const string Fall = "fall";
        public const string Dash = "dash";
        public const string Inhale = "inhale";
        public const string Exhale = "exhale";
        public const string Attack = "attack";
        public const string Parry = "parry";
        public const string Hurt = "hurt";
        public const string Duck = "duck";
        public const string Slide = "slide";
        public const string Float = "float";
        public const string Land = "land";
        public const string Crouch = "crouch";
        
        // Climbing animations
        public const string ClimbUp = "climbUp";
        public const string ClimbDown = "climbDown";
        public const string Wallslide = "wallslide";
        
        // Dream dash animations
        public const string DreamDashIn = "dreamDashIn";
        public const string DreamDashLoop = "dreamDashLoop";
        public const string DreamDashOut = "dreamDashOut";

        // Copy ability animations - Fire
        public const string FireIdle = "fire_idle";
        public const string FireWalk = "fire_walk";
        public const string FireAttack = "fire_attack";
        public const string FireBurst = "fire_burst";
        public const string FireDash = "fire_dash";
        public const string FirePillar = "fire_pillar";
        
        // Copy ability animations - Ice
        public const string IceIdle = "ice_idle";
        public const string IceWalk = "ice_walk";
        public const string IceAttack = "ice_attack";
        public const string IceFreeze = "ice_freeze";
        public const string IceGlide = "ice_glide";
        public const string IceShatter = "ice_shatter";
        
        // Copy ability animations - Spark
        public const string SparkIdle = "spark_idle";
        public const string SparkWalk = "spark_walk";
        public const string SparkAttack = "spark_attack";
        public const string SparkChain = "spark_chain";
        public const string SparkPulse = "spark_pulse";
        
        // Copy ability animations - Stone
        public const string StoneIdle = "stone_idle";
        public const string StoneWalk = "stone_walk";
        public const string StoneAttack = "stone_attack";
        public const string StoneTransform = "stone_transform";
        public const string StoneForm = "stone_form";
        public const string StoneCrush = "stone_crush";
        public const string StoneCounter = "stone_counter";
        
        // Copy ability animations - Sword
        public const string SwordIdle = "sword_idle";
        public const string SwordWalk = "sword_walk";
        public const string SwordAttack = "sword_attack";
        public const string SwordAttack1 = "sword_attack1";
        public const string SwordAttack2 = "sword_attack2";
        public const string SwordSpin = "sword_spin";
        public const string SwordThrust = "sword_thrust";
        public const string SwordCombo = "sword_combo";
        
        // Copy ability animations - Beam
        public const string BeamIdle = "beam_idle";
        public const string BeamWalk = "beam_walk";
        public const string BeamAttack = "beam_attack";
        public const string BeamWhip = "beam_whip";
        public const string BeamCycle = "beam_cycle";
        
        // Copy ability animations - Cutter
        public const string CutterIdle = "cutter_idle";
        public const string CutterWalk = "cutter_walk";
        public const string CutterAttack = "cutter_attack";
        public const string CutterThrow = "cutter_throw";
        public const string CutterBoomerang = "cutter_boomerang";
        
        // Copy ability animations - Hammer
        public const string HammerIdle = "hammer_idle";
        public const string HammerWalk = "hammer_walk";
        public const string HammerAttack = "hammer_attack";
        public const string HammerSpin = "hammer_spin";
        public const string HammerSlam = "hammer_slam";
        
        // Copy ability animations - Wing
        public const string WingIdle = "wing_idle";
        public const string WingFly = "wing_fly";
        public const string WingAttack = "wing_attack";
        public const string WingDive = "wing_dive";
        public const string WingGlide = "wing_glide";
        
        // Combat animations
        public const string PunchA = "punchA";
        public const string PunchB = "punchB";
        public const string Backflip = "backflip";
        public const string Spin = "spin";
        public const string GroundPound = "groundpound";
        public const string GrabEnemy = "grab_enemy";
        public const string SlideAttack = "slide";
        
        // Death animations
        public const string PreDeath = "pre_death";
        public const string MidDeath = "mid_death";
        public const string PostDeath = "post_death";
        public const string Death = "death";

        private static Dictionary<string, KirbyAnimMetadata> FrameMetadata = new Dictionary<string, KirbyAnimMetadata>(StringComparer.OrdinalIgnoreCase);

        public int HairCount = 4;
        private string spriteName;
        private string currentAbility = "None";

        public PlayerSpriteMode Mode { get; private set; }

        // Hat textures for different abilities
        private Dictionary<string, MTexture> abilityHats = new Dictionary<string, MTexture>();

        public Vector2 HairOffset
        {
            get
            {
                if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
                {
                    return value.HairOffset;
                }
                return Vector2.Zero;
            }
        }

        public Vector2 HatOffset
        {
            get
            {
                if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
                {
                    return value.HatOffset;
                }
                return new Vector2(0f, -12f); // Default hat position
            }
        }

        public float CarryYOffset
        {
            get
            {
                if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
                {
                    return (float)value.CarryYOffset * Scale.Y;
                }
                return 0f;
            }
        }

        public int HairFrame
        {
            get
            {
                if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
                {
                    return value.Frame;
                }
                return 0;
            }
        }

        public bool HasHair
        {
            get
            {
                if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
                {
                    return value.HasHair;
                }
                return false;
            }
        }

        public bool HasHat => !string.IsNullOrEmpty(currentAbility) && currentAbility != "None";

        public bool Running
        {
            get
            {
                if (base.LastAnimationID != null)
                {
                    return base.LastAnimationID.StartsWith("run") || base.LastAnimationID.StartsWith("walk");
                }
                return false;
            }
        }

        public KirbyPlayerSprite(PlayerSpriteMode mode)
            : base(null, null)
        {
            Mode = mode;
            spriteName = "kirby_player";
            
            try
            {
                GFX.SpriteBank.CreateOn(this, spriteName);
                LoadAbilityHats();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "KirbyPlayerSprite", $"Failed to create sprite: {ex.Message}");
                // Fallback to basic player sprite
                GFX.SpriteBank.CreateOn(this, "player");
            }
        }

        /// <summary>
        /// Load all ability hat textures
        /// </summary>
        private void LoadAbilityHats()
        {
            string[] abilities = { "Fire", "Ice", "Spark", "Stone", "Sword", "Beam", "Cutter", "Hammer", "Wing" };
            
            foreach (string ability in abilities)
            {
                string hatPath = $"characters/kirby/hats/{ability.ToLower()}";
                if (GFX.Game.Has(hatPath))
                {
                    abilityHats[ability] = GFX.Game[hatPath];
                }
            }
        }

        /// <summary>
        /// Set the current copy ability and update visuals
        /// </summary>
        public void SetAbility(string ability)
        {
            if (currentAbility == ability)
                return;

            currentAbility = ability;
            
            // Update animations based on ability
            string idleAnim = ability == "None" ? Idle : $"{ability.ToLower()}_idle";
            if (Has(idleAnim))
            {
                Play(idleAnim);
            }
        }

        /// <summary>
        /// Get the current copy ability
        /// </summary>
        public string GetAbility()
        {
            return currentAbility;
        }

        public override void Render()
        {
            Vector2 renderPosition = base.RenderPosition;
            base.RenderPosition = base.RenderPosition.Floor();
            
            // Render base sprite
            base.Render();
            
            // Render ability hat if applicable
            if (HasHat && abilityHats.TryGetValue(currentAbility, out MTexture hatTexture))
            {
                Vector2 hatPosition = base.RenderPosition + HatOffset * Scale;
                hatTexture.DrawCentered(hatPosition, Color.White, Scale);
            }
            
            base.RenderPosition = renderPosition;
        }

        /// <summary>
        /// Create frame metadata for Kirby animations
        /// </summary>
        public static void CreateFramesMetadata(string sprite)
        {
            foreach (SpriteDataSource source in GFX.SpriteBank.SpriteData[sprite].Sources)
            {
                XmlElement xmlElement = source.XML["Metadata"];
                string text = source.Path;
                
                if (xmlElement == null)
                {
                    continue;
                }
                
                if (!string.IsNullOrEmpty(source.OverridePath))
                {
                    text = source.OverridePath;
                }
                
                foreach (XmlElement item in xmlElement.GetElementsByTagName("Frames"))
                {
                    string text2 = text + item.Attr("path", "");
                    string[] array = item.Attr("hair").Split(new char[1] { '|' });
                    string[] array2 = item.Attr("carry", "").Split(new char[1] { ',' });
                    string[] array3 = item.Attr("hat", "").Split(new char[1] { '|' });
                    
                    for (int i = 0; i < Math.Max(Math.Max(array.Length, array2.Length), array3.Length); i++)
                    {
                        KirbyAnimMetadata kirbyAnimMetadata = new KirbyAnimMetadata();
                        string text3 = text2 + ((i < 10) ? "0" : "") + i;
                        
                        if (i == 0 && !GFX.Game.Has(text3))
                        {
                            text3 = text2;
                        }
                        
                        FrameMetadata[text3] = kirbyAnimMetadata;
                        
                        // Parse hair data
                        if (i < array.Length)
                        {
                            if (array[i].Equals("x", StringComparison.OrdinalIgnoreCase) || array[i].Length <= 0)
                            {
                                kirbyAnimMetadata.HasHair = false;
                            }
                            else
                            {
                                string[] array4 = array[i].Split(new char[1] { ':' });
                                string[] array5 = array4[0].Split(new char[1] { ',' });
                                kirbyAnimMetadata.HasHair = true;
                                kirbyAnimMetadata.HairOffset = new Vector2(Convert.ToInt32(array5[0]), Convert.ToInt32(array5[1]));
                                kirbyAnimMetadata.Frame = ((array4.Length >= 2) ? Convert.ToInt32(array4[1]) : 0);
                            }
                        }
                        
                        // Parse carry data
                        if (i < array2.Length && array2[i].Length > 0)
                        {
                            kirbyAnimMetadata.CarryYOffset = int.Parse(array2[i]);
                        }
                        
                        // Parse hat data
                        if (i < array3.Length && array3[i].Length > 0)
                        {
                            string[] array6 = array3[i].Split(new char[1] { ',' });
                            if (array6.Length >= 2)
                            {
                                kirbyAnimMetadata.HatOffset = new Vector2(Convert.ToInt32(array6[0]), Convert.ToInt32(array6[1]));
                            }
                        }
                    }
                }
            }
        }

        public static void ClearFramesMetadata()
        {
            FrameMetadata.Clear();
        }

        /// <summary>
        /// Get ability-specific color for effects
        /// </summary>
        public Color GetAbilityColor()
        {
            return currentAbility switch
            {
                "Fire" => new Color(255, 100, 50),
                "Ice" => new Color(100, 200, 255),
                "Spark" => new Color(255, 255, 100),
                "Stone" => new Color(150, 150, 150),
                "Sword" => new Color(200, 200, 255),
                "Beam" => new Color(255, 200, 255),
                "Cutter" => new Color(255, 220, 100),
                "Hammer" => new Color(255, 150, 100),
                "Wing" => new Color(200, 255, 255),
                _ => Color.White
            };
        }

        /// <summary>
        /// Check if the current ability has a specific attack animation
        /// </summary>
        public bool HasAbilityAttack(string attackType)
        {
            if (currentAbility == "None")
                return false;

            string animName = $"{currentAbility.ToLower()}_{attackType}";
            return Has(animName);
        }
        
        /// <summary>
        /// Play an ability-specific animation if it exists, otherwise fall back to base animation
        /// </summary>
        public void PlayAbilityAnimation(string baseAnimation, bool restart = false)
        {
            if (currentAbility != "None")
            {
                string abilityAnim = $"{currentAbility.ToLower()}_{baseAnimation}";
                if (Has(abilityAnim))
                {
                    Play(abilityAnim, restart);
                    return;
                }
            }
            
            if (Has(baseAnimation))
            {
                Play(baseAnimation, restart);
            }
        }
        
        /// <summary>
        /// Play a combat animation (punch, kick, spin, etc.)
        /// </summary>
        public void PlayCombatAnimation(string combatAnim, bool restart = true)
        {
            if (Has(combatAnim))
            {
                Play(combatAnim, restart);
            }
            else
            {
                IngesteLogger.Warn($"Combat animation '{combatAnim}' not found for Kirby sprite");
            }
        }
        
        /// <summary>
        /// Check if currently playing a combat animation
        /// </summary>
        public bool IsPlayingCombatAnimation()
        {
            if (CurrentAnimationID == null) return false;
            
            return CurrentAnimationID == PunchA ||
                   CurrentAnimationID == PunchB ||
                   CurrentAnimationID == Backflip ||
                   CurrentAnimationID == Spin ||
                   CurrentAnimationID == GroundPound ||
                   CurrentAnimationID == GrabEnemy ||
                   CurrentAnimationID == SlideAttack;
        }
        
        /// <summary>
        /// Get hitbox data for current combat frame
        /// </summary>
        public bool TryGetCombatHitbox(out Vector2 offset, out Vector2 size)
        {
            offset = Vector2.Zero;
            size = new Vector2(16f, 16f);
            
            if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value) && value.IsCombatFrame)
            {
                offset = value.HitboxOffset;
                size = value.HitboxSize;
                return true;
            }
            
            return false;
        }
    }
}




