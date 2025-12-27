using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    public class Cs02CharaMirror : CutsceneEntity
    {
        private global::Celeste.Player player;
        private NightmareMirror mirror;
        private float playerEndX;
        private int direction = 1;
        private SoundSource sfx;

        public Cs02CharaMirror(global::Celeste.Player v, NightmareMirror v1) {
            player = v;
            mirror = v1;
            playerEndX = mirror.X + direction * 32f;
        }

        public override void OnBegin(Level level)
        {
            this.Add(new Coroutine(this.cutscene(level)));
        }

        private IEnumerator cutscene(Level level)
        {
            this.Add(this.sfx = new SoundSource());
            this.sfx.Position = this.mirror.Center;
            this.sfx.Play("event:/Ingeste/music/lvl2/dreamblock_sting_pt1");
            this.direction = Math.Sign(this.player.X - this.mirror.X);
            this.player.StateMachine.State = 11;
            this.playerEndX = 8 * this.direction;
            yield return 1f;
            this.player.Facing = (global::Celeste.Facings)(-this.direction);
            yield return 0.4f;
            yield return this.player.DummyRunTo(this.mirror.X + this.playerEndX);
            yield return 0.5f;
            yield return level.ZoomTo(this.mirror.Position - level.Camera.Position - Vector2.UnitY * 24f, 2f, 1f);
            yield return 0.5f;
            yield return this.mirror.BreakRoutine(this.direction);
            this.player.DummyAutoAnimate = false;
            if (true)
            {
                ((PlayerSprite)this.player.Sprite).Play(PlayerSprite.Animations.LOOK_UP);
            }
            Vector2 from = level.Camera.Position;
            Vector2 to = level.Camera.Position + new Vector2(0.0f, -80f);
            for (float ease = 0.0f; ease < 1.0f; ease += Engine.DeltaTime * 1.2f)
            {
                level.Camera.Position = from + (to - from) * Ease.CubeInOut(ease);
                yield return null;
            }
            this.Add(new Coroutine(this.zoomBack()));

            // --- Reworked block: Safely get NightmareBlock entities ---
            var tracker = this.Scene?.Tracker;
            List<Entity> blocks = null;
            if (tracker != null)
            {
                try
                {
                    blocks = tracker.GetEntities<NightmareBlock>();
                }
                catch (KeyNotFoundException)
                {
                    blocks = null;
                }
            }
            if (blocks != null)
            {
                foreach (NightmareBlock block in blocks)
                {
                    yield return block.Activate();
                }
            }
            // ---------------------------------------------------------

            // Make chara disappear here if it exists
            DisappearChara();

            yield return 0.5f;
            this.EndCutscene(level);
        }

        public void EndCutscene(Level level)
        {
            level.Session.SetFlag("mirror_shattered");
            base.EndCutscene(level);
        }

        public void DisappearChara()
        {
            var chara = this.mirror?.Chara;
            if (chara != null)
            {
                chara.Vanish();
            }
        }

        private IEnumerator zoomBack()
        {
            yield return 1.2f;
            yield return this.Level.ZoomBack(3f);
        }

        public override void OnEnd(Level level)
        {
            this.mirror.Broken(this.WasSkipped);
            if (this.WasSkipped)
                this.SceneAs<Level>().ParticlesFG.Clear();

            global::Celeste.Player entity1 = this.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            if (entity1 != null)
            {
                entity1.StateMachine.State = 0;
                entity1.DummyAutoAnimate = true;
                entity1.Speed = Vector2.Zero;
                entity1.X = this.mirror.X + this.playerEndX;
                entity1.Facing = this.direction == 0 ? Facings.Right : (global::Celeste.Facings)(-this.direction);
            }

            // --- Safely get NightmareBlock entities ---
            var tracker = this.Scene?.Tracker;
            List<Entity> blocks = null;
            if (tracker != null)
            {
                try
                {
                    blocks = tracker.GetEntities<NightmareBlock>();
                }
                catch (KeyNotFoundException)
                {
                    blocks = null;
                }
            }
            if (blocks != null)
            {
                foreach (var entity in blocks)
                {
                    var entity2 = (NightmareBlock)entity;
                    entity2.ActivateNoRoutine();
                }
            }
            // ------------------------------------------

            level.ResetZoom();
            level.Session.Inventory.DreamDash = true;
            level.Session.Audio.Music.Event = "event:/Ingeste/music/music/lvl2/mirror";
            level.Session.Audio.Apply();
        }
    }
}




