using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// A rainbow berry behaves just like a red berry, but appears when you got all silver berries in a defined level set.
    /// </summary>
    [CustomEntity(IngesteConstants.EntityNames.DELTA_BERRY)]
    [RegisterStrawberry(tracked: true, blocksCollection: true)]
    public class DeltaBerry : Strawberry
    {
        private readonly string levelSet;
        private readonly string mapsRaw;
        private readonly string[] maps;
        private readonly int? requiredBerries;

        internal HoloDeltaBerry HologramForCutscene;
        internal int CutsceneTotalBerries;
        public static ParticleType P_Glow;
        public static ParticleType PPulseGlow;

        // Add Golden property for compatibility
        public new bool Golden { get; private set; }
        
        // Add ID property for tracking
        public EntityID ID { get; private set; }
        
        // Total berries counter
        public int TotalBerries { get; set; }

        // Add Follower property for golden berry-like behavior (make public for hooks)
        public Follower Follower { get; private set; }
        
        // Static method for checking leader
        public static bool HasLeader { get; private set; }

        // Add private collectTimer field since we can't access the base class's private field
        private float collectTimer = 0f;

        public DeltaBerry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid)
        {
            ID = gid;
            levelSet = data.Attr(nameof(levelSet));
            mapsRaw = data.Attr(nameof(maps));

            if (string.IsNullOrEmpty(mapsRaw))
            {
                maps = new string[0];
            }
            else
            {
                maps = mapsRaw.Split(',').Select(map => $"{levelSet}/{map}").ToArray();
            }

            if (int.TryParse(data.Attr("requires"), out int result))
            {
                requiredBerries = result;
            }

            // Initialize Follower
            Follower = new Follower(gid, onLoseLeader: OnLoseLeader);
            Follower.FollowDelay = 0.3f;
            Add(Follower);
        }

        public bool TimeRanOut { get; set; }

        internal void Added(Scene scene, IngesteModule ingesteHelperModule)
        {
            base.Added(scene);

            // Fix bloom alpha value to match golden berry
            DynData<DeltaBerry> self = new DynData<DeltaBerry>(this);
            self.Get<BloomPoint>("bloom")?.Equals(0.5f);

            // TODO: Need to implement proper check for delta berry level sets
            // For now, always proceed with the logic
            
            int missingBerries = 0;
            int totalBerries = 0;

            if (PinkPLatBerry is IEnumerable<KeyValuePair<string, EntityID>> pinkPlatBerries)
            {
                foreach (var requiredPinkPlat in pinkPlatBerries)
                {
                    bool contains = maps.Contains(requiredPinkPlat.Key);
                    if (!contains) continue;

                    totalBerries++;

                    // Check if the silver berry was collected
                    var areaData = AreaData.Get(requiredPinkPlat.Key) as AreaData;
                    if (areaData == null)
                    {
                        throw new InvalidCastException($"The object returned by AreaData.Get is not of type AreaData for key: {requiredPinkPlat.Key}");
                    }
                    AreaStats stats = SaveData.Instance.GetAreaStatsFor(areaData.ToKey());
                    if (!stats.Modes[0].Strawberries.Contains(requiredPinkPlat.Value))
                    {
                        missingBerries++;
                    }
                }
            }

            if (requiredBerries.HasValue)
            {
                // Adjust total and missing berry count based on required amount
                int collectedBerries = totalBerries - missingBerries;
                missingBerries = Math.Max(0, requiredBerries.Value - collectedBerries);
                totalBerries = requiredBerries.Value;
            }

            if (missingBerries > 0)
            {
                // Some berries are missing, spawn the hologram instead of the berry
                scene.Add(new HoloDeltaBerry(Position, totalBerries - missingBerries, totalBerries));
                RemoveSelf();
            }
            else
            {
                // All berries are collected, check if we should play the unlock cutscene
                var saveData = IngesteModule.SaveData;
                dynamic typedSaveData = saveData;
                if (typedSaveData.CombinedDeltaBerries == null ||
                    !typedSaveData.CombinedDeltaBerries.Equals(GetCombinedRainbowId(scene as Level)))
                {
                    HoloDeltaBerry hologram = new HoloDeltaBerry(Position, totalBerries, totalBerries)
                    {
                        Tag = Tags.FrozenUpdate
                    };
                    scene.Add(hologram);

                    // Make rainbow berry temporarily invisible
                    Visible = true;
                    Collidable = true;
                    self.Get<BloomPoint>("bloom").Visible = false;
                    self.Get<VertexLight>("light").Visible = false;
                    // Wait for trigger activation
                    HologramForCutscene = hologram;
                    CutsceneTotalBerries = totalBerries;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, EntityID>> PinkPLatBerry { get; set; }

        public bool MatchesRainbowBerryTriggerWithSettings(string s, string maps1)
        {
            string[] maps2 = maps1.Split(',');
            return maps == null || maps2.Any(map => maps.Contains($"{levelSet}/{map}"));
        }

        public object GetCombinedRainbowId(Level scene)
        {
            return $"{levelSet}:{mapsRaw}";
        }

        internal void CollectedSeeds()
        {
            var saveData = IngesteModule.SaveData;
            dynamic typedSaveData = saveData;
            typedSaveData.CombinedDeltaBerries = GetCombinedRainbowId(Scene as Level);
        }

        private void OnLoseLeader()
        {
            // Reset collect timer when losing leader
            this.collectTimer = 0.15f;
        }

        // Override Update() method to add collection logic for follower berries
        public override void Update()
        {
            base.Update();

            // Collection logic for when the berry is following the player
            if (Follower.Leader != null && Follower.DelayTimer <= 0.0 &&
                StrawberryRegistry.IsFirstStrawberry(this))
            {
                if (Follower.Leader.Entity is global::Celeste.Player entity && entity.Scene != null &&
                    !entity.StrawberriesBlocked)
                {
                    // Only collect when player is on the ground
                    if (entity.OnGround())
                    {
                        Vector2 toBerry = this.Position - entity.Center;
                        if (toBerry.LengthSquared() < 2304.0f)
                        {
                            this.collectTimer += Engine.DeltaTime;
                            if (this.collectTimer > 0.15f)
                                this.OnCollect();
                        }
                        else
                            this.collectTimer = Calc.Approach(this.collectTimer, 0.0f, Engine.DeltaTime);
                    }
                    else
                    {
                        // Reset timer if player is in air
                        this.collectTimer = 0.0f;
                    }
                }
            }
        }
    }
}




