using System;
using UnityEngine;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x0200000A RID: 10
	internal class BtnCollider2 : MonoBehaviour
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00007348 File Offset: 0x00005548
		private void OnTriggerEnter(Collider collider)
		{
			bool flag = Time.frameCount >= MenuPatch.framePressCooldown + 30;
			if (flag)
			{
				MenuPatch.Toggle2(this.relatedText);
				MenuPatch.framePressCooldown = Time.frameCount;
			}
		}

		// Token: 0x0400007C RID: 124
		public string relatedText;
	}
}
