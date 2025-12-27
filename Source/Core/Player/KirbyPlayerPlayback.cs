namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby playback system with helpful tips for new and moderate players
    /// Shows how to perform various Kirby abilities and techniques
    /// </summary>
    public class KirbyPlayerPlayback : Entity
    {
        public Vector2 LastPosition;
        public List<global::Celeste.Player.ChaserState> Timeline;
        public PlayerSprite Sprite;
        public KirbyHair Hair;

        private Vector2 start;
        private float time;
        private int index;
        private float loopDelay;
        private float startDelay;
        public float TrimStart;
        public float TrimEnd;
        public float Duration { get; private set; }

        private float rangeMinX;
        private float rangeMaxX;
        private bool ShowTrail;

        // Tip system for players
        private string currentTip = "";
        private float tipDisplayTime = 0f;
        private const float TIP_DURATION = 5f;

        // Collection of helpful tips for Kirby gameplay
        private static readonly List<string> KirbyTips = new List<string>
        {
            "Press H to Inhale enemies and objects!",
            "Use S to Attack or Parry incoming threats!",
            "Dash with Z for a quick burst of speed!",
            "Watch your shoe color - it shows your dash count!",
            "Blue shoes = 0 dashes, Red shoes = max dashes",
            "Hold Inhale to pull in distant objects!",
            "Gain copy abilities by inhaling special enemies!",
            "Parry at the right moment to counter attacks!",
            "Use copy abilities with the Dash button!",
            "Star Warrior mode gives extra powerful attacks!",
            "Yellow shoes (4 dashes) = Good mobility",
            "Green shoes (5 dashes) = Excellent mobility",
            "Combine abilities for powerful combos!",
            "Practice timing your parries for best results!",
            "Explore to find new copy abilities!",
            "Different enemies grant different powers!",
            "Fire ability: Great for burning obstacles",
            "Ice ability: Freeze enemies and platforms",
            "Sword ability: Extended reach and damage",
            "Stone ability: Become invincible briefly",
            "Spark ability: Chain lightning to multiple foes"
        };

        public Vector2 DashDirection { get; private set; }

        public float Time => time;
        public int FrameIndex => index;
        public int FrameCount => Timeline.Count;

        public KirbyPlayerPlayback(EntityData e, Vector2 offset)
        {
            string text = e.Attr("tutorial");
            if (PlaybackData.Tutorials.ContainsKey(text))
            {
                orig_ctor(e, offset);
                return;
            }
            LevelEnter.ErrorMessage = Dialog.Get("postcard_missingtutorial").Replace("((tutorial))", text);
            Logger.Warn("KirbyPlayerPlayback", "Failed to load tutorial '" + text + "'");
            throw new KeyNotFoundException();
        }

        public KirbyPlayerPlayback(Vector2 start, PlayerSpriteMode sprite, List<global::Celeste.Player.ChaserState> timeline)
        {
            rangeMinX = float.MinValue;
            rangeMaxX = float.MaxValue;
            this.start = start;
            base.Collider = new Hitbox(8f, 11f, -4f, -11f);
            Timeline = timeline;
            Position = start;
            time = 0f;
            index = 0;
            Duration = timeline[timeline.Count - 1].TimeStamp;
            TrimStart = 0f;
            TrimEnd = Duration;
            
            Sprite = new PlayerSprite(sprite);
            Add(Hair = new KirbyHair(Sprite));
            Add(Sprite);
            
            base.Collider = new Hitbox(8f, 4f, -4f, -4f);
            
            if (sprite == PlayerSpriteMode.Playback)
            {
                ShowTrail = true;
            }
            
            base.Depth = 9008;
            SetFrame(0);
            
            for (int i = 0; i < 10; i++)
            {
                Hair.AfterUpdate();
            }
            
            Visible = false;
            index = Timeline.Count;

            // Display initial tip
            ShowRandomTip();
        }

        private void Restart()
        {
            Audio.Play("event:/new_content/char/tutorial_ghost/appear", Position);
            Visible = true;
            time = TrimStart;
            index = 0;
            loopDelay = 0.25f;
            
            while (time > Timeline[index].TimeStamp)
            {
                index++;
            }
            
            SetFrame(index);
            
            // Show a new tip when restarting
            ShowRandomTip();
        }

        public void SetFrame(int index)
        {
            global::Celeste.Player.ChaserState chaserState = Timeline[index];
            string currentAnimationID = Sprite.CurrentAnimationID;
            bool flag = base.Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f));
            _ = DashDirection;
            
            Position = start + chaserState.Position;
            
            if (chaserState.Animation != Sprite.CurrentAnimationID && chaserState.Animation != null && Sprite.Has(chaserState.Animation))
            {
                Sprite.Play(chaserState.Animation, restart: true);
            }
            
            Sprite.Scale = chaserState.Scale;
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }
            
            Hair.Color = chaserState.HairColor;
            
            if (Sprite.Mode == PlayerSpriteMode.Playback)
            {
                Sprite.Color = Hair.Color;
            }
            
            DashDirection = chaserState.DashDirection;
            
            if (base.Scene == null)
            {
                return;
            }
            
            // Play sound effects based on animations
            if (!flag && base.Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f)))
            {
                Audio.Play("event:/new_content/char/tutorial_ghost/land", Position);
                ShowTipForAction("landing");
            }
            
            if (!(currentAnimationID != Sprite.CurrentAnimationID))
            {
                return;
            }
            
            string currentAnimationID2 = Sprite.CurrentAnimationID;
            int currentAnimationFrame = Sprite.CurrentAnimationFrame;
            
            switch (currentAnimationID2)
            {
                case "jumpFast":
                case "jumpSlow":
                    Audio.Play("event:/new_content/char/tutorial_ghost/jump", Position);
                    ShowTipForAction("jumping");
                    break;
                case "dreamDashIn":
                    Audio.Play("event:/new_content/char/tutorial_ghost/dreamblock_sequence", Position);
                    ShowTipForAction("dreamdash");
                    break;
                case "dash":
                    if (DashDirection.Y != 0f)
                    {
                        Audio.Play("event:/new_content/char/tutorial_ghost/jump_super", Position);
                    }
                    else if (chaserState.Scale.X > 0f)
                    {
                        Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_right", Position);
                    }
                    else
                    {
                        Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_left", Position);
                    }
                    ShowTipForAction("dashing");
                    break;
                case "climbUp":
                case "climbDown":
                case "wallslide":
                    Audio.Play("event:/new_content/char/tutorial_ghost/grab", Position);
                    ShowTipForAction("climbing");
                    break;
                case "inhale":
                    ShowTipForAction("inhaling");
                    break;
                case "attack":
                    ShowTipForAction("attacking");
                    break;
                default:
                    if ((currentAnimationID2.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) ||
                        (currentAnimationID2.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) ||
                        (currentAnimationID2.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) ||
                        (currentAnimationID2.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) ||
                        (currentAnimationID2.Equals("runStumble") && currentAnimationFrame == 6) ||
                        (currentAnimationID2.Equals("flip") && currentAnimationFrame == 4) ||
                        (currentAnimationID2.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
                    {
                        Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (startDelay > 0f)
            {
                startDelay -= Engine.DeltaTime;
            }
            
            LastPosition = Position;
            base.Update();
            
            // Update tip display timer
            if (tipDisplayTime > 0f)
            {
                tipDisplayTime -= Engine.DeltaTime;
            }
            
            if (index >= Timeline.Count - 1 || Time >= TrimEnd)
            {
                if (Visible)
                {
                    Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);
                }
                
                Visible = false;
                Position = start;
                loopDelay -= Engine.DeltaTime;
                
                if (loopDelay <= 0f)
                {
                    global::Celeste.Player player = ((base.Scene == null) ? null : base.Scene.Tracker.GetEntity<global::Celeste.Player>());
                    if (player == null || (player.X > rangeMinX && player.X < rangeMaxX))
                    {
                        Restart();
                    }
                }
            }
            else if (startDelay <= 0f)
            {
                SetFrame(index);
                time += Engine.DeltaTime;
                
                while (index < Timeline.Count - 1 && time >= Timeline[index + 1].TimeStamp)
                {
                    index++;
                }
            }
            
            if (Visible && ShowTrail && base.Scene != null && base.Scene.OnInterval(0.1f))
            {
                // Note: TrailManager.Add signature may need adjustment for Kirby compatibility
                // TrailManager.Add(Position, Sprite, Hair, Sprite.Scale, Hair.Color, base.Depth + 1);
            }
        }

        public override void Render()
        {
            base.Render();
            
            // Render tip text if active
            if (tipDisplayTime > 0f && !string.IsNullOrEmpty(currentTip))
            {
                Vector2 tipPosition = Position + new Vector2(0f, -40f);
                float alpha = Math.Min(tipDisplayTime / 0.5f, 1f);
                
                ActiveFont.Draw(currentTip, tipPosition, new Vector2(0.5f, 0.5f), 
                    Vector2.One * 0.5f, Color.White * alpha);
            }
        }

        public void orig_ctor(EntityData e, Vector2 offset)
        {
            // Initialize with correct constructor call
            var tutorialTimeline = PlaybackData.Tutorials[e.Attr("tutorial")];
            
            rangeMinX = float.MinValue;
            rangeMaxX = float.MaxValue;
            start = e.Position + offset;
            base.Collider = new Hitbox(8f, 11f, -4f, -11f);
            Timeline = tutorialTimeline;
            Position = start;
            time = 0f;
            index = 0;
            Duration = tutorialTimeline[tutorialTimeline.Count - 1].TimeStamp;
            TrimStart = 0f;
            TrimEnd = Duration;
            
            if (e.Nodes != null && e.Nodes.Length != 0)
            {
                rangeMinX = base.X;
                rangeMaxX = base.X;
                Vector2[] array = e.NodesOffset(offset);
                
                for (int i = 0; i < array.Length; i++)
                {
                    Vector2 vector = array[i];
                    rangeMinX = Math.Min(rangeMinX, vector.X);
                    rangeMaxX = Math.Max(rangeMaxX, vector.X);
                }
            }
            
            startDelay = 1f;
        }

        /// <summary>
        /// Show a random helpful tip
        /// </summary>
        private void ShowRandomTip()
        {
            if (KirbyTips.Count > 0)
            {
                currentTip = KirbyTips[new Random().Next(KirbyTips.Count)];
                tipDisplayTime = TIP_DURATION;
            }
        }

        /// <summary>
        /// Show a contextual tip based on the current action
        /// </summary>
        private void ShowTipForAction(string action)
        {
            string tip = action.ToLower() switch
            {
                "jumping" => "Jump with C! Hold for higher jumps!",
                "dashing" => "Dash with Z! Check your shoe color for dash count!",
                "climbing" => "Grab walls and climb to reach new heights!",
                "landing" => "Time your landings well to maintain momentum!",
                "dreamdash" => "Dream dash through special blocks!",
                "inhaling" => "Inhale with H to copy enemy abilities!",
                "attacking" => "Attack with S or parry enemy attacks!",
                _ => ""
            };

            if (!string.IsNullOrEmpty(tip))
            {
                currentTip = tip;
                tipDisplayTime = TIP_DURATION;
            }
        }

        /// <summary>
        /// Add a custom tip to the tip pool
        /// </summary>
        public static void AddCustomTip(string tip)
        {
            if (!string.IsNullOrEmpty(tip) && !KirbyTips.Contains(tip))
            {
                KirbyTips.Add(tip);
            }
        }
    }
}




