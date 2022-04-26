using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace ModMenuPatch.HarmonyPatches
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(GorillaLocomotion.Player))]
	[HarmonyPatch("LateUpdate", MethodType.Normal)]
	internal class MenuPatch
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002330 File Offset: 0x00000530
		private static void Prefix()
		{
			bool allowSpaceMonke = ModMenuPatch.allowSpaceMonke;
			try
			{
				if (MenuPatch.maxJumpSpeed == null)
				{
					MenuPatch.maxJumpSpeed = new float?(GorillaLocomotion.Player.Instance.maxJumpSpeed);
				}
				if (MenuPatch.jumpMultiplier == null)
				{
					MenuPatch.jumpMultiplier = new float?(GorillaLocomotion.Player.Instance.jumpMultiplier);
				}
				if (MenuPatch.maxArmLengthInitial == null)
				{
					MenuPatch.maxArmLengthInitial = new float?(GorillaLocomotion.Player.Instance.maxArmLength);
					MenuPatch.leftHandOffsetInitial = new Vector3?(GorillaLocomotion.Player.Instance.leftHandOffset);
					MenuPatch.rightHandOffsetInitial = new Vector3?(GorillaLocomotion.Player.Instance.rightHandOffset);
				}
				List<InputDevice> list = new List<InputDevice>();
				InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, list);
				list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out MenuPatch.gripDown);
				if (MenuPatch.gripDown && MenuPatch.menu == null)
				{
					if (MenuPatch.page == 0)
					{
						MenuPatch.Draw();
					}
					if (MenuPatch.page == 1)
					{
						MenuPatch.Draw2();
					}
					if (MenuPatch.page == 2)
					{
						MenuPatch.Draw3();
					}
					if (MenuPatch.page == 3)
					{
						MenuPatch.Draw4();
					}
					if (MenuPatch.page == 4)
					{
						MenuPatch.Draw5();
					}
					if (MenuPatch.reference == null)
					{
						MenuPatch.reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(MenuPatch.reference.GetComponent<MeshRenderer>());
						MenuPatch.reference.transform.parent = GorillaLocomotion.Player.Instance.rightHandTransform;
						MenuPatch.reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
						MenuPatch.reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					}
				}
				else if (!MenuPatch.gripDown && MenuPatch.menu != null)
				{
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					UnityEngine.Object.Destroy(MenuPatch.reference);
					MenuPatch.reference = null;
				}
				if (MenuPatch.gripDown && MenuPatch.menu != null)
				{
					MenuPatch.menu.transform.position = GorillaLocomotion.Player.Instance.leftHandTransform.position;
					MenuPatch.menu.transform.rotation = GorillaLocomotion.Player.Instance.leftHandTransform.rotation;
				}
				bool? flag = MenuPatch.buttonsActive[0];
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag3 = false;
					bool flag4 = false;
					list = new List<InputDevice>();
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
					list[0].TryGetFeatureValue(CommonUsages.primaryButton, out flag3);
					list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out flag4);
					if (flag3)
					{
						GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 40f;
						GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
						if (!MenuPatch.flying)
						{
							MenuPatch.flying = true;
						}
					}
					else if (MenuPatch.flying)
					{
						GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 36f;
						MenuPatch.flying = false;
					}
					if (flag4)
					{
						if (!MenuPatch.gravityToggled && GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity)
						{
							GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity = false;
							MenuPatch.gravityToggled = true;
						}
						else if (!MenuPatch.gravityToggled && !GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity)
						{
							GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity = true;
							MenuPatch.gravityToggled = true;
						}
					}
					else
					{
						MenuPatch.gravityToggled = false;
					}
				}
				flag = MenuPatch.buttonsActive[1];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag5 = false;
					bool flag6 = false;
					list = new List<InputDevice>();
					InputDevices.GetDevices(list);
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
					list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag5);
					list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag6);
					if (flag6)
					{
						RaycastHit hitInfo;
						Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out hitInfo);
						if (MenuPatch.pointer == null)
						{
							MenuPatch.pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<SphereCollider>());
							MenuPatch.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
						}
						MenuPatch.pointer.transform.position = hitInfo.point;
						Photon.Realtime.Player player;
						bool flag7 = GorillaTagger.Instance.TryToTag(hitInfo, true, out player);
						if (flag5 && !flag7)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
						}
						else if (!flag5 && flag7)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
						}
						else if (flag5 && flag7)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
							MenuPatch.C4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
							UnityEngine.Object.Destroy(MenuPatch.C4.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.C4.GetComponent<BoxCollider>());
							MenuPatch.C4.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
							MenuPatch.Fuze = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
							UnityEngine.Object.Destroy(MenuPatch.Fuze.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.Fuze.GetComponent<BoxCollider>());
							MenuPatch.Fuze.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
							MenuPatch.C4.transform.position = hitInfo.point;
							MenuPatch.C4.transform.localScale = new Vector3(0.2f, 0.1f, 0.4f);
							MenuPatch.Fuze.transform.position = hitInfo.point;
							MenuPatch.Fuze.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
							MenuPatch.spawned = true;
						}
					}
					else
					{
						UnityEngine.Object.Destroy(MenuPatch.pointer);
						MenuPatch.pointer = null;
					}
				}
				flag = MenuPatch.buttonsActive[2];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					GorillaLocomotion.Player.Instance.maxJumpSpeed = ModMenuPatch.speedMultiplier.Value;
					GorillaLocomotion.Player.Instance.jumpMultiplier = ModMenuPatch.jumpMultiplier.Value;
				}
				else
				{
					GorillaLocomotion.Player.Instance.maxJumpSpeed = MenuPatch.maxJumpSpeed.Value;
					GorillaLocomotion.Player.Instance.jumpMultiplier = 1.15f;
				}
				flag = MenuPatch.buttonsActive[3];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					foreach (Photon.Realtime.Player player2 in PhotonNetwork.PlayerList)
					{
						PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[] { player2 });
					}
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw();
				}
				flag = MenuPatch.buttonsActive2[7];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					GorillaLocomotion.Player.Instance.disableMovement = false;
				}
				flag = MenuPatch.buttonsActive[5];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page++;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw2();
				}
				flag = MenuPatch.buttonsActive[6];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag8 = false;
					bool flag9 = false;
					list = new List<InputDevice>();
					InputDevices.GetDevices(list);
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
					list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag8);
					list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag9);
					if (flag9)
					{
						RaycastHit hitInfo2;
						Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out hitInfo2);
						if (MenuPatch.pointer == null)
						{
							MenuPatch.pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<SphereCollider>());
							MenuPatch.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
						}
						MenuPatch.pointer.transform.position = hitInfo2.point;
						Photon.Realtime.Player targetPlayer;
						bool flag10 = GorillaTagger.Instance.TryToTag(hitInfo2, true, out targetPlayer);
						if (flag8 && !flag10)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
						}
						else if (!flag8 && flag10)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
						}
						else if (flag8 && flag10)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
							GorillaGameManager.instance.photonView.RPC("JoinPubWithFreinds", targetPlayer, Array.Empty<object>());
						}
					}
					else
					{
						UnityEngine.Object.Destroy(MenuPatch.pointer);
						MenuPatch.pointer = null;
					}
				}
				flag = MenuPatch.buttonsActive[7];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag11 = false;
					bool flag12 = false;
					list = new List<InputDevice>();
					InputDevices.GetDevices(list);
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
					list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag11);
					list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag12);
					if (flag12)
					{
						RaycastHit hitInfo3;
						Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out hitInfo3);
						if (MenuPatch.pointer == null)
						{
							MenuPatch.pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<SphereCollider>());
							MenuPatch.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
						}
						MenuPatch.pointer.transform.position = hitInfo3.point;
						Photon.Realtime.Player targetPlayer2;
						bool flag13 = GorillaTagger.Instance.TryToTag(hitInfo3, true, out targetPlayer2);
						if (flag11 && !flag13)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
							GorillaGameManager.instance.photonView.RPC("JoinPubWithFreinds", targetPlayer2, Array.Empty<object>());
						}
						else if (!flag11 && flag13)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
							GorillaGameManager.instance.photonView.RPC("JoinPubWithFreinds", targetPlayer2, Array.Empty<object>());
						}
						else if (flag11 && flag13)
						{
							MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
							GorillaGameManager.instance.photonView.RPC("JoinPubWithFreinds", targetPlayer2, Array.Empty<object>());
						}
					}
					else
					{
						UnityEngine.Object.Destroy(MenuPatch.pointer);
						MenuPatch.pointer = null;
					}
				}
				flag = MenuPatch.buttonsActive[8];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessHotRod();
				}
				flag = MenuPatch.buttonsActive2[0];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					foreach (VRRig vrrig in (VRRig[])UnityEngine.Object.FindObjectsOfType(typeof(VRRig)))
					{
						if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer && !vrrig.photonView.IsMine)
						{
							GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
							UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
							UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
							gameObject.transform.rotation = Quaternion.identity;
							gameObject.transform.localScale = new Vector3(0.04f, 200f, 0.04f);
							gameObject.transform.position = vrrig.transform.position;
							gameObject.GetComponent<MeshRenderer>().material = vrrig.mainSkin.material;
							UnityEngine.Object.Destroy(gameObject, Time.deltaTime);
						}
					}
				}
				flag = MenuPatch.buttonsActive2[1];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessPlatformMonke();
				}
				flag = MenuPatch.buttonsActive2[2];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessRGB();
				}
				bool flag14 = false;
				flag = MenuPatch.buttonsActive2[3];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessLongArms();
				}
				else if (!flag14)
				{
					GorillaLocomotion.Player.Instance.maxArmLength = MenuPatch.maxArmLengthInitial.Value;
					GorillaLocomotion.Player.Instance.leftHandOffset = MenuPatch.leftHandOffsetInitial.Value;
					GorillaLocomotion.Player.Instance.rightHandOffset = MenuPatch.rightHandOffsetInitial.Value;
				}
				flag = MenuPatch.buttonsActive2[4];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page++;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw3();
				}
				flag = MenuPatch.buttonsActive2[5];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page--;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw();
				}
				flag = MenuPatch.buttonsActive2[6];
				flag14 = true;
				if ((flag.GetValueOrDefault() == flag14) & (flag != null))
				{
					GorillaLocomotion.Player.Instance.transform.localScale = new Vector3(2f, 2f, 2f);
				}
				else
				{
					GorillaLocomotion.Player.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				flag = MenuPatch.buttonsActive[4];
				flag14 = true;
				if (((flag.GetValueOrDefault() == flag14) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					int num = new System.Random().Next(MenuPatch.randomNames.Length);
					string text = MenuPatch.randomNames[num];
					PhotonNetwork.LocalPlayer.NickName = text;
					PhotonNetwork.NickName = text;
					PlayerPrefs.SetString("playerName", text);
					GorillaComputer.instance.currentName = text;
					GorillaComputer.instance.offlineVRRigNametagText.text = text;
					PlayerPrefs.Save();
					MenuPatch.color = UnityEngine.Random.ColorHSV(0f, 1f, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value);
					GorillaTagger.Instance.UpdateColor(MenuPatch.color.r, MenuPatch.color.g, MenuPatch.color.b);
					GorillaTagger.Instance.myVRRig.photonView.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						MenuPatch.color.r,
						MenuPatch.color.g,
						MenuPatch.color.b
					});
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw2();
				}
				flag = MenuPatch.buttonsActive3[0];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessNoclip();
				}
				flag = MenuPatch.buttonsActive3[1];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessTeleportGun();
				}
				flag = MenuPatch.buttonsActive3[2];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag15 = false;
					bool flag16 = false;
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
					list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag15);
					list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag16);
					if (flag16)
					{
						RaycastHit hitInfo4;
						Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out hitInfo4);
						if (MenuPatch.pointer == null)
						{
							MenuPatch.pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<Rigidbody>());
							UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<SphereCollider>());
							MenuPatch.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
						}
						MenuPatch.pointer.transform.position = hitInfo4.point;
						Photon.Realtime.Player player3;
						GorillaTagger.Instance.TryToTag(hitInfo4, true, out player3);
						MenuPatch.pointer.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
						Hashtable hashtable = new Hashtable();
						hashtable.Add("matIndex", MenuPatch.DefaultMaterial);
						player3.SetCustomProperties(hashtable, null, null);
						MenuPatch.DefaultMaterial++;
						if (MenuPatch.DefaultMaterial >= 7)
						{
							MenuPatch.DefaultMaterial = 0;
						}
					}
				}
				flag = MenuPatch.buttonsActive3[3];
				flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					GorillaLocomotion.Player.Instance.slideControl = 0.00425f;
				}
				else
				{
					GorillaLocomotion.Player.Instance.slideControl = 1f;
				}
				flag = MenuPatch.buttonsActive3[4];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page++;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw4();
				}
				flag = MenuPatch.buttonsActive3[5];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page--;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw2();
				}
				flag = MenuPatch.buttonsActive3[6];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					Application.Quit();
				}
				flag = MenuPatch.buttonsActive3[7];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.updateTimer += Time.deltaTime;
					MenuPatch.color = UnityEngine.Random.ColorHSV(0f, 1f, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value);
					MenuPatch.timer = Time.time + ModMenuPatch.cycleSpeed.Value;
					if ((double)MenuPatch.updateTimer > (double)MenuPatch.updateRate)
					{
						MenuPatch.updateTimer = 0f;
						GorillaTagger.Instance.UpdateColor(MenuPatch.color.r, MenuPatch.color.g, MenuPatch.color.b);
						GorillaTagger.Instance.myVRRig.photonView.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
						{
							MenuPatch.color.r,
							MenuPatch.color.g,
							MenuPatch.color.b
						});
					}
				}
				flag = MenuPatch.buttonsActive2[8];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag17 = false;
					bool flag18 = false;
					list = new List<InputDevice>();
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, list);
					list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag17);
					list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out flag18);
					int num2 = 0;
					if (flag17)
					{
						GorillaTagger.Instance.myVRRig.transform.position = new Vector3(100f, 100f, 100f);
						GorillaTagger.Instance.myVRRig.enabled = false;
					}
					else if (num2 < 15)
					{
						num2++;
						GorillaTagger.Instance.myVRRig.enabled = true;
					}
				}
				flag = MenuPatch.buttonsActive4[0];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessCarMonke();
				}
				flag = MenuPatch.buttonsActive4[1];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					GorillaTagger.Instance.myVRRig.enabled = true;
				}
				flag = MenuPatch.buttonsActive4[3];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessSpiderMonke();
				}
				flag = MenuPatch.buttonsActive4[4];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page++;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw5();
				}
				flag = MenuPatch.buttonsActive4[5];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page--;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw3();
				}
				flag = MenuPatch.buttonsActive4[6];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/MOD STICKRIGHT.").SetActive(true);
				}
				else
				{
					GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/MOD STICKRIGHT.").SetActive(false);
				}
				flag = MenuPatch.buttonsActive5[0];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					List<InputDevice> list2 = new List<InputDevice>();
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list2);
					InputDevice inputDevice = list2[0];
					inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out MenuPatch.SpawnGrip);
					inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out MenuPatch.BoomGrip);
					if (MenuPatch.SpawnGrip && !MenuPatch.spawned)
					{
						MenuPatch.C4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
						UnityEngine.Object.Destroy(MenuPatch.C4.GetComponent<Rigidbody>());
						UnityEngine.Object.Destroy(MenuPatch.C4.GetComponent<BoxCollider>());
						MenuPatch.C4.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
						MenuPatch.Fuze = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
						UnityEngine.Object.Destroy(MenuPatch.Fuze.GetComponent<Rigidbody>());
						UnityEngine.Object.Destroy(MenuPatch.Fuze.GetComponent<BoxCollider>());
						MenuPatch.Fuze.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
						MenuPatch.C4.transform.position = GorillaLocomotion.Player.Instance.rightHandTransform.transform.position;
						MenuPatch.C4.transform.localScale = new Vector3(0.2f, 0.1f, 0.4f);
						MenuPatch.Fuze.transform.position = GorillaLocomotion.Player.Instance.rightHandTransform.transform.position;
						MenuPatch.Fuze.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
						MenuPatch.spawned = true;
					}
					if (MenuPatch.spawned && MenuPatch.BoomGrip)
					{
						GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddExplosionForce(100f, MenuPatch.C4.transform.position, 10f, 2f);
						UnityEngine.Object.Destroy(MenuPatch.C4);
						UnityEngine.Object.Destroy(MenuPatch.Fuze);
						MenuPatch.spawned = false;
					}
				}
				flag = MenuPatch.buttonsActive5[1];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					bool flag19 = false;
					List<InputDevice> list3 = new List<InputDevice>();
					InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list3);
					list3[0].TryGetFeatureValue(CommonUsages.primaryButton, out flag19);
					if (!flag19)
					{
						MenuPatch.ghostToggled = false;
					}
					else if (!MenuPatch.ghostToggled && GorillaTagger.Instance.myVRRig.enabled)
					{
						GorillaTagger.Instance.myVRRig.enabled = false;
						MenuPatch.ghostToggled = true;
					}
					else if (!MenuPatch.ghostToggled && !GorillaTagger.Instance.myVRRig.enabled)
					{
						GorillaTagger.Instance.myVRRig.enabled = true;
						MenuPatch.ghostToggled = true;
					}
				}
				flag = MenuPatch.buttonsActive5[2];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					MenuPatch.ProcessNoclip();
				}
				flag = MenuPatch.buttonsActive5[3];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					PhotonNetwork.Disconnect();
				}
				flag = MenuPatch.buttonsActive5[4];
				flag.GetValueOrDefault();
				bool flag20 = flag != null;
				flag = MenuPatch.buttonsActive5[5];
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && MenuPatch.btnCooldown == 0)
				{
					MenuPatch.btnCooldown = Time.frameCount + 30;
					MenuPatch.page--;
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					MenuPatch.Draw4();
				}
				if (MenuPatch.btnCooldown > 0 && Time.frameCount > MenuPatch.btnCooldown)
				{
					MenuPatch.btnCooldown = 0;
					MenuPatch.buttonsActive[3] = new bool?(false);
					MenuPatch.buttonsActive[5] = new bool?(false);
					MenuPatch.buttonsActive2[4] = new bool?(false);
					MenuPatch.buttonsActive2[5] = new bool?(false);
					MenuPatch.buttonsActive3[4] = new bool?(false);
					MenuPatch.buttonsActive3[5] = new bool?(false);
					MenuPatch.buttonsActive4[4] = new bool?(false);
					MenuPatch.buttonsActive4[5] = new bool?(false);
					MenuPatch.buttonsActive5[4] = new bool?(false);
					MenuPatch.buttonsActive5[5] = new bool?(false);
					UnityEngine.Object.Destroy(MenuPatch.menu);
					MenuPatch.menu = null;
					if (MenuPatch.page == 0)
					{
						MenuPatch.Draw();
					}
					if (MenuPatch.page == 1)
					{
						MenuPatch.Draw2();
					}
					if (MenuPatch.page == 2)
					{
						MenuPatch.Draw3();
					}
					if (MenuPatch.page == 3)
					{
						MenuPatch.Draw4();
					}
					if (MenuPatch.page == 4)
					{
						MenuPatch.Draw5();
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText("infinitemenu_error.log", ex.ToString());
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00003ED0 File Offset: 0x000020D0
		private static void ProcessNoclip()
		{
			bool flag = false;
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, list);
			list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag);
			if (flag)
			{
				if (!MenuPatch.flag2)
				{
					foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>())
					{
						meshCollider.transform.localScale = meshCollider.transform.localScale / 10000f;
					}
					MenuPatch.flag2 = true;
					MenuPatch.flag1 = false;
					return;
				}
			}
			else if (!MenuPatch.flag1)
			{
				foreach (MeshCollider meshCollider2 in Resources.FindObjectsOfTypeAll<MeshCollider>())
				{
					meshCollider2.transform.localScale = meshCollider2.transform.localScale * 10000f;
				}
				MenuPatch.flag1 = true;
				MenuPatch.flag2 = false;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00003FB4 File Offset: 0x000021B4
		private static void ProcessPlatformMonke()
		{
			if (!MenuPatch.once_networking)
			{
				PhotonNetwork.NetworkingClient.EventReceived += MenuPatch.PlatformNetwork;
				MenuPatch.once_networking = true;
			}
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, list);
			list[0].TryGetFeatureValue(CommonUsages.gripButton, out MenuPatch.gripDown_left);
			InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
			list[0].TryGetFeatureValue(CommonUsages.gripButton, out MenuPatch.gripDown_right);
			if (MenuPatch.gripDown_right)
			{
				if (!MenuPatch.once_right && MenuPatch.jump_right_local == null)
				{
					MenuPatch.jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
					MenuPatch.jump_right_local.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
					MenuPatch.jump_right_local.transform.localScale = MenuPatch.scale;
					MenuPatch.jump_right_local.transform.position = new Vector3(0f, -0.0075f, 0f) + GorillaLocomotion.Player.Instance.rightHandTransform.position;
					MenuPatch.jump_right_local.transform.rotation = GorillaLocomotion.Player.Instance.rightHandTransform.rotation;
					object[] eventContent = new object[]
					{
						new Vector3(0f, -0.0075f, 0f) + GorillaLocomotion.Player.Instance.rightHandTransform.position,
						GorillaLocomotion.Player.Instance.rightHandTransform.rotation
					};
					RaiseEventOptions raiseEventOptions = new RaiseEventOptions
					{
						Receivers = ReceiverGroup.Others
					};
					PhotonNetwork.RaiseEvent(70, eventContent, raiseEventOptions, SendOptions.SendReliable);
					MenuPatch.once_right = true;
					MenuPatch.once_right_false = false;
				}
			}
			else if (!MenuPatch.once_right_false && MenuPatch.jump_right_local != null)
			{
				UnityEngine.Object.Destroy(MenuPatch.jump_right_local);
				MenuPatch.jump_right_local = null;
				MenuPatch.once_right = false;
				MenuPatch.once_right_false = true;
				RaiseEventOptions raiseEventOptions2 = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others
				};
				PhotonNetwork.RaiseEvent(72, null, raiseEventOptions2, SendOptions.SendReliable);
			}
			if (MenuPatch.gripDown_left)
			{
				if (!MenuPatch.once_left && MenuPatch.jump_left_local == null)
				{
					MenuPatch.jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
					MenuPatch.jump_left_local.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
					MenuPatch.jump_left_local.transform.localScale = MenuPatch.scale;
					MenuPatch.jump_left_local.transform.position = GorillaLocomotion.Player.Instance.leftHandTransform.position;
					MenuPatch.jump_left_local.transform.rotation = GorillaLocomotion.Player.Instance.leftHandTransform.rotation;
					object[] eventContent2 = new object[]
					{
						GorillaLocomotion.Player.Instance.leftHandTransform.position,
						GorillaLocomotion.Player.Instance.leftHandTransform.rotation
					};
					RaiseEventOptions raiseEventOptions3 = new RaiseEventOptions
					{
						Receivers = ReceiverGroup.Others
					};
					PhotonNetwork.RaiseEvent(69, eventContent2, raiseEventOptions3, SendOptions.SendReliable);
					MenuPatch.once_left = true;
					MenuPatch.once_left_false = false;
				}
			}
			else if (!MenuPatch.once_left_false && MenuPatch.jump_left_local != null)
			{
				UnityEngine.Object.Destroy(MenuPatch.jump_left_local);
				MenuPatch.jump_left_local = null;
				MenuPatch.once_left = false;
				MenuPatch.once_left_false = true;
				RaiseEventOptions raiseEventOptions4 = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others
				};
				PhotonNetwork.RaiseEvent(71, null, raiseEventOptions4, SendOptions.SendReliable);
			}
			if (!PhotonNetwork.InRoom)
			{
				for (int i = 0; i < MenuPatch.jump_right_network.Length; i++)
				{
					UnityEngine.Object.Destroy(MenuPatch.jump_right_network[i]);
				}
				for (int j = 0; j < MenuPatch.jump_left_network.Length; j++)
				{
					UnityEngine.Object.Destroy(MenuPatch.jump_left_network[j]);
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000434C File Offset: 0x0000254C
		private static void PlatformNetwork(EventData eventData)
		{
			byte code = eventData.Code;
			if (code == 69)
			{
				object[] array = (object[])eventData.CustomData;
				MenuPatch.jump_left_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				MenuPatch.jump_left_network[eventData.Sender].GetComponent<Renderer>().material.SetColor("_Color", Color.black);
				MenuPatch.jump_left_network[eventData.Sender].transform.localScale = MenuPatch.scale;
				MenuPatch.jump_left_network[eventData.Sender].transform.position = (Vector3)array[0];
				MenuPatch.jump_left_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
				return;
			}
			if (code == 70)
			{
				object[] array2 = (object[])eventData.CustomData;
				MenuPatch.jump_right_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				MenuPatch.jump_right_network[eventData.Sender].GetComponent<Renderer>().material.SetColor("_Color", Color.black);
				MenuPatch.jump_right_network[eventData.Sender].transform.localScale = MenuPatch.scale;
				MenuPatch.jump_right_network[eventData.Sender].transform.position = (Vector3)array2[0];
				MenuPatch.jump_right_network[eventData.Sender].transform.rotation = (Quaternion)array2[1];
				return;
			}
			if (code == 71)
			{
				UnityEngine.Object.Destroy(MenuPatch.jump_left_network[eventData.Sender]);
				MenuPatch.jump_left_network[eventData.Sender] = null;
				return;
			}
			if (code == 72)
			{
				UnityEngine.Object.Destroy(MenuPatch.jump_right_network[eventData.Sender]);
				MenuPatch.jump_right_network[eventData.Sender] = null;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000044F0 File Offset: 0x000026F0
		private static void ProcessRGB()
		{
			MenuPatch.updateTimer += Time.deltaTime;
			if (!ModMenuPatch.randomColor.Value)
			{
				if ((double)MenuPatch.hue >= 1.0)
				{
					MenuPatch.hue = 0f;
				}
				MenuPatch.hue += ModMenuPatch.cycleSpeed.Value;
				MenuPatch.color = Color.HSVToRGB(MenuPatch.hue, 1f * ModMenuPatch.glowAmount.Value, 1f * ModMenuPatch.glowAmount.Value);
			}
			else if ((double)Time.time > (double)MenuPatch.timer)
			{
				MenuPatch.color = UnityEngine.Random.ColorHSV(0f, 1f, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value, ModMenuPatch.glowAmount.Value);
				MenuPatch.timer = Time.time + ModMenuPatch.cycleSpeed.Value;
			}
			if ((double)MenuPatch.updateTimer > (double)MenuPatch.updateRate)
			{
				MenuPatch.updateTimer = 0f;
				GorillaTagger.Instance.UpdateColor(MenuPatch.color.r, MenuPatch.color.g, MenuPatch.color.b);
				GorillaTagger.Instance.myVRRig.photonView.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					MenuPatch.color.r,
					MenuPatch.color.g,
					MenuPatch.color.b
				});
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004674 File Offset: 0x00002874
		private static void ProcessTeleportGun()
		{
			bool flag = false;
			bool flag2 = false;
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevices(list);
			InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
			list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag);
			list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag2);
			if (!flag2)
			{
				UnityEngine.Object.Destroy(MenuPatch.pointer);
				MenuPatch.pointer = null;
				MenuPatch.antiRepeat = false;
				return;
			}
			RaycastHit raycastHit;
			Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out raycastHit);
			if (MenuPatch.pointer == null)
			{
				MenuPatch.pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<Rigidbody>());
				UnityEngine.Object.Destroy(MenuPatch.pointer.GetComponent<SphereCollider>());
				MenuPatch.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			MenuPatch.pointer.transform.position = raycastHit.point;
			if (!flag)
			{
				MenuPatch.antiRepeat = false;
				return;
			}
			if (!MenuPatch.antiRepeat)
			{
				GorillaLocomotion.Player.Instance.transform.position = raycastHit.point;
				GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
				MenuPatch.antiRepeat = true;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000047E4 File Offset: 0x000029E4
		private static void ProcessCarMonke()
		{
			if (!MenuPatch.Start)
			{
				ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Bananacfg"), true);
				ModMenuPatch.Acceleration_con = configFile.Bind<float>("Banana-Car Settings", "Acceleration (m/s/s)", 5f, "The speed added per second while driving");
				MenuPatch.acceleration = ModMenuPatch.Acceleration_con.Value;
				ModMenuPatch.Max_con = configFile.Bind<float>("Banana-Car Settings", "Max-Speed", 7.5f, "The max amount of speed you can get while driving");
				MenuPatch.maxs = ModMenuPatch.Max_con.Value;
				ModMenuPatch.multi = configFile.Bind<float>("Banana-Car Settings", "Multiplier", 1f, "The speed multiplier (not necessary to have here but might as well add it)");
				MenuPatch.multiplier = ModMenuPatch.multi.Value;
				MenuPatch.Start = true;
			}
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevices(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
				{
					list[i].TryGetFeatureValue(CommonUsages.primary2DAxis, out MenuPatch.left_joystick);
				}
				list[i].characteristics.HasFlag(InputDeviceCharacteristics.Right);
			}
			RaycastHit raycastHit;
			Physics.Raycast(GorillaLocomotion.Player.Instance.bodyCollider.transform.position, Vector3.down, out raycastHit, 100f, MenuPatch.layers);
			MenuPatch.head_direction = GorillaLocomotion.Player.Instance.headCollider.transform.forward;
			MenuPatch.roll_direction = Vector3.ProjectOnPlane(MenuPatch.head_direction, raycastHit.normal);
			if (MenuPatch.left_joystick.y != 0f)
			{
				if (MenuPatch.left_joystick.y < 0f)
				{
					if (MenuPatch.speed > -MenuPatch.maxs)
					{
						MenuPatch.speed -= MenuPatch.acceleration * Math.Abs(MenuPatch.left_joystick.y) * Time.deltaTime;
					}
				}
				else if (MenuPatch.speed < MenuPatch.maxs)
				{
					MenuPatch.speed += MenuPatch.acceleration * Math.Abs(MenuPatch.left_joystick.y) * Time.deltaTime;
				}
			}
			else if (MenuPatch.speed < 0f)
			{
				MenuPatch.speed += MenuPatch.acceleration * Time.deltaTime * 0.5f;
			}
			else if (MenuPatch.speed > 0f)
			{
				MenuPatch.speed -= MenuPatch.acceleration * Time.deltaTime * 0.5f;
			}
			if (MenuPatch.speed > MenuPatch.maxs)
			{
				MenuPatch.speed = MenuPatch.maxs;
			}
			if (MenuPatch.speed < -MenuPatch.maxs)
			{
				MenuPatch.speed = -MenuPatch.maxs;
			}
			if (MenuPatch.speed != 0f && raycastHit.distance < MenuPatch.distance)
			{
				GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity = MenuPatch.roll_direction.normalized * MenuPatch.speed * MenuPatch.multiplier;
			}
			if (GorillaLocomotion.Player.Instance.IsHandTouching(true) || GorillaLocomotion.Player.Instance.IsHandTouching(false))
			{
				MenuPatch.speed *= 0.75f;
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004B04 File Offset: 0x00002D04
		private static void ProcessSpiderMonke()
		{
			if (MenuPatch.inAllowedRoom)
			{
				MenuPatch.start = true;
				List<InputDevice> list = new List<InputDevice>();
				InputDevices.GetDevices(list);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
					{
						list[i].TryGetFeatureValue(CommonUsages.trigger, out MenuPatch.lefttriggerpressed);
					}
					if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Right))
					{
						list[i].TryGetFeatureValue(CommonUsages.trigger, out MenuPatch.triggerpressed);
					}
				}
				if (!MenuPatch.wackstart)
				{
					GameObject gameObject = new GameObject();
					MenuPatch.Spring = ModMenuPatch.sp.Value;
					MenuPatch.Damper = ModMenuPatch.dp.Value;
					MenuPatch.MassScale = ModMenuPatch.ms.Value;
					MenuPatch.grapplecolor = ModMenuPatch.rc.Value;
					MenuPatch.lr = GorillaLocomotion.Player.Instance.gameObject.AddComponent<LineRenderer>();
					MenuPatch.lr.material = new Material(Shader.Find("Sprites/Default"));
					MenuPatch.lr.startColor = MenuPatch.grapplecolor;
					MenuPatch.lr.endColor = MenuPatch.grapplecolor;
					MenuPatch.lr.startWidth = 0.02f;
					MenuPatch.lr.endWidth = 0.02f;
					MenuPatch.leftlr = gameObject.AddComponent<LineRenderer>();
					MenuPatch.leftlr.material = new Material(Shader.Find("Sprites/Default"));
					MenuPatch.leftlr.startColor = MenuPatch.grapplecolor;
					MenuPatch.leftlr.endColor = MenuPatch.grapplecolor;
					MenuPatch.leftlr.startWidth = 0.02f;
					MenuPatch.leftlr.endWidth = 0.02f;
					MenuPatch.wackstart = true;
				}
				MenuPatch.DrawRope(GorillaLocomotion.Player.Instance);
				MenuPatch.LeftDrawRope(GorillaLocomotion.Player.Instance);
				if (MenuPatch.triggerpressed > 0.1f)
				{
					if (MenuPatch.cangrapple)
					{
						MenuPatch.Spring = ModMenuPatch.sp.Value;
						MenuPatch.StartGrapple(GorillaLocomotion.Player.Instance);
						MenuPatch.cangrapple = false;
					}
				}
				else
				{
					MenuPatch.StopGrapple(GorillaLocomotion.Player.Instance);
				}
				if (MenuPatch.triggerpressed > 0.1f && MenuPatch.lefttriggerpressed > 0.1f)
				{
					MenuPatch.Spring /= 2f;
				}
				else
				{
					MenuPatch.Spring = ModMenuPatch.sp.Value;
				}
				if (MenuPatch.lefttriggerpressed > 0.1f)
				{
					if (MenuPatch.canleftgrapple)
					{
						MenuPatch.Spring = ModMenuPatch.sp.Value;
						MenuPatch.LeftStartGrapple(GorillaLocomotion.Player.Instance);
						MenuPatch.canleftgrapple = false;
						return;
					}
				}
				else
				{
					MenuPatch.LeftStopGrapple();
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00004DA0 File Offset: 0x00002FA0
		public static void StartGrapple(GorillaLocomotion.Player __instance)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(__instance.rightHandTransform.position, __instance.rightHandTransform.forward, out raycastHit, MenuPatch.maxDistance))
			{
				MenuPatch.grapplePoint = raycastHit.point;
				MenuPatch.joint = __instance.gameObject.AddComponent<SpringJoint>();
				MenuPatch.joint.autoConfigureConnectedAnchor = false;
				MenuPatch.joint.connectedAnchor = MenuPatch.grapplePoint;
				float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, MenuPatch.grapplePoint);
				MenuPatch.joint.maxDistance = num * 0.8f;
				MenuPatch.joint.minDistance = num * 0.25f;
				MenuPatch.joint.spring = MenuPatch.Spring;
				MenuPatch.joint.damper = MenuPatch.Damper;
				MenuPatch.joint.massScale = MenuPatch.MassScale;
				MenuPatch.lr.positionCount = 2;
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000208E File Offset: 0x0000028E
		public static void DrawRope(GorillaLocomotion.Player __instance)
		{
			if (MenuPatch.joint)
			{
				MenuPatch.lr.SetPosition(0, __instance.rightHandTransform.position);
				MenuPatch.lr.SetPosition(1, MenuPatch.grapplePoint);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000020C2 File Offset: 0x000002C2
		public static void StopGrapple(GorillaLocomotion.Player __instance)
		{
			MenuPatch.lr.positionCount = 0;
			UnityEngine.Object.Destroy(MenuPatch.joint);
			MenuPatch.cangrapple = true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00004E80 File Offset: 0x00003080
		public static void LeftStartGrapple(GorillaLocomotion.Player __instance)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(__instance.leftHandTransform.position, __instance.leftHandTransform.forward, out raycastHit, MenuPatch.maxDistance))
			{
				MenuPatch.leftgrapplePoint = raycastHit.point;
				MenuPatch.leftjoint = __instance.gameObject.AddComponent<SpringJoint>();
				MenuPatch.leftjoint.autoConfigureConnectedAnchor = false;
				MenuPatch.leftjoint.connectedAnchor = MenuPatch.leftgrapplePoint;
				float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, MenuPatch.leftgrapplePoint);
				MenuPatch.leftjoint.maxDistance = num * 0.8f;
				MenuPatch.leftjoint.minDistance = num * 0.25f;
				MenuPatch.leftjoint.spring = MenuPatch.Spring;
				MenuPatch.leftjoint.damper = MenuPatch.Damper;
				MenuPatch.leftjoint.massScale = MenuPatch.MassScale;
				MenuPatch.leftlr.positionCount = 2;
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000020DF File Offset: 0x000002DF
		public static void LeftDrawRope(GorillaLocomotion.Player __instance)
		{
			if (MenuPatch.leftjoint)
			{
				MenuPatch.leftlr.SetPosition(0, __instance.leftHandTransform.position);
				MenuPatch.leftlr.SetPosition(1, MenuPatch.leftgrapplePoint);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002113 File Offset: 0x00000313
		public static void LeftStopGrapple()
		{
			MenuPatch.leftlr.positionCount = 0;
			UnityEngine.Object.Destroy(MenuPatch.leftjoint);
			MenuPatch.canleftgrapple = true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00004F60 File Offset: 0x00003160
		private static void ProcessLongArms()
		{
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, list);
			list[0].TryGetFeatureValue(CommonUsages.gripButton, out MenuPatch.gain);
			list[0].TryGetFeatureValue(CommonUsages.triggerButton, out MenuPatch.less);
			list[0].TryGetFeatureValue(CommonUsages.primaryButton, out MenuPatch.reset);
			list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out MenuPatch.fastr);
			MenuPatch.timeSinceLastChange += Time.deltaTime;
			if (MenuPatch.timeSinceLastChange > 0.2f)
			{
				GorillaLocomotion.Player.Instance.leftHandOffset = new Vector3(0f, MenuPatch.myVarY1, 0f);
				GorillaLocomotion.Player.Instance.rightHandOffset = new Vector3(0f, MenuPatch.myVarY2, 0f);
				GorillaLocomotion.Player.Instance.maxArmLength = 200f;
				if (MenuPatch.gain)
				{
					MenuPatch.timeSinceLastChange = 0f;
					MenuPatch.myVarY1 += MenuPatch.gainSpeed;
					MenuPatch.myVarY2 += MenuPatch.gainSpeed;
					if (MenuPatch.myVarY1 >= 201f)
					{
						MenuPatch.myVarY1 = 200f;
						MenuPatch.myVarY2 = 200f;
					}
				}
				if (MenuPatch.less)
				{
					MenuPatch.timeSinceLastChange = 0f;
					MenuPatch.myVarY1 -= MenuPatch.gainSpeed;
					MenuPatch.myVarY2 -= MenuPatch.gainSpeed;
					if (MenuPatch.myVarY2 <= -6f)
					{
						MenuPatch.myVarY1 = -5f;
						MenuPatch.myVarY2 = -5f;
					}
				}
				if (MenuPatch.reset)
				{
					MenuPatch.timeSinceLastChange = 0f;
					MenuPatch.myVarY1 = 0f;
					MenuPatch.myVarY2 = 0f;
				}
				if (MenuPatch.fastr && MenuPatch.myVarY1 == 5f)
				{
					MenuPatch.myVarY1 = 10f;
					MenuPatch.myVarY2 = 10f;
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00005140 File Offset: 0x00003340
		private static void AddButton(float offset, string text)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<BtnCollider>().relatedText = text;
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons.Length; i++)
			{
				if (text == MenuPatch.buttons[i])
				{
					num = i;
					break;
				}
			}
			bool? flag = MenuPatch.buttonsActive[num];
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			}
			else
			{
				flag = MenuPatch.buttonsActive[num];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				}
				else
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
				}
			}
			Text text2 = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.text = text;
			text2.fontSize = 1;
			text2.alignment = TextAnchor.MiddleCenter;
			text2.resizeTextForBestFit = true;
			text2.resizeTextMinSize = 0;
			RectTransform component = text2.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00005368 File Offset: 0x00003568
		private static void AddButton2(float offset, string text)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<BtnCollider2>().relatedText = text;
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons2.Length; i++)
			{
				if (text == MenuPatch.buttons2[i])
				{
					num = i;
					break;
				}
			}
			bool? flag = MenuPatch.buttonsActive2[num];
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			}
			else
			{
				flag = MenuPatch.buttonsActive2[num];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				}
				else
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
				}
			}
			Text text2 = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.text = text;
			text2.fontSize = 1;
			text2.alignment = TextAnchor.MiddleCenter;
			text2.resizeTextForBestFit = true;
			text2.resizeTextMinSize = 0;
			RectTransform component = text2.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00005590 File Offset: 0x00003790
		private static void AddButton3(float offset, string text)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<BtnCollider3>().relatedText = text;
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons3.Length; i++)
			{
				if (text == MenuPatch.buttons3[i])
				{
					num = i;
					break;
				}
			}
			bool? flag = MenuPatch.buttonsActive3[num];
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			}
			else
			{
				flag = MenuPatch.buttonsActive3[num];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				}
				else
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
				}
			}
			Text text2 = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.text = text;
			text2.fontSize = 1;
			text2.alignment = TextAnchor.MiddleCenter;
			text2.resizeTextForBestFit = true;
			text2.resizeTextMinSize = 0;
			RectTransform component = text2.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000057B8 File Offset: 0x000039B8
		private static void AddButton4(float offset, string text)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<BtnCollider4>().relatedText = text;
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons4.Length; i++)
			{
				if (text == MenuPatch.buttons4[i])
				{
					num = i;
					break;
				}
			}
			bool? flag = MenuPatch.buttonsActive4[num];
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			}
			else
			{
				flag = MenuPatch.buttonsActive4[num];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				}
				else
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
				}
			}
			Text text2 = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.text = text;
			text2.fontSize = 1;
			text2.alignment = TextAnchor.MiddleCenter;
			text2.resizeTextForBestFit = true;
			text2.resizeTextMinSize = 0;
			RectTransform component = text2.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000059E0 File Offset: 0x00003BE0
		private static void AddButton5(float offset, string text)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<BtnCollider5>().relatedText = text;
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons5.Length; i++)
			{
				if (text == MenuPatch.buttons5[i])
				{
					num = i;
					break;
				}
			}
			bool? flag = MenuPatch.buttonsActive5[num];
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			}
			else
			{
				flag = MenuPatch.buttonsActive5[num];
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				}
				else
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
				}
			}
			Text text2 = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.text = text;
			text2.fontSize = 1;
			text2.alignment = TextAnchor.MiddleCenter;
			text2.resizeTextForBestFit = true;
			text2.resizeTextMinSize = 0;
			RectTransform component = text2.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 3.05f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00005C08 File Offset: 0x00003E08
		public static void Draw()
		{
			MenuPatch.menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
			MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.4f);
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			gameObject.transform.position = new Vector3(0.05f, 0f, -0.05f);
			MenuPatch.canvasObj = new GameObject();
			MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
			Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
			CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
			MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvasScaler.dynamicPixelsPerUnit = 1000f;
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Infinite Mods Menu v1";
			text.fontSize = 1;
			text.alignment = TextAnchor.MiddleCenter;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.position = new Vector3(0.06f, 0f, 0.175f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			for (int i = 0; i < MenuPatch.buttons.Length; i++)
			{
				MenuPatch.AddButton((float)i * 0.13f, MenuPatch.buttons[i]);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00005E44 File Offset: 0x00004044
		public static void Draw2()
		{
			MenuPatch.menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
			MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.4f);
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			gameObject.transform.position = new Vector3(0.05f, 0f, -0.05f);
			MenuPatch.canvasObj = new GameObject();
			MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
			Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
			CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
			MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvasScaler.dynamicPixelsPerUnit = 1000f;
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Infinite Mods Menu";
			text.fontSize = 1;
			text.alignment = TextAnchor.MiddleCenter;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.28f, 0.05f);
			component.position = new Vector3(0.06f, 0f, 0.175f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			for (int i = 0; i < MenuPatch.buttons2.Length; i++)
			{
				MenuPatch.AddButton2((float)i * 0.13f, MenuPatch.buttons2[i]);
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00006080 File Offset: 0x00004280
		public static void Draw3()
		{
			MenuPatch.menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
			MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.4f);
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			gameObject.transform.position = new Vector3(0.05f, 0f, -0.05f);
			MenuPatch.canvasObj = new GameObject();
			MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
			Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
			CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
			MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvasScaler.dynamicPixelsPerUnit = 1000f;
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Infinite Mods Menu";
			text.fontSize = 1;
			text.alignment = TextAnchor.MiddleCenter;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.28f, 0.05f);
			component.position = new Vector3(0.06f, 0f, 0.175f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			for (int i = 0; i < MenuPatch.buttons3.Length; i++)
			{
				MenuPatch.AddButton3((float)i * 0.13f, MenuPatch.buttons3[i]);
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000062BC File Offset: 0x000044BC
		public static void Draw4()
		{
			MenuPatch.menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
			MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.4f);
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			gameObject.transform.position = new Vector3(0.05f, 0f, -0.05f);
			MenuPatch.canvasObj = new GameObject();
			MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
			Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
			CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
			MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvasScaler.dynamicPixelsPerUnit = 1000f;
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Infinite Mods Menu";
			text.fontSize = 1;
			text.alignment = TextAnchor.MiddleCenter;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.28f, 0.05f);
			component.position = new Vector3(0.06f, 0f, 0.175f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			for (int i = 0; i < MenuPatch.buttons4.Length; i++)
			{
				MenuPatch.AddButton4((float)i * 0.13f, MenuPatch.buttons4[i]);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000064F8 File Offset: 0x000046F8
		public static void Draw5()
		{
			MenuPatch.menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
			UnityEngine.Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
			MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.4f);
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
			gameObject.transform.position = new Vector3(0.05f, 0f, -0.05f);
			MenuPatch.canvasObj = new GameObject();
			MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
			Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
			CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
			MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvasScaler.dynamicPixelsPerUnit = 1000f;
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Infinite Mods Menu";
			text.fontSize = 1;
			text.alignment = TextAnchor.MiddleCenter;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.28f, 0.05f);
			component.position = new Vector3(0.06f, 0f, 0.175f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			for (int i = 0; i < MenuPatch.buttons5.Length; i++)
			{
				MenuPatch.AddButton5((float)i * 0.13f, MenuPatch.buttons5[i]);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00006734 File Offset: 0x00004934
		public static void Toggle(string relatedText)
		{
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons.Length; i++)
			{
				if (relatedText == MenuPatch.buttons[i])
				{
					num = i;
					break;
				}
			}
			if (MenuPatch.buttonsActive[num] != null)
			{
				MenuPatch.buttonsActive[num] = !MenuPatch.buttonsActive[num];
				UnityEngine.Object.Destroy(MenuPatch.menu);
				MenuPatch.menu = null;
				MenuPatch.Draw();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000067CC File Offset: 0x000049CC
		public static void Toggle2(string relatedText)
		{
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons2.Length; i++)
			{
				if (relatedText == MenuPatch.buttons2[i])
				{
					num = i;
					break;
				}
			}
			if (MenuPatch.buttonsActive2[num] != null)
			{
				MenuPatch.buttonsActive2[num] = !MenuPatch.buttonsActive2[num];
				UnityEngine.Object.Destroy(MenuPatch.menu);
				MenuPatch.menu = null;
				MenuPatch.Draw2();
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00006864 File Offset: 0x00004A64
		public static void Toggle3(string relatedText)
		{
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons3.Length; i++)
			{
				if (relatedText == MenuPatch.buttons3[i])
				{
					num = i;
					break;
				}
			}
			if (MenuPatch.buttonsActive3[num] != null)
			{
				MenuPatch.buttonsActive3[num] = !MenuPatch.buttonsActive3[num];
				UnityEngine.Object.Destroy(MenuPatch.menu);
				MenuPatch.menu = null;
				MenuPatch.Draw3();
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000068FC File Offset: 0x00004AFC
		public static void Toggle4(string relatedText)
		{
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons4.Length; i++)
			{
				if (relatedText == MenuPatch.buttons4[i])
				{
					num = i;
					break;
				}
			}
			if (MenuPatch.buttonsActive4[num] != null)
			{
				MenuPatch.buttonsActive4[num] = !MenuPatch.buttonsActive4[num];
				UnityEngine.Object.Destroy(MenuPatch.menu);
				MenuPatch.menu = null;
				MenuPatch.Draw4();
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006994 File Offset: 0x00004B94
		public static void Toggle5(string relatedText)
		{
			int num = -1;
			for (int i = 0; i < MenuPatch.buttons5.Length; i++)
			{
				if (relatedText == MenuPatch.buttons5[i])
				{
					num = i;
					break;
				}
			}
			if (MenuPatch.buttonsActive5[num] != null)
			{
				MenuPatch.buttonsActive5[num] = !MenuPatch.buttonsActive5[num];
				UnityEngine.Object.Destroy(MenuPatch.menu);
				MenuPatch.menu = null;
				MenuPatch.Draw5();
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00006A2C File Offset: 0x00004C2C
		static MenuPatch()
		{
			MenuPatch.buttonsActive5 = new bool?[]
			{
				new bool?(false),
				new bool?(false),
				new bool?(false),
				new bool?(false),
				new bool?(false),
				new bool?(false)
			};
			MenuPatch.menu = null;
			MenuPatch.canvasObj = null;
			MenuPatch.reference = null;
			MenuPatch.framePressCooldown = 0;
			MenuPatch.pointer = null;
			MenuPatch.gravityToggled = false;
			MenuPatch.flying = false;
			MenuPatch.btnCooldown = 0;
			MenuPatch.page = 0;
			MenuPatch.speedPlusCooldown = 0;
			MenuPatch.speedMinusCooldown = 0;
			MenuPatch.maxJumpSpeed = null;
			MenuPatch.jumpMultiplier = null;
			MenuPatch.leftHandOffsetInitial = null;
			MenuPatch.rightHandOffsetInitial = null;
			MenuPatch.maxArmLengthInitial = null;
			MenuPatch.flag2 = false;
			MenuPatch.flag1 = true;
			MenuPatch.antiRepeat = false;
			MenuPatch.DefaultMaterial = 5;
			MenuPatch.spawned = false;
			MenuPatch.C4 = null;
			MenuPatch.Fuze = null;
			MenuPatch.scale = new Vector3(0.0125f, 0.38f, 0.3825f);
			MenuPatch.jump_left_network = new GameObject[9999];
			MenuPatch.jump_right_network = new GameObject[9999];
			MenuPatch.jump_left_local = null;
			MenuPatch.jump_right_local = null;
			MenuPatch.layers = 512;
			MenuPatch.acceleration = 5f;
			MenuPatch.maxs = 10f;
			MenuPatch.distance = 0.35f;
			MenuPatch.multiplier = 1f;
			MenuPatch.speed = 0f;
			MenuPatch.Start = false;
			MenuPatch.cangrapple = true;
			MenuPatch.canleftgrapple = true;
			MenuPatch.wackstart = false;
			MenuPatch.start = true;
			MenuPatch.inAllowedRoom = true;
			MenuPatch.hauntedModMenuEnabled = true;
			MenuPatch.maxDistance = 100f;
			MenuPatch.timeSinceLastChange = 0f;
			MenuPatch.myVarY1 = 0f;
			MenuPatch.myVarY2 = 0f;
			MenuPatch.gain = false;
			MenuPatch.less = false;
			MenuPatch.reset = false;
			MenuPatch.fastr = false;
			MenuPatch.speed1 = true;
			MenuPatch.gainSpeed = 1f;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00006FB4 File Offset: 0x000051B4
		private static void ProcessHotRod()
		{
			if (!MenuPatch.Start)
			{
				ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Bananacfg"), true);
				ModMenuPatch.Acceleration_con = configFile.Bind<float>("Banana-Car Settings", "Acceleration (m/s/s)", 10f, "The speed added per second while driving");
				MenuPatch.acceleration = ModMenuPatch.Acceleration_con.Value;
				ModMenuPatch.Max_con = configFile.Bind<float>("Banana-Car Settings", "Max-Speed", 100f, "The max amount of speed you can get while driving");
				MenuPatch.maxs = ModMenuPatch.Max_con.Value;
				ModMenuPatch.multi = configFile.Bind<float>("Banana-Car Settings", "Multiplier", 10f, "The speed multiplier (not necessary to have here but might as well add it)");
				MenuPatch.multiplier = ModMenuPatch.multi.Value;
				MenuPatch.Start = true;
			}
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevices(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
				{
					list[i].TryGetFeatureValue(CommonUsages.primary2DAxis, out MenuPatch.left_joystick);
				}
				list[i].characteristics.HasFlag(InputDeviceCharacteristics.Right);
			}
			RaycastHit raycastHit;
			Physics.Raycast(GorillaLocomotion.Player.Instance.bodyCollider.transform.position, Vector3.down, out raycastHit, 100f, MenuPatch.layers);
			MenuPatch.head_direction = GorillaLocomotion.Player.Instance.headCollider.transform.forward;
			MenuPatch.roll_direction = Vector3.ProjectOnPlane(MenuPatch.head_direction, raycastHit.normal);
			if (MenuPatch.left_joystick.y != 0f)
			{
				if (MenuPatch.left_joystick.y < 0f)
				{
					if (MenuPatch.speed > -MenuPatch.maxs)
					{
						MenuPatch.speed -= MenuPatch.acceleration * Math.Abs(MenuPatch.left_joystick.y) * Time.deltaTime;
					}
				}
				else if (MenuPatch.speed < MenuPatch.maxs)
				{
					MenuPatch.speed += MenuPatch.acceleration * Math.Abs(MenuPatch.left_joystick.y) * Time.deltaTime;
				}
			}
			else if (MenuPatch.speed < 0f)
			{
				MenuPatch.speed += MenuPatch.acceleration * Time.deltaTime * 0.5f;
			}
			else if (MenuPatch.speed > 0f)
			{
				MenuPatch.speed -= MenuPatch.acceleration * Time.deltaTime * 0.5f;
			}
			if (MenuPatch.speed > MenuPatch.maxs)
			{
				MenuPatch.speed = MenuPatch.maxs;
			}
			if (MenuPatch.speed < -MenuPatch.maxs)
			{
				MenuPatch.speed = -MenuPatch.maxs;
			}
			if (MenuPatch.speed != 0f && raycastHit.distance < MenuPatch.distance)
			{
				GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity = MenuPatch.roll_direction.normalized * MenuPatch.speed * MenuPatch.multiplier;
			}
			if (GorillaLocomotion.Player.Instance.IsHandTouching(true) || GorillaLocomotion.Player.Instance.IsHandTouching(false))
			{
				MenuPatch.speed *= 0.75f;
			}
		}

		// Token: 0x04000013 RID: 19
		public static bool ResetSpeed = false;

		// Token: 0x04000014 RID: 20
		private static string[] buttons = new string[] { "Fly", "Lagger", "Speed", "Removed", "Identity Fraud", "Next Page", "Kicker Gun", "Kicker (half server)", "heater rod" };

		// Token: 0x04000015 RID: 21
		private static bool?[] buttonsActive = new bool?[]
		{
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false)
		};

		// Token: 0x04000016 RID: 22
		private static string[] buttons2 = new string[] { "ESP", "Platform Monke", "Pride Monke", "Long Arms", "Next Page", "Back", "Big Monke [Client]", "No Tag Freeze", "Invisible" };

		// Token: 0x04000017 RID: 23
		private static bool?[] buttonsActive2 = new bool?[]
		{
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false)
		};

		// Token: 0x04000018 RID: 24
		private static string[] buttons3 = new string[] { "Wall Hump", "Teleport", "Material Gun [DW]", "Slide Control", "Next Page", "Back", "Quit Gtag", "Strobe" };

		// Token: 0x04000019 RID: 25
		private static bool?[] buttonsActive3 = new bool?[]
		{
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false)
		};

		// Token: 0x0400001A RID: 26
		private static string[] buttons4 = new string[] { "Car Monke", "Invisible [NW]", "Slip Wall Run", "Spider Monke", "Next Page", "Back", "Mods Stick" };

		// Token: 0x0400001B RID: 27
		private static bool?[] buttonsActive4 = new bool?[]
		{
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false),
			new bool?(false)
		};

		// Token: 0x0400001C RID: 28
		private static string[] buttons5 = new string[] { "C4", "Break Rig", "Removed", "Disconnect", "Next Page", "Back" };

		// Token: 0x0400001D RID: 29
		private static bool?[] buttonsActive5;

		// Token: 0x0400001E RID: 30
		private static bool gripDown;

		// Token: 0x0400001F RID: 31
		private static GameObject menu;

		// Token: 0x04000020 RID: 32
		private static GameObject canvasObj;

		// Token: 0x04000021 RID: 33
		private static GameObject reference;

		// Token: 0x04000022 RID: 34
		public static int framePressCooldown;

		// Token: 0x04000023 RID: 35
		private static GameObject pointer;

		// Token: 0x04000024 RID: 36
		private static bool gravityToggled;

		// Token: 0x04000025 RID: 37
		private static bool flying;

		// Token: 0x04000026 RID: 38
		private static int btnCooldown;

		// Token: 0x04000027 RID: 39
		private static int page;

		// Token: 0x04000028 RID: 40
		private static int speedPlusCooldown;

		// Token: 0x04000029 RID: 41
		private static int speedMinusCooldown;

		// Token: 0x0400002A RID: 42
		private static float? maxJumpSpeed;

		// Token: 0x0400002B RID: 43
		private static float? jumpMultiplier;

		// Token: 0x0400002C RID: 44
		private static Vector3? leftHandOffsetInitial;

		// Token: 0x0400002D RID: 45
		private static Vector3? rightHandOffsetInitial;

		// Token: 0x0400002E RID: 46
		private static float? maxArmLengthInitial;

		// Token: 0x0400002F RID: 47
		private static bool flag2;

		// Token: 0x04000030 RID: 48
		private static bool flag1;

		// Token: 0x04000031 RID: 49
		private static bool antiRepeat;

		// Token: 0x04000032 RID: 50
		private static int DefaultMaterial;

		// Token: 0x04000033 RID: 51
		private static bool spawned;

		// Token: 0x04000034 RID: 52
		private static bool SpawnGrip;

		// Token: 0x04000035 RID: 53
		private static bool BoomGrip;

		// Token: 0x04000036 RID: 54
		private static GameObject C4;

		// Token: 0x04000037 RID: 55
		private static GameObject Fuze;

		// Token: 0x04000038 RID: 56
		private static bool ghostToggled;

		// Token: 0x04000039 RID: 57
		private static Vector3 scale;

		// Token: 0x0400003A RID: 58
		private static bool gripDown_left;

		// Token: 0x0400003B RID: 59
		private static bool gripDown_right;

		// Token: 0x0400003C RID: 60
		private static bool once_left;

		// Token: 0x0400003D RID: 61
		private static bool once_right;

		// Token: 0x0400003E RID: 62
		private static bool once_left_false;

		// Token: 0x0400003F RID: 63
		private static bool once_right_false;

		// Token: 0x04000040 RID: 64
		private static bool once_networking;

		// Token: 0x04000041 RID: 65
		private static GameObject[] jump_left_network;

		// Token: 0x04000042 RID: 66
		private static GameObject[] jump_right_network;

		// Token: 0x04000043 RID: 67
		private static GameObject jump_left_local;

		// Token: 0x04000044 RID: 68
		private static GameObject jump_right_local;

		// Token: 0x04000045 RID: 69
		private static float timer;

		// Token: 0x04000046 RID: 70
		private static float updateTimer;

		// Token: 0x04000047 RID: 71
		private static float hue;

		// Token: 0x04000048 RID: 72
		private static Color color;

		// Token: 0x04000049 RID: 73
		private static float updateRate;

		// Token: 0x0400004A RID: 74
		private static int layers;

		// Token: 0x0400004B RID: 75
		private static Vector3 head_direction;

		// Token: 0x0400004C RID: 76
		private static Vector3 roll_direction;

		// Token: 0x0400004D RID: 77
		private static Vector2 left_joystick;

		// Token: 0x0400004E RID: 78
		private static float acceleration;

		// Token: 0x0400004F RID: 79
		private static float maxs;

		// Token: 0x04000050 RID: 80
		private static float distance;

		// Token: 0x04000051 RID: 81
		private static float multiplier;

		// Token: 0x04000052 RID: 82
		private static float speed;

		// Token: 0x04000053 RID: 83
		private static bool Start;

		// Token: 0x04000054 RID: 84
		public static bool cangrapple;

		// Token: 0x04000055 RID: 85
		public static bool canleftgrapple;

		// Token: 0x04000056 RID: 86
		public static bool wackstart;

		// Token: 0x04000057 RID: 87
		public static bool start;

		// Token: 0x04000058 RID: 88
		public static bool inAllowedRoom;

		// Token: 0x04000059 RID: 89
		public static bool hauntedModMenuEnabled;

		// Token: 0x0400005A RID: 90
		public static float triggerpressed;

		// Token: 0x0400005B RID: 91
		public static float lefttriggerpressed;

		// Token: 0x0400005C RID: 92
		public static float maxDistance;

		// Token: 0x0400005D RID: 93
		public static float Spring;

		// Token: 0x0400005E RID: 94
		public static float Damper;

		// Token: 0x0400005F RID: 95
		public static float MassScale;

		// Token: 0x04000060 RID: 96
		public static Vector3 grapplePoint;

		// Token: 0x04000061 RID: 97
		public static Vector3 leftgrapplePoint;

		// Token: 0x04000062 RID: 98
		public static SpringJoint joint;

		// Token: 0x04000063 RID: 99
		public static SpringJoint leftjoint;

		// Token: 0x04000064 RID: 100
		public static LineRenderer lr;

		// Token: 0x04000065 RID: 101
		public static LineRenderer leftlr;

		// Token: 0x04000066 RID: 102
		public static Color grapplecolor;

		// Token: 0x04000067 RID: 103
		private static float timeSinceLastChange;

		// Token: 0x04000068 RID: 104
		private static float myVarY1;

		// Token: 0x04000069 RID: 105
		private static float myVarY2;

		// Token: 0x0400006A RID: 106
		private static bool gain;

		// Token: 0x0400006B RID: 107
		private static bool less;

		// Token: 0x0400006C RID: 108
		private static bool reset;

		// Token: 0x0400006D RID: 109
		private static bool fastr;

		// Token: 0x0400006E RID: 110
		private static bool speed1;

		// Token: 0x0400006F RID: 111
		private static float gainSpeed;

		// Token: 0x04000070 RID: 112
		public Material[] materialsToChangeTo;

		// Token: 0x04000071 RID: 113
		public float r;

		// Token: 0x04000072 RID: 114
		public float g;

		// Token: 0x04000073 RID: 115
		public float b;

		// Token: 0x04000074 RID: 116
		private static string[] randomNames = new string[] { "monkey", "chimp", "gorilla", "moder", "PBBV", "Daisy09", "jman", "J3VU", "In4init3", "RUN", "bob", "mark", "mike", "riley", "john"};

		// Token: 0x02000006 RID: 6
		public enum PhotonEventCodes
		{
			// Token: 0x04000076 RID: 118
			left_jump_photoncode = 69,
			// Token: 0x04000077 RID: 119
			right_jump_photoncode,
			// Token: 0x04000078 RID: 120
			left_jump_deletion,
			// Token: 0x04000079 RID: 121
			right_jump_deletion
		}

		// Token: 0x02000007 RID: 7
		[BepInPlugin("org.ivy.gtag.gripmonke", "GripMonke", "2.1.1")]
		public class Plugin : BaseUnityPlugin
		{
			// Token: 0x06000030 RID: 48 RVA: 0x00002140 File Offset: 0x00000340
			public void OnEnable()
			{
				MenuPatch.Plugin.harmony = new Harmony("com.ivy.gtag.gripmonke");
				MenuPatch.Plugin.harmony.PatchAll(Assembly.GetExecutingAssembly());
			}

			// Token: 0x0400007A RID: 122
			private static Harmony harmony;

			// Token: 0x02000008 RID: 8
			[HarmonyPatch(typeof(GorillaLocomotion.Player), "GetSlidePercentage")]
			private class slidepatch
			{
				// Token: 0x06000032 RID: 50 RVA: 0x000072D4 File Offset: 0x000054D4
				private static void Postfix(ref float __result)
				{
					bool? flag = MenuPatch.buttonsActive4[2];
					bool flag2 = true;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						__result = 0.03f;
					}
				}
			}
		}
	}
}
