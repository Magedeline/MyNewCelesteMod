using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.DesoloZatnas.Core
{
    /// <summary>
    /// Custom Overworld implementation for DesoloZatnas mod
    /// Extends the base Overworld with custom UI elements and behaviors
    /// </summary>
    public class OverworldMod : Overworld
    {
        public static OverworldMod Instance { get; private set; }

        public bool CustomMusicEnabled { get; set; } = true;
        public string CustomMenuMusic { get; set; } = "event:/Ingeste/music/menu/level_select";
        public string CustomMenuAmbience { get; set; } = "event:/env/amb/worldmap";

        public OverworldMod(OverworldLoader loader) : base(loader)
        {
            Instance = this;
        }

        public override void Begin()
        {
            base.Begin();
            
            // Initialize custom overworld elements
            if (CustomMusicEnabled)
            {
                ApplyCustomMusic();
            }
        }

        public override void Update()
        {
            base.Update();
            
            // Add custom update logic here
            UpdateCustomElements();
        }

        private void ApplyCustomMusic()
        {
            if (!string.IsNullOrEmpty(CustomMenuMusic))
            {
                Audio.SetMusic(CustomMenuMusic);
            }
            
            if (!string.IsNullOrEmpty(CustomMenuAmbience))
            {
                Audio.SetAmbience(CustomMenuAmbience);
            }
        }

        private void UpdateCustomElements()
        {
            // Custom update logic for mod-specific overworld elements
        }

        public void RegisterCustomOui<T>() where T : Oui, new()
        {
            T customOui = new T();
            customOui.Visible = false;
            Add(customOui);
            UIs.Add(customOui);
        }

        public override void End()
        {
            Instance = null;
            base.End();
        }
    }
}
