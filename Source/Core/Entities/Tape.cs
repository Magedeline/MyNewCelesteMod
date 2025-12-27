using System.Runtime.CompilerServices;
using FMOD.Studio;

namespace DesoloZantas.Core.Core.Entities;

[CustomEntity("Ingeste/Tape")]
public class DesoloZantasTape : Entity
{
    private class UnlockedSide : Entity
    {
        private float alpha;

        private string text;

        private bool waitForKeyPress;

        private float timer;

        private char sideUnlocked;

        public UnlockedSide(char side)
        {
            // Validate and clamp side to A/B/C/D range
            if (side < 'A' || side > 'D')
                side = 'A';
            sideUnlocked = side;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Added(Scene scene)
        {
            base.Added(scene);
            base.Tag = (int)Tags.HUD | (int)Tags.PauseUpdate;
            
            string sideText = sideUnlocked switch
            {
                'B' => "UI_REMIX_UNLOCKED",
                'C' => "UI_CSIDE_UNLOCKED", 
                'D' => "UI_DSIDE_UNLOCKED",
                _ => "UI_BSIDE_UNLOCKED" // Default fallback for any invalid input
            };
            
            text = ActiveFont.FontSize.AutoNewline(Dialog.Clean(sideText), 900);
            base.Depth = -10000;
        }

        public IEnumerator EaseIn()
        {
            _ = Scene;
            while ((alpha += Engine.DeltaTime / 0.5f) < 1f)
            {
                yield return null;
            }
            alpha = 1f;
            yield return 1.5f;
            waitForKeyPress = true;
        }

        public IEnumerator EaseOut()
        {
            waitForKeyPress = false;
            while ((alpha -= Engine.DeltaTime / 0.5f) > 0f)
            {
                yield return null;
            }
            alpha = 0f;
            RemoveSelf();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Update()
        {
            timer += Engine.DeltaTime;
            base.Update();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Render()
        {
            float num = Ease.CubeOut(alpha);
            Vector2 vector = Celeste.Celeste.TargetCenter + new Vector2(0f, 64f);
            Vector2 vector2 = Vector2.UnitY * 64f * (1f - num);
            Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num * 0.8f);
            GFX.Gui["collectables/tape"].DrawJustified(vector - vector2 + new Vector2(0f, 32f), new Vector2(0.5f, 1f), Color.White * num);
            ActiveFont.Draw(text, vector + vector2, new Vector2(0.5f, 0f), Vector2.One, Color.White * num);
            if (waitForKeyPress)
            {
                GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1824f, 984 + ((timer % 1f < 0.25f) ? 6 : 0)));
            }
        }
    }

    public static ParticleType P_Shine;

    public static ParticleType P_Collect;

    public bool IsGhost;

    private Sprite sprite;

    private SineWave hover;

    private BloomPoint bloom;

    private VertexLight light;

    private Wiggler scaleWiggler;

    private bool collected;

    private Vector2[] nodes;

    private EventInstance remixSfx;

    private bool collecting;

    // Custom properties for remix extra support
    public string CustomSide { get; set; } = "";
    public string RemixTrack { get; set; } = "";
    public bool IsRemixExtra { get; set; } = false;
    public Color TapeColor { get; set; } = Color.White;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public DesoloZantasTape(Vector2 position, Vector2[] nodes)
        : base(position)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        this.nodes = nodes;
        Add(new PlayerCollider(OnPlayer));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public DesoloZantasTape(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.NodesOffset(offset))
    {
        // Parse custom properties for remix extra support
        CustomSide = data.Attr("customSide", "");
        RemixTrack = data.Attr("remixTrack", "");
        IsRemixExtra = data.Bool("isRemixExtra", false);
        
        // Parse color if specified
        string colorStr = data.Attr("tapeColor", "White");
        if (System.Enum.TryParse<Color>(colorStr, out Color parsedColor))
        {
            TapeColor = parsedColor;
        }
        else
        {
            // Try to parse hex color
            try
            {
                if (colorStr.StartsWith("#"))
                {
                    colorStr = colorStr.Substring(1);
                }
                uint colorValue = Convert.ToUInt32(colorStr, 16);
                TapeColor = new Color((colorValue >> 16) & 0xFF, (colorValue >> 8) & 0xFF, colorValue & 0xFF);
            }
            catch
            {
                TapeColor = Color.White;
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Added(Scene scene)
    {
        base.Added(scene);
        IsGhost = SaveData.Instance.Areas_Safe[SceneAs<Level>().Session.Area.ID].Cassette;
        
        // Use custom sprite for remix extra if specified
        string spriteId = IsRemixExtra ? "tapeRemix" : (IsGhost ? "tapeGhost" : "tape");
        Add(sprite = GFX.SpriteBank.Create(spriteId));
        sprite.Play("idle");
        
        Add(scaleWiggler = Wiggler.Create(0.25f, 4f, [MethodImpl(MethodImplOptions.NoInlining)] (float f) =>
        {
            sprite.Scale = Vector2.One * (1f + f * 0.25f);
        }));
        Add(bloom = new BloomPoint(0.25f, 16f));
        Add(light = new VertexLight(Color.White, 0.4f, 32, 64));
        Add(hover = new SineWave(0.5f, 0f));
        hover.OnUpdate = [MethodImpl(MethodImplOptions.NoInlining)] (float f) =>
        {
            Sprite obj = sprite;
            VertexLight vertexLight = light;
            float num = (bloom.Y = f * 2f);
            float y = (vertexLight.Y = num);
            obj.Y = y;
        };
        
        // Apply custom color
        if (IsGhost)
        {
            sprite.Color = TapeColor * 0.8f;
        }
        else
        {
            sprite.Color = TapeColor;
        }
        
        // Apply special effects for remix extra
        if (IsRemixExtra)
        {
            // Add rainbow effect or special coloring for remix extra
            float hue = (Engine.Scene.TimeActive * 0.5f) % 1.0f;
            Color rainbowColor = Calc.HsvToColor(hue, 0.8f, 1.0f);
            sprite.Color = Color.Lerp(TapeColor, rainbowColor, 0.3f);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Audio.Stop(remixSfx);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        Audio.Stop(remixSfx);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Update()
    {
        base.Update();
        
        // Apply continuous rainbow effect for remix extra tapes
        if (IsRemixExtra && !collecting)
        {
            float hue = (Engine.Scene.TimeActive * 0.5f) % 1.0f;
            Color rainbowColor = Calc.HsvToColor(hue, 0.8f, 1.0f);
            sprite.Color = Color.Lerp(TapeColor, rainbowColor, 0.3f);
        }
        
        if (!collecting && base.Scene.OnInterval(0.1f))
        {
            SceneAs<Level>().Particles.Emit(P_Shine, 1, base.Center, new Vector2(12f, 10f));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void OnPlayer(CelestePlayer player)
    {
        if (!collected)
        {
            player?.RefillStamina();
            Audio.Play("event:/Ingeste/game/general/tape_unlocked", Position);
            collected = true;
            Celeste.Celeste.Freeze(0.1f);
            
            // Integrate with TapeManager for C-Side and remix extra support
            var level = Scene as Level;
            if (level != null && TapeManager.IsInitialized)
            {
                var areaKey = level.Session.Area;
                var levelName = level.Session.Level;
                char currentSide = getCurrentSide(areaKey);
                
                // Collect the tape through TapeManager
                TapeManager.CollectTape(areaKey, levelName, currentSide);
            }
            
            Add(new Coroutine(CollectRoutine(player)));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IEnumerator CollectRoutine(CelestePlayer player)
    {
        collecting = true;
        Level level = Scene as Level;
        CassetteBlockManager cbm = Scene.Tracker.GetEntity<CassetteBlockManager>();
        level.PauseLock = true;
        level.Frozen = true;
        Tag = Tags.FrozenUpdate;
        level.Session.Cassette = true;
        level.Session.RespawnPoint = level.GetSpawnPoint(nodes[1]);
        level.Session.UpdateLevelStartDashes();
        SaveData.Instance.RegisterCassette(level.Session.Area);
        cbm?.StopBlocks();
        Depth = -1000000;
        level.Shake();
        level.Flash(Color.White);
        level.Displacement.Clear();
        Vector2 camWas = level.Camera.Position;
        Vector2 camTo = (Position - new Vector2(160f, 90f)).Clamp(level.Bounds.Left - 64, level.Bounds.Top - 32, level.Bounds.Right + 64 - 320, level.Bounds.Bottom + 32 - 180);
        level.Camera.Position = camTo;
        level.ZoomSnap((Position - level.Camera.Position).Clamp(60f, 60f, 260f, 120f), 2f);
        sprite.Play("spin", restart: true);
        sprite.Rate = 2f;
        for (float p = 0f; p < 1.5f; p += Engine.DeltaTime)
        {
            sprite.Rate += Engine.DeltaTime * 4f;
            yield return null;
        }
        sprite.Rate = 0f;
        sprite.SetAnimationFrame(0);
        scaleWiggler.Start();
        yield return 0.25f;
        Vector2 from = Position;
        Vector2 to = new Vector2(X, level.Camera.Top - 16f);
        float duration = 0.4f;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
        {
            sprite.Scale.X = MathHelper.Lerp(1f, 0.1f, p);
            sprite.Scale.Y = MathHelper.Lerp(1f, 3f, p);
            Position = Vector2.Lerp(from, to, Ease.CubeIn(p));
            yield return null;
        }
        Visible = false;
        
        // Determine which side to unlock based on current area mode
        char nextSide = determineNextSide(level.Session.Area);
        
        // Use custom remix track if specified, otherwise use default
        string audioEvent = "event:/Ingeste/game/general/tape_preview";
        if (!string.IsNullOrEmpty(RemixTrack))
        {
            audioEvent = RemixTrack;
        }
        
        remixSfx = Audio.Play(audioEvent, "remix_extra", level.Session.Area.ID);
        UnlockedSide message = new UnlockedSide(nextSide);
        Scene.Add(message);
        yield return message.EaseIn();
        while (!Input.MenuConfirm.Pressed)
        {
            yield return null;
        }
        Audio.SetParameter(remixSfx, "end", 1f);
        yield return message.EaseOut();
        duration = 0.25f;
        Add(new Coroutine(level.ZoomBack(duration - 0.05f)));
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
        {
            level.Camera.Position = Vector2.Lerp(camTo, camWas, Ease.SineInOut(p));
            yield return null;
        }
        if (!player.Dead && nodes != null && nodes.Length >= 2)
        {
            Audio.Play("event:/game/general/cassette_bubblereturn", level.Camera.Position + new Vector2(160f, 90f));
            player.StartCassetteFly(nodes[1], nodes[0]);
        }
        foreach (SandwichLava item in level.Entities.FindAll<SandwichLava>())
        {
            item.Leave();
        }
        level.Frozen = false;
        yield return 0.25f;
        cbm?.Finish();
        level.PauseLock = false;
        level.ResetZoom();
        RemoveSelf();
    }

    private char determineNextSide(AreaKey areaKey)
    {
        // Use custom side if specified, but validate it's within A/B/C/D range
        if (!string.IsNullOrEmpty(CustomSide) && CustomSide.Length > 0)
        {
            char customSide = CustomSide[0];
            if (customSide >= 'A' && customSide <= 'D')
            {
                return customSide;
            }
        }
        
        // Get current side and determine next side to unlock
        char currentSide = getCurrentSide(areaKey);
        
        // Use TapeManager to determine what should be unlocked next
        if (TapeManager.IsInitialized)
        {
            char highestUnlocked = TapeManager.GetHighestUnlockedSide(areaKey);
            char nextSide = (char)(highestUnlocked + 1);
            // Ensure we don't go beyond D-Side
            if (nextSide > 'D')
                return 'D';
            return nextSide;
        }
        
        // Fallback logic if TapeManager isn't available - restrict to A/B/C/D only
        return currentSide switch
        {
            'A' => 'B',
            'B' => 'C', 
            'C' => 'D',
            'D' => 'D', // D-Side is the maximum
            _ => 'A'
        };
    }

    private char getCurrentSide(AreaKey areaKey)
    {
        return areaKey.Mode switch
        {
            AreaMode.Normal => 'A',
            AreaMode.BSide => 'B', 
            AreaMode.CSide => 'C',
            // Note: Celeste doesn't have a built-in D-Side mode, so we default to A for unknown modes
            _ => 'A'
        };
    }
}




