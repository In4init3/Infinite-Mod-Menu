using System;
using UnityEngine;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x0200000C RID: 12
	internal class BtnCollider4 : MonoBehaviour
	{
		// Token: 0x06000039 RID: 57 RVA: 0x000073C0 File Offset: 0x000055C0
		private void OnTriggerEnter(Collider collider)
		{
			bool flag = Time.frameCount >= MenuPatch.framePressCooldown + 30;
			if (flag)
			{
				MenuPatch.Toggle4(this.relatedText);
				MenuPatch.framePressCooldown = Time.frameCount;
			}
		}

		// Token: 0x0400007E RID: 126
		public string relatedText;
	}
}
