using System;
using System.Reflection;
using HarmonyLib;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x02000003 RID: 3
	public class ModMenuPatches
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002073 File Offset: 0x00000273
		// (set) Token: 0x06000008 RID: 8 RVA: 0x0000207A File Offset: 0x0000027A
		public static bool IsPatched { get; private set; }

		// Token: 0x06000009 RID: 9 RVA: 0x000022A8 File Offset: 0x000004A8
		internal static void ApplyHarmonyPatches()
		{
			bool flag = !ModMenuPatches.IsPatched;
			if (flag)
			{
				bool flag2 = ModMenuPatches.instance == null;
				if (flag2)
				{
					ModMenuPatches.instance = new Harmony("com.legoandmars.gorillatag.infinitemenu");
				}
				ModMenuPatches.instance.PatchAll(Assembly.GetExecutingAssembly());
				ModMenuPatches.IsPatched = true;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022F8 File Offset: 0x000004F8
		internal static void RemoveHarmonyPatches()
		{
			bool flag = ModMenuPatches.instance != null && ModMenuPatches.IsPatched;
			if (flag)
			{
				ModMenuPatches.instance.UnpatchSelf();
				ModMenuPatches.IsPatched = false;
			}
		}

		// Token: 0x0400000F RID: 15
		private static Harmony instance;

		// Token: 0x04000011 RID: 17
		public const string InstanceId = "com.legoandmars.gorillatag.infinitemenu";
	}
}
