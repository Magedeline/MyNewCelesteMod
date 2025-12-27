namespace DesoloZantas.Core.Core
{
    public class FixedMapDataProcessor : EverestMapDataProcessor
    {
        private static readonly Dictionary<string, string> TilesetMappings = new Dictionary<string, string>
        {
            // Core tilesets
            [""] = "stone", // Empty tileset defaults to stone
            ["default"] = "stone",
            ["dirt"] = "dirt",
            ["stone"] = "stone", 
            ["wood"] = "wood",
            ["snow"] = "snow",
            ["summit"] = "summit",
            ["summitNoSnow"] = "summitNoSnow",
            ["reflection"] = "reflection",
            ["reflectionAlt"] = "reflectionAlt",
            ["templeA"] = "templeA",
            ["templeB"] = "templeB",
            ["rock"] = "rock",
            ["scifi"] = "scifi",
            ["tower"] = "tower",
            ["cliffside"] = "cliffside",
            ["core"] = "core",
            ["deadgrass"] = "deadgrass",
            ["lostlevels"] = "lostlevels",
            ["poolEdges"] = "poolEdges",
            ["scenery"] = "scenery",
            ["starJump"] = "starJump",
            ["woodStoneEdges"] = "woodStoneEdges"
        };

        private static readonly Dictionary<string, string> DecalMappings = new Dictionary<string, string>
        {
            // Fix common decal path issues
            ["grass_a.png"] = "generic/grass_a.png",
            ["grass_b.png"] = "generic/grass_b.png",
            ["grass_c.png"] = "generic/grass_c.png",
            ["grass_d.png"] = "generic/grass_d.png",
            ["snow_a.png"] = "generic/snow_a.png",
            ["snow_b.png"] = "generic/snow_b.png",
            ["snow_c.png"] = "generic/snow_c.png", 
            ["snow_d.png"] = "generic/snow_d.png",
            ["snow_e.png"] = "generic/snow_e.png",
            ["snow_f.png"] = "generic/snow_f.png",
            ["snow_g.png"] = "generic/snow_g.png",
            ["snow_h.png"] = "generic/snow_h.png",
            ["snow_i.png"] = "generic/snow_i.png",
            ["snow_j.png"] = "generic/snow_j.png",
            ["snow_k.png"] = "generic/snow_k.png",
            ["snow_l.png"] = "generic/snow_l.png",
            ["snow_m.png"] = "generic/snow_m.png",
            ["snow_n.png"] = "generic/snow_n.png",
            ["snow_o.png"] = "generic/snow_o.png",
            ["hanginggrass_a.png"] = "generic/hanginggrass_a.png",
            
            // Chapter specific decals - map decals to their location folders
            ["sign.png"] = "0-prologue/sign.png",
            ["axe.png"] = "0-prologue/axe.png", 
            ["house.png"] = "0-prologue/house.png",
            ["leftArrowSign.png"] = "1-forsakencity/leftArrowSign.png",
            
            // Maggy-specific decals - map to maggy location folder
            ["maggy_sign.png"] = "maggy/sign.png",
            ["maggy_house.png"] = "maggy/house.png",
            ["maggy_tower.png"] = "maggy/tower.png",
            ["tower.png"] = "maggy/tower.png",
            ["truth_tower.png"] = "maggy/8-truth/tower.png",
            ["goodbye_tower.png"] = "maggy/19-goodbye/tower.png",
            ["true_goodbye_tower.png"] = "maggy/19-true_goodbye/tower.png",
            
            // Additional common decals that should be in generic
            ["vine_a.png"] = "generic/vine_a.png",
            ["vine_b.png"] = "generic/vine_b.png",
            ["rock_a.png"] = "generic/rock_a.png",
            ["rock_b.png"] = "generic/rock_b.png",
            ["crystal_a.png"] = "generic/crystal_a.png",
            ["crystal_b.png"] = "generic/crystal_b.png"
        };

        public override Dictionary<string, Action<BinaryPacker.Element>> Init()
        {
            return new Dictionary<string, Action<BinaryPacker.Element>>
            {
                {
                    "level", level => {
                        FixTilesets(level);
                        FixDecals(level);
                        FixAnimatedTiles(level);
                    }
                }
            };
        }

        public override void Reset()
        {
            // Reset any processor state if needed
            // This implementation doesn't maintain state, so nothing to reset
        }

        public override void End()
        {
            // Clean up any resources or finalize processing
            // This implementation doesn't need cleanup
        }

        private static void FixTilesets(BinaryPacker.Element level)
        {
            // Fix jumpThru and solid tilesets
            foreach (BinaryPacker.Element child in level.Children)
            {
                if (child.Name == "entities")
                {
                    foreach (BinaryPacker.Element entity in child.Children)
                    {
                        if (entity.Name == "jumpThru" && entity.Attributes.TryGetValue("texture", out object textureObj))
                        {
                            string texture = textureObj?.ToString() ?? "";
                            if (TilesetMappings.TryGetValue(texture, out string fixedTexture))
                            {
                                entity.Attributes["texture"] = fixedTexture;
                                Logger.Log(LogLevel.Info, "Ingeste/FixedMapDataProcessor", 
                                    $"Fixed jumpThru tileset: {texture} -> {fixedTexture}");
                            }
                        }
                        
                        if (entity.Name == "solid" && entity.Attributes.TryGetValue("tiletype", out object tiletypeObj))
                        {
                            string tiletype = tiletypeObj?.ToString() ?? "";
                            if (TilesetMappings.TryGetValue(tiletype, out string fixedTiletype))
                            {
                                entity.Attributes["tiletype"] = fixedTiletype;
                            }
                        }
                    }
                }
            }
        }

        private static void FixDecals(BinaryPacker.Element level)
        {
            // Fix foreground and background decals
            foreach (BinaryPacker.Element child in level.Children)
            {
                if (child.Name == "fgdecals" || child.Name == "bgdecals")
                {
                    FixDecalTextures(child, child.Name);
                }
            }
        }

        private static void FixDecalTextures(BinaryPacker.Element decalsElement, string type)
        {
            foreach (BinaryPacker.Element decal in decalsElement.Children)
            {
                if (decal.Attributes.TryGetValue("texture", out object textureObj))
                {
                    string texture = textureObj?.ToString() ?? "";
                    string fixedTexture = GetFixedDecalTexture(texture);
                    
                    if (fixedTexture != texture)
                    {
                        decal.Attributes["texture"] = fixedTexture;
                        Logger.Log(LogLevel.Info, "Ingeste/FixedMapDataProcessor", 
                            $"Fixed {type} decal: {texture} -> {fixedTexture}");
                    }
                }

                // Fix color format (ensure 8-char hex with alpha)
                if (decal.Attributes.TryGetValue("color", out object colorObj))
                {
                    string color = colorObj?.ToString() ?? "";
                    if (color.Length == 6 && IsHexString(color))
                    {
                        decal.Attributes["color"] = color + "ff";
                    }
                }
            }
        }

        private static string GetFixedDecalTexture(string texture)
        {
            // Direct mapping first
            if (DecalMappings.TryGetValue(texture, out string mapped))
            {
                return mapped;
            }

            // Auto-fix patterns
            if (!texture.Contains("/") && texture.EndsWith(".png"))
            {
                // Try adding generic/ prefix for standalone files
                string genericPath = $"generic/{texture}";
                if (DecalMappings.ContainsValue(genericPath))
                {
                    return genericPath;
                }
            }

            return texture; // Return original if no fix found
        }

        private static void FixAnimatedTiles(BinaryPacker.Element level)
        {
            // Fix animated tile references in solids and jumpThrus
            foreach (BinaryPacker.Element child in level.Children)
            {
                if (child.Name == "solids" || child.Name == "bg")
                {
                    FixTileGrids(child);
                }
                else if (child.Name == "entities")
                {
                    // Also fix entity tileset references
                    FixEntityTilesets(child);
                }
            }
        }

        private static void FixTileGrids(BinaryPacker.Element tilesElement)
        {
            if (tilesElement.Attributes.TryGetValue("innerText", out object innerTextObj))
            {
                string innerText = innerTextObj?.ToString() ?? "";
                
                // Fix common tile character mappings for animated tiles
                string fixedInnerText = innerText
                    .Replace('?', '0') // Replace unknown chars with default
                    .Replace('\0', '0'); // Replace null chars
                    
                if (fixedInnerText != innerText)
                {
                    tilesElement.Attributes["innerText"] = fixedInnerText;
                    Logger.Log(LogLevel.Info, "Ingeste/FixedMapDataProcessor", 
                        $"Fixed tile grid innerText");
                }
            }
        }

        private static void FixEntityTilesets(BinaryPacker.Element entitiesElement)
        {
            foreach (BinaryPacker.Element entity in entitiesElement.Children)
            {
                // Fix common entity tileset issues
                if (entity.Attributes.TryGetValue("texture", out object textureObj))
                {
                    string texture = textureObj?.ToString() ?? "";
                    
                    // Fix IngesteHelper sprite path issues
                    if (texture.StartsWith("IngesteHelper/"))
                    {
                        string fixedTexture = "objects/" + texture;
                        entity.Attributes["texture"] = fixedTexture;
                        Logger.Log(LogLevel.Info, "Ingeste/FixedMapDataProcessor", 
                            $"Fixed entity texture path: {texture} -> {fixedTexture}");
                    }
                }
                
                // Fix missing sprite paths
                if (entity.Attributes.TryGetValue("sprite", out object spriteObj))
                {
                    string sprite = spriteObj?.ToString() ?? "";
                    
                    if (sprite.StartsWith("IngesteHelper/"))
                    {
                        string fixedSprite = "objects/" + sprite;
                        entity.Attributes["sprite"] = fixedSprite;
                        Logger.Log(LogLevel.Info, "Ingeste/FixedMapDataProcessor", 
                            $"Fixed entity sprite path: {sprite} -> {fixedSprite}");
                    }
                }
            }
        }

        private static bool IsHexString(string str)
        {
            return str.All(c => Uri.IsHexDigit(c));
        }
    }
}



