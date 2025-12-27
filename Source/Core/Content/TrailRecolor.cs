using Celeste.Mod.SkinModHelper;
using static Celeste.Mod.SkinModHelper.SkinsSystem;

namespace DesoloZantas.Core.Core.Content {
    public static class TrailRecolor {
        #region Hooks
        public static void Load() {
            On.Celeste.TrailManager.Add_Vector2_Image_PlayerHair_Vector2_Color_int_float_bool_bool += onTrailManager_Add_V2IV2CIFBB;
        }

        public static void Unload() {
            On.Celeste.TrailManager.Add_Vector2_Image_PlayerHair_Vector2_Color_int_float_bool_bool -= onTrailManager_Add_V2IV2CIFBB;
        }

        private static TrailManager.Snapshot onTrailManager_Add_V2IV2CIFBB(On.Celeste.TrailManager.orig_Add_Vector2_Image_PlayerHair_Vector2_Color_int_float_bool_bool orig,
            Vector2 position, Image image, PlayerHair hair, Vector2 scale, Color color, int depth, float duration, bool frozenUpdate, bool useRawDeltaTime) {


            return orig(position, image, hair, scale, (TrailsRecolor(image, hair) ?? color), depth, duration, frozenUpdate, useRawDeltaTime);
        }

        public static Color? TrailsRecolor(Image sprite, PlayerHair hair) {
            if (hair != null && hair.Sprite?.Mode != Celeste.PlayerSpriteMode.Badeline) {
                return null; // Exclude players and silhouette.
            }

            string TrailsColor = CharacterConfig.For(sprite).TrailsColor;

            if (RGB_IsMatch(TrailsColor))
                return Calc.HexToColor(TrailsColor);
            if (TrailsColor == "HairColor" && hair != null)
                return hair.Color;
            return null;
        }
        #endregion
    }
}



