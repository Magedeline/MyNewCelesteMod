namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Factory class to create DesoloZantas entities that replace vanilla entities.
    /// </summary>
    public static class EntityFactory
    {
        /// <summary>
        /// Create an NPC using the character mapping system
        /// </summary>
        public static NPC CreateNPC(
            Vector2 position, 
            CharacterMapping.VanillaCharacter vanillaChar,
            string levelId = null)
        {
            var modChar = string.IsNullOrEmpty(levelId)
                ? CharacterMapping.GetModCharacter(vanillaChar)
                : CharacterMapping.GetCharacterForLevel(levelId, vanillaChar);

            return CreateModCharacterNPC(position, modChar);
        }

        /// <summary>
        /// Create an NPC from a specific mod character
        /// </summary>
        public static NPC CreateModCharacterNPC(Vector2 position, CharacterMapping.ModCharacter character)
        {
            var npc = new NPC(position)
            {
                Maxspeed = CharacterMapping.GetMaxSpeed(character)
            };

            string spriteId = CharacterMapping.GetSpriteBankId(character);
            
            // Try to create sprite from sprite bank
            try
            {
                npc.Sprite = GFX.SpriteBank.Create(spriteId);
                npc.Add(npc.Sprite);
            }
            catch
            {
                // Fallback to default sprite if custom one doesn't exist
                npc.Sprite = GFX.SpriteBank.Create("player");
                npc.Add(npc.Sprite);
            }

            // Setup character-specific sounds
            SetupCharacterSounds(npc, character);

            return npc;
        }

        private static void SetupCharacterSounds(NPC npc, CharacterMapping.ModCharacter character)
        {
            switch (character)
            {
                case CharacterMapping.ModCharacter.Kirby:
                    npc.SetupKirbySpriteSounds();
                    break;
                case CharacterMapping.ModCharacter.Magolor:
                    npc.SetupMagolorSpriteSounds();
                    break;
                case CharacterMapping.ModCharacter.Toriel:
                    npc.SetupTorielSpriteSounds();
                    break;
                case CharacterMapping.ModCharacter.Ralsei:
                    npc.SetupRalseiSpriteSounds();
                    break;
                case CharacterMapping.ModCharacter.Theo:
                    npc.SetupTheoSpriteSounds();
                    break;
                case CharacterMapping.ModCharacter.Granny:
                    npc.SetupGrannySpriteSounds();
                    break;
            }
        }

        /// <summary>
        /// Create a cutscene entity for a specific character interaction
        /// </summary>
        public static CutsceneEntity CreateCharacterCutscene(
            CharacterMapping.ModCharacter character,
            string cutsceneId,
            Func<Level, IEnumerator> cutsceneRoutine)
        {
            return new GenericCutscene(cutsceneId, cutsceneRoutine);
        }

        /// <summary>
        /// Create a trigger that works with DesoloZantas characters
        /// </summary>
        public static Trigger CreateEventTrigger(
            EntityData data,
            Vector2 offset,
            string eventId,
            Action<Level, global::Celeste.Player> onTrigger)
        {
            return new GenericEventTrigger(data, offset, eventId, onTrigger);
        }
    }

    /// <summary>
    /// Generic cutscene implementation for easy cutscene creation
    /// </summary>
    internal class GenericCutscene : CutsceneEntity
    {
        private string _cutsceneId;
        private Func<Level, IEnumerator> _cutsceneRoutine;
        private Coroutine _coroutine;

        public GenericCutscene(string cutsceneId, Func<Level, IEnumerator> routine) : base()
        {
            _cutsceneId = cutsceneId;
            _cutsceneRoutine = routine;
        }

        public override void OnBegin(Level level)
        {
            _coroutine = new Coroutine(_cutsceneRoutine(level));
            Add(_coroutine);
        }

        public override void OnEnd(Level level)
        {
            level.Session.SetFlag($"cs_{_cutsceneId}_complete", true);
        }

        public override void Update()
        {
            base.Update();
            if (_coroutine != null && _coroutine.Finished)
            {
                EndCutscene(Level);
            }
        }
    }

    /// <summary>
    /// Generic event trigger for easy trigger creation
    /// </summary>
    // Note: Removed [CustomEntity] - this class should only be instantiated via factory method
    // since it requires callback parameters that can't be provided by EntityData
    internal class GenericEventTrigger : Trigger
    {
        private string _eventId;
        private Action<Level, global::Celeste.Player> _onTrigger;
        private bool _triggered;
        private bool _onlyOnce;

        public GenericEventTrigger(
            EntityData data, 
            Vector2 offset,
            string eventId,
            Action<Level, global::Celeste.Player> onTrigger) : base(data, offset)
        {
            _eventId = eventId;
            _onTrigger = onTrigger;
            _onlyOnce = data.Bool("onlyOnce", true);
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);
            
            if (_triggered && _onlyOnce)
                return;

            if (!ShouldActivate(player))
                return;

            _triggered = true;
            Level level = Scene as Level;
            _onTrigger?.Invoke(level, player);
        }
    }
}
