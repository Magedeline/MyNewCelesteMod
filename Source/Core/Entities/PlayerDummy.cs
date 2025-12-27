using DesoloZantas.Core.Core.Content;

#pragma warning disable CS0436 // Type conflicts with imported type - intentional override
namespace DesoloZantas.Core.Core.Entities {
    [Tracked(false)]
    public class PlayerDummy : BadelineDummy {

        public int Dashes = 1;
        public string CharacterId { get; private set; }
        public CharacterAbilityRegistry.CharacterData? CharacterData { get; private set; }
        
        public PlayerDummy(CelestePlayer player, string spriteName, string id, string frame, string characterId = null)
            : base(player.Position) {

            Tag = Tags.Persistent;
            CharacterId = characterId ?? "default";

            Wave.Active = AutoAnimator.Enabled = false;
            Hair.Color = player.Hair.Color;

            Depth = player.Depth - 1;
            Dashes = player.Dashes;

            // Get character-specific sprite or default
            string _spriteName = GetCharacterSprite(characterId) ?? spriteName ?? "player";
            
            try 
            {
                GFX.SpriteBank.CreateOn(Sprite, _spriteName);
                
                // Handle special sprite modes
                if (_spriteName == "player_playback") {
                    // Set playback mode using reflection if needed
                    try {
                        var modeProperty = Sprite.GetType().GetProperty("Mode");
                        if (modeProperty != null && modeProperty.CanWrite) {
                            modeProperty.SetValue(Sprite, Celeste.PlayerSpriteMode.Playback);
                        }
                    } catch (Exception ex) {
                        Logger.Log(LogLevel.Warn, "PlayerDummy", $"Failed to set sprite mode: {ex.Message}");
                    }
                }
                
                Sprite.Scale = player.Sprite.Scale;
                Sprite.Scale.X *= (float)player.Facing;

                Sprite.Play(id ?? player.Sprite.LastAnimationID ?? "idle");
                Sprite.SetAnimationFrame(int.TryParse(frame, out int f) ? f : player.Sprite.CurrentAnimationFrame);

                // Set animation state using reflection
                try {
                    var animatingProperty = Sprite.GetType().GetProperty("Animating");
                    if (animatingProperty != null && animatingProperty.CanWrite) {
                        animatingProperty.SetValue(Sprite, false);
                    }
                } catch (Exception ex) {
                    Logger.Log(LogLevel.Warn, "PlayerDummy", $"Failed to set animating state: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "PlayerDummy", $"Failed to create sprite {_spriteName}: {ex.Message}");
                // Fallback to default player sprite
                GFX.SpriteBank.CreateOn(Sprite, "player");
            }
            
            // Load character data if specified
            if (!string.IsNullOrEmpty(characterId))
            {
                CharacterData = CharacterAbilityRegistry.GetCharacter(characterId);
            }
        }

        private static string GetCharacterSprite(string characterId)
        {
            if (string.IsNullOrEmpty(characterId)) return null;
            
            var character = CharacterAbilityRegistry.GetCharacter(characterId);
            return character?.SpriteId;
        }

        public override void Update()
        {
            base.Update();
            
            // Update character-specific behavior if needed
            if (CharacterData.HasValue)
            {
                UpdateCharacterBehavior();
            }
        }
        
        private static void UpdateCharacterBehavior()
        {
            // Character-specific dummy behavior can be added here
            // For example, different idle animations, particle effects, etc.
        }

        public static void RemoveAllDummy() {
            foreach (Entity e in Engine.Scene.Tracker.GetEntities<PlayerDummy>()) {
                e.RemoveSelf();
            }
        }
        
        /// <summary>
        /// Create a character-specific dummy
        /// </summary>
        public static PlayerDummy CreateCharacterDummy(CelestePlayer player, string characterId, string animationId = null, string frame = null)
        {
            var characterData = CharacterAbilityRegistry.GetCharacter(characterId);
            string spriteId = characterData?.SpriteId ?? "player";
            
            return new PlayerDummy(player, spriteId, animationId, frame, characterId);
        }
    }
}
#pragma warning restore CS0436



