using System.Runtime.CompilerServices;

namespace DesoloZantas.Core.Core.Entities;
[CustomEntity("Ingeste/CharaChaser")]
[Tracked(false)]
public class CharaChaser : Entity
{
    public static ParticleType P_Vanish = new ParticleType
    {
        Size = 1f,
        Color = Color.White,
        Color2 = Calc.HexToColor("9B3FB5"),  // Chara's purple color
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.8f,
        LifeMax = 1.4f,
        SpeedMin = 12f,
        SpeedMax = 24f,
        DirectionRange = (float)Math.PI * 2f
    };

    public static Color HairColor = Calc.HexToColor("9B3FB5");

    public Monocle.Sprite Sprite;

    public PlayerHair Hair;

    private LightOcclude occlude;

    private bool ignorePlayerAnim;

    private int index;

    private CelestePlayer player;

    private bool following;

    private float followBehindTime;

    private float followBehindIndexDelay;

    public bool Hovering;

    private float hoveringTimer;

    private Dictionary<string, SoundSource> loopingSounds;

    private List<SoundSource> inactiveLoopingSounds;

    private bool canChangeMusic;

    public CharaChaser(Vector2 position, int index)
        : base(position)
    {
        loopingSounds = new Dictionary<string, SoundSource>();
        inactiveLoopingSounds = new List<SoundSource>();
        this.index = index;
        base.Depth = -1;
        base.Collider = new Hitbox(6f, 6f, -3f, -7f);
        Collidable = false;
        Sprite = new Monocle.Sprite(GFX.Game, "characters/chara/");
        Sprite.AddLoop("idle", "idle", 0.1f);
        Sprite.Add("fallSlow", "jumpSlow", 0.1f, 2, 3);
        Sprite.Add("laugh", "laugh", 0.04f);
        Sprite.Add("pretendDead", "sleep", 0f, 23);
        Sprite.Add("spawn", "spawn", 0.08f, new int[] { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        Sprite.Add("angry", "angry", 0.04f);
        Sprite.Justify = new Vector2(0.5f, 1f);
        Sprite.Play("fallSlow", restart: true);
        // Chara doesn't use PlayerHair - skip hair initialization to avoid NullReferenceException
        Hair = null;
        Add(Sprite);
        Visible = false;
        followBehindTime = 1.55f;
        followBehindIndexDelay = 0.4f * (float)index;
        Add(new PlayerCollider(OnPlayer));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public CharaChaser(EntityData data, Vector2 offset, int index)
        : this(data.Position + offset, index)
    {
        canChangeMusic = data.Bool("canChangeMusic", defaultValue: true);
    }

    public CharaChaser(EntityData data, Vector2 offset)
        : this(data.Position + offset, 0)
    {
        canChangeMusic = data.Bool("canChangeMusic", defaultValue: true);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Added(Scene scene)
    {
        Level level = scene as Level;
        if (level?.Session.Area.GetLevelSet() == "DesoloZantas")
        {
            orig_Added(scene);
            return;
        }
        base.Added(scene);
        
        // Check for cutscene conditions before starting chase
        Session session = level.Session;
        if (session.GetLevelFlag("11") && session.Area.Mode == AreaMode.Normal)
        {
            RemoveSelf();
            return;
        }
        if (!session.GetLevelFlag("3") && session.Area.Mode == AreaMode.Normal)
        {
            RemoveSelf();
            return;
        }
        if (!session.GetFlag("evil_chara_intro") && session.Level == "3" && session.Area.Mode == AreaMode.Normal)
        {
            // Level 3 cutscene - don't start chasing yet
            return;
        }
        if (!session.GetFlag("chara_intro_warning") && session.Level == "4" && session.Area.Mode == AreaMode.Normal)
        {
            // Level 4 cutscene - don't start chasing yet
            return;
        }
        
        Add(new Coroutine(StartChasingRoutine(level)));
    }
    public IEnumerator StartChasingRoutine(Level level)
    {
        Hovering = true;
        while ((player = Scene.Tracker.GetEntity<CelestePlayer>()) == null || player.JustRespawned)
        {
            yield return null;
        }
        Vector2 to = player.Position;
        yield return followBehindIndexDelay;
        if (!Visible)
        {
            PopIntoExistance(0.5f);
        }
        Sprite.Play("fallSlow");
        if (Hair != null) Hair.Visible = true;
        Hovering = false;
        if (CanChangeMusic(level.Session.Area.Mode == AreaMode.Normal))
        {
            // Set music based on current level
            string musicEvent = level.Session.Level == "4" || level.Session.Level.StartsWith("lvl_3start") 
                ? "event:/Ingeste/music/lvl4/chase" 
                : "event:/Ingeste/music/lvl2/chase";
            level.Session.Audio.Music.Event = musicEvent;
            level.Session.Audio.Apply(forceSixteenthNoteHack: false);
        }
        yield return TweenToPlayer(to);
        Collidable = true;
        following = true;
        Add(occlude = new LightOcclude());
        if (IsChaseEnd(level.Session.Level == "3"))
        {
            Add(new Coroutine(StopChasing()));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IEnumerator TweenToPlayer(Vector2 to)
    {
        Audio.Play("event:/char/badeline/level_entry", Position, "chaser_count", index);
        Vector2 from = Position;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, followBehindTime - 0.1f, start: true);
        tween.OnUpdate = [MethodImpl(MethodImplOptions.NoInlining)] (Tween t) =>
        {
            Position = Vector2.Lerp(from, to, t.Eased);
            if (to.X != from.X)
            {
                Sprite.Scale.X = Math.Abs(Sprite.Scale.X) * (float)Math.Sign(to.X - from.X);
            }
            Trail();
        };
        Add(tween);
        yield return tween.Duration;
    }

    private IEnumerator StopChasing()
    {
        if ((base.Scene as Level).Session.Area.GetLevelSet() == "Desolozantas")
        {
            return orig_StopChasing();
        }
        return custom_StopChasing();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Update()
    {
        if (player != null && player.Dead)
        {
            Sprite.Play("laugh");
            Sprite.X = (float)(Math.Sin(hoveringTimer) * 4.0);
            Hovering = true;
            hoveringTimer += Engine.DeltaTime * 2f;
            Depth = -12500;
            foreach (KeyValuePair<string, SoundSource> loopingSound in loopingSounds)
            {
                loopingSound.Value.Stop();
            }
            Trail();
        }
        else if (following && player != null && player.GetChasePosition(Scene.TimeActive, followBehindTime + followBehindIndexDelay, out var chaseState))
        {
            Position = Calc.Approach(Position, chaseState.Position, 500f * Engine.DeltaTime);
            if (!ignorePlayerAnim && chaseState.Animation != Sprite.CurrentAnimationID && chaseState.Animation != null && Sprite.Has(chaseState.Animation))
            {
                Sprite.Play(chaseState.Animation, restart: true);
            }
            if (!ignorePlayerAnim)
            {
                Sprite.Scale.X = Math.Abs(Sprite.Scale.X) * (float)chaseState.Facing;
            }
            for (int i = 0; i < chaseState.Sounds; i++)
            {
                if (chaseState[i].Action == CelestePlayer.ChaserStateSound.Actions.Oneshot)
                {
                    Audio.Play(chaseState[i].Event, Position, chaseState[i].Parameter, chaseState[i].ParameterValue, "chaser_count", index);
                }
                else if (chaseState[i].Action == CelestePlayer.ChaserStateSound.Actions.Loop && !loopingSounds.ContainsKey(chaseState[i].Event))
                {
                    SoundSource soundSource;
                    if (inactiveLoopingSounds.Count > 0)
                    {
                        soundSource = inactiveLoopingSounds[0];
                        inactiveLoopingSounds.RemoveAt(0);
                    }
                    else
                    {
                        Add(soundSource = new SoundSource());
                    }
                    soundSource.Play(chaseState[i].Event, "chaser_count", index);
                    loopingSounds.Add(chaseState[i].Event, soundSource);
                }
                else if (chaseState[i].Action == CelestePlayer.ChaserStateSound.Actions.Stop)
                {
                    if (loopingSounds.TryGetValue(chaseState[i].Event, out var value))
                    {
                        value.Stop();
                        loopingSounds.Remove(chaseState[i].Event);
                        inactiveLoopingSounds.Add(value);
                    }
                }
            }
            Depth = chaseState.Depth;
            Trail();
        }
        if (Hovering)
        {
            hoveringTimer += Engine.DeltaTime;
            Sprite.Y = (float)(Math.Sin(hoveringTimer * 2f) * 4f);
        }
        if (occlude != null)
        {
            occlude.Visible = !CollideCheck<Solid>();
        }
        base.Update();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Trail()
    {
        if (base.Scene.OnInterval(0.1f))
        {
            TrailManager.Add(this, CelestePlayer.NormalHairColor, 1f, frozenUpdate: false, useRawDeltaTime: false);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void OnPlayer(CelestePlayer player)
    {
        player.Die((player.Position - Position).SafeNormalize());
    }

    private void Die()
    {
        RemoveSelf();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void PopIntoExistance(float duration)
    {
        Visible = true;
        Sprite.Scale = Vector2.Zero;
        Sprite.Color = Color.Transparent;
        if (Hair != null)
        {
            Hair.Visible = true;
            Hair.Alpha = 0f;
        }
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, start: true);
        tween.OnUpdate = [MethodImpl(MethodImplOptions.NoInlining)] (Tween t) =>
        {
            Sprite.Scale = Vector2.One * t.Eased;
            Sprite.Color = Color.White * t.Eased;
            if (Hair != null) Hair.Alpha = t.Eased;
        };
        Add(tween);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private bool OnGround(int dist = 1)
    {
        for (int i = 1; i <= dist; i++)
        {
            if (CollideCheck<Solid>(Position + new Vector2(0f, i)))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void orig_ctor(EntityData data, Vector2 offset, int index)
    {
        loopingSounds = new Dictionary<string, SoundSource>();
        inactiveLoopingSounds = new List<SoundSource>();
        Position = data.Position + offset;
        this.index = index;
        base.Depth = -1;
        base.Collider = new Hitbox(6f, 6f, -3f, -7f);
        Collidable = false;
        Sprite = new Monocle.Sprite(GFX.Game, "characters/chara/");
        Sprite.AddLoop("idle", "idle", 0.1f);
        Sprite.Add("fallSlow", "jumpSlow", 0.1f, 2, 3);
        Sprite.Add("laugh", "laugh", 0.04f);
        Sprite.Add("pretendDead", "sleep", 0f, 23);
        Sprite.Add("spawn", "spawn", 0.08f, new int[] { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        Sprite.Add("angry", "angry", 0.04f);
        Sprite.Justify = new Vector2(0.5f, 1f);
        Sprite.Play("fallSlow", restart: true);
        // Chara doesn't use PlayerHair - skip hair initialization to avoid NullReferenceException
        Hair = null;
        Add(Sprite);
        Visible = false;
        followBehindTime = 1.55f;
        followBehindIndexDelay = 0.4f * (float)index;
        Add(new PlayerCollider(OnPlayer));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void orig_Added(Scene scene)
    {
        base.Added(scene);
        Session session = SceneAs<Level>().Session;
        if (session.GetLevelFlag("2_end") && session.Area.Mode == AreaMode.Normal)
        {
            RemoveSelf();
        }
        else if (!session.GetLevelFlag("14_end") && session.Area.Mode == AreaMode.Normal)
        {
            RemoveSelf();
        }
        else if (!session.GetFlag("evil_chara_intro") && session.Level == "3_intro" && session.Area.Mode == AreaMode.Normal)
        {
            Hovering = false;
            Visible = true;
            if (Hair != null) Hair.Visible = false;
            Sprite.Play("pretendDead");
            if (session.Area.Mode == AreaMode.Normal)
            {
                session.Audio.Music.Event = "event:/Ingeste/music/lvl2/evil_chara";
                session.Audio.Apply(forceSixteenthNoteHack: false);
            }
            base.Scene.Add(new Cutscenes.CS02_CharaIntro(this));
        }
        else if (!session.GetFlag("chara_intro_warning") && session.Level == "3_warning" && session.Area.Mode == AreaMode.Normal)
        {
            Hovering = false;
            Visible = true;
            if (Hair != null) Hair.Visible = false;
            Sprite.Play("pretendDead");
            if (session.Area.Mode == AreaMode.Normal)
            {
                session.Audio.Music.Event = "event:/Ingeste/music/lvl4/chara_warning";
                session.Audio.Apply(forceSixteenthNoteHack: false);
            }
            base.Scene.Add(new Cutscenes.CS04_CharaIntroSecond(this));
        }
        else
        {
            Add(new Coroutine(StartChasingRoutine(base.Scene as Level)));
        }
    }

    private IEnumerator orig_StopChasing()
    {
        Level level = SceneAs<Level>();
        int boundsRight = level.Bounds.X + 148;
        int boundsBottom = level.Bounds.Y + 168 + 184;
        while (X != (float)boundsRight || Y != (float)boundsBottom)
        {
            yield return null;
            if (X > (float)boundsRight)
            {
                X = boundsRight;
            }
            if (Y > (float)boundsBottom)
            {
                Y = boundsBottom;
            }
        }
        following = false;
        ignorePlayerAnim = true;
        Sprite.Play("laugh");
        Sprite.Scale.X = 1f;
        yield return 1f;
        Audio.Play("event:/char/badeline/disappear", Position);
        level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f);
        level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        RemoveSelf();
    }

    private IEnumerator custom_StopChasing()
    {
        Level level = Scene as Level;
        while (!CollideCheck<BadelineOldsiteEnd>())
        {
            yield return null;
        }
        following = false;
        ignorePlayerAnim = true;
        Sprite.Play("laugh");
        Sprite.Scale.X = 1f;
        yield return 1f;
        Audio.Play("event:/char/badeline/disappear", Position);
        level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f);
        level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        RemoveSelf();
    }

    public bool IsChaseEnd(bool value)
    {
        Level level = base.Scene as Level;
        if (level.Session.Area.GetLevelSet() == "DesoloZantas")
        {
            return value;
        }
        if (level.Tracker.CountEntities<CharaChaserEnd>() != 0)
        {
            return true;
        }
        return false;
    }

    public bool CanChangeMusic(bool value)
    {
        if ((base.Scene as Level).Session.Area.GetLevelSet() == "DesoloZantas")
        {
            return value;
        }
        return canChangeMusic;
    }
}




