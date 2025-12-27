using System.Runtime.CompilerServices;
using Celeste.Mod.Meta;
using FMOD.Studio;

namespace DesoloZantas.Core.Core;

[Tracked(true)]
public class TapeBlockManager : Entity
{
    private int currentIndex;

    private float beatTimer;

    private int beatIndex;

    private float tempoMult;

    private int leadBeats;

    private int maxBeat;

    private bool isLevelMusic;

    private int beatIndexOffset;

    private EventInstance sfx;

    private EventInstance snapshot;

    private int beatsPerTick;

    private int ticksPerSwap;

    private int beatIndexMax;

    public TapeBlockManager()
    {
        base.Tag = Tags.Global;
        Add(new TransitionListener
        {
            OnOutBegin = () =>
            {
                if (!SceneAs<Level>().HasCassetteBlocks)
                {
                    RemoveSelf();
                }
                else
                {
                    maxBeat = SceneAs<Level>().CassetteBlockBeats;
                    tempoMult = SceneAs<Level>().CassetteBlockTempo;
                }
            }
        });
    }
    public override void Awake(Scene scene)
    {
        AreaData areaData = AreaData.Get(scene);
        if (areaData.CassetteSong == "-" || string.IsNullOrWhiteSpace(areaData.CassetteSong))
        {
            areaData.CassetteSong = null;
        }
        orig_Awake(scene);
        beatsPerTick = 4;
        ticksPerSwap = 2;
        beatIndexMax = 256;
        MapMetaCassetteModifier mapMetaCassetteModifier = areaData.Meta?.CassetteModifier;
        if (mapMetaCassetteModifier == null)
        {
            return;
        }
        if (mapMetaCassetteModifier.OldBehavior)
        {
            tempoMult = mapMetaCassetteModifier.TempoMult;
            maxBeat = mapMetaCassetteModifier.Blocks;
        }
        leadBeats = mapMetaCassetteModifier.LeadBeats;
        beatsPerTick = mapMetaCassetteModifier.BeatsPerTick;
        ticksPerSwap = mapMetaCassetteModifier.TicksPerSwap;
        beatIndexMax = mapMetaCassetteModifier.BeatsMax;
        beatIndexOffset = mapMetaCassetteModifier.BeatIndexOffset;
        if (!mapMetaCassetteModifier.ActiveDuringTransitions)
        {
            return;
        }
        TransitionListener transitionListener = Get<TransitionListener>();
        if (transitionListener == null)
        {
            return;
        }
        transitionListener.OnOut = delegate
        {
            if (base.Scene != null)
            {
                Update();
            }
        };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        if (!isLevelMusic)
        {
            Audio.Stop(snapshot);
            Audio.Stop(sfx);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        if (!isLevelMusic)
        {
            Audio.Stop(snapshot);
            Audio.Stop(sfx);
        }
    }

    public override void Update()
    {
        base.Update();
        if (isLevelMusic)
        {
            sfx = Audio.CurrentMusicEventInstance;
        }
        if (sfx == null && !isLevelMusic)
        {
            string cassetteSong = AreaData.Areas[SceneAs<Level>().Session.Area.ID].CassetteSong;
            sfx = Audio.CreateInstance(cassetteSong);
            Audio.Play("event:/game/general/cassette_block_switch_2");
            if (leadBeats == 0)
            {
                beatIndex = 0;
                sfx?.start();
            }
        }
        else
        {
            AdvanceMusic(Engine.DeltaTime * tempoMult);
        }
    }

    public void AdvanceMusic(float time)
    {
        beatTimer += time;
        if (beatTimer < 1f / 6f)
        {
            return;
        }
        beatTimer -= 1f / 6f;
        beatIndex++;
        beatIndex %= beatIndexMax;
        if (beatIndex % (beatsPerTick * ticksPerSwap) == 0)
        {
            currentIndex++;
            currentIndex %= maxBeat;
            SetActiveIndex(currentIndex);
            if (!isLevelMusic)
            {
                Audio.Play("event:/game/general/cassette_block_switch_2");
            }
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        }
        else
        {
            if ((beatIndex + 1) % (beatsPerTick * ticksPerSwap) == 0)
            {
                SetWillActivate((currentIndex + 1) % maxBeat);
            }
            if (beatIndex % beatsPerTick == 0 && !isLevelMusic)
            {
                Audio.Play("event:/game/general/cassette_block_switch_1");
            }
        }
        if (leadBeats > 0)
        {
            leadBeats--;
            if (leadBeats == 0)
            {
                beatIndex = 0;
                if (!isLevelMusic)
                {
                    sfx?.start();
                }
            }
        }
        if (leadBeats <= 0)
        {
            sfx?.setParameterValue("sixteenth_note", GetSixteenthNote());
        }
    }

    public int GetSixteenthNote()
    {
        return (beatIndex + beatIndexOffset) % beatIndexMax + 1;
    }

    public void StopBlocks()
    {
        foreach (CassetteBlock entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
        {
            entity.Finish();
        }
        foreach (CassetteListener component in base.Scene.Tracker.GetComponents<CassetteListener>())
        {
            component.Finish();
        }
        if (!isLevelMusic)
        {
            Audio.Stop(sfx);
        }
    }
    public void Finish()
    {
        if (!isLevelMusic)
        {
            Audio.Stop(snapshot);
        }
        RemoveSelf();
    }

    public void OnLevelStart()
    {
        Level level = base.Scene as Level;
        MapMetaCassetteModifier mapMetaCassetteModifier = AreaData.Get(level.Session).Meta?.CassetteModifier;
        if (mapMetaCassetteModifier != null && mapMetaCassetteModifier.OldBehavior)
        {
            currentIndex = maxBeat - 1 - beatIndex / beatsPerTick % maxBeat;
        }
        else
        {
            maxBeat = level.CassetteBlockBeats;
            tempoMult = level.CassetteBlockTempo;
            if (beatIndex % (beatsPerTick * ticksPerSwap) > beatsPerTick * ticksPerSwap / 2)
            {
                currentIndex = maxBeat - 2;
            }
            else
            {
                currentIndex = maxBeat - 1;
            }
        }
        SilentUpdateBlocks();
    }

    private void SilentUpdateBlocks()
    {
        foreach (CassetteBlock entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
        {
            if (entity.ID.Level == SceneAs<Level>().Session.Level)
            {
                entity.SetActivatedSilently(entity.Index == currentIndex);
            }
        }
        foreach (CassetteListener component in base.Scene.Tracker.GetComponents<CassetteListener>())
        {
            if (component.ID.ID == EntityID.None.ID || component.ID.Level == SceneAs<Level>().Session.Level)
            {
                component.Start(component.Index == currentIndex);
            }
        }
    }

    public void SetActiveIndex(int index)
    {
        foreach (CassetteBlock entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
        {
            entity.Activated = entity.Index == index;
        }
        foreach (CassetteListener component in base.Scene.Tracker.GetComponents<CassetteListener>())
        {
            component.SetActivated(component.Index == index);
        }
    }

    public void SetWillActivate(int index)
    {
        foreach (CassetteBlock entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
        {
            if (entity.Index == index || entity.Activated)
            {
                entity.WillToggle();
            }
        }
        foreach (CassetteListener component in base.Scene.Tracker.GetComponents<CassetteListener>())
        {
            if (component.Index == index || component.Activated)
            {
                component.WillToggle();
            }
        }
    }
    public void orig_Awake(Scene scene)
    {
        base.Awake(scene);
        isLevelMusic = AreaData.Areas[SceneAs<Level>().Session.Area.ID].CassetteSong == null;
        if (isLevelMusic)
        {
            leadBeats = 0;
            beatIndexOffset = 5;
        }
        else
        {
            beatIndexOffset = 0;
            leadBeats = 16;
            snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute");
        }
        maxBeat = SceneAs<Level>().CassetteBlockBeats;
        tempoMult = SceneAs<Level>().CassetteBlockTempo;
    }
}




