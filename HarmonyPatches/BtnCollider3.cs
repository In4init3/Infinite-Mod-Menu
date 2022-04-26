using System;
using UnityEngine;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x0200000B RID: 11
	internal class BtnCollider3 : MonoBehaviour
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00007384 File Offset: 0x00005584
		private void OnTriggerEnter(Collider collider)
		{
			bool flag = Time.frameCount >= MenuPatch.framePressCooldown + 30;
			if (flag)
			{
				MenuPatch.Toggle3(this.relatedText);
				MenuPatch.framePressCooldown = Time.frameCount;
			}
		}

		// Token: 0x0400007D RID: 125
		public string relatedText;
	}
}
