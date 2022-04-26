using System;
using GorillaLocomotion;
using HarmonyLib;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(Player))]
	[HarmonyPatch("LateUpdate", MethodType.Normal)]
	internal class JumpPatch
	{
		// Token: 0x0600000C RID: 12 RVA: 0x0000208B File Offset: 0x0000028B
		private static void Prefix()
		{
		}

		// Token: 0x04000012 RID: 18
		public static bool ResetSpeed;
	}
}
