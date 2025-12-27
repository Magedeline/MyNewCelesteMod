using System.Collections;
using System.Runtime.CompilerServices;
using DesoloZantas.Core.Core.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using MonoMod.Utils;

namespace DesoloZantas.Core.Cutscenes;

public class CS20_Saved : CutsceneEntity
{
    private Player player;
    private Npc20_Granny granny;
    private Npc20_Asriel asriel;
    private Npc20_Madeline maddy;
    private ParticleType flash;
    private float fade;
    private Coroutine grannyWalk;
    private Coroutine maddyWalk;
    private Coroutine asrielWalk;
    private EventInstance snapshot;
    private EventInstance dissipate;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public CS20_Saved(Player player)
        : base(fadeInOnSkip: false)
    {
        this.player = player;
        base.Depth = -1000000;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level obj = scene as Level;
        obj.TimerStopped = true;
        obj.TimerHidden = true;
        obj.SaveQuitDisabled = true;
        obj.SnapColorGrade("none");
        snapshot = Audio.CreateSnapshot("snapshot:/game_10_granny_clouds_dialogue");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void OnBegin(Level level)
    {
        Add(new Coroutine(Cutscene(level)));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IEnumerator Cutscene(Level level)
    {
        // Setup
        player.Dashes = 1;
        player.StateMachine.State = 11;
        player.Sprite.Play("idle");
        player.Visible = false;

        // Music for this scene
        Audio.SetMusic("event:/Ingeste/final_content/music/lvl20/saved");

        // Fade in
        FadeWipe fadeWipe = new FadeWipe(Level, wipeIn: true);
        fadeWipe.Duration = 2f;
        ScreenWipe.WipeColor = Color.White;
        yield return fadeWipe.Duration;
        yield return 1.5f;

        // Camera and player entry
        Add(new Coroutine(Level.ZoomTo(new Vector2(160f, 125f), 2f, 5f)));
        yield return 0.2f;
        Audio.Play("event:/new_content/char/madeline/screenentry_gran");
        yield return 0.3f;
        player.Position = new Vector2(player.X, level.Bounds.Bottom + 8);
        player.Speed.Y = -160f;
        player.Visible = true;
        player.DummyGravity = false;
        player.MuffleLanding = true;
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);

        // Land
        while (!player.OnGround() || player.Speed.Y < 0f)
        {
            float y = player.Speed.Y;
            player.Speed.Y += Engine.DeltaTime * 900f * 0.2f;
            if (y < 0f && player.Speed.Y >= 0f)
            {
                player.Speed.Y = 0f;
                yield return 0.2f;
            }
            yield return null;
        }
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        Audio.Play("event:/new_content/char/madeline/screenentry_gran_landing", player.Position);

        maddy = new Npc20_Madeline(new EntityData(), player.Position + new Vector2(164f, 0f));
        maddy.IdleAnim = "idle";
        maddy.MoveAnim = "walk";
        maddy.Maxspeed = 15f;
        maddy.Add(maddy.Sprite = GFX.SpriteBank.Create("madeline"));
        maddy.Sprite.OnFrameChange = delegate (string anim)
        {
            int currentAnimationFrame = maddy.Sprite.CurrentAnimationFrame;
            if (anim == "walk" && currentAnimationFrame == 2)
            {
                float volume = Calc.ClampedMap((player.Position - maddy.Position).Length(), 64f, 128f, 1f, 0f);
                Audio.Play("event:/char/madeline/footstep", maddy.Position).setVolume(volume);
            }
        };
        Scene.Add(maddy);

        // madeline walks toward player first
        maddyWalk = new Coroutine(maddy.MoveTo(player.Position.X + 32f));
        Add(maddyWalk);

        // Setup Granny NPC
        granny = new Npc20_Granny(new EntityData(), player.Position + new Vector2(164f, 0f));
        granny.IdleAnim = "idle";
        granny.MoveAnim = "walk";
        granny.Maxspeed = 15f;
        granny.Add(granny.Sprite = GFX.SpriteBank.Create("granny"));
        GrannyLaughSfx grannyLaughSfx = new GrannyLaughSfx(granny.Sprite);
        grannyLaughSfx.FirstPlay = false;
        granny.Add(grannyLaughSfx);
        granny.Sprite.OnFrameChange = delegate (string anim)
        {
            int currentAnimationFrame = granny.Sprite.CurrentAnimationFrame;
            if (anim == "walk" && currentAnimationFrame == 2)
            {
                float volume = Calc.ClampedMap((player.Position - granny.Position).Length(), 64f, 136f, 1f, 0f);
                Audio.Play("event:/new_content/char/granny/cane_tap_ending", granny.Position).setVolume(volume);
            }
        };
        Scene.Add(granny);

        // Granny walks toward player
        grannyWalk = new Coroutine(granny.MoveTo(player.Position.X + 32f));
        Add(grannyWalk);

        // Setup Asriel NPC
        asriel = new Npc20_Asriel(new EntityData(), player.Position + new Vector2(164f, 0f));
        asriel.IdleAnim = "idle";
        asriel.MoveAnim = "walk";
        asriel.Maxspeed = 15f;
        asriel.Add(asriel.Sprite = GFX.SpriteBank.Create("asriel"));
        asriel.Sprite.OnFrameChange = delegate (string anim)
        {
            int currentAnimationFrame = asriel.Sprite.CurrentAnimationFrame;
            if (anim == "walk" && currentAnimationFrame == 2)
            {
                float volume = Calc.ClampedMap((player.Position - asriel.Position).Length(), 64f, 132f, 1f, 0f);
                Audio.Play("event:/char/madeline/footstep", asriel.Position).setVolume(volume);
            }
        };
        Scene.Add(asriel);

        // Asriel walks toward player
        asrielWalk = new Coroutine(asriel.MoveTo(player.Position.X + 32f, 0f, true));
        Add(asrielWalk);

        // Small animation beats
        yield return 2f;
        player.Facing = Facings.Left;
        yield return 1.6f;
        player.Facing = Facings.Right;
        yield return 0.8f;
        yield return player.DummyWalkToExact((int)player.X + 4, walkBackwards: false, 0.4f);
        yield return 0.8f;

        // Dialog block 1: Restoration and Farewell
        yield return Textbox.Say(
            "CH20_RESTORATION_AND_FAREWELL",
            Laugh, StopLaughing, StepForward, GrannyDisappear, FadeToWhite, WaitForGranny,
            Trigger21, Trigger21_6, Trigger21_7
        );

        // Dialog block 2: Goodbye Asriel and Granny
        yield return Textbox.Say(
            "CH20_goodbye_asriel_and_granny",
            Laugh, StopLaughing, StepForward, GrannyDisappear, FadeToWhite, WaitForGranny, Trigger23
        );

        // Wait for final fade if any
        yield return 2f;
        while (fade < 1f)
        {
            yield return null;
        }
        EndCutscene(level);
    }

    private IEnumerator WaitForGranny()
    {
        while (grannyWalk != null && !grannyWalk.Finished)
        {
            yield return null;
        }
    }

    private IEnumerator Laugh()
    {
        granny.Sprite.Play("laugh");
        yield break;
    }

    private IEnumerator StopLaughing()
    {
        granny.Sprite.Play("idle");
        yield break;
    }

    private IEnumerator StepForward()
    {
        yield return player.DummyWalkToExact((int)player.X + 8, walkBackwards: false, 0.4f);
    }

    private IEnumerator FadeToWhite()
    {
        Add(new Coroutine(DoFadeToWhite()));
        yield break;
    }

    private IEnumerator DoFadeToWhite()
    {
        Add(new Coroutine(Level.ZoomBack(8f)));
        while (fade < 1f)
        {
            fade = Calc.Approach(fade, 1f, Engine.DeltaTime / 8f);
            yield return null;
        }
    }

    // Custom trigger callbacks to align with dialog tokens:
    // {dz_trigger 21 ...}
    private IEnumerator Trigger21()
    {
        // The two mountains begin to merge back into one
        // Hook VFX/SFX for mountain merge here        Audio.Play("event:/Ingeste/final_content/vfx/mountain_merge");
        Level.Shake(0.5f);
        yield return 1f;
    }

    // {dz_trigger 21.5 ...}

    // {trigger 21.6 ...}
    private IEnumerator Trigger21_6()
    {
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
            {
                // Brilliant light surrounds Madeline and Badeline's spirits
                Color from2 = Color.White * 0.5f;
                Color to2 = Color.White * 1f;
            for (float p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime / 0.5f)
            {
                Color.Lerp(from2, to2, p2);
                Level.ParticlesFG.Emit(flash, 1, player.Center, Vector2.One * 8f, from2);
                yield return null;
            }

                yield break;
            }
    }

    // {trigger 21.7 ...}
    private IEnumerator Trigger21_7()
    {
        Audio.Play("event:/Ingeste/final_content/char/kirby/restored");
        yield return 1f;
        
        yield break;
    }

    // {trigger 22 ...}
    private IEnumerator GrannyDisappear()
    {
        Audio.SetMusicParam("end", 1f);
        Add(new Coroutine(player.DummyWalkToExact((int)player.X + 8, walkBackwards: false, 0.4f)));
        yield return 0.1f;
        dissipate = Audio.Play("event:/new_content/char/granny/dissipate", granny.Position);
        MTexture frame = granny.Sprite.GetFrame(granny.Sprite.CurrentAnimationID, granny.Sprite.CurrentAnimationFrame);
        Level.Add(new DisperseImage(granny.Position, new Vector2(1f, -0.1f), granny.Sprite.Origin, granny.Sprite.Scale, frame));
        frame = asriel.Sprite.GetFrame(asriel.Sprite.CurrentAnimationID, asriel.Sprite.CurrentAnimationFrame);
        Level.Add(new DisperseImage(asriel.Position, new Vector2(1f, -0.1f), asriel.Sprite.Origin, asriel.Sprite.Scale, frame));
        yield return null;
        granny.Visible = false;
        asriel.Visible = false;
        yield return 3.5f;
    }

    // {trigger 23 ...}
    private IEnumerator Trigger23()
    {
        // The screen fades to white
        yield return FadeToWhite();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void OnEnd(Level level)
    {
        Dispose();
        if (WasSkipped)
        {
            Audio.Stop(dissipate);
        }
        Level.OnEndOfFrame += [MethodImpl(MethodImplOptions.NoInlining)] () =>
        {
            Level.TeleportTo(player, "end-cinematic_maggy", Player.IntroTypes.Transition);
        };
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Dispose();
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        Dispose();
    }

    private void Dispose()
    {
        Audio.ReleaseSnapshot(snapshot);
        snapshot = null;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Render()
    {
        if (fade > 0f)
        {
            Draw.Rect(Level.Camera.X - 1f, Level.Camera.Y - 1f, 322f, 182f, Color.White * fade);
        }
    }
}
