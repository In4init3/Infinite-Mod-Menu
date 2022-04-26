using System;
using UnityEngine;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x02000009 RID: 9
	internal class BtnCollider : MonoBehaviour
	{
		// Token: 0x06000033 RID: 51 RVA: 0x0000730C File Offset: 0x0000550C
		private void OnTriggerEnter(Collider collider)
		{
			bool flag = Time.frameCount >= MenuPatch.framePressCooldown + 30;
			if (flag)
			{
				MenuPatch.Toggle(this.relatedText);
				MenuPatch.framePressCooldown = Time.frameCount;
			}
		}

		// Token: 0x0400007B RID: 123
		public string relatedText;
	}
}
