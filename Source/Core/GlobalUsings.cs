// Global using directives

// Global using directives for Ingeste mod
global using System;
global using System.IO;
global using System.Collections.Generic;
global using System.Collections;
global using System.Linq;
global using Microsoft.Xna.Framework;
global using Monocle;
global using Celeste;
// REMOVED: global using FMOD.Studio; - Use Celeste's Audio system instead
global using Celeste.Mod;
global using Celeste.Mod.Entities; // For CustomEntity attribute
global using DesoloZantas.Core;
// For AudioHelper type
// Import AudioHelper as a static using so it can be called directly without namespace prefix
global using static DesoloZantas.Core.Core.AudioSystems.AudioHelper;
// Use Celeste.Audio explicitly to avoid conflict with DesoloZantas.Core.AudioSupport.Audio proxy
global using Audio = Celeste.Audio;
// Type aliases to disambiguate custom entities from Celeste types
global using CelestePayphone = Celeste.Payphone;
global using CelesteWire = Celeste.Wire;
global using CelesteDreamBlock = Celeste.DreamBlock;
global using CelesteDashBlock = Celeste.DashBlock;
global using CelesteAscendManager = Celeste.AscendManager;
global using CelesteStrawberry = Celeste.Strawberry;
global using CelesteCutsceneNode = Celeste.CutsceneNode;
global using CelesteFlingBirdIntro = Celeste.FlingBirdIntro;
global using CelesteFacings = Celeste.Facings;
global using CelestePlayer = Celeste.Player;
global using CelesteStarJumpBlock = Celeste.StarJumpBlock;
global using CelesteJumpThru = Celeste.JumpThru;
global using CelesteBridge = Celeste.Bridge;
// NOTE: Types like Payphone, Wire, DreamBlock etc have custom versions in DesoloZantas.Core.Entities
// Use the Celeste* aliases above when you need the vanilla Celeste version explicitly



