using System;
using System.ComponentModel;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ModMenuPatch.HarmonyPatches;
using UnityEngine;
using Utilla;

namespace ModMenuPatch
{
	// Token: 0x02000002 RID: 2
	[Description("InfiniteMenu")]
	[BepInPlugin("org.In4init3.gorillatag.infinitymod", "Infinite Mods Menu", "1.0.0")]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[ModdedGamemode]
	public class ModMenuPatch : BaseUnityPlugin
	{
		// Token: 0x06000001 RID: 1 RVA: 0x0000216C File Offset: 0x0000036C
		private void OnEnable()
		{
			ModMenuPatches.ApplyHarmonyPatches();
			ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "ModMonkeyPatch.cfg"), true);
			ModMenuPatch.speedMultiplier = configFile.Bind<float>("Configuration", "SpeedMultiplier", 12f, "How much to multiply the speed. 10 = 10x higher jumps");
			ModMenuPatch.jumpMultiplier = configFile.Bind<float>("Configuration", "JumpMultiplier", 12f, "How much to multiply the jump height/distance by. 10 = 10x higher jumps");
			ModMenuPatch.randomColor = configFile.Bind<bool>("rgb_monke", "RandomColor", false, "Whether to cycle through colours of rainbow or choose random colors");
			ModMenuPatch.cycleSpeed = configFile.Bind<float>("rgb_monke", "CycleSpeed", 1E-06f, "The speed the color cycles at each frame (1=Full colour cycle). If random colour is enabled, this is the time in seconds before switching color");
			ModMenuPatch.glowAmount = configFile.Bind<float>("rgb_monke", "GlowAmount", 1f, "The brightness of your monkey. The higher the value, the more emissive your monkey is");
			ModMenuPatch.sp = configFile.Bind<float>("Configuration", "Spring", 10f, "spring");
			ModMenuPatch.dp = configFile.Bind<float>("Configuration", "Damper", 30f, "damper");
			ModMenuPatch.ms = configFile.Bind<float>("Configuration", "MassScale", 12f, "massscale");
			ModMenuPatch.rc = configFile.Bind<Color>("Configuration", "webColor", Color.white, "webcolor hex code");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		private void OnDisable()
		{
			ModMenuPatches.RemoveHarmonyPatches();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		[ModdedGamemodeJoin]
		private void RoomJoined()
		{
			ModMenuPatch.allowSpaceMonke = true;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002059 File Offset: 0x00000259
		[ModdedGamemodeLeave]
		private void RoomLeft()
		{
			ModMenuPatch.allowSpaceMonke = true;
		}

		// Token: 0x04000001 RID: 1
		public static bool allowSpaceMonke = true;

		// Token: 0x04000002 RID: 2
		public static ConfigEntry<float> multiplier;

		// Token: 0x04000003 RID: 3
		public static ConfigEntry<float> speedMultiplier;

		// Token: 0x04000004 RID: 4
		public static ConfigEntry<float> jumpMultiplier;

		// Token: 0x04000005 RID: 5
		public static ConfigEntry<bool> randomColor;

		// Token: 0x04000006 RID: 6
		public static ConfigEntry<float> cycleSpeed;

		// Token: 0x04000007 RID: 7
		public static ConfigEntry<float> glowAmount;

		// Token: 0x04000008 RID: 8
		public static ConfigEntry<float> sp;

		// Token: 0x04000009 RID: 9
		public static ConfigEntry<float> dp;

		// Token: 0x0400000A RID: 10
		public static ConfigEntry<float> ms;

		// Token: 0x0400000B RID: 11
		public static ConfigEntry<Color> rc;

		// Token: 0x0400000C RID: 12
		public static ConfigEntry<float> Acceleration_con;

		// Token: 0x0400000D RID: 13
		public static ConfigEntry<float> Max_con;

		// Token: 0x0400000E RID: 14
		public static ConfigEntry<float> multi;
	}
}
