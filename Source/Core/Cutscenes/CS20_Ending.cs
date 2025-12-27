using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using Celeste;

namespace DesoloZantastras.Core.Cutscenes;

public class CS20_Ending : CutsceneEntity
{
    private const int FPS = 12;

    private const float DELAY = 1f / 12f;

    private Atlas Atlas;

    private List<MTexture> Frames;

    private int frame;

    private float fade = 1f;

    private float zoom = 1f;

    private float computerFade;

    private Coroutine talkingLoop;

    private Vector2 center = Celeste.Celeste.TargetCenter;

    private Coroutine cutscene;

    private Color fadeColor = Color.White;

    private Image attachment;

    private Image cursor;

    private Image ok;

    private Image picture;

    private const float PictureIdleScale = 0.9f;

    private float speedrunTimerEase;

    private string speedrunTimerChapterString;

    private string speedrunTimerFileString;

    private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER") + ":";

    private string version = Celeste.Celeste.Instance.Version.ToString();

    private bool showTimer;

    private EventInstance endAmbience;

    private EventInstance cinIntro;

    private bool counting;

    private float timer;

    public CS20_Ending(global::Celeste.Player player)
        : base(fadeInOnSkip: false, endingChapterAfter: true)
    {
        base.Tag = Tags.HUD;
        player.StateMachine.State = 11;
        player.DummyAutoAnimate = false;
        player.Sprite.Rate = 0f;
        RemoveOnSkipped = false;
        Add(new LevelEndingHook(delegate
        {
            Audio.Stop(cinIntro);
        }));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level obj = scene as Level;
        obj.TimerStopped = true;
        obj.TimerHidden = true;
        obj.SaveQuitDisabled = true;
        obj.PauseLock = true;
        obj.AllowHudHide = false;
    }

    public override void OnBegin(Level level)
    {
        Audio.SetAmbience(null);
        level.AutoSave();
        speedrunTimerChapterString = TimeSpan.FromTicks(level.Session.Time).ShortGameplayFormat();
        speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
        SpeedrunTimerDisplay.CalculateBaseSizes();
        Add(cutscene = new Coroutine(Cutscene(level)));
    }

    private IEnumerator Cutscene(Level level)
    {
        if (level.Wipe != null)
        {
            level.Wipe.Cancel();
        }
        while (level.IsAutoSaving())
        {
            yield return null;
        }
        yield return 1f;
        Atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "TheEnd"), Atlas.AtlasDataFormat.PackerNoAtlas);
        Frames = Atlas.GetAtlasSubtextures("");
        Add(attachment = new Image(Atlas["21-window"]));
        Add(picture = new Image(Atlas["21-picture"]));
        Add(ok = new Image(Atlas["21-button"]));
        Add(cursor = new Image(Atlas["21-cursor"]));
        attachment.Visible = false;
        picture.Visible = false;
        ok.Visible = false;
        cursor.Visible = false;
        level.PauseLock = false;
        yield return 2f;
        cinIntro = Audio.Play("event:/Ingeste/final_content/music/lvl20/cinematic/end_intro");
        Audio.SetAmbience(null);
        counting = true;
        Add(new Coroutine(Fade(1f, 0f, 4f)));
        Add(new Coroutine(Zoom(1.38f, 1.2f, 4f)));
        yield return Loop("0", 2f);
        Input.Rumble(RumbleStrength.Climb, RumbleLength.TwoSeconds);
        yield return Loop("0,1,1,0,0,1,1,0*8", 2f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        Audio.SetMusic("event:/Ingeste/final_content/music/lvl20/cinematic/end", startPlaying: true, allowFadeOut: false);
        endAmbience = Audio.Play("event:/new_content/env/10_endscene");
        Add(new Coroutine(Zoom(1.2f, 1.05f, 0.06f, Ease.CubeOut)));
        yield return Play("2-7");
        yield return Loop("7", 1.5f);
        yield return Play("8-10,10*20");
        List<int> frameData = GetFrameData("10-13,13*16,14*28,14-17,14*24");
        float num = (float)(frameData.Count + 3) * (1f / 12f);
        fadeColor = Color.Black;
        Add(new Coroutine(Zoom(1.05f, 1f, num, Ease.Linear)));
        Add(new Coroutine(Fade(0f, 1f, num * 0.1f, num * 0.85f)));
        Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
        {
            Audio.Play("event:/new_content/game/10_farewell/endscene_dial_theo");
        }, num, start: true));
        yield return Play(frameData);
        frame = 18;
        fade = 1f;
        yield return 0.5f;
        yield return Fade(1f, 0f, 1.2f);
        Add(talkingLoop = new Coroutine(Loop("18*24,19,19,18*6,20,20")));
        yield return 1f;
        yield return Textbox.Say("CH20_END_CINEMATIC", ShowPicture);
        Audio.SetMusicParam("end", 1f);
        Audio.Play("event:/Ingeste/final_content/game/20_the_beginning/endscene_videozoom");
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 4f)
        {
            Audio.SetParameter(endAmbience, "fade", 1f - p);
            computerFade = p;
            picture.Scale = Vector2.One * (0.9f + 0.100000024f * p);
            yield return null;
        }
        EndCutscene(level, removeSelf: false);
    }

    private IEnumerator ShowPicture()
    {
        center = new Vector2(1230f, 312f);
        Add(new Coroutine(Fade(0f, 1f, 0.25f)));
        Add(new Coroutine(Zoom(1f, 1.1f, 0.25f)));
        yield return 0.25f;
        if (talkingLoop != null)
        {
            talkingLoop.RemoveSelf();
        }
        talkingLoop = null;
        yield return null;
        frame = 21; // Assuming frame 21 is the computer screen with attachment
        cursor.Visible = true;
        center = Celeste.Celeste.TargetCenter;
        Add(new Coroutine(Fade(1f, 0f, 0.25f)));
        Add(new Coroutine(Zoom(1.1f, 1f, 0.25f)));
        yield return 0.25f;
        Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_notify");
        attachment.Origin = Celeste.Celeste.TargetCenter;
        attachment.Position = Celeste.Celeste.TargetCenter;
        attachment.Visible = true; // Make attachment visible
        attachment.Scale = Vector2.Zero; // Start scaled down
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.3f)
        {
            attachment.Scale.Y = 0.25f + 0.75f * Ease.BigBackOut(p);
            attachment.Scale.X = 1.5f - 0.5f * Ease.BigBackOut(p);
            yield return null;
        }
        yield return 0.25f;
        ok.Position = new Vector2(1208f, 620f);
        ok.Origin = ok.Position;
        ok.Visible = true;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
        {
            ok.Scale.Y = 0.25f + 0.75f * Ease.BigBackOut(p);
            ok.Scale.X = 1.5f - 0.5f * Ease.BigBackOut(p);
            yield return null;
        }
        yield return 0.8f;
        Vector2 from = cursor.Position;
        Vector2 to = from + new Vector2(-160f, -190f);
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
        {
            cursor.Position = from + (to - from) * Ease.CubeInOut(p);
            yield return null;
        }
        yield return 0.2f;
        Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_click");
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
        {
            ok.Scale.Y = 1f - Ease.BigBackIn(p);
            ok.Scale.X = 1f - Ease.BigBackIn(p);
            yield return null;
        }
        ok.Visible = false;
        yield return 0.1f;
        picture.Origin = Celeste.Celeste.TargetCenter;
        picture.Position = Celeste.Celeste.TargetCenter;
        picture.Visible = true;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.4f)
        {
            picture.Scale.Y = (0.9f + 0.1f * Ease.BigBackOut(p)) * 0.9f;
            picture.Scale.X = (1.1f - 0.1f * Ease.BigBackOut(p)) * 0.9f;
            picture.Position = Celeste.Celeste.TargetCenter + Vector2.UnitY * 120f * (1f - Ease.CubeOut(p));
            picture.Color = Color.White * p; // Fade in picture
            yield return null;
        }
        picture.Position = Celeste.Celeste.TargetCenter;
        attachment.Visible = false;
        to = cursor.Position;
        from = new Vector2(120f, 30f);
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
        {
            cursor.Position = to + (from - to) * Ease.CubeInOut(p);
            yield return null;
        }
        cursor.Visible = false;
        yield return 2f;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void OnEnd(Level level)
    {
        ScreenWipe.WipeColor = Color.Black;
        if (Audio.CurrentMusicEventInstance == null)
        {
            Audio.SetMusic("event:/Ingeste/final_content/music/lvl20/cinematic/end");
        }
        Audio.SetMusicParam("end", 1f);
        frame = 21;
        zoom = 1f;
        fade = 0f;
        fadeColor = Color.Black; // Ensure fade color is black for the transition
        center = Celeste.Celeste.TargetCenter;
        picture.Scale = Vector2.One;
        picture.Visible = true;
        picture.Position = Celeste.Celeste.TargetCenter;
        picture.Origin = Celeste.Celeste.TargetCenter;
        computerFade = 1f;
        attachment.Visible = false;
        cursor.Visible = false;
        ok.Visible = false;
        Audio.Stop(cinIntro);
        cinIntro = null;
        Audio.Stop(endAmbience);
        endAmbience = null;
        List<Coroutine> list = new List<Coroutine>();
        foreach (Coroutine item in base.Components.GetAll<Coroutine>())
        {
            list.Add(item);
        }
        foreach (Coroutine item2 in list)
        {
            item2.RemoveSelf();
        }
        base.Scene.Entities.FindFirst<Textbox>()?.RemoveSelf();
        Level.InCutscene = true;
        Level.PauseLock = true;
        Level.TimerHidden = true;
        Add(new Coroutine(EndingRoutine()));
    }

    private IEnumerator EndingRoutine()
    {
        AreaComplete.InitAreaCompleteInfoForEverest2(pieScreen: false, (Scene as Level)?.Session);
        IEnumerator orig = orig_EndingRoutine();
        while (orig.MoveNext())
        {
            yield return orig.Current;
        }
        AreaComplete.DisposeAreaCompleteInfoForEverest();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Update()
    {
        if (counting)
        {
            timer += Engine.DeltaTime;
        }
        speedrunTimerEase = Calc.Approach(speedrunTimerEase, showTimer ? 1f : 0f, Engine.DeltaTime * 4f);
        base.Update();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Render()
    {
        Draw.Rect(-100f, -100f, 2120f, 1280f, Color.Black);
        if (Atlas != null && Frames != null && frame < Frames.Count)
        {
            MTexture mTexture = Frames[frame];
            MTexture linkedTexture = Atlas.GetLinkedTexture(mTexture.AtlasPath);
            linkedTexture?.DrawJustified(center, new Vector2(center.X / (float)linkedTexture.Width, center.Y / (float)linkedTexture.Height), Color.White, zoom);
            mTexture.DrawJustified(center, new Vector2(center.X / (float)mTexture.Width, center.Y / (float)mTexture.Height), Color.White, zoom);
            if (computerFade > 0f)
            {
                Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black * computerFade);
            }
            base.Render();
            AreaComplete.Info(speedrunTimerEase, speedrunTimerChapterString, speedrunTimerFileString, chapterSpeedrunText, version);
        }
        Draw.Rect(0f, 0f, 1920f, 1080f, fadeColor * fade);
        if ((base.Scene as Level).Paused)
        {
            Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black * 0.5f);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private List<int> GetFrameData(string data)
    {
        List<int> list = new List<int>();
        string[] array = data.Split(new char[1] { ',' });
        for (int i = 0; i < array.Length; i++)
        {
            if (Enumerable.Contains(array[i], '*'))
            {
                string[] array2 = array[i].Split(new char[1] { '*' });
                int item = int.Parse(array2[0]);
                int num = int.Parse(array2[1]);
                for (int j = 0; j < num; j++)
                {
                    list.Add(item);
                }
            }
            else if (Enumerable.Contains(array[i], '-'))
            {
                string[] array3 = array[i].Split(new char[1] { '-' });
                int num2 = int.Parse(array3[0]);
                int num3 = int.Parse(array3[1]);
                for (int k = num2; k <= num3; k++)
                {
                    list.Add(k);
                }
            }
            else
            {
                list.Add(int.Parse(array[i]));
            }
        }
        return list;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IEnumerator Zoom(float from, float to, float duration, Ease.Easer ease = null)
    {
        if (ease == null)
        {
            ease = Ease.Linear;
        }
        zoom = from;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
        {
            zoom = from + (to - from) * ease(p);
            if (picture != null)
            {
                picture.Scale = Vector2.One * zoom;
            }
            yield return null;
        }
        zoom = to;
    }

    private IEnumerator Play(string data)
    {
        return Play(GetFrameData(data));
    }

    private IEnumerator Play(List<int> frames)
    {
        for (int i = 0; i < frames.Count; i++)
        {
            frame = frames[i];
            yield return 1f / 12f;
        }
    }

    private IEnumerator Loop(string data, float duration = -1f)
    {
        List<int> frames = GetFrameData(data);
        float time = 0f;
        while (time < duration || duration < 0f)
        {
            frame = frames[(int)(time / (1f / 12f)) % frames.Count];
            time += Engine.DeltaTime;
            yield return null;
        }
    }

    private IEnumerator Fade(float from, float to, float duration, float delay = 0f)
    {
        fade = from;
        yield return delay;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
        {
            fade = from + (to - from) * p;
            yield return null;
        }
        fade = to;
    }

    private IEnumerator orig_EndingRoutine()
    {
        Level.InCutscene = true;
        Level.PauseLock = true;
        Level.TimerHidden = true;
        yield return 0.5f;
        if (Settings.Instance.SpeedrunClock != SpeedrunType.Off)
        {
            showTimer = true;
        }
        while (!Input.MenuConfirm.Pressed)
        {
            yield return null;
        }
        Audio.Play("event:/Ingeste/ui/postgame/final_input");
        showTimer = false;
        Add(new Coroutine(Zoom(1f, 0.75f, 5f, Ease.CubeIn)));
        Add(new Coroutine(Fade(0f, 1f, 5f)));
        yield return 4f;
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / 3f)
        {
            Audio.SetMusicParam("fade", 1f - p);
            yield return null;
        }
        Audio.SetMusic(null);
        yield return 1f;
        if (Atlas != null)
        {
            Atlas.Dispose();
        }
        Atlas = null;
        Level.CompleteArea(spotlightWipe: false, skipScreenWipe: true, skipCompleteScreen: true);
    }
}
