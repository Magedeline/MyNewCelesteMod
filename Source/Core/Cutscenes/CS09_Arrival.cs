

// CharaDummy is in the main IngesteHelper namespace, not the Cutscenes namespace


using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    // Token: 0x0200016E RID: 366
    public class Cs09Vortex : CutsceneEntity
    {
        // Token: 0x06000D09 RID: 3337 RVA: 0x0002C46C File Offset: 0x0002A66C
        public Cs09Vortex(global::Celeste.Player player, Vector2 target) : base(false, true)
        {
            this.player = player;
            this.target = target;
        }

        // Token: 0x06000D0A RID: 3338 RVA: 0x0002C484 File Offset: 0x0002A684
        public override void OnBegin(Level level)
        {
            Add(new Coroutine(cutscene(level), true));
        }

        // Token: 0x06000D0B RID: 3339 RVA: 0x0002C49F File Offset: 0x0002A69F
        private IEnumerator cutscene(Level level)
        {
            Audio.SetMusic(null, true, true);
            player.StateMachine.State = 11;
            yield return player.DummyWalkTo(target.X, false, 1f, false);
            yield return 0.25f;

            Add(new Coroutine(CameraTo(target + new Vector2(-160f, -130f), 3f, Ease.CubeInOut, 0f), true));
            player.Facing = Facings.Right;
            yield return 1f;

            player.Sprite.Play("idle", false, false);
            player.DummyAutoAnimate = false;
            player.Dashes = 1;
            level.Session.Inventory.Dashes = 1;

            // Split: spawn Badeline
            level.Add(badeline = new BadelineDummy(player.Center));
            player.CreateSplitParticles();
            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            Level.Displacement.AddBurst(player.Center, 0.4f, 8f, 32f, 0.5f, null, null);
            badeline.Sprite.Scale.X = 1f;
            Audio.Play("event:/char/badeline/maddy_split", player.Position);

            // Badeline floats near the target
            yield return badeline.FloatTo(target + new Vector2(-10f, -30f), new int?(1), false, false, false);

            // ALSO spawn Ralsei and Chara, and start followers (Madeline + Theo)
            yield return spawnRalseiAndChara();
            yield return startFollowers();

            yield return 0.5f;

            // Optional dialog sequence that already existed
            yield return Textbox.Say("CH9_VORTEX", new Func<IEnumerator> []
            {
                new Func<IEnumerator>(waitABit),
                new Func<IEnumerator>(sitDown),
                new Func<IEnumerator>(badelineApproaches)
            });

            // Vanish Badeline, Ralsei, Chara; spawn Badeline Booster
            yield return vanishAndSpawnBooster();

            yield return 1f;
            EndCutscene(level, true);
            yield break;
        }

        // Token: 0x06000D0C RID: 3340 RVA: 0x0002C4B5 File Offset: 0x0002A6B5
        private IEnumerator waitABit()
        {
            yield return 3f;
            yield break;
        }

        // Token: 0x06000D0D RID: 3341 RVA: 0x0002C4BD File Offset: 0x0002A6BD
        private IEnumerator sitDown()
        {
            yield return 0.5f;
            player.DummyAutoAnimate = true;
            yield return player.DummyWalkTo(player.X + 16f, false, 0.25f, false);
            yield return 0.1f;
            player.DummyAutoAnimate = false;
            player.Sprite.Play(nameof(sitDown), false, false);
            yield return 1f;
            yield break;
        }

        // Token: 0x06000D0E RID: 3342 RVA: 0x0002C4CC File Offset: 0x0002A6CC
        private IEnumerator badelineApproaches()
        {
            yield return 0.5f;
            badeline.Sprite.Scale.X = -1f;
            yield return 1f;
            Add(new Coroutine(CameraTo(Level.Camera.Position + new Vector2(88f, 0f), 6f, Ease.CubeInOut, 0f), true));
            yield return badeline.FloatTo(new Vector2(player.X - 10f, player.Y - 4f), null, true, false, false);
            yield return 0.5f;
            yield break;
        }

        // New: Spawn Ralsei & Chara and float them near target
        private IEnumerator spawnRalseiAndChara()
        {
            // Ralsei
            ralsei = new RalseiDummy(player.Position);
            Level.Add(ralsei);
            ralsei.Appear(Level, silent: false);
            ralsei.Sprite.Scale.X = 1f;

            // Chara
            chara = new CharaDummy(player.Position);
            Level.Add((IEnumerable<Entity>)chara);
            chara.Added(Level);
            ((Sprite)chara.Sprite).Scale.X = 1f;

            // Float Ralsei to position
            var ralseiFloat = ralsei.FloatTo(target + new Vector2(-30f, -26f), 1, false, false, IngesteConfig.FLOAT_SPEED);
            yield return ralseiFloat;

            // Move Chara to position (since CharaDummy does not have FloatTo, use a simple move coroutine)
            yield return walkEntityTo(chara, target + new Vector2(10f, -26f), IngesteConfig.FLOAT_SPEED);
        }

        // New: Add Madeline + Theo as simple followers �walking� to spots behind the player
        private IEnumerator startFollowers()
        {
            // Madeline dummy follower
            playerPosition = player.Position + new Vector2(-24f, 0f);
            Level.Add();
            madeline = new Entity(player.Position + new Vector2(-24f, 0f)) { Depth = player.Depth + 1 };
            var madelineSprite = GFX.SpriteBank.Create("Madeline");
            madelineSprite.Play("walk");
            madelineSprite.Scale.X = 1f;
            madeline.Add(madelineSprite);
            Level.Add(madeline);

            yield return walkEntityTo(madeline, new Vector2(player.X - 24f, player.Y), 64f);
            madelineSprite.Play("idle");

            // Temporary Theo entity with sprite, also �walks� into place
            theo = new Entity(player.Position + new Vector2(-48f, 0f)) { Depth = player.Depth + 1 };
            var theoSprite = GFX.SpriteBank.Create("Theo");
            theoSprite.Play("walk");
            theoSprite.Scale.X = 1f;
            theo.Add(theoSprite);
            Level.Add(theo);

            yield return walkEntityTo(theo, new Vector2(player.X - 48f, player.Y), 64f);
            theoSprite.Play("idle");
        }

        // New: helper to move an entity to a target with a simple walk effect
        private IEnumerator walkEntityTo(Entity e, Vector2 to, float speed = 64f)
        {
            // Preserve Y style: approach linearly
            while ((e.Position - to).LengthSquared() > 0.25f)
            {
                var dir = to - e.Position;
                var step = Math.Min(speed * Engine.DeltaTime, dir.Length());
                if (step <= 0f) break;
                e.Position += dir.SafeNormalize() * step;

                yield return null;
            }
        }

        // New: vanish the three companions and spawn a Badeline Booster
        private IEnumerator vanishAndSpawnBooster()
        {
            yield return 0.2f;

            // Disappear SFX + particles for Badeline, Ralsei, Chara
            if (badeline != null && badeline.Scene != null)
            {
                Audio.Play("event:/char/badeline/disappear", badeline.Position);
                Level.Displacement.AddBurst(badeline.Position, 0.5f, 24f, 96f, 0.4f);
                badeline.RemoveSelf();
                badeline = null;
            }

            if (ralsei != null && ralsei.Scene != null)
            {
                Audio.Play("event:/char/badeline/disappear", ralsei.Position);
                Level.Displacement.AddBurst(ralsei.Position, 0.5f, 24f, 96f, 0.4f);
                ralsei.RemoveSelf();
                ralsei = null;
            }

            if (chara != null && chara.Scene != null)
            {
                // CharaDummy has a Vanish() helper; if not present, fallback to RemoveSelf with effects
                try
                {
                    chara.Any();
                }
                catch
                {
                    Audio.Play("event:/char/badeline/disappear", chara.Position);
                    Level.Displacement.AddBurst(chara.Position, 0.5f, 24f, 96f, 0.4f);
                    chara.RemoveSelf();
                }
                chara = null;
            }

            // Decide where to place the booster (just in front of target)
            var boostTarget = target + new Vector2(24f, -12f);

            // FX + spawn base BadelineBoost (from vanilla Celeste)
            Level.Displacement.AddBurst(boostTarget, 0.5f, 8f, 32f, 0.5f);
            Audio.Play("event:/new_content/char/badeline/booster_first_appear", boostTarget);

            // Construct the vanilla booster with one node, camera unlocked, no special flags
            // Signature in vanilla is: BadelineBoost(Vector2[] nodes, bool lockCamera, bool canSkip, bool finalCh9Boost, bool finalCh9GoldenBoost, bool finalCh9Dialog)
            Level.Add(new BadelineBoost(
                new Vector2[] { boostTarget },
                false, // lockCamera
                false, // canSkip
                false, // finalCh9Boost
                false, // finalCh9GoldenBoost
                false  // finalCh9Dialog
            ));

            yield return 0.2f;
        }

        // Token: 0x06000D0F RID: 3343 RVA: 0x0002C4DC File Offset: 0x0002A6DC
        public override void OnEnd(Level level)
        {
            Audio.SetMusic(null, true, true);

            // Clean-up temp followers if still present
            if (madeline != null && madeline.Scene != null)
            {
                madeline.RemoveSelf();
                madeline = null;
            }
            if (theo != null && theo.Scene != null)
            {
                theo.RemoveSelf();
                theo = null;
            }
        }

        // Token: 0x04000850 RID: 2128
        private global::Celeste.Player player;

        // Token: 0x04000851 RID: 2129
        private BadelineDummy badeline;

        // New fields
        private RalseiDummy ralsei;
        private CharaDummy chara;
        private Entity madeline;
        private Entity theo;

        // Token: 0x04000852 RID: 2130
        private Vector2 target;
        private Vector2 playerPosition;
    }
}



