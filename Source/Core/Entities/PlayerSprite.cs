namespace DesoloZantas.Core.Core.Entities {
    public enum PlayerSpriteCharacter {
        Madeline,
        Badeline,
        Chara,
        Theo,
        Magolor,
        Dummy
    }

    public class PlayerSprite : Sprite {
        private PlayerSpriteMode spriteMode;

        public PlayerSpriteCharacter CharacterType { get; private set; }

        public PlayerSprite(PlayerSpriteCharacter characterType)
            : base(null, null) // Base initialization
        {
            CharacterType = characterType;
            // Load the correct sprite/animation set based on characterType
            switch (characterType) {
                case PlayerSpriteCharacter.Madeline:
                    // Map to the equivalent PlayerSpriteMode
                    InitializeFromSpriteMode(PlayerSpriteMode.Madeline);
                    break;
                case PlayerSpriteCharacter.Badeline:
                    InitializeFromSpriteMode(PlayerSpriteMode.Badeline);
                    break;
                case PlayerSpriteCharacter.Chara:
                    InitializeFromSpriteMode(PlayerSpriteMode.Chara);
                    break;
                case PlayerSpriteCharacter.Theo:
                    // For characters without direct PlayerSpriteMode equivalents
                    // Load specific sprite assets
                    GFX.SpriteBank.CreateOn(this, "theo");
                    break;
                case PlayerSpriteCharacter.Magolor:
                    GFX.SpriteBank.CreateOn(this, "magolor");
                    break;
                case PlayerSpriteCharacter.Dummy:
                    // Default dummy sprite
                    GFX.SpriteBank.CreateOn(this, "dummy");
                    break;
            }
        }

        public PlayerSprite(PlayerSpriteMode spriteMode)
            : base(null, null) // Base initialization
        {
            InitializeFromSpriteMode(spriteMode);

            // Map PlayerSpriteMode to appropriate PlayerSpriteCharacter
            switch (spriteMode) {
                case PlayerSpriteMode.Madeline:
                case PlayerSpriteMode.Madelinealt:
                case PlayerSpriteMode.MadelineNoBackpack:
                    CharacterType = PlayerSpriteCharacter.Madeline;
                    break;
                case PlayerSpriteMode.Badeline:
                case PlayerSpriteMode.MadelineAsBadeline:
                    CharacterType = PlayerSpriteCharacter.Badeline;
                    break;
                case PlayerSpriteMode.Chara:
                    CharacterType = PlayerSpriteCharacter.Chara;
                    break;
                default:
                    CharacterType = PlayerSpriteCharacter.Madeline; // Default
                    break;
            }
        }

        private void InitializeFromSpriteMode(PlayerSpriteMode mode) {
            this.spriteMode = mode;
            var modeToIdMap = new Dictionary<PlayerSpriteMode, string> {
                { PlayerSpriteMode.Chara, "chara" },
                { PlayerSpriteMode.Kirby, "kirby" },
                { PlayerSpriteMode.Ralsei, "ralsei" },
                { PlayerSpriteMode.Madeline, "player" },
                { PlayerSpriteMode.Madelinealt, "madeline" },
                { PlayerSpriteMode.MadelineNoBackpack, "player_no_backpack" },
                { PlayerSpriteMode.Badeline, "badeline" },
                { PlayerSpriteMode.MadelineAsBadeline, "player_badeline" },
                { PlayerSpriteMode.Playback, "player_playback" },
                { PlayerSpriteMode.Default, "player" } // Default to player if not specified
            };

            if (modeToIdMap.TryGetValue(mode, out string spriteName)) {
                GFX.SpriteBank.CreateOn(this, spriteName);
            } else {
                // Fallback to default player sprite
                GFX.SpriteBank.CreateOn(this, "player");
            }
        }

        private PlayerSprite()
            : base(null, null) // Base initialization with null, replaced by proper sprite creation
        {
            // Default to Madeline if no character type is specified
            CharacterType = PlayerSpriteCharacter.Madeline;
            InitializeFromSpriteMode(PlayerSpriteMode.Madeline);
        }

        public static PlayerSprite Create(PlayerSpriteCharacter characterType) {
            return new PlayerSprite(characterType);
        }

        // Optionally, create a list of sprites for all characters/dummies
        public static List<PlayerSprite> CreateAllCharactersAndDummies() {
            var sprites = new List<PlayerSprite>();
            foreach (PlayerSpriteCharacter type in Enum.GetValues(typeof(PlayerSpriteCharacter))) {
                sprites.Add(new PlayerSprite(type));
            }
            return sprites;
        }
                    
    }
}



