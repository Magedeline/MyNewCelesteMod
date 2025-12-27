using DesoloZantas.Core.Core.Entities;
using DesoloZantas.Core.Core.NPCs;
using FMOD.Studio;

namespace DesoloZantas.Core.Core.Cutscenes
{
    public class Cs19TrapinLoop : CutsceneEntity
    {
        // Existing fields
        private readonly global::Celeste.Player player;
        private readonly CharaDummy chara;
        private BirdNPC bird;
        private EventInstance snapshot;
        private Vector2 playerSpeed;

        // Additional entities for the cutscene
        private CharaDummy charaEntity;
        private CustomCharaBoost charabooster;
        private global::Celeste.Player kirbyEntity;
        private Npc19MaggyLoop magolorEntity;

        // Constructor implementation
        public Cs19TrapinLoop(global::Celeste.Player sourceTrapinLoop, CharaDummy charaDummy)
        {
            // Initialize the player with the provided Player object
            player = sourceTrapinLoop;
            // Initialize other fields where necessary
            chara = charaDummy; // Assign the provided CharaDummy instance
            bird = null; // BirdNPC may also appear depending on the logic inside cutscene
            snapshot = null; // May need initialization later if FMOD Events are used

            // Set up other properties as required for CutsceneEntity
            Tag = Tags.TransitionUpdate; // Ensure it updates during scene transitions
            playerSpeed = player.Speed;
        }

        // Replace coroutine override with a void override that starts the coroutine
        public override void OnBegin(Level level)
        {
            this.player.StateMachine.Locked = true;
            Add(new Coroutine(CutsceneRoutine(level)));
        }

        // extracted routine from the old IEnumerator OnBegin
        private IEnumerator CutsceneRoutine(Level level)
        {
            var boost = Scene.Entities.FindFirst<CustomCharaBoost>();
            if (boost != null)
                boost.Active = boost.Visible = boost.Collidable = false;

            yield return MovePlayerToGround();

            Engine.TimeRate = 0.65f;
            player.Dashes = 1;
            player.DummyGravity = false;
            player.DummyFriction = false;
            player.Speed.X = 200f;
            playerSpeed.X = 200f;
            player.DummyAutoAnimate = false;

            yield return MovePlayerAndBird();

            // Restore normal player physics and continue the cutscene logic
            playerSpeed.X = 0.0f;
            player.Speed.X = 0.0f;
            player.DummyAutoAnimate = true;
            player.DummyGravity = true;
            yield return 0.25f;

            yield return RestoreTimeRate();

            player.ForceCameraUpdate = false;
            yield return 0.6f;
            yield return 0.8f;

            yield return Textbox.Say(
                "CH19_TRAP_IN_LOOP",
                CameraZoomin,
                CharaAppear,
                CharaTurnLeft,
                KirbyTurnLeft,
                MagolorWalkin,
                KirbyandCharaTurnRight,
                ShiftCameraToPowergen
            );

            yield return level.ZoomBack(0.5f);

            // Remove Chara and Magolor before charabooster appears
            if (charaEntity != null)
            {
                Level.Displacement.AddBurst(charaEntity.Position, 0.5f, 8f, 32f, 0.5f);
                Audio.Play("event:/char/badeline/disappear", charaEntity.Position);
                charaEntity.RemoveSelf();
            }
            
            if (magolorEntity != null)
            {
                magolorEntity.RemoveSelf();
            }

            yield return 0.3f;

            // Make charabooster appear
            if (boost != null)
            {
                Level.Displacement.AddBurst(boost.Center, 0.5f, 8f, 32f, 0.5f);
                Audio.Play("event:/new_content/char/badeline/booster_first_appear", boost.Center);
                boost.Active = boost.Visible = boost.Collidable = true;
            }

            yield return 0.3f;

            // Unlock player movement
            player.StateMachine.Locked = false;

            EndCutscene(level);
        }

        private IEnumerator MovePlayerToGround()
        {
            while (!player.OnGround())
                player.MoveVExact(1);
            while (player.CollideCheck<Solid>())
                player.MoveVExact(-1);
            yield return null;
        }

        private IEnumerator MovePlayerAndBird()
        {
            for (var p = 0.0f; p < 1.0; p += Engine.DeltaTime)
            {
                playerSpeed.X = Calc.Approach(playerSpeed.X, 0.0f, 160f * Engine.DeltaTime);
                player.Speed.X = playerSpeed.X;
                if (playerSpeed.X != 0.0 && Scene.OnInterval(0.1f))
                    Dust.BurstFG(player.Position, -1.57079637f, 2);
                yield return null;
            }
        }

        private IEnumerator RestoreTimeRate()
        {
            while (Engine.TimeRate < 1.0)
            {
                Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 4f * Engine.DeltaTime);
                yield return null;
            }
        }

        // Trigger -1: Camera zoom in
        private IEnumerator CameraZoomin()
        {
            // Zoom to position between player and where Chara will appear
            // Convert world position to screen-space coordinates
            Vector2 focusPoint = (player.Position + new Vector2(-16f, -8f)) - Level.Camera.Position;
            yield return Level.ZoomTo(focusPoint, 2f, 0.5f);
        }

        // Trigger 0: Chara appears
        private IEnumerator CharaAppear()
        {
            Vector2 position = player.Position + new Vector2(-32f, 0f);
            Level.Displacement.AddBurst(position, 0.5f, 8f, 32f, 0.5f);
            Level.Add(charaEntity = new CharaDummy(position));
            Audio.Play("event:/char/badeline/maddy_split", position);
            charaEntity.Sprite.Scale.X = 1f;
            yield return 0.2f;
        }

        // Trigger 1: Chara turns left
        private IEnumerator CharaTurnLeft()
        {
            if (charaEntity != null)
            {
                charaEntity.Sprite.Scale.X = -1f;
            }
            yield return null;
        }

        // Trigger 2: Kirby turns left
        private IEnumerator KirbyTurnLeft()
        {
            // Find Kirby NPC if exists or create placeholder
            kirbyEntity = Scene.Entities.FindFirst<global::Celeste.Player>();
            if (kirbyEntity != null)
            {
                kirbyEntity.Sprite.Scale.X = -1f;
            }
            yield return null;
        }

        // Trigger 3: Magolor walks in
        private IEnumerator MagolorWalkin()
        {
            Vector2 MagolorStartPos = player.Position + new Vector2(-120f, 0f);
            Vector2 MagolorEndPos = player.Position + new Vector2(-64f, 0f);
            
            // Create Magolor NPC with EntityData
            var entityData = new EntityData();
            entityData.Position = MagolorStartPos;
            magolorEntity = new Npc19MaggyLoop(entityData, Vector2.Zero);
            Level.Add(magolorEntity);
            
            // Animate Magolor walking to end position
            float duration = 1.5f;
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                magolorEntity.Position = Vector2.Lerp(MagolorStartPos, MagolorEndPos, t / duration);
                yield return null;
            }
            magolorEntity.Position = MagolorEndPos;
            yield return 0.2f;
        }

        // Trigger 4: Kirby and Chara turn right
        private IEnumerator KirbyandCharaTurnRight()
        {
            if (kirbyEntity != null)
            {
                kirbyEntity.Sprite.Scale.X = 1f;
            }
            if (charaEntity != null)
            {
                charaEntity.Sprite.Scale.X = 1f;
            }
            yield return null;
        }

        // Trigger 5: Shift camera to power generator
        private IEnumerator ShiftCameraToPowergen()
        {
            // Pan camera to show the power generator/scene element
            Vector2 targetPos = player.Position + new Vector2(150f, -20f);
            
            // Pan camera to the target
            yield return Level.ZoomTo(targetPos, 2f, 1.2f);
            yield return 1.2f;
        }

        // Other methods remain unchanged...

        public override void OnEnd(Level level)
        {
            Add(new Coroutine(OnEndRoutine(level)));
        }

        private IEnumerator OnEndRoutine(Level level)
        {
            // Player is already unlocked in CutsceneRoutine, so no need to unlock again
            Audio.ReleaseSnapshot(snapshot);
            snapshot = null;
            if (WasSkipped)
            {
                var eventTrigger = Scene.Entities.FindFirst<EventTrigger>();
                if (eventTrigger != null)
                {
                    // use pattern matching instead of 'as' + null check
                    if (player.Sprite is global::Celeste.PlayerSprite playerSprite)
                        playerSprite.Play(global::Celeste.PlayerSprite.Idle);
                    player.Position = eventTrigger.Position.Floor();
                    level.Camera.Position = player.BottomCenter;
                }
                foreach (var lightning in Scene.Entities.FindAll<Lightning>())
                    lightning.ToggleCheck();
                Scene.Tracker.GetEntity<LightningRenderer>()?.ToggleEdges(true);
                level.Session.Audio.Ambience.Event = "event:/Ingeste/final_content/env/19_vortex";
                level.Session.Audio.Apply();
            }
            if (chara != null)
            {
                chara.Vanish();
            }
            yield return 0.5f;
            if (charabooster != null)
            {
                Level.Displacement.AddBurst(charabooster.Center, 0.5f, 8f, 32f, 0.5f);
                Audio.Play("event:/new_content/char/badeline/booster_first_appear", charabooster.Center);
                bool visible = true;
                charabooster.Collidable = true;
                charabooster.Active = (charabooster.Visible = visible);
                yield return 0.2f;
            }
            playerSpeed = Vector2.Zero;
            player.DummyGravity = true;
            player.DummyFriction = true;
            player.DummyAutoAnimate = true;
            player.ForceCameraUpdate = false;
            player.StateMachine.State = 0;
            var first = Scene.Entities.FindFirst<CustomCharaBoost>();
            if (first != null)
                first.Active = first.Visible = first.Collidable = true;
            chara?.RemoveSelf();
            Vector2 birdWaitPosition = default;
            if (WasSkipped)
            {
                bird?.RemoveSelf();
                Scene.Add(bird = new BirdNPC(birdWaitPosition, BirdNPC.Modes.WaitForLightningOff));
                bird.Facing = Facings.Right;
                bird.FlyAwayUp = false;
                bird.WaitForLightningPostDelay = 1f;
                level.SnapColorGrade("none");
            }
            level.ResetZoom();
        }
    }
}



