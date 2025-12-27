using DesoloZantas.Core.Core.Cutscenes;

namespace DesoloZantas.Core.Core.NPCs
{
    [CustomEntity("Ingeste/NPC00_Theo")]
    public class Npc00Theo : Entity
    {
        // Synchronized with CS00_Theo cutscene flag
        private const string FLAG = "theo_00_house";

        private Sprite sprite;
        public Sprite TheoSprite => sprite; // For cutscene compatibility
        private TalkComponent talker;
        private Coroutine talkRoutine;
        private bool isInteracting = false;

        public Npc00Theo(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            setupSprite();
            setupCollision();
            Depth = 100;
        }

        private void setupSprite()
        {
            Add(sprite = GFX.SpriteBank.Create("theo"));
            sprite.Play("idle");
        }

        private void setupCollision()
        {
            Add(talker = new TalkComponent(
                new Rectangle(-20, -8, 40, 16),
                new Vector2(0f, -24f),
                ontalk
            ));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (Scene is Level level)
            {
                // Check if the cutscene has been completed
                if (level.Session.GetFlag(FLAG))
                {
                    talker.Enabled = false;
                    return;
                }
            }

            talker.Enabled = true;
        }

        private void ontalk(global::Celeste.Player player)
        {
            if (isInteracting) return;

            if (Scene is Level level)
            {
                isInteracting = true;
                
                // Start the full cutscene instead of simple dialog
                level.StartCutscene(ontalkend);
                
                // Create and start the CS00_Theo cutscene
                var cutscene = new Cs00Theo(player);
                level.Add(cutscene);
            }
        }

        private void ontalkend(Level level)
        {
            isInteracting = false;
            
            // The cutscene sets the flag, so we check it here
            if (level.Session.GetFlag(FLAG))
            {
                talker.Enabled = false;
            }

            talkRoutine?.RemoveSelf();
            talkRoutine = null;

            // Player state restoration is handled by the cutscene
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && player.StateMachine.State == global::Celeste.Player.StDummy)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }
        }

        public override void Update()
        {
            base.Update();

            if (sprite != null && !isInteracting)
            {
                sprite.Play("idle");
            }
        }

        public override void Removed(Scene scene)
        {
            talkRoutine?.RemoveSelf();
            base.Removed(scene);
        }
    }
}




