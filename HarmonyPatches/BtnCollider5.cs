using System;
using UnityEngine;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x0200000D RID: 13
	internal class BtnCollider5 : MonoBehaviour
	{
		// Token: 0x0600003B RID: 59 RVA: 0x000073FC File Offset: 0x000055FC
		private void OnTriggerEnter(Collider collider)
		{
			bool flag = Time.frameCount >= MenuPatch.framePressCooldown + 30;
			if (flag)
			{
				MenuPatch.Toggle5(this.relatedText);
				MenuPatch.framePressCooldown = Time.frameCount;
			}
		}

		// Token: 0x0400007F RID: 127
		public string relatedText;
	}
}
