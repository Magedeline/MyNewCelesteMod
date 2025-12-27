#nullable disable
using System.Runtime.CompilerServices;

namespace DesoloZantas.Core.Core.Triggers;
[CustomEntity("Ingeste/GiygasGlitch")]
public class GiygasGlitchBackgroundTrigger : Trigger
{
  private GiygasGlitchBackgroundTrigger.Duration duration;
  private bool triggered;
  private bool stayOn;
  private bool running;
  private bool doGlitch;
  private const string music_event_path = "event:/final_content/music/lvl19/L.O.A.D.I.N.G";

  [MethodImpl(MethodImplOptions.NoInlining)]
  public GiygasGlitchBackgroundTrigger(EntityData data, Vector2 offset)
    : base(data, offset)
  {
    this.duration = data.Enum<GiygasGlitchBackgroundTrigger.Duration>(nameof(duration));
    this.stayOn = data.Bool("stay");
    this.doGlitch = data.Bool("glitch", true);
  }

  public override void OnEnter(global::Celeste.Player player) => this.Invoke();

  public override void OnLeave(global::Celeste.Player player)
  {
    Audio.SetMusicParam("giygas_glitch", 0f);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public void Invoke()
  {
    if (this.triggered)
      return;
    this.triggered = true;
    if (this.doGlitch)
    {
      this.Add((Component)new Coroutine(this.internalGlitchRoutine()));
    }
    else
    {
      if (this.stayOn)
        return;
      GiygasGlitchBackgroundTrigger.toggle(false);
    }
  }

  private IEnumerator internalGlitchRoutine()
  {
    running = true;
    Tag = (int)Tags.Persistent;

    var duration = this.duration switch
    {
      Duration.Short => 0.2f,
      Duration.Medium => 0.5f,
      Duration.Long => 1.25f,
      _ => throw new ArgumentOutOfRangeException()
    };

    if (this.duration == Duration.Short)
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_short");
    }
    else if (this.duration == Duration.Medium)
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_medium");
    }
    else
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_long");
    }

    Audio.SetMusicParam("giygas_glitch", 1f);
    Tween tween = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.3f, start: true);
    tween.OnUpdate = (System.Action<Tween>)(t => Glitch.Value = 0.6f * t.Eased);
    this.Add(tween);

    yield return GlitchRoutine(duration, stayOn);

    Tag = 0;
    running = false;
  }

  private IEnumerator internalGlitchRoutine(object follow)
  {
    GiygasGlitchBackgroundTrigger backgroundTrigger = this;
    backgroundTrigger.running = true;
    backgroundTrigger.Tag = (int)Tags.Persistent;
    float duration;
    if (backgroundTrigger.duration == GiygasGlitchBackgroundTrigger.Duration.Short)
    {
      duration = 0.2f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_short");
    }
    else if (backgroundTrigger.duration == GiygasGlitchBackgroundTrigger.Duration.Medium)
    {
      duration = 0.5f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_medium");
    }
    else
    {
      duration = 1.25f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      Audio.Play("event:/final_content/game/19_TheEnd/giygas_glitch_long");
    }

    Audio.SetMusicParam("giygas_glitch", 1f);
    Tween tween = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.3f, start: true);
    tween.OnUpdate = (System.Action<Tween>)(t => Glitch.Value = 0.6f * t.Eased);
    backgroundTrigger.Add(tween);

    yield return (object)GiygasGlitchBackgroundTrigger.GlitchRoutine(duration, backgroundTrigger.stayOn);
    backgroundTrigger.Tag = 0;
    backgroundTrigger.running = false;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void toggle(bool on)
  {
    Level scene = Engine.Scene as Level;
    foreach (Backdrop backdrop in scene.Background.GetEach<Backdrop>("GiygasBlackhole"))
      backdrop.ForceVisible = on;
    foreach (Backdrop backdrop in scene.Foreground.GetEach<Backdrop>("GiygasBlackhole"))
      backdrop.ForceVisible = on;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void fade(float alpha, bool max = false)
  {
    Level scene = Engine.Scene as Level;
    foreach (Backdrop backdrop in scene.Background.GetEach<Backdrop>("GiygasBlackhole"))
      backdrop.FadeAlphaMultiplier = max ? Math.Max(backdrop.FadeAlphaMultiplier, alpha) : alpha;
    foreach (Backdrop backdrop in scene.Foreground.GetEach<Backdrop>("GiygasBlackhole"))
      backdrop.FadeAlphaMultiplier = max ? Math.Max(backdrop.FadeAlphaMultiplier, alpha) : alpha;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static IEnumerator GlitchRoutine(float duration, bool stayOn)
  {
    GiygasGlitchBackgroundTrigger.toggle(true);
    if (duration > 0.4f)
    {
      Glitch.Value = 0.3f;
      yield return 0.2f;
      Glitch.Value = 0.0f;
      yield return duration - 0.4f;
      if (!stayOn)
      {
        Glitch.Value = 0.3f;
        yield return 0.2f;
        Glitch.Value = 0.0f;
      }
    }
    else
    {
      Glitch.Value = 0.3f;
      yield return duration;
      Glitch.Value = 0.0f;
    }

    if (!stayOn)
    {
      float a;
      for (a = 0.0f; a < 1.0; a += Engine.DeltaTime / 0.1f)
      {
        GiygasGlitchBackgroundTrigger.fade(a, true);
        yield return null;
      }
      GiygasGlitchBackgroundTrigger.fade(1f);
      yield return duration;
      if (!stayOn)
      {
        for (a = 0.0f; a < 1.0; a += Engine.DeltaTime / 0.1f)
        {
          GiygasGlitchBackgroundTrigger.fade(1f - a);
          yield return null;
        }
        GiygasGlitchBackgroundTrigger.fade(1f);
      }
    }

    if (!stayOn)
    {
      GiygasGlitchBackgroundTrigger.toggle(false);
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public override void Removed(Scene scene)
  {
    if (this.running)
    {
      Glitch.Value = 0.0f;
      GiygasGlitchBackgroundTrigger.fade(1f);
      if (!this.stayOn)
        GiygasGlitchBackgroundTrigger.toggle(false);
    }
    base.Removed(scene);
  }

  private enum Duration
  {
    Short,
    Medium,
    Long,
  }
}




