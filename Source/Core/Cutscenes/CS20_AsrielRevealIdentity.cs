using System.Runtime.CompilerServices;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Cutscene for Asriel's identity reveal
    /// Shows Asriel facing away (back sprite), then flipping to face Kirby (idle sprite)
    /// when the dramatic revelation happens
    /// </summary>
    public class CS20_AsrielRevealIdentity : CutsceneEntity
    {
        public const string Flag = "asriel_reveal_identity";

        private global::Celeste.Player player;
        private Entity asrielDummy;
        private Sprite asrielSprite;
        private string roomId;

        public CS20_AsrielRevealIdentity(string roomId = null) : base()
        {
            this.roomId = roomId;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator Cutscene(Level level)
        {
            // Wait for player to be available
            while (player == null)
            {
                player = Scene.Tracker.GetEntity<global::Celeste.Player>();
                yield return null;
            }

            // Lock player in dummy state
            player.StateMachine.State = 11; // Player.StDummy
            player.StateMachine.Locked = true;

            // Wait for player to land if in air
            while (!player.OnGround())
            {
                yield return null;
            }

            // Create Asriel dummy entity if not present
            asrielDummy = level.Entities.FindFirst<AsrielDummy>();
            if (asrielDummy == null)
            {
                // Create a new Asriel entity at a position relative to player
                asrielDummy = new Entity(player.Position + new Vector2(100f, 0f));
                asrielSprite = GFX.SpriteBank.Create("asriel");
                asrielDummy.Add(asrielSprite);
                level.Add(asrielDummy);
            }
            else
            {
                asrielSprite = asrielDummy.Get<Sprite>();
            }

            // Start with Asriel facing away (back animation)
            if (asrielSprite != null && asrielSprite.Has("back"))
            {
                asrielSprite.Play("back");
            }

            // Brief pause for tension
            yield return 0.5f;

            // Zoom in on the confrontation
            yield return level.ZoomTo(new Vector2(160f, 90f), 2f, 0.5f);

            // Small pause
            yield return 0.3f;

            // Main dialogue - Kirby questions who this is
            // Dialog uses {trigger 3} for the reveal and {trigger 4} for the internal struggle
            yield return Textbox.Say("ch20_asriel_reveal_identity", new Func<IEnumerator>[] {
                // Trigger 0: Not used in this dialog, placeholder
                NoOperation,
                // Trigger 1: Not used in this dialog, placeholder
                NoOperation,
                // Trigger 2: Not used in this dialog, placeholder
                NoOperation,
                // Trigger 3: The dark shell completely breaks, revealing Asriel
                // Asriel turns to face Kirby (flip from back to idle)
                FlipAsrielToFaceKirby,
                // Trigger 4: Asriel's form shifts between his true self and Els
                AsrielInternalStruggle
            });

            // Small pause after dialogue
            yield return 0.3f;

            // Zoom back out
            yield return level.ZoomBack(0.5f);

            // Screen shake for dramatic effect
            level.Shake(0.3f);

            yield return 0.5f;

            // End cutscene
            EndCutscene(level);
        }

        /// <summary>
        /// Coroutine triggered during dialog to flip Asriel from back sprite to idle (facing Kirby)
        /// This represents the moment the dark shell breaks and reveals Asriel
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator FlipAsrielToFaceKirby()
        {
            if (asrielSprite == null)
                yield break;

            // Screen flash for dramatic reveal
            Level level = Scene as Level;
            if (level != null)
            {
                level.Flash(Color.White * 0.6f, false);
                level.Shake(0.5f);
                
                // Displacement burst for transformation effect
                level.Displacement.AddBurst(
                    asrielDummy.Position, 
                    1.0f, 
                    64f, 
                    128f, 
                    1.5f
                );
            }

            // Small pause for flash effect
            yield return 0.2f;

            // Flip to idle animation (facing the player)
            if (asrielSprite.Has("idle"))
            {
                asrielSprite.Play("idle");
            }

            // Ensure Asriel is facing Kirby (flip sprite if needed)
            if (player != null)
            {
                float direction = Math.Sign(player.X - asrielDummy.X);
                asrielSprite.Scale.X = direction != 0 ? direction : -1f;
            }

            // Brief pause for the reveal to sink in
            yield return 0.3f;
        }

        /// <summary>
        /// Placeholder coroutine for unused triggers
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator NoOperation()
        {
            yield break;
        }

        /// <summary>
        /// Coroutine triggered during dialog when Asriel's form shifts between his true self and Els
        /// Shows the internal struggle visually
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator AsrielInternalStruggle()
        {
            if (asrielSprite == null)
                yield break;

            Level level = Scene as Level;
            
            // Visual effect for the internal struggle - flickering between states
            for (int i = 0; i < 3; i++)
            {
                // Shake effect for turmoil
                if (level != null)
                {
                    level.Shake(0.2f);
                }

                // Switch to fighting_internal animation if available
                if (asrielSprite.Has("fighting_internal"))
                {
                    asrielSprite.Play("fighting_internal");
                }
                
                yield return 0.3f;

                // Brief flash
                if (level != null)
                {
                    level.Flash(Color.Red * 0.3f, false);
                }

                yield return 0.2f;
            }

            // Return to conflicted state
            if (asrielSprite.Has("conflicted"))
            {
                asrielSprite.Play("conflicted");
            }
            else if (asrielSprite.Has("idle"))
            {
                asrielSprite.Play("idle");
            }

            yield return 0.2f;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void OnEnd(Level level)
        {
            // Handle skipped cutscene - make sure player is on ground
            if (WasSkipped && player != null)
            {
                while (!player.OnGround() && player.Y < (float)level.Bounds.Bottom)
                {
                    player.Y++;
                }
            }

            // Restore player control
            if (player != null)
            {
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0; // Player.StNormal
            }

            // Make sure Asriel is facing the right direction if cutscene was skipped
            if (WasSkipped && asrielSprite != null)
            {
                if (asrielSprite.Has("idle"))
                {
                    asrielSprite.Play("idle");
                }
                if (player != null && asrielDummy != null)
                {
                    float direction = Math.Sign(player.X - asrielDummy.X);
                    asrielSprite.Scale.X = direction != 0 ? direction : -1f;
                }
            }

            // Set flag to prevent re-triggering (use room-specific flag if roomId provided)
            string flagToSet = !string.IsNullOrEmpty(roomId) ? $"{Flag}_{roomId}" : Flag;
            level.Session.SetFlag(flagToSet);
        }
    }
}
