namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Friendly NPCs in Kirby style - can interact with player, give items, or provide dialog
    /// Examples: Bandana Waddle Dee, King Dedede (friendly), Meta Knight (friendly), Magolor, etc.
    /// </summary>
    [CustomEntity("Ingeste/KirbyNPC")]
    [Tracked]
    public class KirbyNPC : KirbyActorBase
    {
        /// <summary>
        /// NPC character types
        /// </summary>
        public enum NPCCharacter
        {
            BandanaWaddleDee,
            KingDedede,
            MetaKnight,
            Magolor,
            Taranza,
            Susie,
            Elfilin,
            Adeleine,
            Ribbon,
            Gooey,
            Marx,
            DarkMetaKnight,
            Custom
        }

        /// <summary>
        /// NPC behavior types
        /// </summary>
        public enum NPCBehavior
        {
            Stationary,     // Stays in place
            Wander,         // Walks around randomly
            Follow,         // Follows player
            Patrol,         // Patrols between points
            Shop            // Shop keeper
        }

        public NPCCharacter Character { get; private set; }
        public NPCBehavior Behavior { get; private set; }
        
        // Dialog and interaction
        public string DialogId { get; set; }
        public bool HasInteracted { get; private set; }
        public bool CanGiveItem { get; set; }
        public string GiveItemId { get; set; }
        
        // Shop properties
        public bool IsShopKeeper => Behavior == NPCBehavior.Shop;
        public List<ShopItem> ShopInventory { get; private set; }
        
        // Movement
        private float wanderTimer;
        private Vector2 wanderTarget;
        private float followDistance = 48f;
        private List<Vector2> patrolNodes;
        private int currentPatrolNode;
        
        // Components
        private TalkComponent talkComponent;
        private Wiggler interactWiggler;

        public KirbyNPC(Vector2 position, NPCCharacter character) : base(position)
        {
            Character = character;
            IsFriendly = true;
            CanBeInhaled = false;
            ShopInventory = new List<ShopItem>();
            patrolNodes = new List<Vector2>();
            
            SetupCharacter();
        }

        public KirbyNPC(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            Character = (NPCCharacter)data.Int("character", 0);
            Behavior = (NPCBehavior)data.Int("behavior", 0);
            DialogId = data.Attr("dialogId", "");
            CanGiveItem = data.Bool("canGiveItem", false);
            GiveItemId = data.Attr("giveItemId", "");
            followDistance = data.Float("followDistance", 48f);
            
            IsFriendly = true;
            CanBeInhaled = false;
            ShopInventory = new List<ShopItem>();
            patrolNodes = new List<Vector2>();
            
            // Parse patrol nodes if any
            var nodes = data.NodesOffset(offset);
            foreach (var node in nodes)
            {
                patrolNodes.Add(node);
            }
            
            SetupCharacter();
        }

        private void SetupCharacter()
        {
            // Set character-specific properties
            switch (Character)
            {
                case NPCCharacter.BandanaWaddleDee:
                    MoveSpeed = 40f;
                    light.Color = Color.Blue;
                    break;

                case NPCCharacter.KingDedede:
                    MoveSpeed = 30f;
                    Collider = new Hitbox(32f, 40f, -16f, -40f);
                    light.Color = Color.Red;
                    break;

                case NPCCharacter.MetaKnight:
                    MoveSpeed = 50f;
                    light.Color = Color.Gold;
                    break;

                case NPCCharacter.Magolor:
                    MoveSpeed = 45f;
                    light.Color = Color.Purple;
                    break;

                case NPCCharacter.Taranza:
                    MoveSpeed = 35f;
                    light.Color = Color.Pink;
                    break;

                case NPCCharacter.Susie:
                    MoveSpeed = 40f;
                    light.Color = Color.HotPink;
                    break;

                case NPCCharacter.Elfilin:
                    MoveSpeed = 50f;
                    light.Color = Color.LightBlue;
                    break;

                case NPCCharacter.Adeleine:
                    MoveSpeed = 35f;
                    light.Color = Color.Orange;
                    break;

                case NPCCharacter.Ribbon:
                    MoveSpeed = 60f;
                    light.Color = Color.Pink;
                    break;

                case NPCCharacter.Gooey:
                    MoveSpeed = 30f;
                    light.Color = Color.Blue;
                    break;

                case NPCCharacter.Marx:
                    MoveSpeed = 45f;
                    light.Color = Color.Purple;
                    break;

                case NPCCharacter.DarkMetaKnight:
                    MoveSpeed = 55f;
                    light.Color = Color.DarkGray;
                    break;

                default:
                    MoveSpeed = 40f;
                    break;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Add talk component
            Add(talkComponent = new TalkComponent(
                new Rectangle(-24, -32, 48, 32),
                new Vector2(0f, -24f),
                OnTalk
            ));
            talkComponent.PlayerMustBeFacing = false;
            
            // Add interact wiggler
            Add(interactWiggler = Wiggler.Create(0.4f, 4f, v => {
                sprite.Scale = Vector2.One * (1f + v * 0.1f);
            }));
            
            Depth = Depths.NPCs;
        }

        protected override void SetupSprite()
        {
            string spritePath = GetSpritePathForCharacter();
            
            Add(sprite = new Sprite(GFX.Game, spritePath));
            sprite.AddLoop("idle", "", 0.15f);
            sprite.AddLoop("walk", "", 0.1f);
            sprite.AddLoop("talk", "", 0.12f);
            sprite.AddLoop("happy", "", 0.1f);
            sprite.CenterOrigin();
            sprite.Play("idle");
        }

        private string GetSpritePathForCharacter()
        {
            return Character switch
            {
                NPCCharacter.BandanaWaddleDee => "characters/kirby/bandana_dee/",
                NPCCharacter.KingDedede => "characters/kirby/king_dedede/",
                NPCCharacter.MetaKnight => "characters/kirby/meta_knight/",
                NPCCharacter.Magolor => "characters/kirby/magolor/",
                NPCCharacter.Taranza => "characters/kirby/taranza/",
                NPCCharacter.Susie => "characters/kirby/susie/",
                NPCCharacter.Elfilin => "characters/kirby/elfilin/",
                NPCCharacter.Adeleine => "characters/kirby/adeleine/",
                NPCCharacter.Ribbon => "characters/kirby/ribbon/",
                NPCCharacter.Gooey => "characters/kirby/gooey/",
                NPCCharacter.Marx => "characters/kirby/marx/",
                NPCCharacter.DarkMetaKnight => "characters/kirby/dark_metaknight/",
                _ => "characters/kirby/bandana_dee/"
            };
        }

        public override void Update()
        {
            base.Update();
            
            // Update behavior
            switch (Behavior)
            {
                case NPCBehavior.Wander:
                    UpdateWander();
                    break;
                case NPCBehavior.Follow:
                    UpdateFollow();
                    break;
                case NPCBehavior.Patrol:
                    UpdatePatrol();
                    break;
            }
            
            // Update sprite animation based on movement
            if (State == ActorState.Talking)
            {
                sprite?.Play("talk");
            }
            else if (Math.Abs(Speed.X) > 1f || Math.Abs(Speed.Y) > 1f)
            {
                sprite?.Play("walk");
            }
            else
            {
                sprite?.Play("idle");
            }
        }

        private Vector2 Speed = Vector2.Zero;

        private void UpdateWander()
        {
            if (State == ActorState.Talking) return;
            
            wanderTimer -= Engine.DeltaTime;
            
            if (wanderTimer <= 0)
            {
                // Pick new wander target
                wanderTarget = startPosition + new Vector2(
                    Calc.Random.Range(-64f, 64f),
                    Calc.Random.Range(-32f, 32f)
                );
                wanderTimer = Calc.Random.Range(2f, 5f);
            }
            
            // Move toward target
            Vector2 toTarget = wanderTarget - Position;
            if (toTarget.Length() > 8f)
            {
                Vector2 moveDir = toTarget.SafeNormalize();
                Speed = moveDir * MoveSpeed;
                MoveH(Speed.X * Engine.DeltaTime);
                MoveV(Speed.Y * Engine.DeltaTime);
                facingRight = Speed.X > 0;
            }
            else
            {
                Speed = Vector2.Zero;
            }
        }

        private void UpdateFollow()
        {
            if (State == ActorState.Talking || targetPlayer == null) return;
            
            Vector2 toPlayer = targetPlayer.Position - Position;
            float distance = toPlayer.Length();
            
            if (distance > followDistance)
            {
                Vector2 moveDir = toPlayer.SafeNormalize();
                Speed = moveDir * MoveSpeed;
                MoveH(Speed.X * Engine.DeltaTime);
                MoveV(Speed.Y * Engine.DeltaTime);
                facingRight = Speed.X > 0;
            }
            else
            {
                Speed = Vector2.Zero;
                // Face player when idle
                facingRight = targetPlayer.Position.X > Position.X;
            }
        }

        private void UpdatePatrol()
        {
            if (State == ActorState.Talking || patrolNodes.Count == 0) return;
            
            Vector2 target = patrolNodes[currentPatrolNode];
            Vector2 toTarget = target - Position;
            float distance = toTarget.Length();
            
            if (distance > 8f)
            {
                Vector2 moveDir = toTarget.SafeNormalize();
                Speed = moveDir * MoveSpeed;
                MoveH(Speed.X * Engine.DeltaTime);
                MoveV(Speed.Y * Engine.DeltaTime);
                facingRight = Speed.X > 0;
            }
            else
            {
                // Reached node, move to next
                currentPatrolNode = (currentPatrolNode + 1) % patrolNodes.Count;
                Speed = Vector2.Zero;
            }
        }

        private void OnTalk(global::Celeste.Player player)
        {
            if (State == ActorState.Talking) return;
            
            State = ActorState.Talking;
            interactWiggler.Start();
            
            // Face player
            facingRight = player.Position.X > Position.X;
            
            // Start dialog
            Level level = Scene as Level;
            if (level != null && !string.IsNullOrEmpty(DialogId))
            {
                level.StartCutscene(OnTalkEnd);
                Add(new Coroutine(TalkRoutine(player, level)));
            }
            else
            {
                // No dialog, just give item if applicable
                if (CanGiveItem && !string.IsNullOrEmpty(GiveItemId))
                {
                    GiveItemToPlayer(player);
                }
                State = ActorState.Idle;
            }
            
            HasInteracted = true;
        }

        private IEnumerator TalkRoutine(global::Celeste.Player player, Level level)
        {
            player.StateMachine.State = global::Celeste.Player.StDummy;
            yield return 0.1f;
            
            // Show dialog
            yield return Textbox.Say(DialogId);
            
            // Give item after dialog if applicable
            if (CanGiveItem && !string.IsNullOrEmpty(GiveItemId))
            {
                yield return 0.2f;
                GiveItemToPlayer(player);
                yield return 0.5f;
            }
            
            player.StateMachine.State = global::Celeste.Player.StNormal;
            level.EndCutscene();
            State = ActorState.Idle;
        }

        private void OnTalkEnd(Level level)
        {
            State = ActorState.Idle;
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && player.StateMachine.State == global::Celeste.Player.StDummy)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }
        }

        private void GiveItemToPlayer(global::Celeste.Player player)
        {
            // Give item based on ID
            switch (GiveItemId.ToLower())
            {
                case "food":
                case "maxtomato":
                    Scene.Add(new KirbyFood(Position + new Vector2(0, -16), KirbyFood.FoodType.MaxTomato));
                    break;
                case "health":
                    var kirby = Scene.Tracker.GetEntity<KirbyPlayer>();
                    kirby?.Heal(20);
                    break;
                case "star":
                    Scene.Add(new KirbyFood(Position + new Vector2(0, -16), KirbyFood.FoodType.InvincibilityStar));
                    break;
                default:
                    // Generic item
                    Scene.Add(new KirbyFood(Position + new Vector2(0, -16), KirbyFood.FoodType.Apple));
                    break;
            }
            
            Audio.Play("event:/game/general/diamond_touch", Position);
            interactWiggler.Start();
        }

        protected override void OnPlayerCollision(global::Celeste.Player player)
        {
            // NPCs don't hurt the player - friendly collision
        }

        /// <summary>
        /// Add an item to this NPC's shop inventory
        /// </summary>
        public void AddShopItem(string itemId, string name, int price, int stock = -1)
        {
            ShopInventory.Add(new ShopItem(itemId, name, price, stock));
        }

        /// <summary>
        /// Open the shop UI
        /// </summary>
        public void OpenShop()
        {
            if (!IsShopKeeper || ShopInventory.Count == 0) return;
            
            // TODO: Implement shop UI
            Audio.Play("event:/game/general/diamond_touch", Position);
        }
    }

    /// <summary>
    /// Represents an item in a shop
    /// </summary>
    public class ShopItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; } // -1 for unlimited

        public ShopItem(string itemId, string name, int price, int stock = -1)
        {
            ItemId = itemId;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public bool IsAvailable => Stock != 0;

        public void Purchase()
        {
            if (Stock > 0)
                Stock--;
        }
    }
}
