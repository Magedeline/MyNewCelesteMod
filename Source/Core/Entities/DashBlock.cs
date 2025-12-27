namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("dashBlock")]
    [Monocle.Tracked]
    public class DashBlock : Solid
    {
        public static ParticleType PBreak;

        private char tiletype;
        private bool blendin;
        private bool permanent;
        private bool canDash = true;
        private EntityID id;

        public DashBlock(EntityData data, Vector2 offset, EntityID id)
            : base(data.Position + offset, data.Width, data.Height, false)
        {
            this.id = id;
            tiletype = data.Char(nameof(tiletype), '3');
            blendin = data.Bool(nameof(blendin), true);
            permanent = data.Bool(nameof(permanent), true);
            
            OnDashCollide = OnDash;
            
            // Simplified surface sound - use constant
            SurfaceSoundIndex = 1;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            
            // Simplified tile generation - just create a basic tile grid
            TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tiletype, (int)Width / 8, (int)Height / 8).TileGrid;
            Add(tileGrid);
            Add(new TileInterceptor(tileGrid, highPriority: false));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Check if this block was already broken
            var level = scene as Level;
            if (level != null && level.Session.DoNotLoad.Contains(id))
            {
                RemoveSelf();
            }
        }

        private DashCollisionResults OnDash(global::Celeste.Player player, Vector2 direction)
        {
            if (!canDash)
                return DashCollisionResults.NormalCollision;

            canDash = false;
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            
            Add(new Coroutine(@break(player, direction)));
            
            return DashCollisionResults.Rebound;
        }

        private IEnumerator @break(global::Celeste.Player player, Vector2 direction)
        {
            Audio.Play("event:/game/general/wall_break_dirt", Center);
            
            // Create break particles
            var level = Scene as Level;
            if (level != null && PBreak != null)
            {
                for (int x = 0; x < Width / 8f; x++)
                {
                    for (int y = 0; y < Height / 8f; y++)
                    {
                        Vector2 particlePos = Position + new Vector2(x * 8 + 4, y * 8 + 4);
                        level.Particles.Emit(PBreak, particlePos, Color.Gray);
                    }
                }
            }
            
            Visible = false;
            Collidable = false;
            
            yield return 0.15f;
            
            if (!permanent)
            {
                // Respawn after delay
                yield return 3f;
                
                Audio.Play("event:/game/general/wall_break_dirt_reform", Center);
                
                Visible = true;
                Collidable = true;
                canDash = true;
            }
            else
            {
                // Mark as permanently broken
                var session = (Scene as Level)?.Session;
                if (session != null)
                {
                    session.DoNotLoad.Add(id);
                }
                
                RemoveSelf();
            }
        }

        public static void LoadParticles()
        {
            PBreak = new ParticleType
            {
                Color = Color.Gray,
                Color2 = Color.DarkGray,
                ColorMode = ParticleType.ColorModes.Choose,
                Size = 1f,
                SizeRange = 0.5f,
                LifeMin = 0.6f,
                LifeMax = 1.2f,
                SpeedMin = 20f,
                SpeedMax = 60f,
                DirectionRange = (float)Math.PI * 2f,
                FadeMode = ParticleType.FadeModes.Late
            };
        }

        public void Break(Vector2 from, Vector2 direction, bool playSound = true, bool playDebrisSound = true)
        {
            if (!Collidable)
                return;

            canDash = false;
            
            if (playSound)
                Audio.Play("event:/game/general/wall_break_dirt", Center);
            
            // Create break particles
            var level = Scene as Level;
            if (level != null && PBreak != null)
            {
                for (int x = 0; x < Width / 8f; x++)
                {
                    for (int y = 0; y < Height / 8f; y++)
                    {
                        Vector2 particlePos = Position + new Vector2(x * 8 + 4, y * 8 + 4);
                        level.Particles.Emit(PBreak, particlePos, Color.Gray);
                    }
                }
            }
            
            Visible = false;
            Collidable = false;
            
            if (permanent)
            {
                // Mark as permanently broken
                var session = (Scene as Level)?.Session;
                if (session != null)
                {
                    session.DoNotLoad.Add(id);
                }
                
                RemoveSelf();
            }
            else
            {
                // Respawn after delay
                Add(new Coroutine(RespawnSequence()));
            }
        }

        private IEnumerator RespawnSequence()
        {
            yield return 3f;
            
            Audio.Play("event:/game/general/wall_break_dirt_reform", Center);
            
            Visible = true;
            Collidable = true;
            canDash = true;
        }
    }
}



