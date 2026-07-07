using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Interaction;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Circuits;
using Il2CppSLZ.Marrow.Combat;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.VFX;
using Il2CppSLZ.Marrow.VoidLogic;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;
using Il2CppSLZ.VFX;
using Il2CppSystem.Collections.Generic;
using LabFusion.Data;
using LabFusion.Downloading;
using LabFusion.Downloading.ModIO;
using LabFusion.Entities;
using LabFusion.Marrow;
using LabFusion.Marrow.Extenders;
using LabFusion.Marrow.Messages;
using LabFusion.Marrow.Pool;
using LabFusion.Marrow.Proxies;
using LabFusion.Marrow.Serialization;
using LabFusion.Menu;
using LabFusion.Network;
using LabFusion.Permissions;
using LabFusion.Player;
using LabFusion.RPC;
using LabFusion.Representation;
using LabFusion.SDK.Gamemodes;
using LabFusion.SDK.Modules;
using LabFusion.SDK.Points;
using LabFusion.Safety;
using LabFusion.Scene;
using LabFusion.Senders;
using LabFusion.UI.Popups;
using LabFusion.Utilities;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace NemesisAntiCheatByNeonDeveloper;

public class Main : MelonMod
{
	internal class NemesisAntiCheatMediaPlayerProtections
	{
		public string Link { get; set; }

		public string Reason { get; set; }

		public NemesisAntiCheatMediaPlayerProtections(string link, string REASON)
		{
			Link = link;
			Reason = REASON;
		}

		public NemesisAntiCheatMediaPlayerProtections()
		{
		}
	}

	internal class SpoofChecker
	{
		public string Username { get; set; }

		public ulong PlatformID { get; set; }
	}

	internal class NemesisAntiCheatControversialPeople
	{
		public string FusionNicknameAtTheTime { get; set; }

		public string SteamNameAtTheTime { get; set; }

		public string SteamIDAtTheTime { get; set; }

		public System.Collections.Generic.List<string> KnownSteamIds { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownFusionNickNames { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownSteamNames { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownDiscordsUserIDS { get; set; } = new System.Collections.Generic.List<string>();

		public string Reason { get; set; }

		public string ControversyLevel { get; set; }

		public NemesisAntiCheatControversialPeople(string fusionNicknameAtTheTime, System.Collections.Generic.List<string> knownSteamIds, System.Collections.Generic.List<string> Knownfusionnicknames, System.Collections.Generic.List<string> knownsteamnames, System.Collections.Generic.List<string> knowndiscordsids, string reason, string steamNameAtTheTime, string controversyLevel, string steamidatt)
		{
			FusionNicknameAtTheTime = fusionNicknameAtTheTime;
			KnownSteamIds = knownSteamIds;
			KnownFusionNickNames = Knownfusionnicknames;
			KnownSteamNames = knownsteamnames;
			KnownDiscordsUserIDS = knowndiscordsids;
			Reason = reason;
			SteamNameAtTheTime = steamNameAtTheTime;
			ControversyLevel = controversyLevel;
			SteamIDAtTheTime = steamidatt;
		}

		public string ToPrettyString()
		{
			return $"Fusion Nickname At The Time = \"{FusionNicknameAtTheTime}\"\r\nSteam Name At The Time = \"{SteamNameAtTheTime}\"\r\nSteam ID At The Time = \"{SteamIDAtTheTime}\"\r\nKnown Fusion Nicknames = {FormatList(KnownFusionNickNames)}\r\nKnown Steam Names = {FormatList(KnownSteamNames)}\r\nKnown Steam IDs = {FormatList(KnownSteamIds)}\r\nKnown Discords User IDs = {FormatList(KnownDiscordsUserIDS)}\r\nReason = \"{Reason}\"\r\nControversy Level = \"{ControversyLevel}\"\r\nSteam Page Link = https://steamcommunity.com/profiles/{SteamIDAtTheTime}";
			static string FormatList(System.Collections.Generic.List<string> list)
			{
				return (list.Count == 0) ? "None" : string.Join(", ", list.Select((string x) => "\"" + x + "\""));
			}
		}

		public void ShowOnPC()
		{
			string text = Path.Combine(MelonEnvironment.UserDataDirectory, FusionNicknameAtTheTime + "_ControversialPlayer.json");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.WriteAllText(text, ToPrettyString());
			Process.Start(new ProcessStartInfo
			{
				FileName = text,
				UseShellExecute = true
			});
		}

		public NemesisAntiCheatControversialPeople()
		{
		}
	}

	internal class NemesisAntiCheatCommunityNotes
	{
		public string FusionNicknameAtTheTime { get; set; }

		public string SteamNameAtTheTime { get; set; }

		public string SteamIdAtTheTime { get; set; }

		public System.Collections.Generic.List<string> KnownFusionNickNames { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownSteamNames { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownSteamIds { get; set; } = new System.Collections.Generic.List<string>();

		public System.Collections.Generic.List<string> KnownDiscordsUserIDS { get; set; } = new System.Collections.Generic.List<string>();

		public string Note { get; set; }

		public NemesisAntiCheatCommunityNotes(string fusionNicknameAtTheTime, string steamNameAtTheTime, string steamIdAtTheTime, string note, System.Collections.Generic.List<string> knownFusionNickNames, System.Collections.Generic.List<string> knownSteamNames, System.Collections.Generic.List<string> knownSteamIds, System.Collections.Generic.List<string> knownDiscordsIDS)
		{
			FusionNicknameAtTheTime = fusionNicknameAtTheTime;
			SteamNameAtTheTime = steamNameAtTheTime;
			SteamIdAtTheTime = steamIdAtTheTime;
			Note = note;
			KnownFusionNickNames = knownFusionNickNames;
			KnownSteamNames = knownSteamNames;
			KnownSteamIds = knownSteamIds;
			KnownDiscordsUserIDS = knownDiscordsIDS;
		}

		public string ToPrettyString()
		{
			return $"Fusion Nickname At The Time = \"{FusionNicknameAtTheTime}\"\r\nSteam Name At The Time = \"{SteamNameAtTheTime}\"\r\nSteam Id At The Time = \"{SteamIdAtTheTime}\"\r\nKnown Fusion Nicknames = {FormatList(KnownFusionNickNames)}\r\nKnown Steam Names = {FormatList(KnownSteamNames)}\r\nKnown SteamIds = {FormatList(KnownSteamIds)}\r\nKnown Discords UserIDS = {FormatList(KnownDiscordsUserIDS)}\r\nNote = \"{Note}\";\r\nSteam Page Link = https://steamcommunity.com/profiles/{SteamIdAtTheTime}";
			static string FormatList(System.Collections.Generic.List<string> list)
			{
				return (list.Count == 0) ? "None" : string.Join(", ", list.Select((string x) => "\"" + x + "\""));
			}
		}

		public void ShowOnPC()
		{
			string text = Path.Combine(MelonEnvironment.UserDataDirectory, FusionNicknameAtTheTime + "_CommunityNote.json");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.WriteAllText(text, ToPrettyString());
			Process.Start(new ProcessStartInfo
			{
				FileName = text,
				UseShellExecute = true
			});
		}

		public NemesisAntiCheatCommunityNotes()
		{
		}
	}

	internal class NemesisAntiCheatGlobalBan
	{
		public string FusionNicknameAtTheTime { get; set; }

		public string SteamNameAtTheTime { get; set; }

		public System.Collections.Generic.List<string> KnownSteamIds { get; set; }

		public string Reason { get; set; }

		public string OptionalProof { get; set; }

		public NemesisAntiCheatGlobalBan(string fusionnicknameatthetime, System.Collections.Generic.List<string> knownsteamids, string reason, string steamNameAtTheTime)
		{
			FusionNicknameAtTheTime = fusionnicknameatthetime;
			KnownSteamIds = knownsteamids;
			Reason = reason;
			SteamNameAtTheTime = steamNameAtTheTime;
		}

		public string ToPrettyString()
		{
			string value = FusionNicknameAtTheTime ?? "Unknown";
			string value2 = SteamNameAtTheTime ?? "Unknown";
			string value3 = Reason ?? "None";
			string value4 = OptionalProof ?? "None";
			string value5 = ((KnownSteamIds != null && KnownSteamIds.Count > 0) ? KnownSteamIds[0] : "Unknown");
			return $"Fusion Nickname At The Time = \"{value}\"\r\nSteam Name At The Time = \"{value2}\"\r\nKnown Steam IDs = {FormatList(KnownSteamIds)}\r\nReason = \"{value3}\"\r\nOptional Proof = \"{value4}\"\r\nSteam Page Link = https://steamcommunity.com/profiles/{value5}";
			static string FormatList(System.Collections.Generic.List<string> list)
			{
				return (list == null || list.Count == 0) ? "None" : string.Join(", ", list.Select((string x) => "\"" + x + "\""));
			}
		}

		public void ShowOnPC()
		{
			string text = Path.Combine(MelonEnvironment.UserDataDirectory, FusionNicknameAtTheTime + "_FPGlobalBanInfo.json");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.WriteAllText(text, ToPrettyString());
			Process.Start(new ProcessStartInfo
			{
				FileName = text,
				UseShellExecute = true
			});
		}
	}

	internal class AISpawner
	{
		public readonly string InstanceID;

		public Action<object> _spawnAction;

		public float _spawnIntervalMins = 1f;

		public object _spawnRoutine;

		public string BarcodeID;

		public string NameOfUsed;

		public Vector3 LocationOfSpawner;

		public System.Collections.Generic.HashSet<NetworkEntity> NetworkEntitiesMP = new System.Collections.Generic.HashSet<NetworkEntity>();

		public NetworkEntity LastSpawnedEntityMP;

		public GameObject LastSpawnedEntitySP;

		public System.Collections.Generic.HashSet<GameObject> SinglePlayerEntites = new System.Collections.Generic.HashSet<GameObject>();

		public string SpawnerMapLockedTo;

		public int MaxSpawnsInSpawner = 10;

		public bool despawndeads = true;

		public bool onlyspawnifalldead = false;

		public bool eachspawnisrandom = false;

		public RandomizerType eachspawnrandomtype;

		public static AISpawner CurrentInstance;

		public bool IsRunning => _spawnRoutine != null;

		public AISpawner(Action<object> spawnAction, float spawnIntervalMins)
		{
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
			if ((level != PermissionLevel.OWNER && !NetworkInfo.IsHost) || !NetworkInfo.HasServer)
			{
				throw new InvalidOperationException("Cannot do AISpawner without owner permission.");
			}
			_spawnAction = spawnAction ?? throw new ArgumentNullException("spawnAction");
			_spawnIntervalMins = spawnIntervalMins;
			InstanceID = Guid.NewGuid().ToString("N").Substring(0, 6)
				.ToUpper();
			CurrentInstance = this;
		}

		public AISpawner Start(string Barcodeid, Vector3 locationOfSpawner, int maxEntitiesFromThisSpawner, RandomizerType randomizertypenow = RandomizerType.AllNPCs)
		{
			StopAndClear(notification: false);
			if (eachspawnisrandom)
			{
				eachspawnrandomtype = randomizertypenow;
				BarcodeID = GetRandomByType(eachspawnrandomtype);
				NameOfUsed = "Randomized Spawner";
			}
			else
			{
				if (!IsBarcodeInGame(Barcodeid))
				{
					MelonLogger.Warning("[Spawner] Barcode '" + Barcodeid + "' does not exist in game. Spawner not started.");
					NotificationNow("Nemesis Anti-Cheat", "[Spawner] Barcode '" + Barcodeid + "' does not exist in game. Spawner not started.", NotificationType.ERROR, 3f);
					return null;
				}
				BarcodeID = Barcodeid ?? throw new ArgumentNullException("Barcodeid");
				NameOfUsed = StripColorTags(new SpawnableCrateReference(BarcodeID)?.Crate?.Title) ?? "Unknown";
			}
			LocationOfSpawner = locationOfSpawner;
			MaxSpawnsInSpawner = maxEntitiesFromThisSpawner;
			SpawnerMapLockedTo = SceneStreamer.Session?.Level?.Barcode?.ID;
			_spawnRoutine = MelonCoroutines.Start(SpawnEveryXMins());
			MelonLogger.Warning($"[Spawner {CurrentInstance.InstanceID}] Started spawner for '{NameOfUsed}' ({BarcodeID}) at {LocationOfSpawner}");
			NotificationNow("Nemesis Anti-Cheat", "Spawner created for " + NameOfUsed + " at your current location!", NotificationType.SUCCESS, 3f);
			return this;
		}

		public void Pause(bool notification = true)
		{
			if (_spawnRoutine != null)
			{
				try
				{
					MelonCoroutines.Stop(_spawnRoutine);
				}
				catch (Exception ex)
				{
					MelonLogger.Error("[Spawner] Error pausing coroutine: " + ex.Message);
				}
				_spawnRoutine = null;
				if (notification)
				{
					MelonLogger.Warning("[Spawner " + CurrentInstance.InstanceID + "] Paused spawner (can be resumed).");
					NotificationNow("Nemesis Anti-Cheat", "Paused Spawner " + CurrentInstance.NameOfUsed + " | ID: " + CurrentInstance.InstanceID, NotificationType.SUCCESS, 3f);
				}
			}
		}

		public void StopAndClear(bool notification = true)
		{
			if (_spawnRoutine != null)
			{
				Pause(notification: false);
			}
			BarcodeID = null;
			LocationOfSpawner = Vector3.zero;
			NameOfUsed = string.Empty;
			NetworkEntitiesMP.Clear();
			SinglePlayerEntites.Clear();
			if (notification)
			{
				MelonLogger.Warning("[Spawner " + CurrentInstance.InstanceID + "] Stopped and cleared instance.");
			}
		}

		public void Resume(bool notification = true)
		{
			if (_spawnRoutine != null)
			{
				MelonLogger.Error("[Spawner " + CurrentInstance.InstanceID + "] Already running.");
				return;
			}
			if (string.IsNullOrEmpty(BarcodeID))
			{
				MelonLogger.Error("[Spawner " + CurrentInstance.InstanceID + "] Cannot resume: NPC info missing.");
				return;
			}
			_spawnRoutine = MelonCoroutines.Start(SpawnEveryXMins());
			if (notification)
			{
				MelonLogger.Warning("[Spawner " + CurrentInstance.InstanceID + "] Resumed spawner.");
				NotificationNow("Nemesis Anti-Cheat", "Resuming Spawner " + CurrentInstance.NameOfUsed + " | ID: " + CurrentInstance.InstanceID, NotificationType.SUCCESS, 3f);
			}
		}

		public void UpdateSpawnInterval(float newSpawnIntervalMins)
		{
			if (_spawnIntervalMins != newSpawnIntervalMins)
			{
				_spawnIntervalMins = newSpawnIntervalMins;
				if (_spawnRoutine != null)
				{
					MelonCoroutines.Stop(_spawnRoutine);
					_spawnRoutine = MelonCoroutines.Start(SpawnEveryXMins());
				}
				MelonLogger.Warning($"[Spawner {InstanceID}] Spawn interval updated to {_spawnIntervalMins} minutes.");
			}
		}

		public void UpdateMaxSpawns(int newMaxEntities)
		{
			if (MaxSpawnsInSpawner != newMaxEntities)
			{
				MaxSpawnsInSpawner = newMaxEntities;
				MelonLogger.Warning($"[Spawner {InstanceID}] Max spawns updated to {MaxSpawnsInSpawner}.");
			}
		}

		public static IEnumerator RunTaskAsCoroutine(Task task)
		{
			while (!task.IsCompleted)
			{
				yield return null;
			}
			if (task.IsFaulted)
			{
				throw task.Exception;
			}
		}

		public IEnumerator SpawnEveryXMins()
		{
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var yourcurrentpermlevel, out var _);
			if ((yourcurrentpermlevel != PermissionLevel.OWNER && !NetworkInfo.IsHost) || !NetworkInfo.HasServer)
			{
				yield break;
			}
			WaitForSeconds wait = new WaitForSeconds(_spawnIntervalMins);
			LevelCrateReference levelCrateTemp = new LevelCrateReference(SpawnerMapLockedTo);
			MelonLogger.Warning($"[Spawner {InstanceID}] Started for {NameOfUsed}. Interval: {_spawnIntervalMins} seconds). Map locked to: {levelCrateTemp?.Scannable?.Title}");
			yield return RunTaskAsCoroutine(SpawnOnce(MaxSpawnsInSpawner));
			while (_spawnRoutine != null)
			{
				yield return wait;
				if (_spawnRoutine == null)
				{
					break;
				}
				yield return RunTaskAsCoroutine(SpawnOnce(MaxSpawnsInSpawner));
			}
		}

		public void KillAllInSpawner()
		{
			if (NetworkInfo.HasServer)
			{
				foreach (NetworkEntity item in NetworkEntitiesMP)
				{
					if (item.ND_IsNPC() && !item.ND_GetNPCAIBrain().isDead)
					{
						item?.ND_GetMarrowEntity()?.ND_GetNPCAIBrain()?.puppetMaster?.Kill();
					}
				}
				return;
			}
			foreach (GameObject singlePlayerEntite in SinglePlayerEntites)
			{
				if (singlePlayerEntite.ND_IsNPC() && !singlePlayerEntite.ND_GetNPCAIBrain().isDead)
				{
					singlePlayerEntite?.gameObject?.GetComponent<AIBrain>()?.puppetMaster?.Kill();
				}
			}
		}

		public async Task SpawnOnce(int maxEntitiesFromThisSpawner)
		{
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var yourcurrentpermlevel, out var _);
			if ((yourcurrentpermlevel != PermissionLevel.OWNER && !NetworkInfo.IsHost) || !NetworkInfo.HasServer)
			{
				return;
			}
			if (eachspawnisrandom)
			{
				BarcodeID = GetRandomByType(eachspawnrandomtype);
				NameOfUsed = "Randomized Spawner";
			}
			if (string.IsNullOrEmpty(BarcodeID) || _spawnAction == null)
			{
				return;
			}
			try
			{
				if (SceneStreamer.Session?.Level?.Barcode?.ID != SpawnerMapLockedTo)
				{
					return;
				}
				if (NetworkInfo.HasServer)
				{
					foreach (NetworkEntity entity in NetworkEntitiesMP.ToList())
					{
						if (entity == null || entity.ND_GetMarrowEntity() == null)
						{
							NetworkEntitiesMP.Remove(entity);
						}
						else if (despawndeads && entity.ND_IsNPC() && entity.ND_GetNPCAIBrain().isDead)
						{
							NetworkAssetSpawner.DespawnRequestInfo despawninfo = new NetworkAssetSpawner.DespawnRequestInfo
							{
								DespawnEffect = true,
								EntityID = entity.ID
							};
							NetworkAssetSpawner.Despawn(despawninfo);
							NetworkEntitiesMP.Remove(entity);
						}
					}
					if ((!onlyspawnifalldead || !NetworkEntitiesMP.Any((NetworkEntity e) => e.ND_IsNPC() && !e.ND_GetNPCAIBrain().isDead)) && NetworkEntitiesMP.Count < maxEntitiesFromThisSpawner)
					{
						var (netties, _, _, _) = await SpawnersSpawner(BarcodeID, LocationOfSpawner, Player.Head.rotation, effect: false);
						if (netties != null)
						{
							NetworkEntitiesMP.Add(netties);
							LastSpawnedEntityMP = netties;
							_spawnAction?.Invoke(netties);
						}
					}
					return;
				}
				foreach (GameObject entity2 in SinglePlayerEntites.ToList())
				{
					if (entity2 == null || entity2.gameObject == null)
					{
						SinglePlayerEntites.Remove(entity2);
					}
					else if (despawndeads && entity2.ND_IsNPC() && entity2.ND_GetNPCAIBrain().isDead)
					{
						entity2.ND_GetPoolee()?.Despawn();
						SinglePlayerEntites.Remove(entity2);
					}
				}
				if ((!onlyspawnifalldead || !SinglePlayerEntites.Any((GameObject e) => e.ND_IsNPC() && !e.ND_GetNPCAIBrain().isDead)) && SinglePlayerEntites.Count < maxEntitiesFromThisSpawner)
				{
					GameObject gameobby = (await SpawnersSpawner(BarcodeID, LocationOfSpawner, Player.Head.rotation, effect: false)).Item3;
					if (gameobby != null)
					{
						SinglePlayerEntites.Add(gameobby);
						LastSpawnedEntitySP = gameobby;
						_spawnAction?.Invoke(gameobby);
					}
				}
			}
			catch (Exception value)
			{
				MelonLogger.Error($"[Spawner {CurrentInstance.InstanceID}] Exception during spawn: {value}");
			}
		}
	}

	internal class SimpleTimer
	{
		public Action? _codenow;

		public float _mins;

		public object? _coroutine;

		private bool _quicker;

		private float _quickerSeconds;

		private bool _running;

		public SimpleTimer(Action codenow, float mins)
		{
			_codenow = codenow ?? throw new ArgumentNullException("codenow");
			_mins = mins;
		}

		public SimpleTimer Start(bool quicker = false, float quickerseconds = 10f)
		{
			Stop();
			_quicker = quicker;
			_quickerSeconds = quickerseconds;
			_running = true;
			_coroutine = MelonCoroutines.Start(RunEveryXMins());
			return this;
		}

		public void Stop()
		{
			_running = false;
			if (_coroutine != null)
			{
				try
				{
					MelonCoroutines.Stop(_coroutine);
				}
				catch
				{
				}
				_coroutine = null;
			}
		}

		public void Refresh(Action? newAction = null, float? newMins = null)
		{
			Stop();
			if (newAction != null)
			{
				_codenow = newAction;
			}
			if (newMins.HasValue)
			{
				_mins = newMins.Value;
			}
			Start(_quicker, _quickerSeconds);
			MelonLogger.Warning("Timer refreshed, first execution will happen after interval.");
		}

		public IEnumerator RunEveryXMins()
		{
			while (_running)
			{
				float waitTime = (_quicker ? _quickerSeconds : (_mins * 60f));
				yield return new WaitForSecondsRealtime(waitTime);
				if (!_running)
				{
					break;
				}
				try
				{
					_codenow?.Invoke();
				}
				catch (NullReferenceException)
				{
					MelonLogger.Warning("Timer skipped null Unity reference (object destroyed).");
				}
				catch (Exception value)
				{
					MelonLogger.Error($"Timer error: {value}");
				}
			}
		}
	}

	internal static class SiteStuff
	{
		public static System.Collections.Generic.HashSet<string> mostusedexcl = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<NemesisAntiCheatCommunityNotes> communitynotedplayers = new System.Collections.Generic.HashSet<NemesisAntiCheatCommunityNotes>();

		public static System.Collections.Generic.HashSet<NemesisAntiCheatGlobalBan> globalfpbans = new System.Collections.Generic.HashSet<NemesisAntiCheatGlobalBan>();

		public static System.Collections.Generic.HashSet<int> globalblocklistmodioid = new System.Collections.Generic.HashSet<int>();

		public static System.Collections.Generic.HashSet<int> globalaviblocklistmodioid = new System.Collections.Generic.HashSet<int>();

		public static System.Collections.Generic.HashSet<string> globalaviblocklistavatar = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalaviblocklistpallet = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalaviblocklistauthor = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalblocklistspawnable = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalblocklistpallet = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalblocklistauthor = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> globalblocklistavatar = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<string> blockednsfw = new System.Collections.Generic.HashSet<string>();

		public static System.Collections.Generic.HashSet<NemesisAntiCheatMediaPlayerProtections> MediaPlayerProtectionnow = new System.Collections.Generic.HashSet<NemesisAntiCheatMediaPlayerProtections>();

		public static System.Collections.Generic.HashSet<NemesisAntiCheatControversialPeople> fpcpeople = new System.Collections.Generic.HashSet<NemesisAntiCheatControversialPeople>();

		public static string custommediadoms;

		public static string VersionChecking;

		public static string GlobalBanList;

		public static string GlobalBanCheckinglink;

		public static string AdditonWhitelistMediaPlayer;

		public static string MediaBlocker;

		public static string NSFWBlocker;

		public static string GlobalNACBlacklist;

		public static string NemesisAntiCheatControversialPeoplenow;

		public static string communitynotes;

		public static string mostusedthings;

		private static readonly JsonSerializerOptions options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString
		};

		public static bool isUpdating = false;

		private static readonly System.Collections.Generic.Dictionary<ulong, bool> CheckedSteamIDs = new System.Collections.Generic.Dictionary<ulong, bool>();

		public static void CreateBackupAndStore(string fileName, string data)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException("Filename cannot be null or empty.", "fileName");
			}
			if (data == null)
			{
				data = string.Empty;
			}
			string text = Path.Combine(MelonEnvironment.UserDataDirectory, "NemesisAntiCheatPasteBackUps");
			Directory.CreateDirectory(text);
			string path = Path.Combine(text, fileName);
			try
			{
				File.WriteAllText(path, data);
				MelonLogger.Msg("Backup stored successfully: " + fileName);
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to store backup '" + fileName + "': " + ex.Message);
			}
		}

		public static void FetchPastesForFetchers()
		{
			MelonCoroutines.Start(ReadFromSite("http://tiny.cc/ixs4101", delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisPasteLinks.json", sitetext);
				string[] array = sitetext.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string text2 = text.Trim();
					if (text2.Contains(":"))
					{
						string[] array2 = text2.Split(new char[1] { ':' }, 2);
						string text3 = array2[0].Trim().ToLowerInvariant();
						string text4 = array2[1].Trim();
						switch (text3)
						{
						case "versionchecking":
							VersionChecking = text4;
							break;
						case "globalbancheckinglink":
							GlobalBanCheckinglink = text4;
							break;
						case "globalbanlist":
							GlobalBanList = text4;
							break;
						case "additonwhitelistmediaplayer":
							AdditonWhitelistMediaPlayer = text4;
							break;
						case "mediablocker":
							MediaBlocker = text4;
							break;
						case "nsfwblocker":
							NSFWBlocker = text4;
							break;
						case "globalfpblacklist":
							GlobalNACBlacklist = text4;
							break;
						case "naccontroversialpeople":
							NemesisAntiCheatControversialPeoplenow = text4;
							break;
						case "communitynotes":
							communitynotes = text4;
							break;
						case "mostuseditemsexclude":
							mostusedthings = text4;
							break;
						default:
							MelonLogger.Warning("Unknown key: " + text3);
							break;
						}
					}
				}
				MelonLogger.Warning("Fetched All Paste Links!!!");
			}));
		}

		public static void FetchMostItemsUsedExclude()
		{
			mostusedexcl.Clear();
			MelonCoroutines.Start(ReadFromSite(mostusedthings, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatmostusedthingsexclude.json", sitetext);
				System.Collections.Generic.HashSet<string> hashSet = new System.Collections.Generic.HashSet<string>(sitetext.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
				mostusedexcl = hashSet;
				MelonLogger.Warning($"Total Nemesis Anti-Cheat Spawn Delay Excluded Items : {mostusedexcl.Count}");
			}));
		}

		public static void FetchCommunitynotes()
		{
			communitynotedplayers.Clear();
			MelonCoroutines.Start(ReadFromSite(communitynotes, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatCommunityNotes.json", sitetext);
				System.Collections.Generic.HashSet<NemesisAntiCheatCommunityNotes> hashSet = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.HashSet<NemesisAntiCheatCommunityNotes>>(sitetext, options);
				communitynotedplayers = hashSet;
				MelonLogger.Warning($"Total Nemesis Anti-Cheat Community Noted Players : {communitynotedplayers.Count}");
			}));
		}

		public static void FetchnacGlobalBan()
		{
			globalfpbans.Clear();
			MelonCoroutines.Start(ReadFromSite(GlobalBanList, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatGlobalBanList.json", sitetext);
				System.Collections.Generic.HashSet<NemesisAntiCheatGlobalBan> hashSet = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.HashSet<NemesisAntiCheatGlobalBan>>(sitetext, options);
				globalfpbans = hashSet;
				MelonLogger.Warning($"Total Nemesis Anti-Cheat Global Bans : {globalfpbans.Count}");
			}));
		}

		public static void NACControversialPeople()
		{
			fpcpeople.Clear();
			MelonCoroutines.Start(ReadFromSite(NemesisAntiCheatControversialPeoplenow, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatControversialPeople.json", sitetext);
				System.Collections.Generic.HashSet<NemesisAntiCheatControversialPeople> hashSet = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.HashSet<NemesisAntiCheatControversialPeople>>(sitetext, options);
				fpcpeople = hashSet;
				MelonLogger.Warning($"Total Fusion Controversial People : {fpcpeople.Count}");
			}));
		}

		public static void FetchVersionChecker()
		{
			MelonCoroutines.Start(ReadFromSite(VersionChecking, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatVersion.json", sitetext);
				if ("1.48.94".Trim() != sitetext.Trim())
				{
					NotificationNow("Nemesis Anti-Cheat", "Update " + sitetext + " Is Available\nJoin the Discord To Download Update!", NotificationType.ERROR, 3f, showtitle: true, savetomenu: true, delegate
					{
						OpenPageNow("https://shorturl.at/YymS0");
					});
				}
			}));
		}

		public static void GlobalBanChecking()
		{
			MelonCoroutines.Start(ReadFromSite(GlobalBanCheckinglink, delegate(string siteText)
			{
				GlobalBanList globalBanList;
				try
				{
					globalBanList = System.Text.Json.JsonSerializer.Deserialize<GlobalBanList>(siteText, options);
				}
				catch (Exception ex)
				{
					MelonLogger.Error("Failed to deserialize GlobalBanList: " + ex.Message);
					return;
				}
				MelonLogger.Warning($"Total Fusion Global Bans: {globalBanList.Bans.Count}");
				GlobalBanInfo globalBanInfo = globalBanList.Bans.FirstOrDefault((GlobalBanInfo b) => b?.Platforms != null && b.Platforms.Any((PlatformInfo p) => p.PlatformID == SteamIdYours()));
				if (globalBanInfo != null)
				{
					string text = $"You're Fusion Global Banned!\nUser: {globalBanInfo.Username}\nReason: {globalBanInfo.Reason}";
					MelonLogger.Error(text);
					NotificationNow("Nemesis Anti-Cheat", text, NotificationType.ERROR, 10f);
				}
				else
				{
					MelonLogger.Warning($"✅ You're Not Banned! {SteamIdYours()}");
				}
			}));
		}

		public static void FetchGlobalBlockList()
		{
			globalblocklistspawnable.Clear();
			globalblocklistmodioid.Clear();
			globalblocklistpallet.Clear();
			globalblocklistauthor.Clear();
			globalblocklistavatar.Clear();
			globalaviblocklistmodioid.Clear();
			globalaviblocklistavatar.Clear();
			globalaviblocklistpallet.Clear();
			globalaviblocklistauthor.Clear();
			MelonCoroutines.Start(ReadFromSite(GlobalNACBlacklist, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatGlobalBlacklist.json", sitetext);
				string[] array = sitetext.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string text2 = text.Trim();
					if (!string.IsNullOrEmpty(text2) && text2.Contains(':'))
					{
						string[] array2 = text2.Split(':', 2);
						string text3 = array2[0].Trim().ToLowerInvariant();
						string text4 = array2[1].Trim();
						if (text3 == "spawnable" && !globalblocklistspawnable.Contains(text2))
						{
							globalblocklistspawnable.Add(text2);
						}
						switch (text3)
						{
						case "spawnable_pallet":
							if (!globalblocklistpallet.Contains(text4))
							{
								globalblocklistpallet.Add(text4);
							}
							break;
						case "spawnable_author":
							if (!globalblocklistauthor.Contains(text4))
							{
								globalblocklistauthor.Add(text4);
							}
							break;
						case "spawnable_modid":
						{
							if (int.TryParse(text4, out var result) && !globalblocklistmodioid.Contains(result))
							{
								globalblocklistmodioid.Add(result);
							}
							break;
						}
						case "avatar":
							if (!globalaviblocklistavatar.Contains(text4))
							{
								globalaviblocklistavatar.Add(text4);
							}
							break;
						}
						switch (text3)
						{
						case "avatar_pallet":
							if (!globalaviblocklistpallet.Contains(text4))
							{
								globalaviblocklistpallet.Add(text4);
							}
							break;
						case "avatar_author":
							if (!globalaviblocklistauthor.Contains(text4))
							{
								globalaviblocklistauthor.Add(text4);
							}
							break;
						case "avatar_modid":
						{
							if (int.TryParse(text4, out var result2) && !globalaviblocklistmodioid.Contains(result2))
							{
								globalaviblocklistmodioid.Add(result2);
							}
							break;
						}
						}
					}
				}
			}));
		}

		public static void FetchNSFWBLOCKED()
		{
			blockednsfw.Clear();
			MelonCoroutines.Start(ReadFromSite(NSFWBlocker, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatNSFWBlocker.json", sitetext);
				string[] array = sitetext.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string text2 = text.Trim();
					if (!string.IsNullOrEmpty(text2) && !blockednsfw.Contains(text2))
					{
						blockednsfw.Add(text2);
					}
				}
			}));
		}

		public static void FetchMediaProtections()
		{
			MediaPlayerProtectionnow.Clear();
			MelonCoroutines.Start(ReadFromSite(MediaBlocker, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatMediaBlocker.json", sitetext);
				MediaPlayerProtectionnow = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.HashSet<NemesisAntiCheatMediaPlayerProtections>>(sitetext, options);
			}));
		}

		public static void FetchMediaDoms()
		{
			MelonCoroutines.Start(ReadFromSite(AdditonWhitelistMediaPlayer, delegate(string sitetext)
			{
				CreateBackupAndStore("NemesisAntiCheatAdditionalWhitelist.json", sitetext);
				custommediadoms = sitetext;
			}));
		}

		public static IEnumerator UpdateSites()
		{
			if (isUpdating)
			{
				MelonLogger.Warning("Nemesis Anti-Cheat site update already running.");
				yield break;
			}
			isUpdating = true;
			(string Name, Action Action)[] steps = new(string, Action)[10]
			{
				("Fetching Nemesis Anti-Cheat pastes links", FetchPastesForFetchers),
				("Fetching Nemesis Anti-Cheat controversial people", NACControversialPeople),
				("Fetching Nemesis Anti-Cheat community notes", FetchCommunitynotes),
				("Fetching Nemesis Anti-Cheat global bans", FetchnacGlobalBan),
				("Fetching Nemesis Anti-Cheat NSFW block list", FetchNSFWBLOCKED),
				("Fetching Nemesis Anti-Cheat media protections", FetchMediaProtections),
				("Fetching Nemesis Anti-Cheat media domains", FetchMediaDoms),
				("Fetching Nemesis Anti-Cheat global block list", FetchGlobalBlockList),
				("Checking Nemesis Anti-Cheat version", FetchVersionChecker),
				("Fetching Nemesis Anti-Cheat Spawn Delay Excluded Items", FetchMostItemsUsedExclude)
			};
			int totalSteps = steps.Length;
			for (int i = 0; i < totalSteps; i++)
			{
				(string Name, Action Action) step = steps[i];
				MelonLogger.Warning($"[{i + 1}/{totalSteps}] {step.Name}...");
				try
				{
					step.Action();
				}
				catch (Exception ex)
				{
					MelonLogger.Error("Nemesis Anti-Cheat step failed: " + step.Name);
					MelonLogger.Error(ex.ToString());
				}
				yield return new WaitForSeconds(3f);
			}
			MelonLogger.Warning("All Nemesis Anti-Cheat data successfully updated from sites.");
			isUpdating = false;
		}

		public static IEnumerator ReadFromSite(string url, Action<string> sitetextaction)
		{
			UnityWebRequest www = UnityWebRequest.Get(url);
			www.SetRequestHeader("User-Agent", "Mozilla/5.0");
			www.SetRequestHeader("Accept", "*/*");
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				MelonLogger.Warning("Error fetching Site Text: " + www.error + " url : " + url);
			}
			else
			{
				string json = www.downloadHandler.text;
				try
				{
					sitetextaction?.Invoke(json);
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					MelonLogger.Warning("Error fetching Site Text : " + ex2.Message + " url : " + url);
				}
			}
			www.Dispose();
		}

		public static void NotificationNowDifferent(string messagetype)
		{
			NotificationNow("Nemesis Anti-Cheat", messagetype, NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
			{
				GUIUtility.systemCopyBuffer = messagetype;
				NotificationNow("Nemesis Anti-Cheat", "Copied Details To Clipboard!", NotificationType.SUCCESS, 3f);
			});
		}

		public static IEnumerator RunNotificationThenKick(PlayerID playernow, string message, float messagetime, Action CodeNow)
		{
			if (!HideNemesisAntiCheat)
			{
				SendNotificationData data = SendNotificationData.Create(PlayerIDManager.LocalSmallID, message, "Fusion Protetor Kick/Ban System", messagetime);
				MessageRelay.RelayModule<SendNotificationMessage, SendNotificationData>(data, new MessageRoute(playernow.SmallID, NetworkChannel.Reliable));
				yield return new WaitForSecondsRealtime(messagetime + 0.5f);
				CodeNow?.Invoke();
			}
		}

		public static async Task AltPrevention(ulong steamid)
		{
			if (!disablesteamreading || steamid == SteamIdYours())
			{
				return;
			}
			if (CheckedSteamIDs.TryGetValue(steamid, out var isAlt))
			{
				MelonLogger.Msg(steamid + ": Already Checked");
				if (!isAlt || !AltRemov || !NetworkInfo.IsHost)
				{
					return;
				}
				MelonLogger.Msg(steamid + ": Is A Alt");
				NetworkPlayer playerxz = NetworkPlayer.Players.FirstOrDefault((NetworkPlayer p) => p.PlayerID.PlatformID == steamid);
				try
				{
					if (!baninsteadalt)
					{
						MelonCoroutines.Start(RunNotificationThenKick(playerxz.PlayerID, "Your Alt Account Was Detected On Nemesis Anti-Cheat, Kicking You From Lobby!", 3f, delegate
						{
							NotificationNow("Nemesis Anti-Cheat", $"Alt Account Detected!\nKicking {steamid} {playerxz.PlayerID.Metadata.Nickname.GetValue()} [{playerxz.PlayerID.Metadata.Username.GetValue()}] ({playerxz.PlayerID.PlatformID})", NotificationType.ERROR, 3f);
							NetworkHelper.KickUser(playerxz.PlayerID);
						}));
					}
					else if (!NetworkHelper.IsBanned(playerxz.PlayerID.PlatformID))
					{
						MelonCoroutines.Start(RunNotificationThenKick(playerxz.PlayerID, "Your Alt Account Was Detected On Nemesis Anti-Cheat, Banning You From Lobby!", 3f, delegate
						{
							NotificationNow("Nemesis Anti-Cheat", $"Alt Account Detected!\nBanning {steamid} {playerxz.PlayerID.Metadata.Nickname.GetValue()} [{playerxz.PlayerID.Metadata.Username.GetValue()}] ({playerxz.PlayerID.PlatformID})", NotificationType.ERROR, 3f);
							NetworkHelper.BanUser(playerxz.PlayerID);
						}));
					}
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					MelonLogger.Msg(ex2);
				}
				return;
			}
			CheckedSteamIDs[steamid] = false;
			try
			{
				string steamname = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				_ = string.Empty;
				FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var _, out var _);
				_ = LobbyInfoManager.LobbyInfo;
				NetworkPlayer player = NetworkPlayer.Players.FirstOrDefault((NetworkPlayer p) => p.PlayerID.PlatformID == steamid);
				if (player == null)
				{
					return;
				}
				(string steamname, string level, string games, string friends, string recentPlaytime, string vacBans, string profileAwards, string badges, string reviews, string artwork, string inventory, string groups) extracted1 = ExtractOther(await ReadFromSiteAsync($"https://steamcommunity.com/profiles/{steamid}"));
				steamname = StripColorTags(extracted1.steamname);
				string accountlevel = extracted1.level;
				string numberofgames = extracted1.games;
				string numberoffriends = extracted1.friends;
				string recentplaytime = extracted1.recentPlaytime;
				_ = extracted1.groups;
				string vacBans = extracted1.vacBans;
				string artwork = extracted1.artwork;
				string inventory = extracted1.inventory;
				string profileAwards = extracted1.profileAwards;
				string reviews = extracted1.reviews;
				string badges = extracted1.badges;
				(string region, string accountage, string status, string accountworth) extracted2 = ExtractPlayerInfo(await ReadFromSiteAsync($"https://steamid.pro/lookup/{steamid}"));
				string region = extracted2.region;
				string accountage = extracted2.accountage;
				string status = extracted2.status;
				string worth = extracted2.accountworth;
				string joiningUsername = StripColorTags(player.PlayerID.Metadata.Username.GetValue());
				string joiningnickname = StripColorTags(player.PlayerID.Metadata.Nickname.GetValue());
				MelonLogger.Msg("Fusion Info:\n" + ((player.PlayerID.Metadata.Nickname.GetValue() != "?") ? ("Nickname : " + player.PlayerID.Metadata.Nickname.GetValue() + "\n") : "") + ((joiningUsername != "?") ? ("Username : " + joiningUsername + "\n\n") : "\n") + "Steam Account Info:\n" + ((steamname != "?") ? ("Name : " + steamname + "\n") : "") + ((accountlevel != "?") ? ("Level : " + accountlevel + "\n") : "") + ((region != "?") ? ("Region : " + region + "\n") : "") + ((accountage != "?") ? ("Account Age : " + accountage + "\n") : "") + ((numberofgames != "?") ? ("Number of Games : " + numberofgames + "\n") : "") + ((numberoffriends != "?") ? ("Number of Friends : " + numberoffriends + "\n") : "") + ((recentplaytime != "?") ? ("Recent Playtime : " + recentplaytime + "\n") : "") + ((vacBans != "?") ? ("VAC Bans : " + vacBans + "\n") : "") + ((profileAwards != "?") ? ("Profile Awards : " + profileAwards + "\n") : "") + ((badges != "?") ? ("Badges : " + badges + "\n") : "") + ((artwork != "?") ? ("Artwork : " + artwork + "\n") : "") + ((inventory != "?") ? ("Inventory : " + inventory + "\n") : "") + ((reviews != "?") ? ("Reviews : " + reviews + "\n") : "") + ((status != "?") ? ("Status : " + status + "\n") : "") + ((worth != "$1") ? ("Account Worth : " + worth) : ""));
				if (clonedetector && NetworkInfo.HasLayer)
				{
					IMatchmaker matchmaker = NetworkLayerManager.Layer.Matchmaker;
					if (matchmaker != null)
					{
						try
						{
							TaskCompletionSource<IMatchmaker.MatchmakerCallbackInfo> tcs = new TaskCompletionSource<IMatchmaker.MatchmakerCallbackInfo>();
							matchmaker.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo i)
							{
								tcs.TrySetResult(i);
							});
							IMatchmaker.MatchmakerCallbackInfo info = await tcs.Task;
							if (info.Lobbies == null)
							{
								return;
							}
							IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
							for (int num = 0; num < lobbies.Length; num++)
							{
								IMatchmaker.LobbyInfo lobby = lobbies[num];
								if (lobby.Metadata.LobbyInfo == null || lobby.Metadata.LobbyInfo == LobbyInfoManager.LobbyInfo)
								{
									continue;
								}
								PlayerInfo[] players = lobby.Metadata.LobbyInfo.PlayerList.Players;
								foreach (PlayerInfo playerInLobby in players)
								{
									if (playerInLobby != null && playerInLobby.PlatformID != player.PlayerID.PlatformID && !string.IsNullOrEmpty(playerInLobby.Username) && playerInLobby.Username == joiningUsername)
									{
										NotificationNowDifferent($"Player is cloning username of another player => Cheater: [{joiningUsername}] ({player.PlayerID.PlatformID})");
										break;
									}
								}
							}
						}
						catch (Exception ex)
						{
							Exception ex3 = ex;
							MelonLogger.Msg("Error while checking lobbies: " + ex3.Message);
						}
					}
				}
				if (spooferprofiledetection && Encoding.UTF8.GetString(Encoding.Default.GetBytes(StripColorTags(player.PlayerID.Metadata.Username.GetValue()))).ToLower() != steamname.ToLower())
				{
					StringBuilder stringBuilder = new StringBuilder().AppendLine("Player Is Spoofing Profile:").AppendLine("Spoofer's Current Account:");
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder);
					handler.AppendLiteral("Steam : ");
					handler.AppendFormatted(steamname);
					handler.AppendLiteral(" ");
					handler.AppendFormatted(steamid);
					StringBuilder sb = stringBuilder2.AppendLine(ref handler);
					if (!string.IsNullOrEmpty(joiningnickname))
					{
						stringBuilder = sb;
						StringBuilder stringBuilder3 = stringBuilder;
						handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
						handler.AppendLiteral("Nickname : ");
						handler.AppendFormatted(joiningnickname);
						stringBuilder3.AppendLine(ref handler);
					}
					stringBuilder = sb;
					StringBuilder stringBuilder4 = stringBuilder;
					handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
					handler.AppendLiteral("Username : ");
					handler.AppendFormatted(joiningUsername);
					stringBuilder4.AppendLine(ref handler);
					NotificationNow("Nemesis Anti-Cheat", sb.ToString(), NotificationType.WARNING, 3f, showtitle: true, savetomenu: true);
				}
				if (privatekicksteam && NetworkInfo.IsHost)
				{
					if (accountlevel == "?")
					{
						MelonCoroutines.Start(RunNotificationThenKick(player.PlayerID, "Steam Profile Is Private Nemesis Anti-Cheat Is Kicking You!", 3f, delegate
						{
							NotificationNow("Nemesis Anti-Cheat", "Steam Profile Is Private Removing! : " + joiningUsername, NotificationType.ERROR, 3f);
							NetworkHelper.KickUser(player.PlayerID);
						}));
					}
					else
					{
						NotificationNow("Nemesis Anti-Cheat", "Profile Is Private Can't Detect If It's A Alt Account! : " + joiningUsername, NotificationType.ERROR, 3f);
					}
				}
				if (AltNotifications && accountage == "new")
				{
					NotificationNowDifferent($"Alt Account Detected!\nPossibly Cheater : {player.PlayerID.Metadata.Nickname.GetValue()} [{player.PlayerID.Metadata.Username.GetValue()}] ({player.PlayerID.PlatformID})");
				}
				if (teleportaltacctoyou && NetworkInfo.IsHost)
				{
					PermissionSender.SendPermissionRequest(PermissionCommandType.TELEPORT_TO_ME, player.PlayerID.SmallID);
				}
				if (!AltRemov || !NetworkInfo.IsHost || (!(accountlevel == "0") && !(accountage == "new")))
				{
					return;
				}
				CheckedSteamIDs[steamid] = true;
				try
				{
					if (!baninsteadalt)
					{
						MelonCoroutines.Start(RunNotificationThenKick(player.PlayerID, "Your Alt Account Was Detected On Nemesis Anti-Cheat, Kicking You From Lobby!", 3f, delegate
						{
							NotificationNow("Nemesis Anti-Cheat", $"Alt Account Detected!\nKicking {steamname} {player.PlayerID.Metadata.Nickname.GetValue()} [{player.PlayerID.Metadata.Username.GetValue()}] ({player.PlayerID.PlatformID})", NotificationType.ERROR, 3f);
							NetworkHelper.KickUser(player.PlayerID);
						}));
					}
					else if (!NetworkHelper.IsBanned(player.PlayerID.PlatformID))
					{
						MelonCoroutines.Start(RunNotificationThenKick(player.PlayerID, "Your Alt Account Was Detected On Nemesis Anti-Cheat, Banning You From Lobby!", 3f, delegate
						{
							NotificationNow("Nemesis Anti-Cheat", $"Alt Account Detected!\nBanning {steamname} {player.PlayerID.Metadata.Nickname.GetValue()} [{player.PlayerID.Metadata.Username.GetValue()}] ({player.PlayerID.PlatformID})", NotificationType.ERROR, 3f);
							NetworkHelper.BanUser(player.PlayerID);
						}));
					}
				}
				catch (Exception ex)
				{
					Exception ex4 = ex;
					MelonLogger.Msg(ex4);
				}
			}
			catch (Exception ex5)
			{
				if (alterrornotis)
				{
					NotificationNow("Nemesis Anti-Cheat", "AltPrevention ERROR: " + ex5.Message, NotificationType.ERROR, 2f);
				}
				else
				{
					MelonLogger.Msg("AltPrevention ERROR: " + ex5.Message);
				}
			}
		}

		public static async Task<string> ReadFromSiteAsync(string url)
		{
			using HttpClient client = new HttpClient();
			await Task.Delay(2000);
			byte[] bytes = await client.GetByteArrayAsync(url);
			return Encoding.UTF8.GetString(bytes);
		}

		public static (string region, string accountage, string status, string accountworth) ExtractPlayerInfo(string html)
		{
			Match match = Regex.Match(html, "<ul class=\"player-info\">(.*?)</ul>", RegexOptions.Singleline);
			if (!match.Success)
			{
				return (region: "?", accountage: "?", status: "?", accountworth: "?");
			}
			string value = match.Groups[1].Value;
			string text = Regex.Match(value, "flag-icon-([a-z]{2})", RegexOptions.IgnoreCase).Groups[1].Value.ToUpper();
			string text2 = Regex.Match(value, "<span class=\"number\"[^>]*>([^<]+)</span>").Groups[1].Value.Trim();
			string text3 = Regex.Match(value, "<li>\\s*(online|offline)\\s*</li>", RegexOptions.IgnoreCase).Groups[1].Value;
			Match match2 = Regex.Match(html, "<div class=\"prices tooltipped tooltipped-s\">.*?<span class=\"number-price\">(.*?)</span>", RegexOptions.Singleline);
			string text4 = (match2.Success ? match2.Groups[1].Value.Trim() : "?");
			if (string.IsNullOrEmpty(text))
			{
				text = "?";
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "?";
			}
			if (string.IsNullOrEmpty(text3))
			{
				text3 = "?";
			}
			if (string.IsNullOrEmpty(text4))
			{
				text4 = "?";
			}
			text2 = text2.Replace(",", "").Replace(".", "").Trim();
			return (region: text, accountage: text2, status: text3, accountworth: text4);
		}

		public static (string steamname, string level, string games, string friends, string recentPlaytime, string vacBans, string profileAwards, string badges, string reviews, string artwork, string inventory, string groups) ExtractOther(string html)
		{
			string value = Regex.Match(html, "<span class=\"actual_persona_name\">([\\s\\S]*?)</span>", RegexOptions.Singleline).Groups[1].Value;
			value = WebUtility.HtmlDecode(value).Trim();
			if (string.IsNullOrEmpty(value))
			{
				value = "?";
			}
			string text = Regex.Match(html, "<span class=\"friendPlayerLevelNum\">(\\d+)</span>", RegexOptions.Singleline).Groups[1].Value;
			if (string.IsNullOrEmpty(text))
			{
				text = "?";
			}
			string text2 = Regex.Match(html, "<div class=\"recentgame_quicklinks recentgame_recentplaytime\">(?:\\r?\\n\\s*)<div>(.*?)</div>", RegexOptions.Singleline).Groups[1].Value.Trim();
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "?";
			}
			string item = GetCount("Games");
			string item2 = GetCount("Friends");
			string item3 = GetCount("Profile Awards");
			string item4 = GetCount("Badges");
			string item5 = GetCount("Reviews");
			string item6 = GetCount("Artwork");
			string item7 = GetCount("Inventory");
			string item8 = GetCount("Groups");
			string text3 = Regex.Match(html, "<div class=\"profile_ban\">.*?<span class=\"profile_count_link_total\">\\s*(\\d+)\\s*</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
			if (string.IsNullOrEmpty(text3))
			{
				text3 = "?";
			}
			return (steamname: value, level: text, games: item, friends: item2, recentPlaytime: text2, vacBans: text3, profileAwards: item3, badges: item4, reviews: item5, artwork: item6, inventory: item7, groups: item8);
			string GetCount(string label)
			{
				Match match = Regex.Match(html, "<span class=\"count_link_label\">\\s*" + Regex.Escape(label) + "\\s*</span>.*?<span class=\"profile_count_link_total\">\\s*(\\d+)\\s*</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				return string.IsNullOrEmpty(match.Groups[1].Value) ? "?" : match.Groups[1].Value;
			}
		}
	}

	internal class TeleporterManager
	{
		public static System.Collections.Generic.List<TeleporterManager> Teleportersnowx = new System.Collections.Generic.List<TeleporterManager>();

		public static readonly string teleportmanager = Path.Combine(MelonEnvironment.UserDataDirectory, "NemesisAntiCheatTeleportManager.json");

		[JsonProperty]
		public string Map { get; set; }

		[JsonProperty]
		public Vector3 Position { get; set; }

		[JsonProperty]
		public Quaternion Rotation { get; set; }

		[JsonProperty]
		public string TitleOfTeleporter { get; set; }

		[JsonProperty]
		public string LevelBarcode { get; set; }

		[Newtonsoft.Json.JsonConstructor]
		public TeleporterManager(string map, Vector3 position, Quaternion rotation, string titleOfTeleporter, string levelBarcode)
		{
			TitleOfTeleporter = titleOfTeleporter;
			Position = new Vector3(position.x, position.y, position.z);
			Rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
			LevelBarcode = levelBarcode;
			Map = map;
		}

		public void TeleportToIt()
		{
			if (SceneStreamer.Session?.Level?.Barcode?.ID == LevelBarcode)
			{
				PhysicsRig physicsRig = Player.RigManager.physicsRig;
				foreach (Rigidbody componentsInChild in physicsRig.GetComponentsInChildren<Rigidbody>())
				{
					componentsInChild.velocity = Vector3.zero;
					componentsInChild.angularVelocity = Vector3.zero;
				}
				physicsRig.marrowEntity.Teleport(Position, Rotation, doResetPose: true);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Can't Use — You Are Not On " + Map, NotificationType.WARNING, 2.5f);
			}
		}

		public void EditPresetName(string newValue)
		{
			if (!Teleportersnowx.Any((TeleporterManager X) => X.TitleOfTeleporter == newValue))
			{
				TitleOfTeleporter = newValue;
				SaveTeleporters();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfTeleporter + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public static void LoadTeleporters()
		{
			try
			{
				if (File.Exists(teleportmanager))
				{
					string value = File.ReadAllText(teleportmanager);
					System.Collections.Generic.List<TeleporterManager> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<TeleporterManager>>(value, new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						FloatFormatHandling = FloatFormatHandling.Symbol,
						Culture = CultureInfo.InvariantCulture,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					});
					if (list != null)
					{
						Teleportersnowx = list;
					}
				}
			}
			catch
			{
			}
		}

		public static void SaveTeleporters()
		{
			try
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					FloatFormatHandling = FloatFormatHandling.Symbol,
					Culture = CultureInfo.InvariantCulture,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				string contents = JsonConvert.SerializeObject(Teleportersnowx, settings);
				File.WriteAllText(teleportmanager, contents);
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to save teleporters: " + ex.Message);
			}
		}
	}

	internal static class ColorClasseyPoo
	{
		private static readonly Color[] presetColors = new Color[7]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.magenta,
			Color.cyan,
			Color.white
		};

		public static Color RandomUnityColor()
		{
			return presetColors[System.Random.Shared.Next(0, presetColors.Length)];
		}

		public static Color ReturnColor(float r, float g, float b)
		{
			return new Color(r, g, b, 30f);
		}

		public static Color RandomColor()
		{
			System.Random random = new System.Random();
			System.Random random2 = new System.Random();
			System.Random random3 = new System.Random();
			return ReturnColor(random.Next(0, 255), random2.Next(0, 255), random3.Next(0, 255));
		}

		public static Color ConvertRGBAToUnityColor(int r, int g, int b, int a = 255)
		{
			return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
		}
	}

	internal class CreateCheatToolsPreset
	{
		public class Item
		{
			private int? _modIoID;

			[JsonProperty]
			public string BarcodeId { get; set; } = "Empty";

			[JsonProperty]
			public string SpawnableName { get; set; } = "Empty";

			[JsonProperty]
			public bool LocalSpawn { get; set; } = false;

			[JsonProperty]
			public int ModIoID
			{
				get
				{
					if (_modIoID.HasValue && _modIoID.Value != -1)
					{
						return _modIoID.Value;
					}
					try
					{
						if (string.IsNullOrEmpty(BarcodeId) || BarcodeId == "Empty")
						{
							return -1;
						}
						SpawnableCrate spawnableCrate = new SpawnableCrateReference(BarcodeId)?.Crate;
						if (spawnableCrate == null)
						{
							return -1;
						}
						Pallet pallet = spawnableCrate.Pallet;
						int modID = CrateFilterer.GetModID(pallet);
						if (modID == -1)
						{
							return -1;
						}
						_modIoID = modID;
						return modID;
					}
					catch
					{
						return -1;
					}
				}
				set
				{
					_modIoID = value;
				}
			}
		}

		public static CreateCheatToolsPreset CurrentPresetNow;

		public static System.Collections.Generic.List<CreateCheatToolsPreset> CheatPresets = new System.Collections.Generic.List<CreateCheatToolsPreset>();

		public static readonly string devitemscurrent = Path.Combine(MelonEnvironment.UserDataDirectory, "CustomDevToolsCurrent.json");

		public static readonly string devitems = Path.Combine(MelonEnvironment.UserDataDirectory, "CustomDevItems.json");

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public Item Item1 { get; set; }

		[JsonProperty]
		public Item Item2 { get; set; }

		[JsonProperty]
		public Item Item3 { get; set; }

		[JsonProperty]
		public Item Item4 { get; set; }

		[JsonProperty]
		public Item Item5 { get; set; }

		[Newtonsoft.Json.JsonIgnore]
		public System.Collections.Generic.IEnumerable<Item> Items
		{
			get
			{
				yield return Item1;
				yield return Item2;
				yield return Item3;
				yield return Item4;
				yield return Item5;
			}
		}

		[Newtonsoft.Json.JsonConstructor]
		public CreateCheatToolsPreset(string TitleOfPreset, Item Item1, Item Item2, Item Item3, Item Item4, Item Item5)
		{
			this.TitleOfPreset = TitleOfPreset;
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
			this.Item5 = Item5;
			Normalize();
		}

		private void Normalize()
		{
			if (TitleOfPreset == null)
			{
				string text = (TitleOfPreset = "Empty");
			}
			if (Item1 == null)
			{
				Item item = (Item1 = new Item());
			}
			if (Item2 == null)
			{
				Item item = (Item2 = new Item());
			}
			if (Item3 == null)
			{
				Item item = (Item3 = new Item());
			}
			if (Item4 == null)
			{
				Item item = (Item4 = new Item());
			}
			if (Item5 == null)
			{
				Item item = (Item5 = new Item());
			}
			foreach (Item item7 in Items)
			{
				_ = item7.ModIoID;
			}
		}

		private static void NormalizeCurrentPresetNow()
		{
			if (CurrentPresetNow == null)
			{
				CurrentPresetNow = new CreateCheatToolsPreset("Empty", null, null, null, null, null);
			}
			CurrentPresetNow.Normalize();
		}

		private Item GetItem(int slotNumber)
		{
			if (1 == 0)
			{
			}
			Item result = slotNumber switch
			{
				1 => Item1, 
				2 => Item2, 
				3 => Item3, 
				4 => Item4, 
				5 => Item5, 
				_ => null, 
			};
			if (1 == 0)
			{
			}
			return result;
		}

		public static void LoadPresets()
		{
			try
			{
				if (File.Exists(devitems))
				{
					string value = File.ReadAllText(devitems);
					System.Collections.Generic.List<CreateCheatToolsPreset> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<CreateCheatToolsPreset>>(value);
					if (list != null)
					{
						CheatPresets = list;
					}
				}
				if (File.Exists(devitemscurrent))
				{
					string value2 = File.ReadAllText(devitemscurrent);
					CreateCheatToolsPreset createCheatToolsPreset = null;
					try
					{
						createCheatToolsPreset = JsonConvert.DeserializeObject<CreateCheatToolsPreset>(value2);
					}
					catch (JsonSerializationException)
					{
						System.Collections.Generic.List<CreateCheatToolsPreset> list2 = JsonConvert.DeserializeObject<System.Collections.Generic.List<CreateCheatToolsPreset>>(value2);
						if (list2 != null && list2.Count > 0)
						{
							createCheatToolsPreset = list2[0];
						}
					}
					if (createCheatToolsPreset != null)
					{
						CurrentPresetNow = createCheatToolsPreset;
						CurrentPresetNow.Normalize();
					}
				}
				NormalizeCurrentPresetNow();
				if (InstanceOfIt != null && CurrentPresetNow != null)
				{
					InstanceOfIt.crates = CurrentPresetNow.Items.Select((Item i) => new SpawnableCrateReference(i.BarcodeId)).ToArray();
				}
			}
			catch (Exception value3)
			{
				MelonLogger.Error($"Load Presets Failed : {value3}");
			}
		}

		public static void SavePresets()
		{
			try
			{
				if (CheatPresets == null)
				{
					CheatPresets = new System.Collections.Generic.List<CreateCheatToolsPreset>();
				}
				foreach (CreateCheatToolsPreset cheatPreset in CheatPresets)
				{
					cheatPreset.Normalize();
				}
				NormalizeCurrentPresetNow();
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					FloatFormatHandling = FloatFormatHandling.Symbol,
					Culture = CultureInfo.InvariantCulture,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				File.WriteAllText(devitems, JsonConvert.SerializeObject(CheatPresets, settings));
				File.WriteAllText(devitemscurrent, JsonConvert.SerializeObject(CurrentPresetNow, settings));
			}
			catch (Exception value)
			{
				MelonLogger.Error($"Failed to save Presets: {value}");
			}
		}

		public void EditDevSlot(int slotNumber, string newValue)
		{
			Item item = GetItem(slotNumber);
			if (item != null)
			{
				item.BarcodeId = newValue;
				SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(newValue);
				item.SpawnableName = StripColorTags(spawnableCrateReference?.Crate?.name) ?? "Unknown";
				item.ModIoID = CrateFilterer.GetModID(spawnableCrateReference.Crate.Pallet);
				NotificationNow("Nemesis Anti-Cheat", $"Edited Dev Slot {slotNumber} With {item.SpawnableName}", NotificationType.SUCCESS, 1.5f);
				SavePresets();
				RefreshIt();
			}
		}

		public void EditPresetName(string newValue)
		{
			if (!CheatPresets.Any((CreateCheatToolsPreset X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public void EditSlotLocalSpawn(int slotNumber, bool localspawningvalue)
		{
			Item item = GetItem(slotNumber);
			if (item != null)
			{
				item.LocalSpawn = localspawningvalue;
				NotificationNow("Nemesis Anti-Cheat", $"Edited Dev Slot [{slotNumber}] With Local Spawning Value [{localspawningvalue}]", NotificationType.SUCCESS, 1.5f);
				SavePresets();
				RefreshIt();
			}
		}

		public void ClearDevSlot(int slotNumber)
		{
			Item item = GetItem(slotNumber);
			if (item != null)
			{
				item.BarcodeId = "Empty";
				item.SpawnableName = "Empty";
				item.LocalSpawn = false;
				item.ModIoID = -1;
				NotificationNow("Nemesis Anti-Cheat", $"Cleared Dev Slot {slotNumber}!", NotificationType.SUCCESS, 1.5f);
				SavePresets();
				RefreshIt();
			}
		}

		public void RefreshIt()
		{
			CurrentPresetNow = this;
			InstanceOfIt.crates = Items.Select((Item i) => new SpawnableCrateReference(i.BarcodeId)).ToArray();
			NotificationNow("Nemesis Anti-Cheat", "Refreshed Preset & Applied!", NotificationType.SUCCESS, 1.5f);
		}

		public string GetDevBarcode(int slotNumber)
		{
			return GetItem(slotNumber)?.BarcodeId;
		}

		public void RemovePreset()
		{
			CreateCheatToolsPreset createCheatToolsPreset = CheatPresets.FirstOrDefault((CreateCheatToolsPreset p) => p.TitleOfPreset == TitleOfPreset);
			if (createCheatToolsPreset != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Remove Preset " + createCheatToolsPreset.TitleOfPreset, NotificationType.SUCCESS, 1.5f);
				CheatPresets.Remove(createCheatToolsPreset);
				SavePresets();
			}
		}
	}

	internal class BodyLogPage
	{
		public static System.Collections.Generic.List<BodyLogPage> BodyLogPages = new System.Collections.Generic.List<BodyLogPage>();

		public static readonly string Bodylogpages = Path.Combine(MelonEnvironment.UserDataDirectory, "NemesisAntiCheatBodyLogPages.json");

		private int? _modIoID1;

		private int? _modIoID2;

		private int? _modIoID3;

		private int? _modIoID4;

		private int? _modIoID5;

		private int? _modIoID6;

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public string Slot1 { get; set; }

		[JsonProperty]
		public string Slot2 { get; set; }

		[JsonProperty]
		public string Slot3 { get; set; }

		[JsonProperty]
		public string Slot4 { get; set; }

		[JsonProperty]
		public string Slot5 { get; set; }

		[JsonProperty]
		public string Slot6 { get; set; }

		[JsonProperty]
		public int ModIoID1
		{
			get
			{
				return RepairModId(ref _modIoID1, Slot1);
			}
			set
			{
				_modIoID1 = value;
			}
		}

		[JsonProperty]
		public int ModIoID2
		{
			get
			{
				return RepairModId(ref _modIoID2, Slot2);
			}
			set
			{
				_modIoID2 = value;
			}
		}

		[JsonProperty]
		public int ModIoID3
		{
			get
			{
				return RepairModId(ref _modIoID3, Slot3);
			}
			set
			{
				_modIoID3 = value;
			}
		}

		[JsonProperty]
		public int ModIoID4
		{
			get
			{
				return RepairModId(ref _modIoID4, Slot4);
			}
			set
			{
				_modIoID4 = value;
			}
		}

		[JsonProperty]
		public int ModIoID5
		{
			get
			{
				return RepairModId(ref _modIoID5, Slot5);
			}
			set
			{
				_modIoID5 = value;
			}
		}

		[JsonProperty]
		public int ModIoID6
		{
			get
			{
				return RepairModId(ref _modIoID6, Slot6);
			}
			set
			{
				_modIoID6 = value;
			}
		}

		[Newtonsoft.Json.JsonConstructor]
		public BodyLogPage(string TitleOfPreset, string Slot1, string Slot2, string Slot3, string Slot4, string Slot5, string Slot6, int? ModIoID1 = null, int? ModIoID2 = null, int? ModIoID3 = null, int? ModIoID4 = null, int? ModIoID5 = null, int? ModIoID6 = null)
		{
			this.TitleOfPreset = TitleOfPreset ?? "Empty";
			this.Slot1 = Slot1;
			this.Slot2 = Slot2;
			this.Slot3 = Slot3;
			this.Slot4 = Slot4;
			this.Slot5 = Slot5;
			this.Slot6 = Slot6;
			_modIoID1 = ModIoID1;
			_modIoID2 = ModIoID2;
			_modIoID3 = ModIoID3;
			_modIoID4 = ModIoID4;
			_modIoID5 = ModIoID5;
			_modIoID6 = ModIoID6;
		}

		private int RepairModId(ref int? storedId, string barcode)
		{
			if (string.IsNullOrEmpty(barcode) || barcode == "c3534c5a-94b2-40a4-912a-24a8506f6c79")
			{
				return -1;
			}
			if (storedId.HasValue && storedId.Value > 0)
			{
				return storedId.Value;
			}
			try
			{
				Pallet pallet = new SpawnableCrateReference(barcode)?.Crate?.Pallet;
				int modID = CrateFilterer.GetModID(pallet);
				storedId = modID;
				return modID;
			}
			catch
			{
				return -1;
			}
		}

		private string GetBarcode(int slot)
		{
			if (1 == 0)
			{
			}
			string result = slot switch
			{
				1 => Slot1, 
				2 => Slot2, 
				3 => Slot3, 
				4 => Slot4, 
				5 => Slot5, 
				6 => Slot6, 
				_ => null, 
			};
			if (1 == 0)
			{
			}
			return result;
		}

		private int GetModId(int slot)
		{
			if (1 == 0)
			{
			}
			int result = slot switch
			{
				1 => ModIoID1, 
				2 => ModIoID2, 
				3 => ModIoID3, 
				4 => ModIoID4, 
				5 => ModIoID5, 
				6 => ModIoID6, 
				_ => -1, 
			};
			if (1 == 0)
			{
			}
			return result;
		}

		private void SetModId(int slot, int id)
		{
			switch (slot)
			{
			case 1:
				ModIoID1 = id;
				break;
			case 2:
				ModIoID2 = id;
				break;
			case 3:
				ModIoID3 = id;
				break;
			case 4:
				ModIoID4 = id;
				break;
			case 5:
				ModIoID5 = id;
				break;
			case 6:
				ModIoID6 = id;
				break;
			}
		}

		public void EditPresetName(string newValue)
		{
			if (!BodyLogPages.Any((BodyLogPage X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public void ApplyPreset()
		{
			for (int i = 1; i <= 6; i++)
			{
				string barcode = GetBarcode(i);
				int modId = GetModId(i);
				if (string.IsNullOrEmpty(barcode) || barcode == "c3534c5a-94b2-40a4-912a-24a8506f6c79" || modId <= 0)
				{
					continue;
				}
				try
				{
					if (!IsAvatarCrateExist(barcode))
					{
						DownloadModIOMod(modId, noti: false);
						MelonLogger.Msg($"Downloading missing ModIO mod {modId} for barcode {barcode}");
					}
				}
				catch (Exception value)
				{
					MelonLogger.Error($"Failed to process slot {i} with ModIO {modId}: {value}");
				}
			}
		}

		public void EditSlot(int slotNumber, string newValue)
		{
			switch (slotNumber)
			{
			default:
				return;
			case 1:
				Slot1 = newValue;
				break;
			case 2:
				Slot2 = newValue;
				break;
			case 3:
				Slot3 = newValue;
				break;
			case 4:
				Slot4 = newValue;
				break;
			case 5:
				Slot5 = newValue;
				break;
			case 6:
				Slot6 = newValue;
				break;
			}
			SetModId(slotNumber, -1);
			SavePresets();
			NotificationNow("Nemesis Anti-Cheat", "Edited Slot With " + newValue, NotificationType.SUCCESS, 3.5f);
		}

		public void ClearSlot(int slotNumber)
		{
			switch (slotNumber)
			{
			default:
				return;
			case 1:
				Slot1 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			case 2:
				Slot2 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			case 3:
				Slot3 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			case 4:
				Slot4 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			case 5:
				Slot5 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			case 6:
				Slot6 = "c3534c5a-94b2-40a4-912a-24a8506f6c79";
				break;
			}
			SetModId(slotNumber, -1);
			SavePresets();
			NotificationNow("Nemesis Anti-Cheat", "Cleared Slot!", NotificationType.SUCCESS, 3.5f);
		}

		public void RemovePreset()
		{
			BodyLogPage bodyLogPage = BodyLogPages.FirstOrDefault((BodyLogPage p) => p.TitleOfPreset == TitleOfPreset);
			if (bodyLogPage != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Remove Preset " + bodyLogPage.TitleOfPreset, NotificationType.SUCCESS, 3.5f);
				BodyLogPages.Remove(bodyLogPage);
				SavePresets();
			}
		}

		public static void LoadPresets()
		{
			try
			{
				if (File.Exists(Bodylogpages))
				{
					string value = File.ReadAllText(Bodylogpages);
					System.Collections.Generic.List<BodyLogPage> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<BodyLogPage>>(value);
					if (list != null)
					{
						BodyLogPages = list;
					}
				}
			}
			catch (Exception value2)
			{
				MelonLogger.Error($"Load BodyLogs Failed : {value2}");
			}
		}

		public static void SavePresets()
		{
			try
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					FloatFormatHandling = FloatFormatHandling.Symbol,
					Culture = CultureInfo.InvariantCulture,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				string contents = JsonConvert.SerializeObject(BodyLogPages, settings);
				File.WriteAllText(Bodylogpages, contents);
			}
			catch (Exception value)
			{
				MelonLogger.Error($"Failed to save Pages: {value}");
			}
		}
	}

	internal class InventoryPage
	{
		public static System.Collections.Generic.List<InventoryPage> InventoryPresets = new System.Collections.Generic.List<InventoryPage>();

		public static System.Collections.Generic.Dictionary<string, string> CurrentPreset;

		public static readonly string PresetsFile = Path.Combine(MelonEnvironment.UserDataDirectory, "InventoryPresets.json");

		public static readonly string PresetsFileCurrent = Path.Combine(MelonEnvironment.UserDataDirectory, "FusionInventoryPresetsCurrent.json");

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public System.Collections.Generic.Dictionary<string, string> Slots { get; set; }

		[Newtonsoft.Json.JsonConstructor]
		public InventoryPage(string TitleOfPreset, System.Collections.Generic.Dictionary<string, string> Slots)
		{
			this.TitleOfPreset = TitleOfPreset;
			this.Slots = Slots;
		}

		private void NormalizeSlots()
		{
			if (Slots == null)
			{
				Slots = new System.Collections.Generic.Dictionary<string, string>();
				return;
			}
			System.Collections.Generic.List<string> list = Slots.Keys.ToList();
			foreach (string item in list)
			{
				if (Slots[item] == null)
				{
					Slots[item] = "Empty";
				}
			}
		}

		public void SaveToFile()
		{
			NormalizeSlots();
			InventoryPage inventoryPage = InventoryPresets.FirstOrDefault((InventoryPage p) => p.TitleOfPreset == TitleOfPreset);
			if (inventoryPage == null)
			{
				InventoryPresets.Add(this);
			}
			try
			{
				string contents = JsonConvert.SerializeObject(InventoryPresets, Formatting.Indented);
				File.WriteAllText(PresetsFile, contents);
				string contents2 = JsonConvert.SerializeObject(PresetsFileCurrent, Formatting.Indented);
				File.WriteAllText(PresetsFileCurrent, contents2);
			}
			catch (Exception value)
			{
				MelonLogger.Error($"Failed to save presets: {value}");
			}
		}

		public void EditPresetName(string newValue)
		{
			if (!InventoryPresets.Any((InventoryPage X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SaveAllPresetsToFile();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public static void LoadAllPresets()
		{
			try
			{
				if (!File.Exists(PresetsFile))
				{
					return;
				}
				string value = File.ReadAllText(PresetsFile);
				InventoryPresets = JsonConvert.DeserializeObject<System.Collections.Generic.List<InventoryPage>>(value) ?? new System.Collections.Generic.List<InventoryPage>();
				if (File.Exists(PresetsFileCurrent))
				{
					string value2 = File.ReadAllText(PresetsFileCurrent);
					System.Collections.Generic.Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(value2);
					if (dictionary != null)
					{
						CurrentPreset = dictionary;
					}
				}
			}
			catch (Exception value3)
			{
				MelonLogger.Error($"Failed to load presets: {value3}");
			}
		}

		public void LoadIntoPlayer(bool notificationnow = true)
		{
			if (Player.RigManager == null)
			{
				return;
			}
			InventorySlotReceiver inventorySlotReceiver = Player.RigManager.physicsRig.m_head.transform?.Find("HeadSlotContainer/WeaponReciever_01")?.GetComponent<InventorySlotReceiver>();
			SlotContainer[] source = Player.RigManager.inventory.bodySlots.Concat(Player.RigManager.inventory.specialItems).ToArray();
			foreach (System.Collections.Generic.KeyValuePair<string, string> slot in Slots)
			{
				string slotName = slot.Key;
				string value = slot.Value;
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}
				if (slotName == "Head" && inventorySlotReceiver != null)
				{
					inventorySlotReceiver.DropWeapon();
					inventorySlotReceiver.SpawnInSlotAsync(new Barcode(value));
					continue;
				}
				InventorySlotReceiver inventorySlotReceiver2 = source.FirstOrDefault((SlotContainer s) => s.name == slotName)?.inventorySlotReceiver;
				if (inventorySlotReceiver2 != null)
				{
					inventorySlotReceiver2.DropWeapon();
					inventorySlotReceiver2.SpawnInSlotAsync(new Barcode(value));
				}
			}
			if (notificationnow)
			{
				NotificationNow("Nemesis Anti-Cheat", "Loadout [" + TitleOfPreset.Trim() + "] Loaded Into Player!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public static InventoryPage CaptureFromCurrentInventory(string title)
		{
			Inventory inventory = Player.RigManager?.inventory;
			if (inventory == null)
			{
				return null;
			}
			if (InventoryPresets.Any((InventoryPage p) => !string.IsNullOrEmpty(p.TitleOfPreset) && p.TitleOfPreset.Trim().Equals(title.Trim(), StringComparison.OrdinalIgnoreCase)))
			{
				NotificationNow("Nemesis Anti-Cheat", "Preset [" + title.Trim() + "] Already Exist!", NotificationType.SUCCESS, 3.5f);
				return null;
			}
			System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>();
			string value = (Player.RigManager.physicsRig.m_head.transform?.Find("HeadSlotContainer/WeaponReciever_01")?.GetComponent<InventorySlotReceiver>())?._slottedWeapon?.interactableHost?.marrowEntity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID;
			dictionary["Head"] = value;
			System.Collections.Generic.IEnumerable<SlotContainer> enumerable = inventory.bodySlots.Concat(inventory.specialItems);
			foreach (SlotContainer item in enumerable)
			{
				string value2 = (item?.inventorySlotReceiver)?._slottedWeapon?.interactableHost?.marrowEntity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID;
				dictionary[item.name] = value2;
			}
			InventoryPage inventoryPage = new InventoryPage(title, dictionary);
			InventoryPresets.Add(inventoryPage);
			inventoryPage.SaveToFile();
			CurrentPreset = dictionary;
			try
			{
				string contents = JsonConvert.SerializeObject(CurrentPreset, Formatting.Indented);
				File.WriteAllText(PresetsFileCurrent, contents);
				NotificationNow("Nemesis Anti-Cheat", "Saved Loadout [" + title.Trim() + "]!", NotificationType.SUCCESS, 3.5f);
			}
			catch (Exception value3)
			{
				MelonLogger.Error($"Failed to save current preset: {value3}");
			}
			return inventoryPage;
		}

		public void RemovePreset()
		{
			InventoryPage inventoryPage = InventoryPresets.FirstOrDefault((InventoryPage p) => p.TitleOfPreset == TitleOfPreset);
			if (inventoryPage != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Remove Preset " + inventoryPage.TitleOfPreset, NotificationType.SUCCESS, 3.5f);
				InventoryPresets.Remove(inventoryPage);
				SaveAllPresetsToFile();
			}
		}

		public static void SaveAllPresetsToFile()
		{
			try
			{
				string contents = JsonConvert.SerializeObject(InventoryPresets, Formatting.Indented);
				File.WriteAllText(PresetsFile, contents);
			}
			catch (Exception value)
			{
				MelonLogger.Error($"Failed to save presets: {value}");
			}
		}

		public string GetSlotBarcode(string slotName)
		{
			if (string.IsNullOrEmpty(slotName))
			{
				return null;
			}
			if (Slots == null)
			{
				return null;
			}
			Slots.TryGetValue(slotName, out var value);
			return value;
		}

		public void EditSlotBarcode(string slotName, string newBarcode)
		{
			if (!string.IsNullOrEmpty(slotName))
			{
				InventoryPage inventoryPage = InventoryPresets.FirstOrDefault((InventoryPage p) => p.TitleOfPreset == TitleOfPreset);
				if (inventoryPage != null && inventoryPage.Slots.ContainsKey(slotName))
				{
					inventoryPage.Slots[slotName] = newBarcode;
					string text = new SpawnableCrateReference(newBarcode)?.Crate?.name ?? "Empty";
					NotificationNow("Nemesis Anti-Cheat", "Added " + text + " To " + slotName, NotificationType.SUCCESS, 3.5f);
					SaveAllPresetsToFile();
				}
			}
		}
	}

	internal class BodyLogRadialMenuColorPreset
	{
		public static string[] CurrentPreset;

		public static System.Collections.Generic.List<BodyLogRadialMenuColorPreset> ColorPresets = new System.Collections.Generic.List<BodyLogRadialMenuColorPreset>();

		public static readonly string ColorsCurrent = Path.Combine(MelonEnvironment.UserDataDirectory, "ColorsCurrent.json");

		public static readonly string ColorsPresets = Path.Combine(MelonEnvironment.UserDataDirectory, "ColorPresetsNow.json");

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public float BodyLogColor_R { get; set; }

		[JsonProperty]
		public float BodyLogColor_G { get; set; }

		[JsonProperty]
		public float BodyLogColor_B { get; set; }

		[JsonProperty]
		public float BodyLogColor_A { get; set; }

		[JsonProperty]
		public float BodyLogBallColor_R { get; set; }

		[JsonProperty]
		public float BodyLogBallColor_G { get; set; }

		[JsonProperty]
		public float BodyLogBallColor_B { get; set; }

		[JsonProperty]
		public float BodyLogBallColor_A { get; set; }

		[JsonProperty]
		public float BodyLogLineColor_R { get; set; }

		[JsonProperty]
		public float BodyLogLineColor_G { get; set; }

		[JsonProperty]
		public float BodyLogLineColor_B { get; set; }

		[JsonProperty]
		public float BodyLogLineColor_A { get; set; }

		[JsonProperty]
		public float RadialMenuColor_R { get; set; }

		[JsonProperty]
		public float RadialMenuColor_G { get; set; }

		[JsonProperty]
		public float RadialMenuColor_B { get; set; }

		[JsonProperty]
		public float RadialMenuColor_A { get; set; }

		[Newtonsoft.Json.JsonConstructor]
		public BodyLogRadialMenuColorPreset(string TitleOfPreset, float BodyLogColor_R, float BodyLogColor_G, float BodyLogColor_B, float BodyLogColor_A, float BodyLogBallColor_R, float BodyLogBallColor_G, float BodyLogBallColor_B, float BodyLogBallColor_A, float BodyLogLineColor_R, float BodyLogLineColor_G, float BodyLogLineColor_B, float BodyLogLineColor_A, float RadialMenuColor_R, float RadialMenuColor_G, float RadialMenuColor_B, float RadialMenuColor_A)
		{
			this.TitleOfPreset = TitleOfPreset;
			this.BodyLogColor_R = BodyLogColor_R;
			this.BodyLogColor_G = BodyLogColor_G;
			this.BodyLogColor_B = BodyLogColor_B;
			this.BodyLogColor_A = BodyLogColor_A;
			this.BodyLogBallColor_R = BodyLogBallColor_R;
			this.BodyLogBallColor_G = BodyLogBallColor_G;
			this.BodyLogBallColor_B = BodyLogBallColor_B;
			this.BodyLogBallColor_A = BodyLogBallColor_A;
			this.BodyLogLineColor_R = BodyLogLineColor_R;
			this.BodyLogLineColor_G = BodyLogLineColor_G;
			this.BodyLogLineColor_B = BodyLogLineColor_B;
			this.BodyLogLineColor_A = BodyLogLineColor_A;
			this.RadialMenuColor_R = RadialMenuColor_R;
			this.RadialMenuColor_G = RadialMenuColor_G;
			this.RadialMenuColor_B = RadialMenuColor_B;
			this.RadialMenuColor_A = RadialMenuColor_A;
		}

		public static void LoadPresets()
		{
			try
			{
				if (File.Exists(ColorsPresets))
				{
					string value = File.ReadAllText(ColorsPresets);
					System.Collections.Generic.List<BodyLogRadialMenuColorPreset> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<BodyLogRadialMenuColorPreset>>(value);
					if (list != null)
					{
						ColorPresets = list;
					}
				}
				if (File.Exists(ColorsCurrent))
				{
					string value2 = File.ReadAllText(ColorsCurrent);
					string[] array = JsonConvert.DeserializeObject<string[]>(value2);
					if (array != null)
					{
						CurrentPreset = array;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to load presets: " + ex.Message);
			}
		}

		public static void SavePresets()
		{
			try
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					Culture = CultureInfo.InvariantCulture
				};
				string contents = JsonConvert.SerializeObject(ColorPresets, settings);
				File.WriteAllText(ColorsPresets, contents);
				string contents2 = JsonConvert.SerializeObject(CurrentPreset, settings);
				File.WriteAllText(ColorsCurrent, contents2);
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to save presets: " + ex.Message);
			}
		}

		public void EditPresetName(string newValue)
		{
			if (!ColorPresets.Any((BodyLogRadialMenuColorPreset X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public void RemovePreset()
		{
			BodyLogRadialMenuColorPreset bodyLogRadialMenuColorPreset = ColorPresets.FirstOrDefault((BodyLogRadialMenuColorPreset p) => p.TitleOfPreset == TitleOfPreset);
			if (bodyLogRadialMenuColorPreset != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Removed preset " + bodyLogRadialMenuColorPreset.TitleOfPreset, NotificationType.SUCCESS, 3.5f);
				ColorPresets.Remove(bodyLogRadialMenuColorPreset);
				SavePresets();
			}
		}

		public float GetValue(string propertyName)
		{
			PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.PropertyType == typeof(float))
			{
				return (float)property.GetValue(this);
			}
			throw new ArgumentException("Property " + propertyName + " not found or not float.");
		}

		public void SetValue(string propertyName, float value)
		{
			PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.PropertyType == typeof(float))
			{
				property.SetValue(this, value);
				return;
			}
			throw new ArgumentException("Property " + propertyName + " not found or not float.");
		}
	}

	internal class StatsKickerPresets
	{
		public static string[] CurrentPreset;

		public static System.Collections.Generic.List<StatsKickerPresets> StatsKickerPresetz = new System.Collections.Generic.List<StatsKickerPresets>();

		public static readonly string StatsKickerPresetsNow = Path.Combine(MelonEnvironment.UserDataDirectory, "FusionStatKickerPresets.json");

		public static readonly string StatsKickerCurrent = Path.Combine(MelonEnvironment.UserDataDirectory, "FusionStatKickerCurrents.json");

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public string Height { get; set; }

		[JsonProperty]
		public string MassArm { get; set; }

		[JsonProperty]
		public string MassChest { get; set; }

		[JsonProperty]
		public string MassHead { get; set; }

		[JsonProperty]
		public string MassLeg { get; set; }

		[JsonProperty]
		public string MassPelvis { get; set; }

		[JsonProperty]
		public string MassTotal { get; set; }

		[JsonProperty]
		public string Speed { get; set; }

		[JsonProperty]
		public string StrengthLower { get; set; }

		[JsonProperty]
		public string StrengthUpper { get; set; }

		[JsonProperty]
		public string Vitality { get; set; }

		[Newtonsoft.Json.JsonConstructor]
		public StatsKickerPresets(string TitleOfPreset, string height, string massArm, string massChest, string massHead, string massLeg, string massPelvis, string massTotal, string speed, string strengthLower, string strengthUpper, string vitality)
		{
			this.TitleOfPreset = TitleOfPreset;
			Height = height;
			MassArm = massArm;
			MassChest = massChest;
			MassHead = massHead;
			MassLeg = massLeg;
			MassPelvis = massPelvis;
			MassTotal = massTotal;
			Speed = speed;
			StrengthLower = strengthLower;
			StrengthUpper = strengthUpper;
			Vitality = vitality;
		}

		public void EditPresetName(string newValue)
		{
			if (!StatsKickerPresetz.Any((StatsKickerPresets X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public static void LoadPresets()
		{
			try
			{
				if (File.Exists(StatsKickerPresetsNow))
				{
					string value = File.ReadAllText(StatsKickerPresetsNow);
					System.Collections.Generic.List<StatsKickerPresets> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<StatsKickerPresets>>(value, new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						FloatFormatHandling = FloatFormatHandling.Symbol,
						Culture = CultureInfo.InvariantCulture,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					});
					if (list != null)
					{
						StatsKickerPresetz = list;
					}
				}
				if (File.Exists(StatsKickerCurrent))
				{
					string value2 = File.ReadAllText(StatsKickerCurrent);
					CurrentPreset = JsonConvert.DeserializeObject<string[]>(value2);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to load presets: " + ex.Message);
			}
		}

		public static void SavePresets()
		{
			try
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					FloatFormatHandling = FloatFormatHandling.Symbol,
					Culture = CultureInfo.InvariantCulture,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				string contents = JsonConvert.SerializeObject(StatsKickerPresetz, settings);
				File.WriteAllText(StatsKickerPresetsNow, contents);
				if (CurrentPreset != null)
				{
					string contents2 = JsonConvert.SerializeObject(CurrentPreset, settings);
					File.WriteAllText(StatsKickerCurrent, contents2);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to save presets: " + ex.Message);
			}
		}

		public void RemovePreset()
		{
			StatsKickerPresets statsKickerPresets = StatsKickerPresetz.FirstOrDefault((StatsKickerPresets p) => p.TitleOfPreset == TitleOfPreset);
			if (statsKickerPresets != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Removed Preset " + statsKickerPresets.TitleOfPreset, NotificationType.SUCCESS, 3.5f);
				StatsKickerPresetz.Remove(statsKickerPresets);
				SavePresets();
			}
		}
	}

	internal class FusionProfilePresets
	{
		public static string[] CurrentPreset;

		public static System.Collections.Generic.List<FusionProfilePresets> ProfilePresets = new System.Collections.Generic.List<FusionProfilePresets>();

		public static readonly string PresetsPath = Path.Combine(MelonEnvironment.UserDataDirectory, "FusionPresetsNow.json");

		public static bool IsApplying;

		[JsonProperty]
		public string TitleOfPreset { get; set; }

		[JsonProperty]
		public string Nickname { get; set; }

		[JsonProperty]
		public string Description { get; set; }

		[JsonProperty]
		public string AvatarAtTheTime { get; set; }

		[JsonProperty]
		public System.Collections.Generic.List<string> BitMartItems { get; set; }

		[Newtonsoft.Json.JsonConstructor]
		public FusionProfilePresets(string TitleOfPreset, string Nickname, string Description, string AvatarAtTheTime, System.Collections.Generic.List<string> BitMartItems)
		{
			this.TitleOfPreset = TitleOfPreset;
			this.Nickname = Nickname;
			this.Description = Description;
			this.AvatarAtTheTime = AvatarAtTheTime;
			this.BitMartItems = BitMartItems;
		}

		public static void LoadPresets()
		{
			try
			{
				if (File.Exists(PresetsPath))
				{
					string value = File.ReadAllText(PresetsPath);
					System.Collections.Generic.List<FusionProfilePresets> list = JsonConvert.DeserializeObject<System.Collections.Generic.List<FusionProfilePresets>>(value);
					if (list != null)
					{
						ProfilePresets = list;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to load presets: " + ex.Message);
			}
		}

		public static void SavePresets()
		{
			try
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					Culture = CultureInfo.InvariantCulture
				};
				File.WriteAllText(PresetsPath, JsonConvert.SerializeObject(ProfilePresets, settings));
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Failed to save presets: " + ex.Message);
			}
		}

		public IEnumerator ApplyPreset()
		{
			if (IsApplying)
			{
				NotificationNow("Nemesis Anti-Cheat", "Preset is already applying, please wait...", NotificationType.WARNING, 2.5f);
				yield break;
			}
			IsApplying = true;
			NetworkPlayer player = ND_YourNetworkPlayer();
			if (player == null)
			{
				MelonLogger.Warning("ApplyPreset aborted: player was null.");
				IsApplying = false;
				yield break;
			}
			try
			{
				PointItemManager.UnequipAll();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				MelonLogger.Error($"ApplyPreset step failed: {ex2}");
			}
			yield return new WaitForSecondsRealtime(1.5f);
			try
			{
				player.PlayerID.Metadata.Nickname.SetValue(Nickname);
			}
			catch (Exception ex)
			{
				Exception ex3 = ex;
				MelonLogger.Error($"ApplyPreset step failed: {ex3}");
			}
			yield return new WaitForSecondsRealtime(1.5f);
			try
			{
				player.PlayerID.Metadata.Description.SetValue(Description);
			}
			catch (Exception ex)
			{
				Exception ex4 = ex;
				MelonLogger.Error($"ApplyPreset step failed: {ex4}");
			}
			yield return new WaitForSecondsRealtime(1.5f);
			try
			{
				if (AvatarAtTheTime != null)
				{
					ChangeIntoAvi(AvatarAtTheTime);
				}
			}
			catch (Exception ex)
			{
				Exception ex5 = ex;
				MelonLogger.Error($"ApplyPreset step failed: {ex5}");
			}
			yield return new WaitForSecondsRealtime(1.5f);
			if (BitMartItems != null)
			{
				foreach (string barcode in BitMartItems)
				{
					if (!string.IsNullOrWhiteSpace(barcode))
					{
						if (PointItemManager.TryGetPointItem(barcode, out var pointy) && pointy != null)
						{
							MelonLogger.Msg("Equipping item: " + pointy.Barcode);
							PointItemManager.SetEquipped(pointy, isEquipped: true);
							yield return new WaitForSecondsRealtime(0.5f);
						}
						pointy = null;
					}
				}
			}
			try
			{
				EditFusionPreferences("Nickname", Nickname);
				EditFusionPreferences("Description", Description);
			}
			catch (Exception value)
			{
				MelonLogger.Error($"ApplyPreset step failed: {value}");
			}
			yield return new WaitForSecondsRealtime(1f);
			NotificationNow("Nemesis Anti-Cheat", "Applied Preset " + TitleOfPreset, NotificationType.SUCCESS, 3.5f);
			IsApplying = false;
		}

		public void EditPresetName(string newValue)
		{
			if (!ProfilePresets.Any((FusionProfilePresets X) => X.TitleOfPreset == newValue))
			{
				TitleOfPreset = newValue;
				SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Changed Preset Name To [" + TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			}
		}

		public string GetValue(string propertyName)
		{
			PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.PropertyType == typeof(string))
			{
				return (string)property.GetValue(this);
			}
			throw new ArgumentException("Property " + propertyName + " not found or not string.");
		}

		public void SetValue(string propertyName, string value)
		{
			PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.PropertyType == typeof(string))
			{
				property.SetValue(this, value);
				return;
			}
			throw new ArgumentException("Property " + propertyName + " not found or not string.");
		}

		public void RemovePreset()
		{
			FusionProfilePresets fusionProfilePresets = ProfilePresets.FirstOrDefault((FusionProfilePresets p) => p.TitleOfPreset == TitleOfPreset);
			if (fusionProfilePresets != null)
			{
				NotificationNow("Nemesis Anti-Cheat", "Removed preset " + fusionProfilePresets.TitleOfPreset, NotificationType.SUCCESS, 3.5f);
				ProfilePresets.Remove(fusionProfilePresets);
				SavePresets();
			}
		}
	}

	[HarmonyPatch(typeof(PlayerRepDamageMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class AntiOneShotNonHost
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (AreYouOPERATOR() && antioneshot && !GamemodeManager.IsGamemodeStarted && !NetworkInfo.IsHost)
			{
				PlayerRepDamageData playerRepDamageData = received.ReadData<PlayerRepDamageData>();
				byte? sender = received.Sender;
				if (!sender.HasValue)
				{
					return true;
				}
				if (NetworkPlayerManager.TryGetPlayer(sender.Value, out var player))
				{
					float damage = playerRepDamageData.Attack.Attack.damage;
					float max_Health = player.RigRefs.Health.max_Health;
					MelonLogger.Msg($"Damage From {CleanedNAME(player)} Damage : {damage} | Max Damage They Can Do : {max_Health:F1}");
					if (damage > max_Health)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerRepTeleportMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class AntiModerationTeleport
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (AntiModTP)
			{
				return false;
			}
			return true;
		}
	}

	internal static class HidePlayerState
	{
		public static int CurrentCount = 5;
	}

	[HarmonyPatch(typeof(LobbyInfo), "get_MaxPlayers")]
	[HarmonyPriority(2147483646)]
	internal static class HidePlayerList3
	{
		public static bool Prefix(LobbyInfo __instance, ref int __result)
		{
			if (!HIDEPLAYERLIST || __instance != LobbyInfoManager.LobbyInfo)
			{
				return true;
			}
			__result = HidePlayerState.CurrentCount + 3;
			return false;
		}
	}

	[HarmonyPatch(typeof(PlayerIDManager), "get_PlayerCount")]
	[HarmonyPriority(2147483646)]
	internal static class HidePlayerList2
	{
		public static bool Prefix(ref int __result)
		{
			if (!HIDEPLAYERLIST)
			{
				return true;
			}
			__result = HidePlayerState.CurrentCount + 1;
			return false;
		}
	}

	[HarmonyPatch(typeof(PlayerList), "WritePlayers")]
	[HarmonyPriority(2147483646)]
	internal static class HidePlayerList
	{
		public static bool Prefix(PlayerList __instance)
		{
			if (!HIDEPLAYERLIST)
			{
				return true;
			}
			PlayerInfo localPlayer = new PlayerInfo
			{
				Username = ND_YourNetworkPlayer().ND_Username(),
				Nickname = ND_YourNetworkPlayer().ND_Nickname(),
				PlatformID = ND_YourNetworkPlayer().ND_SteamID(),
				Description = ND_YourNetworkPlayer().ND_Description(),
				AvatarModID = ND_YourNetworkPlayer().PlayerID.Metadata.AvatarModID.GetValue(),
				AvatarTitle = ND_YourNetworkPlayer().PlayerID.Metadata.AvatarTitle.GetValue(),
				PermissionLevel = PermissionLevel.OWNER
			};
			System.Collections.Generic.List<PlayerInfo> list = (from p in PlayersOnline
				where p.PlatformID != localPlayer.PlatformID && !BanManager.BanList.Bans.Any((BanInfo x) => x.Player.PlatformID == p.PlatformID) && !SiteStuff.globalfpbans.Any((NemesisAntiCheatGlobalBan x) => x.KnownSteamIds.Contains(p.PlatformID.ToString())) && !SiteStuff.fpcpeople.Any((NemesisAntiCheatControversialPeople x) => x.KnownSteamIds.Contains(p.PlatformID.ToString()))
				group p by p.PlatformID into g
				select g.First()).ToList();
			int num = Mathf.Min(HidePlayerState.CurrentCount, list.Count);
			System.Collections.Generic.List<PlayerInfo> list2 = list.OrderBy((PlayerInfo _) => UnityEngine.Random.value).Take(num).ToList();
			foreach (PlayerInfo item in list2)
			{
				item.PermissionLevel = PermissionLevel.DEFAULT;
			}
			__instance.Players = new PlayerInfo[num + 1];
			__instance.Players[0] = localPlayer;
			for (int num2 = 0; num2 < num; num2++)
			{
				__instance.Players[num2 + 1] = list2[num2];
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(ModDownloadManager), "DeleteTemporaryDirectories")]
	internal static class NoDeleteHiddenOrPrivateMods
	{
		public static bool Prefix()
		{
			if (keephiddenmods)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(ModDownloadManager), "get_ModsTempPath")]
	internal static class NoDeleteHiddenOrPrivateMods2
	{
		public static bool Prefix(ref string __result)
		{
			if (keephiddenmods)
			{
				__result = Application.persistentDataPath + "/Mods";
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SpawnablesPanelView), "SelectItem")]
	internal static class DoubleClickSpawn
	{
		private static float lastClickTime;

		private const float doubleClickDelay = 0.35f;

		public static void Postfix()
		{
			if (!spawngunuialways || NemesisAntiCheatPage == null || (bool)ND_YourGetHand(WhichHand.Left).ND_HandGrabbedSpawnGun() || (bool)ND_YourGetHand(WhichHand.Right).ND_HandGrabbedSpawnGun())
			{
				return;
			}
			float time = Time.time;
			if (time - lastClickTime <= 0.35f)
			{
				lastClickTime = 0f;
				string text = (UIRig.Instance?.popUpMenu?.spawnablesPanelView?.selectedObject)?.Barcode?.ID;
				if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
				{
					Transform head = Player.Head;
					if (!(head == null))
					{
						SpawnIt(text, head.transform.position + head.transform.forward * 2f + head.transform.up, Quaternion.identity);
					}
				}
			}
			else
			{
				lastClickTime = time;
			}
		}
	}

	[HarmonyPatch(typeof(ConstraintCreateMessage), "OnHandleMessage")]
	internal static class BlockSpawnDespawnIncludeConstraints1
	{
		public static bool Prefix(ReceivedMessage received)
		{
			if (NetworkInfo.IsHost)
			{
				NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
				if (blockedspawnies.Contains(player.ND_SteamID().ToString()))
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(ConstraintDeleteMessage), "OnHandleMessage")]
	internal static class BlockSpawnDespawnIncludeConstraints2
	{
		internal static bool Prefix(ReceivedMessage received)
		{
			if (NetworkInfo.IsHost)
			{
				NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
				if (blockedspawnies.Contains(player.ND_SteamID().ToString()))
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RigManager), "SwitchAvatar")]
	internal static class OnSwapAvatarPatch2
	{
		public static bool Prefix(RigManager __instance)
		{
			if (aviswitchprotection && (NetworkInfo.IsHost || NetworkInfo.HasServer) && __instance == Player.RigManager)
			{
				PullCordDevice item = ND_BodyLog(Player.PhysicsRig).bodylogreturn;
				if (item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Left) || item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Right))
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RigManager), "SwapAvatarCrate")]
	internal static class OnSwapAvatarPatch3
	{
		public static bool _hasSkippedInitialSwap;

		public static bool Prefix(RigManager __instance, ref Barcode barcode)
		{
			if (aviswitchprotection && (NetworkInfo.IsHost || NetworkInfo.HasServer))
			{
				if (__instance != Player.RigManager)
				{
					return true;
				}
				if (Player.RigManager == null)
				{
					return true;
				}
				if (Player.RigManager.avatar == null || Player.RigManager.physicsRig == null || Player.RigManager.physicsRig.leftHand == null || Player.RigManager.physicsRig.rightHand == null)
				{
					return true;
				}
				if (!_hasSkippedInitialSwap)
				{
					_hasSkippedInitialSwap = true;
					return true;
				}
				AvatarCrateReference avatarCrateReference = new AvatarCrateReference(barcode.ID);
				string barcodenow = barcode.ID;
				PullCordDevice item = ND_BodyLog(Player.PhysicsRig).bodylogreturn;
				if (item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Left) || item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Right))
				{
					return true;
				}
				NotificationNow("Nemesis Anti-Cheat", "Avatar Change Request : " + avatarCrateReference.Crate.name + " | Author " + avatarCrateReference.Crate.Pallet.Author, NotificationType.INFORMATION, 3.5f, showtitle: true, savetomenu: true, delegate
				{
					_hasSkippedInitialSwap = false;
					ChangeIntoAvi(barcodenow);
				});
				barcode = new AvatarCrateReference(Player.RigManager.AvatarCrate.Barcode.ID).Barcode;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RigAvatarSetter), "OnSwapAvatar")]
	internal static class OnSwapAvatarPatch
	{
		public static void Postfix(RigAvatarSetter __instance, bool success)
		{
			if (!success)
			{
				return;
			}
			object obj = typeof(RigAvatarSetter).GetField("_references", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(__instance);
			RigRefs rigReferences = obj as RigRefs;
			NetworkPlayer networkPlayer = NetworkPlayers().FirstOrDefault((NetworkPlayer p) => p.RigRefs?.RigManager == rigReferences.RigManager);
			string iD = rigReferences.RigManager.AvatarCrate.Scannable.Barcode.ID;
			AvatarCrateReference avatarCrateReference = new AvatarCrateReference(iD);
			int modID = CrateFilterer.GetModID(avatarCrateReference.Crate.Pallet);
			if (networkPlayer == null || obj == null)
			{
				return;
			}
			FusionPermissions.FetchPermissionLevel(networkPlayer.ND_SteamID(), out var level, out var _);
			if ((level == PermissionLevel.OWNER || !HostIsMe(networkPlayer)) && HostIsMe(networkPlayer))
			{
				return;
			}
			if (warnavinow && warnavilist.Contains(iD))
			{
				NetworkSpawnerNotif(networkPlayer, $"Warning Avatar : {StripColorTags(avatarCrateReference.Crate.name)} [{avatarCrateReference.Crate.Pallet.Author}]", NotificationType.WARNING, 2.5f);
			}
			if (blockedavifallbacks.Contains(iD))
			{
				NetworkSpawnerNotif(networkPlayer, "Avatar Blocked " + iD.ND_BarcodeCrateName(), NotificationType.ERROR, 2.5f);
				networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
			}
			if (blockaviauthornow && blockaviauthorlist.Contains(avatarCrateReference.Crate.Pallet.Author))
			{
				NetworkSpawnerNotif(networkPlayer, "Blocked Avatar Author : " + avatarCrateReference.Crate.Pallet.Author);
				networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
			}
			if (blockavipalletnow && blockavipalletlist.Contains(StripColorTags(avatarCrateReference.Crate.Pallet.name)))
			{
				NetworkSpawnerNotif(networkPlayer, "Blocked Avatar Pallet : " + StripColorTags(avatarCrateReference.Crate.Pallet.name));
				networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
			}
			if (globalblocklistnow)
			{
				if (SiteStuff.globalblocklistavatar.Contains(iD) && globalblocklistnotification)
				{
					NetworkSpawnerNotif(networkPlayer, "[FP] Global Blacklisted Avatar Blocked : " + iD);
					networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
				}
				if (SiteStuff.globalaviblocklistmodioid.Contains(modID) && globalblocklistnotification)
				{
					NetworkSpawnerNotif(networkPlayer, $"[FP] Global Blacklisted Avatar Mod.IO : {modID}");
					networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
				}
				if (SiteStuff.globalaviblocklistpallet.Contains(StripColorTags(avatarCrateReference.Crate.Pallet.name)) && globalblocklistnotification)
				{
					NetworkSpawnerNotif(networkPlayer, "[FP] Global Blacklisted Avatar Pallet : " + iD?.ND_BarcodePalletName());
					networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
				}
				if (SiteStuff.globalaviblocklistauthor.Contains(avatarCrateReference.Crate.Pallet.Author) && globalblocklistnotification)
				{
					NetworkSpawnerNotif(networkPlayer, "[FP] Global Blacklisted Avatar Author : " + iD?.ND_BarcodeAuthor());
					networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
				}
			}
			if (IsAvatarCrateExist(iD) && SiteStuff.blockednsfw.Contains(iD))
			{
				MelonLogger.Warning("NSFW Protection\nReport User : " + networkPlayer?.ND_Nickname() + "\nAvatar Pallet: " + iD.ND_BarcodePalletName());
				NetworkSpawnerNotif(networkPlayer, "NSFW Protection\nReport Avatar Pallet! => " + iD.ND_BarcodePalletName());
				SpawnEffects.CallDespawnEffect(networkPlayer?.MarrowEntity);
				networkPlayer.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
			}
		}
	}

	[HarmonyPatch(typeof(LocalHealth), "OnRespawned")]
	internal static class SpawnProtectionPatch
	{
		public static bool spawnProtection;

		public static IEnumerator SpawnProtection(float timeforit)
		{
			spawnProtection = true;
			yield return new WaitForSecondsRealtime(timeforit);
			spawnProtection = false;
		}

		public static void Postfix()
		{
			if (fullspawnprotection && !GamemodeManager.IsGamemodeStarted)
			{
				MelonCoroutines.Start(SpawnProtection(spawnprotectiontimer));
			}
		}
	}

	[HarmonyPatch(typeof(LocalRagdoll), "KnockoutCoroutine")]
	internal static class SpawnProtectionPatch2
	{
		public static IEnumerator Postfix(IEnumerator __result)
		{
			while (__result.MoveNext())
			{
				yield return __result.Current;
			}
			if (fullspawnprotection && !GamemodeManager.IsGamemodeStarted)
			{
				MelonCoroutines.Start(SpawnProtectionPatch.SpawnProtection(5f));
			}
		}
	}

	[HarmonyPatch(typeof(PlayerSender), "SendPlayerDamage")]
	[HarmonyPatch(new Type[]
	{
		typeof(byte),
		typeof(Attack)
	})]
	internal static class SpawnProtectionPatch3
	{
		public static bool Prefix(byte target, Attack attack)
		{
			if (fullspawnprotection && !GamemodeManager.IsGamemodeStarted && SpawnProtectionPatch.spawnProtection)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerSender), "SendPlayerDamage")]
	[HarmonyPatch(new Type[]
	{
		typeof(byte),
		typeof(Attack),
		typeof(PlayerDamageReceiver.BodyPart)
	})]
	internal static class SpawnProtectionPatch4
	{
		public static bool Prefix(byte target, Attack attack)
		{
			if (fullspawnprotection && !GamemodeManager.IsGamemodeStarted && SpawnProtectionPatch.spawnProtection)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RandomCodeGenerator), "GetString")]
	internal static class RandomCodeGeneratorPatch
	{
		private static readonly System.Collections.Generic.HashSet<string> ExistingFusionCodes = new System.Collections.Generic.HashSet<string>();

		public static void Postfix(ref string __result)
		{
			if (HideNemesisAntiCheat || !nemesisprotectedlobby || !NetworkInfo.HasLayer)
			{
				return;
			}
			NetworkLayerManager.Layer.Matchmaker?.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo result)
			{
				ExistingFusionCodes.Clear();
				IMatchmaker.LobbyInfo[] lobbies = result.Lobbies;
				for (int i = 0; i < lobbies.Length; i++)
				{
					IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
					string lobbyCode = lobbyInfo.Metadata.LobbyInfo.LobbyCode;
					if (lobbyCode.StartsWith("FP-"))
					{
						ExistingFusionCodes.Add(lobbyCode);
					}
				}
			});
			string text;
			do
			{
				text = "FP-" + UnityEngine.Random.Range(0, 1000);
			}
			while (ExistingFusionCodes.Contains(text));
			__result = text;
			ExistingFusionCodes.Add(text);
		}
	}

	[HarmonyPatch(typeof(DownloadNotifications), "SendDownloadNotification")]
	[HarmonyPriority(2147483646)]
	internal static class LogDownloads
	{
		public static System.Collections.Generic.Dictionary<LobbyInfo, System.Collections.Generic.List<Pallet>> DownloadLogger = new System.Collections.Generic.Dictionary<LobbyInfo, System.Collections.Generic.List<Pallet>>();

		public static System.Collections.Generic.List<Pallet> DeleteThesOnLeave = new System.Collections.Generic.List<Pallet>();

		private static void Prefix(string palletTitle)
		{
			Il2CppArrayBase<PalletManifest> il2CppArrayBase = AssetWarehouse.Instance?.GetPalletManifests()?.ToArray();
			if (il2CppArrayBase == null)
			{
				return;
			}
			Pallet pallet = il2CppArrayBase.FirstOrDefault((PalletManifest m) => m.Pallet != null && m.Pallet.Title == palletTitle)?.Pallet;
			if (!(pallet == null))
			{
				if (!DownloadLogger.TryGetValue(LobbyInfoManager.LobbyInfo, out var value))
				{
					value = new System.Collections.Generic.List<Pallet>();
					DownloadLogger[LobbyInfoManager.LobbyInfo] = value;
				}
				value.Remove(pallet);
				value.Insert(0, pallet);
				DeleteThesOnLeave.Remove(pallet);
				DeleteThesOnLeave.Insert(0, pallet);
				MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly, enableLogging: false));
			}
		}
	}

	[HarmonyPatch(typeof(ConnectionSender), "SendDisconnect")]
	[HarmonyPriority(2147483646)]
	internal static class KickedAndBanned
	{
		public static void Postfix(ulong platformID, string reason = "")
		{
			if (reason == "Banned from Server")
			{
				NotificationNow("Nemesis Anti-Cheat", $"{platformID} Got Banned!", NotificationType.WARNING, 3f);
			}
			if (reason == "Kicked from Server")
			{
				NotificationNow("Nemesis Anti-Cheat", $"{platformID} Got Kicked!", NotificationType.WARNING, 3f);
			}
		}
	}

	[HarmonyPatch(typeof(NetworkHelper), "PardonUser")]
	[HarmonyPriority(2147483646)]
	internal static class PardonMessage
	{
		public static void Postfix(ulong longId)
		{
			NotificationNow("Nemesis Anti-Cheat", $"{longId} Got Unbanned!", NotificationType.SUCCESS, 3f);
		}
	}

	[HarmonyPatch(typeof(PageView), "CoSummonAnimation")]
	[HarmonyPriority(2147483646)]
	internal static class ColorRadialMenuCool
	{
		public static void Postfix()
		{
			if (!Bodylogradialcolors)
			{
				return;
			}
			PopUpMenuView popUpMenu = UIRig.Instance.popUpMenu;
			foreach (PageItemView button in popUpMenu.radialPageView.buttons)
			{
				int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[12], out var result);
				int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[13], out var result2);
				int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[14], out var result3);
				int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[15], out var result4);
				Color color = (button.color2 = ColorClasseyPoo.ConvertRGBAToUnityColor(result, result2, result3, result4));
				button.textMesh.color = color;
				button.icon.GetComponent<CanvasRenderer>().SetColor(color);
			}
			PhysicsRig physicsRig = Player.PhysicsRig;
			if (!(physicsRig == null))
			{
				var (pullCordDevice, _) = ND_BodyLog(physicsRig);
				if (!(pullCordDevice == null))
				{
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[0], out var result5);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[1], out var result6);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[2], out var result7);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[3], out var result8);
					pullCordDevice.hologramTint = ColorClasseyPoo.ConvertRGBAToUnityColor(result5, result6, result7, result8);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[4], out var result9);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[5], out var result10);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[6], out var result11);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[7], out var result12);
					pullCordDevice.transform.Find("spheregrip/Sphere/Art/GrabGizmo").GetComponent<MeshRenderer>().material.color = ColorClasseyPoo.ConvertRGBAToUnityColor(result9, result10, result11, result12);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[8], out var result13);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[9], out var result14);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[10], out var result15);
					int.TryParse(BodyLogRadialMenuColorPreset.CurrentPreset[11], out var result16);
					Color color3 = ColorClasseyPoo.ConvertRGBAToUnityColor(result13, result14, result15, result16);
					pullCordDevice.lineRenderer?.SetColors(color3, color3);
				}
			}
		}
	}

	[HarmonyPatch(typeof(CheatTool), "Start")]
	[HarmonyPriority(2147483646)]
	internal static class CustomDevTools
	{
		public static bool Prefix(CheatTool __instance)
		{
			InstanceOfIt = __instance;
			try
			{
				if (File.Exists(CreateCheatToolsPreset.devitemscurrent))
				{
					string value = File.ReadAllText(CreateCheatToolsPreset.devitemscurrent);
					CreateCheatToolsPreset createCheatToolsPreset = JsonConvert.DeserializeObject<CreateCheatToolsPreset>(value);
					if (createCheatToolsPreset != null)
					{
						CreateCheatToolsPreset.CurrentPresetNow = createCheatToolsPreset;
					}
				}
				CreateCheatToolsPreset currentPresetNow = CreateCheatToolsPreset.CurrentPresetNow;
				CreateCheatToolsPreset.Item[] array = new CreateCheatToolsPreset.Item[5] { currentPresetNow.Item1, currentPresetNow.Item2, currentPresetNow.Item3, currentPresetNow.Item4, currentPresetNow.Item5 };
				CreateCheatToolsPreset.Item[] array2 = array;
				foreach (CreateCheatToolsPreset.Item item in array2)
				{
					int modIoID = item.ModIoID;
					if (modIoID != -1 && !IsBarcodeInGame(item.BarcodeId))
					{
						DownloadModIOMod(modIoID, noti: false);
					}
				}
				InstanceOfIt.crates = new SpawnableCrateReference[5]
				{
					new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item1.BarcodeId),
					new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item2.BarcodeId),
					new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item3.BarcodeId),
					new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item4.BarcodeId),
					new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item5.BarcodeId)
				};
			}
			catch (Exception value2)
			{
				MelonLogger.Error($"Preset apply failed: {value2}");
			}
			return true;
		}
	}

	internal static class CustomDevToolsFusion
	{
		public static bool Prefix(PopUpMenuView menu)
		{
			if (!NetworkSceneManager.IsLevelNetworked)
			{
				return true;
			}
			Transform transform = menu.radialPageView.transform;
			spawnnow(CreateCheatToolsPreset.CurrentPresetNow.Item1);
			spawnnow(CreateCheatToolsPreset.CurrentPresetNow.Item2);
			spawnnow(CreateCheatToolsPreset.CurrentPresetNow.Item3);
			spawnnow(CreateCheatToolsPreset.CurrentPresetNow.Item4);
			spawnnow(CreateCheatToolsPreset.CurrentPresetNow.Item5);
			return false;
			void spawnnow(CreateCheatToolsPreset.Item itemnow)
			{
				if (itemnow.ModIoID != -1 && !IsBarcodeInGame(itemnow.BarcodeId))
				{
					DownloadModIOMod(itemnow.ModIoID, noti: false);
				}
				Spawnable spawnable = new Spawnable
				{
					crateRef = new SpawnableCrateReference(itemnow.BarcodeId)
				};
				if (localonlydevtools || !itemnow.LocalSpawn)
				{
					NetworkAssetSpawner.SpawnRequestInfo info = new NetworkAssetSpawner.SpawnRequestInfo
					{
						Spawnable = spawnable,
						Position = transform.position,
						Rotation = transform.rotation,
						SpawnSource = EntitySource.Player
					};
					NetworkAssetSpawner.Spawn(info);
				}
				else
				{
					LocalAssetSpawner.Register(spawnable);
					LocalAssetSpawner.Spawn(spawnable, transform.position, transform.rotation, delegate
					{
					});
				}
			}
		}
	}

	internal static class BodyLogEffect
	{
		public static bool Prefix(ReceivedMessage received)
		{
			if (antibodylogeffect)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PullCordDevice), "SwapAvatar")]
	[HarmonyPriority(2147483646)]
	internal static class BodylogSwapFunctionPatch
	{
		private static bool Prefix()
		{
			if (AntiBodyLogGrief)
			{
				PullCordDevice item = ND_BodyLog(Player.PhysicsRig).bodylogreturn;
				if (item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Left) || item?.ballGrip.GetHand() == ND_YourGetHand(WhichHand.Right))
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(LevelRequestMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class MakeOWNERSAutoChangeMap
	{
		public static bool Prefix(ReceivedMessage received)
		{
			byte? sender = received.Sender;
			if (!sender.HasValue)
			{
				return false;
			}
			LevelRequestData data = received.ReadData<LevelRequestData>();
			PlayerID playerID = PlayerIDManager.GetPlayerID(sender.Value);
			if (playerID != null && playerID.TryGetDisplayName(out var name))
			{
				if (ownerscanchangemap)
				{
					FusionPermissions.FetchPermissionLevel(playerID, out var level, out var _);
					if (FusionPermissions.HasSufficientPermissions(level, PermissionLevel.OWNER))
					{
						if (IsBarcodeInGame(data.Barcode))
						{
							SceneStreamer.Load(new Barcode(data.Barcode));
						}
						else
						{
							NotificationNow("Nemesis Anti-Cheat", "You Don't Have This [" + StripColorTags(data.Title) + "]", NotificationType.WARNING, 3f);
						}
					}
					else
					{
						Notifier.Send(new Notification
						{
							Title = StripColorTags(data.Title) + " Load Request",
							Message = new NotificationText(name + " has requested to load " + StripColorTags(data.Title) + ".", Color.yellow),
							SaveToMenu = true,
							ShowPopup = true,
							OnAccepted = delegate
							{
								SceneStreamer.Load(new Barcode(data.Barcode));
							}
						});
					}
				}
				else
				{
					Notifier.Send(new Notification
					{
						Title = StripColorTags(data.Title) + " Load Request",
						Message = new NotificationText(name + " has requested to load " + StripColorTags(data.Title) + ".", Color.yellow),
						SaveToMenu = true,
						ShowPopup = true,
						OnAccepted = delegate
						{
							SceneStreamer.Load(new Barcode(data.Barcode));
						}
					});
				}
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(DecalProjector), "Awake")]
	[HarmonyPriority(2147483646)]
	internal static class AntiDecals
	{
		public static bool Prefix()
		{
			if (antidecal)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(InteractableIcon), "Awake")]
	[HarmonyPriority(2147483646)]
	internal static class RemoveIconsProtect
	{
		public static bool Prefix()
		{
			if (grippy)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(FusionPlayer), "CheckFloatingPoint")]
	[HarmonyPriority(2147483646)]
	internal static class outofbounds
	{
		public static bool Prefix(ref bool ____brokeBounds)
		{
			RigManager rigManager = RigData.Refs.RigManager;
			Vector3 position = rigManager.physicsRig.feet.transform.position;
			if (NetworkTransformManager.IsInBounds(position))
			{
				return false;
			}
			outofboundslobbycode = LobbyInfoManager.LobbyInfo.LobbyCode;
			Physics.autoSimulation = false;
			LocalPlayer.TeleportToCheckpoint();
			____brokeBounds = true;
			outofboundsnow = true;
			if (NetworkInfo.HasServer && !NetworkInfo.IsHost)
			{
				NetworkHelper.Disconnect("Left Bounds");
			}
			SceneStreamer.Reload();
			return false;
		}
	}

	[HarmonyPatch(typeof(SpawnEffects), "CallDespawnEffect")]
	[HarmonyPriority(2147483646)]
	internal static class AntiDespawneffectnow
	{
		private static bool Prefix()
		{
			if (antidespawneffect)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(URLWhitelistManager), "IsURLWhitelisted")]
	[HarmonyPriority(2147483646)]
	internal static class ModifyWhitelistJR
	{
		private static bool Prefix(string url)
		{
			URLWhitelist uRLWhitelist = DataSaver.ReadJsonFromText<URLWhitelist>(SiteStuff.custommediadoms);
			if (!Uri.TryCreate(url, UriKind.Absolute, out Uri result))
			{
				return true;
			}
			if (!(result.Scheme == Uri.UriSchemeHttp) && !(result.Scheme == Uri.UriSchemeHttps))
			{
				return true;
			}
			string host = result.Host;
			foreach (URLInfo item in uRLWhitelist.Whitelist)
			{
				if (host == item.Domain)
				{
					return true;
				}
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(VideoPlayer))]
	internal static class MediaPlayerProtection
	{
		private static bool IsLinkBlocked(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return false;
			}
			string base64url = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
			return SiteStuff.MediaPlayerProtectionnow.Any((NemesisAntiCheatMediaPlayerProtections p) => p.Link == base64url);
		}

		[HarmonyPrefix]
		[HarmonyPatch("Prepare")]
		[HarmonyPriority(2147483646)]
		public static bool PrepPrefix(VideoPlayer __instance)
		{
			if (__instance == null)
			{
				return true;
			}
			if (disablemediaplayers)
			{
				__instance.Stop();
				return false;
			}
			if (!NetworkSceneManager.IsLevelNetworked)
			{
				return true;
			}
			if (__instance.source == VideoSource.Url)
			{
				string url = __instance.url;
				if (string.IsNullOrEmpty(url))
				{
					return true;
				}
				if (IsLinkBlocked(url))
				{
					return false;
				}
				if (MEDIAPLAYERBLOCKERNOWList.Contains(url))
				{
					return false;
				}
				if (!URLWhitelistManager.IsURLWhitelisted(url))
				{
					return false;
				}
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("url")]
		[HarmonyPriority(2147483646)]
		[HarmonyPatch(MethodType.Setter)]
		public static void SetItURLPrefix(ref string value)
		{
			if (disablemediaplayers)
			{
				value = string.Empty;
			}
			else
			{
				if (!NetworkSceneManager.IsLevelNetworked || string.IsNullOrEmpty(value))
				{
					return;
				}
				if (mediaplayerprotection)
				{
					if (!IsLinkBlocked(value) && !MediaPlayerLogs.Contains(value))
					{
						MediaPlayerLogs.Add(value);
					}
					string encodedValue = value;
					NemesisAntiCheatMediaPlayerProtections NemesisAntiCheatMediaPlayerProtections = SiteStuff.MediaPlayerProtectionnow.FirstOrDefault((NemesisAntiCheatMediaPlayerProtections p) => p.Link.ToLower() == Convert.ToBase64String(Encoding.UTF8.GetBytes(encodedValue)).ToLower());
					if (!IsLinkBlocked(value))
					{
						MelonLogger.Warning("[Nemesis Anti-Cheat] Media Player Triggered : [" + value + "]");
					}
					if (IsLinkBlocked(value))
					{
						MelonLogger.Warning("[Nemesis Anti-Cheat Media Protection] Blocked Media [" + Convert.ToBase64String(Encoding.UTF8.GetBytes(value)) + "]\nReason : " + NemesisAntiCheatMediaPlayerProtections.Reason);
						NotificationNow("Nemesis Anti-Cheat", "[Nemesis Anti-Cheat Media Protection] Blocked Media!\nReason : " + NemesisAntiCheatMediaPlayerProtections.Reason, NotificationType.WARNING, 4f);
						value = string.Empty;
						return;
					}
					if (MEDIAPLAYERBLOCKERNOWList.Contains(value))
					{
						NotificationNow("Nemesis Anti-Cheat", "[Nemesis Anti-Cheat Media Protection] Blocked Media!", NotificationType.WARNING, 4f);
						value = string.Empty;
						return;
					}
				}
				if (!URLWhitelistManager.IsURLWhitelisted(value))
				{
					value = string.Empty;
				}
			}
		}
	}

	[HarmonyPatch(typeof(MenuLocation), "ApplyPlayerToElement")]
	[HarmonyPriority(2147483646)]
	internal static class InGame
	{
		public static PermissionLevel TempLevel;

		public static void SendSettingToServer(string serversetting, object valuedefault)
		{
			if (!(TimeReferences.TimeSinceStartup - OwnerServerSettingMessage._timeOfRequest <= 5f))
			{
				OwnerServerSettingData data = OwnerServerSettingData.Create(PlayerIDManager.LocalSmallID, serversetting, valuedefault);
				MessageRelay.RelayModule<OwnerServerSettingMessage, OwnerServerSettingData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}

		public static bool Prefix(PlayerElement element, PlayerID player)
		{
			string username = TextFilter.SanitizeName(player.Metadata.Username.GetValue());
			element.UsernameElement.Title = username;
			element.NicknameElement.Title = "Nickname";
			element.NicknameElement.Value = TextFilter.SanitizeName(player.Metadata.Nickname.GetValue());
			element.NicknameElement.Interactable = false;
			element.NicknameElement.EmptyFormat = "No {0}";
			element.DescriptionElement.Title = "Description";
			element.DescriptionElement.Value = TextFilter.SanitizeName(player.Metadata.Description.GetValue());
			element.DescriptionElement.Interactable = false;
			element.DescriptionElement.EmptyFormat = "No {0}";
			string value = player.Metadata.AvatarTitle.GetValue();
			int value2 = player.Metadata.AvatarModID.GetValue();
			ElementIconHelper.SetProfileIcon(element, value, value2);
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var selfLevel, out var _);
			FusionPermissions.FetchPermissionLevel(player.PlatformID, out var level, out var _);
			Tuple<ulong, string, PermissionLevel> tuple = PermissionList.PermittedUsers.FirstOrDefault((Tuple<ulong, string, PermissionLevel> u) => u.Item1.Equals(player.PlatformID));
			if (tuple != null)
			{
				level = tuple.Item3;
			}
			LobbyInfo activeLobbyInfo = LobbyInfoManager.LobbyInfo;
			if (player.IsMe)
			{
				element.VolumeElement.gameObject.SetActive(value: false);
			}
			else
			{
				LabFusion.Marrow.Proxies.FloatElement floatElement = element.VolumeElement.Cleared().WithTitle("Volume").WithIncrement(0.1f)
					.WithLimits(0f, 2f);
				floatElement.gameObject.SetActive(value: true);
				floatElement.Value = ContactsList.GetContact(player).volume;
				floatElement.OnValueChanged = (Action<float>)Delegate.Combine(floatElement.OnValueChanged, (Action<float>)delegate(float v)
				{
					LabFusion.Data.Contact contact = ContactsList.GetContact(player);
					contact.volume = v;
					ContactsList.UpdateContact(contact);
				});
			}
			LabFusion.Marrow.Proxies.EnumElement enumElement = element.PermissionsElement.Cleared().WithTitle(NetworkInfo.IsHost ? "Permissions" : "Your Server Permissions").WithColor(Color.yellow);
			enumElement.EnumType = typeof(PermissionLevel);
			object value3;
			if (!NetworkInfo.IsHost)
			{
				PermissionLevel? permissionLevel = tuple?.Item3;
				value3 = (permissionLevel.HasValue ? ((object)permissionLevel.GetValueOrDefault()) : enumElement.Value);
			}
			else
			{
				value3 = level;
			}
			enumElement.Value = (Enum)value3;
			enumElement.OnValueChanged = (Action<Enum>)Delegate.Combine(enumElement.OnValueChanged, (Action<Enum>)delegate(Enum v)
			{
				FusionPermissions.TrySetPermission(player.PlatformID, username, (PermissionLevel)(object)v);
			});
			enumElement.Interactable = !player.IsMe;
			LabFusion.Marrow.Proxies.StringElement stringElement = ((!player.IsMe) ? element.PlatformIDElement.Cleared().WithTitle("Steam ID").WithColor(Color.yellow)
				.WithInteractability(interactable: false) : element.PlatformIDElement.Cleared().WithTitle("[YOU] Steam ID").WithColor(Color.yellow)
				.WithInteractability(interactable: false));
			stringElement.Value = player.PlatformID.ToString();
			element.ActionsElement.Clear();
			PageElement pageElement = element.ActionsElement.AddPage();
			if (!NetworkInfo.IsHost)
			{
				pageElement.AddElement<ButtonElement>($"Current Server Perms : {level}").WithColor(Color.cyan).WithInteractability(interactable: false);
			}
			if (NetworkHelper.IsBanned(player.PlatformID))
			{
				pageElement.AddElement<ButtonElement>("Banned From Your Lobby : YES").WithColor(Color.red).WithInteractability(interactable: false);
			}
			if (tuple != null)
			{
				pageElement.AddElement<ButtonElement>($"Permission In Your Server : {tuple.Item3}").WithColor(Color.red).WithInteractability(interactable: false);
			}
			InvokeWithType(typeof(MenuLocation), "AddModerationGroup", new object[5] { activeLobbyInfo, pageElement, player, selfLevel, level });
			NemesisAntiCheatGlobalBan banchecknow = SiteStuff.globalfpbans.FirstOrDefault((NemesisAntiCheatGlobalBan result) => result.KnownSteamIds.Contains(player.PlatformID.ToString()));
			if (banchecknow != null)
			{
				pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Nemesis Anti-Cheat Global Banned : YES").WithColor(Color.red).Do(delegate
				{
					NotificationNowAlways("Nemesis Anti-Cheat Global Ban", $"User : {banchecknow.FusionNicknameAtTheTime}\nReason : {banchecknow.Reason}\nSteam ID : {player.PlatformID.ToString()}", NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(player.PlatformID);
					});
				});
				if (!string.IsNullOrEmpty(banchecknow.OptionalProof))
				{
					pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Ban Info").WithColor(Color.red).Do(delegate
					{
						NotificationNowAlways("Nemesis Anti-Cheat Global Ban", "Proof Of Ban\nUser : " + banchecknow.OptionalProof, NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
						{
							banchecknow.ShowOnPC();
						});
					});
				}
			}
			NemesisAntiCheatControversialPeople playerfromfpcl = SiteStuff.fpcpeople.FirstOrDefault((NemesisAntiCheatControversialPeople result) => result.KnownSteamIds.Contains(player.PlatformID.ToString()));
			if (playerfromfpcl != null)
			{
				pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Controversial Status").WithColor(Color.yellow).Do(delegate
				{
					NotificationNowAlways("Controversial Person", $"User : {playerfromfpcl.FusionNicknameAtTheTime}\nReason : {playerfromfpcl.Reason}\nControversy Level : {playerfromfpcl.ControversyLevel}", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						playerfromfpcl.ShowOnPC();
					});
				});
			}
			NemesisAntiCheatCommunityNotes playerfromcomnotes = SiteStuff.communitynotedplayers.FirstOrDefault((NemesisAntiCheatCommunityNotes result) => result.KnownSteamIds.Contains(player.PlatformID.ToString()));
			if (playerfromcomnotes != null)
			{
				GroupElement groupElement = pageElement.AddElement<GroupElement>("Nemesis Anti-Cheat Community Note");
				groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show On PC").WithColor(Color.yellow).Do(delegate
				{
					NotificationNowAlways("Community Note", "Showing On PC", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						playerfromcomnotes.ShowOnPC();
					});
				});
				groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Note").WithColor(Color.yellow).Do(delegate
				{
					NotificationNow("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.Note), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(player.PlatformID);
					});
				});
				groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Fusion NickNames").WithColor(Color.yellow).Do(delegate
				{
					NotificationNow("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownFusionNickNames), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(player.PlatformID);
					});
				});
				groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Steam Names").WithColor(Color.yellow).Do(delegate
				{
					NotificationNow("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownSteamNames), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(player.PlatformID);
					});
				});
				groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Steam SteamIDS").WithColor(Color.yellow).Do(delegate
				{
					NotificationNow("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownSteamIds), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(player.PlatformID);
					});
				});
			}
			if (player.PlatformID != SteamIdYours())
			{
				GroupElement groupElement2 = pageElement.AddElement<GroupElement>("Protector Options");
				if (NetworkInfo.IsHost)
				{
					groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>($"Kick For {kicktime} Mins").WithColor(Color.yellow).Do(delegate
					{
						if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
						{
							TimedObject timedObject = new TimedObject(player2.ND_SteamID(), kicktime);
							NetworkHelper.KickUser(player2.PlayerID);
						}
					});
					groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Kick Until Server Restarts").WithColor(Color.yellow).Do(delegate
					{
						if (NetworkPlayerManager.TryGetPlayer(player, out var player2) && !kicktillrestart.Contains(player2.ND_SteamID()))
						{
							kicktillrestart.Add(player2.ND_SteamID());
							NetworkHelper.KickUser(player2.PlayerID);
						}
					});
				}
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove Avatar To Avatar Blocker").WithColor(Color.yellow).Do(delegate
				{
					if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
					{
						if (player2?.ND_PlayersAvatarBarcodeID() != "c3534c5a-94b2-40a4-912a-24a8506f6c79")
						{
							ToggleAddRemoveFromFile(player2?.ND_PlayersAvatarBarcodeID(), blockedavifallbacks, avatarsblocked, "Nemesis Anti-Cheat", "Added " + player2?.ND_PlayersAvatarBarcodeID().ND_BarcodeCrateName() + " To Avatar Blocker!.", "Removed " + player2?.ND_PlayersAvatarBarcodeID().ND_BarcodeCrateName() + " From Avatar Blocker!.");
						}
						else
						{
							NotificationNow("Nemesis Anti-Cheat", "Can't Do That!", NotificationType.ERROR, 2.5f);
						}
					}
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Clone Avatar").WithColor(Color.yellow).Do(delegate
				{
					if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
					{
						ChangeIntoAvi(player2.ND_PlayersAvatarBarcodeID());
					}
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Copy Avatar Details").WithColor(Color.yellow).Do(delegate
				{
					if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
					{
						SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(player2.ND_PlayersAvatarBarcodeID());
						GUIUtility.systemCopyBuffer = $"Barcode : {spawnableCrateReference.Barcode.ID}\nCrate : {StripColorTags(spawnableCrateReference.Crate.name)} Author [{spawnableCrateReference.Crate.Pallet.Author}]\nPallet It's In : {StripColorTags(spawnableCrateReference.Crate.Pallet.name)}";
					}
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Local Protection Avi").WithColor(Color.yellow).Do(delegate
				{
					if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
					{
						player2.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
						SpawnEffects.CallDespawnEffect(player2?.MarrowEntity);
					}
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("UnBan/Ban From Your Lobbies").WithColor(Color.yellow).Do(delegate
				{
					if (!NetworkHelper.IsBanned(player.PlatformID))
					{
						BanInfo item = new BanInfo
						{
							Player = new PlayerInfo
							{
								Username = player.Metadata.Username.GetValue(),
								Nickname = player.Metadata.Nickname.GetValue(),
								PlatformID = player.PlatformID,
								Description = player.Metadata.Description.GetValue(),
								AvatarModID = player.Metadata.AvatarModID.GetValue(),
								AvatarTitle = player.Metadata.AvatarTitle.GetValue()
							},
							Reason = "Manually Banned [Nemesis Anti-Cheat]"
						};
						BanManager.BanList.Bans.RemoveAll((BanInfo info2) => info2.Player.PlatformID == player.PlatformID);
						BanManager.BanList.Bans.Add(item);
						DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
						NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
					}
					else
					{
						BanManager.Pardon(player.PlatformID);
						NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
					}
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Copy All Profile To Clipboard").WithColor(Color.yellow).Do(delegate
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						WriteIndented = true
					};
					string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(player, options);
					GUIUtility.systemCopyBuffer = systemCopyBuffer;
					NotificationNow("Nemesis Anti-Cheat", "Copied Players Entire Details To Clipboard", NotificationType.SUCCESS);
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Copy Steam ID").WithColor(Color.yellow).Do(delegate
				{
					GUIUtility.systemCopyBuffer = player.PlatformID.ToString();
					NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID", NotificationType.SUCCESS);
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Open Steam Profile").WithColor(Color.yellow).Do(delegate
				{
					CheckSteamID(player.PlatformID);
				});
				if (selfLevel == PermissionLevel.OWNER)
				{
					GroupElement groupElement3 = pageElement.AddElement<GroupElement>("Protector Owner Options");
					groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Clear Constraints").WithColor(Color.yellow).Do(delegate
					{
						if (NetworkPlayerManager.TryGetPlayer(player, out var player2))
						{
							ClearConstraints(player2);
						}
					});
					groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Teleport All").WithColor(Color.yellow).Do(delegate
					{
						if (FusionPermissions.HasSufficientPermissions(selfLevel, activeLobbyInfo.Teleportation))
						{
							foreach (PlayerID playerID in PlayerIDManager.PlayerIDs)
							{
								if ((byte)playerID != PlayerIDManager.LocalSmallID)
								{
									PermissionSender.SendPermissionRequest(PermissionCommandType.TELEPORT_TO_ME, playerID);
								}
							}
						}
					});
				}
				GroupElement groupElement4 = pageElement.AddElement<GroupElement>(NetworkInfo.IsHost ? "Protector Server Options" : "Protector Host Only Options");
				if (voicblocked.Contains(player.PlatformID.ToString()))
				{
					groupElement4.AddElement<ButtonElement>("Voice Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockedplatformids.Contains(player.PlatformID.ToString()))
				{
					groupElement4.AddElement<ButtonElement>("Damage Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockedspawnies.Contains(player.PlatformID.ToString()))
				{
					groupElement4.AddElement<ButtonElement>("Spawn/Despawn Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockmessages.Contains(player.PlatformID.ToString()))
				{
					groupElement4.AddElement<ButtonElement>("Blocked Messages : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockmovements.Contains(player.PlatformID.ToString()))
				{
					groupElement4.AddElement<ButtonElement>("Disable Movement Syncing : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				groupElement4.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Block Player Messaging").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(player.PlatformID.ToString(), blockmessages, blockmessagingnowpath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " To Block Player Messaging [Server]!.", "Removed " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " From Block Player Messaging [Server]!.");
				});
				groupElement4.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Voice Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(player.PlatformID.ToString(), voicblocked, voicepathblocked, "Nemesis Anti-Cheat", "Added " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " To Voice Blocker [Server]!.", "Removed " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " From Voice Blocker [Server]!.");
				});
				groupElement4.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Damage Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(player.PlatformID.ToString(), blockedplatformids, DamageBlockPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " To Damage Blocker [Server]!.", "Removed " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " From Damage Blocker [Server]!.");
				});
				groupElement4.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Spawn/Despawn Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(player.PlatformID.ToString(), blockedspawnies, ServerBlockSpawnPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " To Server Spawn Blocker [Server]!.", "Removed " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " From Server Spawn/Despawn Blocker [Server]!.");
				});
				groupElement4.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Disable Movement Syncing").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(player.PlatformID.ToString(), blockmovements, BlockMovementsPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " To Server Disable Movement Syncing [Server]!.", "Removed " + CleanedNAME(player.Metadata.Nickname.GetValue(), player.Metadata.Username.GetValue()) + " From Server Disable Movement Syncing [Server]!.");
				});
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(MenuMatchmaking), "ApplyPlayerToElement")]
	[HarmonyPriority(2147483646)]
	internal static class WithoutInGame
	{
		public static bool Prefix(PlayerElement element, PlayerInfo info)
		{
			element.UsernameElement.Title = TextFilter.SanitizeName(info.Username);
			element.NicknameElement.Title = "Nickname";
			element.NicknameElement.Value = TextFilter.SanitizeName(info.Nickname);
			element.NicknameElement.EmptyFormat = "No {0}";
			element.DescriptionElement.Title = "Description";
			element.DescriptionElement.Value = TextFilter.SanitizeName(info.Description);
			element.DescriptionElement.EmptyFormat = "No {0}";
			element.PermissionsElement.WithTitle("Permissions").WithColor(Color.yellow).WithInteractability(interactable: false);
			element.PermissionsElement.Value = info.PermissionLevel;
			element.PermissionsElement.EnumType = typeof(PermissionLevel);
			element.PlatformIDElement.WithTitle("Steam ID").WithColor(Color.yellow).WithInteractability(interactable: false);
			element.ActionsElement.Clear();
			if (info.PlatformID != SteamIdYours())
			{
				PageElement pageElement = element.ActionsElement.AddPage();
				PermissionLevel permissionLevel = PermissionList.PermittedUsers.FirstOrDefault((Tuple<ulong, string, PermissionLevel> u) => u.Item1.Equals(info.PlatformID))?.Item3 ?? PermissionLevel.DEFAULT;
				pageElement.AddElement<ButtonElement>($"Permission In Your Server : {permissionLevel}").WithColor(Color.red).WithInteractability(interactable: false);
				LabFusion.Marrow.Proxies.EnumElement enumElement = pageElement.AddElement<LabFusion.Marrow.Proxies.EnumElement>("Your Server Permissions").WithColor(Color.red).WithInteractability(interactable: false);
				enumElement.Value = permissionLevel;
				enumElement.EnumType = typeof(PermissionLevel);
				enumElement.OnValueChanged = (Action<Enum>)Delegate.Combine(enumElement.OnValueChanged, (Action<Enum>)delegate(Enum v)
				{
					FusionPermissions.TrySetPermission(info.PlatformID, info.Username, (PermissionLevel)(object)v);
				});
				enumElement.Interactable = true;
				if (NetworkHelper.IsBanned(info.PlatformID))
				{
					pageElement.AddElement<ButtonElement>("Banned From Your Lobby : YES").WithColor(Color.red).WithInteractability(interactable: false);
				}
				NemesisAntiCheatGlobalBan banchecknow = SiteStuff.globalfpbans.FirstOrDefault((NemesisAntiCheatGlobalBan result) => result.KnownSteamIds.Contains(info.PlatformID.ToString()));
				if (banchecknow != null)
				{
					pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Nemesis Anti-Cheat Global Banned : YES").WithColor(Color.red).Do(delegate
					{
						NotificationNowAlways("Nemesis Anti-Cheat Global Ban", $"User : {banchecknow.FusionNicknameAtTheTime}\nReason : {banchecknow.Reason}\nSteam ID : {info.PlatformID.ToString()}", NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
						{
							CheckSteamID(info.PlatformID);
						});
					});
					if (!string.IsNullOrEmpty(banchecknow.OptionalProof))
					{
						pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Ban Info").WithColor(Color.red).Do(delegate
						{
							NotificationNowAlways("Nemesis Anti-Cheat Global Ban", "Proof Of Ban\nUser : " + banchecknow.OptionalProof, NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
							{
								banchecknow.ShowOnPC();
							});
						});
					}
				}
				NemesisAntiCheatControversialPeople playerfromfpcl = SiteStuff.fpcpeople.FirstOrDefault((NemesisAntiCheatControversialPeople result) => result.KnownSteamIds.Contains(info.PlatformID.ToString()));
				if (playerfromfpcl != null)
				{
					pageElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Controversial Status").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Controversial Person", $"User : {playerfromfpcl.FusionNicknameAtTheTime}\nReason : {playerfromfpcl.Reason}\nControversy Level : {playerfromfpcl.ControversyLevel}", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							playerfromfpcl.ShowOnPC();
						});
					});
				}
				NemesisAntiCheatCommunityNotes playerfromcomnotes = SiteStuff.communitynotedplayers.FirstOrDefault((NemesisAntiCheatCommunityNotes result) => result.KnownSteamIds.Contains(info.PlatformID.ToString()));
				if (playerfromcomnotes != null)
				{
					GroupElement groupElement = pageElement.AddElement<GroupElement>("Nemesis Anti-Cheat Community Note");
					groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show On PC").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Community Note", "Showing On PC", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							playerfromcomnotes.ShowOnPC();
						});
					});
					groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Note").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Community Note", playerfromcomnotes.Note, NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							CheckSteamID(info.PlatformID);
						});
					});
					groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Fusion NickNames").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownFusionNickNames), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							CheckSteamID(info.PlatformID);
						});
					});
					groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Steam Names").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownSteamNames), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							CheckSteamID(info.PlatformID);
						});
					});
					groupElement.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Show Known Steam SteamIDS").WithColor(Color.yellow).Do(delegate
					{
						NotificationNowAlways("Community Note", string.Join(Environment.NewLine, playerfromcomnotes.KnownSteamIds), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
						{
							CheckSteamID(info.PlatformID);
						});
					});
				}
				GroupElement groupElement2 = pageElement.AddElement<GroupElement>(NetworkInfo.IsHost ? "Protector Server Options" : "Protector Host Only Options");
				if (voicblocked.Contains(info.PlatformID.ToString()))
				{
					groupElement2.AddElement<ButtonElement>("Voice Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockedplatformids.Contains(info.PlatformID.ToString()))
				{
					groupElement2.AddElement<ButtonElement>("Damage Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockedspawnies.Contains(info.PlatformID.ToString()))
				{
					groupElement2.AddElement<ButtonElement>("Spawn/Despawn Blocked : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockmessages.Contains(info.PlatformID.ToString()))
				{
					groupElement2.AddElement<ButtonElement>("Blocked Messages : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				if (blockmovements.Contains(info.PlatformID.ToString()))
				{
					groupElement2.AddElement<ButtonElement>("Disable Movement Syncing : YES").WithColor(Color.yellow).WithInteractability(interactable: false);
				}
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Block Player Messaging").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(info.PlatformID.ToString(), blockmessages, blockmessagingnowpath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(info.Nickname, info.Username) + " To Block Player Messaging [Server]!.", "Removed " + CleanedNAME(info.Nickname, info.Username) + " From Block Player Messaging!.");
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Voice Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(info.PlatformID.ToString(), voicblocked, voicepathblocked, "Nemesis Anti-Cheat", "Added " + CleanedNAME(info.Nickname, info.Username) + " To Voice Blocker [Server]!.", "Removed " + CleanedNAME(info.Nickname, info.Username) + " From Voice Blocker!.");
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Damage Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(info.PlatformID.ToString(), blockedplatformids, DamageBlockPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(info.Nickname, info.Username) + " To Damage Blocker [Server]!.", "Removed " + CleanedNAME(info.Nickname, info.Username) + " From Damage Blocker!.");
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Spawn/Despawn Blocker").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(info.PlatformID.ToString(), blockedspawnies, ServerBlockSpawnPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(info.Nickname, info.Username) + " To Server Spawn Blocker [Server]!.", "Removed " + CleanedNAME(info.Nickname, info.Username) + " From Server Spawn/Despawn Blocker!.");
				});
				groupElement2.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Add/Remove To Disable Movement Syncing").WithColor(Color.yellow).Do(delegate
				{
					ToggleAddRemoveFromFile(info.PlatformID.ToString(), blockmovements, BlockMovementsPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(info.Nickname, info.Username) + " To Server Disable Movement Syncing [Server]!.", "Removed " + CleanedNAME(info.Nickname, info.Username) + " From Server Disable Movement Syncing!.");
				});
				GroupElement groupElement3 = pageElement.AddElement<GroupElement>("Protector Options");
				groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("UnBan/Ban From Your Lobbies").WithColor(Color.yellow).Do(delegate
				{
					if (!NetworkHelper.IsBanned(info.PlatformID))
					{
						BanInfo item = new BanInfo
						{
							Player = new PlayerInfo
							{
								Username = info.Username,
								Nickname = info.Nickname,
								PlatformID = info.PlatformID,
								Description = info.Description,
								PermissionLevel = info.PermissionLevel,
								AvatarModID = info.AvatarModID,
								AvatarTitle = info.AvatarTitle
							},
							Reason = "Manually Banned [Nemesis Anti-Cheat]"
						};
						BanManager.BanList.Bans.RemoveAll((BanInfo banInfo) => banInfo.Player.PlatformID == info.PlatformID);
						BanManager.BanList.Bans.Add(item);
						DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
						NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
					}
					else
					{
						BanManager.Pardon(info.PlatformID);
						NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
					}
				});
				groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Copy All Profile To Clipboard").WithColor(Color.yellow).Do(delegate
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						WriteIndented = true
					};
					string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(info, options);
					GUIUtility.systemCopyBuffer = systemCopyBuffer;
					NotificationNow("Nemesis Anti-Cheat", "Copied Players Entire Details To Clipboard", NotificationType.SUCCESS);
				});
				groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Copy Steam ID").WithColor(Color.yellow).Do(delegate
				{
					GUIUtility.systemCopyBuffer = info.PlatformID.ToString();
					NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID", NotificationType.SUCCESS);
				});
				groupElement3.AddElement<LabFusion.Marrow.Proxies.FunctionElement>("Open Steam Profile").WithColor(Color.yellow).Do(delegate
				{
					CheckSteamID(info.PlatformID);
				});
			}
			element.PlatformIDElement.Value = info.PlatformID.ToString();
			ElementIconHelper.SetProfileIcon(element, info.AvatarTitle, info.AvatarModID);
			element.VolumeElement.gameObject.SetActive(value: false);
			return false;
		}
	}

	[HarmonyPatch(typeof(NativeMessageHandler), "Handle")]
	[HarmonyPriority(2147483646)]
	internal static class ProtectionFromClients
	{
		private class SpamState
		{
			public int Count;

			public Stopwatch Timer = Stopwatch.StartNew();
		}

		public static string lastSentMessage;

		public static string steengthnoti;

		public static NetworkPlayer LastPlayer;

		public static System.Collections.Generic.Dictionary<ulong, float> lastSpawnTime = new System.Collections.Generic.Dictionary<ulong, float>();

		public static System.Collections.Generic.Dictionary<string, int> barcodeCounts = new System.Collections.Generic.Dictionary<string, int>();

		public static System.Collections.Generic.Dictionary<PlayerID, System.Collections.Generic.List<PlayerRepDamageData>> PlayerDamageLogs = new System.Collections.Generic.Dictionary<PlayerID, System.Collections.Generic.List<PlayerRepDamageData>>();

		public static Stopwatch CountForSpawnRequestTimeout = Stopwatch.StartNew();

		public static int SpamDetection;

		private static readonly ConcurrentDictionary<ulong, SpamState> SpawnSpam = new ConcurrentDictionary<ulong, SpamState>();

		private static readonly System.Collections.Generic.HashSet<byte> validTags = (from f in typeof(NativeMessageTag).GetFields(BindingFlags.Static | BindingFlags.Public)
			select (byte)f.GetValue(null)).ToHashSet();

		private static readonly System.Collections.Generic.HashSet<int> capturedUnknowns = new System.Collections.Generic.HashSet<int>();

		public static bool ValidateAndSanitizeAvatarData(ref PlayerRepAvatarData playerRepAvatarData, NetworkPlayer go)
		{
			if (go == null)
			{
				return false;
			}
			if (playerRepAvatarData.Equals(null))
			{
				FailAndSwap("Blocked invalid avatar data (default struct)");
				return false;
			}
			if (string.IsNullOrWhiteSpace(playerRepAvatarData.Barcode))
			{
				FailAndSwap("Blocked avatar data (empty barcode)");
				return false;
			}
			if (playerRepAvatarData.Barcode == "00000-00000--00000-0")
			{
				FailAndSwap("Blocked avatar data (invalid barcode)");
				return false;
			}
			if (playerRepAvatarData.Stats.Equals(null))
			{
				FailAndSwap("Blocked avatar data (default stats)");
				return false;
			}
			object stats = playerRepAvatarData.Stats;
			if (stats == null)
			{
				FailAndSwap("Blocked avatar data (null stats)");
				return false;
			}
			Type type = stats.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields == null)
			{
				FailAndSwap("Blocked avatar data (reflection failure)");
				return false;
			}
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo == null || string.IsNullOrEmpty(fieldInfo.Name) || fieldInfo.Name == "vitality" || fieldInfo.Name == "intelligence")
				{
					continue;
				}
				object value = fieldInfo.GetValue(stats);
				if (!fieldInfo.FieldType.IsValueType && value == null)
				{
					FailAndSwap("Blocked avatar data (null field: " + fieldInfo.Name + ")");
					return false;
				}
				if (fieldInfo.FieldType == typeof(float))
				{
					float num;
					try
					{
						num = Convert.ToSingle(value);
					}
					catch
					{
						num = 1f;
					}
					// Crash Lobby v1 (Awesome Sauce) sends astronomically large / non-finite skeleton
					// proportions (up to ~50000) to overflow the receiver's avatar rig and crash the
					// lobby. Clamp every field by magnitude instead of rejecting: this neutralizes the
					// crash while leaving real avatars (whose mass/strength stats sit well above the old
					// 15 cap, and whose offsets can be small or negative) untouched - no more false
					// "Invalid stat" flags or force-swaps on join. Do NOT early-return: check every field.
					float clamped = num;
					if (float.IsNaN(num) || float.IsInfinity(num))
					{
						clamped = 1f;
					}
					else if (num > 1000f)
					{
						clamped = 1000f;
					}
					else if (num < -1000f)
					{
						clamped = -1000f;
					}
					if (clamped != num)
					{
						fieldInfo.SetValue(stats, clamped);
					}
					continue;
				}
			}
			playerRepAvatarData.Stats = (SerializedAvatarStats)stats;
			return true;
			void FailAndSwap(string reason)
			{
				if (go == null)
				{
					return;
				}
				if (Invalidstatsnow)
				{
					NotificationNowAlways("Nemesis Anti-Cheat", reason + " From: " + CleanedNAME(go), NotificationType.WARNING, 3.5f, showtitle: true, savetomenu: true);
				}
				MelonLogger.Msg(reason + " From: " + CleanedNAME(go));
				try
				{
					go.RigRefs?.RigManager?.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
				}
				catch
				{
				}
			}
		}

		public static IEnumerator RunNotificationThenKickProtection(PlayerID playernow, string message, float messagetime, Action CodeNow)
		{
			if (!HideNemesisAntiCheat)
			{
				SendNotificationData data = SendNotificationData.Create(PlayerIDManager.LocalSmallID, message, "Fusion Protetor Kick/Ban System", messagetime);
				MessageRelay.RelayModule<SendNotificationMessage, SendNotificationData>(data, new MessageRoute(playernow.SmallID, NetworkChannel.Reliable));
			}
			yield return new WaitForSecondsRealtime(messagetime + 1f);
			CodeNow?.Invoke();
		}

		private static bool Prefix(NativeMessageHandler __instance, ReceivedMessage received)
		{
			if (!validTags.Contains(__instance.Tag) && capturedUnknowns.Add(__instance.Tag))
			{
				MelonLogger.Msg($"Message That Does Not Exist In Native Tags [Perhaps A Client User Sending] : {__instance.Tag}");
			}
			if (__instance is SpawnResponseMessage || __instance is PlayerRepAvatarMessage)
			{
				NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var _);
			}
			if (AntiGrab && !GamemodeManager.IsGamemodeStarted && __instance is PlayerRepGrabMessage)
			{
				PlayerRepGrabData playerRepGrabData = received.ReadData<PlayerRepGrabData>();
				if (playerRepGrabData == null)
				{
					return true;
				}
				Grip grip = playerRepGrabData.GetGrip();
				if (grip == null || grip.gameObject == null || Player.RigManager == null)
				{
					return true;
				}
				GameObject gameObject = grip.gameObject.transform?.root?.gameObject;
				if (gameObject == null)
				{
					return true;
				}
				if (gameObject == Player.RigManager.gameObject)
				{
					return false;
				}
			}
			if (__instance is ConnectionRequestMessage || __instance is EntityDataRequestMessage || __instance is EntityZoneRegisterMessage)
			{
				return true;
			}
			if (NetworkInfo.IsHost)
			{
				try
				{
					NetworkPlayer playerusingexploits;
					PermissionLevel playerPermLevel;
					if (blockexploitscompletely)
					{
						if (NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out playerusingexploits))
						{
							ulong value = received.PlatformID.Value;
							FusionPermissions.FetchPermissionLevel(value, out playerPermLevel, out var color);
							FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var _, out color);
							if (!IsOwner() && blockmovements.Contains(received.PlatformID.ToString()) && (__instance is PlayerPoseUpdateMessage || __instance is PlayerRepGrabMessage || __instance is EntityPoseUpdateMessage))
							{
								return false;
							}
							if (__instance is ModInfoRequestMessage)
							{
								string item = playerusingexploits.PlayerID.PlatformID.ToString();
								ModInfoRequestData modInfoRequestData = received.ReadData<ModInfoRequestData>();
								if (modInfoRequestData == null)
								{
									return true;
								}
								string barcodemodioid = modInfoRequestData.Barcode;
								if (spawnbypassprotection && !FusionPermissions.HasSufficientPermissions(playerPermLevel, LobbyInfoManager.LobbyInfo.DevTools))
								{
									return false;
								}
								if (blockedspawnies.Contains(item))
								{
									return false;
								}
								if (spawnableskickon && SpawnablesKick.Contains(barcodemodioid))
								{
									MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, "Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + barcodemodioid + "]", 3f, delegate
									{
										NetworkSpawnerNotif(playerusingexploits, CleanedNAME(playerusingexploits) + " Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + barcodemodioid + "]", NotificationType.WARNING, 2.5f);
										NetworkHelper.KickUser(playerusingexploits.PlayerID);
									}));
								}
								if (AntiLasereyes && barcodemodioid == "BamBaeYoh.LaserEyes.Spawnable.LaserEyes")
								{
									return false;
								}
								if (globalblocklistnow && SiteStuff.globalblocklistspawnable.Contains(barcodemodioid))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, "[FP] Global Blacklisted Spawnable : " + barcodemodioid);
									}
									return false;
								}
								if (blockedspawnables && BlockedSpawnables.Contains(barcodemodioid))
								{
									string text = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
									string text2 = "Blocked Spawnable : " + barcodemodioid;
									if (blockspwnnotis)
									{
										NetworkSpawnerNotif(playerusingexploits, text2);
									}
									else
									{
										string txt = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text2 : ("Person Doing : " + text + "\n" + text2));
										MelonLogger.Msg(txt);
									}
									return false;
								}
							}
							if (__instance is ModInfoResponseMessage)
							{
								ModInfoResponseData modInfoResponseData = received.ReadData<ModInfoResponseData>();
								if (modInfoResponseData == null)
								{
									return true;
								}
								int modID = modInfoResponseData.ModFile.File.ModID;
								if (spawnbypassprotection && !FusionPermissions.HasSufficientPermissions(playerPermLevel, LobbyInfoManager.LobbyInfo.DevTools))
								{
									return false;
								}
								string item2 = playerusingexploits.PlayerID.PlatformID.ToString();
								if (globalblocklistnow && SiteStuff.globalblocklistmodioid.Contains(modID))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, $"[FP] Global Blacklisted Mod.IO Mod Spawned: {modID}");
									}
									return false;
								}
								if (!IsOwner() && modidblocked.Contains(modID.ToString()))
								{
									NetworkSpawnerNotif(playerusingexploits, $"Mod ID Blocked : {modID}");
									return false;
								}
								if (blockedspawnies.Contains(item2))
								{
									return false;
								}
							}
							if (__instance is DespawnResponseMessage)
							{
								return DespawnFuncs();
							}
							if (__instance is DespawnRequestMessage)
							{
								return DespawnFuncs();
							}
							if (__instance is SpawnRequestMessage)
							{
								SerializedSpawnData serializedSpawnData = received.ReadData<SerializedSpawnData>();
								if (serializedSpawnData == null || string.IsNullOrEmpty(serializedSpawnData.Barcode))
								{
									MelonLogger.Warning("Blocked invalid spawnable barcode");
									return false;
								}
								if (safedistancespawning)
								{
									return safeawaydistance(serializedSpawnData, serializedSpawnData.Barcode);
								}
								if (ballplayersprevention && serializedSpawnData.Barcode == "BaBaCorp.BaBasToybox.Spawnable.ReinforcedBall")
								{
									return safeawaydistance(serializedSpawnData, serializedSpawnData.Barcode);
								}
								if (blindplayersprevention && serializedSpawnData.Barcode == "SLZ.BONELAB.Core.Spawnable.ImpactVoid")
								{
									return safeawaydistance(serializedSpawnData, serializedSpawnData.Barcode);
								}
								return SpawnProtections(serializedSpawnData.Barcode, spawnlogging: true);
							}
							if (__instance is SpawnResponseMessage)
							{
								SpawnResponseData spawnResponseData = received.ReadData<SpawnResponseData>();
								if (spawnResponseData == null || string.IsNullOrEmpty(spawnResponseData.SpawnData.Barcode))
								{
									MelonLogger.Warning("Blocked invalid spawnable barcode");
									return false;
								}
								return SpawnProtections(spawnResponseData.SpawnData.Barcode, spawnlogging: true);
							}
							if (__instance is PlayerRepDamageMessage)
							{
								PlayerRepDamageData playerRepDamageData = received.ReadData<PlayerRepDamageData>();
								if (playerRepDamageData == null || string.IsNullOrEmpty(playerRepDamageData.Attack.Attack.damage.ToString()))
								{
									MelonLogger.Warning("Blocked invalid damage data");
									return false;
								}
								if (!PlayerDamageLogs.TryGetValue(playerusingexploits.PlayerID, out var value2))
								{
									value2 = new System.Collections.Generic.List<PlayerRepDamageData>();
									PlayerDamageLogs[playerusingexploits.PlayerID] = value2;
								}
								value2.Add(playerRepDamageData);
								if (!IsOwner())
								{
									if (antioneshot)
									{
										float damage = playerRepDamageData.Attack.Attack.damage;
										float max_Health = playerusingexploits.RigRefs.Health.max_Health;
										MelonLogger.Msg($"Damage From {CleanedNAME(playerusingexploits)} Damage : {damage} | Max Damage They Can Do : {max_Health:F1}");
										if (damage > max_Health)
										{
											return false;
										}
									}
									if (nodamageunlessweapons)
									{
										Hand hand = playerusingexploits.ND_GetHand(WhichHand.Left);
										Hand hand2 = playerusingexploits.ND_GetHand(WhichHand.Right);
										bool flag = (bool)hand.ND_GetGun() || (bool)hand.ND_GetMelee();
										bool flag2 = (bool)hand2.ND_GetGun() || (bool)hand2.ND_GetMelee();
										if (!flag || !flag2)
										{
											return false;
										}
									}
									if (servermaxdamagethres && float.TryParse(maxdamagethressy, out var result) && playerRepDamageData.Attack.Attack.damage > result)
									{
										MelonLogger.Msg(CleanedNAME(playerusingexploits) + $" Passed The Max Damage Value [{result}] Server Blocked It!");
										return false;
									}
								}
								if (blockedplatformids.Contains(playerusingexploits.PlayerID.PlatformID.ToString()))
								{
									return false;
								}
								if (playerRepDamageData.Attack.Attack.damage == float.MaxValue || float.IsInfinity(playerRepDamageData.Attack.Attack.damage) || float.IsNaN(playerRepDamageData.Attack.Attack.damage))
								{
									NetworkSpawnerNotif(playerusingexploits, "Kill Attempt By Client");
									return false;
								}
							}
							if (__instance is PlayerRepAvatarMessage)
							{
								PlayerRepAvatarData data = received.ReadData<PlayerRepAvatarData>();
								if (!ValidateAndSanitizeAvatarData(ref data, playerusingexploits))
								{
									return false;
								}
								AvatarCrateReference avatarCrateReference = new AvatarCrateReference(data.Barcode);
								int modID2 = CrateFilterer.GetModID(avatarCrateReference.Crate.Pallet);
								string text3 = playerusingexploits.PlayerID.PlatformID.ToString().Trim();
								string barcode = data.Barcode;
								(string, string, ulong, string, string, int, string) tuple = (playerusingexploits.ND_Nickname(), playerusingexploits.ND_Username(), playerusingexploits.PlayerID.PlatformID, StripColorTags(avatarCrateReference.Crate.name) ?? string.Empty, avatarCrateReference.Crate.Pallet.Author ?? string.Empty, CrateFilterer.GetModID(avatarCrateReference.Crate.Pallet), data.Barcode);
								string text4 = $"Player: {CleanedNAME(playerusingexploits)} [{tuple.Item3}]\nSwitching Into:\nPallet Name: {tuple.Item4}\nPallet Author: {tuple.Item5}\nMOD.IO ID: {tuple.Item6}\nAvatar Barcode ID: {tuple.Item7}";
								if (text4 != lastSentMessage)
								{
									lastSentMessage = text4;
									MelonLogger.Warning(text4);
								}
								if (HostIsMe(playerusingexploits))
								{
									return true;
								}
								if (!IsOwner())
								{
									if (statkicker && StatsKickerPresets.CurrentPreset != null && StatsKickerPresets.CurrentPreset.Length >= 11)
									{
										float[] playerStats = new float[11]
										{
											data.Stats.height,
											data.Stats.massArm,
											data.Stats.massChest,
											data.Stats.massHead,
											data.Stats.massLeg,
											data.Stats.massPelvis,
											data.Stats.massTotal,
											data.Stats.speed,
											data.Stats.strengthLower,
											data.Stats.strengthUpper,
											data.Stats.vitality
										};
										string[] statNames = new string[11]
										{
											"Height", "Mass Arm", "Mass Chest", "Mass Head", "Mass Leg", "Mass Pelvis", "Mass Total", "Speed", "Strength Lower", "Strength Upper",
											"Vitality"
										};
										for (int i = 0; i < 11; i++)
										{
											if (float.TryParse(StatsKickerPresets.CurrentPreset[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var limit) && playerStats[i] >= limit)
											{
												MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, $"Kick triggered by stat: {statNames[i]} | value: {playerStats[i]} | Limit: {limit}", 3f, delegate
												{
													NetworkSpawnerNotif(playerusingexploits, $"Kick triggered by stat: {statNames[i]} | Player value: {playerStats[i]} | Limit: {limit}");
													NetworkHelper.KickUser(playerusingexploits.PlayerID);
												}));
												break;
											}
										}
									}
									if (avatarskickon && AvatarsKick.Contains(data.Barcode))
									{
										MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, "Avatar You Switched Into Has Auto Kick | " + data.Barcode.ND_BarcodeCrateName(), 3f, delegate
										{
											NetworkSpawnerNotif(playerusingexploits, "Avatar Kicked | " + data.Barcode.ND_BarcodeCrateName(), NotificationType.ERROR, 2.5f);
											NetworkHelper.KickUser(playerusingexploits.PlayerID);
										}));
									}
									if (kickifvitaly && float.IsInfinity(data.Stats.vitality))
									{
										MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, "Your Avatar Had Infinite Vitality You Was Kicked", 2.5f, delegate
										{
											NetworkSpawnerNotif(playerusingexploits, "Infinite Vitality Kicked", NotificationType.ERROR, 2.5f);
											NetworkHelper.KickUser(playerusingexploits.PlayerID);
										}));
									}
									if (strengththresprotection)
									{
										float strengthLower = data.Stats.strengthLower;
										float strengthUpper = data.Stats.strengthUpper;
										if (strengthLower > strengththreshnow)
										{
											float value3 = strengthLower;
											data.Stats.strengthLower = 3.5f;
											if (strengthnotif)
											{
												string text5 = $"Strength Threshold Protection Triggered\nUser: {CleanedNAME(playerusingexploits)}\nOld Strength: {value3}\nThreshold: {strengththreshnow}\nStrength has been reset to: {data.Stats.strengthLower}";
												if (steengthnoti != text5)
												{
													steengthnoti = text5;
													MelonLogger.Warning(text5);
													NetworkSpawnerNotif(playerusingexploits, "Strength Threshold Protection: Strength was reset to a lower value.");
												}
											}
										}
										if (strengthUpper > strengththreshnow)
										{
											float value4 = strengthUpper;
											data.Stats.strengthUpper = 3.5f;
											if (strengthnotif)
											{
												string text6 = $"Strength Threshold Protection Triggered\nUser: {CleanedNAME(playerusingexploits)}\nOld Strength: {value4}\nThreshold: {strengththreshnow}\nStrength has been reset to: {data.Stats.strengthUpper}";
												if (steengthnoti != text6)
												{
													steengthnoti = text6;
													MelonLogger.Warning(text6);
													NetworkSpawnerNotif(playerusingexploits, "Strength Threshold Protection: Strength was reset to a lower value.");
												}
											}
										}
									}
								}
								if (IsSpawnableCrateExist(data.Barcode) && SiteStuff.blockednsfw.Contains(data.Barcode))
								{
									MelonLogger.Warning($"NSFW Protection\nReport User :{CleanedNAME(playerusingexploits)}\nReport Spawnable Pallet! => {data.Barcode}");
									NetworkSpawnerNotif(playerusingexploits, "NSFW Protection!\n Tried Swapping To : " + data.Barcode);
									playerusingexploits.RigRefs.RigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
									SpawnEffects.CallDespawnEffect(playerusingexploits?.MarrowEntity);
									return false;
								}
								FieldInfo[] fields = data.Stats.GetType().GetFields();
								foreach (FieldInfo fieldInfo in fields)
								{
									if (fieldInfo.FieldType == typeof(float))
									{
										float x = (float)fieldInfo.GetValue(data.Stats);
										x = MathF.Max(0.01f, MathF.Min(x, 3000f));
										fieldInfo.SetValue(data.Stats, x);
									}
								}
								if (data.Stats.massArm == float.MaxValue || data.Stats.massChest == float.MaxValue || data.Stats.massHead == float.MaxValue || data.Stats.massLeg == float.MaxValue || data.Stats.massPelvis == float.MaxValue || data.Stats.massTotal == float.MaxValue)
								{
									MelonLogger.Warning($"Prevented Kick/Crash Lobby\nOn Player : {CleanedNAME(playerusingexploits)} ({playerusingexploits.PlayerID.PlatformID})");
									NetworkSpawnerNotif(playerusingexploits, "Prevented Kick/Crash Lobby");
									return false;
								}
								data.Stats.height = MathF.Max(data.Stats.height, 0.1f);
								if (!playerusingexploits.IsMe())
								{
									if (data.Stats.massArm == float.MaxValue || data.Stats.massChest == float.MaxValue || data.Stats.massHead == float.MaxValue || data.Stats.massLeg == float.MaxValue || data.Stats.massPelvis == float.MaxValue || data.Stats.massTotal == float.MaxValue)
									{
										MelonLogger.Warning($"Attempted To Crash/Kick Exploit\nExploiter :{CleanedNAME(playerusingexploits)} ({playerusingexploits.PlayerID.PlatformID})");
										NetworkSpawnerNotif(playerusingexploits, "Attempted To Crash/Kick Exploit");
										return false;
									}
									if (!playerusingexploits.PlayerID.Metadata.Loading.GetValue() && (Iswithintwovalues(data.Stats.agility, 0f, 0.2f) || Iswithintwovalues(data.Stats.speed, 0f, 0.2f)))
									{
										MelonLogger.Warning($"(Maybe) Used No Moving Exploit\nExploiter :{CleanedNAME(playerusingexploits)} ({playerusingexploits.PlayerID.PlatformID})");
										NetworkSpawnerNotif(playerusingexploits, "(Maybe) Used No Moving Exploit");
										return false;
									}
								}
							}
							if (__instance is PlayerVoiceChatMessage && voicblocked.Contains(received.PlatformID.Value.ToString()) && !IsOwner())
							{
								return false;
							}
							if (__instance is PlayerMetadataRequestMessage)
							{
								if (playerusingexploits.IsMe())
								{
									return true;
								}
								PlayerMetadataData playerMetadataData = received.ReadData<PlayerMetadataData>();
								if (kickunincodenames && playerMetadataData.Key == "Username" && ContainsInvisibleUnicode(playerusingexploits.Username))
								{
									NotificationNow("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Kicked For Invisible Unincode Username!.", NotificationType.WARNING, 3.5f);
									NetworkHelper.KickUser(playerusingexploits.PlayerID);
								}
								if (playerMetadataData.Key == "PermissionLevel")
								{
									if (playerMetadataData.Value == "OWNER" && playerPermLevel != PermissionLevel.OWNER)
									{
										NotificationNow("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Kicked For Spoofing Permission Level [OWNER]!.", NotificationType.WARNING, 3.5f);
										NetworkHelper.KickUser(playerusingexploits.PlayerID);
									}
									if (playerMetadataData.Value == "OPERATOR" && playerPermLevel != PermissionLevel.OPERATOR)
									{
										NotificationNow("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Kicked For Spoofing Permission Level [OPERATOR]!.", NotificationType.WARNING, 3.5f);
										NetworkHelper.KickUser(playerusingexploits.PlayerID);
									}
									if (playerMetadataData.Value == "DEFAULT" && playerPermLevel != PermissionLevel.DEFAULT)
									{
										NotificationNow("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Kicked For Spoofing Permission Level [DEFAULT]!.", NotificationType.WARNING, 3.5f);
										NetworkHelper.KickUser(playerusingexploits.PlayerID);
									}
									if (playerMetadataData.Value == "GUEST" && playerPermLevel != PermissionLevel.GUEST)
									{
										NotificationNow("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Kicked For Spoofing Permission Level [GUEST]!.", NotificationType.WARNING, 3.5f);
										NetworkHelper.KickUser(playerusingexploits.PlayerID);
									}
								}
								if (AntiAnimatedName)
								{
									bool flag3;
									switch (playerMetadataData.Key)
									{
									case "Nickname":
									case "Username":
									case "Description":
										flag3 = true;
										break;
									default:
										flag3 = false;
										break;
									}
									if (flag3)
									{
										return false;
									}
								}
							}
						}
					}
					return true;
					bool DespawnFuncs()
					{
						despawnresponselogger.TryAdd(playerusingexploits.PlayerID, new System.Collections.Generic.List<string>());
						despawnresponselogger[playerusingexploits.PlayerID].Add("DespawnResponseMessage triggered");
						if (blockedspawnies.Contains(playerusingexploits.ND_SteamID().ToString()))
						{
							return false;
						}
						if (DESPAWNPROTECTION && !IsOwner())
						{
							bool flag4 = playerusingexploits.ND_GetHand(WhichHand.Left).ND_IsGrabbedSpawnGun();
							bool flag5 = playerusingexploits.ND_GetHand(WhichHand.Right).ND_IsGrabbedSpawnGun();
							if (!flag4 && !flag5)
							{
								return false;
							}
						}
						return true;
					}
					bool IsOwner()
					{
						return OwnerCheckEnabled && playerPermLevel == PermissionLevel.OWNER;
					}
					bool SpawnProtections(string text7, bool spawnlogging = false)
					{
						SpawnableCrateReference reference = new SpawnableCrateReference(text7);
						string text8 = playerusingexploits.PlayerID.PlatformID.ToString().Trim();
						int modID3 = CrateFilterer.GetModID(reference.Crate.Pallet);
						string text9 = StripColorTags(reference.Crate.name);
						string author = reference.Crate.Pallet.Author;
						int modID4 = CrateFilterer.GetModID(reference.Crate.Pallet);
						bool item3 = text7.Contains(".Avatar.");
						(string, string, ulong, string, string, int, string, bool) item4 = (playerusingexploits.ND_Nickname(), playerusingexploits.ND_Username(), playerusingexploits.ND_SteamID(), text9, author, modID4, text7, item3);
						if (!SpawnLogs.Contains(item4))
						{
							SpawnLogs.Add(item4);
						}
						if (spawnlogsmelonlog)
						{
							MelonLogger.Warning($"Player({CleanedNAME(playerusingexploits)} [{text8}])\nIs Spawning\n(Pallet Name : {text9} | Pallet Author : {author})\nMOD IO # ID {modID4}\nSending Spawnable Barcode ID : {text7}");
						}
						if (spawnlogging)
						{
							if (!PlayerSpawningStuff.TryGetValue(text8, out var value5))
							{
								value5 = new System.Collections.Generic.HashSet<string>();
								PlayerSpawningStuff[text8] = value5;
							}
							value5.Add(text7);
						}
						if (HostIsMe(playerusingexploits))
						{
							return true;
						}
						if (!IsOwner())
						{
							if (text7 == "SLZ.BONELAB.Core.Spawnable.GameplaySystems")
							{
								NetworkSpawnerNotif(playerusingexploits, "Tried To Delete Fusion UI [GameplaySystems] Auto Kicking...", NotificationType.WARNING, 3.5f);
								NetworkHelper.KickUser(playerusingexploits.PlayerID);
							}
							if (spamspawnprevention)
							{
								SpamState orAdd = SpawnSpam.GetOrAdd(playerusingexploits.ND_SteamID(), (ulong _) => new SpamState());
								if (orAdd.Timer.ElapsedMilliseconds >= antispamspawntimer)
								{
									int num2 = Interlocked.Exchange(ref orAdd.Count, 0);
									orAdd.Timer.Restart();
									if (num2 >= countbeforespam)
									{
										if (notificationspamspawn)
										{
											NotificationNowAlways("Nemesis Anti-Cheat", CleanedNAME(playerusingexploits) + " Is Spamming Spawn", NotificationType.WARNING, 2.5f);
										}
										return false;
									}
								}
								Interlocked.Increment(ref orAdd.Count);
							}
							if (BLOCKAVATARSASSPAWNABLES && IsAvatarCrateExist(text7))
							{
								return false;
							}
							if (IsSpawnableCrateExist(text7) && SiteStuff.blockednsfw.Contains(text7))
							{
								string text10 = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
								string text11 = $"NSFW Protection\nReport Spawnable! => {StripColorTags(reference.Crate.name)} [{reference.Crate.Pallet.Author}]";
								string txt2 = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text11 : ("Person Doing : " + text10 + "\n" + text11));
								MelonLogger.Warning(txt2);
								NetworkSpawnerNotif(playerusingexploits, text11);
								return false;
							}
							if (blockedspawnables && BlockedSpawnables.Contains(text7))
							{
								string text12 = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
								string text13 = "Blocked Spawnable : " + StripColorTags(reference.Crate.name) + " [" + reference.Crate.Pallet.Author + "]";
								if (blockspwnnotis)
								{
									NetworkSpawnerNotif(playerusingexploits, text13);
								}
								else
								{
									string txt3 = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text13 : ("Person Doing : " + text12 + "\n" + text13));
									MelonLogger.Msg(txt3);
								}
								return false;
							}
							if (blockedspawnies.Contains(text8))
							{
								return false;
							}
							if (globalblocklistnow)
							{
								if (SiteStuff.globalblocklistavatar.Contains(text7))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, "[FP] Global Blacklisted Avatar Spawnable : " + text7);
									}
									return false;
								}
								if (SiteStuff.globalblocklistmodioid.Contains(modID3))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, $"[FP] Global Blacklisted Spawnable Mod.IO Mod : {modID3}");
									}
									return false;
								}
								if (SiteStuff.globalblocklistspawnable.Contains(text7))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, "[FP] Global Blacklisted Spawnable : " + text7?.ND_BarcodeCrateName());
									}
									return false;
								}
								if (SiteStuff.globalblocklistpallet.Contains(StripColorTags(reference.Crate.Pallet.name)))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, "[FP] Global Blacklisted Spawnable Pallet : " + text7?.ND_BarcodePalletName());
									}
									return false;
								}
								if (SiteStuff.globalblocklistauthor.Contains(reference.Crate.Pallet.Author))
								{
									if (globalblocklistnotification)
									{
										NetworkSpawnerNotif(playerusingexploits, "[FP] Global Blacklisted Spawnable Author : " + text7?.ND_BarcodeAuthor());
									}
									return false;
								}
							}
							if (ModIDBlocker && modidblocked.Contains(modID3.ToString()))
							{
								string text14 = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
								string text15 = $"Mod ID Blocked : {modID3}";
								if (blockspwnnotis)
								{
									NetworkSpawnerNotif(playerusingexploits, text15);
								}
								else
								{
									string txt4 = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text15 : ("Person Doing : " + text14 + "\n" + text15));
									MelonLogger.Msg(txt4);
								}
								return false;
							}
							if (BlockPalletCompletely && blockentirepallet.Contains(StripColorTags(reference.Crate.Pallet.name)))
							{
								string text16 = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
								string text17 = $"Blocked Entire Pallet : {StripColorTags(reference.Crate.name)} [{reference.Crate.Pallet.Author}]";
								if (blockspwnnotis)
								{
									NetworkSpawnerNotif(playerusingexploits, text17);
								}
								else
								{
									string txt5 = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text17 : ("Person Doing : " + text16 + "\n" + text17));
									MelonLogger.Msg(txt5);
								}
								return false;
							}
							if (BlockAuthorOfSpawnable && blockentireauthor.Contains(reference.Crate.Pallet.Author))
							{
								string text18 = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
								string text19 = $"Blocked Entire Author : {StripColorTags(reference.Crate.name)} [{reference.Crate.Pallet.Author}]";
								if (blockspwnnotis)
								{
									NetworkSpawnerNotif(playerusingexploits, text19);
								}
								else
								{
									string txt6 = ((playerusingexploits != null && HostIsMe(playerusingexploits)) ? text19 : ("Person Doing : " + text18 + "\n" + text19));
									MelonLogger.Msg(txt6);
								}
								return false;
							}
							if (warnedspawnables && WarnedSpawnables.Contains(text7))
							{
								NetworkSpawnerNotif(playerusingexploits, $"Warning Spawnable : {StripColorTags(reference.Crate.name)} [{reference.Crate.Pallet.Author}]", NotificationType.WARNING, 2.5f);
							}
							if (AntiDevManip && text7 == "c1534c5a-c6a8-45d0-aaa2-2c954465764d")
							{
								return false;
							}
							if (AntiLasereyes && text7 == "BamBaeYoh.LaserEyes.Spawnable.LaserEyes")
							{
								return false;
							}
							if (SpawnGunProtection && !IsMagazine(reference))
							{
								switch (antimodguntypereal)
								{
								case antimodguntype.AnySpawnGun:
									if (text7 != "c1534c5a-5747-42a2-bd08-ab3b47616467" && text7 != "Doctor.AdvancedUtilityGun.Spawnable.AUG1" && text7 != "doge15567.PersonalSpawnguns.Spawnable.doge15567sPersonalSpawngun" && text7 != "doge15567.PersonalSpawnguns.Spawnable.SPAWNTEST")
									{
										bool flag6 = playerusingexploits.ND_GetHand(WhichHand.Left).ND_IsGrabbedSpawnGun();
										bool flag7 = playerusingexploits.ND_GetHand(WhichHand.Right).ND_IsGrabbedSpawnGun();
										if (!flag6 && !flag7)
										{
											return false;
										}
									}
									break;
								case antimodguntype.DefaultBlue:
									if (text7 != "c1534c5a-5747-42a2-bd08-ab3b47616467")
									{
										MarrowEntity marrowEntity = playerusingexploits.ND_GetMarrowEntityInHand(WhichHand.Left);
										MarrowEntity marrowEntity2 = playerusingexploits.ND_GetMarrowEntityInHand(WhichHand.Right);
										bool flag4 = marrowEntity != null && marrowEntity.ND_GetBarcodeID() == "c1534c5a-5747-42a2-bd08-ab3b47616467";
										bool flag5 = marrowEntity2 != null && marrowEntity2.ND_GetBarcodeID() == "c1534c5a-5747-42a2-bd08-ab3b47616467";
										if (!flag4 && !flag5)
										{
											return false;
										}
									}
									break;
								}
							}
							if (spawnableskickon && SpawnablesKick.Contains(text7))
							{
								MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, "Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + text7.ND_BarcodeCrateName() + "]", 3f, delegate
								{
									NetworkSpawnerNotif(playerusingexploits, CleanedNAME(playerusingexploits) + " Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + text7.ND_BarcodeCrateName() + "]", NotificationType.WARNING, 2.5f);
									NetworkHelper.KickUser(playerusingexploits.PlayerID);
								}));
							}
							if (spawnbypassprotection && !FusionPermissions.HasSufficientPermissions(playerPermLevel, LobbyInfoManager.LobbyInfo.DevTools) && !IsMagazine(reference))
							{
								return false;
							}
							if (hostonlyspawnlimiter && spawnlimitline.TryGetValue(text7.ToLowerInvariant(), out var value6))
							{
								spawnablekickfunc(text7);
								return LimitItem(text7, value6);
							}
							if (globalspawnlimitperitem)
							{
								spawnablekickfunc(text7);
								return LimitItem(text7, limitnowofglobal);
							}
							if (spawnlimiternow)
							{
								if (MostUsedItems(text7))
								{
									spawnablekickfunc(text7);
									return LimitItem(text7, 15, maxnotif: false);
								}
								spawnablekickfunc(text7);
								ulong platformID = playerusingexploits.PlayerID.PlatformID;
								float realtimeSinceStartup = Time.realtimeSinceStartup;
								if (lastSpawnTime.TryGetValue(platformID, out var value7) && realtimeSinceStartup - value7 < spawnlimitertimer)
								{
									return false;
								}
								lastSpawnTime[platformID] = realtimeSinceStartup;
							}
						}
						return true;
						bool LimitItem(string barcodenow, int maxspawns, bool maxnotif = true)
						{
							if (!IsMagazine(reference))
							{
								return true;
							}
							int num3 = 0;
							foreach (NetworkEntity item5 in NetworkEntities())
							{
								string text20 = item5.ND_GetMarrowEntity().ND_GetBarcodeID();
								if (!barcodeCounts.ContainsKey(text20))
								{
									barcodeCounts[text20] = 0;
								}
								barcodeCounts[text20]++;
								if (text20 == barcodenow)
								{
									num3++;
								}
							}
							int num4 = (limitplayercount ? NetworkPlayers().Count : maxspawns);
							if (num3 >= num4)
							{
								if (maxnotif)
								{
									NetworkSpawnerNotif(playerusingexploits, "Max Server Limit For : " + text7.ND_BarcodeCrateName());
								}
								return false;
							}
							return true;
						}
					}
					bool safeawaydistance(SerializedSpawnData serializedSpawnData2, string barcode2, float safedistancemeter = 4.5f)
					{
						bool flag4 = true;
						SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcode2);
						if (spawnableCrateReference.Crate.Pallet.Author != "SLZ" && !IsMagazine(spawnableCrateReference))
						{
							foreach (NetworkPlayer item6 in NetworkPlayers())
							{
								if (item6.ND_SmallID() != playerusingexploits.ND_SmallID())
								{
									float num2 = Vector3.Distance(serializedSpawnData2.SerializedTransform.position, item6.RigRefs.Head.transform.position);
									if (num2 < safedistancemeter)
									{
										flag4 = false;
										break;
									}
								}
							}
							if (!flag4)
							{
								return false;
							}
						}
						return flag4;
					}
					void spawnablekickfunc(string text7)
					{
						if (SpawnablesKick.Contains(text7))
						{
							MelonCoroutines.Start(RunNotificationThenKickProtection(playerusingexploits.PlayerID, "Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + text7.ND_BarcodeCrateName() + "]", 3f, delegate
							{
								NetworkSpawnerNotif(playerusingexploits, CleanedNAME(playerusingexploits) + " Spawned A Spawnable Host Has On Auto Kick, Removed From Lobby! Barcode [" + text7.ND_BarcodeCrateName() + "]", NotificationType.WARNING, 2.5f);
								NetworkHelper.KickUser(playerusingexploits.PlayerID);
							}));
						}
					}
				}
				catch
				{
					return true;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerRepAvatarMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NonHostPlayerRepAvatarMessage
	{
		public static bool Prefix(ReceivedMessage received)
		{
			PlayerRepAvatarData playerRepAvatarData = received.ReadData<PlayerRepAvatarData>();
			if (!NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player))
			{
				MelonLogger.Warning("Failed to resolve sender player");
				return false;
			}
			if (!ProtectionFromClients.ValidateAndSanitizeAvatarData(ref playerRepAvatarData, player))
			{
				return false;
			}
			string key = player.PlayerID.PlatformID.ToString();
			if (!PlayeravatarStuff.TryGetValue(key, out var value))
			{
				value = new System.Collections.Generic.HashSet<string>();
				PlayeravatarStuff[key] = value;
			}
			value.Add(playerRepAvatarData.Barcode);
			return true;
		}
	}

	[HarmonyPatch(typeof(SpawnResponseMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NonHostSpawnResponseEditor
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (blockallspawnslocally)
			{
				return false;
			}
			if (NetworkInfo.IsHost)
			{
				return true;
			}
			if (!received.Sender.HasValue)
			{
				return true;
			}
			SpawnResponseData spawnResponseData = received.ReadData<SpawnResponseData>();
			if (spawnResponseData?.SpawnData?.Barcode == null)
			{
				return true;
			}
			string barcode = spawnResponseData.SpawnData.Barcode;
			if (barcode == "SLZ.BONELAB.Core.Spawnable.GameplaySystems")
			{
				return false;
			}
			SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcode);
			if (spawnableCrateReference?.Crate?.Pallet == null)
			{
				return true;
			}
			NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
			ulong platformID = player.PlayerID.PlatformID;
			string text = platformID.ToString();
			SpawnableCrate crate = spawnableCrateReference.Crate;
			Pallet pallet = crate.Pallet;
			string text2 = StripColorTags(crate.name ?? string.Empty);
			string text3 = pallet.Author ?? "Unknown";
			int modID = CrateFilterer.GetModID(pallet);
			bool item = barcode.Contains(".Avatar.");
			(string, string, ulong, string, string, int, string, bool) item2 = (player.ND_Nickname() ?? "Unknown", player.ND_Username() ?? "Unknown", platformID, text2, text3, modID, barcode, item);
			if (!PlayerSpawningStuff.TryGetValue(text, out var value))
			{
				value = (PlayerSpawningStuff[text] = new System.Collections.Generic.HashSet<string>());
			}
			value.Add(barcode);
			if (!SpawnLogs.Contains(item2))
			{
				SpawnLogs.Add(item2);
			}
			if (spawnlogsmelonlog)
			{
				MelonLogger.Warning($"Player({CleanedNAME(player)} [{platformID}])\nIs Spawning\n(Pallet Name : {text2} | Pallet Author : {text3})\nMOD IO # ID {modID}\nSending Spawnable Barcode ID : {barcode}");
			}
			if (globalblocklistnow)
			{
				if (SiteStuff.globalblocklistavatar.Contains(barcode) || SiteStuff.globalblocklistspawnable.Contains(barcode))
				{
					if (globalblocklistnotification)
					{
						NetworkSpawnerNotif(player, "[FP] Global Blacklisted Spawnable : " + barcode);
					}
					return false;
				}
				if (SiteStuff.globalblocklistmodioid.Contains(modID))
				{
					if (globalblocklistnotification)
					{
						NetworkSpawnerNotif(player, $"[FP] Global Blacklisted Spawnable Mod.IO Mod : {modID}");
					}
					return false;
				}
				if (SiteStuff.globalblocklistpallet.Contains(text2))
				{
					if (globalblocklistnotification)
					{
						NetworkSpawnerNotif(player, "[FP] Global Blacklisted Spawnable Pallet : " + text2);
					}
					return false;
				}
				if (SiteStuff.globalblocklistauthor.Contains(text3))
				{
					if (globalblocklistnotification)
					{
						NetworkSpawnerNotif(player, "[FP] Global Blacklisted Spawnable Author : " + text3);
					}
					return false;
				}
			}
			if (spawnprotectionsnot_host)
			{
				if (BLOCKAVATARSASSPAWNABLES && IsAvatarCrateExist(barcode))
				{
					return false;
				}
				if (IsSpawnableCrateExist(barcode) && SiteStuff.blockednsfw.Contains(barcode))
				{
					string text4 = $"NSFW Protection\nReport Spawnable! => {text2} [{text3}]";
					MelonLogger.Warning(text4);
					NetworkSpawnerNotif(player, text4);
					return false;
				}
				if (blockedspawnables && BlockedSpawnables.Contains(barcode))
				{
					if (blockspwnnotis)
					{
						NetworkSpawnerNotif(player, $"Blocked Spawnable : {text2} [{text3}]");
					}
					else
					{
						MelonLogger.Msg($"Person Doing : {CleanedNAME(player)}\nBlocked Spawnable : {text2} [{text3}]");
					}
					return false;
				}
				if (blockedspawnies.Contains(text))
				{
					return false;
				}
				if (ModIDBlocker && modidblocked.Contains(modID.ToString()))
				{
					NetworkSpawnerNotif(player, $"Mod ID Blocked : {modID}");
					return false;
				}
				if (BlockPalletCompletely && blockentirepallet.Contains(text2))
				{
					if (blockspwnnotis)
					{
						NetworkSpawnerNotif(player, $"Blocked Entire Pallet : {text2} [{text3}]");
					}
					return false;
				}
				if (BlockAuthorOfSpawnable && blockentireauthor.Contains(text3))
				{
					if (blockspwnnotis)
					{
						NetworkSpawnerNotif(player, $"Blocked Entire Author : {text2} [{text3}]");
					}
					else
					{
						MelonLogger.Msg($"Person Doing : {CleanedNAME(player)}\nBlocked Entire Author : {text2} [{text3}]");
					}
					return false;
				}
				if (warnedspawnables && WarnedSpawnables.Contains(barcode))
				{
					NetworkSpawnerNotif(player, $"Warning Spawnable : {text2} [{text3}]", NotificationType.WARNING, 2.5f);
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(ModInfoResponseMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NonHostModInfoResponseEditor
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (blockallspawnslocally)
			{
				return false;
			}
			if (!NetworkInfo.IsHost)
			{
				ModInfoResponseData modInfoResponseData = received.ReadData<ModInfoResponseData>();
				if (modInfoResponseData != null)
				{
					SerializedModIOFile modFile = modInfoResponseData.ModFile;
					if (modFile != null)
					{
						_ = modFile.File;
						if (0 == 0)
						{
							int modID = modInfoResponseData.ModFile.File.ModID;
							NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
							if (globalblocklistnow && SiteStuff.globalblocklistmodioid.Contains(modID))
							{
								if (globalblocklistnotification)
								{
									NetworkSpawnerNotif(player, $"[FP] Global Blacklisted Mod.IO Mod Spawned: {modID}");
								}
								return false;
							}
							if (ModIDBlocker && modidblocked.Contains(modID.ToString()))
							{
								NetworkSpawnerNotif(player, $"Mod ID Blocked : {modID}");
								return false;
							}
							goto IL_013b;
						}
					}
				}
				return true;
			}
			goto IL_013b;
			IL_013b:
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerRepDamageMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NonHostPlayerRepDamageEditor
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (!NetworkInfo.IsHost)
			{
				PlayerRepDamageData playerRepDamageData = received.ReadData<PlayerRepDamageData>();
				if (playerRepDamageData?.Attack?.Attack == null)
				{
					return true;
				}
				if (playerRepDamageData.Attack.Attack.damage == float.MaxValue || float.IsInfinity(playerRepDamageData.Attack.Attack.damage) || float.IsNaN(playerRepDamageData.Attack.Attack.damage))
				{
					if (NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player))
					{
						NetworkSpawnerNotif(player, "Kill Attempt By Client");
					}
					return false;
				}
			}
			return true;
		}
	}

	// Blocks Awesome Sauce's metadata impersonation (Random Player Username/Nickname, Erase Nickname):
	// it sends PlayerMetadataResponse with Player = a victim while the message originates from the
	// attacker. A real metadata change is only authored by that player (self) or relayed by the host,
	// so a response whose target isn't the sender is dropped. False-positive free.
	[HarmonyPatch(typeof(PlayerMetadataResponseMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NemesisAntiMetadataSpoof
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (!antimetadataspoof || !received.Sender.HasValue)
			{
				return true;
			}
			byte sender = received.Sender.Value;
			if (sender == PlayerIDManager.HostSmallID)
			{
				return true;
			}
			PlayerMetadataData playerMetadataData = received.ReadData<PlayerMetadataData>();
			if (playerMetadataData == null || playerMetadataData.Player.ID == sender)
			{
				return true;
			}
			if (NetworkPlayerManager.TryGetPlayer(sender, out var player) && ExploitNotifyReady(player.PlayerID.PlatformID, "metaspoof"))
			{
				NetworkSpawnerNotif(player, "Metadata Spoof / Impersonation [" + (playerMetadataData.Key ?? "?") + "]", NotificationType.WARNING, 3.5f);
			}
			return false;
		}
	}

	// Ghost Mode (Awesome Sauce) makes a player invisible/unreachable by overriding their outgoing
	// pose so their rep teleports to a fixed far point (e.g. 1000,1000,2000) while they keep acting.
	// Detect a PlayerPoseUpdate whose pelvis is non-finite or far out of bounds and drop it so the
	// rep stays at its last real position (ghost neutralized). Kicking is opt-in (off by default) so
	// it never false-flags a legitimate player on an unusually large map.
	[HarmonyPatch(typeof(PlayerPoseUpdateMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class NemesisAntiGhostMode
	{
		private static bool Prefix(ReceivedMessage received)
		{
			if (!antighostmode || !received.Sender.HasValue)
			{
				return true;
			}
			if (received.Sender.Value == PlayerIDManager.HostSmallID)
			{
				return true;
			}
			PlayerPoseUpdateData poseData = received.ReadData<PlayerPoseUpdateData>();
			if (poseData == null || poseData.Pose == null)
			{
				return true;
			}
			Vector3 pelvis = poseData.Pose.PelvisPose.Position;
			bool nonFinite = float.IsNaN(pelvis.x) || float.IsNaN(pelvis.y) || float.IsNaN(pelvis.z) || float.IsInfinity(pelvis.x) || float.IsInfinity(pelvis.y) || float.IsInfinity(pelvis.z);
			bool outOfBounds = Mathf.Abs(pelvis.x) > ghostmodebounds || Mathf.Abs(pelvis.y) > ghostmodebounds || Mathf.Abs(pelvis.z) > ghostmodebounds;
			if (!nonFinite && !outOfBounds)
			{
				return true;
			}
			if (!NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player) || player.IsMe())
			{
				return true;
			}
			ulong ghostPid = player.PlayerID.PlatformID;
			if (ghostmodekick && NetworkInfo.IsHost)
			{
				if (ExploitNotifyReady(ghostPid, "ghostkick", 12f))
				{
					NetworkSpawnerNotif(player, "Ghost Mode (Out-of-Bounds Teleport)", NotificationType.ERROR, 2.5f);
					MelonCoroutines.Start(ProtectionFromClients.RunNotificationThenKickProtection(player.PlayerID, "Ghost Mode / out-of-bounds pose detected. Removed from lobby!", 3f, delegate
					{
						NetworkHelper.KickUser(player.PlayerID);
					}));
				}
			}
			else if (ExploitNotifyReady(ghostPid, "ghost"))
			{
				NetworkSpawnerNotif(player, "Ghost Mode (Out-of-Bounds Teleport)", NotificationType.WARNING, 2.5f);
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(LobbyInfo), "get_NameTags")]
	[HarmonyPriority(2147483646)]
	internal static class SpoofNameTags
	{
		private static bool Prefix(ref bool __result)
		{
			if (forcenametagson)
			{
				__result = true;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SteamNetworkLayer), "OnPlayerJoin")]
	[HarmonyPriority(2147483646)]
	internal static class PlayerJoinLogger
	{
		private static void Postfix(PlayerID id)
		{
			if (id != null && !JoinLogger.Any((PlayerInfo p) => p?.PlatformID == id.PlatformID))
			{
				JoinLogger.Add(new PlayerInfo
				{
					Nickname = id.Metadata.Nickname.GetValue(),
					Username = id.Metadata.Username.GetValue(),
					PlatformID = id.PlatformID,
					AvatarModID = id.Metadata.AvatarModID.GetValue(),
					AvatarTitle = id.Metadata.AvatarTitle.GetValue(),
					Description = id.Metadata.Description.GetValue()
				});
			}
		}
	}

	[HarmonyPatch(typeof(ConnectionRequestMessage), "OnHandleMessage")]
	[HarmonyPriority(2147483646)]
	internal static class ConnectionBlocker
	{
		private static bool Prefix(ReceivedMessage received)
		{
			ConnectionRequestData connectionRequestData = received.ReadData<ConnectionRequestData>();
			ulong? platformId = received.PlatformID;
			if (kicktillrestart.Contains(platformId.Value))
			{
				ConnectionSender.SendConnectionDeny(platformId.Value, string.Empty);
				return false;
			}
			if (TimedObject.ActiveTimers.TryGetValue(platformId, out var value) && !value.IsExpired())
			{
				ConnectionSender.SendConnectionDeny(platformId.Value, string.Empty);
				return false;
			}
			if (globalFPBANLIST)
			{
				NemesisAntiCheatGlobalBan banchecknow = SiteStuff.globalfpbans.FirstOrDefault((NemesisAntiCheatGlobalBan result) => result.KnownSteamIds.Contains(platformId.ToString()));
				if (platformId != connectionRequestData.BackupPlatformID)
				{
					ConnectionSender.SendConnectionDeny(platformId.Value, string.Empty);
					return false;
				}
				if (banchecknow != null)
				{
					if (globalbannotification)
					{
						NotificationNowAlways("Nemesis Anti-Cheat Global Ban", $"User Tried Connecting : {banchecknow.FusionNicknameAtTheTime}\nReason : {banchecknow.Reason}\nSteam ID : {platformId.ToString()}", NotificationType.SUCCESS, 3f, showtitle: true, savetomenu: true, delegate
						{
							banchecknow.ShowOnPC();
						});
					}
					ConnectionSender.SendConnectionDeny(platformId.Value, string.Empty);
					return false;
				}
			}
			// Awesome Sauce's Patch_CheckLobbyVisibility forces non-public lobbies to show up in the
			// cheater's lobby browser so they can be found and joined. The visibility patch runs on the
			// joiner, so the only effective counter is here on the host: if our lobby isn't PUBLIC and
			// the host has enabled this, deny the incoming connection. Off by default so it never blocks
			// legitimate code-based joins to a Private/Friends lobby unless the host opts in.
			if (blockprivatejoiners && NetworkInfo.IsHost && platformId.HasValue && LobbyInfoManager.LobbyInfo != null && LobbyInfoManager.LobbyInfo.Privacy != ServerPrivacy.PUBLIC)
			{
				if (ExploitNotifyReady(platformId.Value, "privatejoin", 2f))
				{
					NotificationNowAlways("Nemesis Anti-Cheat", "Blocked a join - lobby privacy is " + LobbyInfoManager.LobbyInfo.Privacy + ". SteamID: " + platformId.Value, NotificationType.WARNING, 3f, showtitle: true, savetomenu: true);
				}
				ConnectionSender.SendConnectionDeny(platformId.Value, "This lobby is private - Nemesis Anti-Cheat is blocking new joins.");
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Health), "TAKEDAMAGE")]
	internal static class RealGodModeScrubs
	{
		private static bool Prefix()
		{
			if (SpawnProtectionPatch.spawnProtection)
			{
				return false;
			}
			if (godmode && AreYouOWNER() && !GamemodeManager.IsGamemodeStarted)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(LocalRagdoll), "KnockoutCoroutine")]
	[HarmonyPriority(2147483646)]
	internal static class RealKnockOutScrubs
	{
		public static bool Prefix()
		{
			if (antiknockout && AreYouOWNER() && !GamemodeManager.IsGamemodeStarted)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Il2CppSLZ.Marrow.AmmoInventory), "RemoveCartridge")]
	[HarmonyPriority(2147483646)]
	internal static class RealInfiniteAmmoScrubs
	{
		private static void Postfix(Il2CppSLZ.Marrow.AmmoInventory __instance, CartridgeData cartridge, int count)
		{
			if (unlammo && !GamemodeManager.IsGamemodeStarted)
			{
				__instance.AddCartridge(cartridge, count);
			}
		}
	}

	[HarmonyPatch(typeof(ForcePullGrip), "OnFarHandHoverBegin")]
	internal static class ForceGrabDisabler
	{
		private static bool Prefix()
		{
			return !forcegrabdisablernow;
		}
	}

	[HarmonyPatch(typeof(ForcePullGrip), "OnFarHandHoverEnd")]
	internal static class ForceGrabDisabler2
	{
		private static bool Prefix()
		{
			return !forcegrabdisablernow;
		}
	}

	[HarmonyPatch(typeof(ForcePullGrip), "OnFarHandHoverUpdate")]
	internal static class ForceGrabDisabler3
	{
		private static bool Prefix()
		{
			return !forcegrabdisablernow;
		}
	}

	[HarmonyPatch(typeof(ForcePullGrip), "OnStartAttach")]
	internal static class ForceGrabDisabler4
	{
		private static bool Prefix()
		{
			return !forcegrabdisablernow;
		}
	}

	[HarmonyPatch(typeof(ForcePullGrip), "OnForcePullComplete")]
	internal static class ForceGrabDisabler5
	{
		private static bool Prefix()
		{
			return !forcegrabdisablernow;
		}
	}

	[HarmonyPatch(typeof(WindBuffetSFX), "Awake")]
	internal static class AudioPatch1
	{
		private static bool Prefix()
		{
			return !disablewindsfx;
		}
	}

	[HarmonyPatch(typeof(RigVoiceSource), "get_MinMicrophoneDistance")]
	[HarmonyPriority(2147483646)]
	internal static class ProxRemoverPart1
	{
		private static bool Prefix(ref float __result)
		{
			if (removeproxchat)
			{
				__result = 9999999f;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RigVoiceSource), "get_MaxMicrophoneDistance")]
	[HarmonyPriority(2147483646)]
	internal static class ProxRemoverPart2
	{
		private static bool Prefix(ref float __result)
		{
			if (removeproxchat)
			{
				__result = 9999999f;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RigVoiceSource), "UpdateOcclusion")]
	[HarmonyPriority(2147483646)]
	internal static class ProxRemoverPart3
	{
		private static bool Prefix()
		{
			if (removeproxchat)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(TrustedListManager), "VerifyPlayer")]
	[HarmonyPatch(new Type[]
	{
		typeof(ulong),
		typeof(string)
	})]
	[HarmonyPriority(2147483646)]
	internal static class FusionRemoveDaMastas
	{
		private static bool Prefix(ref TrustedStatus __result)
		{
			__result = TrustedStatus.None;
			return false;
		}
	}

	[HarmonyPatch(typeof(TrustedListManager), "VerifyPlayer")]
	[HarmonyPatch(new Type[]
	{
		typeof(TrustedListManager.TrustedPlayer[]),
		typeof(ulong),
		typeof(string)
	})]
	[HarmonyPriority(2147483646)]
	internal static class FusionRemoveDaMastas2
	{
		private static bool Prefix(ref TrustedStatus __result)
		{
			__result = TrustedStatus.None;
			return false;
		}
	}

	[HarmonyPatch(typeof(MasterPermissionsManager), "IsMaster")]
	[HarmonyPriority(2147483646)]
	internal static class FusionRemoveDaMastas3
	{
		private static bool Prefix(ref bool __result)
		{
			__result = false;
			return false;
		}
	}

	[HarmonyPatch(typeof(GlobalBanManager), "GetBanInfo")]
	[HarmonyPriority(2147483646)]
	internal static class Patch_GetBanInfo
	{
		private static bool Prefix(ref GlobalBanInfo __result)
		{
			if (REMOVEDGLOBALBANLIST)
			{
				__result = null;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GlobalBanManager), "IsBanned")]
	[HarmonyPriority(2147483646)]
	[HarmonyPatch(new Type[] { typeof(LobbyInfo) })]
	internal static class Patch_IsBanned_Lobby
	{
		private static bool Prefix(ref bool __result)
		{
			if (REMOVEDGLOBALBANLIST)
			{
				__result = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GlobalBanManager), "IsBanned")]
	[HarmonyPriority(2147483646)]
	[HarmonyPatch(new Type[] { typeof(PlatformInfo) })]
	internal static class Patch_IsBanned_Platform
	{
		private static bool Prefix(ref bool __result)
		{
			if (REMOVEDGLOBALBANLIST)
			{
				__result = false;
				return false;
			}
			return true;
		}
	}

	internal enum antimodguntype
	{
		DefaultBlue,
		AnySpawnGun
	}

	internal enum SpawnableSearchType
	{
		Spawn,
		CopyDetailsToClipboard,
		UnFavoriteAndFavorite,
		DespawnAllOfThis,
		SetInSpawnGun
	}

	internal enum AvatarSearchType
	{
		ChangeInto,
		CopyDetailsToClipboard,
		SetBodyLog
	}

	internal enum DespawnerAll
	{
		NoFilter,
		Guns,
		Melees,
		Npcs,
		EverythingButGuns,
		EverythingButMelees,
		EverythingButNpcs,
		NetworkProps,
		AllNotButtonsLeverCircuits
	}

	internal enum handnow
	{
		Left,
		Right
	}

	internal enum WhichHand
	{
		Both,
		Left,
		Right
	}

	internal enum RandomizerType
	{
		AllAvatars,
		AllNPCs,
		AllSpawnables,
		NoTagsSpawnables,
		AllWeapons,
		AllMelees,
		AllRifle,
		AllSMG,
		AllRanged,
		AllPistol,
		AllShotgun,
		AllSniper,
		AllBlunt,
		AllBlade,
		AllKnife
	}

	internal enum Slots
	{
		HolsterLeft,
		HolsterRight,
		BackLeft,
		BackRight,
		BottomRight,
		Head
	}

	internal enum SearchMethod
	{
		CrateNames,
		BarcodeIDNames,
		PalletName,
		PalletAuthor
	}

	internal enum SearchMethodType
	{
		Level,
		Spawnable,
		Avatar
	}

	internal class SearchHistoryEntry
	{
		public string SearchText;

		public SearchMethod Method;

		public SearchMethodType Type;

		public bool IsAvatar { get; private set; }

		public bool IsSpawnable { get; private set; }

		public bool IsLevelCrate { get; private set; }

		public SearchHistoryEntry(string searchText, SearchMethod method, SearchMethodType type)
		{
			SearchText = searchText;
			Method = method;
			Type = type;
			IsAvatar = type == SearchMethodType.Avatar;
			IsSpawnable = type == SearchMethodType.Spawnable;
			IsLevelCrate = type == SearchMethodType.Level;
		}
	}

	internal enum MenuSections
	{
		SelfCat,
		SpawnLogs,
		AvatarSwitchLogs,
		SpawnableSearcher,
		AvatarSearcher,
		CustomAviFav,
		CustomSpawnableFav,
		PlayerInformation,
		SceneEntities,
		ServerHistory,
		FusionBanManager,
		DownloadLogger,
		MediaPlayerLogger,
		JoinLoggerNow,
		DamageLogger,
		NetworkLogger,
		PlayerSeriStats,
		PlayerSearch,
		MODIOSEARCHER
	}

	internal static Version NACVersionCurrent = new Version("1.0.0");

	internal static readonly string NACsavetxt = Path.Combine(MelonEnvironment.UserDataDirectory, "NACSavedFiles");

	internal static readonly string RECNETLYMETLOGGED = Path.Combine(NACsavetxt, "RecentlyMetPlayersLog.json");

	internal static readonly string MEDIAPLAYERLOGS = Path.Combine(NACsavetxt, "MediaPlayerLogs.json");

	internal static readonly string LOBBIESLOGGEDSINCE = Path.Combine(NACsavetxt, "LobbiesSinceLoginLog.json");

	internal static readonly string PLAYERSLOGGEDSINCE = Path.Combine(NACsavetxt, "PlayersSinceLoginLog.json");

	internal static readonly string spawnlimitshostonly = Path.Combine(MelonEnvironment.UserDataDirectory, "HostOnlySpawnLimits.json");

	internal static readonly string avatarsblocked = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockAvatarsNow.json");

	internal static readonly string homeworldnow = Path.Combine(MelonEnvironment.UserDataDirectory, "FusionHomeWorld.json");

	internal static readonly string PalletDumpLocation = Path.Combine(MelonEnvironment.UserDataDirectory, "Bonelab_PalletDump.json");

	internal static readonly string BlockedSpawnablesPath = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockedSpawnables.json");

	internal static readonly string WarnedSpawnablesPath = Path.Combine(MelonEnvironment.UserDataDirectory, "WarnedSpawnables.json");

	internal static readonly string ProtectorSettings = Path.Combine(MelonEnvironment.UserDataDirectory, "NemesisAntiCheatSettings.json");

	internal static readonly string SpawnablesKickPath = Path.Combine(MelonEnvironment.UserDataDirectory, "SpawnablesKick.json");

	internal static readonly string AvatarsKickPath = Path.Combine(MelonEnvironment.UserDataDirectory, "AvatarsKick.json");

	internal static readonly string SpawnableCustomFav = Path.Combine(MelonEnvironment.UserDataDirectory, "CustomSpawnableFavorites.json");

	internal static readonly string blockpalletnowlist = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockedPalletsFP.json");

	internal static readonly string blockauthornowlist = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockedAuthorsFP.json");

	internal static readonly string MEDIAPLAYERBLOCKERNOW = Path.Combine(MelonEnvironment.UserDataDirectory, "MediaPlayerBlocker.json");

	internal static readonly string AvatarCustomFav = Path.Combine(MelonEnvironment.UserDataDirectory, "CustomAvatarFavorites.json");

	internal static readonly string DamageBlockPath = Path.Combine(MelonEnvironment.UserDataDirectory, "HostOnlyBlockDamageOfPlayers.json");

	internal static readonly string ServerBlockSpawnPath = Path.Combine(MelonEnvironment.UserDataDirectory, "HostOnlyBlockSpawns.json");

	internal static readonly string BlockMovementsPath = Path.Combine(MelonEnvironment.UserDataDirectory, "HostOnlyBlockMovements.json");

	internal static readonly string blockmessagingnowpath = Path.Combine(MelonEnvironment.UserDataDirectory, "HostOnlyBlockMessages.json");

	internal static readonly string ModIDBLOCKSPATH = Path.Combine(MelonEnvironment.UserDataDirectory, "ModIDBlocks.json");

	internal static readonly string voicepathblocked = Path.Combine(MelonEnvironment.UserDataDirectory, "VoiceBlockerIds.json");

	internal static readonly string WarnAvisNow = Path.Combine(MelonEnvironment.UserDataDirectory, "WarnAvatars.json");

	internal static readonly string BlockAviAuthorNowp = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockAuthorAvatars.json");

	internal static readonly string BlockPalletAviNowp = Path.Combine(MelonEnvironment.UserDataDirectory, "BlockPalletAvatars.json");

	internal static readonly string permissionshere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Stress Level Zero", "BONELAB", "Fusion", "permissionList.xml");

	internal static readonly string modiotokenfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Stress Level Zero", "BONELAB", "mod_settings.json");

	internal static string searchavi = "base";

	internal static string spwnblesearch = "flash";

	internal static string SteamIDSearch = "";

	internal static string customspwner = "Drax.DraxPack.Spawnable.AK50";

	internal static string teleportername = "Spawn Here";

	internal static string CHEATPRS = "Spawn Here";

	internal static string bodylogpagename = "Spawn Here";

	internal static string statskickernows = "Kick Preset Here";

	internal static string maxdamagethressy = "1000";

	internal static string colornamenowx = "Color Preset Here";

	internal static string profilepresz = "Profile Preset Here";

	internal static string loadoutname = "Loadout Here";

	internal static string levelsrch = "SLZ";

	internal static BoneLib.BoneMenu.Page PlayersOnlinePage;

	internal static BoneLib.BoneMenu.Page NemesisAntiCheatPage;

	internal static BoneLib.BoneMenu.Page ProtectionLogs;

	internal static BoneLib.BoneMenu.Page AltpreventPG;

	internal static BoneLib.BoneMenu.Page Protectsettings;

	internal static BoneLib.BoneMenu.Page playermessages;

	internal static BoneLib.BoneMenu.Page avatarsearcher;

	internal static BoneLib.BoneMenu.Page aviresults;

	internal static BoneLib.BoneMenu.Page avisearchhistory;

	internal static BoneLib.BoneMenu.Page spawnablesearch;

	internal static BoneLib.BoneMenu.Page spawnableresults;

	internal static BoneLib.BoneMenu.Page spawnablehistory;

	internal static BoneLib.BoneMenu.Page AISpawnersPage;

	internal static BoneLib.BoneMenu.Page Timersz;

	internal static BoneLib.BoneMenu.Page OwnerOnlyPg;

	internal static BoneLib.BoneMenu.Page OPERATORPG;

	internal static BoneLib.BoneMenu.Page cheatspresetsnow;

	internal static BoneLib.BoneMenu.Page bodylognowpage;

	internal static BoneLib.BoneMenu.Page bodylognowpagexx;

	internal static BoneLib.BoneMenu.Page loadoutpages;

	internal static BoneLib.BoneMenu.Page loadoutpagesnow;

	internal static BoneLib.BoneMenu.Page colorpresets;

	internal static BoneLib.BoneMenu.Page colorpresetsnow;

	internal static BoneLib.BoneMenu.Page searchersnow;

	internal static BoneLib.BoneMenu.Page FusionProfiles;

	internal static BoneLib.BoneMenu.Page FusionProfilesnow;

	internal static BoneLib.BoneMenu.Page statskick;

	internal static BoneLib.BoneMenu.Page statskicknow;

	internal static BoneLib.BoneMenu.Page teleportersnow;

	internal static BoneLib.BoneMenu.Page teleporters;

	internal static BoneLib.BoneMenu.Page cheatspreset;

	internal static BoneLib.BoneMenu.Page pubs;

	internal static BoneLib.BoneMenu.Page fppubs;

	internal static BoneLib.BoneMenu.Page fpsdespawn;

	internal static BoneLib.BoneMenu.Page spawnlimitersz;

	internal static BoneLib.BoneMenu.Page levelsearcher;

	internal static BoneLib.BoneMenu.Page levelresults;

	internal static BoneLib.BoneMenu.Page HOSTONLYPGE;

	internal static BoneLib.BoneMenu.Page levelhistory;

	internal static BoneLib.BoneMenu.Page protectionstuff;

	internal static BoneLib.BoneMenu.Page spawngunesst;

	internal static BoneLib.BoneMenu.Page holdinginhands;

	internal static BoneLib.BoneMenu.Page playeroptions;

	internal static BoneLib.BoneMenu.Page selfrestrictions;

	internal static BoneLib.BoneMenu.Page Notifications;

	internal static BoneLib.BoneMenu.Page playerjoinlogsnow;

	internal static BoneLib.BoneMenu.Page unblockingnow;

	internal static BoneLib.BoneMenu.Page WarnedSpawnablesnow;

	internal static BoneLib.BoneMenu.Page modidblockednow;

	internal static BoneLib.BoneMenu.Page BlockedSpawnablesnow;

	internal static BoneLib.BoneMenu.Page SpawnablesKicknow;

	internal static BoneLib.BoneMenu.Page blockentirepalletnow;

	internal static BoneLib.BoneMenu.Page blockentireauthornow;

	internal static BoneLib.BoneMenu.Page permissioneditornow;

	internal static BoneLib.BoneMenu.Page OnlineFriends;

	internal static BoneLib.BoneMenu.Page Mostusedprotections;

	internal static BoneLib.BoneMenu.Page featuresprotection;

	internal static BoneLib.BoneMenu.Page SaveTotxtpg;

	internal static System.Collections.Generic.SortedSet<(string Nickname, string Username, string PlatformId, string ExploitType)> ClientExploitLogs = new System.Collections.Generic.SortedSet<(string, string, string, string)>();

	internal static System.Collections.Generic.SortedSet<(string PlayerName, string Username, ulong PlatformID, string PalletName, string PalletAuthor, int ModioID, string BarcodeID, bool isspawnableavatar)> SpawnLogs = new System.Collections.Generic.SortedSet<(string, string, ulong, string, string, int, string, bool)>();

	internal static System.Collections.Generic.HashSet<string> BlockedSpawnables = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> MediaPlayerLogs = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> WarnedSpawnables = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<PlayerInfo> PlayersOnlines = new System.Collections.Generic.HashSet<PlayerInfo>();

	internal static System.Collections.Generic.HashSet<string> SpawnablesKick = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> voicblocked = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.Dictionary<string, int> spawnlimitline = new System.Collections.Generic.Dictionary<string, int>();

	internal static System.Collections.Generic.HashSet<string> spawnlimitlinelist = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<ulong> kicktillrestart = new System.Collections.Generic.HashSet<ulong>();

	internal static System.Collections.Generic.HashSet<string> AvatarsKick = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockedplatformids = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockedspawnies = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.Dictionary<PlayerID, System.Collections.Generic.List<string>> despawnresponselogger = new System.Collections.Generic.Dictionary<PlayerID, System.Collections.Generic.List<string>>();

	internal static System.Collections.Generic.HashSet<PlayerInfo> JoinLogger = new System.Collections.Generic.HashSet<PlayerInfo>();

	internal static System.Collections.Generic.HashSet<string> blockmovements = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockmessages = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockedavifallbacks = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> CustomAvFav = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<AvatarCrateReference> CustomAvFavref = new System.Collections.Generic.HashSet<AvatarCrateReference>();

	internal static System.Collections.Generic.HashSet<string> CustomSpawnFav = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<SpawnableCrateReference> CustomSpawnFavref = new System.Collections.Generic.HashSet<SpawnableCrateReference>();

	internal static System.Collections.Generic.HashSet<AISpawner> NPCSpawnersNow = new System.Collections.Generic.HashSet<AISpawner>();

	internal static System.Collections.Generic.HashSet<string> modidblocked = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockentirepallet = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockentireauthor = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockavipalletlist = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> blockaviauthorlist = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> warnavilist = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<string> MEDIAPLAYERBLOCKERNOWList = new System.Collections.Generic.HashSet<string>();

	internal static System.Collections.Generic.HashSet<AvatarCrateReference> AvatarsStored = new System.Collections.Generic.HashSet<AvatarCrateReference>();

	internal static System.Collections.Generic.SortedSet<string> SpawnablesStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> LevelStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> AllWeaponsStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> AllNPCStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunRiflesStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunSMGStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunRangedStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunPistolStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunShotgunStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> GunSniperStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> MeleeStored = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> MeleeStoredBlunt = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> MeleeStoredBlade = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> MeleeStoredKnife = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.SortedSet<string> NoTagSpawnables = new System.Collections.Generic.SortedSet<string>();

	internal static System.Collections.Generic.HashSet<IMatchmaker.LobbyInfo> CachedLobbies = new System.Collections.Generic.HashSet<IMatchmaker.LobbyInfo>();

	internal static System.Collections.Generic.HashSet<PlayerInfo> PlayersOnline = new System.Collections.Generic.HashSet<PlayerInfo>();

	internal static bool Invalidstatsnow = true;

	internal static bool spamspawnprevention = true;

	internal static bool notificationspamspawn = true;

	internal static bool AntiModTP = false;

	internal static bool blindplayersprevention = true;

	internal static bool ballplayersprevention = false;

	internal static bool safedistancespawning = false;

	internal static bool base64files = true;

	internal static PlayerInfo StoredNowplayerrrr;

	internal static SimpleTimer DespawnAllTimera;

	internal static bool notificationsofexploits = false;

	internal static bool blockexploitscompletely = true;

	internal static bool antibodylogeffect = true;

	internal static bool strengththresprotection = true;

	internal static bool kickifvitaly = false;

	internal static bool strengthnotif = false;

	internal static float strengththreshnow = 350f;

	internal static float speedthreshold = 120f;

	internal static bool TeleportThresHold = true;

	internal static bool spawnlogsmelonlog = false;

	internal static bool homeworldsnow = true;

	internal static bool disablesteamreading = true;

	internal static bool AltRemov = true;

	internal static bool baninsteadalt = false;

	internal static bool AltNotifications = false;

	internal static bool teleportaltacctoyou = false;

	internal static bool clonedetector = true;

	internal static bool togglesavesbool = false;

	internal static bool clientexploitclearonnewserver = false;

	internal static bool spawnlogexploitclearonnewserver = true;

	internal static bool switchlogexploitclearonnewserver = false;

	internal static bool godmode = false;

	internal static bool show = false;

	internal static bool antiknockout = false;

	internal static bool _isDumpRunning = false;

	internal static int timertodo = 0;

	internal static int timeravisafe = 0;

	internal static int timerreloadlevel = 0;

	internal static bool spooferprofiledetection = true;

	internal static bool unlammo = false;

	internal static bool selfconstraint = false;

	internal static bool isSearching = false;

	internal static int maxentrefresh = 10;

	internal static float Timerrefresh = 5f;

	internal static int vmaxentrefresh = 10;

	internal static float vTimerrefresh = 5f;

	internal static int cmaxentrefresh = 10;

	internal static int kicktime = 30;

	internal static float cTimerrefresh = 5f;

	internal static bool DespawnAllTimer = false;

	internal static int DespawnAllTimerMins = 30;

	internal static bool randomizerslzonly = false;

	internal static bool dashingnow = false;

	internal static bool autorunnow = false;

	internal static bool doublejumpnow = false;

	internal static bool Aircontrolnow = false;

	internal static bool alterrornotis = false;

	internal static bool antidespawneffect = false;

	internal static bool blockallspawnslocally = false;

	internal static bool AntiGrab = false;

	internal static bool Bodylogradialcolors = true;

	internal static bool spawnbypassprotection = true;

	internal static float fps = 0f;

	internal static float fpslimit = 13f;

	internal static bool fpsdesapwner = false;

	internal static bool servermaxdamagethres = false;

	internal static bool grippy = false;

	internal static string outofboundslobbycode;

	internal static bool outofboundsnow = false;

	internal static bool bodylog = false;

	internal static bool bodylogplayers = false;

	internal static bool AntiDevManip = false;

	internal static bool BLOCKAVATARSASSPAWNABLES = true;

	internal static bool AntiLasereyes = false;

	internal static bool AntiBodyLogGrief = true;

	internal static bool antidecal = false;

	internal static string rejoinlastserver = "";

	internal static bool globalblocklistnotification = true;

	internal static bool globalblocklistnow = true;

	internal static float timerfoeesa = 10f;

	internal static bool globalFPBANLIST = true;

	internal static bool globalbannotification = true;

	internal static bool DESPAWNPROTECTION = true;

	internal static bool privatekicksteam = false;

	internal static bool AntiGravityChange = false;

	internal static bool hideholsters = false;

	internal static bool hideholstersplayers = false;

	internal static bool infiniteinventory = false;

	internal static bool infiniteinvall = true;

	internal static int bodylogindex = 1;

	internal static int currentbodylogindex = 1;

	internal static bool spawnableskickon = true;

	internal static bool avatarskickon = true;

	internal static bool tpback10seconds = false;

	internal static Vector3 Seconds10back = new Vector3(0f, 0f, 0f);

	internal static bool ownerscanchangemap = true;

	internal static bool localonlydevtools = false;

	internal static bool bodylogsending = true;

	internal static string COLORR = "0";

	internal static string COLORG = "255";

	internal static string COLORB = "0";

	internal static string COLORA = "255";

	internal static bool dropallbefore = false;

	internal static bool statkicker = false;

	internal static bool KEEPLOADOUTINVENTORY = false;

	internal static string bansearcher = "";

	internal static bool SpawnGunProtection = false;

	internal static bool showammoalways = false;

	internal static bool personalspace = false;

	internal static float personalspacevalue = 1.8f;

	internal static bool blockspwnnotis = true;

	internal static bool autosavenow = false;

	internal static bool warnavinow = true;

	internal static bool blockaviauthornow = true;

	internal static bool blockavipalletnow = true;

	internal static bool OWNERSCANCHANGESERVER = true;

	internal static bool ownerscanchangegamemode = true;

	internal static bool aviswitchprotection = false;

	internal static bool HideNemesisAntiCheat = false;

	internal static bool removesounds = false;

	internal static bool sharebodylogpagenow = true;

	internal static bool sharedevtoolpresets = true;

	internal static SearchMethod searchmethodavatarreal = SearchMethod.CrateNames;

	internal static SearchMethod searchmethodlevelreal = SearchMethod.CrateNames;

	internal static SearchMethod searchspawnabletypereal = SearchMethod.CrateNames;

	internal static handnow handnowreal = handnow.Right;

	internal static PermissionLevel permlevel = PermissionLevel.DEFAULT;

	internal static DespawnerAll DespawnerAllReal = DespawnerAll.AllNotButtonsLeverCircuits;

	internal static DespawnerAll DespawnerAllReal2 = DespawnerAll.AllNotButtonsLeverCircuits;

	internal static DespawnerAll DespawnerTimerAllReal = DespawnerAll.AllNotButtonsLeverCircuits;

	internal static Slots BodySlotReal = Slots.BackRight;

	internal static DespawnerAll DespawnerTimerz = DespawnerAll.AllNotButtonsLeverCircuits;

	internal static SpawnableSearchType spawnablesrchtype = SpawnableSearchType.Spawn;

	internal static AvatarSearchType AvatarSearchTypeReal = AvatarSearchType.ChangeInto;

	internal static Slots SlotsNowReal = Slots.HolsterLeft;

	internal static antimodguntype antimodguntypereal = antimodguntype.AnySpawnGun;

	internal static float scroll = 0f;

	internal static float scrollX = 0f;

	internal static float pscroll = 0f;

	internal static MenuSections PageNow = MenuSections.SelfCat;

	internal static MenuSections PreviousPage = MenuSections.SelfCat;

	internal static string aviSpawnLogsSearcher = "";

	internal static string SpawnLogsSearcher = "";

	internal static string modrecnon = "";

	internal static string modrecmature = "";

	internal static string aviseachnow = "";

	internal static string damagelogsearcher = "";

	internal static string SpawnableSearchies = "";

	internal static string AvatarSearchies = "";

	internal static string avifavsearcher = "";

	internal static string spawnablefavsearcher = "";

	internal static string playerinfospawnlogs = "";

	internal static string lobbyinfospawnlogs = "";

	internal static string playerinfoavatarswitch = "";

	internal static string netentities = "";

	internal static string findem = "";

	internal static string avichnge = "SLZ.BONELAB.Core.Avatar.PeasantFemaleA";

	internal static (string PlayerName, string Username, string PlatformID, string PalletName, string PalletAuthor, string ModioID, string BarcodeID, string IsSpawnableAvatar) SpawnLogsRef;

	internal static (string modname, string thumbnail, string id, string ismature) ModInforecv;

	internal static (string PlayerName, string Username, string PlatformID, string PalletName, string PalletAuthor, string ModioID, string BarcodeID) AvatarSwitchyNow;

	internal static System.Collections.Generic.List<string> SpawnablerResultsNow = new System.Collections.Generic.List<string>();

	internal static System.Collections.Generic.List<string> AvisResultsNow = new System.Collections.Generic.List<string>();

	internal static System.Collections.Generic.List<NetworkEntity> ListNetworkEntities = new System.Collections.Generic.List<NetworkEntity>();

	internal static SpawnableCrateReference serresult;

	internal static AvatarCrateReference avirez;

	internal static SpawnableCrateReference spawnyfavorite;

	internal static AvatarCrateReference avifavorite;

	internal static string MODIOINT = "";

	internal static NetworkPlayer storeynow;

	internal static string[] testholder = new string[9];

	internal static System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>> PlayerSpawningStuff = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>();

	internal static System.Collections.Generic.HashSet<SpawnableCrateReference> playersspawnrefs = new System.Collections.Generic.HashSet<SpawnableCrateReference>();

	internal static SpawnableCrateReference playersspawnrefsstored;

	internal static NetworkEntity nettyspawnedc;

	internal static System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>> PlayeravatarStuff = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>();

	internal static System.Collections.Generic.HashSet<AvatarCrateReference> playersavatarrefs = new System.Collections.Generic.HashSet<AvatarCrateReference>();

	internal static AvatarCrateReference playersavatarrefsstored;

	internal static int itemsPerPage = 20;

	internal static int currentPage = 0;

	internal static int currentPage2 = 0;

	internal static int currentPage3 = 0;

	internal static CheatTool InstanceOfIt;

	internal static LobbyInfo storedlobby;

	internal static NetworkPlayer storedplz;

	internal static Pallet modiostorednow;

	internal static LobbyInfo lobbyinfofrominstall;

	internal static string storedmedia;

	internal static BanInfo storedban;

	internal static System.Collections.Generic.HashSet<LobbyInfo> ServerHistorys = new System.Collections.Generic.HashSet<LobbyInfo>();

	internal static float spawnlimitertimer = 0.75f;

	internal static bool spawnlimiternow = false;

	internal static bool hostonlyspawnlimiter = false;

	internal static bool OwnerCheckEnabled = true;

	internal static bool despawndeadnpcs = false;

	internal static bool limitplayercount = false;

	internal static bool BlockPalletCompletely = true;

	internal static bool BlockAuthorOfSpawnable = true;

	internal static bool blockedspawnables = true;

	internal static bool warnedspawnables = true;

	internal static bool ModIDBlocker = true;

	internal static bool donotdisturb = false;

	internal static bool spoofedusernameusername = true;

	internal static bool globalspawnlimitperitem = false;

	internal static int limitnowofglobal = 10;

	internal static int limithostonly = 10;

	internal static string messagenowplayer = "Hi!...";

	internal static float messgfloattime = 2.5f;

	internal static bool nemesisprotectedlobby = true;

	internal static bool disablemediaplayers = false;

	internal static bool mediaplayerprotection = true;

	internal static bool spawnprotectionsnot_host = true;

	internal static readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("nac_bonelabsupportpatches");

	internal static bool REMOVEDGLOBALBANLIST = false;

	internal static bool forcegrabdisablernow = false;

	internal static bool disablewindsfx = false;

	internal static bool cleandisconnect = false;

	internal static bool modomatonload = false;

	internal static bool autokickoldNemesisAntiCheatusers = false;

	internal static bool AutoKickSpoofers = false;

	internal static bool fullspawnprotection = false;

	internal static int spawnprotectiontimer = 20;

	internal static PermissionLevel TempLevelNow = PermissionLevel.DEFAULT;

	internal static SimpleTimer EmergencyEscapetimer;

	internal static bool removeproxchat = false;

	internal static bool forcenametagson = false;

	internal static bool preventnotificationlag = true;

	internal static bool spawngunatleastonce = false;

	internal static bool spawngunuialways = true;

	internal static bool keephiddenmods = false;

	internal static bool DeleteLastLobbyMods = false;

	internal static bool nodamageunlessweapons = false;

	internal static bool modidsending = true;

	internal static bool kickunincodenames = false;

	internal static bool AntiAnimatedName = false;

	internal static bool playermessaging = true;

	internal static bool bitsending = true;

	internal static Il2CppSystem.Collections.Generic.List<string> originalbodylog;

	internal static PlayerInfo originalprofiledetails;

	internal static string searchedloggedlobbies = "";

	internal static string searchedloggedPLAYER = "";

	internal static string modiosearchernow = "";

	internal static bool logrecentlymet = false;

	internal static bool logmediaplayer = false;

	internal static bool loglobbiessince = false;

	internal static bool logplayersince = false;

	internal static ModCallbackInfo moddyinfostored;

	internal static bool HIDEPLAYERLIST = false;

	internal static string modiotoken = "";

	internal static bool antioneshot = false;

	internal static bool antighostmode = true;

	internal static bool ghostmodekick = false;

	internal static float ghostmodebounds = 900f;

	internal static bool antimetadataspoof = true;

	internal static bool blockprivatejoiners = false;

	internal static System.Collections.Generic.Dictionary<string, float> exploitNotifyTimes = new System.Collections.Generic.Dictionary<string, float>();

	internal static int countbeforespam = 10;

	internal static int antispamspawntimer = 1000;

	internal static readonly System.Collections.Generic.Dictionary<InventorySlotReceiver, (string Barcode, string SlotName)> weaponsInInventory = new System.Collections.Generic.Dictionary<InventorySlotReceiver, (string, string)>();

	internal static readonly System.Collections.Generic.List<FileSystemWatcher> watchers = new System.Collections.Generic.List<FileSystemWatcher>();

	internal static System.Collections.Generic.List<SpoofChecker> StoredJoinPlayers = new System.Collections.Generic.List<SpoofChecker>();

	internal static System.Collections.Generic.Dictionary<ulong, string> LastKnownUsernames = new System.Collections.Generic.Dictionary<ulong, string>();

	internal bool fpsCheckRunning = false;

	internal static System.Collections.Generic.List<SearchHistoryEntry> SearchHistorynow = new System.Collections.Generic.List<SearchHistoryEntry>();

	private static bool _isLookingUpMod = false;

	internal static void CreateDevToolPreset(string titleOfPreset, CreateCheatToolsPreset.Item item1, CreateCheatToolsPreset.Item item2, CreateCheatToolsPreset.Item item3, CreateCheatToolsPreset.Item item4, CreateCheatToolsPreset.Item item5)
	{
		bool flag = false;
		foreach (CreateCheatToolsPreset cheatPreset in CreateCheatToolsPreset.CheatPresets)
		{
			if (string.Equals(cheatPreset.TitleOfPreset, titleOfPreset, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			CreateCheatToolsPreset.CheatPresets.Add(new CreateCheatToolsPreset(titleOfPreset, item1, item2, item3, item4, item5));
			CreateCheatToolsPreset.SavePresets();
			NotificationNow("Nemesis Anti-Cheat", "Added Preset " + titleOfPreset + "!", NotificationType.SUCCESS, 2.5f);
		}
		else
		{
			NotificationNow("Nemesis Anti-Cheat", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
		}
	}

	internal static void CreateBodyLogPage(string pagename, string slot1 = "c3534c5a-94b2-40a4-912a-24a8506f6c79", string slot2 = "c3534c5a-94b2-40a4-912a-24a8506f6c79", string slot3 = "c3534c5a-94b2-40a4-912a-24a8506f6c79", string slot4 = "c3534c5a-94b2-40a4-912a-24a8506f6c79", string slot5 = "c3534c5a-94b2-40a4-912a-24a8506f6c79", string slot6 = "c3534c5a-94b2-40a4-912a-24a8506f6c79")
	{
		bool flag = false;
		foreach (BodyLogPage bodyLogPage in BodyLogPage.BodyLogPages)
		{
			if (string.Equals(bodyLogPage.TitleOfPreset, pagename, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			BodyLogPage.BodyLogPages.Add(new BodyLogPage(pagename, slot1, slot2, slot3, slot4, slot5, slot6));
			BodyLogPage.SavePresets();
			NotificationNow("Nemesis Anti-Cheat", "Added Preset " + pagename + "!", NotificationType.SUCCESS, 2.5f);
		}
		else
		{
			NotificationNow("Nemesis Anti-Cheat", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
		}
	}

	internal static string DataToJsonString(object listorsomething)
	{
		string text = "";
		JsonSerializerOptions options = new JsonSerializerOptions
		{
			WriteIndented = true,
			IncludeFields = true
		};
		return System.Text.Json.JsonSerializer.Serialize(listorsomething, options);
	}

	internal static void SaveTXTFunc()
	{
		if (logrecentlymet)
		{
			File.WriteAllText(RECNETLYMETLOGGED, DataToJsonString(JoinLogger));
		}
		if (logmediaplayer)
		{
			File.WriteAllText(MEDIAPLAYERLOGS, DataToJsonString(MediaPlayerLogs));
		}
		if (loglobbiessince)
		{
			File.WriteAllText(LOBBIESLOGGEDSINCE, DataToJsonString(CachedLobbies));
		}
		if (logplayersince)
		{
			File.WriteAllText(PLAYERSLOGGEDSINCE, DataToJsonString(PlayersOnline));
		}
	}

	internal static bool MostUsedItems(string barcode)
	{
		return SiteStuff.mostusedexcl.Contains(barcode);
	}

	internal static void DeleteModioMod(Pallet PalletNow, bool notif = true)
	{
		if (CrateFilterer.GetModID(PalletNow) == -1)
		{
			return;
		}
		var (text, path, text2, palletManifest) = GetPalletFolder(PalletNow?.name);
		if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
		{
			UnloadPallet(PalletNow);
			Directory.Delete(text, recursive: true);
			File.Delete(path);
			if (notif)
			{
				NotificationNow("Nemesis Anti-Cheat", "Deleted " + PalletNow?.name + ".", NotificationType.ERROR, 3f);
			}
		}
	}

	internal static void UnloadPallet(Pallet pallet)
	{
		if (pallet == null)
		{
			return;
		}
		Il2CppSystem.Collections.Generic.List<PackedAsset> packedAssets = pallet._packedAssets;
		if (packedAssets != null)
		{
			Il2CppSystem.Collections.Generic.List<PackedAsset>.Enumerator enumerator = packedAssets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current?.marrowAsset.UnloadAsset(forced: true);
			}
		}
		AssetWarehouse.Instance.UnloadPallet(pallet);
		MelonLogger.Msg("Unloaded pallet: " + pallet.Title);
	}

	internal static (string folderpath, string manifestpath, string fullpallet, PalletManifest palletm) GetPalletFolder(string palletTitle, bool openSelectionPc = false)
	{
		Il2CppArrayBase<PalletManifest> il2CppArrayBase = AssetWarehouse.Instance?.GetPalletManifests()?.ToArray();
		if (il2CppArrayBase == null)
		{
			return (folderpath: null, manifestpath: null, fullpallet: null, palletm: null);
		}
		PalletManifest palletManifest = il2CppArrayBase.FirstOrDefault((PalletManifest m) => m.Pallet != null && m.Pallet.Title == palletTitle);
		if (palletManifest?.PalletPath == null)
		{
			return (folderpath: null, manifestpath: null, fullpallet: null, palletm: null);
		}
		string directoryName = Path.GetDirectoryName(palletManifest.PalletPath);
		if (openSelectionPc && Directory.Exists(directoryName))
		{
			Process.Start("explorer.exe", "/select,\"" + directoryName + "\"");
			NotificationNow("Nemesis Anti-Cheat", "Opened " + palletTitle + " download folder.", NotificationType.SUCCESS, 2f);
		}
		return (folderpath: directoryName, manifestpath: palletManifest.ManifestPath, fullpallet: palletManifest.PalletPath, palletm: palletManifest);
	}

	internal static IEnumerator OnStartOfGame()
	{
		if (homeworldsnow)
		{
			string defaultBarcode = "c2534c5a-80e1-4a29-93ca-f3254d656e75";
			string barcode = defaultBarcode;
			if (File.Exists(homeworldnow))
			{
				string text = File.ReadAllText(homeworldnow);
				if (IsBarcodeInGame(text))
				{
					barcode = text;
				}
			}
			SceneStreamer.Load(new LevelCrateReference(barcode).Barcode);
		}
		yield return new WaitForSeconds(15f);
		MelonCoroutines.Start(SiteStuff.UpdateSites());
	}

	internal static IEnumerator RunAfterBuild()
	{
		yield return new WaitForSeconds(5f);
		ManuallySave(notify: false);
		if (disablewindsfx)
		{
			Il2CppArrayBase<WindBuffetSFX> icons = UnityEngine.Object.FindObjectsOfType<WindBuffetSFX>();
			foreach (WindBuffetSFX icon in icons)
			{
				icon.windBuffetClip = null;
				icon._buffetSrc = null;
			}
		}
		if (grippy)
		{
			Il2CppArrayBase<InteractableIcon> icons2 = UnityEngine.Object.FindObjectsOfType<InteractableIcon>();
			foreach (InteractableIcon icon2 in icons2)
			{
				icon2.IconSize = 0f;
				icon2.scaledIconSize = 0f;
			}
		}
		if (infiniteinventory)
		{
			NotificationNow("Nemesis Anti-Cheat", "Stored Current Inventory!\nRe-Enable For Storing New Stuff!", NotificationType.SUCCESS, 2f);
			StoreInventoryItems();
		}
		if (!HideNemesisAntiCheat)
		{
			ModuleMessageManager.RegisterHandler<SendNotificationMessage>();
			ModuleMessageManager.RegisterHandler<ProtectorPingMessage>();
		}
		ModuleMessageManager.RegisterHandler<OwnerServerSettingMessage>();
		ModuleMessageManager.RegisterHandler<SendGameModeOverMessage>();
		ModuleMessageManager.RegisterHandler<SendBodyLogMessage>();
		ModuleMessageManager.RegisterHandler<SendModIDMessage>();
		ModuleMessageManager.RegisterHandler<ShareBodyLogPageMessage>();
		ModuleMessageManager.RegisterHandler<ShareDevToolPresetMessage>();
		ModuleMessageManager.RegisterHandler<SendBitMessage>();
		ModuleMessageManager.RegisterHandler<SendBase64FileMessage>();
	}

	internal static ulong SteamIdYours()
	{
		if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
		{
			return 0uL;
		}
		return SteamClient.SteamId.Value;
	}

	internal static void WatchFileChanges(string filePath)
	{
		string directoryName = Path.GetDirectoryName(filePath);
		string fileName = Path.GetFileName(filePath);
		FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(directoryName)
		{
			Filter = fileName,
			NotifyFilter = NotifyFilters.LastWrite,
			EnableRaisingEvents = true
		};
		fileSystemWatcher.Changed += delegate
		{
			ReloadList();
			PermissionList.ReadFile();
		};
		watchers.Add(fileSystemWatcher);
	}

	// Cooldown gate so the per-frame protections (e.g. ghost mode) only raise an exploit
	// notification once every few seconds per player instead of every dropped message. Throttles
	// only WHEN we notify; the notification itself still goes through NetworkSpawnerNotif.
	internal static bool ExploitNotifyReady(ulong platformID, string tag, float cooldown = 3f)
	{
		string key = platformID.ToString() + "|" + tag;
		float now = Time.realtimeSinceStartup;
		if (exploitNotifyTimes.TryGetValue(key, out var last) && now - last < cooldown)
		{
			return false;
		}
		exploitNotifyTimes[key] = now;
		return true;
	}

	internal static void NetworkSpawnerNotif(NetworkPlayer playerusingexploits, string messageforit, NotificationType typenow = NotificationType.ERROR, float notificationtime = 1.5f, bool savetothemenu = true)
	{
		string text = ((playerusingexploits != null) ? CleanedNAME(playerusingexploits) : "Unknown");
		string message = (playerusingexploits.PlayerID.IsHost ? messageforit : ("Person Doing : " + text + "\n" + messageforit));
		NotificationNow("Nemesis Anti-Cheat", message, typenow, notificationtime, showtitle: true, savetothemenu);
		(string, string, string, string) item = (playerusingexploits.ND_Nickname(), playerusingexploits.ND_Username(), playerusingexploits.PlayerID.PlatformID.ToString(), messageforit);
		if (!ClientExploitLogs.Contains(item))
		{
			ClientExploitLogs.Add(item);
		}
	}

	internal static void EditFusionPreferences(string valuetoedit, object valuetochange)
	{
		MelonPreferences_Category category = MelonPreferences.GetCategory("BONELAB Fusion");
		if (category != null)
		{
			MelonPreferences_Entry melonPreferences_Entry = category.Entries.FirstOrDefault((MelonPreferences_Entry e) => e.DisplayName == valuetoedit);
			if (melonPreferences_Entry != null)
			{
				melonPreferences_Entry.BoxedValue = valuetochange;
				category.SaveToFile();
			}
		}
	}

	internal IEnumerator FPSCheck()
	{
		fpsCheckRunning = true;
		float badTime = 0f;
		for (float t = 0f; t < 10f; t += Time.deltaTime)
		{
			if (fps <= fpslimit)
			{
				badTime += Time.deltaTime;
				if (badTime >= 6f)
				{
					DespawnAll(DespawnerTimerz);
					break;
				}
			}
			else
			{
				badTime = 0f;
			}
			yield return null;
		}
		fpsCheckRunning = false;
	}

	internal static void TryPatchit(string dll, string typeName, string methodName, MethodInfo prefix, MethodInfo postfix)
	{
		Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == dll);
		if (assembly == null)
		{
			return;
		}
		Type type = assembly.GetType(typeName);
		if (!(type == null))
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!(method == null))
			{
				harmony.Patch(method, (prefix != null) ? new HarmonyMethod(prefix) : null, (postfix != null) ? new HarmonyMethod(postfix) : null);
			}
		}
	}

	internal static bool IsMagazine(SpawnableCrateReference reffy)
	{
		if (reffy == null || reffy.Crate == null)
		{
			return false;
		}
		SpawnableCrate crate = reffy.Crate;
		string text = crate.name.ToLower();
		string text2 = crate.Barcode?.ID.ToLower();
		for (int i = 0; i < crate.Tags.Count; i++)
		{
			crate.Tags[i] = crate.Tags[i]?.ToLowerInvariant();
		}
		Il2CppSystem.Collections.Generic.List<string> tags = crate.Tags;
		return text.Contains(" mag ") || text.EndsWith(" mag") || text.StartsWith("mag ") || text.StartsWith("mag_") || text.Contains("magazine") || text.EndsWith(" mag") || text.EndsWith("_mag") || text.StartsWith("cartridge") || text.Contains("cartridge") || text.EndsWith(" shells") || text.StartsWith("cartridge - ") || tags.Contains("mag") || tags.Contains("magazine") || tags.Contains("magazines") || tags.Contains("cartridge") || text2.EndsWith("mag") || text2.EndsWith("cartridge") || text2.StartsWith("cartridge") || text2.Contains("cartridge");
	}

	internal static bool IsMagazine(Crate reffy)
	{
		if (reffy == null)
		{
			return false;
		}
		string text = reffy.name.ToLower();
		string text2 = reffy.Barcode?.ID.ToLower();
		for (int i = 0; i < reffy.Tags.Count; i++)
		{
			reffy.Tags[i] = reffy.Tags[i]?.ToLowerInvariant();
		}
		Il2CppSystem.Collections.Generic.List<string> tags = reffy.Tags;
		return text.Contains(" mag ") || text.EndsWith(" mag") || text.StartsWith("mag ") || text.StartsWith("mag_") || text.Contains("magazine") || text.EndsWith(" mag") || text.EndsWith("_mag") || text.StartsWith("cartridge") || text.Contains("cartridge") || text.EndsWith(" shells") || text.StartsWith("cartridge - ") || tags.Contains("mag") || tags.Contains("magazine") || tags.Contains("magazines") || tags.Contains("cartridge") || text2.EndsWith("mag") || text2.EndsWith("cartridge") || text2.StartsWith("cartridge") || text2.Contains("cartridge");
	}

	internal static (PullCordDevice bodylogreturn, MeshRenderer Outerring) ND_BodyLog(PhysicsRig PlayerTodo)
	{
		if (PlayerTodo != null)
		{
			Transform transform = PlayerTodo.m_elbowRt?.Find("BodyLogSlot/BodyLog");
			Transform transform2 = PlayerTodo.m_elbowRt?.Find("BodyLogSlot/BodyLog/BodyLog/BodyLog");
			MeshRenderer item = ((transform2 != null) ? transform2.GetComponent<MeshRenderer>() : null);
			if (transform != null)
			{
				PullCordDevice component = transform.GetComponent<PullCordDevice>();
				if (component != null)
				{
					return (bodylogreturn: component, Outerring: item);
				}
			}
			Transform transform3 = PlayerTodo.m_elbowLf?.Find("BodyLogSlot/BodyLog");
			Transform transform4 = PlayerTodo.m_elbowLf?.Find("BodyLogSlot/BodyLog/BodyLog/BodyLog");
			MeshRenderer item2 = ((transform4 != null) ? transform4.GetComponent<MeshRenderer>() : null);
			if (transform3 != null)
			{
				PullCordDevice component2 = transform3.GetComponent<PullCordDevice>();
				if (component2 != null)
				{
					return (bodylogreturn: component2, Outerring: item2);
				}
			}
		}
		return (bodylogreturn: null, Outerring: null);
	}

	internal static NetworkPlayer ND_YourNetworkPlayer()
	{
		return LocalPlayer.GetNetworkPlayer();
	}

	internal static System.Collections.Generic.HashSet<NetworkEntity> NetworkEntities()
	{
		return NetworkEntityManager.IDManager?.RegisteredEntities?.EntityIDLookup?.Keys?.Where(delegate(NetworkEntity p)
		{
			MarrowEntity marrowEntity = p.ND_GetMarrowEntity();
			return marrowEntity != null && !marrowEntity.ND_IsNetPlayer() && marrowEntity.ND_GetBarcodeID() != "Lakatrazz.FusionContent.Spawnable.NameTag" && p != ND_YourNetworkPlayer().NetworkEntity;
		}).ToHashSet();
	}

	internal static System.Collections.Generic.HashSet<NetworkPlayer> NetworkPlayers(bool excludeMe = false, bool excludeMeAndHost = false)
	{
		return (from p in NetworkPlayer.Players.Where(delegate(NetworkPlayer p)
			{
				if (p == null || !p.PlayerID.IsValid)
				{
					return false;
				}
				if (excludeMe && p.IsMe())
				{
					return false;
				}
				return (!excludeMeAndHost || (!p.IsMe() && !p.PlayerID.IsHost)) ? true : false;
			})
			orderby p.PlayerID.IsHost descending
			select p).ThenBy<NetworkPlayer, string>((NetworkPlayer p) => StripColorTags(string.IsNullOrEmpty(p.Username) ? "" : p.Username).Trim(), StringComparer.OrdinalIgnoreCase).ToHashSet();
	}

	internal static string BodySlot(Slots slot)
	{
		if (1 == 0)
		{
		}
		string result = slot switch
		{
			Slots.HolsterLeft => "SideLf", 
			Slots.HolsterRight => "SideRt", 
			Slots.BackLeft => "BackLf", 
			Slots.BackRight => "BackRt", 
			Slots.BottomRight => "BackCt", 
			Slots.Head => "HeadSlot", 
			_ => string.Empty, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	internal static Hand ND_YourGetHand(WhichHand hand)
	{
		PhysicsRig physicsRig = Player.RigManager?.physicsRig;
		if (physicsRig == null)
		{
			return null;
		}
		if (1 == 0)
		{
		}
		Hand result = hand switch
		{
			WhichHand.Left => physicsRig.leftHand, 
			WhichHand.Right => physicsRig.rightHand, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	internal static IEnumerator DumpPalletsCoroutine()
	{
		if (_isDumpRunning)
		{
			MelonLogger.Error("⚠\ufe0f Crate dump is already running!");
			yield break;
		}
		_isDumpRunning = true;
		MelonLogger.Warning("\ud83d\udd0d Starting crate dump...");
		StringBuilder sb = new StringBuilder();
		Il2CppSystem.Collections.Generic.List<Pallet> pallets = AssetWarehouse.Instance.GetPallets();
		int batchSize = 150;
		int processedCrates = 0;
		int totalCrates = 0;
		Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator = pallets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Pallet pallet = enumerator.Current;
			if (pallet.Crates != null)
			{
				totalCrates += pallet.Crates.Count;
			}
		}
		Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator2 = pallets.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Pallet pallet2 = enumerator2.Current;
			StringBuilder stringBuilder = sb;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(20, 2, stringBuilder);
			handler.AppendLiteral("[Pallet] ");
			handler.AppendFormatted(StripColorTags(pallet2.Title));
			handler.AppendLiteral(" (Author: ");
			handler.AppendFormatted(pallet2.Author);
			handler.AppendLiteral(")");
			stringBuilder2.AppendLine(ref handler);
			if (pallet2.Crates == null)
			{
				continue;
			}
			Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator3 = pallet2.Crates.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				Crate cratey = enumerator3.Current;
				if (cratey.Tags != null)
				{
					stringBuilder = sb;
					StringBuilder stringBuilder3 = stringBuilder;
					handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder);
					handler.AppendLiteral("  └── [Crate] ");
					handler.AppendFormatted(StripColorTags(cratey.Title));
					stringBuilder3.AppendLine(ref handler);
					stringBuilder = sb;
					StringBuilder stringBuilder4 = stringBuilder;
					handler = new StringBuilder.AppendInterpolatedStringHandler(19, 1, stringBuilder);
					handler.AppendLiteral("      └── Barcode: ");
					handler.AppendFormatted(cratey.Barcode.ID);
					stringBuilder4.AppendLine(ref handler);
					System.Collections.Generic.HashSet<string> tagsSet = new System.Collections.Generic.HashSet<string>();
					Il2CppSystem.Collections.Generic.List<string>.Enumerator enumerator4 = cratey.Tags.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						string tag = enumerator4.Current;
						tagsSet.Add(tag);
					}
					if (tagsSet.Count > 0)
					{
						stringBuilder = sb;
						StringBuilder stringBuilder5 = stringBuilder;
						handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder);
						handler.AppendLiteral("      └── Tags: ");
						handler.AppendFormatted(string.Join(", ", tagsSet));
						stringBuilder5.AppendLine(ref handler);
					}
					processedCrates++;
					if (processedCrates % batchSize == 0)
					{
						int barLength = 30;
						float progress = (float)processedCrates / (float)totalCrates;
						int filled = (int)(progress * (float)barLength);
						string bar = $"[{new string('#', filled)}{new string('-', barLength - filled)}] {progress:P1}";
						MelonLogger.Warning("⏳ Dumping crates... " + bar);
						yield return new WaitForSeconds(0.1f);
					}
				}
			}
			sb.AppendLine();
		}
		MelonLogger.Warning("⏳ Dumping crates... [##############################] 100%");
		try
		{
			File.WriteAllText(PalletDumpLocation, sb.ToString());
			MelonLogger.Warning("✅ Crate dump written to: " + PalletDumpLocation);
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = "/c start \"\" \"" + PalletDumpLocation + "\"",
				CreateNoWindow = true,
				UseShellExecute = false
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MelonLogger.Error("Failed to write pallet dump: " + ex2.Message);
		}
		_isDumpRunning = false;
	}

	internal static void CheckSteamID(ulong steamid)
	{
		string linknow = $"https://steamcommunity.com/profiles/{steamid}";
		try
		{
			OpenPageNow(linknow);
			NotificationNow("Nemesis Anti-Cheat", "Opened Steam profile in browser.", NotificationType.SUCCESS);
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("Failed to open URL: " + ex.Message);
			NotificationNow("Nemesis Anti-Cheat", "Failed to open profile link.", NotificationType.ERROR);
		}
	}

	internal static void NotificationNow(string Title, string Message, NotificationType Type, float length = 1f, bool showtitle = true, bool savetomenu = false, Action Accept = null, Action Decline = null)
	{
		if (!donotdisturb)
		{
			Notifier.Cancel(Notifier.CurrentNotification);
			Notifier.Send(new Notification
			{
				PopupLength = length,
				Title = Title,
				Message = Message,
				ShowPopup = showtitle,
				SaveToMenu = savetomenu,
				OnAccepted = Accept,
				OnDeclined = Decline,
				Type = Type,
				Tag = "Nemesis Anti-Cheat"
			});
		}
	}

	internal static void NotificationNowAlways(string Title, string Message, NotificationType Type, float length = 1f, bool showtitle = true, bool savetomenu = false, Action Accept = null, Action Decline = null)
	{
		Notifier.Cancel(Notifier.CurrentNotification);
		Notifier.Send(new Notification
		{
			PopupLength = length,
			Title = Title,
			Message = Message,
			ShowPopup = showtitle,
			SaveToMenu = savetomenu,
			OnAccepted = Accept,
			OnDeclined = Decline,
			Type = Type,
			Tag = "Nemesis Anti-Cheat"
		});
	}

	internal static (object result, Type returnType) InvokeWithType(Type classToAccess, string functionToInvoke, object[] functionParameters)
	{
		MethodInfo methodInfo = AccessTools.Method(classToAccess, functionToInvoke);
		if (methodInfo == null)
		{
			return (result: null, returnType: null);
		}
		object item = methodInfo.Invoke(null, functionParameters);
		return (result: item, returnType: methodInfo.ReturnType);
	}

	internal static void ToggleAddRemoveFromFile(string item, System.Collections.Generic.HashSet<string> listToUse, string filePath, string notificationTitle, string addedMessage, string removedMessage, bool notifications = true)
	{
		if (string.IsNullOrWhiteSpace(item))
		{
			return;
		}
		if (listToUse.Remove(item))
		{
			if (notifications)
			{
				NotificationNow(notificationTitle, removedMessage, NotificationType.ERROR, 3f);
			}
		}
		else
		{
			listToUse.Add(item);
			if (notifications)
			{
				NotificationNow(notificationTitle, addedMessage, NotificationType.SUCCESS, 3f);
			}
		}
		File.WriteAllLines(filePath, listToUse);
	}

	internal static (string left, string right) ParseLine(string line)
	{
		string text = line.Trim();
		string[] array = text.Split(':');
		string item = ((array.Length != 0) ? array[0].Trim() : text);
		string item2 = ((array.Length > 1) ? array[1].Trim() : string.Empty);
		return (left: item, right: item2);
	}

	internal static string ND_YourAvatarBarcodeID()
	{
		return Player.RigManager?.AvatarCrate?.Barcode?.ID ?? "NULL";
	}

	internal static void ChangeIntoAvi(string avibarcode)
	{
		if (IsBarcodeInGame(avibarcode) && !(ND_YourAvatarBarcodeID() == avibarcode))
		{
			AvatarCrateReference avatarCrateReference = new AvatarCrateReference(avibarcode);
			Player.RigManager.SwapAvatarCrate(avatarCrateReference.Barcode);
			DataManager.ActiveSave.PlayerSettings.CurrentAvatar = avatarCrateReference.Barcode.ID;
			DataManager.TrySaveActiveSave(SaveFlags.Progression);
			AvatarCrate crate = avatarCrateReference.Crate;
			if (crate != null)
			{
				LocalPlayer.Metadata.AvatarTitle.SetValue(crate.Title);
				LocalPlayer.Metadata.AvatarModID.SetValue(CrateFilterer.GetModID(crate.Pallet));
			}
		}
	}

	internal static bool ContainsInvisibleUnicode(string text)
	{
		foreach (char c in text)
		{
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
			if (unicodeCategory == UnicodeCategory.Format || unicodeCategory == UnicodeCategory.Control || unicodeCategory == UnicodeCategory.OtherNotAssigned)
			{
				return true;
			}
		}
		return false;
	}

	internal static IEnumerator Search(string search, BoneLib.BoneMenu.Page results, SearchMethod SearchMethodNow, SearchMethodType SpawnableType, Action<string> FunctionPress)
	{
		if (isSearching)
		{
			NotificationNow("Nemesis Anti-Cheat", "[Searcher] Is Running Please Wait Till It Says Completed!", NotificationType.SUCCESS, 3f);
			yield break;
		}
		isSearching = true;
		int count = 0;
		results?.RemoveAll();
		string searchLower = search.ToLower();
		Il2CppSystem.Collections.Generic.List<Pallet> pallets = AssetWarehouse.Instance.GetPallets();
		int batchSize = 100;
		int processed = 0;
		Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator = pallets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Pallet pallet = enumerator.Current;
			if (SearchMethodNow == SearchMethod.PalletName && pallet.name.ToLower().Contains(searchLower))
			{
				int validCount = 0;
				Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator2 = pallet.Crates.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Crate crate = enumerator2.Current;
					bool valid = false;
					if (SpawnableType == SearchMethodType.Avatar)
					{
						valid = CrateFilterer.GetCrate<AvatarCrate>(crate.Barcode) != null;
					}
					if (SpawnableType == SearchMethodType.Level)
					{
						valid = CrateFilterer.GetCrate<LevelCrate>(crate.Barcode) != null;
					}
					if (SpawnableType == SearchMethodType.Spawnable)
					{
						valid = CrateFilterer.GetCrate<SpawnableCrate>(crate.Barcode) != null;
					}
					if (valid)
					{
						validCount++;
					}
				}
				if (validCount > 0)
				{
					BoneLib.BoneMenu.Page palletPage = results?.CreatePage("+ " + pallet.name, Color.green);
					Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator3 = pallet.Crates.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						Crate crate2 = enumerator3.Current;
						bool valid2 = false;
						if (SpawnableType == SearchMethodType.Avatar)
						{
							valid2 = CrateFilterer.GetCrate<AvatarCrate>(crate2.Barcode) != null;
						}
						if (SpawnableType == SearchMethodType.Level)
						{
							valid2 = CrateFilterer.GetCrate<LevelCrate>(crate2.Barcode) != null;
						}
						if (SpawnableType == SearchMethodType.Spawnable)
						{
							valid2 = CrateFilterer.GetCrate<SpawnableCrate>(crate2.Barcode) != null;
						}
						if (valid2)
						{
							string id = crate2.Barcode.ID;
							palletPage?.CreateFunction(crate2.name, Color.green, delegate
							{
								FunctionPress?.Invoke(id);
							});
							count++;
						}
					}
				}
			}
			if (SearchMethodNow == SearchMethod.PalletAuthor && !string.IsNullOrEmpty(pallet.Author) && pallet.Author.ToLower().Contains(searchLower))
			{
				int validCount2 = 0;
				Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator4 = pallet.Crates.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					Crate crate3 = enumerator4.Current;
					bool valid3 = false;
					if (SpawnableType == SearchMethodType.Avatar)
					{
						valid3 = CrateFilterer.GetCrate<AvatarCrate>(crate3.Barcode) != null;
					}
					if (SpawnableType == SearchMethodType.Level)
					{
						valid3 = CrateFilterer.GetCrate<LevelCrate>(crate3.Barcode) != null;
					}
					if (SpawnableType == SearchMethodType.Spawnable)
					{
						valid3 = CrateFilterer.GetCrate<SpawnableCrate>(crate3.Barcode) != null;
					}
					if (valid3)
					{
						validCount2++;
					}
				}
				if (validCount2 > 0)
				{
					BoneLib.BoneMenu.Page palletPage2 = results?.CreatePage($"+ {pallet.name} ({pallet.Author})", Color.green);
					Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator5 = pallet.Crates.GetEnumerator();
					while (enumerator5.MoveNext())
					{
						Crate crate4 = enumerator5.Current;
						bool valid4 = false;
						if (SpawnableType == SearchMethodType.Avatar)
						{
							valid4 = CrateFilterer.GetCrate<AvatarCrate>(crate4.Barcode) != null;
						}
						if (SpawnableType == SearchMethodType.Level)
						{
							valid4 = CrateFilterer.GetCrate<LevelCrate>(crate4.Barcode) != null;
						}
						if (SpawnableType == SearchMethodType.Spawnable)
						{
							valid4 = CrateFilterer.GetCrate<SpawnableCrate>(crate4.Barcode) != null;
						}
						if (valid4)
						{
							string id2 = crate4.Barcode.ID;
							palletPage2?.CreateFunction(crate4.name, Color.green, delegate
							{
								FunctionPress?.Invoke(id2);
							});
							count++;
						}
					}
				}
			}
			Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator6 = pallet.Crates.GetEnumerator();
			while (enumerator6.MoveNext())
			{
				Crate crate5 = enumerator6.Current;
				bool valid5 = false;
				if (SpawnableType == SearchMethodType.Avatar)
				{
					valid5 = CrateFilterer.GetCrate<AvatarCrate>(crate5.Barcode) != null;
				}
				if (SpawnableType == SearchMethodType.Level)
				{
					valid5 = CrateFilterer.GetCrate<LevelCrate>(crate5.Barcode) != null;
				}
				if (SpawnableType == SearchMethodType.Spawnable)
				{
					valid5 = CrateFilterer.GetCrate<SpawnableCrate>(crate5.Barcode) != null;
				}
				if (!valid5)
				{
					continue;
				}
				string crateName = crate5.name.ToLower();
				string barcodeLower = crate5.Barcode.ID.ToLower();
				string id3 = crate5.Barcode.ID;
				if (SearchMethodNow == SearchMethod.CrateNames && (crateName.Contains(searchLower) || crateName.StartsWith(searchLower)))
				{
					results?.CreateFunction(crate5.name, Color.green, delegate
					{
						FunctionPress?.Invoke(id3);
					});
					count++;
				}
				if (SearchMethodNow == SearchMethod.BarcodeIDNames && (barcodeLower.Contains(searchLower) || barcodeLower.StartsWith(searchLower)))
				{
					results?.CreateFunction(crate5.name, Color.green, delegate
					{
						FunctionPress?.Invoke(id3);
					});
					count++;
				}
				processed++;
				if (processed >= batchSize)
				{
					processed = 0;
					yield return null;
				}
			}
		}
		MelonLogger.Msg($"[Searcher] Completed Found {count} Results.");
		NotificationNow("Nemesis Anti-Cheat", $"[Searcher] Completed Found {count} Results.", NotificationType.SUCCESS, 3f);
		SearchHistoryEntry searchhistorynow = new SearchHistoryEntry(search, SearchMethodNow, SpawnableType);
		if (!SearchHistorynow.Contains(searchhistorynow))
		{
			SearchHistorynow.Add(new SearchHistoryEntry(search, SearchMethodNow, SpawnableType));
		}
		isSearching = false;
	}

	internal static BaseController RightController()
	{
		RigManager rigManager = Player.RigManager;
		if (rigManager == null)
		{
			return null;
		}
		ControllerRig controllerRig = rigManager.ControllerRig;
		if (controllerRig == null)
		{
			return null;
		}
		return controllerRig.rightController;
	}

	internal static BaseController LeftController()
	{
		RigManager rigManager = Player.RigManager;
		if (rigManager == null)
		{
			return null;
		}
		ControllerRig controllerRig = rigManager.ControllerRig;
		if (controllerRig == null)
		{
			return null;
		}
		return controllerRig.leftController;
	}

	internal static bool FullLoadedNow()
	{
		if (!NetworkInfo.HasServer)
		{
			return false;
		}
		if (!NetworkInfo.HasLayer)
		{
			return false;
		}
		if (!FusionSceneManager.HasTargetLoaded() && !FusionSceneManager.IsDelayedLoading())
		{
			return false;
		}
		if (!RigData.HasPlayer)
		{
			return false;
		}
		return true;
	}

	internal static string GetRandomByType(RandomizerType type)
	{
		System.Random random = new System.Random();
		if (1 == 0)
		{
		}
		string result = type switch
		{
			RandomizerType.AllAvatars => (AvatarsStored.Count > 0) ? AvatarsStored.ElementAt(random.Next(AvatarsStored.Count)).Barcode.ID : null, 
			RandomizerType.AllSpawnables => (SpawnablesStored.Count > 0) ? SpawnablesStored.OrderBy((string x) => x).ElementAt(random.Next(SpawnablesStored.Count)) : null, 
			RandomizerType.NoTagsSpawnables => (NoTagSpawnables.Count > 0) ? NoTagSpawnables.OrderBy((string x) => x).ElementAt(random.Next(NoTagSpawnables.Count)) : null, 
			RandomizerType.AllNPCs => (AllNPCStored.Count > 0) ? AllNPCStored.OrderBy((string x) => x).ElementAt(random.Next(AllNPCStored.Count)) : null, 
			RandomizerType.AllWeapons => (AllWeaponsStored.Count > 0) ? AllWeaponsStored.OrderBy((string x) => x).ElementAt(random.Next(AllWeaponsStored.Count)) : null, 
			RandomizerType.AllMelees => (MeleeStored.Count > 0) ? MeleeStored.OrderBy((string x) => x).ElementAt(random.Next(MeleeStored.Count)) : null, 
			RandomizerType.AllRifle => (GunRiflesStored.Count > 0) ? GunRiflesStored.OrderBy((string x) => x).ElementAt(random.Next(GunRiflesStored.Count)) : null, 
			RandomizerType.AllSMG => (GunSMGStored.Count > 0) ? GunSMGStored.OrderBy((string x) => x).ElementAt(random.Next(GunSMGStored.Count)) : null, 
			RandomizerType.AllRanged => (GunRangedStored.Count > 0) ? GunRangedStored.OrderBy((string x) => x).ElementAt(random.Next(GunRangedStored.Count)) : null, 
			RandomizerType.AllPistol => (GunPistolStored.Count > 0) ? GunPistolStored.OrderBy((string x) => x).ElementAt(random.Next(GunPistolStored.Count)) : null, 
			RandomizerType.AllShotgun => (GunShotgunStored.Count > 0) ? GunShotgunStored.OrderBy((string x) => x).ElementAt(random.Next(GunShotgunStored.Count)) : null, 
			RandomizerType.AllSniper => (GunSniperStored.Count > 0) ? GunSniperStored.OrderBy((string x) => x).ElementAt(random.Next(GunSniperStored.Count)) : null, 
			RandomizerType.AllBlunt => (MeleeStoredBlunt.Count > 0) ? MeleeStoredBlunt.OrderBy((string x) => x).ElementAt(random.Next(MeleeStoredBlunt.Count)) : null, 
			RandomizerType.AllBlade => (MeleeStoredBlade.Count > 0) ? MeleeStoredBlade.OrderBy((string x) => x).ElementAt(random.Next(MeleeStoredBlade.Count)) : null, 
			RandomizerType.AllKnife => (MeleeStoredKnife.Count > 0) ? MeleeStoredKnife.OrderBy((string x) => x).ElementAt(random.Next(MeleeStoredKnife.Count)) : null, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	internal static Poolee SpawnIt(string BARCODE, Vector3 Position, Quaternion rotation, bool localonly = false)
	{
		if (localonly || !NetworkInfo.HasServer)
		{
			Spawnable spawnable = new Spawnable
			{
				crateRef = new SpawnableCrateReference(BARCODE)
			};
			Poolee spawnyc = null;
			LocalAssetSpawner.Register(spawnable);
			LocalAssetSpawner.Spawn(spawnable, Position, rotation, delegate(Poolee callbackpoole)
			{
				spawnyc = callbackpoole;
			});
			return spawnyc;
		}
		FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
		if (FusionPermissions.HasSufficientPermissions(level, LobbyInfoManager.LobbyInfo.DevTools))
		{
			Spawnable spawnable2 = new Spawnable
			{
				crateRef = new SpawnableCrateReference(BARCODE)
			};
			NetworkAssetSpawner.SpawnRequestInfo info = new NetworkAssetSpawner.SpawnRequestInfo
			{
				Spawnable = spawnable2,
				Position = Position,
				Rotation = rotation,
				SpawnSource = EntitySource.Player,
				SpawnEffect = true,
				SpawnCallback = delegate
				{
				}
			};
			NetworkAssetSpawner.Spawn(info);
			return null;
		}
		NotificationNow("Nemesis Anti-Cheat", "Invalid Permissions!", NotificationType.ERROR, 3f);
		return null;
	}

	internal static string StripColorTags(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return Regex.Replace(input, "<.*?>", string.Empty, RegexOptions.Singleline);
	}

	internal static void FindPlayersLobbyFromPlayerSteamID(ulong steamId, Action<bool, PlayerInfo> onResult)
	{
		IMatchmaker matchmaker = NetworkLayerManager.Layer?.Matchmaker;
		if (matchmaker == null)
		{
			onResult?.Invoke(arg1: false, null);
			return;
		}
		matchmaker.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo info)
		{
			IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
			for (int i = 0; i < lobbies.Length; i++)
			{
				IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
				PlayerInfo[] array = lobbyInfo.Metadata.LobbyInfo?.PlayerList?.Players;
				if (array != null)
				{
					PlayerInfo[] array2 = array;
					foreach (PlayerInfo playerInfo in array2)
					{
						if (playerInfo != null && playerInfo.PlatformID == steamId)
						{
							onResult?.Invoke(arg1: true, playerInfo);
							return;
						}
					}
				}
			}
			onResult?.Invoke(arg1: false, new PlayerInfo
			{
				AvatarModID = -1,
				Description = "Player Not On Fusion Publically!",
				AvatarTitle = "Player Not On Fusion Publically!",
				Nickname = "Player Not On Fusion Publically!",
				Username = "Player Not On Fusion Publically!"
			});
		});
	}

	internal static void FindPlayersLobbyFromPlayerName(string searchName, System.Collections.Generic.Dictionary<LobbyInfo, System.Collections.Generic.List<PlayerInfo>> results, Action<bool> onResult)
	{
		IMatchmaker matchmaker = NetworkLayerManager.Layer?.Matchmaker;
		if (matchmaker == null || string.IsNullOrWhiteSpace(searchName))
		{
			onResult?.Invoke(obj: false);
			return;
		}
		string search = searchName.Trim().ToLowerInvariant();
		results.Clear();
		matchmaker.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo lobbiesInfo)
		{
			IMatchmaker.LobbyInfo[] lobbies = lobbiesInfo.Lobbies;
			for (int i = 0; i < lobbies.Length; i++)
			{
				IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
				LobbyInfo lobbyInfo2 = lobbyInfo.Metadata.LobbyInfo;
				PlayerInfo[] array = lobbyInfo2?.PlayerList?.Players;
				if (array != null)
				{
					PlayerInfo[] array2 = array;
					foreach (PlayerInfo playerInfo in array2)
					{
						if (playerInfo != null)
						{
							bool flag = false;
							if (!string.IsNullOrWhiteSpace(playerInfo.Nickname))
							{
								string text = playerInfo.Nickname.Trim().ToLowerInvariant();
								if (text.Contains(search))
								{
									flag = true;
								}
							}
							if (!flag && !string.IsNullOrWhiteSpace(playerInfo.Username))
							{
								string text2 = playerInfo.Username.Trim().ToLowerInvariant();
								if (text2.Contains(search))
								{
									flag = true;
								}
							}
							if (flag)
							{
								if (!results.TryGetValue(lobbyInfo2, out var value))
								{
									value = new System.Collections.Generic.List<PlayerInfo>();
									results[lobbyInfo2] = value;
								}
								value.Add(playerInfo);
							}
						}
					}
				}
			}
			onResult?.Invoke(results.Count > 0);
		});
	}

	internal static bool IsAvatarCrateExist(string barcode)
	{
		AvatarCrateReference avatarCrateReference = new AvatarCrateReference(barcode);
		if (avatarCrateReference.Crate != null)
		{
			Pallet pallet = avatarCrateReference.Crate.Pallet;
			return pallet != null && pallet.Barcode != null;
		}
		return false;
	}

	internal static bool IsLevelCrateExist(string barcode)
	{
		LevelCrateReference levelCrateReference = new LevelCrateReference(barcode);
		if (levelCrateReference.Crate != null)
		{
			Pallet pallet = levelCrateReference.Crate.Pallet;
			return pallet != null && pallet.Barcode != null;
		}
		return false;
	}

	internal static bool IsSpawnableCrateExist(string barcode)
	{
		SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcode);
		if (spawnableCrateReference.Crate != null)
		{
			Pallet pallet = spawnableCrateReference.Crate.Pallet;
			return pallet != null && pallet.Barcode != null;
		}
		return false;
	}

	internal static bool IsBarcodeInGame(string barcode)
	{
		SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcode);
		if (spawnableCrateReference.Crate != null)
		{
			Pallet pallet = spawnableCrateReference.Crate.Pallet;
			return pallet != null && pallet.Barcode != null;
		}
		LevelCrateReference levelCrateReference = new LevelCrateReference(barcode);
		if (levelCrateReference.Crate != null)
		{
			Pallet pallet2 = levelCrateReference.Crate.Pallet;
			return pallet2 != null && pallet2.Barcode != null;
		}
		AvatarCrateReference avatarCrateReference = new AvatarCrateReference(barcode);
		if (avatarCrateReference.Crate != null)
		{
			Pallet pallet3 = avatarCrateReference.Crate.Pallet;
			return pallet3 != null && pallet3.Barcode != null;
		}
		return false;
	}

	internal static async Task<(NetworkEntity NetworkEntityReturn, Spawnable SpawnableReturn, GameObject GameObjectReturn, InteractableHost InteractableHostReturn)> SpawnersSpawner(string barcodenow, Vector3 location, Quaternion rotation, bool effect = true, bool manuallyInvisToYouOnly = false, bool copyLocationToClipboard = false)
	{
		if (!IsBarcodeInGame(barcodenow))
		{
			return (NetworkEntityReturn: null, SpawnableReturn: null, GameObjectReturn: null, InteractableHostReturn: null);
		}
		TaskCompletionSource<(NetworkEntity, Spawnable, GameObject, InteractableHost)> tcs = new TaskCompletionSource<(NetworkEntity, Spawnable, GameObject, InteractableHost)>();
		Spawnable spawnable = new Spawnable
		{
			crateRef = new SpawnableCrateReference(barcodenow)
		};
		if (NetworkInfo.HasServer)
		{
			NetworkAssetSpawner.SpawnRequestInfo request = new NetworkAssetSpawner.SpawnRequestInfo
			{
				Spawnable = spawnable,
				Position = location,
				Rotation = Player.Head.rotation,
				SpawnCallback = delegate(NetworkAssetSpawner.SpawnCallbackInfo info)
				{
					if (manuallyInvisToYouOnly)
					{
						info.Entity.ND_GetMarrowEntity()?.gameObject.DestroyNow();
					}
					InteractableHost item = info.Entity.ND_GetMarrowEntity()?.GetComponent<InteractableHost>();
					tcs.TrySetResult((info.Entity, spawnable, info.Spawned, item));
				},
				SpawnEffect = effect,
				SpawnSource = EntitySource.Player
			};
			NetworkAssetSpawner.Spawn(request);
			if (copyLocationToClipboard)
			{
				GUIUtility.systemCopyBuffer = $"Title: {StripColorTags(request.Spawnable.crateRef.Scannable.Title)}\nBarcode: {request.Spawnable.crateRef.Barcode.ID}\nLocation: {request.Position}\nRotation: {request.Rotation}";
			}
		}
		else
		{
			LocalAssetSpawner.Register(spawnable);
			LocalAssetSpawner.Spawn(spawnable, location, rotation, delegate(Poolee callback)
			{
				InteractableHost component = callback.gameObject.GetComponent<MarrowEntity>().GetComponent<InteractableHost>();
				tcs.TrySetResult((null, spawnable, callback.gameObject, component));
			});
			if (copyLocationToClipboard)
			{
				GUIUtility.systemCopyBuffer = $"Title: {StripColorTags(spawnable.crateRef.Scannable.Title)}\nBarcode: {spawnable.crateRef.Barcode.ID}\nLocation: {location}\nRotation: {rotation}";
			}
		}
		return await tcs.Task;
	}

	internal static IEnumerator LoadAssetsEnum(bool randomizerslzonly, bool enableLogging = true)
	{
		if (AvatarsStored.Count > 0)
		{
			AvatarsStored.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared AvatarsStored");
			}
			yield return null;
		}
		if (SpawnablesStored.Count > 0)
		{
			SpawnablesStored.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared SpawnablesStored");
			}
			yield return null;
		}
		if (NoTagSpawnables.Count > 0)
		{
			NoTagSpawnables.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared NoTagSpawnables");
			}
			yield return null;
		}
		if (AllNPCStored.Count > 0)
		{
			AllNPCStored.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared AllNPCStored");
			}
			yield return null;
		}
		if (AllWeaponsStored.Count > 0)
		{
			AllWeaponsStored.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared AllWeaponsStored");
			}
			yield return null;
		}
		if (MeleeStored.Count > 0)
		{
			MeleeStored.Clear();
			if (enableLogging)
			{
				MelonLogger.Warning("Cleared MeleeStored");
			}
			yield return null;
		}
		if (GunRiflesStored.Count > 0)
		{
			GunRiflesStored.Clear();
			yield return null;
		}
		if (GunSMGStored.Count > 0)
		{
			GunSMGStored.Clear();
			yield return null;
		}
		if (GunRangedStored.Count > 0)
		{
			GunRangedStored.Clear();
			yield return null;
		}
		if (GunPistolStored.Count > 0)
		{
			GunPistolStored.Clear();
			yield return null;
		}
		if (GunShotgunStored.Count > 0)
		{
			GunShotgunStored.Clear();
			yield return null;
		}
		if (GunSniperStored.Count > 0)
		{
			GunSniperStored.Clear();
			yield return null;
		}
		if (MeleeStoredBlunt.Count > 0)
		{
			MeleeStoredBlunt.Clear();
			yield return null;
		}
		if (MeleeStoredBlade.Count > 0)
		{
			MeleeStoredBlade.Clear();
			yield return null;
		}
		if (MeleeStoredKnife.Count > 0)
		{
			MeleeStoredKnife.Clear();
			yield return null;
		}
		Il2CppSystem.Collections.Generic.List<Pallet> pallets = AssetWarehouse.Instance.GetPallets();
		int crateCounter = 0;
		Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator = pallets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Pallet pallet = enumerator.Current;
			if (randomizerslzonly && pallet.Author != "SLZ")
			{
				continue;
			}
			Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator2 = pallet.Crates.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Crate crate = enumerator2.Current;
				string id = crate.Barcode.ID;
				Il2CppSystem.Collections.Generic.List<string> tags = crate.Tags;
				if (IsLevelCrateExist(id))
				{
					LevelStored.Add(id);
				}
				if (IsSpawnableCrateExist(id))
				{
					SpawnablesStored.Add(id);
				}
				if (IsAvatarCrateExist(id))
				{
					AvatarsStored.Add(new AvatarCrateReference(id));
				}
				int tagCount = tags.Count;
				if (tagCount == 0)
				{
					NoTagSpawnables.Add(id);
				}
				for (int i = 0; i < tagCount; i++)
				{
					switch (tags[i])
					{
					case "Weapon":
						AllWeaponsStored.Add(id);
						break;
					case "NPC":
						AllNPCStored.Add(id);
						break;
					case "Rifle":
						GunRiflesStored.Add(id);
						break;
					case "SMG":
						GunSMGStored.Add(id);
						break;
					case "Ranged":
						GunRangedStored.Add(id);
						break;
					case "Pistol":
						GunPistolStored.Add(id);
						break;
					case "Shotgun":
						GunShotgunStored.Add(id);
						break;
					case "Sniper":
						GunSniperStored.Add(id);
						break;
					case "Melee":
						MeleeStored.Add(id);
						break;
					case "Blunt":
						MeleeStoredBlunt.Add(id);
						break;
					case "Blade":
						MeleeStoredBlade.Add(id);
						break;
					case "Knife":
						MeleeStoredKnife.Add(id);
						break;
					}
				}
				crateCounter++;
				if (crateCounter % 50 == 0)
				{
					yield return null;
				}
			}
		}
		if (enableLogging)
		{
			MelonLogger.Warning("Nemesis Anti-Cheat Loaded Assets");
			MelonLogger.Warning("------------------------------");
			MelonLogger.Warning($"All Levels Loaded : [{LevelStored.Count}]");
			MelonLogger.Warning($"All Avatars Loaded : [{AvatarsStored.Count}]");
			MelonLogger.Warning($"All Spawnables Loaded : [{SpawnablesStored.Count}]");
			MelonLogger.Warning($"All No Tags Spawnables Loaded : [{NoTagSpawnables.Count}]");
			MelonLogger.Warning($"All NPCs Loaded : [{AllNPCStored.Count}]");
			MelonLogger.Warning($"All Weapons Loaded : [{AllWeaponsStored.Count}]");
			MelonLogger.Warning($"All Melees Loaded : [{MeleeStored.Count}]");
			MelonLogger.Warning($"Weapons [Rifle] Loaded : [{GunRiflesStored.Count}]");
			MelonLogger.Warning($"Weapons [SMG] Loaded : [{GunSMGStored.Count}]");
			MelonLogger.Warning($"Weapons [Ranged] Loaded : [{GunRangedStored.Count}]");
			MelonLogger.Warning($"Weapons [Pistol] Loaded : [{GunPistolStored.Count}]");
			MelonLogger.Warning($"Weapons [Shotgun] Loaded : [{GunShotgunStored.Count}]");
			MelonLogger.Warning($"Weapons [Sniper] Loaded : [{GunSniperStored.Count}]");
			MelonLogger.Warning($"Melees [Blunt] Loaded : [{MeleeStoredBlunt.Count}]");
			MelonLogger.Warning($"Melees [Blade] Loaded : [{MeleeStoredBlade.Count}]");
			MelonLogger.Warning($"Melees [Knife] Loaded : [{MeleeStoredKnife.Count}]");
			MelonLogger.Warning("------------------------------");
		}
	}

	internal static void OpenPageNow(string linknow)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = linknow,
			UseShellExecute = true
		});
	}

	internal static System.Collections.Generic.IEnumerable<string> GetLoggedSettingsLines()
	{
		foreach (var item in PageEx.boolslogged)
		{
			yield return item.entry;
		}
		foreach (var item2 in PageEx.floatslogged)
		{
			yield return item2.entry;
		}
		foreach (var item3 in PageEx.intslogged)
		{
			yield return item3.entry;
		}
		foreach (var item4 in PageEx.enumvaluelogged)
		{
			yield return item4.entry;
		}
		foreach (var item5 in PageEx.stringslogged)
		{
			yield return item5.entry;
		}
	}

	internal static async Task SaveSettingsAsync(bool notify = true)
	{
		try
		{
			await File.WriteAllLinesAsync(contents: GetLoggedSettingsLines().ToArray(), path: ProtectorSettings);
			if (notify)
			{
				MelonLogger.Warning("Saved Settings!");
				NotificationNow("Nemesis Anti-Cheat", "Manually Saved Settings!", NotificationType.SUCCESS);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MelonLogger.Error($"Failed to save settings: {ex2}");
		}
	}

	internal static async void ManuallySave(bool notify = true)
	{
		await SaveSettingsAsync(notify);
	}

	internal static void DespawnNow(NetworkEntity entity)
	{
		if (AreYouOWNER())
		{
			NetworkAssetSpawner.Despawn(new NetworkAssetSpawner.DespawnRequestInfo
			{
				EntityID = entity.ID,
				DespawnEffect = false
			});
		}
	}

	internal static void DespawnAll(DespawnerAll filter, bool localOnly = false)
	{
		if (!localOnly)
		{
			if (!AreYouOWNER())
			{
				NotificationNow("Nemesis Anti-Cheat", "Invalid Permissions! [Owner Required!]", NotificationType.ERROR, 3f);
				return;
			}
			if (dropallbefore)
			{
				foreach (NetworkPlayer item in NetworkPlayers(excludeMe: true))
				{
					InventorySlotReceiver[] array = item?.RigRefs?.RigSlots;
					if (array != null)
					{
						InventorySlotReceiver[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i]?.DropWeapon();
						}
					}
				}
			}
		}
		foreach (NetworkEntity item2 in NetworkEntities())
		{
			if (PassesFilter(item2, filter))
			{
				if (localOnly)
				{
					item2.ND_GetMarrowEntity()?.gameObject.DestroyNow();
				}
				else
				{
					DespawnNow(item2);
				}
			}
		}
	}

	private static bool PassesFilter(NetworkEntity entity, DespawnerAll filter)
	{
		switch (filter)
		{
		case DespawnerAll.NoFilter:
			return true;
		case DespawnerAll.Guns:
			return entity.ND_IsGun();
		case DespawnerAll.Melees:
			return entity.ND_IsMelee();
		case DespawnerAll.Npcs:
			return entity.ND_IsNPC();
		case DespawnerAll.EverythingButGuns:
			return !entity.ND_IsGun();
		case DespawnerAll.EverythingButMelees:
			return !entity.ND_IsMelee();
		case DespawnerAll.EverythingButNpcs:
			return !entity.ND_IsNPC();
		case DespawnerAll.NetworkProps:
		{
			NetworkProp extender = entity.GetExtender<NetworkProp>();
			PooleeExtender extender2 = entity.GetExtender<PooleeExtender>();
			return extender != null && extender2 != null;
		}
		case DespawnerAll.AllNotButtonsLeverCircuits:
		{
			MarrowEntity marrowEntity = entity.ND_GetMarrowEntity();
			if (marrowEntity == null)
			{
				return false;
			}
			GameObject gameObject = marrowEntity.gameObject;
			bool flag = (bool)gameObject.GetComponentInChildren<ButtonNode>(includeInactive: true) || (bool)gameObject.GetComponentInParent<ButtonNode>();
			bool flag2 = (bool)gameObject.GetComponentInChildren<HingeController>(includeInactive: true) || (bool)gameObject.GetComponentInParent<HingeController>();
			bool flag3 = (bool)gameObject.GetComponentInChildren<CircuitSocket>(includeInactive: true) || (bool)gameObject.GetComponentInParent<CircuitSocket>();
			return !(flag || flag2 || flag3);
		}
		default:
			return true;
		}
	}

	internal static IEnumerator ModioInfo(int modIOID, Action<ModCallbackInfo> onFinished)
	{
		if (_isLookingUpMod)
		{
			NotificationNow("Nemesis Anti-Cheat", "Already looking up a mod please WAIT!", NotificationType.WARNING);
			yield break;
		}
		_isLookingUpMod = true;
		NotificationNow("Nemesis Anti-Cheat", $"Reading mod info for ID {modIOID}... please wait.", NotificationType.INFORMATION);
		ModCallbackInfo infoNow = default(ModCallbackInfo);
		bool finished = false;
		ModTransaction transaction = new ModTransaction
		{
			ModFile = new ModIOFile(modIOID),
			Callback = Callback
		};
		ModIOManager.GetMod(transaction.ModFile.ModID, OnRequestedMod);
		while (!finished)
		{
			yield return null;
		}
		_isLookingUpMod = false;
		onFinished?.Invoke(infoNow);
		static void Callback(DownloadCallbackInfo info)
		{
			if (info.Result != ModResult.SUCCEEDED)
			{
				NotificationNow("Nemesis Anti-Cheat", "The content failed to install! Make sure you are logged into mod.io in VoidG114 or BONELAB Hub!", NotificationType.WARNING);
			}
		}
		void OnRequestedMod(ModCallbackInfo info)
		{
			infoNow = info;
			finished = true;
			if (info.Result == ModResult.SUCCEEDED)
			{
				NotificationNow("Nemesis Anti-Cheat", "Mod found: " + info.Data.NameID, NotificationType.INFORMATION);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Failed to retrieve mod info.", NotificationType.WARNING);
			}
		}
	}

	internal static void DownloadModIOMod(int modIOID, bool noti = true)
	{
		ModTransaction modTransaction = new ModTransaction();
		modTransaction.ModFile = new ModIOFile(modIOID);
		modTransaction.Callback = Callback;
		ModTransaction transaction = modTransaction;
		ModIODownloader.EnqueueDownload(transaction);
		if (noti)
		{
			NotificationNow("Nemesis Anti-Cheat", "Wait Until You See Installed Notification Then Press Whatever You Pressed AGAIN!", NotificationType.WARNING, 6f);
		}
		static void Callback(DownloadCallbackInfo info)
		{
			if (info.Result != ModResult.SUCCEEDED)
			{
				NotificationNow("Nemesis Anti-Cheat", "The Content failed to install! Make sure you are logged into mod.io in VoidG114 or BONELAB Hub!", NotificationType.WARNING);
			}
		}
	}

	internal static void AllNemesisAntiCheatLobbies()
	{
		fppubs.RemoveAll();
		if (!NetworkInfo.HasLayer)
		{
			return;
		}
		NetworkLayerManager.Layer.Matchmaker?.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo lobbyQueryResult)
		{
			IMatchmaker.LobbyInfo[] lobbies = lobbyQueryResult.Lobbies;
			for (int i = 0; i < lobbies.Length; i++)
			{
				IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
				LobbyInfo fusionLobbyInfo = lobbyInfo.Metadata.LobbyInfo;
				if (fusionLobbyInfo.LobbyCode.StartsWith("FP-") && fusionLobbyInfo.Privacy == ServerPrivacy.PUBLIC && fusionLobbyInfo.PlayerCount < fusionLobbyInfo.MaxPlayers)
				{
					string value = (string.IsNullOrEmpty(fusionLobbyInfo.LobbyName) ? (fusionLobbyInfo.LobbyHostName + "'s Server") : ("[" + fusionLobbyInfo.LobbyName + "] "));
					string name = $"{value}[{fusionLobbyInfo.LobbyHostName}] Join | {fusionLobbyInfo.PlayerCount}/{fusionLobbyInfo.MaxPlayers} | {fusionLobbyInfo.LevelTitle}";
					BoneLib.BoneMenu.Page page = fppubs.CreatePage($"+ {value}[{fusionLobbyInfo.LobbyHostName}]", Color.green);
					page.CreateFunction(name, Color.yellow, delegate
					{
						if (CrateFilterer.HasCrate<LevelCrate>(new Barcode(fusionLobbyInfo.LevelBarcode)))
						{
							NetworkHelper.JoinServerByCode(fusionLobbyInfo.LobbyCode);
						}
						else
						{
							DownloadModIOMod(fusionLobbyInfo.LevelModID, noti: false);
							NotificationNow("Nemesis Anti-Cheat", "Wait Until You See Installed Notification Then Click Join Again!", NotificationType.WARNING, 5f);
						}
					});
					BoneLib.BoneMenu.Page page2 = page.CreatePage("+ Players in Lobby", Color.green);
					PlayerInfo[] players = fusionLobbyInfo.PlayerList.Players;
					foreach (PlayerInfo fusionPlayer in players)
					{
						page2.CreateFunction($"[{CleanedNAME(fusionPlayer.Nickname, fusionPlayer.Username)}] [{fusionPlayer.PlatformID}]", Color.green, delegate
						{
							GUIUtility.systemCopyBuffer = fusionPlayer.PlatformID.ToString();
							NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID To Clipboard", NotificationType.SUCCESS, 3f);
						});
					}
				}
			}
		});
	}

	internal static void FriendLobbies()
	{
		OnlineFriends.RemoveAll();
		if (!NetworkInfo.HasLayer)
		{
			return;
		}
		int friends = 0;
		NetworkLayerManager.Layer.Matchmaker?.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo info)
		{
			IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
			for (int i = 0; i < lobbies.Length; i++)
			{
				IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
				LobbyInfo lobbyInfo2 = lobbyInfo.Metadata.LobbyInfo;
				if (lobbyInfo2?.PlayerList?.Players != null)
				{
					PlayerInfo[] players = lobbyInfo2.PlayerList.Players;
					foreach (PlayerInfo playerInfo in players)
					{
						if (NetworkHelper.IsFriend(playerInfo.PlatformID))
						{
							friends++;
							ServerPrivacy privacy = lobbyInfo2.Privacy;
							bool flag = (uint)privacy <= 1u;
							if (flag && lobbyInfo2.PlayerCount < lobbyInfo2.MaxPlayers)
							{
								string name = $"[{lobbyInfo2.LobbyName}] [{lobbyInfo2.LobbyHostName}] Join | {lobbyInfo2.PlayerCount} / {lobbyInfo2.MaxPlayers} | {lobbyInfo2.LevelTitle}";
								BoneLib.BoneMenu.Page page = OnlineFriends.CreatePage($" + [{playerInfo.Nickname}] [{playerInfo.Username}] [{playerInfo.PlatformID}]", Color.yellow);
								page?.CreateFunction(name, Color.white, delegate
								{
									if (CrateFilterer.HasCrate<LevelCrate>(new Barcode(lobbyInfo2.LevelBarcode)))
									{
										NetworkHelper.JoinServerByCode(lobbyInfo2.LobbyCode);
									}
									else
									{
										DownloadModIOMod(lobbyInfo2.LevelModID, noti: false);
										NotificationNow("Nemesis Anti-Cheat", "Wait Untill You See Installed Notification Then Click Join Again!", NotificationType.WARNING, 5f);
									}
								});
								BoneLib.BoneMenu.Page page2 = page?.CreatePage("+ Players in Lobby", Color.yellow);
								PlayerInfo[] players2 = lobbyInfo.Metadata.LobbyInfo.PlayerList.Players;
								foreach (PlayerInfo playernow in players2)
								{
									page2?.CreateFunction($"[{playernow.Nickname}] [{playernow.Username}] [{playernow.PlatformID}]", Color.white, delegate
									{
										GUIUtility.systemCopyBuffer = playernow.PlatformID.ToString();
										NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID To Clipboard", NotificationType.SUCCESS, 3f);
									});
								}
							}
						}
					}
				}
			}
		});
		if (friends != 0)
		{
			NotificationNowAlways("Nemesis Anti-Cheat", "Friends Online : " + friends, NotificationType.SUCCESS, 3.5f);
		}
	}

	internal static void AllLobbies()
	{
		pubs.RemoveAll();
		if (!NetworkInfo.HasLayer)
		{
			return;
		}
		NetworkLayerManager.Layer.Matchmaker?.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo info)
		{
			IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
			for (int i = 0; i < lobbies.Length; i++)
			{
				IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
				LobbyInfo lobbyInfo2 = lobbyInfo.Metadata.LobbyInfo;
				if (lobbyInfo2.Privacy == ServerPrivacy.PUBLIC && lobbyInfo2.PlayerCount < lobbyInfo2.MaxPlayers)
				{
					string value = (string.IsNullOrEmpty(lobbyInfo2.LobbyName) ? (lobbyInfo2.LobbyHostName + "'s Server") : ("[" + lobbyInfo2.LobbyName + "] "));
					string name = $"{value}[{lobbyInfo2.LobbyHostName}] Join | {lobbyInfo2.PlayerCount}/{lobbyInfo2.MaxPlayers} | {lobbyInfo2.LevelTitle}";
					BoneLib.BoneMenu.Page page = pubs.CreatePage($"+ {value}[{lobbyInfo2.LobbyHostName}]", Color.green);
					page.CreateFunction(name, Color.yellow, delegate
					{
						if (CrateFilterer.HasCrate<LevelCrate>(new Barcode(lobbyInfo2.LevelBarcode)))
						{
							NetworkHelper.JoinServerByCode(lobbyInfo2.LobbyCode);
						}
						else
						{
							DownloadModIOMod(lobbyInfo2.LevelModID, noti: false);
							NotificationNow("Nemesis Anti-Cheat", "Wait Untill You See Installed Notification Then Click Join Again!", NotificationType.WARNING, 5f);
						}
					});
					BoneLib.BoneMenu.Page page2 = page.CreatePage("+ Players in Lobby", Color.green);
					PlayerInfo[] players = lobbyInfo.Metadata.LobbyInfo.PlayerList.Players;
					foreach (PlayerInfo playernow in players)
					{
						page2.CreateFunction($"[{playernow.Nickname}] [{playernow.Username}] [{playernow.PlatformID}]", Color.green, delegate
						{
							GUIUtility.systemCopyBuffer = playernow.PlatformID.ToString();
							NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID To Clipboard", NotificationType.SUCCESS, 3f);
						});
					}
				}
			}
		});
	}

	internal static bool Iswithintwovalues(float valuetocheck, float min, float max)
	{
		if (valuetocheck >= min && valuetocheck <= max)
		{
			return true;
		}
		return false;
	}

	internal static void ClearConstraints(NetworkPlayer Netty)
	{
		if (!Netty.PlayerID.IsValid)
		{
			return;
		}
		try
		{
			foreach (ConstraintTracker componentsInChild in Netty.RigRefs.RigManager.physicsRig.GetComponentsInChildren<ConstraintTracker>())
			{
				componentsInChild.DeleteConstraint();
			}
		}
		catch
		{
		}
	}

	internal static void HolsterHiderAll(NetworkPlayer playerTodo, bool activeNow = false)
	{
		PhysicsRig physicsRig = ((playerTodo == null) ? Player.RigManager?.physicsRig : playerTodo.RigRefs?.RigManager?.physicsRig);
		if (!(physicsRig == null))
		{
			Toggle(physicsRig.m_spine?.transform, "SideRt/prop_handGunHolster/strap_geo");
			Toggle(physicsRig.m_spine?.transform, "SideRt/prop_handGunHolster/handgunHolster_geo");
			Toggle(physicsRig.m_spine?.transform, "SideLf/prop_handGunHolster/strap_geo");
			Toggle(physicsRig.m_spine?.transform, "SideLf/prop_handGunHolster/handgunHolster_geo");
			Toggle(physicsRig.m_pelvis?.transform, "BeltLf1/InventoryAmmoReceiver/Holder");
			Toggle(physicsRig.m_pelvis?.transform, "BeltRt1/InventoryAmmoReceiver/Holder");
			Toggle(physicsRig.m_pelvis?.transform, "BackCt/prop_pouch");
		}
		void Toggle(Transform root, string path)
		{
			if (!(root == null))
			{
				Transform transform = root.Find(path);
				if (!(transform == null))
				{
					transform.GetComponent<MeshRenderer>()?.gameObject.SetActive(activeNow);
				}
			}
		}
	}

	internal static void StoreInventoryItems()
	{
		if (!FullLoadedNow())
		{
			return;
		}
		weaponsInInventory.Clear();
		Inventory inventory = Player.RigManager?.inventory;
		if (inventory == null)
		{
			return;
		}
		System.Collections.Generic.IEnumerable<SlotContainer> enumerable = inventory.bodySlots.Concat(inventory.specialItems);
		InventorySlotReceiver inventorySlotReceiver = Player.RigManager.physicsRig.m_head.transform?.Find("HeadSlotContainer/WeaponReciever_01")?.GetComponent<InventorySlotReceiver>();
		if (inventorySlotReceiver != null)
		{
			string text = inventorySlotReceiver._slottedWeapon?.interactableHost?.marrowEntity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID;
			if (!string.IsNullOrEmpty(text))
			{
				weaponsInInventory[inventorySlotReceiver] = (text, "HeadSlot");
			}
		}
		foreach (SlotContainer item2 in enumerable)
		{
			InventorySlotReceiver inventorySlotReceiver2 = item2?.inventorySlotReceiver;
			if (!(inventorySlotReceiver2 == null))
			{
				string text2 = inventorySlotReceiver2._slottedWeapon?.interactableHost?.marrowEntity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID;
				if (!string.IsNullOrEmpty(text2))
				{
					string item = item2.name ?? "UnknownSlot";
					weaponsInInventory[inventorySlotReceiver2] = (text2, item);
				}
			}
		}
	}

	internal static void SpawnInventoryRefresh()
	{
		if (!infiniteinventory || weaponsInInventory == null || weaponsInInventory.Count == 0 || !AreYouOWNER())
		{
			return;
		}
		Inventory inventory = Player.RigManager?.inventory;
		if (inventory == null || !FullLoadedNow())
		{
			return;
		}
		foreach (System.Collections.Generic.KeyValuePair<InventorySlotReceiver, (string, string)> item3 in weaponsInInventory)
		{
			InventorySlotReceiver key = item3.Key;
			string item = item3.Value.Item1;
			string item2 = item3.Value.Item2;
			if (key == null)
			{
				continue;
			}
			if (!infiniteinvall)
			{
				if (BodySlot(SlotsNowReal) == item2 && key._slottedWeapon == null)
				{
					key.SpawnInSlotAsync(new Barcode(item));
				}
			}
			else if (key._slottedWeapon == null)
			{
				key.SpawnInSlotAsync(new Barcode(item));
			}
		}
	}

	internal static void ChangeBodyLogAvatarSlot(int slotindex, string avatarbarcode, bool notification = true)
	{
		if (IsBarcodeInGame(avatarbarcode))
		{
			if (DataManager.ActiveSave.PlayerSettings.FavoriteAvatars == null)
			{
				DataManager.ActiveSave.PlayerSettings.FavoriteAvatars = new Il2CppSystem.Collections.Generic.List<string>();
				for (int i = 0; i < 6; i++)
				{
					DataManager.ActiveSave.PlayerSettings.FavoriteAvatars.Add("EMPTY");
				}
			}
			DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[slotindex - 1] = avatarbarcode;
			if (notification)
			{
				NotificationNowAlways("Nemesis Anti-Cheat", $"Changed {slotindex} Slot And Saved!", NotificationType.SUCCESS);
			}
			DataManager.TrySaveActiveSave(SaveFlags.Progression);
			ND_BodyLog(Player.PhysicsRig).bodylogreturn.LoadFavoriteAvatars();
			ND_BodyLog(Player.PhysicsRig).bodylogreturn.BodyMallUpdate();
		}
		else
		{
			NotificationNowAlways("Nemesis Anti-Cheat", "This Does Not Exist In Your Game Install This Mod For It To Exist...", NotificationType.ERROR, 3f);
		}
	}

	internal static bool IfDontHaveInstallThenDo(string barcode, int modioID, bool notification = false)
	{
		if (!CrateFilterer.HasCrate<SpawnableCrate>(new Barcode(barcode)))
		{
			DownloadModIOMod(modioID, notification);
			return false;
		}
		return true;
	}

	internal static string CleanedNAME(NetworkPlayer playernow)
	{
		string text = StripColorTags(playernow?.ND_Nickname());
		string text2 = playernow?.ND_Username();
		return string.IsNullOrWhiteSpace(text) ? text2 : (text + " | " + text2);
	}

	internal static string CleanedNAME(string nicknamex, string usernamex)
	{
		string text = StripColorTags(nicknamex);
		return string.IsNullOrWhiteSpace(nicknamex) ? usernamex : (nicknamex + " | " + usernamex);
	}

	internal static IEnumerator KeepLoadOut()
	{
		yield return new WaitForSecondsRealtime(2.5f);
		FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var selfLevel, out var _);
		if (FusionPermissions.HasSufficientPermissions(selfLevel, LobbyInfoManager.LobbyInfo.DevTools))
		{
			InventoryPage page = new InventoryPage("Current", InventoryPage.CurrentPreset);
			page.LoadIntoPlayer(notificationnow: false);
			NotificationNow("Nemesis Anti-Cheat", "Loaded Inventory Into Player!", NotificationType.SUCCESS, 3.5f);
		}
		else
		{
			NotificationNow("Nemesis Anti-Cheat", "Invalid Permissions.", NotificationType.ERROR, 3.5f);
		}
	}

	internal static void SetBarCodeToSpawnGun(string barcode)
	{
		WhichHand[] array = new WhichHand[2]
		{
			WhichHand.Left,
			WhichHand.Right
		};
		foreach (WhichHand hand in array)
		{
			Hand hand2 = ND_YourGetHand(hand);
			if (hand2.ND_IsGrabbedSpawnGun())
			{
				SpawnGun spawnGun = hand2.ND_HandGrabbedSpawnGun();
				SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcode);
				spawnGun._selectedCrate = spawnableCrateReference.Crate;
				spawnGun.SetPreviewMesh();
				SpawnEffects.CallDespawnEffect(ND_YourGetHand(hand)?.ND_GetMarrowEntity());
				SpawnEffects.CallSpawnEffect(ND_YourGetHand(hand)?.ND_GetMarrowEntity());
			}
		}
	}

	internal static void SendMessageToHost(string messagenow)
	{
		if (!HideNemesisAntiCheat && !ND_YourNetworkPlayer().PlayerID.IsHost)
		{
			SendNotificationData data = SendNotificationData.Create(PlayerIDManager.LocalSmallID, CleanedNAME(ND_YourNetworkPlayer()) + "\n" + messagenow, "Nemesis Anti-Cheat", 3f);
			MessageRelay.RelayModule<SendNotificationMessage, SendNotificationData>(data, CommonMessageRoutes.ReliableToServer);
		}
	}

	internal static void SendPlayerMessage(NetworkPlayer PlayerTodo, string message, float messagetime, byte playerssmallid, string messageonrecv = "Message sent. If they have Nemesis Anti-Cheat, they’ll receive it.", Action OnAcceptNow = null)
	{
		if (HideNemesisAntiCheat)
		{
			NotificationNow("Nemesis Anti-Cheat", "Can't Use This While Hiding Nemesis Anti-Cheat.", NotificationType.ERROR, 3f);
		}
		else if (!(TimeReferences.TimeSinceStartup - SendNotificationMessage._timeOfRequest <= 5f))
		{
			byte b = PlayerTodo.ND_SmallID();
			NotificationNow("Nemesis Anti-Cheat", "Sending message to " + CleanedNAME(PlayerTodo) + "...", NotificationType.WARNING);
			SendNotificationData data = SendNotificationData.Create(PlayerIDManager.LocalSmallID, TextFilter.SanitizeName(message), string.IsNullOrWhiteSpace(ND_YourNetworkPlayer().ND_Nickname()) ? (ND_YourNetworkPlayer().ND_Username() + " Says") : (ND_YourNetworkPlayer().ND_Nickname() + " Says"), messagetime);
			MessageRelay.RelayModule<SendNotificationMessage, SendNotificationData>(data, new MessageRoute(playerssmallid, NetworkChannel.Reliable));
			NotificationNow("Nemesis Anti-Cheat", messageonrecv, NotificationType.SUCCESS, 5f, showtitle: true, savetomenu: true, OnAcceptNow);
		}
	}

	internal static void PlayersListNow()
	{
		playermessages.RemoveAll();
		foreach (NetworkPlayer player in NetworkPlayers())
		{
			string text = StripColorTags(CleanedNAME(player)) ?? "Unknown Player";
			bool isHost = player.PlayerID.IsHost;
			string value = (isHost ? "[HOST] " : "");
			Color color = (isHost ? Color.cyan : Color.yellow);
			player.PlayerID.TryGetPermissionLevel(out var level);
			if (1 == 0)
			{
			}
			string text2 = level switch
			{
				PermissionLevel.OWNER => "[OWN]", 
				PermissionLevel.GUEST => "[GST]", 
				PermissionLevel.DEFAULT => "[DEF]", 
				PermissionLevel.OPERATOR => "[OP]", 
				_ => "", 
			};
			if (1 == 0)
			{
			}
			string value2 = text2;
			BoneLib.BoneMenu.Page page = playermessages.CreatePage($"+ {value}{CleanedNAME(player)} {value2}", color);
			page.CreateString("Send Code Mod Bas64 Raw Link", Color.yellow, "Link Here...", delegate(string stringy)
			{
				if (!(TimeReferences.TimeSinceStartup - SendBase64FileMessage._timeOfRequest <= 3f))
				{
					SendBase64FileData data = SendBase64FileData.Create(PlayerIDManager.LocalSmallID, stringy, SendBase64FileMessage.codemodname);
					MessageRelay.RelayModule<SendBase64FileMessage, SendBase64FileData>(data, new MessageRoute(player.ND_SmallID(), NetworkChannel.Reliable));
				}
			});
			page.CreateString("Send Code Mod .dll Name", Color.yellow, SendBase64FileMessage.codemodname, delegate(string stringy)
			{
				SendBase64FileMessage.codemodname = stringy;
			});
			if (bitsending)
			{
				page.CreateString("Send Player Bits", Color.yellow, "100", delegate(string stringy)
				{
					if (!(TimeReferences.TimeSinceStartup - SendBitMessage._timeOfRequest <= 3f) && int.TryParse(stringy, out var result))
					{
						if (PointItemManager.GetBitCount() >= result)
						{
							SendBitData sendBitData = SendBitData.Create(PlayerIDManager.LocalSmallID, result);
							MessageRelay.RelayModule<SendBitMessage, SendBitData>(sendBitData, new MessageRoute(player.ND_SmallID(), NetworkChannel.Reliable));
							PointItemManager.DecrementBits(sendBitData.bits);
						}
						else
						{
							NotificationNowAlways("Nemesis Anti-Cheat", "Not Enough Bits!", NotificationType.WARNING, 3.5f);
						}
					}
				});
			}
			if (playermessaging)
			{
				page.Logsettingsfloat("Send Message Popup Length", Color.green, ref messgfloattime, 1f, 1f, 5f, delegate(float floatnow)
				{
					messgfloattime = floatnow;
				});
				page.LogsettingsString("Send Message", Color.yellow, ref messagenowplayer, delegate(string stringy)
				{
					messagenowplayer = stringy;
					SendPlayerMessage(player, messagenowplayer, messgfloattime, player.ND_SmallID());
				});
			}
			if (bodylogsending)
			{
				page.CreateFunction("Send Player Your Bodylog", Color.yellow, delegate
				{
					if (!(TimeReferences.TimeSinceStartup - SendModIDMessage._timeOfRequest <= 5f))
					{
						if (DataManager.ActiveSave.PlayerSettings.FavoriteAvatars == null)
						{
							DataManager.ActiveSave.PlayerSettings.FavoriteAvatars = new Il2CppSystem.Collections.Generic.List<string>();
							for (int i = 0; i < 6; i++)
							{
								DataManager.ActiveSave.PlayerSettings.FavoriteAvatars.Add("EMPTY");
							}
						}
						int modID = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[0]).Crate.Pallet);
						int modID2 = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[1]).Crate.Pallet);
						int modID3 = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[2]).Crate.Pallet);
						int modID4 = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[3]).Crate.Pallet);
						int modID5 = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[4]).Crate.Pallet);
						int modID6 = CrateFilterer.GetModID(new AvatarCrateReference(DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[5]).Crate.Pallet);
						SendBodyLogData data = SendBodyLogData.Create(PlayerIDManager.LocalSmallID, DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[0], DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[1], DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[2], DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[3], DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[4], DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[5], modID, modID2, modID3, modID4, modID5, modID6);
						MessageRelay.RelayModule<SendBodyLogMessage, SendBodyLogData>(data, new MessageRoute(player.ND_SmallID(), NetworkChannel.Reliable));
						NotificationNowAlways("Nemesis Anti-Cheat", "Sent Bodylog To " + CleanedNAME(player), NotificationType.WARNING, 3.5f);
					}
				});
			}
			if (!modidsending)
			{
				continue;
			}
			page.CreateString("Send Player Mod.IO ID#", Color.yellow, "4158753", delegate(string stringh)
			{
				if (!(TimeReferences.TimeSinceStartup - SendModIDMessage._timeOfRequest <= 5f) && int.TryParse(stringh, out var result))
				{
					SendModIDData data = SendModIDData.Create(PlayerIDManager.LocalSmallID, result);
					MessageRelay.RelayModule<SendModIDMessage, SendModIDData>(data, new MessageRoute(player.ND_SmallID(), NetworkChannel.Reliable));
					NotificationNowAlways("Nemesis Anti-Cheat", "Sent Mod.IO " + stringh + " To " + CleanedNAME(player), NotificationType.WARNING, 3.5f);
				}
			});
		}
	}

	internal static string BarcodeInHand()
	{
		WhichHand hand = ((handnowreal == handnow.Left) ? WhichHand.Left : WhichHand.Right);
		MarrowEntity marrowEntity = ND_YourGetHand(hand)?.ND_GetMarrowEntity();
		return (marrowEntity != null) ? marrowEntity.ND_GetBarcodeID() : string.Empty;
	}

	internal static void Owneroptionsonly()
	{
		BoneLib.BoneMenu.Page page = OwnerOnlyPg.CreatePage("+ Server Settings", Color.green);
		page.CreateFunction("Switch To Sandbox", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "sandbox");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Team Deathmatch", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Team Deathmatch");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Deathmatch", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Deathmatch");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Juggernaut", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Juggernaut");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Entangled", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Entangled");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Smash Bones", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Smash Bones");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("Switch To Hide And Seek", Color.yellow, delegate
		{
			if (!(TimeReferences.TimeSinceStartup - SendGameModeOverMessage._timeOfRequest <= 5f))
			{
				SendGameModeOverData data = SendGameModeOverData.Create(PlayerIDManager.LocalSmallID, "Lakatrazz.Hide And Seek");
				MessageRelay.RelayModule<SendGameModeOverMessage, SendGameModeOverData>(data, CommonMessageRoutes.ReliableToServer);
			}
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0+");
		page.CreateFunction("NameTags ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("NameTags", !LobbyInfoManager.LobbyInfo.NameTags);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("VoiceChat ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("VoiceChat", !LobbyInfoManager.LobbyInfo.VoiceChat);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Mortality ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Mortality", !LobbyInfoManager.LobbyInfo.Mortality);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Friendly Fire ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Friendly Fire", !LobbyInfoManager.LobbyInfo.FriendlyFire);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Knockout ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Knockout", !LobbyInfoManager.LobbyInfo.Knockout);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Player Constraining ON/OFF", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Player Constraining", !LobbyInfoManager.LobbyInfo.PlayerConstraining);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		BoneLib.BoneMenu.EnumElement enummy = page.LogsettingsEnum("Permission Level To Set", Color.yellow, ref TempLevelNow, delegate(PermissionLevel enabled)
		{
			TempLevelNow = enabled;
		});
		page.CreateFunction("Set Permission : Dev Tools", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Dev Tools", enummy.Value);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Set Permission : Constrainer", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Constrainer", enummy.Value);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Set Permission : Custom Avatars", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Custom Avatars", enummy.Value);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		page.CreateFunction("Set Permission : Teleportation", Color.yellow, delegate
		{
			InGame.SendSettingToServer("Teleportation", enummy.Value);
		}).SetTooltip("Only Works If Host Has Nemesis Anti-Cheat At Least 1.0.0");
		OwnerOnlyPg.Logsettings("Despawn Dead NPC's", Color.cyan, ref despawndeadnpcs, delegate(bool enabled)
		{
			despawndeadnpcs = enabled;
		});
		fpsdespawn = OwnerOnlyPg.CreatePage("+ FPS Despawner", Color.green);
		fpsdespawn.Logsettingsfloat("FPS Limit", Color.green, ref fpslimit, 1f, 1f, 15f, delegate(float intnow)
		{
			fpslimit = intnow;
		});
		fpsdespawn.LogsettingsEnum("FPS Despawn Filter", Color.yellow, ref DespawnerTimerz, delegate(DespawnerAll enabled)
		{
			DespawnerTimerz = enabled;
		});
		fpsdespawn.Logsettings("FPS Despawner", Color.cyan, ref fpsdesapwner, delegate(bool enabled)
		{
			fpsdesapwner = enabled;
		});
		AISpawnersPage = OwnerOnlyPg.CreatePage("+ Spawners", Color.green);
		BoneLib.BoneMenu.Page page2 = OwnerOnlyPg.CreatePage("+ Infinite Inventory", Color.green);
		page2.LogsettingsEnum("Infinite Slot", Color.yellow, ref SlotsNowReal, delegate(Slots enabled)
		{
			SlotsNowReal = enabled;
		});
		page2.Logsettings("Infinite Inventory", Color.cyan, ref infiniteinventory, delegate(bool enabled)
		{
			if (!infiniteinventory && enabled)
			{
				NotificationNow("Nemesis Anti-Cheat", "Stored Current Inventory!\nRe-Enable For Storing New Stuff!", NotificationType.SUCCESS, 2f);
				StoreInventoryItems();
			}
			infiniteinventory = enabled;
		});
		page2.Logsettings("Infinite Inventory All Slots", Color.cyan, ref infiniteinvall, delegate(bool enabled)
		{
			infiniteinvall = enabled;
		});
		OwnerOnlyPg.CreateFunction("Teleport All", Color.yellow, delegate
		{
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
			if (FusionPermissions.HasSufficientPermissions(level, LobbyInfoManager.LobbyInfo.Teleportation))
			{
				SendMessageToHost("Teleported Everyone");
				foreach (PlayerID playerID in PlayerIDManager.PlayerIDs)
				{
					if ((byte)playerID != PlayerIDManager.LocalSmallID)
					{
						PermissionSender.SendPermissionRequest(PermissionCommandType.TELEPORT_TO_ME, playerID);
					}
				}
			}
		});
		OwnerOnlyPg.LogsettingsEnum("Despawn All Filter", Color.yellow, ref DespawnerAllReal, delegate(DespawnerAll enabled)
		{
			DespawnerAllReal = enabled;
		});
		OwnerOnlyPg.CreateFunction("Despawn All", Color.yellow, delegate
		{
			if (AreYouOWNER())
			{
				SendMessageToHost("Despawned Everything");
				DespawnAll(DespawnerAllReal);
			}
		});
		OwnerOnlyPg.Logsettings("God Mode", Color.cyan, ref godmode, delegate(bool enabled)
		{
			godmode = enabled;
		});
		OwnerOnlyPg.Logsettings("Anti Knockout", Color.cyan, ref antiknockout, delegate(bool enabled)
		{
			antiknockout = enabled;
		});
		OwnerOnlyPg.Logsettings("Anti Dev Manipulator", Color.cyan, ref AntiDevManip, delegate(bool enabled)
		{
			if (!AntiDevManip && enabled)
			{
				foreach (NetworkEntity item in NetworkEntities())
				{
					if (item.ND_GetMarrowEntity().ND_GetBarcodeID() == "c1534c5a-c6a8-45d0-aaa2-2c954465764d")
					{
						DespawnNow(item);
					}
				}
			}
			AntiDevManip = enabled;
		});
		OwnerOnlyPg.Logsettings("Anti Lasereyes", Color.cyan, ref AntiLasereyes, delegate(bool enabled)
		{
			if (!AntiLasereyes && enabled)
			{
				foreach (NetworkEntity item2 in NetworkEntities())
				{
					if (item2.ND_GetMarrowEntity().ND_GetBarcodeID() == "BamBaeYoh.LaserEyes.Spawnable.LaserEyes")
					{
						DespawnNow(item2);
					}
				}
			}
			AntiLasereyes = enabled;
		});
	}

	internal static void OPERATORoptions()
	{
		OPERATORPG.Logsettings("Anti One Shot", Color.cyan, ref antioneshot, delegate(bool enabled)
		{
			antioneshot = enabled;
		});
		OPERATORPG.CreateFunction("Clone Weapon In Left Hand", Color.yellow, delegate
		{
			string text = (ND_YourGetHand(WhichHand.Left)?.ND_GetMarrowEntity())?.ND_GetBarcodeID();
			if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
			{
				SpawnIt(text, (ND_YourGetHand(WhichHand.Left)?.transform.position + ND_YourGetHand(WhichHand.Left)?.transform.forward + ND_YourGetHand(WhichHand.Left)?.transform.up).Value, Quaternion.identity);
			}
		});
		OPERATORPG.CreateFunction("Clone Weapon In Right Hand", Color.yellow, delegate
		{
			string text = (ND_YourGetHand(WhichHand.Right)?.ND_GetMarrowEntity())?.ND_GetBarcodeID();
			if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
			{
				SpawnIt(text, (ND_YourGetHand(WhichHand.Right)?.transform.position + ND_YourGetHand(WhichHand.Right)?.transform.forward + ND_YourGetHand(WhichHand.Right)?.transform.up).Value, Quaternion.identity);
			}
		});
		OPERATORPG.Logsettings("Dashing", Color.cyan, ref dashingnow, delegate(bool enabled)
		{
			dashingnow = enabled;
		});
		OPERATORPG.Logsettings("Double Jump", Color.cyan, ref doublejumpnow, delegate(bool enabled)
		{
			doublejumpnow = enabled;
		});
		OPERATORPG.Logsettings("Air Control", Color.cyan, ref Aircontrolnow, delegate(bool enabled)
		{
			Aircontrolnow = enabled;
		});
		OPERATORPG.Logsettings("Anti Self Constraints", Color.cyan, ref selfconstraint, delegate(bool enabled)
		{
			selfconstraint = enabled;
		});
	}

	internal static void Hostonlyoptions()
	{
		HOSTONLYPGE.Logsettings("Hide Playerlist From Lobbybrowser", Color.white, ref HIDEPLAYERLIST, delegate(bool enabled)
		{
			if (!HIDEPLAYERLIST && enabled && NetworkInfo.IsHost)
			{
				NetworkHelper.Disconnect();
				NotificationNowAlways("Nemesis Anti-Cheat", "Disconnected Lobby Restart It To Have Playerlist Protection", NotificationType.SUCCESS, 5f);
			}
			HIDEPLAYERLIST = enabled;
		});
		AltpreventPG = HOSTONLYPGE.CreatePage("+ Player Options [PO]", Color.green);
		AltpreventPG.Logsettings("Enable Player Options", Color.cyan, ref disablesteamreading, delegate(bool enabled)
		{
			disablesteamreading = enabled;
		});
		AltpreventPG.Logsettings("[PO] Spoof Profile Notification", Color.cyan, ref spooferprofiledetection, delegate(bool enabled)
		{
			spooferprofiledetection = enabled;
		});
		AltpreventPG.Logsettings("[PO] Clone Exploit Notifications", Color.cyan, ref clonedetector, delegate(bool enabled)
		{
			clonedetector = enabled;
		});
		AltpreventPG.Logsettings("[PO] Kick Private Steam Accounts", Color.cyan, ref privatekicksteam, delegate(bool enabled)
		{
			privatekicksteam = enabled;
		});
		AltpreventPG.Logsettings("[PO] Alt Remover", Color.cyan, ref AltRemov, delegate(bool enabled)
		{
			AltRemov = enabled;
		});
		AltpreventPG.Logsettings("[PO] Alt Removal Ban Instead", Color.cyan, ref baninsteadalt, delegate(bool enabled)
		{
			baninsteadalt = enabled;
		});
		AltpreventPG.Logsettings("[PO] Alt Teleport To You", Color.cyan, ref teleportaltacctoyou, delegate(bool enabled)
		{
			teleportaltacctoyou = enabled;
		});
		statskick = HOSTONLYPGE.CreatePage("+ Stat Kicker Presets", Color.green);
		statskick.LogsettingsString("Preset Name", Color.yellow, ref statskickernows, delegate(string stringy)
		{
			statskickernows = stringy;
		});
		statskick.CreateFunction("Add Preset", Color.yellow, delegate
		{
			bool flag = false;
			foreach (StatsKickerPresets item2 in StatsKickerPresets.StatsKickerPresetz)
			{
				if (string.Equals(item2.TitleOfPreset, statskickernows, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				StatsKickerPresets.StatsKickerPresetz.Add(new StatsKickerPresets(statskickernows, "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000"));
				StatsKickerPresets.SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Added Preset " + statskickernows + "!", NotificationType.SUCCESS, 2.5f);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
			}
		});
		statskicknow = statskick.CreatePage("+ Active Presets", Color.green);
		statskick.Logsettings("Stat Kicker", Color.cyan, ref statkicker, delegate(bool enabled)
		{
			statkicker = enabled;
		});
		spawnlimitersz = HOSTONLYPGE.CreatePage("+ Spawn Limits", Color.green);
		spawnlimitersz.Logsettings("Item Limit To Player Count", Color.cyan, ref limitplayercount, delegate(bool enabled)
		{
			limitplayercount = enabled;
		});
		spawnlimitersz.CreateFunction("Add Per Item Limit Right Hand Item", Color.yellow, delegate
		{
			string id = ND_YourGetHand(WhichHand.Right).ND_GetMarrowEntity().ND_GetBarcodeID();
			if (IsBarcodeInGame(id))
			{
				System.Collections.Generic.List<string> list = (File.Exists(spawnlimitshostonly) ? File.ReadAllLines(spawnlimitshostonly).ToList() : new System.Collections.Generic.List<string>());
				string text = list.FirstOrDefault((string l) => l.StartsWith(id + ":"));
				string text2 = null;
				if (text != null)
				{
					text2 = text.Split(':')[1];
					if (text2 == limithostonly.ToString())
					{
						list.Remove(text);
						File.WriteAllLines(spawnlimitshostonly, list);
						spawnlimitlinelist.Remove(text);
						NotificationNow("Nemesis Anti-Cheat", $"Removed {id.ND_BarcodeCrateName()} Limit [{limithostonly}] From Spawn Limiter!", NotificationType.ERROR, 3f);
						return;
					}
					list.Remove(text);
				}
				string item = $"{id}:{limithostonly}";
				list.Add(item);
				File.WriteAllLines(spawnlimitshostonly, list);
				spawnlimitlinelist.Remove(text);
				spawnlimitlinelist.Add(item);
				if (text2 != null)
				{
					NotificationNow("Nemesis Anti-Cheat", $"Updated {id.ND_BarcodeCrateName()} Limit [{text2} → {limithostonly}]", NotificationType.SUCCESS, 3f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", $"Added {id.ND_BarcodeCrateName()} Limit [{limithostonly}] To Spawn Limiter!", NotificationType.SUCCESS, 3f);
				}
			}
		});
		spawnlimitersz.CreateFunction("Add Per Item Limit Left Hand Item", Color.yellow, delegate
		{
			string id = ND_YourGetHand(WhichHand.Left).ND_GetMarrowEntity().ND_GetBarcodeID();
			if (IsBarcodeInGame(id))
			{
				System.Collections.Generic.List<string> list = (File.Exists(spawnlimitshostonly) ? File.ReadAllLines(spawnlimitshostonly).ToList() : new System.Collections.Generic.List<string>());
				string text = list.FirstOrDefault((string l) => l.StartsWith(id + ":"));
				string text2 = null;
				if (text != null)
				{
					text2 = text.Split(':')[1];
					if (text2 == limithostonly.ToString())
					{
						list.Remove(text);
						File.WriteAllLines(spawnlimitshostonly, list);
						spawnlimitlinelist.Remove(text);
						NotificationNow("Nemesis Anti-Cheat", $"Removed {id.ND_BarcodeCrateName()} Limit [{limithostonly}] From Spawn Limiter!", NotificationType.ERROR, 3f);
						return;
					}
					list.Remove(text);
				}
				string item = $"{id}:{limithostonly}";
				list.Add(item);
				File.WriteAllLines(spawnlimitshostonly, list);
				spawnlimitlinelist.Remove(text);
				spawnlimitlinelist.Add(item);
				if (text2 != null)
				{
					NotificationNow("Nemesis Anti-Cheat", $"Updated {id.ND_BarcodeCrateName()} Limit [{text2} → {limithostonly}]", NotificationType.SUCCESS, 3f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", $"Added {id.ND_BarcodeCrateName()} Limit [{limithostonly}] To Spawn Limiter!", NotificationType.SUCCESS, 3f);
				}
			}
		});
		spawnlimitersz.Logsettingsint("Per Item Limit", Color.green, ref limithostonly, 1, 1, 100, delegate(int intnow)
		{
			limithostonly = intnow;
		});
		spawnlimitersz.Logsettings("Per Item Spawn Limiter", Color.cyan, ref hostonlyspawnlimiter, delegate(bool enabled)
		{
			hostonlyspawnlimiter = enabled;
		});
		spawnlimitersz.Logsettingsint("Global Per Item Limit", Color.green, ref limitnowofglobal, 1, 1, 100, delegate(int intnow)
		{
			limitnowofglobal = intnow;
		});
		spawnlimitersz.Logsettings("Global Per Item Spawn Limiter", Color.cyan, ref globalspawnlimitperitem, delegate(bool enabled)
		{
			globalspawnlimitperitem = enabled;
		});
		HOSTONLYPGE.Logsettings("No Damage Unless Weapons In Hand", Color.cyan, ref nodamageunlessweapons, delegate(bool enabled)
		{
			nodamageunlessweapons = enabled;
		});
		HOSTONLYPGE.Logsettings("Anti Metadata Spoof (Impersonation)", Color.cyan, ref antimetadataspoof, delegate(bool enabled)
		{
			antimetadataspoof = enabled;
		});
		HOSTONLYPGE.Logsettings("Anti Ghost Mode", Color.cyan, ref antighostmode, delegate(bool enabled)
		{
			antighostmode = enabled;
		});
		HOSTONLYPGE.Logsettings("Anti Ghost Mode - Kick Offender", Color.cyan, ref ghostmodekick, delegate(bool enabled)
		{
			ghostmodekick = enabled;
		});
		HOSTONLYPGE.Logsettings("Block Joiners When Lobby Isn't Public", Color.cyan, ref blockprivatejoiners, delegate(bool enabled)
		{
			blockprivatejoiners = enabled;
		});
		HOSTONLYPGE.LogsettingsString("Max Damage Threshold Value", Color.yellow, ref maxdamagethressy, delegate(string stringy)
		{
			if (float.TryParse(stringy, out var _))
			{
				maxdamagethressy = stringy;
				NotificationNow("Nemesis Anti-Cheat", "Set Value " + maxdamagethressy, NotificationType.SUCCESS, 2f);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Failed Needs To Be A Number!!!!!!!!!!!", NotificationType.ERROR, 2f);
			}
		});
		HOSTONLYPGE.Logsettings("Max Damage Threshold", Color.cyan, ref servermaxdamagethres, delegate(bool enabled)
		{
			servermaxdamagethres = enabled;
		});
		HOSTONLYPGE.Logsettings("No Name Change", Color.cyan, ref AntiAnimatedName, delegate(bool enabled)
		{
			AntiAnimatedName = enabled;
		});
		HOSTONLYPGE.Logsettings("Kick If Vitality Infinite", Color.cyan, ref kickifvitaly, delegate(bool enabled)
		{
			kickifvitaly = enabled;
		});
		HOSTONLYPGE.Logsettings("Avatar Kick If Change Into", Color.cyan, ref avatarskickon, delegate(bool enabled)
		{
			avatarskickon = enabled;
		});
		HOSTONLYPGE.Logsettings("Kick If Spawned", Color.cyan, ref spawnableskickon, delegate(bool enabled)
		{
			spawnableskickon = enabled;
		});
		HOSTONLYPGE.Logsettings("Kick Weird/Invisible Usernames", Color.cyan, ref kickunincodenames, delegate(bool enabled)
		{
			kickunincodenames = enabled;
		});
		HOSTONLYPGE.LogsettingsEnum("Anti Modded Spawn Guns Type", Color.yellow, ref antimodguntypereal, delegate(antimodguntype enabled)
		{
			antimodguntypereal = enabled;
		});
		HOSTONLYPGE.Logsettings("Anti Modded Spawn Guns", Color.cyan, ref SpawnGunProtection, delegate(bool enabled)
		{
			SpawnGunProtection = enabled;
		});
		HOSTONLYPGE.Logsettings("Owner Perms Bypass Some Protections", Color.cyan, ref OwnerCheckEnabled, delegate(bool enabled)
		{
			OwnerCheckEnabled = enabled;
		});
		HOSTONLYPGE.Logsettings("Anti Spam Spawning", Color.cyan, ref spamspawnprevention, delegate(bool enabled)
		{
			spamspawnprevention = enabled;
		});
		HOSTONLYPGE.Logsettingsint("Anti Spam Spawn Limit", Color.green, ref countbeforespam, 1, 1, 20, delegate(int intnow)
		{
			countbeforespam = intnow;
		});
		HOSTONLYPGE.Logsettingsint("Anti Spam Spawn Timer", Color.green, ref antispamspawntimer, 1, 1, 10000, delegate(int intnow)
		{
			antispamspawntimer = intnow;
		});
		HOSTONLYPGE.Logsettings("Anti Crash Spawn Delay", Color.cyan, ref spawnlimiternow, delegate(bool enabled)
		{
			spawnlimiternow = enabled;
		});
		HOSTONLYPGE.Logsettingsfloat("Spawn Delay", Color.green, ref spawnlimitertimer, 0.1f, 0.1f, 5f, delegate(float floatnow)
		{
			spawnlimitertimer = floatnow;
		});
		HOSTONLYPGE.Logsettings("Owners Can Change Map", Color.cyan, ref ownerscanchangemap, delegate(bool enabled)
		{
			ownerscanchangemap = enabled;
		});
		HOSTONLYPGE.Logsettings("Owners Can Change Server Settings", Color.cyan, ref OWNERSCANCHANGESERVER, delegate(bool enabled)
		{
			OWNERSCANCHANGESERVER = enabled;
		});
		HOSTONLYPGE.Logsettings("Owners Can Change Server Gamemode", Color.cyan, ref ownerscanchangegamemode, delegate(bool enabled)
		{
			ownerscanchangegamemode = enabled;
		});
		HOSTONLYPGE.Logsettings("Spawn Bypass Protection", Color.cyan, ref spawnbypassprotection, delegate(bool enabled)
		{
			spawnbypassprotection = enabled;
		});
		HOSTONLYPGE.Logsettings("Despawn Protection", Color.cyan, ref DESPAWNPROTECTION, delegate(bool enabled)
		{
			DESPAWNPROTECTION = enabled;
		});
		HOSTONLYPGE.Logsettings("Prevent Balling Players", Color.cyan, ref ballplayersprevention, delegate(bool enabled)
		{
			ballplayersprevention = enabled;
		});
		HOSTONLYPGE.Logsettings("Prevent Blinding Players", Color.cyan, ref blindplayersprevention, delegate(bool enabled)
		{
			blindplayersprevention = enabled;
		});
		HOSTONLYPGE.Logsettings("Kick Username Spoofers", Color.cyan, ref AutoKickSpoofers, delegate(bool enabled)
		{
			AutoKickSpoofers = enabled;
		});
		HOSTONLYPGE.Logsettings("Safe Distance Spawning", Color.cyan, ref safedistancespawning, delegate(bool enabled)
		{
			safedistancespawning = enabled;
		}).SetTooltip("this makes it so if anyone is within a few meters of a player they cannot spawn they have to backup to spawn stuff");
		HOSTONLYPGE.Logsettings("Strength Threshold", Color.green, ref strengththresprotection, delegate(bool enabled)
		{
			strengththresprotection = enabled;
		});
		HOSTONLYPGE.Logsettingsfloat("Strength Threshold Value", Color.cyan, ref strengththreshnow, 10f, 150f, 2000f, delegate(float value)
		{
			strengththreshnow = value;
		});
	}

	internal static bool AreYouOWNER()
	{
		if (!NetworkInfo.HasServer)
		{
			return true;
		}
		FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
		return level == PermissionLevel.OWNER;
	}

	internal static bool AreYouOPERATOR()
	{
		if (!NetworkInfo.HasServer)
		{
			return true;
		}
		FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
		return level == PermissionLevel.OPERATOR || level == PermissionLevel.OWNER;
	}

	internal static bool HostIsMe(NetworkPlayer playerNow)
	{
		return playerNow != null && playerNow.PlayerID != null && playerNow.PlayerID.IsHost && playerNow.IsMe();
	}

	internal static void ReloadList()
	{
		TeleporterManager.LoadTeleporters();
		CreateCheatToolsPreset.LoadPresets();
		BodyLogPage.LoadPresets();
		InventoryPage.LoadAllPresets();
		BodyLogRadialMenuColorPreset.LoadPresets();
		StatsKickerPresets.LoadPresets();
		FusionProfilePresets.LoadPresets();
		BlockedSpawnables.Clear();
		foreach (string item in from l in File.ReadAllLines(BlockedSpawnablesPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			BlockedSpawnables.Add(item);
		}
		WarnedSpawnables.Clear();
		foreach (string item2 in from l in File.ReadAllLines(WarnedSpawnablesPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			WarnedSpawnables.Add(item2);
		}
		SpawnablesKick.Clear();
		foreach (string item3 in from l in File.ReadAllLines(SpawnablesKickPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			SpawnablesKick.Add(item3);
		}
		AvatarsKick.Clear();
		foreach (string item4 in from l in File.ReadAllLines(AvatarsKickPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			AvatarsKick.Add(item4);
		}
		blockedplatformids.Clear();
		foreach (string item5 in from l in File.ReadAllLines(DamageBlockPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockedplatformids.Add(item5);
		}
		CustomAvFav.Clear();
		CustomAvFavref.Clear();
		foreach (string item6 in from l in File.ReadAllLines(AvatarCustomFav)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			CustomAvFav.Add(item6);
			CustomAvFavref.Add(new AvatarCrateReference(item6));
		}
		CustomSpawnFav.Clear();
		CustomSpawnFavref.Clear();
		foreach (string item7 in from l in File.ReadAllLines(SpawnableCustomFav)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			CustomSpawnFav.Add(item7);
			CustomSpawnFavref.Add(new SpawnableCrateReference(item7));
		}
		blockedspawnies.Clear();
		foreach (string item8 in from l in File.ReadAllLines(ServerBlockSpawnPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockedspawnies.Add(item8);
		}
		voicblocked.Clear();
		foreach (string item9 in from l in File.ReadAllLines(voicepathblocked)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			voicblocked.Add(item9);
		}
		spawnlimitlinelist.Clear();
		spawnlimitline.Clear();
		foreach (string item10 in from l in File.ReadAllLines(spawnlimitshostonly)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			spawnlimitlinelist.Add(item10);
			string text = item10.Trim();
			string[] array = text.Split(new char[1] { ':' }, 2);
			string key = array[0].Trim().ToLowerInvariant().ToString();
			string s = array[1].Trim();
			if (int.TryParse(s, out var result))
			{
				spawnlimitline.Add(key, result);
			}
		}
		blockedavifallbacks.Clear();
		foreach (string item11 in from l in File.ReadAllLines(avatarsblocked)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockedavifallbacks.Add(item11);
		}
		modidblocked.Clear();
		foreach (string item12 in from l in File.ReadAllLines(ModIDBLOCKSPATH)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			modidblocked.Add(item12);
		}
		blockmessages.Clear();
		foreach (string item13 in from l in File.ReadAllLines(blockmessagingnowpath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockmessages.Add(item13);
		}
		blockentireauthor.Clear();
		foreach (string item14 in from l in File.ReadAllLines(blockauthornowlist)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockentireauthor.Add(item14);
		}
		blockentirepallet.Clear();
		foreach (string item15 in from l in File.ReadAllLines(blockpalletnowlist)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockentirepallet.Add(item15);
		}
		blockavipalletlist.Clear();
		foreach (string item16 in from l in File.ReadAllLines(BlockPalletAviNowp)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockavipalletlist.Add(item16);
		}
		blockaviauthorlist.Clear();
		foreach (string item17 in from l in File.ReadAllLines(BlockAviAuthorNowp)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockaviauthorlist.Add(item17);
		}
		warnavilist.Clear();
		foreach (string item18 in from l in File.ReadAllLines(WarnAvisNow)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			warnavilist.Add(item18);
		}
		MEDIAPLAYERBLOCKERNOWList.Clear();
		foreach (string item19 in from l in File.ReadAllLines(MEDIAPLAYERBLOCKERNOW)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			MEDIAPLAYERBLOCKERNOWList.Add(item19);
		}
		blockmovements.Clear();
		foreach (string item20 in from l in File.ReadAllLines(BlockMovementsPath)
			where !string.IsNullOrWhiteSpace(l)
			select l)
		{
			blockmovements.Add(item20);
		}
		if (File.Exists(InventoryPage.PresetsFileCurrent))
		{
			string value = File.ReadAllText(InventoryPage.PresetsFileCurrent);
			System.Collections.Generic.Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(value);
			if (dictionary != null)
			{
				InventoryPage.CurrentPreset = dictionary;
			}
		}
		if (File.Exists(CreateCheatToolsPreset.devitemscurrent))
		{
			string value2 = File.ReadAllText(CreateCheatToolsPreset.devitemscurrent);
			CreateCheatToolsPreset createCheatToolsPreset = JsonConvert.DeserializeObject<CreateCheatToolsPreset>(value2);
			if (createCheatToolsPreset != null)
			{
				CreateCheatToolsPreset.CurrentPresetNow = createCheatToolsPreset;
			}
		}
	}

	internal static void RemoveSoundGrip(Grip gruppy)
	{
		GameObject gameObject = gruppy.gameObject.transform.root.gameObject;
		if (!gameObject.ND_IsWeapon() && !gameObject.ND_IsMelee())
		{
			return;
		}
		foreach (AudioSource componentsInChild in gameObject.gameObject.GetComponentsInChildren<AudioSource>(includeInactive: true))
		{
			componentsInChild.Stop();
			componentsInChild.mute = true;
		}
		foreach (AudioSource componentsInChild2 in gameObject.gameObject.GetComponentsInChildren<AudioSource>(includeInactive: true))
		{
			componentsInChild2.enabled = false;
		}
		foreach (GunSFX componentsInChild3 in gameObject.GetComponentsInChildren<GunSFX>(includeInactive: true))
		{
			componentsInChild3.enabled = false;
		}
		foreach (GravGunSFX componentsInChild4 in gameObject.GetComponentsInChildren<GravGunSFX>(includeInactive: true))
		{
			componentsInChild4.enabled = false;
		}
	}

	public override void OnGUI()
	{
		if (NetworkInfo.HasLayer && !HelperMethods.IsLoading() && show)
		{
			int num = 1000;
			int num2 = 1000;
			int num3 = (Screen.width - num) / 2;
			int num4 = (Screen.height - num2) / 2;
			GUI.Window(8888, new Rect(num3, num4, num, num2), (Action<int>)ProtectorUI, "Nemesis Anti-Cheat v1.48.94 | [Press Y To Close/Open | ← → Scroll Left & Right ]");
			GUI.BringWindowToFront(8888);
			int num5 = 320;
			int num6 = 1000;
			int num7 = 10;
			int num8 = num3 - num5 - num7;
			int num9 = num4;
			GUI.Window(7777, new Rect(num8, num9, num5, num6), (Action<int>)PagesMenu, "Nemesis Anti-Cheat Pages");
			GUI.BringWindowToFront(7777);
		}
	}

	internal static void PagesMenu(int windowID)
	{
		if (Event.current != null)
		{
			switch (Event.current.type)
			{
			case EventType.ScrollWheel:
				pscroll += Event.current.delta.y * 20f;
				break;
			case EventType.KeyDown:
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					pscroll -= 20f;
				}
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					pscroll += 20f;
				}
				break;
			}
			pscroll = Mathf.Max(0f, pscroll);
		}
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
		{
			richText = true
		};
		float y = 20f - pscroll;
		GUI.Label(new Rect(10f, y, 300f, 30f), "Pages :");
		y += 35f;
		if (NetworkInfo.HasLayer)
		{
			CreatePageButton("Fusion Lookup", delegate
			{
				PageNow = MenuSections.PlayerSearch;
			});
		}
		if (NetworkInfo.HasServer)
		{
			CreatePageButton("Network Entities", delegate
			{
				PageNow = MenuSections.SceneEntities;
				ListNetworkEntities = NetworkEntities().ToList();
			});
			CreatePageButton("Players Information", delegate
			{
				PageNow = MenuSections.PlayerInformation;
			});
			CreatePageButton("Server History", delegate
			{
				PageNow = MenuSections.ServerHistory;
			});
		}
		CreatePageButton("Main Options", delegate
		{
			PageNow = MenuSections.SelfCat;
		});
		CreatePageButton("Lobby Spawn Logs", delegate
		{
			PageNow = MenuSections.SpawnLogs;
		});
		if (NetworkInfo.IsHost)
		{
			CreatePageButton("Network Message Logger", delegate
			{
				PageNow = MenuSections.NetworkLogger;
			});
			CreatePageButton("Damage Information", delegate
			{
				PageNow = MenuSections.DamageLogger;
			});
			CreatePageButton("Player Avatar Stats", delegate
			{
				PageNow = MenuSections.PlayerSeriStats;
			});
		}
		CreatePageButton("Recently Met Players", delegate
		{
			PageNow = MenuSections.JoinLoggerNow;
		});
		CreatePageButton("Mod.IO Searchup", delegate
		{
			PageNow = MenuSections.MODIOSEARCHER;
		});
		CreatePageButton("Mod.IO Download Logger", delegate
		{
			PageNow = MenuSections.DownloadLogger;
		});
		CreatePageButton("Media Player Logger", delegate
		{
			PageNow = MenuSections.MediaPlayerLogger;
		});
		CreatePageButton("Avatar Logs", delegate
		{
			PageNow = MenuSections.AvatarSwitchLogs;
		});
		CreatePageButton("Spawnable Searcher", delegate
		{
			PageNow = MenuSections.SpawnableSearcher;
		});
		CreatePageButton("Avatar Searcher", delegate
		{
			PageNow = MenuSections.AvatarSearcher;
		});
		CreatePageButton("Custom Avatar Favorites", delegate
		{
			PageNow = MenuSections.CustomAviFav;
		});
		CreatePageButton("Custom Spawnable Favorites", delegate
		{
			PageNow = MenuSections.CustomSpawnableFav;
		});
		CreatePageButton("Fusion Ban Manager", delegate
		{
			PageNow = MenuSections.FusionBanManager;
		});
		void CreatePageButton(string label, Action onClick)
		{
			Rect position = new Rect(10f, y, 300f, 30f);
			if (GUI.Button(position, label, buttonStyle))
			{
				onClick?.Invoke();
				scrollX = 0f;
				scroll = 0f;
			}
			y += 35f;
		}
	}

	internal static void ProtectorUI(int windowID)
	{
		float num = 10f;
		float baseY = 20f;
		if (Event.current != null)
		{
			if (Event.current.type == EventType.ScrollWheel)
			{
				scroll += Event.current.delta.y * 20f;
				if (Event.current.alt)
				{
					scrollX += Event.current.delta.y * 40f;
				}
			}
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					scroll -= 20f;
				}
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					scroll += 20f;
				}
				if (Event.current.keyCode == KeyCode.LeftArrow)
				{
					scrollX -= 40f;
				}
				if (Event.current.keyCode == KeyCode.RightArrow)
				{
					scrollX += 40f;
				}
			}
			scroll = Mathf.Max(0f, scroll);
			scrollX = Mathf.Max(0f, scrollX);
		}
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
		{
			richText = true
		};
		float y = baseY - scroll;
		float xOffset = num - scrollX;
		if (PageNow != PreviousPage)
		{
			scroll = 0f;
			y = 20f;
			PreviousPage = PageNow;
		}
		else
		{
			y = baseY - scroll;
		}
		switch (PageNow)
		{
		case MenuSections.MODIOSEARCHER:
			CreateTextbox(ref modiosearchernow, "Mod.IO Searcher :");
			CreateOptionButton("Search For Mod....", delegate
			{
				if (int.TryParse(modiosearchernow, out var result))
				{
					MelonCoroutines.Start(ModioInfo(result, delegate(ModCallbackInfo info)
					{
						moddyinfostored = info;
					}));
				}
			});
			if (!string.IsNullOrEmpty(moddyinfostored.Data.NameID))
			{
				AddSection(310f);
				CreateLabel("Mod.IO Info :");
				CreateLabel("NameID :" + moddyinfostored.Data.NameID, 1000f);
				CreateLabel("ID :" + moddyinfostored.Data.ID, 1000f);
				CreateLabel("Mature :" + moddyinfostored.Data.Mature, 1000f);
				CreateLabel("MaturityOption :" + moddyinfostored.Data.MaturityOption, 1000f);
				CreateLabel("ThumbnailUrl :" + moddyinfostored.Data.ThumbnailUrl, 1000f);
				CreateOptionButton("Copy Mod.IO Info To Clipboard", delegate
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						WriteIndented = true,
						IncludeFields = true
					};
					string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(moddyinfostored, options);
					GUIUtility.systemCopyBuffer = systemCopyBuffer;
					NotificationNowAlways("Nemesis Anti-Cheat", "Copied Mod.IO Info To Clipboard!", NotificationType.SUCCESS, 3.5f);
				});
				CreateOptionButton("Add/Remove Block This Mod.IO Mod Completely", delegate
				{
					ToggleAddRemoveFromFile(moddyinfostored.Data.ID.ToString(), modidblocked, ModIDBLOCKSPATH, "Nemesis Anti-Cheat", "Added " + moddyinfostored.Data.NameID + " To Blocked Mod.IO Mod!.", "Removed " + moddyinfostored.Data.NameID + " From Blocked Mod.IO Mod!.");
				});
				CreateOptionButton("Download Mod", delegate
				{
					DownloadModIOMod(moddyinfostored.Data.ID);
				});
			}
			break;
		case MenuSections.PlayerSearch:
			CreateLabel("Lobbies Logged Since Fusion Login : " + CachedLobbies.Count);
			CreateTextbox(ref searchedloggedlobbies, "Search Through");
			foreach (IMatchmaker.LobbyInfo lobbies in CachedLobbies)
			{
				if ((StripColorTags(lobbies.Metadata.LobbyInfo.LobbyHostName).ToLower().Contains(searchedloggedlobbies.ToLower()) || StripColorTags(lobbies.Metadata.LobbyInfo.LobbyName).ToLower().Contains(searchedloggedlobbies.ToLower())) && lobbies.Metadata.LobbyInfo.Privacy == ServerPrivacy.PUBLIC)
				{
					string label4 = $"<color=yellow><b>{StripColorTags(lobbies.Metadata.LobbyInfo.LobbyName)}</b></color>\nHost: <color=cyan><b>{lobbies.Metadata.LobbyInfo.LobbyHostName}</b></color>\nPlayers: <color=lime><b>{lobbies.Metadata.LobbyInfo.PlayerCount}/{lobbies.Metadata.LobbyInfo.MaxPlayers}</b></color>\nMap: <color=orange><b>{lobbies.Metadata.LobbyInfo.LevelTitle}</b></color>\nPrivacy: <color=white><b>{lobbies.Metadata.LobbyInfo.Privacy}</b></color>";
					CreateOptionButton(label4, delegate
					{
						JsonSerializerOptions options = new JsonSerializerOptions
						{
							WriteIndented = true,
							IncludeFields = true
						};
						string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(lobbies, options);
						GUIUtility.systemCopyBuffer = systemCopyBuffer;
						NotificationNowAlways("Nemesis Anti-Cheat", "Lobby Info Copied To Clipboard!", NotificationType.SUCCESS, 3.5f);
					}, 95f, 300f, 90f);
				}
			}
			AddSection(310f);
			CreateLabel("Players Logged Since Fusion Login : " + PlayersOnline.Count);
			CreateTextbox(ref searchedloggedPLAYER, "Search Through");
			{
				foreach (PlayerInfo playerfus in PlayersOnline)
				{
					if (StripColorTags(playerfus.Username).ToLower().Contains(searchedloggedPLAYER.ToLower()) || StripColorTags(playerfus.Nickname).ToLower().Contains(searchedloggedPLAYER.ToLower()))
					{
						CreateOptionButton(CleanedNAME(playerfus.Nickname, playerfus.Username), delegate
						{
							JsonSerializerOptions options = new JsonSerializerOptions
							{
								WriteIndented = true,
								IncludeFields = true
							};
							string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(playerfus, options);
							GUIUtility.systemCopyBuffer = systemCopyBuffer;
							NotificationNowAlways("Nemesis Anti-Cheat", "Players Info Copied To Clipboard!", NotificationType.SUCCESS, 3.5f);
						});
					}
				}
				break;
			}
		case MenuSections.JoinLoggerNow:
		{
			CreateLabel("Recently Met Players :" + JoinLogger.Count);
			foreach (PlayerInfo logger in JoinLogger)
			{
				if (logger != null)
				{
					string nicknamex = logger.Nickname ?? "Unknown";
					string usernamex = logger.Username ?? "Unknown";
					CreateOptionButton(CleanedNAME(nicknamex, usernamex), delegate
					{
						StoredNowplayerrrr = logger;
					});
				}
			}
			FreezeScrolling();
			AddSection(310f);
			string text = "None";
			if (StoredNowplayerrrr != null)
			{
				string nicknamex2 = StoredNowplayerrrr.Nickname ?? "Unknown";
				string usernamex2 = StoredNowplayerrrr.Username ?? "Unknown";
				text = CleanedNAME(nicknamex2, usernamex2);
			}
			CreateLabel("Player Options: " + text);
			if (StoredNowplayerrrr == null)
			{
				break;
			}
			CreateOptionButton("Open Steam Profile", delegate
			{
				if (!string.IsNullOrEmpty(StoredNowplayerrrr.PlatformID.ToString()))
				{
					CheckSteamID(StoredNowplayerrrr.PlatformID);
				}
			});
			CreateOptionButton("Copy Details To Clipboard", delegate
			{
				GUIUtility.systemCopyBuffer = $"Nickname : {StoredNowplayerrrr.Nickname ?? "Unknown"}\nUsername : {StoredNowplayerrrr.Username ?? "Unknown"}\nSteamID : {StoredNowplayerrrr.PlatformID}\nDescription : {StoredNowplayerrrr.Description ?? "None"}\nAvatar Mod.io #ID : {StoredNowplayerrrr.AvatarModID}";
				NotificationNow("Nemesis Anti-Cheat", "Copied player information to clipboard", NotificationType.INFORMATION, 2f);
			});
			CreateOptionButton("Ban/Unban From Your Lobby", delegate
			{
				if (!string.IsNullOrEmpty(StoredNowplayerrrr.PlatformID.ToString()))
				{
					if (NetworkHelper.IsBanned(StoredNowplayerrrr.PlatformID))
					{
						BanManager.Pardon(StoredNowplayerrrr.PlatformID);
						NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
					}
					else
					{
						BanManager.BanList.Bans.RemoveAll((BanInfo b) => b.Player.PlatformID == StoredNowplayerrrr.PlatformID);
						BanManager.BanList.Bans.Add(new BanInfo
						{
							Player = StoredNowplayerrrr,
							Reason = "Manually Banned [Nemesis Anti-Cheat]"
						});
						DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
						NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
					}
				}
			});
			break;
		}
		case MenuSections.SelfCat:
			CreateLabel("Main Options :");
			CreateOptionButton("Disconnect From Server", delegate
			{
				NetworkHelper.Disconnect();
			});
			CreateOptionButton("Exit Game", delegate
			{
				Application.Quit(0);
			});
			CreateOptionButton("Reload Level", delegate
			{
				if (!NetworkInfo.IsHost)
				{
					NetworkHelper.Disconnect();
				}
				SceneStreamer.Reload();
			});
			CreateOptionButton("Despawn All [Owner Only]", delegate
			{
				DespawnAll(DespawnerAll.NoFilter);
			});
			CreateOptionButton("Copy Lobby Details To Clipboard", delegate
			{
				GUIUtility.systemCopyBuffer = DataToJsonString(LobbyInfoManager.LobbyInfo);
				NotificationNowAlways("Nemesis Anti-Cheat", "Copied Lobby Info To Clipboard!", NotificationType.SUCCESS, 3.5f);
			});
			CreateOptionButton("Reload Assests", delegate
			{
				MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly));
			});
			CreateTextbox(ref MODIOINT, "Download MOD.IO #ID : ");
			CreateOptionButton("Download MOD.IO #ID", delegate
			{
				if (int.TryParse(MODIOINT, out var result))
				{
					DownloadModIOMod(result);
				}
			});
			CreateOptionButton("Teleport To Spawn", delegate
			{
				LocalPlayer.TeleportToCheckpoint();
			});
			CreateTextbox(ref avichnge, "Change Into Avatar : ");
			CreateOptionButton("Change Into Avatar", delegate
			{
				ChangeIntoAvi(avichnge);
			});
			CreateOptionButton("Cancel Notifications", delegate
			{
				Notifier.CancelAll();
			});
			break;
		case MenuSections.SpawnLogs:
			CreateLabel("Lobby Spawn Logs :");
			CreateTextbox(ref SpawnLogsSearcher, "Lobby Spawn Log Searcher :");
			foreach (var spawnLog in SpawnLogs)
			{
				string PlayerName = spawnLog.PlayerName;
				string Username = spawnLog.Username;
				ulong PlatformID = spawnLog.PlatformID;
				string PalletName = spawnLog.PalletName;
				string PalletAuthor = spawnLog.PalletAuthor;
				int ModioID = spawnLog.ModioID;
				string BarcodeID = spawnLog.BarcodeID;
				bool isspawnableavi = spawnLog.Rest.Item1;
				string text2 = "";
				text2 = ((ModioID != -1) ? $"[#ID : {ModioID}]" : "[SLZ]");
				if (string.IsNullOrWhiteSpace(SpawnLogsSearcher))
				{
					if (!isspawnableavi)
					{
						CreateOptionButton(PalletName + " | " + text2, delegate
						{
							SpawnLogsRef = (PlayerName: PlayerName, Username: Username, PlatformID: PlatformID.ToString(), PalletName: PalletName, PalletAuthor: PalletAuthor, ModioID: ModioID.ToString(), BarcodeID: BarcodeID, IsSpawnableAvatar: isspawnableavi.ToString());
						});
					}
				}
				else if (PalletName.ToLower().Contains(SpawnLogsSearcher.ToLower()) && !isspawnableavi)
				{
					CreateOptionButton(PalletName + " | " + text2, delegate
					{
						SpawnLogsRef = (PlayerName: PlayerName, Username: Username, PlatformID: PlatformID.ToString(), PalletName: PalletName, PalletAuthor: PalletAuthor, ModioID: ModioID.ToString(), BarcodeID: BarcodeID, IsSpawnableAvatar: isspawnableavi.ToString());
					});
				}
			}
			AddSection(310f);
			CreateLabel("Avatar Spawnable Logs :");
			CreateTextbox(ref aviSpawnLogsSearcher, "Avatar Spawnable Logs Searcher :");
			foreach (var spawnLog2 in SpawnLogs)
			{
				string PlayerName2 = spawnLog2.PlayerName;
				string Username2 = spawnLog2.Username;
				ulong PlatformID2 = spawnLog2.PlatformID;
				string PalletName2 = spawnLog2.PalletName;
				string PalletAuthor2 = spawnLog2.PalletAuthor;
				int ModioID2 = spawnLog2.ModioID;
				string BarcodeID2 = spawnLog2.BarcodeID;
				bool isspawnableavi2 = spawnLog2.Rest.Item1;
				string text3 = "";
				text3 = ((ModioID2 != -1) ? $"[#ID : {ModioID2}]" : "[SLZ]");
				if (string.IsNullOrWhiteSpace(aviSpawnLogsSearcher))
				{
					if (isspawnableavi2)
					{
						CreateOptionButton(PalletName2 + " | " + text3, delegate
						{
							SpawnLogsRef = (PlayerName: PlayerName2, Username: Username2, PlatformID: PlatformID2.ToString(), PalletName: PalletName2, PalletAuthor: PalletAuthor2, ModioID: ModioID2.ToString(), BarcodeID: BarcodeID2, IsSpawnableAvatar: isspawnableavi2.ToString());
						});
					}
				}
				else if (PalletName2.ToLower().Contains(aviSpawnLogsSearcher.ToLower()) && isspawnableavi2)
				{
					CreateOptionButton(PalletName2 + " | " + text3, delegate
					{
						SpawnLogsRef = (PlayerName: PlayerName2, Username: Username2, PlatformID: PlatformID2.ToString(), PalletName: PalletName2, PalletAuthor: PalletAuthor2, ModioID: ModioID2.ToString(), BarcodeID: BarcodeID2, IsSpawnableAvatar: isspawnableavi2.ToString());
					});
				}
			}
			AddSection(310f);
			FreezeScrolling();
			if (SpawnLogsRef.PlayerName != null)
			{
				bloockwarnkickspawnables(new SpawnableCrateReference(SpawnLogsRef.BarcodeID));
				if (NetworkInfo.IsHost)
				{
					CreateLabel("First Player Who Spawned Username : " + SpawnLogsRef.Username, 600f);
					CreateLabel("First Player Who Spawned Nickname : " + StripColorTags(SpawnLogsRef.PlayerName), 600f);
					CreateLabel("First Player Who Spawned Steam ID : " + SpawnLogsRef.PlatformID, 600f);
				}
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		case MenuSections.AvatarSwitchLogs:
		{
			CreateTextbox(ref aviseachnow, "Avatar Switch Logs Searcher");
			string value = aviseachnow?.Trim().ToLower();
			foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.HashSet<string>> item3 in PlayeravatarStuff)
			{
				string playerId = item3.Key;
				System.Collections.Generic.HashSet<string> value2 = item3.Value;
				foreach (string barcode2 in value2)
				{
					AvatarCrateReference reference = new AvatarCrateReference(barcode2);
					if (reference?.Crate?.Pallet == null)
					{
						continue;
					}
					string crateName = StripColorTags(reference.Crate.name).ToLower() ?? "";
					string author = reference.Crate.Pallet.Author.ToLower() ?? "";
					string text4 = barcode2?.ToLower() ?? "";
					if ((string.IsNullOrWhiteSpace(value) || crateName.ToLower().Contains(value) || author.ToLower().Contains(value) || text4.Contains(value)) && ulong.TryParse(playerId, out var playsid))
					{
						NetworkPlayer playerstoredha = NetworkPlayer.Players.FirstOrDefault((NetworkPlayer anyx) => anyx.PlayerID.PlatformID == playsid);
						CreateOptionButton($"{crateName} | {author} [{playerId}]", delegate
						{
							AvatarSwitchyNow = (PlayerName: string.IsNullOrEmpty(playerstoredha?.ND_Nickname()) ? "" : playerstoredha.ND_Nickname(), Username: string.IsNullOrEmpty(playerstoredha?.ND_Username()) ? "" : playerstoredha.ND_Username(), PlatformID: playerId, PalletName: StripColorTags(crateName), PalletAuthor: author, ModioID: CrateFilterer.GetModID(reference.Crate.Pallet).ToString(), BarcodeID: barcode2);
						});
					}
				}
			}
			AddSection(310f);
			FreezeScrolling();
			if (AvatarSwitchyNow.PlatformID != null)
			{
				if (NetworkInfo.IsHost)
				{
					CreateLabel("First Player Who Spawned Username : " + AvatarSwitchyNow.Username, 600f);
					CreateLabel("First Player Who Spawned Nickname : " + StripColorTags(AvatarSwitchyNow.PlayerName), 600f);
					CreateLabel("First Player Who Spawned Steam ID : " + AvatarSwitchyNow.PlatformID, 600f);
				}
				CreateOptionButton("Copy Details To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = $"Spawnable Log Information : {AvatarSwitchyNow.PalletName}\nPlayer Who Spawned Username : {AvatarSwitchyNow.Username}\nPlayer Who Spawned Nickname : {AvatarSwitchyNow.PlayerName}\nMod IO : {AvatarSwitchyNow.ModioID}\nBarcode ID : {AvatarSwitchyNow.BarcodeID}\nPallet Author : {AvatarSwitchyNow.PalletAuthor}\nPallet Name : {StripColorTags(AvatarSwitchyNow.PalletName)}\nPlayer Who Spawned Steam ID : {AvatarSwitchyNow.PlatformID}";
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Copy Barcode To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = AvatarSwitchyNow.BarcodeID;
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateLabel("Avatar Switch Log Information : " + AvatarSwitchyNow.PalletName, 600f);
				blockavatarfunctions(new AvatarCrateReference(AvatarSwitchyNow.BarcodeID));
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.SpawnableSearcher:
		{
			FreezeScrolling();
			CreateTextbox(ref SpawnableSearchies, "Spawnable Searcher :");
			CreateOptionButton("Find Crate Name Results", delegate
			{
				SpawnerRESULTS("cratename");
			});
			CreateOptionButton("Find Pallet Name Results", delegate
			{
				SpawnerRESULTS("palletname");
			});
			CreateOptionButton("Find Barcode Name Results", delegate
			{
				SpawnerRESULTS("barcode");
			});
			CreateLabel("Search Results : " + SpawnablerResultsNow.Count);
			int num2 = Mathf.CeilToInt((float)SpawnablerResultsNow.Count / (float)itemsPerPage);
			int num3 = currentPage * itemsPerPage;
			int num4 = Mathf.Min(num3 + itemsPerPage, SpawnablerResultsNow.Count);
			for (int num5 = num3; num5 < num4; num5++)
			{
				string barcode = SpawnablerResultsNow[num5];
				SpawnableCrateReference spawnyreference = new SpawnableCrateReference(barcode);
				if (spawnyreference?.Crate?.Pallet != null)
				{
					string label2 = StripColorTags(spawnyreference.Crate.name) + " | " + spawnyreference.Crate.Pallet.Author;
					CreateOptionButton(label2, delegate
					{
						serresult = spawnyreference;
					});
				}
			}
			if (currentPage > 0)
			{
				CreateOptionButton("Previous Page", delegate
				{
					currentPage--;
				});
			}
			if (currentPage < num2 - 1)
			{
				CreateOptionButton("Next Page", delegate
				{
					currentPage++;
				});
			}
			AddSection(310f);
			FreezeScrolling();
			if (serresult?.Crate?.Pallet != null)
			{
				bloockwarnkickspawnables(serresult);
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.AvatarSearcher:
		{
			FreezeScrolling();
			CreateTextbox(ref AvatarSearchies, "Avatar Searcher :");
			CreateOptionButton("Find Results", delegate
			{
				MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly, enableLogging: false));
				AvisResultsNow.Clear();
				if (AvatarsStored != null)
				{
					foreach (AvatarCrateReference item4 in AvatarsStored)
					{
						if (item4?.Crate?.name != null && StripColorTags(item4.Crate.name).ToLower().Contains(AvatarSearchies.ToLower()))
						{
							AvisResultsNow.Add(item4.Barcode.ID);
						}
					}
				}
				currentPage2 = 0;
			});
			int num12 = AvisResultsNow?.Count ?? 0;
			CreateLabel("Search Results : " + num12);
			int num13 = Mathf.CeilToInt((float)num12 / (float)itemsPerPage);
			int num14 = currentPage2 * itemsPerPage;
			int num15 = Mathf.Min(num14 + itemsPerPage, num12);
			if (AvisResultsNow != null)
			{
				for (int num16 = num14; num16 < num15; num16++)
				{
					string text10 = AvisResultsNow[num16];
					if (string.IsNullOrEmpty(text10))
					{
						continue;
					}
					AvatarCrateReference spawnyreference2 = new AvatarCrateReference(text10);
					AvatarCrate avatarCrate3 = spawnyreference2?.Crate;
					Pallet pallet2 = avatarCrate3?.Pallet;
					if (avatarCrate3 != null && pallet2 != null)
					{
						string text11 = StripColorTags(avatarCrate3.name ?? "Unknown Name");
						string text12 = pallet2.Author ?? "Unknown Author";
						string label5 = text11 + " | " + text12;
						CreateOptionButton(label5, delegate
						{
							avirez = spawnyreference2;
						});
					}
				}
			}
			if (currentPage2 > 0)
			{
				CreateOptionButton("Previous Page", delegate
				{
					currentPage2--;
				});
			}
			if (currentPage2 < num13 - 1)
			{
				CreateOptionButton("Next Page", delegate
				{
					currentPage2++;
				});
			}
			AddSection(310f);
			FreezeScrolling();
			AvatarCrate avatarCrate4 = avirez?.Crate;
			Pallet pallet3 = avatarCrate4?.Pallet;
			if (avatarCrate4?.name != null)
			{
				string text13 = StripColorTags(avatarCrate4.name);
				string text14 = pallet3?.Author ?? "Unknown Author";
				string text15 = pallet3?.name ?? "Unknown Pallet";
				string barcode6 = avatarCrate4.Barcode?.ID ?? "Unknown ID";
				CreateLabel("Selected Avatar Information : " + text13 + " | " + text14, 700f);
				blockavatarfunctions(new AvatarCrateReference(barcode6));
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.CustomAviFav:
		{
			CreateTextbox(ref avifavsearcher, "Custom Avatar Favorites Searcher :");
			if (CustomAvFavref != null)
			{
				foreach (AvatarCrateReference avi in CustomAvFavref)
				{
					AvatarCrate avatarCrate = avi?.Crate;
					if (avatarCrate?.name == null)
					{
						continue;
					}
					string name = avatarCrate.name;
					string value6 = avifavsearcher ?? string.Empty;
					if (string.IsNullOrWhiteSpace(value6) || name.Contains(value6, StringComparison.OrdinalIgnoreCase))
					{
						CreateOptionButton(StripColorTags(name), delegate
						{
							avifavorite = avi;
						});
					}
				}
			}
			AddSection(310f);
			FreezeScrolling();
			AvatarCrate avatarCrate2 = avifavorite?.Crate;
			Pallet pallet = avatarCrate2?.Pallet;
			string barcode3 = avatarCrate2?.Barcode?.ID;
			if (avatarCrate2 != null && pallet != null)
			{
				blockavatarfunctions(new AvatarCrateReference(barcode3));
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.CustomSpawnableFav:
		{
			CreateTextbox(ref spawnablefavsearcher, "Custom Spawnable Favorites Searcher :");
			if (CustomSpawnFavref != null)
			{
				foreach (SpawnableCrateReference avi2 in CustomSpawnFavref)
				{
					SpawnableCrate spawnableCrate = avi2?.Crate;
					if (spawnableCrate?.name == null)
					{
						continue;
					}
					string name2 = spawnableCrate.name;
					string value10 = spawnablefavsearcher ?? string.Empty;
					if (string.IsNullOrWhiteSpace(value10) || name2.Contains(value10, StringComparison.OrdinalIgnoreCase))
					{
						CreateOptionButton(StripColorTags(name2), delegate
						{
							spawnyfavorite = avi2;
						});
					}
				}
			}
			AddSection(310f);
			FreezeScrolling();
			SpawnableCrateReference spawnableCrateReference2 = spawnyfavorite;
			if (spawnableCrateReference2?.Crate != null)
			{
				bloockwarnkickspawnables(spawnableCrateReference2);
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.PlayerInformation:
		{
			CreateLabel("Players : ");
			System.Collections.Generic.HashSet<NetworkPlayer> hashSet = NetworkPlayers();
			if (hashSet != null)
			{
				foreach (NetworkPlayer player2 in hashSet)
				{
					if (player2 == null)
					{
						continue;
					}
					CreateOptionButton(player2.PlayerID.IsHost ? ("[HOST] " + CleanedNAME(player2)) : CleanedNAME(player2), delegate
					{
						storeynow = player2;
						string text18 = (storeynow?.PlayerID)?.PlatformID.ToString().Trim();
						if (NetworkInfo.IsHost)
						{
							playersspawnrefs.Clear();
							if (text18 != null && PlayerSpawningStuff != null && PlayerSpawningStuff.TryGetValue(text18, out var value11) && value11 != null)
							{
								foreach (string item5 in value11)
								{
									if (item5 != null)
									{
										playersspawnrefs.Add(new SpawnableCrateReference(item5));
									}
								}
							}
						}
						playersavatarrefs.Clear();
						if (text18 != null && PlayeravatarStuff != null && PlayeravatarStuff.TryGetValue(text18, out var value12) && value12 != null)
						{
							foreach (string item6 in value12)
							{
								if (item6 != null)
								{
									playersavatarrefs.Add(new AvatarCrateReference(item6));
								}
							}
						}
					});
				}
			}
			AddSection(310f);
			FreezeScrolling();
			if (storeynow == null)
			{
				CreateLabel("No valid player selected.", 700f);
				break;
			}
			CreateLabel("Player Options : ");
			if (NetworkInfo.IsHost)
			{
				string item = storeynow.PlayerID.PlatformID.ToString();
				CreateLabel(blockedspawnies.Contains(item) ? "Server Spawn/Despawn Blocked : YES" : "Server Spawn/Despawn Blocked : NO", 700f);
				CreateLabel(voicblocked.Contains(item) ? "Server Muted : YES" : "Server Muted : NO", 700f);
				CreateLabel(blockedplatformids.Contains(item) ? "Server Damage Blocked : YES" : "Server Damage Blocked : NO", 700f);
				CreateLabel(blockmessages.Contains(item) ? "Server Block Players Messaging : YES" : "Server Block Players Messaging : NO", 700f);
				CreateLabel(blockmovements.Contains(item) ? "Server Disable Movement Syncing : YES" : "Server Disable Movement Syncing : NO", 700f);
				CreateOptionButton("Add/Remove To Server Block Player Messaging", delegate
				{
					string text18 = storeynow?.ND_SteamID().ToString();
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, blockmessages, blockmessagingnowpath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(storeynow) + " To Block Player Messaging [Server]!.", "Removed " + CleanedNAME(storeynow) + " From Block Player Messaging [Server]!.");
					}
				});
				CreateOptionButton("Add/Remove To Server Voice Blocker", delegate
				{
					string text18 = storeynow?.ND_SteamID().ToString();
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, voicblocked, voicepathblocked, "Nemesis Anti-Cheat", "Added " + CleanedNAME(storeynow) + " To Voice Blocker [Server]!.", "Removed " + CleanedNAME(storeynow) + " From Voice Blocker [Server]!.");
					}
				});
				CreateOptionButton("Add/Remove To Server Damage Blocker", delegate
				{
					string text18 = storeynow?.ND_SteamID().ToString();
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, blockedplatformids, DamageBlockPath, "Nemesis Anti-Cheat", "Added " + SpawnLogsRef.BarcodeID + " To Damage Blocker!.", "Removed " + SpawnLogsRef.BarcodeID + " From Damage Blocker!.");
					}
				});
				CreateOptionButton("Add/Remove To Server Spawn/Despawn Blocker", delegate
				{
					string text18 = storeynow?.ND_SteamID().ToString();
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, blockedspawnies, ServerBlockSpawnPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(storeynow) + " To Server Spawn/Despawn Blocker [Server]!.", "Removed " + CleanedNAME(storeynow) + " From Server Spawn/Despawn Blocker [Server]!.");
					}
				});
				CreateOptionButton("Add/Remove To Server Disable Movement Syncing", delegate
				{
					string text18 = storeynow?.ND_SteamID().ToString();
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, blockmovements, BlockMovementsPath, "Nemesis Anti-Cheat", "Added " + CleanedNAME(storeynow) + " To Server Disable Movement Syncing [Server]!.", "Removed " + CleanedNAME(storeynow) + " From Server Disable Movement Syncing [Server]!.");
					}
				});
			}
			FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
			if (FusionPermissions.HasSufficientPermissions(level, PermissionLevel.OWNER))
			{
				CreateOptionButton("[Owner] Clear Constraints", delegate
				{
					if (storeynow != null)
					{
						ClearConstraints(storeynow);
					}
				});
			}
			CreateOptionButton("Add/Remove Avatar To Avatar Blocker", delegate
			{
				if (storeynow?.ND_PlayersAvatarBarcodeID() != "c3534c5a-94b2-40a4-912a-24a8506f6c79")
				{
					ToggleAddRemoveFromFile(storeynow?.ND_PlayersAvatarBarcodeID(), blockedavifallbacks, avatarsblocked, "Nemesis Anti-Cheat", "Added " + storeynow?.ND_PlayersAvatarBarcodeID().ND_BarcodeCrateName() + " To Avatar Blocker!.", "Removed " + storeynow?.ND_PlayersAvatarBarcodeID().ND_BarcodeCrateName() + " From Avatar Blocker!.");
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", "Can't Do That!", NotificationType.ERROR, 2.5f);
				}
			});
			CreateOptionButton("Clone Avatar", delegate
			{
				string text18 = storeynow?.ND_PlayersAvatarBarcodeID();
				if (text18 != null)
				{
					ChangeIntoAvi(text18);
				}
			});
			CreateOptionButton("Copy Avatar Details", delegate
			{
				string text18 = storeynow?.ND_PlayersAvatarBarcodeID();
				if (text18 != null)
				{
					SpawnableCrateReference spawnableCrateReference3 = new SpawnableCrateReference(text18);
					GUIUtility.systemCopyBuffer = $"Barcode : {spawnableCrateReference3?.Barcode?.ID}\nCrate : {spawnableCrateReference3?.Crate?.name} Author [{spawnableCrateReference3?.Crate?.Pallet?.Author}]\nPallet It's In : {spawnableCrateReference3?.Crate?.Pallet?.name}";
				}
			});
			CreateOptionButton("Local Protection Avi", delegate
			{
				RigManager rigManager = storeynow?.RigRefs?.RigManager;
				if (rigManager != null)
				{
					rigManager.SwapAvatarCrate(new Barcode("c3534c5a-94b2-40a4-912a-24a8506f6c79"));
					if (storeynow?.MarrowEntity != null)
					{
						SpawnEffects.CallDespawnEffect(storeynow.MarrowEntity);
					}
				}
			});
			CreateOptionButton("UnBan/Ban From Your Lobbies", delegate
			{
				PlayerID pid = storeynow?.PlayerID;
				if (pid != null)
				{
					if (!NetworkHelper.IsBanned(pid.PlatformID))
					{
						BanInfo item2 = new BanInfo
						{
							Player = new PlayerInfo
							{
								Username = pid.Metadata?.Username?.GetValue(),
								Nickname = pid.Metadata?.Nickname?.GetValue(),
								PlatformID = pid.PlatformID,
								Description = pid.Metadata?.Description?.GetValue(),
								AvatarModID = pid.Metadata.AvatarModID.GetValue(),
								AvatarTitle = pid.Metadata?.AvatarTitle?.GetValue()
							},
							Reason = "Manually Banned [Nemesis Anti-Cheat]"
						};
						if (BanManager.BanList?.Bans != null)
						{
							BanManager.BanList.Bans.RemoveAll((BanInfo info2) => info2?.Player?.PlatformID == pid.PlatformID);
							BanManager.BanList.Bans.Add(item2);
							DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
						}
						NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
					}
					else
					{
						BanManager.Pardon(pid.PlatformID);
						NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
					}
				}
			});
			CreateOptionButton("Copy All Profile To Clipboard", delegate
			{
				if (storeynow != null)
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						WriteIndented = true
					};
					string systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(storeynow.PlayerID, options);
					GUIUtility.systemCopyBuffer = systemCopyBuffer;
					NotificationNow("Nemesis Anti-Cheat", "Copied Players Entire Details To Clipboard", NotificationType.SUCCESS);
				}
			});
			CreateOptionButton("Copy Steam ID", delegate
			{
				ulong? num17 = storeynow?.PlayerID?.PlatformID;
				if (num17.HasValue)
				{
					GUIUtility.systemCopyBuffer = num17.ToString();
					NotificationNow("Nemesis Anti-Cheat", "Copied Steam ID", NotificationType.SUCCESS);
				}
			});
			CreateOptionButton("Open Steam Profile", delegate
			{
				CheckSteamID(storeynow.PlayerID.PlatformID);
			});
			SerializedAvatarStats serializedAvatarStats = storeynow?.ND_SerializedAvatarStats();
			if (serializedAvatarStats != null)
			{
				CreateLabel("Current Health : " + storeynow.RigRefs.Health.curr_Health.ToString("F0") + "/" + storeynow.RigRefs.Health.max_Health.ToString("F0"));
				CreateLabel("Current Death Time : " + storeynow.RigRefs.Health.currDeathTime.ToString("F0"));
			}
			AddSection(310f);
			string text7 = (string.IsNullOrWhiteSpace(storeynow.ND_Nickname()) ? storeynow.ND_Username() : (StripColorTags(storeynow.ND_Nickname()) + " | " + storeynow.ND_Username())) ?? "Player";
			PlayerID playerID3 = storeynow.PlayerID;
			if (playerID3 != null && playerID3.IsValid)
			{
				CreateLabel(text7 + " Information : ");
				testholder[0] = storeynow.ND_PlayersAvatarBarcodeID() ?? "";
				CreateTextbox(ref testholder[0], "Players Avatar : ");
				blockavatarfunctions(new AvatarCrateReference(storeynow.ND_PlayersAvatarBarcodeID()));
				testholder[1] = storeynow.ND_SteamID().ToString() ?? "";
				CreateTextbox(ref testholder[1], "Steam ID : ");
				testholder[2] = storeynow.ND_GetHand(WhichHand.Left)?.ND_GetMarrowEntity()?.ND_GetBarcodeID() ?? "";
				CreateTextbox(ref testholder[2], "Left Handed Item : ");
				bloockwarnkickspawnables(new SpawnableCrateReference(storeynow.ND_GetHand(WhichHand.Left)?.ND_GetMarrowEntity()?.ND_GetBarcodeID()));
				testholder[3] = storeynow.ND_GetHand(WhichHand.Right)?.ND_GetMarrowEntity()?.ND_GetBarcodeID() ?? "";
				CreateTextbox(ref testholder[3], "Right Handed Item : ");
				bloockwarnkickspawnables(new SpawnableCrateReference(storeynow.ND_GetHand(WhichHand.Right)?.ND_GetMarrowEntity()?.ND_GetBarcodeID()));
			}
			AddSection(310f);
			if (NetworkInfo.IsHost)
			{
				CreateLabel(text7 + " Spawns : ");
				if (playersspawnrefs.Count == 0)
				{
					CreateLabel("No spawnables found for this player.", 700f);
				}
				else
				{
					playerinfospawnlogs = playerinfospawnlogs ?? "";
					CreateTextbox(ref playerinfospawnlogs, "Search Spawn Logs : ");
					foreach (SpawnableCrateReference barcode4 in playersspawnrefs)
					{
						if (barcode4?.Crate == null)
						{
							continue;
						}
						string text8 = StripColorTags(barcode4.Crate.name) ?? "";
						if (text8.ToLower().Contains(playerinfospawnlogs.ToLower()))
						{
							CreateOptionButton(text8 + " | " + (barcode4.Crate.Pallet?.Author ?? "Unknown"), delegate
							{
								playersspawnrefsstored = barcode4;
							});
						}
					}
				}
				AddSection(310f);
				FreezeScrolling();
				if (playersspawnrefsstored?.Crate != null)
				{
					bloockwarnkickspawnables(playersspawnrefsstored);
				}
				else
				{
					CreateLabel("Nothing selected", 700f);
				}
				AddSection(310f);
				FreezeScrolling();
			}
			CreateLabel(text7 + " Avatar Switch Logs : ");
			if (playersavatarrefs.Count == 0)
			{
				CreateLabel("No avatars found for this player.", 700f);
			}
			else
			{
				playerinfoavatarswitch = playerinfoavatarswitch ?? "";
				CreateTextbox(ref playerinfoavatarswitch, "Search Avatar Switch Logs : ");
				foreach (AvatarCrateReference barcode5 in playersavatarrefs)
				{
					if (barcode5?.Crate == null)
					{
						continue;
					}
					string text9 = StripColorTags(barcode5.Crate.name) ?? "";
					if (text9.ToLower().Contains(playerinfoavatarswitch.ToLower()))
					{
						CreateOptionButton(text9 + " | " + (barcode5.Crate.Pallet?.Author ?? "Unknown"), delegate
						{
							playersavatarrefsstored = barcode5;
						});
					}
				}
			}
			AddSection(310f);
			FreezeScrolling();
			if (playersavatarrefsstored?.Crate != null)
			{
				AvatarCrate crate = playersavatarrefsstored.Crate;
				CreateLabel("Selected Avatar Information : " + StripColorTags(crate.name ?? "") + " | " + (crate.Pallet?.Author ?? "Unknown"), 700f);
				CreateLabel($"Mod IO : {CrateFilterer.GetModID(crate.Pallet)}", 600f);
				CreateLabel("Barcode ID : " + crate.Barcode?.ID, 600f);
				CreateLabel("Pallet Author : " + crate.Pallet?.Author, 600f);
				CreateLabel("Pallet Name : " + crate.Pallet?.name, 600f);
				CreateOptionButton("Copy Details To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = $"Avatar Information : {crate.Pallet?.name}\nMod IO : {crate.Barcode?.ID}\nBarcode ID : {crate.Barcode?.ID}\nPallet Author : {crate.Pallet?.Author}\nPallet Name : {crate.Pallet?.name}";
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Copy Barcode To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = crate.Barcode?.ID;
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Change Into Avatar", delegate
				{
					string text18 = crate.Barcode?.ID;
					if (text18 != null)
					{
						ChangeIntoAvi(text18);
					}
				});
				CreateOptionButton("Add/Remove Kick If Changed Into", delegate
				{
					string text18 = crate.Barcode?.ID;
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, AvatarsKick, AvatarsKickPath, "Nemesis Anti-Cheat", "Added " + text18 + " To Kick If Changed Into!.", "Removed " + text18 + " From Kick If Changed Into!.");
					}
				});
				CreateOptionButton("Add/Remove Custom Favorites [Avatar]", delegate
				{
					string text18 = crate.Barcode?.ID;
					if (text18 != null)
					{
						ToggleAddRemoveFromFile(text18, CustomAvFav, AvatarCustomFav, "Nemesis Anti-Cheat", "Added " + text18 + " To Custom Avatar Favorites!.", "Removed " + text18 + " From Custom Avatar Favorites!.");
					}
				});
			}
			else
			{
				CreateLabel("Nothing selected", 700f);
			}
			break;
		}
		case MenuSections.SceneEntities:
		{
			FreezeScrolling();
			CreateLabel("Network Entities : " + ListNetworkEntities.Count);
			CreateTextbox(ref netentities, "Network Entities Searcher :");
			System.Collections.Generic.List<NetworkEntity> list3 = NetworkEntities().Where(delegate(NetworkEntity e)
			{
				SpawnableCrateReference spawnableCrateReference3 = new SpawnableCrateReference(e.ND_GetMarrowEntity().ND_GetBarcodeID());
				return spawnableCrateReference3?.Crate?.Pallet != null && StripColorTags(spawnableCrateReference3.Crate.name).ToLower().Contains(netentities.ToLower());
			}).ToList();
			int num8 = Mathf.CeilToInt((float)list3.Count / (float)itemsPerPage);
			int num9 = currentPage3 * itemsPerPage;
			int num10 = Mathf.Min(num9 + itemsPerPage, list3.Count);
			for (int num11 = num9; num11 < num10; num11++)
			{
				NetworkEntity barcodeid = list3[num11];
				SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(barcodeid.ND_GetMarrowEntity().ND_GetBarcodeID());
				string label3 = StripColorTags(spawnableCrateReference.Crate.name) + " | " + spawnableCrateReference.Crate.Pallet.Author;
				CreateOptionButton(label3, delegate
				{
					nettyspawnedc = barcodeid;
				});
			}
			if (currentPage3 > 0)
			{
				CreateOptionButton("Previous Page", delegate
				{
					currentPage3--;
				});
			}
			if (currentPage3 < num8 - 1)
			{
				CreateOptionButton("Next Page", delegate
				{
					currentPage3++;
				});
			}
			AddSection(310f);
			FreezeScrolling();
			CreateLabel("Network Entity Options : ");
			if (!(nettyspawnedc.ND_GetMarrowEntity() != null))
			{
				break;
			}
			CreateOptionButton("Despawn This", delegate
			{
				DespawnNow(nettyspawnedc);
			});
			CreateOptionButton("Force Delete This", delegate
			{
				MarrowEntity marrowEntity = nettyspawnedc?.ND_GetMarrowEntity();
				if (marrowEntity != null)
				{
					marrowEntity.gameObject.DestroyNow();
					NotificationNow("Nemesis Anti-Cheat", "Force Deleted Locally " + marrowEntity.ND_GetBarcodeID().ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
				}
			});
			CreateOptionButton("Spawn This", delegate
			{
				SpawnIt(nettyspawnedc.ND_GetMarrowEntity().ND_GetBarcodeID(), ND_YourGetHand(WhichHand.Left).transform.position + ND_YourGetHand(WhichHand.Left).transform.forward + ND_YourGetHand(WhichHand.Left).transform.up, Quaternion.identity);
			});
			AddSection(310f);
			FreezeScrolling();
			bloockwarnkickspawnables(new SpawnableCrateReference(nettyspawnedc.ND_GetMarrowEntity().ND_GetBarcodeID()));
			break;
		}
		case MenuSections.PlayerSeriStats:
		{
			CreateLabel("Players Avatar Stats Information :");
			foreach (NetworkPlayer playei in NetworkPlayers())
			{
				CreateOptionButton(CleanedNAME(playei), delegate
				{
					storedplz = playei;
				});
			}
			if (storedplz == null)
			{
				break;
			}
			FieldInfo[] fields = storedplz.ND_SerializedAvatarStats().GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(float))
				{
					float num7 = (float)fieldInfo.GetValue(storedplz.ND_SerializedAvatarStats());
					CreateLabel(fieldInfo.Name + " :" + num7);
				}
			}
			break;
		}
		case MenuSections.DamageLogger:
			CreateLabel("Damage Information :");
			CreateTextbox(ref damagelogsearcher, "Damage Information Searcher :");
			{
				foreach (System.Collections.Generic.KeyValuePair<PlayerID, System.Collections.Generic.List<PlayerRepDamageData>> playerDamageLog in ProtectionFromClients.PlayerDamageLogs)
				{
					PlayerID key = playerDamageLog.Key;
					System.Collections.Generic.List<PlayerRepDamageData> value3 = playerDamageLog.Value;
					if (!NetworkPlayerManager.TryGetPlayer(key, out var player) || player == null)
					{
						continue;
					}
					string text5 = StripColorTags(CleanedNAME(player.ND_Nickname(), player.ND_Username())).ToLower();
					string value4 = damagelogsearcher.ToLower();
					if (!text5.Contains(value4))
					{
						continue;
					}
					System.Collections.Generic.IEnumerable<IGrouping<float, PlayerRepDamageData>> enumerable = from d in value3
						where d?.Attack?.Attack != null
						group d by d.Attack.Attack.damage;
					foreach (IGrouping<float, PlayerRepDamageData> item7 in enumerable)
					{
						float key2 = item7.Key;
						int value5 = item7.Count();
						CreateLabel($"Attacker: {StripColorTags(CleanedNAME(player.ND_Nickname(), player.ND_Username()))} | Damage: {key2} x{value5}", 1000f);
					}
				}
				break;
			}
		case MenuSections.NetworkLogger:
			CreateLabel("Network Message Logger :");
			{
				foreach (var (playerID2, list2) in despawnresponselogger)
				{
					CreateLabel($"Despawn Request/Response Message Called : {CleanedNAME(playerID2.Metadata.Nickname.GetValue(), playerID2.Metadata.Username.GetValue())} ({playerID2.PlatformID}) x{list2.Count}", 2000f);
				}
				break;
			}
		case MenuSections.ServerHistory:
			if (ServerHistorys != null)
			{
				foreach (LobbyInfo lobbyInfoNow in ServerHistorys)
				{
					if (lobbyInfoNow != null && !string.IsNullOrWhiteSpace(lobbyInfoNow.LobbyCode))
					{
						string label = (string.IsNullOrWhiteSpace(lobbyInfoNow.LobbyName) ? (lobbyInfoNow.LobbyHostName + "'s Server") : lobbyInfoNow.LobbyName);
						CreateOptionButton(label, delegate
						{
							storedlobby = lobbyInfoNow;
						});
					}
				}
			}
			AddSection(310f);
			FreezeScrolling();
			CreateLabel("Lobby History Information : ");
			ServerInfoNow(storedlobby);
			break;
		case MenuSections.FusionBanManager:
			CreateLabel("Fusion Ban Manager : ");
			CreateOptionButton("Export Ban List To Clipboard", delegate
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					FloatFormatHandling = FloatFormatHandling.Symbol,
					Culture = CultureInfo.InvariantCulture,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(BanManager.BanList.Bans, settings);
				NotificationNow("Nemesis Anti-Cheat", "Exported Ban List To Clipboard", NotificationType.WARNING, 3.5f);
			});
			CreateOptionButton("Import Bans From Clipboard", delegate
			{
				string systemCopyBuffer = GUIUtility.systemCopyBuffer;
				if (!string.IsNullOrEmpty(systemCopyBuffer))
				{
					BanList banList = JsonConvert.DeserializeObject<BanList>(systemCopyBuffer);
					if (banList?.Bans != null && banList.Bans.Count != 0)
					{
						System.Collections.Generic.HashSet<ulong> hashSet2 = new System.Collections.Generic.HashSet<ulong>();
						foreach (BanInfo ban2 in BanManager.BanList.Bans)
						{
							hashSet2.Add(ban2.Player.PlatformID);
						}
						int num17 = 0;
						foreach (BanInfo ban3 in banList.Bans)
						{
							if (ban3?.Player != null && hashSet2.Add(ban3.Player.PlatformID))
							{
								BanManager.BanList.Bans.Add(ban3);
								num17++;
							}
						}
						NotificationNow("Nemesis Anti-Cheat", $"Imported New Bans [{num17}]", NotificationType.WARNING, 3.5f);
						DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
					}
				}
			});
			CreateLabel("Bans Count :" + BanManager.BanList.Bans.Count);
			CreateTextbox(ref bansearcher, "Bans Searcher :");
			foreach (BanInfo ban in BanManager.BanList.Bans)
			{
				string text16 = StripColorTags(ban.Player.Nickname);
				string username = ban.Player.Username;
				if (string.IsNullOrWhiteSpace(bansearcher.ToLower()))
				{
					CreateOptionButton(string.IsNullOrWhiteSpace(text16) ? username : (text16 + " | " + username), delegate
					{
						storedban = ban;
					});
				}
				else if (username.ToLower().Contains(bansearcher.ToLower()) || text16.ToLower().Contains(bansearcher.ToLower()))
				{
					CreateOptionButton(string.IsNullOrWhiteSpace(text16) ? username : (text16 + " | " + username), delegate
					{
						storedban = ban;
					});
				}
			}
			AddSection(310f);
			if (storedban != null)
			{
				string text17 = StripColorTags(storedban.Player.Nickname);
				string username2 = storedban.Player.Username;
				string labelText = (string.IsNullOrWhiteSpace(text17) ? ("Selected Player : " + username2) : ("Selected Player : " + text17 + " | " + username2));
				CreateLabel(labelText);
				CreateOptionButton("UN-BAN", delegate
				{
					NetworkHelper.PardonUser(storedban.Player.PlatformID);
				});
				CreateOptionButton("Open Steam Page", delegate
				{
					CheckSteamID(storedban.Player.PlatformID);
				});
				CreateOptionButton("Copy Ban Details To Clipboard", delegate
				{
					JsonSerializerSettings settings = new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						FloatFormatHandling = FloatFormatHandling.Symbol,
						Culture = CultureInfo.InvariantCulture,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					};
					GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(storedban, settings);
					NotificationNow("Nemesis Anti-Cheat", "Copied Ban Info To Clipboard For Player " + labelText, NotificationType.WARNING, 3.5f);
				});
			}
			else
			{
				CreateLabel("Nothing");
			}
			break;
		case MenuSections.DownloadLogger:
		{
			CreateLabel("Mod.IO Download Logger : ");
			if (LogDownloads.DownloadLogger == null)
			{
				break;
			}
			foreach (System.Collections.Generic.KeyValuePair<LobbyInfo, System.Collections.Generic.List<Pallet>> item8 in LogDownloads.DownloadLogger)
			{
				LobbyInfo key3 = item8.Key;
				System.Collections.Generic.List<Pallet> value7 = item8.Value;
				foreach (Pallet item9 in value7)
				{
					if (!(item9 == null))
					{
						string value8 = (string.IsNullOrWhiteSpace(item9.Title) ? "Unknown Mod" : StripColorTags(item9.Title));
						int modID = CrateFilterer.GetModID(item9);
						Pallet capturedPallet = item9;
						LobbyInfo capturedLobby = key3;
						CreateOptionButton($"{value8} | Mod.IO #ID {modID}", delegate
						{
							modiostorednow = capturedPallet;
							lobbyinfofrominstall = capturedLobby;
						});
					}
				}
			}
			AddSection(310f);
			CreateLabel("Mod.IO Options : " + modiostorednow.name);
			Barcode id = modiostorednow.Barcode;
			string text6 = StripColorTags(modiostorednow.name);
			string author2 = modiostorednow.Author;
			CreateLabel($"Barcode ID : {id}", 600f);
			CreateLabel("Pallet Author : " + author2, 600f);
			CreateLabel("Pallet Name : " + text6, 600f);
			CreateLabel((modidblocked != null && modidblocked.Contains(CrateFilterer.GetModID(modiostorednow).ToString())) ? "Blocked Mod.IO Mod : YES" : "Blocked Mod.IO Mod : NO");
			CreateLabel((blockentirepallet != null && blockentirepallet.Contains(modiostorednow.name)) ? "Blocked Pallet : YES" : "Blocked Pallet : NO");
			CreateOptionButton("Copy Details To Clipboard", delegate
			{
				GUIUtility.systemCopyBuffer = $"Spawnable Searcher Information : {modiostorednow?.name}\nMod IO : {CrateFilterer.GetModID(modiostorednow)}\nPallet Author : {modiostorednow?.Author}\nPallet Name : {modiostorednow?.name}";
				NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
			});
			CreateOptionButton("Copy Barcode To Clipboard", delegate
			{
				GUIUtility.systemCopyBuffer = id.ID;
				NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
			});
			CreateOptionButton("Open Mod Download Folder", delegate
			{
				GetPalletFolder(modiostorednow?.name, openSelectionPc: true);
			});
			CreateOptionButton($"Open Page Mod.IO #ID {CrateFilterer.GetModID(modiostorednow)}", delegate
			{
				OpenPageNow($"https://mod.io/search/mods/{CrateFilterer.GetModID(modiostorednow)}");
			});
			CreateOptionButton("Add/Remove Block Pallet Completely", delegate
			{
				ToggleAddRemoveFromFile(modiostorednow.name, blockentirepallet, blockpalletnowlist, "Nemesis Anti-Cheat", "Added " + modiostorednow.name + " To Blocked Pallets!.", "Removed " + modiostorednow.name + " From Blocked Pallets!.");
			});
			CreateOptionButton("Add/Remove Block This Mod.IO Mod Completely", delegate
			{
				ToggleAddRemoveFromFile(CrateFilterer.GetModID(modiostorednow).ToString(), modidblocked, ModIDBLOCKSPATH, "Nemesis Anti-Cheat", "Added " + modiostorednow.name + " To Blocked Mod.IO Mod!.", "Removed " + modiostorednow.name + " From Blocked Mod.IO Mod!.");
			});
			CreateOptionButton("Delete Mod Completely", delegate
			{
				DeleteModioMod(modiostorednow);
			});
			if (lobbyinfofrominstall == null)
			{
				break;
			}
			AddSection(310f);
			CreateLabel("Mod.IO Downloads In Server Information : " + modiostorednow.name);
			ServerInfoNow(lobbyinfofrominstall);
			LogDownloads.DownloadLogger.TryGetValue(lobbyinfofrominstall, out var value9);
			CreateLabel("Mod.IO Downloaded In Server Information : " + value9.Count);
			CreateOptionButton("Delete All Mods From Server", delegate
			{
				foreach (System.Collections.Generic.KeyValuePair<LobbyInfo, System.Collections.Generic.List<Pallet>> item10 in LogDownloads.DownloadLogger)
				{
					LobbyInfo key4 = item10.Key;
					if (key4 == lobbyinfofrominstall)
					{
						System.Collections.Generic.List<Pallet> value11 = item10.Value;
						foreach (Pallet item11 in value11)
						{
							DeleteModioMod(item11, notif: false);
						}
						NotificationNow("Nemesis Anti-Cheat", "Deleted " + value11.Count + " Mods From This Server!", NotificationType.SUCCESS, 6.5f);
					}
				}
			});
			break;
		}
		case MenuSections.MediaPlayerLogger:
			CreateLabel("Media Player Logger : " + MediaPlayerLogs.Count);
			foreach (string medialink in MediaPlayerLogs)
			{
				CreateOptionButton(medialink, delegate
				{
					storedmedia = medialink;
				});
			}
			if (!string.IsNullOrEmpty(storedmedia))
			{
				AddSection(310f);
				CreateLabel("Media Player Logger Options : " + storedmedia);
				CreateLabel((MEDIAPLAYERBLOCKERNOWList != null && MEDIAPLAYERBLOCKERNOWList.Contains(storedmedia)) ? "Media Player Blocked : YES" : "Media Player Blocked : NO");
				CreateOptionButton("Add/Remove Media Player Blocker", delegate
				{
					ToggleAddRemoveFromFile(storedmedia, MEDIAPLAYERBLOCKERNOWList, MEDIAPLAYERBLOCKERNOW, "Nemesis Anti-Cheat", "Added " + storedmedia + " To Media Player Blocker!.", "Removed " + storedmedia + " From Media Player Blocker!.");
				});
				CreateOptionButton("Copy Link To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = storedmedia;
					NotificationNow("Nemesis Anti-Cheat", "Copied " + storedmedia + " To Clipboard!", NotificationType.SUCCESS, 2f);
				});
			}
			break;
		}
		void AddSection(float offtoadd)
		{
			xOffset += offtoadd;
			y = baseY - scroll;
		}
		void CreateLabel(string text18, float width = 300f)
		{
			GUI.Label(new Rect(xOffset, y, width, 30f), text18);
			y += 35f;
		}
		void CreateOptionButton(string text18, Action act, float spacing = 35f, float width = 300f, float height = 30f)
		{
			Rect position = new Rect(xOffset, y, width, height);
			if (GUI.Button(position, text18, buttonStyle))
			{
				act?.Invoke();
			}
			y += spacing;
		}
		void CreateTextbox(ref string reference2, string text18, float spacing = 35f, float widthcustom = 300f)
		{
			Rect position = new Rect(xOffset, y, 300f, 25f);
			GUI.Label(position, text18);
			y += spacing;
			Rect position2 = new Rect(xOffset, y, widthcustom, 25f);
			reference2 = GUI.TextField(position2, reference2);
			y += spacing;
		}
		void FreezeScrolling()
		{
			y = 20f;
		}
		void ServerInfoNow(LobbyInfo StoredInformation)
		{
			if (StoredInformation != null)
			{
				CreateLabel("Lobby Code : " + StoredInformation.LobbyCode, 1000f);
				CreateLabel("Host Name : " + StoredInformation.LobbyHostName, 1000f);
				CreateLabel("Level Title : " + StoredInformation.LevelTitle, 1000f);
				CreateLabel("Level Barcode : " + StoredInformation.LevelBarcode, 1000f);
				CreateLabel("Level Mod IO #ID : " + StoredInformation.LevelModID, 1000f);
				CreateLabel("Lobby ID : " + StoredInformation.LobbyID, 1000f);
				CreateLabel("Players :");
				PlayerInfo[] players = StoredInformation.PlayerList.Players;
				foreach (PlayerInfo playerInfo in players)
				{
					CreateLabel($" Nickname : {StripColorTags(playerInfo.Nickname)} | Username : {playerInfo.Username} | Steam ID : {playerInfo.PlatformID}", 1000f);
				}
				if (StoredInformation.Privacy == ServerPrivacy.PUBLIC)
				{
					CreateOptionButton("Join Server", delegate
					{
						NetworkHelper.JoinServerByCode(StoredInformation.LobbyCode);
					});
				}
				CreateOptionButton("Copy Lobby Details To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = DataToJsonString(StoredInformation);
					NotificationNowAlways("Nemesis Anti-Cheat", "Copied Lobby Info To Clipboard!", NotificationType.SUCCESS, 3.5f);
				});
				CreateOptionButton("Download Map", delegate
				{
					DownloadModIOMod(StoredInformation.LevelModID);
				});
			}
			else
			{
				CreateLabel("No lobby selected.");
			}
		}
		static void SpawnerRESULTS(string switchname)
		{
			MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly, enableLogging: false));
			SpawnablerResultsNow.Clear();
			Il2CppSystem.Collections.Generic.List<Pallet> list4 = AssetWarehouse.Instance?.GetPallets();
			if (list4 != null)
			{
				Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator22 = list4.GetEnumerator();
				while (enumerator22.MoveNext())
				{
					Pallet current7 = enumerator22.Current;
					if (current7?.Crates != null)
					{
						Il2CppSystem.Collections.Generic.List<Crate>.Enumerator enumerator23 = current7.Crates.GetEnumerator();
						while (enumerator23.MoveNext())
						{
							Crate current8 = enumerator23.Current;
							if (!(current8 == null) && !IsMagazine(current8))
							{
								string value11 = SpawnableSearchies?.ToLower() ?? "";
								string text18 = "";
								switch (switchname)
								{
								case "cratename":
									text18 = StripColorTags(current8.name?.ToLower()) ?? "";
									break;
								case "palletname":
									text18 = StripColorTags(current8.Pallet.name?.ToLower()) ?? "";
									break;
								case "barcode":
									text18 = StripColorTags(current8.Barcode.ID?.ToLower()) ?? "";
									break;
								}
								if (text18.Contains(value11))
								{
									SpawnablerResultsNow.Add(current8.Barcode?.ID);
								}
							}
						}
					}
				}
				currentPage = 0;
			}
		}
		void blockavatarfunctions(AvatarCrateReference avatarresult)
		{
			if (avatarresult != null && !(avatarresult.Crate == null) && !(avatarresult.Crate.Barcode == null))
			{
				string id2 = avatarresult.Crate.Barcode.ID;
				Pallet pallet4 = avatarresult.Crate.Pallet;
				string palletname = StripColorTags(avatarresult.Crate.Pallet.name);
				string palletauthor = avatarresult.Crate.Pallet.Author;
				CreateLabel($"Mod IO : {CrateFilterer.GetModID(pallet4)}", 600f);
				CreateLabel("Barcode ID : " + id2, 600f);
				CreateLabel("Pallet Author : " + palletauthor, 600f);
				CreateLabel("Pallet Name : " + palletname, 600f);
				CreateOptionButton("Open Mod Download Folder", delegate
				{
					GetPalletFolder(avatarresult.Crate.Pallet.name, openSelectionPc: true);
				});
				CreateOptionButton("Change Into Avatar", delegate
				{
					ChangeIntoAvi(id2);
				});
				CreateOptionButton("Copy Details To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = $"Avatar Information : {palletname}\nMod IO : {CrateFilterer.GetModID(pallet4)}\nBarcode ID : {id2}\nPallet Author : {palletauthor}\nPallet Name : {palletname}";
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Copy Barcode To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = id2;
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Add/Remove Kick If Changed Into", delegate
				{
					ToggleAddRemoveFromFile(id2, AvatarsKick, AvatarsKickPath, "Nemesis Anti-Cheat", "Added " + id2 + " To Kick If Changed Into!.", "Removed " + id2 + " From Kick If Changed Into!.");
				});
				CreateOptionButton("Add/Remove Avatar To Avatar Blocker", delegate
				{
					if (id2 != "c3534c5a-94b2-40a4-912a-24a8506f6c79")
					{
						ToggleAddRemoveFromFile(id2, blockedavifallbacks, avatarsblocked, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To Avatar Blocker!.", "Removed " + id2.ND_BarcodeCrateName() + " From Avatar Blocker!.");
					}
					else
					{
						NotificationNow("Nemesis Anti-Cheat", "Can't Do That!", NotificationType.ERROR, 2.5f);
					}
				});
				CreateOptionButton("Add/Remove Block Avatar Pallet", delegate
				{
					ToggleAddRemoveFromFile(palletname, blockavipalletlist, BlockPalletAviNowp, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodePalletName() + " To Block Avatar Pallet!.", "Removed " + id2.ND_BarcodePalletName() + " From Block Avatar Pallet!.");
				});
				CreateOptionButton("Add/Remove Block Avatar Author", delegate
				{
					ToggleAddRemoveFromFile(palletauthor, blockaviauthorlist, BlockAviAuthorNowp, "Nemesis Anti-Cheat", "Added " + palletauthor + " To Block Avatar Author!.", "Removed " + palletauthor + " From Block Avatar Author!.");
				});
				CreateOptionButton("Add/Remove Warn Avatar Change", delegate
				{
					ToggleAddRemoveFromFile(id2, warnavilist, WarnAvisNow, "Nemesis Anti-Cheat", "Added " + id2 + " To Warn Avatar Change!.", "Removed " + id2 + " From Warn Avatar Change!.");
				});
				CreateOptionButton("Add/Remove Custom Favorites [Avatar]", delegate
				{
					ToggleAddRemoveFromFile(id2, CustomAvFav, AvatarCustomFav, "Nemesis Anti-Cheat", "Added " + id2 + " To Custom Avatar Favorites!.", "Removed " + id2 + " From Custom Avatar Favorites!.");
				});
				CreateOptionButton("Delete Mod Completely", delegate
				{
					DeleteModioMod(pallet4);
				});
			}
		}
		void bloockwarnkickspawnables(SpawnableCrateReference serresult)
		{
			if (serresult != null && !(serresult.Crate == null) && !(serresult.Crate.Barcode == null))
			{
				string id2 = serresult.Crate.Barcode.ID;
				Pallet pallet4 = serresult.Crate.Pallet;
				string palletname = StripColorTags(serresult.Crate.Pallet.name);
				string palletauthor = serresult.Crate.Pallet.Author;
				CreateLabel("Selected Spawnable Information : " + StripColorTags(serresult.Crate.name) + " | " + palletauthor, 700f);
				CreateLabel($"Mod IO : {CrateFilterer.GetModID(serresult.Crate.Pallet)}", 600f);
				CreateLabel("Barcode ID : " + id2, 600f);
				CreateLabel("Pallet Author : " + palletauthor, 600f);
				CreateLabel("Pallet Name : " + palletname, 600f);
				CreateLabel((SpawnablesKick != null && SpawnablesKick.Contains(id2)) ? "Kick If Spawned : YES" : "Kick If Spawned : NO");
				CreateLabel((BlockedSpawnables != null && BlockedSpawnables.Contains(id2)) ? "Blocked Spawn : YES" : "Blocked Spawn : NO");
				CreateLabel((WarnedSpawnables != null && WarnedSpawnables.Contains(id2)) ? "Warn Spawn : YES" : "Warn Spawn : NO");
				CreateLabel((blockentirepallet != null && blockentirepallet.Contains(palletname)) ? "Blocked Pallet : YES" : "Blocked Pallet : NO");
				CreateLabel((blockentireauthor != null && blockentireauthor.Contains(palletauthor)) ? "Blocked Author : YES" : "Blocked Author : NO");
				CreateLabel((modidblocked != null && modidblocked.Contains(CrateFilterer.GetModID(pallet4).ToString())) ? "Blocked Mod.IO Mod : YES" : "Blocked Mod.IO Mod : NO");
				CreateLabel(DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Contains(id2) ? "Favorited Spawnable : YES" : "Favorited Spawnable : NO");
				CreateOptionButton("Open Mod Download Folder", delegate
				{
					GetPalletFolder(serresult.Crate.Pallet.name, openSelectionPc: true);
				});
				CreateOptionButton("Despawn All Of This Item", delegate
				{
					foreach (NetworkEntity item12 in NetworkEntities())
					{
						MarrowEntity marrowEntity = item12?.ND_GetMarrowEntity();
						if (marrowEntity != null && marrowEntity.ND_GetBarcodeID() == id2)
						{
							item12.ND_Despawn();
						}
					}
					NotificationNow("Nemesis Anti-Cheat", "Despawned Everything Matching " + id2.ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
				});
				CreateOptionButton("Force Delete All Of This Locally", delegate
				{
					foreach (NetworkEntity item13 in NetworkEntities())
					{
						MarrowEntity marrowEntity = item13?.ND_GetMarrowEntity();
						if (marrowEntity != null && marrowEntity.ND_GetBarcodeID() == id2)
						{
							marrowEntity.gameObject.DestroyNow();
						}
					}
					NotificationNow("Nemesis Anti-Cheat", "Force Deleted Locally Everything Matching " + id2.ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
				});
				CreateOptionButton("Spawn This", delegate
				{
					Hand hand = ND_YourGetHand(WhichHand.Left);
					if (hand != null)
					{
						SpawnIt(id2, hand.transform.position + hand.transform.forward + hand.transform.up, Quaternion.identity);
					}
				});
				CreateOptionButton("Add/Remove Block Author Of Spawnable", delegate
				{
					ToggleAddRemoveFromFile(palletauthor, blockentireauthor, blockauthornowlist, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeAuthor() + " To Blocked Authors!.", "Removed " + id2.ND_BarcodeAuthor() + " From Blocked Authors!.");
				});
				CreateOptionButton("Add/Remove Block Pallet Completely", delegate
				{
					ToggleAddRemoveFromFile(palletname, blockentirepallet, blockpalletnowlist, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodePalletName() + " To Blocked Pallets!.", "Removed " + id2.ND_BarcodePalletName() + " From Blocked Pallets!.");
				});
				CreateOptionButton("Add/Remove Block This Mod.IO Mod Completely", delegate
				{
					ToggleAddRemoveFromFile(CrateFilterer.GetModID(pallet4).ToString(), modidblocked, ModIDBLOCKSPATH, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodePalletName() + " To Blocked Mod.IO Mod!.", "Removed " + id2.ND_BarcodePalletName() + " From Blocked Mod.IO Mod!.");
				});
				CreateOptionButton("Copy Details To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = $"Spawnable Searcher Information : {pallet4?.name}\nMod IO : {CrateFilterer.GetModID(pallet4)}\nBarcode ID : {id2}\nPallet Author : {pallet4?.Author}\nPallet Name : {pallet4?.name}";
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Copy Barcode To Clipboard", delegate
				{
					GUIUtility.systemCopyBuffer = id2;
					NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
				});
				CreateOptionButton("Add/Remove Kick If Spawned", delegate
				{
					ToggleAddRemoveFromFile(id2, SpawnablesKick, SpawnablesKickPath, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To Kick If Spawned!.", "Removed " + id2.ND_BarcodeCrateName() + " From Kick If Spawned!.");
				});
				CreateOptionButton("Block/UnBlock This Spawnable", delegate
				{
					ToggleAddRemoveFromFile(id2, BlockedSpawnables, BlockedSpawnablesPath, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To Blocked Spawnables!.", "Removed " + id2.ND_BarcodeCrateName() + " From Blocked Spawnables!.");
				});
				CreateOptionButton("Warn/UnWarn This Spawnable", delegate
				{
					ToggleAddRemoveFromFile(id2, WarnedSpawnables, WarnedSpawnablesPath, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To Warn Spawnables!.", "Removed " + id2.ND_BarcodeCrateName() + " From Warn Spawnables!.");
				});
				CreateOptionButton("Un/Favorite Spawnable", delegate
				{
					Il2CppSystem.Collections.Generic.List<string> list4 = DataManager.ActiveSave?.PlayerSettings?.FavoriteSpawnables;
					if (list4 != null)
					{
						if (!list4.Contains(id2))
						{
							list4.Add(id2);
							DataManager.TrySaveActiveSave(SaveFlags.Progression);
							NotificationNow("Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To SaveGame Favorites!\nReload Level For Effect!", NotificationType.SUCCESS, 4f);
						}
						else
						{
							list4.Remove(id2);
							DataManager.TrySaveActiveSave(SaveFlags.Progression);
							NotificationNow("Nemesis Anti-Cheat", "Removed " + id2.ND_BarcodeCrateName() + " From SaveGame Favorites!\nReload Level For Effect!", NotificationType.WARNING, 4f);
						}
					}
				});
				CreateOptionButton("Add/Remove Custom Favorites [Spawnable]", delegate
				{
					ToggleAddRemoveFromFile(id2, CustomSpawnFav, SpawnableCustomFav, "Nemesis Anti-Cheat", "Added " + id2.ND_BarcodeCrateName() + " To Custom Spawnable Favorites!.", "Removed " + id2.ND_BarcodeCrateName() + " From Custom Spawnable Favorites!.");
				});
				CreateOptionButton("Delete Mod Completely", delegate
				{
					DeleteModioMod(serresult.Crate.Pallet);
				});
			}
		}
	}

	public override void OnUpdate()
	{
		if (blockentireauthornow == null)
		{
			return;
		}
		if (NetworkInfo.HasLayer && Input.GetKeyDown(KeyCode.Y))
		{
			show = !show;
		}
		if (HelperMethods.IsLoading() || !RigData.HasPlayer)
		{
			return;
		}
		if (spawngunuialways)
		{
			if (!spawngunatleastonce && ((bool)ND_YourGetHand(WhichHand.Left).ND_HandGrabbedSpawnGun() || (bool)ND_YourGetHand(WhichHand.Right).ND_HandGrabbedSpawnGun()))
			{
				spawngunatleastonce = true;
			}
			if (spawngunatleastonce)
			{
				UIRig.Instance.popUpMenu.AddSpawnMenu();
			}
		}
		if (preventnotificationlag && MenuNotifications.SavedNotifications.Count > 400)
		{
			MenuNotifications.ClearNotifications();
			Notifier.Cancel("Nemesis Anti-Cheat");
			NotificationNow("Nemesis Anti-Cheat", "Cleared Notifications To Prevent Lag!", NotificationType.WARNING, 2.5f, showtitle: true, savetomenu: true);
		}
		foreach (NetworkPlayer play in NetworkPlayers())
		{
			if (play.PlayerID.Metadata.Loading.GetValue())
			{
				continue;
			}
			SpoofChecker spoofChecker = StoredJoinPlayers.FirstOrDefault((SpoofChecker pml) => pml.PlatformID == play.ND_SteamID());
			if (spoofChecker == null)
			{
				continue;
			}
			string text = play.ND_Username() ?? "Unknown";
			ulong platformID = spoofChecker.PlatformID;
			if (LastKnownUsernames.TryGetValue(platformID, out var value))
			{
				if (!(value != text))
				{
					continue;
				}
				if (spoofedusernameusername)
				{
					NotificationNowAlways("Nemesis Anti-Cheat", $"Person Doing : {CleanedNAME(play)}\n[Spoofed Username] '{value}' -> '{text}'", NotificationType.WARNING, 3.5f, showtitle: true, savetomenu: true, delegate
					{
						CheckSteamID(platformID);
					});
				}
				LastKnownUsernames[platformID] = text;
				if (AutoKickSpoofers)
				{
					FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var level, out var _);
					if (FusionPermissions.HasSufficientPermissions(level, LobbyInfoManager.LobbyInfo.Kicking))
					{
						NetworkHelper.KickUser(play.PlayerID);
					}
				}
			}
			else
			{
				LastKnownUsernames[platformID] = text;
			}
		}
		if (showammoalways)
		{
			Player.UIRig.uiHud.SHOWAMMO(UI_HUD.AmmoDisplayLocation.Head);
			Player.UIRig.uiHud.HEADHUDFOLLOW(followHead: true);
		}
		if (bodylog)
		{
			ND_BodyLog(Player.PhysicsRig).bodylogreturn?.ballArt.gameObject.SetActive(value: false);
			ND_BodyLog(Player.PhysicsRig).Outerring.gameObject.SetActive(value: false);
		}
		if (GamemodeManager.IsGamemodeStarted)
		{
			return;
		}
		if (personalspace)
		{
			foreach (NetworkPlayer item in NetworkPlayers(excludeMe: true))
			{
				GameObject gameObject = item?.RigRefs?.RigManager?.avatar?.gameObject;
				Transform closetoobject = Player.PhysicsRig?.m_head?.transform;
				gameObject?.SetActive(!(gameObject?.IsTooClose(closetoobject, personalspacevalue) ?? false));
			}
		}
		if (autorunnow)
		{
			InvokeWithType(typeof(SmashBones), "ApplyAutoRun", new object[1] { Player.RigManager });
		}
		if (NetworkInfo.IsHost)
		{
			if (AntiDevManip)
			{
				foreach (NetworkEntity item2 in NetworkEntities())
				{
					if ((item2?.ND_GetMarrowEntity())?.ND_GetBarcodeID() == "c1534c5a-c6a8-45d0-aaa2-2c954465764d")
					{
						DespawnNow(item2);
					}
				}
			}
			if (AntiLasereyes)
			{
				foreach (NetworkEntity item3 in NetworkEntities())
				{
					if (item3.ND_GetMarrowEntity().ND_GetBarcodeID() == "BamBaeYoh.LaserEyes.Spawnable.LaserEyes")
					{
						DespawnNow(item3);
					}
				}
			}
		}
		if (AreYouOWNER())
		{
			if (despawndeadnpcs)
			{
				foreach (NetworkEntity item4 in NetworkEntities())
				{
					if (item4 != null && item4.ND_IsNPC() && item4 != null && item4.ND_GetNPCAIBrain()?.isDead == true)
					{
						item4?.ND_Despawn();
					}
				}
			}
			if (fpsdesapwner && fps <= fpslimit && !fpsCheckRunning)
			{
				MelonCoroutines.Start(FPSCheck());
			}
			if (godmode)
			{
				Player.RigManager?.health?.ResetHits();
			}
		}
		if (TeleportThresHold)
		{
			PhysicsRig physicsRig = Player.RigManager?.physicsRig;
			if (physicsRig != null)
			{
				float magnitude = physicsRig._wholeBodyVelocity_k__BackingField.magnitude;
				if (magnitude >= speedthreshold)
				{
					LocalPlayer.TeleportToCheckpoint();
				}
			}
		}
		BaseController baseController = RightController();
		BaseController baseController2 = LeftController();
		if (baseController != null && baseController2 != null && baseController.GetBButton() && baseController2.GetBButton())
		{
			timertodo++;
			if (!tpback10seconds)
			{
				if (timertodo >= ((fps < 20f) ? 30 : 150))
				{
					LocalPlayer.TeleportToCheckpoint();
					NotificationNow("Nemesis Anti-Cheat", "Emergency Escape To Spawn!", NotificationType.WARNING, 2f);
					timertodo = 0;
				}
			}
			else if (timertodo >= ((fps < 20f) ? 30 : 150))
			{
				LocalPlayer.TeleportToPosition(Seconds10back);
				NotificationNow("Nemesis Anti-Cheat", $"Emergency Escape {timerfoeesa} Seconds Back!", NotificationType.WARNING, 2f);
				timertodo = 0;
			}
		}
		else if (baseController == null || !baseController.GetBButton())
		{
			timertodo = 0;
		}
		if (baseController != null && baseController2 != null && baseController.GetAButton() && baseController2.GetAButton())
		{
			timeravisafe++;
			if (timeravisafe >= ((fps < 20f) ? 30 : 250))
			{
				ChangeIntoAvi("c3534c5a-94b2-40a4-912a-24a8506f6c79");
				NotificationNow("Nemesis Anti-Cheat", "Emergency Avatar!", NotificationType.WARNING, 2f);
				timeravisafe = 0;
			}
		}
		else if (baseController == null || !baseController.GetAButton())
		{
			timeravisafe = 0;
		}
		if (NetworkInfo.IsHost)
		{
			if (baseController != null && baseController2 != null && baseController.GetThumbStick() && baseController2.GetThumbStick())
			{
				timerreloadlevel++;
				if (timerreloadlevel >= ((fps < 20f) ? 50 : 700))
				{
					SceneStreamer.Reload();
					NotificationNow("Nemesis Anti-Cheat", "Emergency Reload Level!", NotificationType.WARNING, 2f);
					timerreloadlevel = 0;
				}
			}
			else if (baseController == null || !baseController.GetThumbStick())
			{
				timerreloadlevel = 0;
			}
		}
		if (AreYouOPERATOR())
		{
			if (dashingnow)
			{
				InvokeWithType(typeof(SmashBones), "ApplyDashing", new object[1] { Player.RigManager });
			}
			if (doublejumpnow)
			{
				InvokeWithType(typeof(SmashBones), "ApplyDoubleJump", new object[1] { Player.RigManager });
			}
			if (Aircontrolnow && !ND_YourGetHand(WhichHand.Left).ND_IsGrabbedNimbusGun() && !ND_YourGetHand(WhichHand.Right).ND_IsGrabbedNimbusGun())
			{
				InvokeWithType(typeof(SmashBones), "ApplyAirControl", new object[1] { Player.RigManager });
			}
			if (selfconstraint)
			{
				LocalPlayer.ClearConstraints();
			}
		}
		if (AntiGravityChange)
		{
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}
	}

	public override void OnApplicationStart()
	{
		if (!Directory.Exists(NACsavetxt))
		{
			Directory.CreateDirectory(NACsavetxt);
		}
		if (!File.Exists(RECNETLYMETLOGGED))
		{
			File.WriteAllText(RECNETLYMETLOGGED, "");
		}
		if (!File.Exists(MEDIAPLAYERLOGS))
		{
			File.WriteAllText(MEDIAPLAYERLOGS, "");
		}
		if (!File.Exists(LOBBIESLOGGEDSINCE))
		{
			File.WriteAllText(LOBBIESLOGGEDSINCE, "");
		}
		if (!File.Exists(PLAYERSLOGGEDSINCE))
		{
			File.WriteAllText(PLAYERSLOGGEDSINCE, "");
		}
		if (!File.Exists(BlockMovementsPath))
		{
			File.WriteAllText(BlockMovementsPath, "");
		}
		if (!File.Exists(WarnAvisNow))
		{
			File.WriteAllText(WarnAvisNow, "");
		}
		if (!File.Exists(BlockAviAuthorNowp))
		{
			File.WriteAllText(BlockAviAuthorNowp, "");
		}
		if (!File.Exists(BlockPalletAviNowp))
		{
			File.WriteAllText(BlockPalletAviNowp, "");
		}
		if (!File.Exists(homeworldnow))
		{
			File.WriteAllText(homeworldnow, "c2534c5a-80e1-4a29-93ca-f3254d656e75");
		}
		if (!File.Exists(BlockedSpawnablesPath))
		{
			File.WriteAllText(BlockedSpawnablesPath, "");
		}
		if (!File.Exists(WarnedSpawnablesPath))
		{
			File.WriteAllText(WarnedSpawnablesPath, "");
		}
		if (!File.Exists(ProtectorSettings))
		{
			File.WriteAllText(ProtectorSettings, "");
		}
		if (!File.Exists(SpawnablesKickPath))
		{
			File.WriteAllText(SpawnablesKickPath, "");
		}
		if (!File.Exists(AvatarsKickPath))
		{
			File.WriteAllText(AvatarsKickPath, "");
		}
		if (!File.Exists(SpawnableCustomFav))
		{
			File.WriteAllText(SpawnableCustomFav, "");
		}
		if (!File.Exists(AvatarCustomFav))
		{
			File.WriteAllText(AvatarCustomFav, "");
		}
		if (!File.Exists(MEDIAPLAYERBLOCKERNOW))
		{
			File.WriteAllText(MEDIAPLAYERBLOCKERNOW, "");
		}
		if (!File.Exists(DamageBlockPath))
		{
			File.WriteAllText(DamageBlockPath, "");
		}
		if (!File.Exists(CreateCheatToolsPreset.devitems))
		{
			File.WriteAllText(CreateCheatToolsPreset.devitems, "");
		}
		if (!File.Exists(CreateCheatToolsPreset.devitemscurrent))
		{
			File.WriteAllText(CreateCheatToolsPreset.devitemscurrent, "{\r\n  \"TitleOfPreset\": \"Default\",\r\n  \"Item1\": {\r\n    \"BarcodeId\": \"c1534c5a-5747-42a2-bd08-ab3b47616467\",\r\n    \"SpawnableName\": \"Spawn Gun\",\r\n    \"LocalSpawn\": false\r\n  },\r\n  \"Item2\": {\r\n    \"BarcodeId\": \"c1534c5a-6b38-438a-a324-d7e147616467\",\r\n    \"SpawnableName\": \"Nimbus Gun\",\r\n    \"LocalSpawn\": false\r\n  },\r\n  \"Item3\": {\r\n    \"BarcodeId\": \"Empty\",\r\n    \"SpawnableName\": \"Empty\",\r\n    \"LocalSpawn\": false\r\n  },\r\n  \"Item4\": {\r\n    \"BarcodeId\": \"Empty\",\r\n    \"SpawnableName\": \"Empty\",\r\n    \"LocalSpawn\": false\r\n  },\r\n  \"Item5\": {\r\n    \"BarcodeId\": \"Empty\",\r\n    \"SpawnableName\": \"Empty\",\r\n    \"LocalSpawn\": false\r\n  }\r\n}");
		}
		if (!File.Exists(BodyLogPage.Bodylogpages))
		{
			File.WriteAllText(BodyLogPage.Bodylogpages, "");
		}
		if (!File.Exists(TeleporterManager.teleportmanager))
		{
			File.WriteAllText(TeleporterManager.teleportmanager, "");
		}
		if (!File.Exists(InventoryPage.PresetsFile))
		{
			File.WriteAllText(InventoryPage.PresetsFile, "");
		}
		if (!File.Exists(InventoryPage.PresetsFileCurrent))
		{
			File.WriteAllText(InventoryPage.PresetsFileCurrent, "");
		}
		if (!File.Exists(ModIDBLOCKSPATH))
		{
			File.WriteAllText(ModIDBLOCKSPATH, "");
		}
		if (!File.Exists(voicepathblocked))
		{
			File.WriteAllText(voicepathblocked, "");
		}
		if (!File.Exists(ServerBlockSpawnPath))
		{
			File.WriteAllText(ServerBlockSpawnPath, "");
		}
		if (!File.Exists(blockmessagingnowpath))
		{
			File.WriteAllText(blockmessagingnowpath, "");
		}
		if (!File.Exists(BodyLogRadialMenuColorPreset.ColorsCurrent))
		{
			File.WriteAllText(BodyLogRadialMenuColorPreset.ColorsCurrent, "[ \r\n   \"0\",\r\n  \"255\",\r\n  \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n    \"0\",\r\n  \"255\",\r\n]");
		}
		if (!File.Exists(BodyLogRadialMenuColorPreset.ColorsPresets))
		{
			File.WriteAllText(BodyLogRadialMenuColorPreset.ColorsPresets, "");
		}
		if (!File.Exists(StatsKickerPresets.StatsKickerCurrent))
		{
			File.WriteAllText(StatsKickerPresets.StatsKickerCurrent, "[\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\",\r\n  \"1000\"\r\n]");
		}
		if (!File.Exists(StatsKickerPresets.StatsKickerPresetsNow))
		{
			File.WriteAllText(StatsKickerPresets.StatsKickerPresetsNow, "");
		}
		if (!File.Exists(FusionProfilePresets.PresetsPath))
		{
			File.WriteAllText(FusionProfilePresets.PresetsPath, "");
		}
		if (!File.Exists(avatarsblocked))
		{
			File.WriteAllText(avatarsblocked, "");
		}
		if (!File.Exists(spawnlimitshostonly))
		{
			File.WriteAllText(spawnlimitshostonly, "");
		}
		if (!File.Exists(blockpalletnowlist))
		{
			File.WriteAllText(blockpalletnowlist, "");
		}
		if (!File.Exists(blockauthornowlist))
		{
			File.WriteAllText(blockauthornowlist, "");
		}
		ReloadList();
	}

	public override void OnInitializeMelon()
	{
		string path = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods");
		string path2 = Path.Combine(path, "MontanaClient.dll");
		string path3 = Path.Combine(path, "Client.dll");
		if (File.Exists(path2) || File.Exists(path3))
		{
			MelonLogger.Warning("Detected a client in Mods folder. Quitting application...");
			Application.Quit();
		}
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>
		{
			spawnlimitshostonly, avatarsblocked, homeworldnow, PalletDumpLocation, BlockedSpawnablesPath, WarnedSpawnablesPath, SpawnablesKickPath, AvatarsKickPath, SpawnableCustomFav, blockpalletnowlist,
			blockauthornowlist, AvatarCustomFav, DamageBlockPath, ServerBlockSpawnPath, blockmessagingnowpath, ModIDBLOCKSPATH, voicepathblocked, WarnAvisNow, BlockAviAuthorNowp, BlockPalletAviNowp,
			permissionshere, MEDIAPLAYERBLOCKERNOW
		};
		foreach (string item4 in list)
		{
			WatchFileChanges(item4);
		}
		NetworkLayer.OnLoggedInEvent += delegate
		{
			try
			{
				if (SteamIdYours() == 0)
				{
					MelonLogger.Warning($"Stored Original Steam ID : {SteamIdYours()}");
					SiteStuff.GlobalBanChecking();
				}
				if (AvatarsStored.Count == 0)
				{
					new SimpleTimer(delegate
					{
						if (NetworkInfo.HasLayer)
						{
							NetworkLayer layer = NetworkLayerManager.Layer;
							if (layer != null)
							{
								IMatchmaker matchmaker = layer.Matchmaker;
								if (matchmaker != null)
								{
									LabFusion.Marrow.Proxies.StringElement searchBarElement = MenuMatchmaking.SearchBarElement;
									MenuPage browserPage = MenuMatchmaking.BrowserPage;
									if (!(searchBarElement == null) && !(browserPage == null) && !(searchBarElement.gameObject == null) && !(browserPage.gameObject == null) && !searchBarElement.gameObject.active && !browserPage.gameObject.active)
									{
										matchmaker.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo info)
										{
											if (info.Lobbies != null && info.Lobbies.Any())
											{
												IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
												for (int i = 0; i < lobbies.Length; i++)
												{
													IMatchmaker.LobbyInfo item = lobbies[i];
													ulong title = item.Metadata.LobbyInfo.LobbyID;
													string code = item.Metadata.LobbyInfo.LobbyHostName;
													if (!CachedLobbies.Any((IMatchmaker.LobbyInfo x) => x.Metadata.LobbyInfo.LobbyID == title && x.Metadata.LobbyInfo.LobbyHostName == code))
													{
														CachedLobbies.Add(item);
													}
													PlayerInfo[] players = item.Metadata.LobbyInfo.PlayerList.Players;
													foreach (PlayerInfo p in players)
													{
														if (!PlayersOnline.Any((PlayerInfo x) => x.PlatformID == p.PlatformID))
														{
															PlayersOnline.Add(new PlayerInfo
															{
																Username = StripColorTags(p.Username),
																PlatformID = p.PlatformID,
																Nickname = p.Nickname,
																Description = p.Description,
																PermissionLevel = p.PermissionLevel,
																AvatarModID = p.AvatarModID,
																AvatarTitle = p.AvatarTitle
															});
														}
													}
												}
											}
										});
									}
								}
							}
						}
					}, 0f).Start(quicker: true, 6f);
					new SimpleTimer(delegate
					{
						if (autosavenow)
						{
							ManuallySave(notify: false);
						}
					}, 0f).Start(quicker: true, 7f);
					new SimpleTimer(delegate
					{
						if (SteamIdYours() != 0)
						{
							SiteStuff.GlobalBanChecking();
						}
					}, 0f).Start(quicker: true, 180f);
					EmergencyEscapetimer = new SimpleTimer(delegate
					{
						if (tpback10seconds)
						{
							Transform transform = Player.RigManager?.physicsRig?.feet?.transform;
							if (!(transform == null))
							{
								Seconds10back = transform.position;
							}
						}
					}, timerfoeesa).Start(quicker: true, timerfoeesa);
					new SimpleTimer(delegate
					{
						if (Time.unscaledDeltaTime > 0f)
						{
							fps = 1f / Time.unscaledDeltaTime;
						}
					}, 0f).Start(quicker: true, 0.1f);
					DespawnAllTimera = new SimpleTimer(delegate
					{
						if (DespawnAllTimer && !HelperMethods.IsLoading() && NetworkInfo.HasServer)
						{
							DespawnAll(DespawnerTimerAllReal);
						}
					}, DespawnAllTimerMins).Start();
					new SimpleTimer(delegate
					{
						if (hideholstersplayers)
						{
							System.Collections.Generic.HashSet<NetworkPlayer> hashSet = NetworkPlayers(excludeMe: true);
							if (hashSet != null)
							{
								foreach (NetworkPlayer item5 in hashSet)
								{
									if (item5 != null)
									{
										try
										{
											HolsterHiderAll(item5);
										}
										catch
										{
										}
									}
								}
							}
						}
						if (hideholsters)
						{
							try
							{
								HolsterHiderAll(null);
							}
							catch
							{
							}
						}
						if (bodylogplayers)
						{
							System.Collections.Generic.HashSet<NetworkPlayer> hashSet2 = NetworkPlayers(excludeMe: true);
							if (hashSet2 != null)
							{
								foreach (NetworkPlayer item6 in hashSet2)
								{
									if (item6 != null)
									{
										PhysicsRig physicsRig = item6.RigRefs?.RigManager?.physicsRig;
										if (!(physicsRig == null))
										{
											PullCordDevice item = ND_BodyLog(physicsRig).bodylogreturn;
											MeshRenderer item2 = ND_BodyLog(physicsRig).Outerring;
											if (item != null)
											{
												try
												{
													item.ballLine?.gameObject?.SetActive(value: false);
													item.ballArt?.gameObject?.SetActive(value: false);
												}
												catch
												{
												}
											}
											item2?.gameObject?.SetActive(value: false);
										}
									}
								}
							}
						}
					}, 0f).Start(quicker: true, 5f);
					new SimpleTimer(delegate
					{
						SpawnInventoryRefresh();
					}, 0f).Start(quicker: true, 0.5f);
					new SimpleTimer(delegate
					{
						if (unlammo && !GamemodeManager.IsGamemodeStarted && !HelperMethods.IsLoading())
						{
							LocalInventory.SetAmmo(10000);
						}
					}, 0f).Start(quicker: true, 2f);
					MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly));
					CreateProtectorUI();
					originalbodylog = DataManager.ActiveSave.PlayerSettings.FavoriteAvatars;
					StringBuilder stringBuilder = new StringBuilder();
					for (int num = 0; num < originalbodylog.Count; num++)
					{
						string text = originalbodylog[num];
						stringBuilder.Append(text ?? "NULL");
						if (num < originalbodylog.Count - 1)
						{
							stringBuilder.Append(", ");
						}
					}
					MelonLogger.Warning("Stored Original Bodylog: " + stringBuilder.ToString());
					originalprofiledetails = new PlayerInfo
					{
						Username = LocalPlayer.Metadata.Username.GetValue(),
						Nickname = LocalPlayer.Metadata.Nickname.GetValue(),
						Description = LocalPlayer.Metadata.Description.GetValue(),
						AvatarModID = LocalPlayer.Metadata.AvatarModID.GetValue(),
						AvatarTitle = LocalPlayer.Metadata.AvatarTitle.GetValue(),
						PermissionLevel = PermissionLevel.DEFAULT,
						PlatformID = SteamIdYours()
					};
					MelonLogger.Warning("Stored Original Profile Details:\nUsername: " + originalprofiledetails.Username + "\nNickname: " + originalprofiledetails.Nickname + "\nDescription: " + originalprofiledetails.Description + "\n" + $"AvatarModID: {originalprofiledetails.AvatarModID}\n" + "AvatarTitle: " + originalprofiledetails.AvatarTitle + "\n" + $"PermissionLevel: {originalprofiledetails.PermissionLevel}\n" + $"PlatformID: {originalprofiledetails.PlatformID}");
				}
			}
			catch
			{
			}
		};
		Hooking.OnLevelLoaded += delegate
		{
			if (NetworkPlayers().Count == 0)
			{
				ProtectionFromClients.PlayerDamageLogs.Clear();
				if (spawngunuialways)
				{
					spawngunatleastonce = false;
				}
				if (aviswitchprotection)
				{
					OnSwapAvatarPatch3._hasSkippedInitialSwap = false;
				}
				PageNow = MenuSections.SelfCat;
				if (KEEPLOADOUTINVENTORY)
				{
					MelonCoroutines.Start(KeepLoadOut());
				}
				if (modomatonload)
				{
					SpawnIt("Atlas.96.ModOMat.Spawnable.ModOMatPortable", Player.RigManager.physicsRig.feet.transform.position + Player.RigManager.physicsRig.feet.transform.forward, Quaternion.identity, localonly: true);
				}
				Seconds10back = Player.RigManager.physicsRig.feet.transform.position;
			}
		};
		MultiplayerHooking.OnTargetLevelLoaded += delegate
		{
			if (NetworkPlayers().Count > 0)
			{
				ProtectionFromClients.PlayerDamageLogs.Clear();
				if (spawngunuialways)
				{
					spawngunatleastonce = false;
				}
				if (aviswitchprotection)
				{
					OnSwapAvatarPatch3._hasSkippedInitialSwap = false;
				}
				Seconds10back = Player.RigManager.physicsRig.feet.transform.position;
				StoredJoinPlayers.Clear();
				if (!NetworkInfo.IsHost)
				{
					foreach (NetworkPlayer play in NetworkPlayers())
					{
						SpoofChecker item = new SpoofChecker
						{
							PlatformID = play.PlayerID.PlatformID,
							Username = play.PlayerID.Metadata.Username.GetValue()
						};
						StoredJoinPlayers.Add(item);
						NemesisAntiCheatControversialPeople playerfromfpcl = SiteStuff.fpcpeople.FirstOrDefault((NemesisAntiCheatControversialPeople result) => result.KnownSteamIds.Contains(play.PlayerID.PlatformID.ToString()) && play.PlayerID.PlatformID.ToString() != SteamIdYours().ToString());
						if (playerfromfpcl != null)
						{
							NotificationNowAlways("Controversial Person", $"User : {playerfromfpcl.FusionNicknameAtTheTime}\nReason : {playerfromfpcl.Reason}\nControversy Level : {playerfromfpcl.ControversyLevel}\nNote On Player : {CleanedNAME(play)}", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
							{
								playerfromfpcl.ShowOnPC();
							});
							SpawnEffects.CallDespawnEffect(play.MarrowEntity);
						}
						NemesisAntiCheatCommunityNotes playerfromcomnotes = SiteStuff.communitynotedplayers.FirstOrDefault((NemesisAntiCheatCommunityNotes result) => result.KnownSteamIds.Contains(play.PlayerID.PlatformID.ToString()) && play.PlayerID.PlatformID.ToString() != SteamIdYours().ToString());
						if (playerfromcomnotes != null)
						{
							NotificationNowAlways(playerfromcomnotes.FusionNicknameAtTheTime + " Community Note", playerfromcomnotes.Note + "\nNote On Player : " + CleanedNAME(play), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
							{
								playerfromcomnotes.ShowOnPC();
							});
							SpawnEffects.CallDespawnEffect(play.MarrowEntity);
						}
					}
				}
				PageNow = MenuSections.SelfCat;
				if (KEEPLOADOUTINVENTORY)
				{
					MelonCoroutines.Start(KeepLoadOut());
				}
				rejoinlastserver = LobbyInfoManager.LobbyInfo.LobbyCode;
				MelonLogger.Warning("Stored Lobby Code For Rejoining : " + rejoinlastserver);
				if (modomatonload)
				{
					SpawnIt("Atlas.96.ModOMat.Spawnable.ModOMatPortable", Player.RigManager.physicsRig.feet.transform.position + Player.RigManager.physicsRig.feet.transform.forward, Quaternion.identity, localonly: true);
				}
			}
		};
		MultiplayerHooking.OnDisconnected += delegate
		{
			kicktillrestart.Clear();
			SaveTXTFunc();
			PlayersListNow();
			playersavatarrefs.Clear();
			playersspawnrefs.Clear();
			ProtectionFromClients.lastSpawnTime = new System.Collections.Generic.Dictionary<ulong, float>();
			Notifier.Cancel("Nemesis Anti-Cheat");
			PageNow = MenuSections.SelfCat;
			if (cleandisconnect)
			{
				foreach (Poolee item7 in Resources.FindObjectsOfTypeAll<Poolee>())
				{
					Transform transform = item7?.transform?.root;
					if (!(transform == null))
					{
						MarrowEntity component = transform.GetComponent<MarrowEntity>();
						if (component != null)
						{
							component.Despawn();
						}
						else
						{
							transform.GetComponent<MarrowBody>()?.Entity.Despawn();
						}
					}
				}
			}
			StoredJoinPlayers.Clear();
			if (DeleteLastLobbyMods && LogDownloads.DeleteThesOnLeave.Count > 0)
			{
				foreach (Pallet item8 in LogDownloads.DeleteThesOnLeave)
				{
					DeleteModioMod(item8, notif: false);
				}
				NotificationNow("Nemesis Anti-Cheat", "Deleted " + LogDownloads.DeleteThesOnLeave.Count + " Mods From Last Lobby!", NotificationType.WARNING, 6.5f);
				LogDownloads.DeleteThesOnLeave.Clear();
			}
			ProtectionFromClients.PlayerDamageLogs.Clear();
			despawnresponselogger.Clear();
		};
		MultiplayerHooking.OnPlayerJoined += delegate
		{
			ProtectionFromClients.lastSpawnTime = new System.Collections.Generic.Dictionary<ulong, float>();
			PlayersListNow();
			SaveTXTFunc();
		};
		MultiplayerHooking.OnPlayerLeft += delegate
		{
			SaveTXTFunc();
			ProtectionFromClients.lastSpawnTime = new System.Collections.Generic.Dictionary<ulong, float>();
			PlayersListNow();
			System.Collections.Generic.HashSet<ulong> currentPlatformIDs = (from p in NetworkPlayers()
				select p.PlayerID.PlatformID).ToHashSet();
			StoredJoinPlayers.RemoveAll((SpoofChecker p) => !currentPlatformIDs.Contains(p.PlatformID));
		};
		NetworkSceneManager.OnPlayerLoadedIntoLevel += delegate(PlayerID now, string hh)
		{
			if (NetworkPlayerManager.TryGetPlayer(now, out var player))
			{
				SpoofChecker item = new SpoofChecker
				{
					PlatformID = now.PlatformID,
					Username = now.Metadata.Username.GetValue()
				};
				StoredJoinPlayers.Add(item);
				NemesisAntiCheatControversialPeople playerfromfpcl = SiteStuff.fpcpeople.FirstOrDefault((NemesisAntiCheatControversialPeople result) => result.KnownSteamIds.Contains(now.PlatformID.ToString()) && now.PlatformID.ToString() != SteamIdYours().ToString());
				if (playerfromfpcl != null)
				{
					NotificationNowAlways("Controversial Person", $"User : {playerfromfpcl.FusionNicknameAtTheTime}\nReason : {playerfromfpcl.Reason}\nControversy Level : {playerfromfpcl.ControversyLevel}\nNote On Player : {CleanedNAME(player)}", NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						playerfromfpcl.ShowOnPC();
					});
					SpawnEffects.CallDespawnEffect(player.MarrowEntity);
				}
				NemesisAntiCheatCommunityNotes playerfromcomnotes = SiteStuff.communitynotedplayers.FirstOrDefault((NemesisAntiCheatCommunityNotes result) => result.KnownSteamIds.Contains(now.PlatformID.ToString()) && now.PlatformID.ToString() != SteamIdYours().ToString());
				if (playerfromcomnotes != null)
				{
					NotificationNowAlways(playerfromcomnotes.FusionNicknameAtTheTime + " Community Note", playerfromcomnotes.Note + "\nNote On Player : " + CleanedNAME(player), NotificationType.WARNING, 6f, showtitle: true, savetomenu: true, delegate
					{
						playerfromcomnotes.ShowOnPC();
					});
					SpawnEffects.CallDespawnEffect(player.MarrowEntity);
				}
				SiteStuff.AltPrevention(now.PlatformID);
			}
		};
		MultiplayerHooking.OnTargetLevelLoaded += delegate
		{
			ServerHistorys.Add(LobbyInfoManager.LobbyInfo);
			if (!HideNemesisAntiCheat && !NetworkInfo.IsHost)
			{
				ProtectorPingData data = ProtectorPingData.Create(PlayerIDManager.LocalSmallID, "1.48.94");
				MessageRelay.RelayModule<ProtectorPingMessage, ProtectorPingData>(data, CommonMessageRoutes.ReliableToServer);
			}
		};
		MultiplayerHooking.OnJoinedServer += delegate
		{
			playersavatarrefs.Clear();
			playersspawnrefs.Clear();
			PlayerSpawningStuff.Clear();
			MelonCoroutines.Start(SiteStuff.UpdateSites());
			if (clientexploitclearonnewserver)
			{
				ProtectionLogs.RemoveAll();
				ClientExploitLogs.Clear();
			}
			if (spawnlogexploitclearonnewserver)
			{
				SpawnLogs.Clear();
			}
			if (switchlogexploitclearonnewserver)
			{
				PlayeravatarStuff.Clear();
			}
		};
		Menu.OnPageOpened += delegate(BoneLib.BoneMenu.Page pageaction)
		{
			BoneLib.BoneMenu.Page childPage = BoneLib.BoneMenu.Page.Root.GetChildPage("Montana Client");
			BoneLib.BoneMenu.Page childPage2 = BoneLib.BoneMenu.Page.Root.GetChildPage("Client");
			BoneLib.BoneMenu.Page page = childPage ?? childPage2;
			if (page != null)
			{
				BoneLib.BoneMenu.Page.Root.Name = "Uninstall Your Client [" + page.Name + "]!";
				BoneLib.BoneMenu.Page.Root.RemoveAll();
			}
			if (pageaction == OnlineFriends)
			{
				FriendLobbies();
			}
			if (pageaction == levelhistory)
			{
				levelhistory.RemoveAll();
				int num = 0;
				foreach (SearchHistoryEntry searchy in SearchHistorynow)
				{
					if (searchy.IsLevelCrate)
					{
						num++;
						levelhistory.CreateFunction(searchy.SearchText, Color.white, delegate
						{
							MelonCoroutines.Start(Search(searchy.SearchText, levelresults, searchy.Method, SearchMethodType.Level, delegate(string barcode)
							{
								LevelCrateReference levelCrateReference2 = new LevelCrateReference(barcode);
								SceneStreamer.Load(levelCrateReference2.Barcode);
							}));
						});
					}
				}
				NotificationNowAlways("Nemesis Anti-Cheat", $"Total Searches {num}", NotificationType.WARNING, 3.5f);
			}
			if (pageaction == avisearchhistory)
			{
				avisearchhistory.RemoveAll();
				int num2 = 0;
				foreach (SearchHistoryEntry searchy2 in SearchHistorynow)
				{
					if (searchy2.IsAvatar)
					{
						num2++;
						avisearchhistory.CreateFunction(searchy2.SearchText, Color.white, delegate
						{
							MelonCoroutines.Start(Search(searchy2.SearchText, aviresults, searchy2.Method, SearchMethodType.Avatar, delegate(string barcode)
							{
								AvatarsearcherLessCode(barcode);
							}));
						});
					}
				}
				NotificationNowAlways("Nemesis Anti-Cheat", $"Total Searches {num2}", NotificationType.WARNING, 3.5f);
			}
			if (pageaction == spawnablehistory)
			{
				int num3 = 0;
				spawnablehistory.RemoveAll();
				foreach (SearchHistoryEntry searchy3 in SearchHistorynow)
				{
					if (searchy3.IsSpawnable)
					{
						num3++;
						spawnablehistory.CreateFunction(searchy3.SearchText, Color.white, delegate
						{
							MelonCoroutines.Start(Search(searchy3.SearchText, spawnableresults, searchy3.Method, SearchMethodType.Spawnable, delegate(string barcode)
							{
								SpawnerFuncLessCode(barcode);
							}));
						});
					}
				}
				NotificationNowAlways("Nemesis Anti-Cheat", $"Total Searches {num3}", NotificationType.WARNING, 3.5f);
			}
			if (pageaction == permissioneditornow)
			{
				permissioneditornow.RemoveAll();
				foreach (Tuple<ulong, string, PermissionLevel> permittedUser in PermissionList.PermittedUsers)
				{
					ulong id = permittedUser.Item1;
					string username = permittedUser.Item2;
					PermissionLevel item = permittedUser.Item3;
					string text = new string(username.Where((char c) => !char.IsControl(c)).ToArray()).Trim();
					string name = (string.IsNullOrWhiteSpace(text) ? id.ToString() : text);
					permissioneditornow.CreateEnum(name, Color.yellow, item, delegate(Enum enabled)
					{
						permlevel = (PermissionLevel)(object)enabled;
						PermissionList.SetPermission(id, username, permlevel);
					});
				}
			}
			if (pageaction == WarnedSpawnablesnow)
			{
				PopulatePage(WarnedSpawnablesPath, WarnedSpawnables, WarnedSpawnablesnow);
			}
			if (pageaction == modidblockednow)
			{
				PopulatePage(ModIDBLOCKSPATH, modidblocked, modidblockednow);
			}
			if (pageaction == BlockedSpawnablesnow)
			{
				PopulatePage(BlockedSpawnablesPath, BlockedSpawnables, BlockedSpawnablesnow);
			}
			if (pageaction == SpawnablesKicknow)
			{
				PopulatePage(SpawnablesKickPath, SpawnablesKick, SpawnablesKicknow);
			}
			if (pageaction == blockentirepalletnow)
			{
				PopulatePage(blockpalletnowlist, blockentirepallet, blockentirepalletnow);
			}
			if (pageaction == blockentireauthornow)
			{
				PopulatePage(blockauthornowlist, blockentireauthor, blockentireauthornow);
			}
			if (pageaction == playerjoinlogsnow)
			{
				playerjoinlogsnow.RemoveAll();
				playerjoinlogsnow.CreateFunction("Copy All Players To Clipboard", Color.green, delegate
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						WriteIndented = true
					};
					GUIUtility.systemCopyBuffer = System.Text.Json.JsonSerializer.Serialize(JoinLogger, options);
				});
				foreach (PlayerInfo logger in JoinLogger)
				{
					string name2 = $"+ [{CleanedNAME(logger.Nickname, logger.Username)}] ({logger.PlatformID})";
					BoneLib.BoneMenu.Page page2 = playerjoinlogsnow.CreatePage(name2, Color.yellow);
					page2.CreateFunction("Open Steam Profile", Color.yellow, delegate
					{
						CheckSteamID(logger.PlatformID);
					});
					page2.CreateFunction("Copy Details To Clipboard", Color.yellow, delegate
					{
						GUIUtility.systemCopyBuffer = $"Nickname : {logger.Nickname}\nUsername : {logger.Username}\nSteamID : {logger.PlatformID}\nDescription : {logger.Description}\nAvatar Mod.io #ID : {logger.AvatarModID}";
						NotificationNow("Nemesis Anti-Cheat", "Copied player information to clipboard", NotificationType.INFORMATION, 2f);
					});
					page2.CreateFunction("Ban/Unban From Your Lobby", Color.yellow, delegate
					{
						if (NetworkHelper.IsBanned(logger.PlatformID))
						{
							BanManager.Pardon(logger.PlatformID);
							NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
						}
						else
						{
							BanManager.BanList.Bans.RemoveAll((BanInfo b) => b.Player.PlatformID == logger.PlatformID);
							BanManager.BanList.Bans.Add(new BanInfo
							{
								Player = new PlayerInfo
								{
									Username = logger.Username,
									Nickname = logger.Nickname,
									PlatformID = logger.PlatformID,
									Description = logger.Description,
									AvatarModID = logger.AvatarModID,
									AvatarTitle = logger.AvatarTitle
								},
								Reason = "Manually Banned [Nemesis Anti-Cheat]"
							});
							DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
							NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
						}
					});
				}
			}
			if (pageaction == AISpawnersPage)
			{
				AISpawnersPage.RemoveAll();
				BoneLib.BoneMenu.Page page3 = AISpawnersPage.CreatePage("+ All Spawners", Color.yellow);
				page3.CreateFunction("Pause All", Color.yellow, delegate
				{
					foreach (AISpawner item9 in NPCSpawnersNow)
					{
						item9.Pause(notification: false);
					}
					NotificationNow("Nemesis Anti-Cheat", "Paused All Spawners", NotificationType.SUCCESS, 2f);
				});
				page3.CreateFunction("Resume All", Color.yellow, delegate
				{
					foreach (AISpawner item10 in NPCSpawnersNow)
					{
						item10.Resume(notification: false);
					}
					NotificationNow("Nemesis Anti-Cheat", "Resumed All Spawners", NotificationType.SUCCESS, 2f);
				});
				page3.CreateFunction("Erase All", Color.yellow, delegate
				{
					Menu.DisplayDialog("Are you sure?", "This will erase all your spawners!", null, delegate
					{
						foreach (AISpawner item11 in NPCSpawnersNow)
						{
							item11.StopAndClear(notification: false);
						}
						NPCSpawnersNow.Clear();
						Menu.OpenPage(AISpawnersPage);
						NotificationNow("Nemesis Anti-Cheat", "Erased All Spawners", NotificationType.SUCCESS, 2f);
					});
				});
				page3.CreateFunction("Kill All In All Spawners", Color.yellow, delegate
				{
					foreach (AISpawner item12 in NPCSpawnersNow)
					{
						item12.KillAllInSpawner();
					}
					NotificationNow("Nemesis Anti-Cheat", "Kill All NPCs In All Spawners", NotificationType.SUCCESS, 2f);
				});
				page3.Logsettingsint("Max Entities", Color.green, ref maxentrefresh, 1, 1, 100, delegate(int intnow)
				{
					maxentrefresh = intnow;
					foreach (AISpawner item13 in NPCSpawnersNow)
					{
						item13.UpdateMaxSpawns(maxentrefresh);
					}
				});
				page3.Logsettingsfloat("Timer Seconds", Color.green, ref Timerrefresh, 1f, 1f, 100000f, delegate(float intnow)
				{
					Timerrefresh = intnow;
					foreach (AISpawner item14 in NPCSpawnersNow)
					{
						item14.UpdateSpawnInterval(Timerrefresh);
					}
				});
				BoneLib.BoneMenu.Page page4 = AISpawnersPage.CreatePage("+ Preset Spawners", Color.yellow);
				BoneLib.BoneMenu.Page page5 = AISpawnersPage.CreatePage("+ Custom Spawner", Color.yellow);
				page5.Logsettingsint("Max Entities", Color.green, ref cmaxentrefresh, 1, 1, 100, delegate(int intnow)
				{
					cmaxentrefresh = intnow;
				});
				page5.Logsettingsfloat("Timer Seconds", Color.green, ref cTimerrefresh, 1f, 1f, 100000f, delegate(float intnow)
				{
					cTimerrefresh = intnow;
				});
				page5.LogsettingsString("Barcode To Create Spawner", Color.yellow, ref customspwner, delegate(string stringy)
				{
					customspwner = stringy;
				});
				page5.CreateFunction("Add Custom Spawner ^", Color.yellow, delegate
				{
					if (IsBarcodeInGame(customspwner.Trim()))
					{
						AISpawner aISpawner = new AISpawner(delegate
						{
						}, cTimerrefresh)
						{
							eachspawnisrandom = false
						};
						aISpawner.Start(customspwner.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh);
						NPCSpawnersNow.Add(aISpawner);
					}
				});
				page5.CreateFunction("Added Left Hand Item As Spawner", Color.yellow, delegate
				{
					string text2 = ND_YourGetHand(WhichHand.Left).ND_GetMarrowEntity().ND_GetBarcodeID();
					if (IsBarcodeInGame(text2))
					{
						AISpawner aISpawner = new AISpawner(delegate
						{
						}, cTimerrefresh)
						{
							eachspawnisrandom = false
						};
						aISpawner.Start(text2, Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh);
						NPCSpawnersNow.Add(aISpawner);
					}
				});
				page5.CreateFunction("Added Right Hand Item As Spawner", Color.yellow, delegate
				{
					string text2 = ND_YourGetHand(WhichHand.Right).ND_GetMarrowEntity().ND_GetBarcodeID();
					if (IsBarcodeInGame(text2))
					{
						AISpawner aISpawner = new AISpawner(delegate
						{
						}, cTimerrefresh)
						{
							eachspawnisrandom = false
						};
						aISpawner.Start(text2, Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh);
						NPCSpawnersNow.Add(aISpawner);
					}
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All NPCS]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Avatars]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllAvatars);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Spawnables]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllSpawnables);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Weapons]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllWeapons);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All No Tag Spawnables]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.NoTagsSpawnables);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Blades]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllBlade);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Blunts]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllBlunt);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Knifes]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllKnife);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Melees]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllMelees);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Pistols]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllPistol);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Ranged]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllRanged);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Rifles]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllRifle);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Shotguns]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllShotgun);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All SMGS]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllSMG);
					NPCSpawnersNow.Add(aISpawner);
				});
				page5.CreateFunction("Spawn Randomizing Spawner! [All Snipers]", Color.yellow, delegate
				{
					AISpawner aISpawner = new AISpawner(delegate
					{
					}, cTimerrefresh)
					{
						eachspawnisrandom = true
					};
					aISpawner.Start(GUIUtility.systemCopyBuffer.Trim(), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh, RandomizerType.AllSniper);
					NPCSpawnersNow.Add(aISpawner);
				});
				BoneLib.BoneMenu.Page page6 = page5.CreatePage("+ Random Spawner", Color.yellow);
				page6.CreateFunction("NPC Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllNPCs), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("Avatar Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllAvatars), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("Melee Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllMelees), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("Spawnable Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllSpawnables), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("No Tag Spawnable Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.NoTagsSpawnables), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("Weapon Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllWeapons), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] Pistol Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllPistol), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] Ranged Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllRanged), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] Rifle Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllRifle), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] Shotgun Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllShotgun), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] SMG Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllSMG), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Weapon] Sniper Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllSniper), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Melee] Blade Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllBlade), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Melee] Blunt Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllBlunt), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page6.CreateFunction("[Melee] Knife Spawner!", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start(GetRandomByType(RandomizerType.AllKnife), Player.PhysicsRig.m_footLf.transform.position, cmaxentrefresh));
				});
				page4.CreateFunction("[Ammo] Ammo Box Light", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-683b-4c01-b378-6795416d6d6f", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[Ammo] Ammo Box Medium", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-57d4-4468-b5f0-c795416d6d6f", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[Ammo] Ammo Box Heavy", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-97a9-43f7-be30-6095416d6d6f", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[NPC] Nullbodys", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-d82d-4f65-89fd-a4954e756c6c", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[NPC] Crablets", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-4583-48b5-ac3f-eb9543726162", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[NPC] Omni Projector Hazmats", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-7c6d-4f53-b61c-e4024f6d6e69", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[NPC] Security Guards", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("SLZ.BONELAB.Content.Spawnable.NPCSecurityGuard", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				page4.CreateFunction("[NPC] Skeleton Steel", Color.yellow, delegate
				{
					NPCSpawnersNow.Add(new AISpawner(delegate
					{
					}, cTimerrefresh).Start("c1534c5a-a750-44ca-9730-b487536b656c", Player.PhysicsRig.m_footLf.transform.position, 10));
				});
				foreach (AISpawner aispawnernow in NPCSpawnersNow)
				{
					LevelCrateReference levelCrateReference = new LevelCrateReference(aispawnernow.SpawnerMapLockedTo);
					BoneLib.BoneMenu.Page page7 = AISpawnersPage.CreatePage("+ " + StripColorTags(levelCrateReference.Scannable.Title), Color.yellow);
					BoneLib.BoneMenu.Page page8 = page7.CreatePage("+ " + aispawnernow.NameOfUsed + " | ID : " + aispawnernow.InstanceID, Color.yellow);
					page8.CreateFunction("Change Spawners Spawnable [Clipboard]", Color.yellow, delegate
					{
						if (!aispawnernow.eachspawnisrandom)
						{
							string text2 = GUIUtility.systemCopyBuffer.Trim();
							if (IsBarcodeInGame(text2))
							{
								aispawnernow.BarcodeID = text2 ?? throw new ArgumentNullException("clippytext");
								SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(text2);
								aispawnernow.NameOfUsed = StripColorTags(spawnableCrateReference?.Crate?.Title) ?? "Unknown";
								Menu.OpenPage(AISpawnersPage);
								NotificationNow("Nemesis Anti-Cheat", "[Spawner " + aispawnernow.InstanceID + "] Changed Spawners Spawnable!", NotificationType.SUCCESS, 2f);
							}
							else
							{
								NotificationNow("Nemesis Anti-Cheat", $"[Spawner {aispawnernow.InstanceID}] Barcode '{aispawnernow.BarcodeID}' does not exist in game or Mod for it is not instlled!. Spawner not started.", NotificationType.ERROR, 3f);
							}
						}
						else
						{
							NotificationNow("Nemesis Anti-Cheat", "[Spawner " + aispawnernow.InstanceID + "] Is a randomizing spawner this is actally pointless so no....", NotificationType.ERROR, 3f);
						}
					});
					page8.CreateFunction("On/Off Spawn Only If All Dead", Color.yellow, delegate
					{
						aispawnernow.onlyspawnifalldead = !aispawnernow.onlyspawnifalldead;
						NotificationNow("Nemesis Anti-Cheat", $"+ Spawn Only If All Dead : {aispawnernow.onlyspawnifalldead} | Spawner ID : {aispawnernow.InstanceID}", NotificationType.SUCCESS, 2f);
					});
					page8.CreateFunction("On/Off Despawn Dead", Color.yellow, delegate
					{
						aispawnernow.despawndeads = !aispawnernow.despawndeads;
						NotificationNow("Nemesis Anti-Cheat", $"+ Despawn Dead : {aispawnernow.despawndeads} | Spawner ID : {aispawnernow.InstanceID}", NotificationType.SUCCESS, 2f);
					});
					page8.CreateFunction("Kill All In Spawner", Color.yellow, delegate
					{
						aispawnernow.KillAllInSpawner();
					});
					page8.CreateFunction("Pause Spawner", Color.yellow, delegate
					{
						aispawnernow.Pause();
					});
					page8.CreateFunction("Resume Spawner", Color.yellow, delegate
					{
						aispawnernow.Resume();
					});
					page8.CreateFunction("Remove Spawner", Color.yellow, delegate
					{
						aispawnernow.StopAndClear(notification: false);
						NPCSpawnersNow.Remove(aispawnernow);
						Menu.OpenPage(AISpawnersPage);
						NotificationNow("Nemesis Anti-Cheat", "Removed Spawner " + aispawnernow.NameOfUsed + " Spawner ID : " + aispawnernow.InstanceID, NotificationType.SUCCESS, 3f);
					});
					page8.Logsettingsint("Max Entities", Color.green, ref vmaxentrefresh, 1, 1, 100, delegate(int intnow)
					{
						vmaxentrefresh = intnow;
						aispawnernow.UpdateMaxSpawns(vmaxentrefresh);
					});
					page8.Logsettingsfloat("Timer Seconds", Color.green, ref vTimerrefresh, 1f, 1f, 100000f, delegate(float intnow)
					{
						vTimerrefresh = intnow;
						aispawnernow.UpdateSpawnInterval(vTimerrefresh);
					});
				}
			}
			if (pageaction == playermessages)
			{
				PlayersListNow();
			}
			if (pageaction == PlayersOnlinePage && NetworkInfo.HasLayer)
			{
				IMatchmaker matchmaker = NetworkLayerManager.Layer.Matchmaker;
				if (matchmaker != null)
				{
					PlayersOnlinePage.RemoveAll();
					PlayersOnlines.Clear();
					if (!MenuMatchmaking.SearchBarElement.gameObject.active && !MenuMatchmaking.BrowserPage.gameObject.active)
					{
						matchmaker.RequestLobbies(delegate(IMatchmaker.MatchmakerCallbackInfo info)
						{
							if (info.Lobbies != null && info.Lobbies.Any())
							{
								IMatchmaker.LobbyInfo[] lobbies = info.Lobbies;
								for (int i = 0; i < lobbies.Length; i++)
								{
									IMatchmaker.LobbyInfo lobbyInfo = lobbies[i];
									PlayerInfo[] players = lobbyInfo.Metadata.LobbyInfo.PlayerList.Players;
									foreach (PlayerInfo p in players)
									{
										if (!PlayersOnlines.Any((PlayerInfo x) => x.PlatformID == p.PlatformID))
										{
											PlayersOnlines.Add(new PlayerInfo
											{
												Username = StripColorTags(p.Username),
												PlatformID = p.PlatformID,
												Nickname = p.Nickname,
												Description = p.Description,
												PermissionLevel = p.PermissionLevel,
												AvatarModID = p.AvatarModID,
												AvatarTitle = p.AvatarTitle
											});
										}
									}
								}
								foreach (PlayerInfo playersonlineline in PlayersOnlines)
								{
									string text2 = (string.IsNullOrEmpty(playersonlineline.Nickname) ? playersonlineline.Username : playersonlineline.Nickname);
									PlayersOnlinePage.CreateFunction("+ " + text2, Color.green, delegate
									{
										if (!NetworkHelper.IsBanned(playersonlineline.PlatformID))
										{
											BanInfo item2 = new BanInfo
											{
												Player = new PlayerInfo
												{
													Username = playersonlineline.Username,
													Nickname = playersonlineline.Nickname,
													PlatformID = playersonlineline.PlatformID,
													Description = playersonlineline.Description,
													AvatarModID = playersonlineline.AvatarModID,
													AvatarTitle = playersonlineline.AvatarTitle
												},
												Reason = "Manually Banned [Nemesis Anti-Cheat]"
											};
											BanManager.BanList.Bans.RemoveAll((BanInfo banInfo) => banInfo.Player.PlatformID == playersonlineline.PlatformID);
											BanManager.BanList.Bans.Add(item2);
											DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
											NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
										}
										else
										{
											BanManager.Pardon(playersonlineline.PlatformID);
											NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
										}
									});
								}
								NotificationNow("Nemesis Anti-Cheat", $"Players Online : {PlayersOnlines.Count}", NotificationType.SUCCESS, 1.5f);
							}
						});
					}
				}
			}
			if (pageaction == ProtectionLogs)
			{
				ProtectionLogs.RemoveAll();
				ProtectionLogs.CreateFunction("Clear Protection Logs", Color.yellow, delegate
				{
					ClientExploitLogs.Clear();
				});
				ProtectionLogs.CreateFunction("Clear Spawn Logs", Color.yellow, delegate
				{
					SpawnLogs.Clear();
				});
				ProtectionLogs.CreateFunction("Clear Avi Switch Logs", Color.yellow, delegate
				{
					PlayeravatarStuff.Clear();
				});
				ProtectionLogs.Logsettings("Melon Console Spawn Logs", Color.cyan, ref spawnlogsmelonlog, delegate(bool enabled)
				{
					spawnlogsmelonlog = enabled;
				});
				foreach (var clientExploitLog in ClientExploitLogs)
				{
					string exploiterNickname = clientExploitLog.Nickname;
					string exploiterUsername = clientExploitLog.Username;
					string exploiterPlatformId = clientExploitLog.PlatformId;
					string Exploitmessage = clientExploitLog.ExploitType;
					BoneLib.BoneMenu.Page page9 = ProtectionLogs.CreatePage("+ " + Exploitmessage, Color.green);
					BoneLib.BoneMenu.Page page10 = page9.CreatePage("+ Exploiters", Color.green);
					page10.CreateFunction("Exploiter :" + exploiterNickname + $" [{exploiterUsername}] ({exploiterPlatformId})", Color.yellow, delegate
					{
						string text2 = $"{exploiterNickname} [{exploiterUsername}] [{exploiterPlatformId}] Exploit : [{Exploitmessage}]";
						GUIUtility.systemCopyBuffer = Exploitmessage;
						NotificationNow("Nemesis Anti-Cheat", "Copied Exploit Details To Clipboard", NotificationType.SUCCESS, 2.5f);
					});
				}
			}
			if (pageaction == statskicknow)
			{
				statskicknow.RemoveAll();
				foreach (StatsKickerPresets presetnow in StatsKickerPresets.StatsKickerPresetz)
				{
					BoneLib.BoneMenu.Page page11 = statskicknow.CreatePage("+ [SK] " + presetnow.TitleOfPreset, Color.green);
					page11.CreateString("Edit Preset Name", Color.white, presetnow.TitleOfPreset, delegate(string stringy)
					{
						presetnow.EditPresetName(stringy);
						Menu.OpenPage(statskicknow);
					});
					PageOptions(page11, presetnow);
					page11.CreateFunction("Apply Preset", Color.yellow, delegate
					{
						string[] currentPreset = new string[11]
						{
							presetnow.Height, presetnow.MassArm, presetnow.MassChest, presetnow.MassHead, presetnow.MassLeg, presetnow.MassPelvis, presetnow.MassTotal, presetnow.Speed, presetnow.StrengthLower, presetnow.StrengthUpper,
							presetnow.Vitality
						};
						StatsKickerPresets.CurrentPreset = currentPreset;
						NotificationNow("Nemesis Anti-Cheat", "Applied Stats Kicker Preset [" + presetnow.TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
						StatsKickerPresets.SavePresets();
					});
					page11.CreateFunction("Remove Preset", Color.yellow, delegate
					{
						presetnow.RemovePreset();
						Menu.OpenPage(statskicknow);
					});
				}
			}
			if (pageaction == teleportersnow)
			{
				teleportersnow.RemoveAll();
				foreach (TeleporterManager teleporter in TeleporterManager.Teleportersnowx)
				{
					BoneLib.BoneMenu.Page page12 = teleportersnow.CreatePage("+ " + teleporter.Map, Color.green);
					BoneLib.BoneMenu.Page page13 = page12.CreatePage("+ [TP] " + teleporter.TitleOfTeleporter, Color.green);
					page13.CreateString("Edit Preset Name", Color.white, teleporter.TitleOfTeleporter, delegate(string stringy)
					{
						teleporter.EditPresetName(stringy);
						Menu.OpenPage(teleportersnow);
					});
					page13.CreateFunction("Teleport To", Color.yellow, delegate
					{
						if (!GamemodeManager.IsGamemodeStarted)
						{
							teleporter.TeleportToIt();
						}
					});
					page13.CreateFunction("Set As Spawn Point", Color.yellow, delegate
					{
						if (!GamemodeManager.IsGamemodeStarted)
						{
							Player.RigManager.checkpointPosition = teleporter.Position;
							NotificationNow("Nemesis Anti-Cheat", "Set Spawn Point!", NotificationType.SUCCESS, 3.5f);
						}
					});
				}
			}
			if (pageaction == loadoutpagesnow)
			{
				loadoutpagesnow.RemoveAll();
				foreach (InventoryPage pagenow in InventoryPage.InventoryPresets)
				{
					if (pagenow != null)
					{
						BoneLib.BoneMenu.Page page14 = loadoutpagesnow.CreatePage("+ [L] " + (pagenow.TitleOfPreset ?? "No Title"), Color.green);
						page14.CreateString("Edit Preset Name", Color.white, pagenow.TitleOfPreset, delegate(string stringy)
						{
							pagenow.EditPresetName(stringy);
							Menu.OpenPage(loadoutpagesnow);
						});
						foreach (System.Collections.Generic.KeyValuePair<string, string> slot in pagenow.Slots)
						{
							string key = slot.Key;
							string value = slot.Value;
							BoneLib.BoneMenu.Page pageynow = page14.CreatePage("+ [" + key + "] " + (new SpawnableCrateReference(value)?.Crate?.name ?? "Empty"), Color.green);
							PageOptions2(pageynow, key, pagenow);
						}
						page14.CreateFunction("Load Loadout", Color.yellow, delegate
						{
							pagenow.LoadIntoPlayer();
						});
						page14.CreateFunction("Remove Loadout", Color.yellow, delegate
						{
							pagenow.RemovePreset();
							Menu.OpenPage(loadoutpages);
						});
					}
				}
			}
			if (pageaction == bodylognowpagexx)
			{
				bodylognowpagexx.RemoveAll();
				if (BodyLogPage.BodyLogPages == null)
				{
					return;
				}
				foreach (BodyLogPage pagenow2 in BodyLogPage.BodyLogPages)
				{
					if (pagenow2 != null)
					{
						BoneLib.BoneMenu.Page page15 = bodylognowpagexx.CreatePage("+ [B] " + (pagenow2.TitleOfPreset ?? "Unnamed"), Color.green);
						BoneLib.BoneMenu.Page sharepresetbodylog = page15.CreatePage("+ [B] Share Preset", Color.green);
						Menu.OnPageOpened += delegate(BoneLib.BoneMenu.Page page19)
						{
							if (page19 == sharepresetbodylog)
							{
								sharepresetbodylog.RemoveAll();
								foreach (NetworkPlayer xxxxm in NetworkPlayers())
								{
									sharepresetbodylog.CreateFunction(CleanedNAME(xxxxm), Color.yellow, delegate
									{
										if (!(TimeReferences.TimeSinceStartup - ShareBodyLogPageMessage._timeOfRequest <= 3f))
										{
											ShareBodyLogPageData shareBodyLogPageData = ShareBodyLogPageData.Create(PlayerIDManager.LocalSmallID, pagenow2.TitleOfPreset, pagenow2.Slot1, pagenow2.Slot2, pagenow2.Slot3, pagenow2.Slot4, pagenow2.Slot5, pagenow2.Slot6, pagenow2.ModIoID1, pagenow2.ModIoID2, pagenow2.ModIoID3, pagenow2.ModIoID4, pagenow2.ModIoID5, pagenow2.ModIoID6);
											MessageRelay.RelayModule<ShareBodyLogPageMessage, ShareBodyLogPageData>(shareBodyLogPageData, new MessageRoute(xxxxm.ND_SmallID(), NetworkChannel.Reliable));
											NotificationNow("Nemesis Anti-Cheat", "Sent Bodylog Page [" + shareBodyLogPageData.TitleOfPreset + "] To " + CleanedNAME(xxxxm), NotificationType.WARNING, 4f);
										}
									});
								}
							}
						};
						page15.CreateString("Edit Preset Name", Color.white, pagenow2.TitleOfPreset, delegate(string stringy)
						{
							pagenow2.EditPresetName(stringy);
							Menu.OpenPage(bodylognowpage);
						});
						string[] array = new string[6] { pagenow2.Slot1, pagenow2.Slot2, pagenow2.Slot3, pagenow2.Slot4, pagenow2.Slot5, pagenow2.Slot6 };
						for (int num4 = 0; num4 < array.Length; num4++)
						{
							BoneLib.BoneMenu.Page pageynow2 = page15.CreatePage($"+ B[{num4 + 1}] " + SafeCrateName(array[num4]), Color.green);
							PageOptionsAvatar(pageynow2, pagenow2, num4 + 1);
						}
						page15.CreateFunction("Apply Entire Current Bodylog To Preset", Color.yellow, delegate
						{
							string[] array2 = new string[6] { "EMPTY", "EMPTY", "EMPTY", "EMPTY", "EMPTY", "EMPTY" };
							int num5 = 0;
							(PullCordDevice bodylogreturn, MeshRenderer Outerring) tuple = ND_BodyLog(Player.PhysicsRig);
							PullCordDevice item2 = tuple.bodylogreturn;
							MeshRenderer item3 = tuple.Outerring;
							Il2CppReferenceArray<AvatarCrateReference> il2CppReferenceArray = item2?.avatarCrateRefs;
							if (il2CppReferenceArray != null)
							{
								foreach (AvatarCrateReference item15 in il2CppReferenceArray)
								{
									if (num5 >= 6)
									{
										break;
									}
									string text2 = item15?.Crate?.Barcode?.ID ?? "EMPTY";
									array2[num5++] = text2;
								}
							}
							pagenow2.Slot1 = array2[0];
							pagenow2.Slot2 = array2[1];
							pagenow2.Slot3 = array2[2];
							pagenow2.Slot4 = array2[3];
							pagenow2.Slot5 = array2[4];
							pagenow2.Slot6 = array2[5];
							BodyLogPage.SavePresets();
							Menu.OpenPage(bodylognowpage);
							NotificationNow("Nemesis Anti-Cheat", "Applied Entire Current Bodylog To Preset [" + pagenow2.TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
						});
						page15.CreateFunction("Apply/Refresh Preset", Color.yellow, delegate
						{
							ChangeBodyLogAvatarSlot(1, pagenow2.Slot1 ?? "EMPTY", notification: false);
							ChangeBodyLogAvatarSlot(2, pagenow2.Slot2 ?? "EMPTY", notification: false);
							ChangeBodyLogAvatarSlot(3, pagenow2.Slot3 ?? "EMPTY", notification: false);
							ChangeBodyLogAvatarSlot(4, pagenow2.Slot4 ?? "EMPTY", notification: false);
							ChangeBodyLogAvatarSlot(5, pagenow2.Slot5 ?? "EMPTY", notification: false);
							ChangeBodyLogAvatarSlot(6, pagenow2.Slot6 ?? "EMPTY", notification: false);
							NotificationNow("Nemesis Anti-Cheat", "Applied Bodylog Page [" + pagenow2.TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
							pagenow2.ApplyPreset();
							BodyLogPage.SavePresets();
							Menu.OpenPage(bodylognowpage);
						});
						page15.CreateFunction("Remove Preset", Color.yellow, delegate
						{
							pagenow2.RemovePreset();
						});
					}
				}
			}
			if (pageaction == cheatspresetsnow)
			{
				cheatspresetsnow.RemoveAll();
				foreach (CreateCheatToolsPreset cheatpresetty in CreateCheatToolsPreset.CheatPresets)
				{
					BoneLib.BoneMenu.Page page16 = cheatspresetsnow.CreatePage("+ [C] " + cheatpresetty.TitleOfPreset, Color.green);
					BoneLib.BoneMenu.Page SHARECHEATTY = page16.CreatePage("+ [C] Share Preset", Color.green);
					Menu.OnPageOpened += delegate(BoneLib.BoneMenu.Page page19)
					{
						if (page19 == SHARECHEATTY)
						{
							SHARECHEATTY.RemoveAll();
							foreach (NetworkPlayer xxxxm in NetworkPlayers())
							{
								SHARECHEATTY.CreateFunction(CleanedNAME(xxxxm), Color.yellow, delegate
								{
									if (!(TimeReferences.TimeSinceStartup - ShareDevToolPresetMessage._timeOfRequest <= 3f))
									{
										ShareDevToolPresetData shareDevToolPresetData = ShareDevToolPresetData.Create(PlayerIDManager.LocalSmallID, cheatpresetty.TitleOfPreset, cheatpresetty.Item1.BarcodeId, cheatpresetty.Item1.ModIoID, cheatpresetty.Item1.SpawnableName, cheatpresetty.Item1.LocalSpawn, cheatpresetty.Item2.BarcodeId, cheatpresetty.Item2.ModIoID, cheatpresetty.Item2.SpawnableName, cheatpresetty.Item2.LocalSpawn, cheatpresetty.Item3.BarcodeId, cheatpresetty.Item3.ModIoID, cheatpresetty.Item3.SpawnableName, cheatpresetty.Item3.LocalSpawn, cheatpresetty.Item4.BarcodeId, cheatpresetty.Item4.ModIoID, cheatpresetty.Item4.SpawnableName, cheatpresetty.Item4.LocalSpawn, cheatpresetty.Item5.BarcodeId, cheatpresetty.Item5.ModIoID, cheatpresetty.Item5.SpawnableName, cheatpresetty.Item5.LocalSpawn);
										MessageRelay.RelayModule<ShareDevToolPresetMessage, ShareDevToolPresetData>(shareDevToolPresetData, new MessageRoute(xxxxm.ND_SmallID(), NetworkChannel.Reliable));
										NotificationNow("Nemesis Anti-Cheat", "Sent DevToolPreset [" + shareDevToolPresetData.TitleOfPreset + "] To " + CleanedNAME(xxxxm), NotificationType.WARNING, 4f);
									}
								});
							}
						}
					};
					page16.CreateString("Edit Preset Name", Color.white, cheatpresetty.TitleOfPreset, delegate(string stringy)
					{
						cheatpresetty.EditPresetName(stringy);
						Menu.OpenPage(cheatspresetsnow);
					});
					BoneLib.BoneMenu.Page pageynow3 = page16.CreatePage("+ C[1] " + cheatpresetty.Item1.SpawnableName, Color.green);
					PageOptions3(pageynow3, cheatpresetty, 1);
					BoneLib.BoneMenu.Page pageynow4 = page16.CreatePage("+ C[2] " + cheatpresetty.Item2.SpawnableName, Color.green);
					PageOptions3(pageynow4, cheatpresetty, 2);
					BoneLib.BoneMenu.Page pageynow5 = page16.CreatePage("+ C[3] " + cheatpresetty.Item3.SpawnableName, Color.green);
					PageOptions3(pageynow5, cheatpresetty, 3);
					BoneLib.BoneMenu.Page pageynow6 = page16.CreatePage("+ C[4] " + cheatpresetty.Item4.SpawnableName, Color.green);
					PageOptions3(pageynow6, cheatpresetty, 4);
					BoneLib.BoneMenu.Page pageynow7 = page16.CreatePage("+ C[5] " + cheatpresetty.Item5.SpawnableName, Color.green);
					PageOptions3(pageynow7, cheatpresetty, 5);
					page16.CreateFunction("Apply/Save/Edit Preset", Color.yellow, delegate
					{
						CreateCheatToolsPreset.CurrentPresetNow = cheatpresetty;
						SpawnableCrateReference[] array2 = new SpawnableCrateReference[5]
						{
							new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item1.BarcodeId),
							new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item2.BarcodeId),
							new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item3.BarcodeId),
							new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item4.BarcodeId),
							new SpawnableCrateReference(CreateCheatToolsPreset.CurrentPresetNow.Item5.BarcodeId)
						};
						InstanceOfIt.crates = array2;
						NotificationNow("Nemesis Anti-Cheat", "Applied Cheat Tool Preset Edit [" + cheatpresetty.TitleOfPreset + "]! Now Re-Apply Preset To Take Effect!", NotificationType.SUCCESS, 3.5f);
						CreateCheatToolsPreset.SavePresets();
					});
					page16.CreateFunction("Remove Preset", Color.yellow, delegate
					{
						cheatpresetty.RemovePreset();
						Menu.OpenPage(cheatspresetsnow);
					});
				}
			}
			if (pageaction == colorpresetsnow)
			{
				colorpresetsnow.RemoveAll();
				foreach (BodyLogRadialMenuColorPreset colorpreset in BodyLogRadialMenuColorPreset.ColorPresets)
				{
					BoneLib.BoneMenu.Page page17 = colorpresetsnow.CreatePage("+ [BLRMC] " + colorpreset.TitleOfPreset, Color.green);
					page17.CreateString("Edit Preset Name", Color.white, colorpreset.TitleOfPreset, delegate(string stringy)
					{
						colorpreset.EditPresetName(stringy);
						Menu.OpenPage(colorpresetsnow);
					});
					BoneLib.BoneMenu.Page pageyNow = page17.CreatePage("+ [BLRMC 1] Bodylog Hologram Color", Color.green);
					PageOptionNow(pageyNow, colorpreset, "BodyLogColor");
					BoneLib.BoneMenu.Page pageyNow2 = page17.CreatePage("+ [BLRMC 2] Bodylog Ball Color", Color.green);
					PageOptionNow(pageyNow2, colorpreset, "BodyLogBallColor");
					BoneLib.BoneMenu.Page pageyNow3 = page17.CreatePage("+ [BLRMC 3] Bodylog Line Color", Color.green);
					PageOptionNow(pageyNow3, colorpreset, "BodyLogLineColor");
					BoneLib.BoneMenu.Page pageyNow4 = page17.CreatePage("+ [BLRMC 4] Radial Menu Color", Color.green);
					PageOptionNow(pageyNow4, colorpreset, "RadialMenuColor");
					page17.CreateFunction("Apply/Refresh Color Preset", Color.yellow, delegate
					{
						ApplyIt(colorpreset);
					});
					page17.CreateFunction("Remove Preset", Color.yellow, delegate
					{
						colorpreset.RemovePreset();
						Menu.OpenPage(colorpresetsnow);
					});
				}
			}
			if (pageaction == FusionProfilesnow)
			{
				FusionProfilesnow.RemoveAll();
				foreach (FusionProfilePresets profilenowha in FusionProfilePresets.ProfilePresets)
				{
					BoneLib.BoneMenu.Page page18 = FusionProfilesnow.CreatePage("+ [FPP] " + profilenowha.TitleOfPreset, Color.green);
					page18.CreateString("Edit Preset Name", Color.white, profilenowha.TitleOfPreset, delegate(string stringy)
					{
						profilenowha.EditPresetName(stringy);
						Menu.OpenPage(FusionProfilesnow);
					});
					page18.CreateFunction("Apply Preset", Color.yellow, delegate
					{
						MelonCoroutines.Start(profilenowha.ApplyPreset());
					});
					page18.CreateFunction("Replace Avatar With Current", Color.yellow, delegate
					{
						profilenowha.SetValue("AvatarAtTheTime", ND_YourAvatarBarcodeID());
						NotificationNow("Nemesis Anti-Cheat", "Set " + profilenowha.TitleOfPreset + " Avatar To Current!", NotificationType.SUCCESS, 3.5f);
						FusionProfilePresets.SavePresets();
					});
					page18.CreateFunction("Replace Description With Current", Color.yellow, delegate
					{
						profilenowha.SetValue("Description", ND_YourNetworkPlayer().ND_Description());
						NotificationNow("Nemesis Anti-Cheat", "Set " + profilenowha.TitleOfPreset + " Description To Current!", NotificationType.SUCCESS, 3.5f);
						FusionProfilePresets.SavePresets();
					});
					page18.CreateFunction("Replace Nickname With Current", Color.yellow, delegate
					{
						profilenowha.SetValue("Nickname", ND_YourNetworkPlayer().ND_Nickname());
						NotificationNow("Nemesis Anti-Cheat", "Set " + profilenowha.TitleOfPreset + " Nickname To Current!", NotificationType.SUCCESS, 3.5f);
						FusionProfilePresets.SavePresets();
					});
					page18.CreateFunction("Clear Bitmart Items", Color.yellow, delegate
					{
						profilenowha.BitMartItems = new System.Collections.Generic.List<string>();
						FusionProfilePresets.SavePresets();
					});
					page18.CreateFunction("Remove Preset", Color.yellow, delegate
					{
						profilenowha.RemovePreset();
					});
				}
			}
			if (pageaction == fppubs)
			{
				AllNemesisAntiCheatLobbies();
			}
			if (pageaction == pubs)
			{
				AllLobbies();
			}
		};
		MultiplayerHooking.OnStartedServer += delegate
		{
			playersavatarrefs.Clear();
			playersspawnrefs.Clear();
			PlayerSpawningStuff.Clear();
		};
		Hooking.OnMarrowGameStarted += delegate
		{
			MelonCoroutines.Start(OnStartOfGame());
		};
		Hooking.OnGripAttached += delegate(Grip gruppy, Hand hund)
		{
			if (removesounds)
			{
				RemoveSoundGrip(gruppy);
			}
		};
		Hooking.OnGripDetached += delegate(Grip gruppy, Hand hund)
		{
			if (removesounds)
			{
				RemoveSoundGrip(gruppy);
			}
		};
		static void ApplyIt(BodyLogRadialMenuColorPreset colorpreset)
		{
			string[] currentPreset = new string[16]
			{
				colorpreset.BodyLogColor_R.ToString(),
				colorpreset.BodyLogColor_G.ToString(),
				colorpreset.BodyLogColor_B.ToString(),
				colorpreset.BodyLogColor_A.ToString(),
				colorpreset.BodyLogBallColor_R.ToString(),
				colorpreset.BodyLogBallColor_G.ToString(),
				colorpreset.BodyLogBallColor_B.ToString(),
				colorpreset.BodyLogBallColor_A.ToString(),
				colorpreset.BodyLogLineColor_R.ToString(),
				colorpreset.BodyLogLineColor_G.ToString(),
				colorpreset.BodyLogLineColor_B.ToString(),
				colorpreset.BodyLogLineColor_A.ToString(),
				colorpreset.RadialMenuColor_R.ToString(),
				colorpreset.RadialMenuColor_G.ToString(),
				colorpreset.RadialMenuColor_B.ToString(),
				colorpreset.RadialMenuColor_A.ToString()
			};
			BodyLogRadialMenuColorPreset.CurrentPreset = currentPreset;
			NotificationNow("Nemesis Anti-Cheat", "Applied Color Preset [" + colorpreset.TitleOfPreset + "]!", NotificationType.SUCCESS, 3.5f);
			BodyLogRadialMenuColorPreset.SavePresets();
		}
		static void AvatarsearcherLessCode(string barcode)
		{
			switch (AvatarSearchTypeReal)
			{
			case AvatarSearchType.ChangeInto:
				ChangeIntoAvi(barcode);
				break;
			case AvatarSearchType.CopyDetailsToClipboard:
			{
				AvatarCrateReference avatarCrateReference = new AvatarCrateReference(barcode);
				GUIUtility.systemCopyBuffer = $"Barcode ID : {avatarCrateReference.Barcode.ID}\nPallet Name : {StripColorTags(avatarCrateReference.Crate.Pallet.name)}\nPallet Author : {avatarCrateReference.Crate.Pallet.Author}";
				break;
			}
			case AvatarSearchType.SetBodyLog:
				ChangeBodyLogAvatarSlot(bodylogindex, barcode);
				break;
			}
		}
		static void PageOptionNow(BoneLib.BoneMenu.Page PageyNow, BodyLogRadialMenuColorPreset NowPreset, string propertystring)
		{
			PageyNow.CreateString("Color R", Color.yellow, COLORR, delegate(string stringy)
			{
				if (float.TryParse(stringy, out var result) && result >= 0f && result <= 255f)
				{
					COLORR = stringy;
					NowPreset.SetValue(propertystring + "_R", result);
					NotificationNow("Nemesis Anti-Cheat", "Set Value Into Color R", NotificationType.SUCCESS, 3.5f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", "Value must be between 0 and 255!", NotificationType.ERROR, 2f);
				}
			});
			PageyNow.CreateString("Color G", Color.yellow, COLORG, delegate(string stringy)
			{
				if (float.TryParse(stringy, out var result) && result >= 0f && result <= 255f)
				{
					COLORG = stringy;
					NowPreset.SetValue(propertystring + "_G", result);
					NotificationNow("Nemesis Anti-Cheat", "Set Value Into Color G", NotificationType.SUCCESS, 3.5f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", "Value must be between 0 and 255!", NotificationType.ERROR, 2f);
				}
			});
			PageyNow.CreateString("Color B", Color.yellow, COLORB, delegate(string stringy)
			{
				if (float.TryParse(stringy, out var result) && result >= 0f && result <= 255f)
				{
					COLORB = stringy;
					NowPreset.SetValue(propertystring + "_B", result);
					NotificationNow("Nemesis Anti-Cheat", "Set Value Into Color B", NotificationType.SUCCESS, 3.5f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", "Value must be between 0 and 255!", NotificationType.ERROR, 2f);
				}
			});
			PageyNow.CreateString("Transparency", Color.yellow, COLORA, delegate(string stringy)
			{
				if (float.TryParse(stringy, out var result) && result >= 0f && result <= 255f)
				{
					COLORA = stringy;
					NowPreset.SetValue(propertystring + "_A", result);
					NotificationNow("Nemesis Anti-Cheat", "Set Value Into Transparency", NotificationType.SUCCESS, 3.5f);
				}
				else
				{
					NotificationNow("Nemesis Anti-Cheat", "Value must be between 0 and 255!", NotificationType.ERROR, 2f);
				}
			});
			PageyNow.CreateFunction("Set Values From Clipboard", Color.yellow, delegate
			{
				if (TryParseRGBA(GUIUtility.systemCopyBuffer, out var r, out var g, out var b, out var a))
				{
					NowPreset.SetValue(propertystring + "_R", r);
					COLORR = r.ToString();
					NowPreset.SetValue(propertystring + "_G", g);
					COLORG = g.ToString();
					NowPreset.SetValue(propertystring + "_B", b);
					COLORB = b.ToString();
					NowPreset.SetValue(propertystring + "_A", a);
					COLORA = a.ToString();
					NotificationNow("Nemesis Anti-Cheat", $"Set Color Values From Clipboard R:{r} G:{g} B:{b} A:{a}", NotificationType.SUCCESS, 3.5f);
				}
				ApplyIt(NowPreset);
			});
			PageyNow.CreateFunction("Save/Edit Color", Color.yellow, delegate
			{
				float.TryParse(COLORR, out var result);
				float.TryParse(COLORG, out var result2);
				float.TryParse(COLORB, out var result3);
				float.TryParse(COLORA, out var result4);
				NowPreset.SetValue(propertystring + "_R", result);
				NowPreset.SetValue(propertystring + "_G", result2);
				NowPreset.SetValue(propertystring + "_B", result3);
				NowPreset.SetValue(propertystring + "_A", result4);
				BodyLogRadialMenuColorPreset.SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Applied Color Edit [" + NowPreset.TitleOfPreset + "]! Now Reopen The Radial Menu To Take Effect!", NotificationType.SUCCESS, 3.5f);
				Menu.OpenPage(colorpresetsnow);
				ApplyIt(NowPreset);
			});
		}
		static void PageOptions(BoneLib.BoneMenu.Page PageyNow, StatsKickerPresets presetnow)
		{
			(string, Func<string>, Action<string>)[] array = new(string, Func<string>, Action<string>)[11]
			{
				("Threshold Height", () => presetnow.Height, delegate(string val)
				{
					presetnow.Height = val;
				}),
				("Threshold Mass Arm", () => presetnow.MassArm, delegate(string val)
				{
					presetnow.MassArm = val;
				}),
				("Threshold Mass Chest", () => presetnow.MassChest, delegate(string val)
				{
					presetnow.MassChest = val;
				}),
				("Threshold Mass Head", () => presetnow.MassHead, delegate(string val)
				{
					presetnow.MassHead = val;
				}),
				("Threshold Mass Leg", () => presetnow.MassLeg, delegate(string val)
				{
					presetnow.MassLeg = val;
				}),
				("Threshold Mass Pelvis", () => presetnow.MassPelvis, delegate(string val)
				{
					presetnow.MassPelvis = val;
				}),
				("Threshold Mass Total", () => presetnow.MassTotal, delegate(string val)
				{
					presetnow.MassTotal = val;
				}),
				("Threshold Speed", () => presetnow.Speed, delegate(string val)
				{
					presetnow.Speed = val;
				}),
				("Threshold Strength Lower", () => presetnow.StrengthLower, delegate(string val)
				{
					presetnow.StrengthLower = val;
				}),
				("Threshold Strength Upper", () => presetnow.StrengthUpper, delegate(string val)
				{
					presetnow.StrengthUpper = val;
				}),
				("Threshold Vitality", () => presetnow.Vitality, delegate(string val)
				{
					presetnow.Vitality = val;
				})
			};
			(string, Func<string>, Action<string>)[] array2 = array;
			for (int num = 0; num < array2.Length; num++)
			{
				var (name, func, setter) = array2[num];
				PageyNow.CreateString(name, Color.yellow, func(), delegate(string stringy)
				{
					if (float.TryParse(stringy, out var result))
					{
						setter(stringy);
						NotificationNow("Nemesis Anti-Cheat", $"Set {name} value to {result}", NotificationType.SUCCESS, 3.5f);
					}
					else
					{
						NotificationNow("Nemesis Anti-Cheat", "Failed Needs To Be A Int!!!!!!!!!!!", NotificationType.ERROR, 2f);
					}
					StatsKickerPresets.SavePresets();
				});
			}
		}
		static void PageOptions2(BoneLib.BoneMenu.Page pageynow, string slotname, InventoryPage Now)
		{
			pageynow.CreateFunction("Remove", Color.yellow, delegate
			{
				NotificationNow("Nemesis Anti-Cheat", "Removed " + Now.GetSlotBarcode(slotname) + " From Slot", NotificationType.SUCCESS, 3.5f);
				Now.EditSlotBarcode(slotname, "Empty");
				Menu.OpenPage(loadoutpagesnow);
			});
			pageynow.CreateFunction("Add Left Hand Item To Slot", Color.yellow, delegate
			{
				string text = (ND_YourGetHand(WhichHand.Left)?.ND_GetMarrowEntity())?.ND_GetBarcodeID();
				if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
				{
					Now.EditSlotBarcode(slotname, text);
					Menu.OpenPage(loadoutpagesnow);
				}
			});
			pageynow.CreateFunction("Add Right Hand Item To Slot", Color.yellow, delegate
			{
				string text = (ND_YourGetHand(WhichHand.Right)?.ND_GetMarrowEntity())?.ND_GetBarcodeID();
				if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
				{
					Now.EditSlotBarcode(slotname, text);
					Menu.OpenPage(loadoutpagesnow);
				}
			});
			pageynow.CreateFunction("Add Clipboard Barcode Item To Slot", Color.yellow, delegate
			{
				string text = GUIUtility.systemCopyBuffer?.Trim();
				if (!string.IsNullOrEmpty(text) && IsBarcodeInGame(text))
				{
					Now.EditSlotBarcode(slotname, text);
					Menu.OpenPage(loadoutpagesnow);
				}
			});
		}
		static void PageOptions3(BoneLib.BoneMenu.Page pageynow, CreateCheatToolsPreset Now, int slotindex)
		{
			CreateCheatToolsPreset.Item item = new CreateCheatToolsPreset.Item();
			switch (slotindex)
			{
			case 1:
				item = Now.Item1;
				break;
			case 2:
				item = Now.Item2;
				break;
			case 3:
				item = Now.Item3;
				break;
			case 4:
				item = Now.Item4;
				break;
			case 5:
				item = Now.Item5;
				break;
			}
			pageynow.CreateBool("Local Spawning", Color.yellow, item.LocalSpawn, delegate(bool enabled)
			{
				Now.EditSlotLocalSpawn(slotindex, enabled);
				Menu.OpenPage(cheatspresetsnow);
			}).Value = item.LocalSpawn;
			pageynow.CreateFunction("Remove", Color.yellow, delegate
			{
				Now.ClearDevSlot(slotindex);
				Menu.OpenPage(cheatspresetsnow);
			});
			pageynow.CreateFunction("Add Left Hand Item To Slot", Color.yellow, delegate
			{
				if (IsBarcodeInGame(ND_YourGetHand(WhichHand.Left).ND_GetMarrowEntity().ND_GetBarcodeID()))
				{
					string newValue = ND_YourGetHand(WhichHand.Left).ND_GetMarrowEntity().ND_GetBarcodeID();
					Now.EditDevSlot(slotindex, newValue);
					Menu.OpenPage(cheatspresetsnow);
				}
			});
			pageynow.CreateFunction("Add Right Hand Item To Slot", Color.yellow, delegate
			{
				if (IsBarcodeInGame(ND_YourGetHand(WhichHand.Right).ND_GetMarrowEntity().ND_GetBarcodeID()))
				{
					string newValue = ND_YourGetHand(WhichHand.Right).ND_GetMarrowEntity().ND_GetBarcodeID();
					Now.EditDevSlot(slotindex, newValue);
					Menu.OpenPage(cheatspresetsnow);
				}
			});
			pageynow.CreateFunction("Add Clipboard Barcode Item To Slot", Color.yellow, delegate
			{
				if (IsBarcodeInGame(GUIUtility.systemCopyBuffer.Trim()))
				{
					string newValue = GUIUtility.systemCopyBuffer.Trim();
					Now.EditDevSlot(slotindex, newValue);
					Menu.OpenPage(cheatspresetsnow);
				}
			});
		}
		static void PageOptionsAvatar(BoneLib.BoneMenu.Page pageynow, BodyLogPage PageNow, int slotindex)
		{
			if (pageynow != null && PageNow != null)
			{
				pageynow.CreateFunction("Remove", Color.yellow, delegate
				{
					PageNow.ClearSlot(slotindex);
					Menu.OpenPage(bodylognowpage);
				});
				pageynow.CreateFunction("Add Current Avatar To Slot", Color.yellow, delegate
				{
					string text = ND_YourAvatarBarcodeID();
					if (!string.IsNullOrEmpty(text) && IsAvatarCrateExist(text))
					{
						PageNow.EditSlot(slotindex, text);
						Menu.OpenPage(bodylognowpage);
					}
				});
				pageynow.CreateFunction("Add Clipboard Barcode Avatar To Slot", Color.yellow, delegate
				{
					string text = GUIUtility.systemCopyBuffer?.Trim();
					if (!string.IsNullOrEmpty(text) && IsAvatarCrateExist(text))
					{
						PageNow.EditSlot(slotindex, text);
						Menu.OpenPage(bodylognowpage);
					}
				});
			}
		}
		static void PopulatePage(string filePath, System.Collections.Generic.HashSet<string> hashSet, BoneLib.BoneMenu.Page page)
		{
			page.RemoveAll();
			if (hashSet.Count > 0)
			{
				NotificationNow("Nemesis Anti-Cheat", $"Amount :{hashSet.Count}", NotificationType.WARNING, 2f);
			}
			foreach (string item in hashSet)
			{
				page.CreateFunction(item, Color.yellow, delegate
				{
					NotificationNow("Nemesis Anti-Cheat", "Removed " + item, NotificationType.SUCCESS);
					RemoveFromFile(item, filePath, hashSet);
					Menu.OpenPage(page);
				});
			}
		}
		static void RemoveFromFile(string itemToRemove, string path4, System.Collections.Generic.HashSet<string> listofstring)
		{
			listofstring.Remove(itemToRemove);
			File.WriteAllLines(path4, listofstring);
		}
		static string SafeCrateName(string barcode)
		{
			if (string.IsNullOrEmpty(barcode))
			{
				return "EMPTY";
			}
			string text = new AvatarCrateReference(barcode)?.Crate?.name;
			return StripColorTags(text ?? "EMPTY");
		}
		static void SpawnerFuncLessCode(string barcode)
		{
			switch (spawnablesrchtype)
			{
			case SpawnableSearchType.Spawn:
				SpawnIt(barcode, ND_YourGetHand(WhichHand.Left).transform.position + ND_YourGetHand(WhichHand.Left).transform.forward + ND_YourGetHand(WhichHand.Left).transform.up, Quaternion.identity);
				break;
			case SpawnableSearchType.CopyDetailsToClipboard:
				GUIUtility.systemCopyBuffer = barcode;
				NotificationNow("Nemesis Anti-Cheat", "Copied Barcode To Clipboard! " + barcode, NotificationType.WARNING, 3f);
				break;
			case SpawnableSearchType.UnFavoriteAndFavorite:
				if (!DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Contains(barcode))
				{
					DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Add(barcode);
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Added " + barcode + " To SaveGame Favorites!", NotificationType.SUCCESS);
				}
				else
				{
					DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Remove(barcode);
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Removed " + barcode + " From SaveGame Favorites!", NotificationType.SUCCESS);
				}
				break;
			case SpawnableSearchType.DespawnAllOfThis:
				foreach (NetworkEntity item16 in NetworkEntities())
				{
					if (item16.ND_GetMarrowEntity().ND_GetBarcodeID() == barcode)
					{
						item16.ND_Despawn();
					}
				}
				NotificationNow("Nemesis Anti-Cheat", "Despawned Everything Matching " + StripColorTags(new SpawnableCrateReference(barcode).Crate.name), NotificationType.SUCCESS, 3.5f);
				break;
			case SpawnableSearchType.SetInSpawnGun:
				SetBarCodeToSpawnGun(barcode);
				break;
			}
		}
		static bool TryParseRGBA(string text, out float r, out float g, out float b, out float a)
		{
			r = (g = (b = 0f));
			a = 255f;
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			text = text.Trim().ToLower();
			if (text.StartsWith("rgba"))
			{
				text = text.Substring(4);
			}
			else if (text.StartsWith("rgb"))
			{
				text = text.Substring(3);
			}
			text = text.TrimStart('(', '[', '{');
			text = text.TrimEnd(')', ']', '}');
			string[] array = text.Split(',', StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 3 && array.Length != 4)
			{
				return false;
			}
			if (!float.TryParse(array[0].Trim(), out r))
			{
				return false;
			}
			if (!float.TryParse(array[1].Trim(), out g))
			{
				return false;
			}
			if (!float.TryParse(array[2].Trim(), out b))
			{
				return false;
			}
			if (array.Length == 4 && !float.TryParse(array[3].Trim(), out a))
			{
				return false;
			}
			return Valid(r) && Valid(g) && Valid(b) && Valid(a);
		}
		static bool Valid(float v)
		{
			return v >= 0f && v <= 255f;
		}
	}

	internal static void CreateProtectorUI()
	{
		PageEx.boolslogged.Clear();
		PageEx.floatslogged.Clear();
		PageEx.intslogged.Clear();
		PageEx.enumvaluelogged.Clear();
		PageEx.stringslogged.Clear();
		string json = File.ReadAllText(modiotokenfile);
		JObject jObject = JObject.Parse(json);
		modiotoken = jObject["mod.io.access_token"]?.ToString();
		TryPatchit("BonelabSupport", "MarrowFusion.Bonelab.Patching.PopUpMenuViewPatches", "OnSpawnDelegate", typeof(CustomDevToolsFusion).GetMethod("Prefix"), null);
		TryPatchit("BonelabSupport", "MarrowFusion.Bonelab.Messages.BodyLogEffectMessage", "OnHandleMessage", typeof(BodyLogEffect).GetMethod("Prefix"), null);
		IfDontHaveInstallThenDo("doge15567.BodyMallUI.Spawnable.BodyMallUI", 5469299);
		IfDontHaveInstallThenDo("Atlas.96.ModOMat.Spawnable.ModOMat", 4140942);
		NemesisAntiCheatPage = BoneLib.BoneMenu.Page.Root.CreatePage("+ Nemesis Anti-Cheat", Color.green);
		Protectsettings = NemesisAntiCheatPage.CreatePage("+ Settings", Color.cyan);
		playermessages = NemesisAntiCheatPage.CreatePage("+ Player Senders", Color.white);
		NemesisAntiCheatPage.CreateFunction("Revert Bodylog To Original", Color.yellow, delegate
		{
			ChangeBodyLogAvatarSlot(1, originalbodylog[0], notification: false);
			ChangeBodyLogAvatarSlot(2, originalbodylog[1], notification: false);
			ChangeBodyLogAvatarSlot(3, originalbodylog[2], notification: false);
			ChangeBodyLogAvatarSlot(4, originalbodylog[3], notification: false);
			ChangeBodyLogAvatarSlot(5, originalbodylog[4], notification: false);
			ChangeBodyLogAvatarSlot(6, originalbodylog[5]);
		});
		NemesisAntiCheatPage.CreateFunction("Revert Profile To Original", Color.yellow, delegate
		{
			LocalPlayer.Metadata.Metadata.TrySetMetadata("Nickname", originalprofiledetails.Nickname);
			LocalPlayer.Metadata.Metadata.TrySetMetadata("Username", originalprofiledetails.Username);
			LocalPlayer.Metadata.Metadata.TrySetMetadata("Description", originalprofiledetails.Description);
			NotificationNowAlways("Nemesis Anti-Cheat", "Reverted Profile Back To Original", NotificationType.SUCCESS, 3.5f);
		});
		NemesisAntiCheatPage.LogsettingsEnum("Local Despawn All Filter", Color.yellow, ref DespawnerAllReal2, delegate(DespawnerAll enabled)
		{
			DespawnerAllReal2 = enabled;
		});
		NemesisAntiCheatPage.CreateFunction("Locally Despawn All", Color.yellow, delegate
		{
			DespawnAll(DespawnerAllReal2, localOnly: true);
		});
		permissioneditornow = NemesisAntiCheatPage.CreatePage("+ Permission Editor", Color.white);
		playeroptions = NemesisAntiCheatPage.CreatePage("+ Online Player Options", Color.white);
		PlayersOnlinePage = playeroptions.CreatePage("+ Ban/UnBan Players Online", Color.white, 30);
		playeroptions.LogsettingsString("Find Player By Steam ID", Color.yellow, ref findem, delegate(string stringy)
		{
			findem = stringy;
			if (ulong.TryParse(findem, out var result))
			{
				FindPlayersLobbyFromPlayerSteamID(result, delegate(bool boolx, PlayerInfo playerinfo)
				{
					Menu.DisplayDialog("Fusion Info By Steam ID!", $"Username: {playerinfo.Username}\nNickname: {(string.IsNullOrEmpty(playerinfo.Nickname) ? "N/A" : playerinfo.Nickname)}\nAvatar Mod ID: {playerinfo.AvatarModID}\nAvatar Title: {playerinfo.AvatarTitle}");
				});
			}
		}).SetTooltip("Finds Players Active Fusion Profile If They Are Online ATM Then Displays It To You...");
		playeroptions.LogsettingsString("Steam ID To Ban", Color.yellow, ref SteamIDSearch, delegate(string stringy)
		{
			if (ulong.TryParse(stringy, out var _))
			{
				SteamIDSearch = stringy;
				NotificationNow("Nemesis Anti-Cheat", "Set Value " + SteamIDSearch, NotificationType.SUCCESS, 2f);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Failed Needs To Be A ULONG AKA STEAM ID!!!!!!!!!!!", NotificationType.ERROR, 2f);
			}
		});
		playeroptions.CreateFunction("UnBan/Ban Steam ID From Your Lobby", Color.yellow, delegate
		{
			string s = SteamIDSearch.Trim();
			if (ulong.TryParse(s, out var longynow))
			{
				FindPlayersLobbyFromPlayerSteamID(longynow, delegate(bool found, PlayerInfo playernowinfo)
				{
					if (found)
					{
						if (!NetworkHelper.IsBanned(longynow))
						{
							BanInfo item = new BanInfo
							{
								Player = playernowinfo,
								Reason = "Manually Banned [Nemesis Anti-Cheat]"
							};
							BanManager.BanList.Bans.RemoveAll((BanInfo info2) => info2.Player.PlatformID == longynow);
							BanManager.BanList.Bans.Add(item);
							DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
							NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
						}
						else
						{
							BanManager.Pardon(longynow);
							NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
						}
					}
					else if (!NetworkHelper.IsBanned(longynow))
					{
						BanInfo item2 = new BanInfo
						{
							Player = new PlayerInfo
							{
								Username = "Nemesis Anti-Cheat Banned",
								Nickname = "Nemesis Anti-Cheat Banned",
								PlatformID = longynow,
								Description = "Nemesis Anti-Cheat Banned",
								AvatarModID = -1,
								AvatarTitle = "Nemesis Anti-Cheat Banned"
							},
							Reason = "Manually Banned [Nemesis Anti-Cheat]"
						};
						BanManager.BanList.Bans.RemoveAll((BanInfo info2) => info2.Player.PlatformID == longynow);
						BanManager.BanList.Bans.Add(item2);
						DataSaver.WriteJsonToFile("bans.json", BanManager.BanList);
						NotificationNow("Nemesis Anti-Cheat", "Banned Player", NotificationType.SUCCESS);
					}
					else
					{
						BanManager.Pardon(longynow);
						NotificationNow("Nemesis Anti-Cheat", "UnBanned Player", NotificationType.SUCCESS);
					}
				});
			}
		});
		OnlineFriends = playeroptions.CreatePage("+ Online Friends", Color.white);
		fppubs = playeroptions.CreatePage("+ Nemesis Anti-Cheat Lobbies", Color.white);
		pubs = playeroptions.CreatePage("+ All Lobbies", Color.cyan);
		HOSTONLYPGE = NemesisAntiCheatPage.CreatePage("+ [HOST ONLY]", Color.white);
		OwnerOnlyPg = NemesisAntiCheatPage.CreatePage("+ [OWNER ONLY]", Color.white);
		OPERATORPG = NemesisAntiCheatPage.CreatePage("+ [OPERATOR ONLY]", Color.white);
		unblockingnow = NemesisAntiCheatPage.CreatePage("+ Unblock Stuff", Color.white);
		WarnedSpawnablesnow = unblockingnow.CreatePage("+ Warned Spawnables", Color.white);
		modidblockednow = unblockingnow.CreatePage("+ Block This Mod.IO Mod Completely", Color.white);
		BlockedSpawnablesnow = unblockingnow.CreatePage("+ Block This Spawnable", Color.white);
		SpawnablesKicknow = unblockingnow.CreatePage("+ Kick If Spawned", Color.white);
		blockentirepalletnow = unblockingnow.CreatePage("+ Block Pallet Completely", Color.white);
		blockentireauthornow = unblockingnow.CreatePage("+ Block Author Of Spawnable", Color.white);
		selfrestrictions = NemesisAntiCheatPage.CreatePage("+ Self Restrictions", Color.white);
		selfrestrictions.Logsettings("Disable Wind SFX", Color.cyan, ref disablewindsfx, delegate(bool enabled)
		{
			if (!disablewindsfx && enabled)
			{
				Il2CppArrayBase<WindBuffetSFX> il2CppArrayBase = UnityEngine.Object.FindObjectsOfType<WindBuffetSFX>();
				foreach (WindBuffetSFX item3 in il2CppArrayBase)
				{
					item3.windBuffetClip = null;
					item3._buffetSrc = null;
				}
			}
			disablewindsfx = enabled;
		});
		selfrestrictions.Logsettings("Disable Force Pull", Color.cyan, ref forcegrabdisablernow, delegate(bool enabled)
		{
			forcegrabdisablernow = enabled;
		});
		selfrestrictions.Logsettings("Visually Hide Holsters Self", Color.cyan, ref hideholsters, delegate(bool enabled)
		{
			if (!hideholsters && enabled)
			{
				HolsterHiderAll(null, activeNow: true);
			}
			hideholsters = enabled;
		});
		selfrestrictions.Logsettings("Visually Hide Holsters Players", Color.cyan, ref hideholstersplayers, delegate(bool enabled)
		{
			if (!hideholstersplayers && enabled)
			{
				foreach (NetworkPlayer item4 in NetworkPlayers(excludeMe: true))
				{
					HolsterHiderAll(item4, activeNow: true);
				}
			}
			hideholstersplayers = enabled;
		});
		selfrestrictions.Logsettings("Visually Disable Your Bodylog", Color.cyan, ref bodylog, delegate(bool enabled)
		{
			if (!bodylog && enabled)
			{
				ND_BodyLog(Player.PhysicsRig).bodylogreturn?.ballArt.gameObject.SetActive(value: true);
				ND_BodyLog(Player.PhysicsRig).Outerring.gameObject.SetActive(value: true);
			}
			bodylog = enabled;
		});
		selfrestrictions.Logsettings("Visually Disable Players Bodylog", Color.cyan, ref bodylogplayers, delegate(bool enabled)
		{
			if (!bodylogplayers && enabled)
			{
				foreach (NetworkPlayer item5 in NetworkPlayers(excludeMe: true))
				{
					(PullCordDevice, MeshRenderer) tuple = ND_BodyLog(item5.RigRefs.RigManager.physicsRig);
					tuple.Item1?.ballLine.gameObject.SetActive(value: true);
					tuple.Item1?.ballArt.gameObject.SetActive(value: true);
					tuple.Item2.gameObject.SetActive(value: true);
				}
			}
			bodylogplayers = enabled;
		});
		selfrestrictions.Logsettings("Disable Media Players", Color.cyan, ref disablemediaplayers, delegate(bool enabled)
		{
			disablemediaplayers = enabled;
		});
		selfrestrictions.Logsettings("Remove Grip Icons", Color.cyan, ref grippy, delegate(bool enabled)
		{
			if (!grippy && enabled)
			{
				Il2CppArrayBase<InteractableIcon> il2CppArrayBase = UnityEngine.Object.FindObjectsOfType<InteractableIcon>();
				foreach (InteractableIcon item6 in il2CppArrayBase)
				{
					item6.IconSize = 0f;
					item6.scaledIconSize = 0f;
				}
			}
			grippy = enabled;
		});
		NemesisAntiCheatPage.CreateFunction("Join Discord [For Updates]", Color.yellow, delegate
		{
			OpenPageNow("https://shorturl.at/YymS0");
			NotificationNow("Nemesis Anti-Cheat", "Opened Neon_Developer's Discord In Browser.", NotificationType.SUCCESS);
		});
		spawngunesst = NemesisAntiCheatPage.CreatePage("+ Spawngun Essentials", Color.green);
		spawngunesst.CreateFunction("Set Left Hand Item To Spawngun", Color.yellow, delegate
		{
			SetBarCodeToSpawnGun(ND_YourGetHand(WhichHand.Left).ND_GetMarrowEntity().ND_GetBarcodeID());
		});
		spawngunesst.CreateFunction("Set Right Hand Item To Spawngun", Color.yellow, delegate
		{
			SetBarCodeToSpawnGun(ND_YourGetHand(WhichHand.Right).ND_GetMarrowEntity().ND_GetBarcodeID());
		});
		spawngunesst.CreateFunction("Set Random Item To Spawngun", Color.yellow, delegate
		{
			SetBarCodeToSpawnGun(GetRandomByType(RandomizerType.AllSpawnables));
		});
		holdinginhands = NemesisAntiCheatPage.CreatePage("+ Holding In Hand Essentials", Color.green);
		holdinginhands.Logsettings("Remove Gun/Melee Sounds From Held", Color.cyan, ref removesounds, delegate(bool enabled)
		{
			removesounds = enabled;
		});
		holdinginhands.LogsettingsEnum("In Hand", Color.yellow, ref handnowreal, delegate(handnow enabled)
		{
			handnowreal = enabled;
		});
		holdinginhands.CreateFunction("Delete Sound From Held Item", Color.yellow, delegate
		{
			WhichHand hand = ((handnowreal == handnow.Left) ? WhichHand.Left : WhichHand.Right);
			Grip grip = ND_YourGetHand(hand)?.ND_GetAttachedObject()?.GetComponentInChildren<Grip>(includeInactive: true);
			if (grip != null)
			{
				RemoveSoundGrip(grip);
				NotificationNow("Nemesis Anti-Cheat", "Tried To Delete Sound Sources From " + BarcodeInHand().ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
			}
		});
		holdinginhands.CreateFunction("Copy Mod ID To Clipboard", Color.yellow, delegate
		{
			GUIUtility.systemCopyBuffer = CrateFilterer.GetModID(new SpawnableCrateReference(BarcodeInHand()).Crate.Pallet).ToString();
		});
		holdinginhands.CreateFunction("Despawn All Of This Item", Color.yellow, delegate
		{
			foreach (NetworkEntity item7 in NetworkEntities())
			{
				MarrowEntity marrowEntity = item7?.ND_GetMarrowEntity();
				if (marrowEntity != null && marrowEntity.ND_GetBarcodeID() == BarcodeInHand())
				{
					item7.ND_Despawn();
				}
			}
			NotificationNow("Nemesis Anti-Cheat", "Despawned Everything Matching " + BarcodeInHand().ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
		});
		holdinginhands.CreateFunction("Force Delete All Of This Locally", Color.yellow, delegate
		{
			foreach (NetworkEntity item8 in NetworkEntities())
			{
				MarrowEntity marrowEntity = item8?.ND_GetMarrowEntity();
				if (marrowEntity != null && marrowEntity.ND_GetBarcodeID() == BarcodeInHand())
				{
					marrowEntity.gameObject.DestroyNow();
				}
			}
			NotificationNow("Nemesis Anti-Cheat", "Force Deleted Locally Everything Matching " + BarcodeInHand().ND_BarcodeCrateName(), NotificationType.SUCCESS, 3.5f);
		});
		holdinginhands.CreateFunction("Add/Remove Block Author Of Spawnable", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(new SpawnableCrateReference(BarcodeInHand()).Crate.Pallet.Author, blockentireauthor, blockauthornowlist, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeAuthor() + " To Blocked Authors!.", "Removed " + BarcodeInHand().ND_BarcodeAuthor() + " From Blocked Authors!.");
		});
		holdinginhands.CreateFunction("Add/Remove Block Pallet Completely", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(StripColorTags(new SpawnableCrateReference(BarcodeInHand()).Crate.Pallet.name), blockentirepallet, blockpalletnowlist, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodePalletName() + " To Blocked Pallets!.", "Removed " + BarcodeInHand().ND_BarcodePalletName() + " From Blocked Pallets!.");
		});
		holdinginhands.CreateFunction("Copy Details To Clipboard", Color.yellow, delegate
		{
			Pallet pallet = new SpawnableCrateReference(BarcodeInHand()).Crate.Pallet;
			GUIUtility.systemCopyBuffer = $"Spawnable Searcher Information : {pallet?.name}\nMod IO : {CrateFilterer.GetModID(pallet)}\nBarcode ID : {BarcodeInHand()}\nPallet Author : {pallet?.Author}\nPallet Name : {pallet?.name}";
			NotificationNow("Nemesis Anti-Cheat", "Copied To Clipboard!", NotificationType.SUCCESS, 2f);
		});
		holdinginhands.CreateFunction("Add/Remove Kick If Spawned", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(BarcodeInHand(), SpawnablesKick, SpawnablesKickPath, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeCrateName() + " To Kick If Spawned!.", "Removed " + BarcodeInHand().ND_BarcodeCrateName() + " From Kick If Spawned!.");
		});
		holdinginhands.CreateFunction("Block/UnBlock This Spawnable", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(BarcodeInHand(), BlockedSpawnables, BlockedSpawnablesPath, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeCrateName() + " To Blocked Spawnables!.", "Removed " + BarcodeInHand().ND_BarcodeCrateName() + " From Blocked Spawnables!.");
		});
		holdinginhands.CreateFunction("Warn/UnWarn This Spawnable", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(BarcodeInHand(), WarnedSpawnables, WarnedSpawnablesPath, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeCrateName() + " To Warn Spawnables!.", "Removed " + BarcodeInHand().ND_BarcodeCrateName() + " From Warn Spawnables!.");
		});
		holdinginhands.CreateFunction("Add/Remove Block This Mod.IO Mod Completely", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(CrateFilterer.GetModID(new SpawnableCrateReference(BarcodeInHand()).Crate.Pallet).ToString(), modidblocked, ModIDBLOCKSPATH, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodePalletName() + " To Blocked Mod.IO Mod!.", "Removed " + BarcodeInHand().ND_BarcodePalletName() + " From Blocked Mod.IO Mod!.");
		});
		holdinginhands.CreateFunction("Un/Favorite Spawnable", Color.yellow, delegate
		{
			Il2CppSystem.Collections.Generic.List<string> list = DataManager.ActiveSave?.PlayerSettings?.FavoriteSpawnables;
			if (list != null)
			{
				if (!list.Contains(BarcodeInHand()))
				{
					list.Add(BarcodeInHand());
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeCrateName() + " To SaveGame Favorites!", NotificationType.SUCCESS);
				}
				else
				{
					list.Remove(BarcodeInHand());
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Removed " + BarcodeInHand().ND_BarcodeCrateName() + " From SaveGame Favorites!", NotificationType.SUCCESS);
				}
			}
		});
		holdinginhands.CreateFunction("Add/Remove Custom Favorites [Spawnable]", Color.yellow, delegate
		{
			ToggleAddRemoveFromFile(BarcodeInHand(), CustomSpawnFav, SpawnableCustomFav, "Nemesis Anti-Cheat", "Added " + BarcodeInHand().ND_BarcodeCrateName() + " To Custom Spawnable Favorites!.", "Removed " + BarcodeInHand().ND_BarcodeCrateName() + " From Custom Spawnable Favorites!.");
		});
		searchersnow = NemesisAntiCheatPage.CreatePage("+ Searchers", Color.green);
		avatarsearcher = searchersnow.CreatePage("+ Avatar Searcher", Color.green);
		spawnablesearch = searchersnow.CreatePage("+ Spawnable Searcher", Color.green);
		levelsearcher = searchersnow.CreatePage("+ Level Searcher", Color.green);
		Timersz = NemesisAntiCheatPage.CreatePage("+ Timers", Color.green);
		protectionstuff = NemesisAntiCheatPage.CreatePage("+ Protections", Color.cyan);
		Notifications = NemesisAntiCheatPage.CreatePage("+ Notifications", Color.cyan);
		Notifications.CreateFunction("Cancel Notifications", Color.yellow, delegate
		{
			Notifier.CancelAll();
		});
		Notifications.Logsettings("Anti Spam Spawning Detection", Color.red, ref notificationspamspawn, delegate(bool enabled)
		{
			notificationspamspawn = enabled;
		});
		Notifications.Logsettings("Invalid/Crash Stat Detection", Color.red, ref Invalidstatsnow, delegate(bool enabled)
		{
			Invalidstatsnow = enabled;
		});
		Notifications.Logsettings("Spoof Username Detections", Color.red, ref spoofedusernameusername, delegate(bool enabled)
		{
			spoofedusernameusername = enabled;
		});
		Notifications.Logsettings("Do Not Disturb", Color.red, ref donotdisturb, delegate(bool enabled)
		{
			donotdisturb = enabled;
		});
		Notifications.Logsettings("FP Banlist Popups", Color.cyan, ref globalbannotification, delegate(bool enabled)
		{
			globalbannotification = enabled;
		});
		Notifications.Logsettings("FP Blacklist Popups", Color.cyan, ref globalblocklistnotification, delegate(bool enabled)
		{
			globalblocklistnotification = enabled;
		});
		Notifications.Logsettings("Exploit Notifications", Color.cyan, ref notificationsofexploits, delegate(bool enabled)
		{
			notificationsofexploits = enabled;
		});
		Notifications.Logsettings("Strength Threshold", Color.green, ref strengthnotif, delegate(bool enabled)
		{
			strengthnotif = enabled;
		});
		Notifications.Logsettings("Alt Notifications", Color.cyan, ref AltNotifications, delegate(bool enabled)
		{
			AltNotifications = enabled;
		});
		Notifications.Logsettings("Alt Errors", Color.cyan, ref alterrornotis, delegate(bool enabled)
		{
			alterrornotis = enabled;
		});
		Notifications.Logsettings("Block Spawnables", Color.cyan, ref blockspwnnotis, delegate(bool enabled)
		{
			blockspwnnotis = enabled;
		});
		playerjoinlogsnow = NemesisAntiCheatPage.CreatePage("+ Recently Met Players", Color.cyan);
		SaveTotxtpg = protectionstuff.CreatePage("+ Save To Text File", Color.cyan);
		SaveTotxtpg.Logsettings("Save Recently Met Players Log", Color.white, ref logrecentlymet, delegate(bool enabled)
		{
			logrecentlymet = enabled;
		});
		SaveTotxtpg.Logsettings("Save Media Player Logs", Color.white, ref logmediaplayer, delegate(bool enabled)
		{
			logmediaplayer = enabled;
		});
		SaveTotxtpg.Logsettings("Save Lobbies Since Login", Color.white, ref loglobbiessince, delegate(bool enabled)
		{
			loglobbiessince = enabled;
		});
		SaveTotxtpg.Logsettings("Save Players Since Login", Color.white, ref logplayersince, delegate(bool enabled)
		{
			logplayersince = enabled;
		});
		Mostusedprotections = protectionstuff.CreatePage("+ Most Used Protections", Color.cyan);
		Mostusedprotections.Logsettings("Anti Freeze Player", Color.cyan, ref AntiModTP, delegate(bool enabled)
		{
			AntiModTP = enabled;
		}).SetTooltip("This makes it so owners can't teleport you to prevent menus who spam the teleport packet to freeze you!");
		Mostusedprotections.Logsettings("Delete Last Lobby Mods", Color.white, ref DeleteLastLobbyMods, delegate(bool enabled)
		{
			DeleteLastLobbyMods = enabled;
		});
		Mostusedprotections.Logsettings("Prevent Notification Lag", Color.cyan, ref preventnotificationlag, delegate(bool enabled)
		{
			preventnotificationlag = enabled;
		});
		Mostusedprotections.Logsettings("Respawn Protection", Color.cyan, ref fullspawnprotection, delegate(bool enabled)
		{
			fullspawnprotection = enabled;
		});
		Mostusedprotections.Logsettingsint("Respawn Protection Timer", Color.green, ref spawnprotectiontimer, 1, 1, 60, delegate(int intnow)
		{
			spawnprotectiontimer = intnow;
		});
		Mostusedprotections.Logsettings("Despawn On Disconnect", Color.cyan, ref cleandisconnect, delegate(bool enabled)
		{
			cleandisconnect = enabled;
		});
		Mostusedprotections.Logsettings("Some Protections [No Host]", Color.cyan, ref spawnprotectionsnot_host, delegate(bool enabled)
		{
			spawnprotectionsnot_host = enabled;
		});
		Mostusedprotections.Logsettings("Avatar Switch Protection", Color.cyan, ref aviswitchprotection, delegate(bool enabled)
		{
			aviswitchprotection = enabled;
		});
		Mostusedprotections.Logsettings("Anti Grief Bodylog", Color.cyan, ref AntiBodyLogGrief, delegate(bool enabled)
		{
			AntiBodyLogGrief = enabled;
		});
		Mostusedprotections.Logsettings("Anti Gravity Change", Color.cyan, ref AntiGravityChange, delegate(bool enabled)
		{
			AntiGravityChange = enabled;
		});
		Mostusedprotections.Logsettings("Anti Grab", Color.cyan, ref AntiGrab, delegate(bool enabled)
		{
			AntiGrab = enabled;
		});
		Mostusedprotections.Logsettings("Anti Despawn EffectS", Color.cyan, ref antidespawneffect, delegate(bool enabled)
		{
			antidespawneffect = enabled;
		});
		Mostusedprotections.Logsettings("Anti Bodylog Effects", Color.cyan, ref antibodylogeffect, delegate(bool enabled)
		{
			antibodylogeffect = enabled;
		});
		Mostusedprotections.Logsettings("TP To Spawn Threshold", Color.cyan, ref TeleportThresHold, delegate(bool enabled)
		{
			TeleportThresHold = enabled;
		});
		Mostusedprotections.Logsettingsfloat("TP To Spawn Value", Color.cyan, ref speedthreshold, 1f, 30f, 1000f, delegate(float value)
		{
			speedthreshold = value;
		});
		Mostusedprotections.Logsettings("Block Net Spawns Locally", Color.cyan, ref blockallspawnslocally, delegate(bool enabled)
		{
			blockallspawnslocally = enabled;
		});
		featuresprotection = protectionstuff.CreatePage("+ Protection Features", Color.cyan);
		featuresprotection.Logsettings("Warn Avatar Change", Color.cyan, ref warnavinow, delegate(bool enabled)
		{
			warnavinow = enabled;
		});
		featuresprotection.Logsettings("Block Avatar Author", Color.cyan, ref blockaviauthornow, delegate(bool enabled)
		{
			blockaviauthornow = enabled;
		});
		featuresprotection.Logsettings("Block Avatar Pallet", Color.cyan, ref blockavipalletnow, delegate(bool enabled)
		{
			blockavipalletnow = enabled;
		});
		featuresprotection.Logsettings("Block Avatars As Spawnables", Color.cyan, ref BLOCKAVATARSASSPAWNABLES, delegate(bool enabled)
		{
			BLOCKAVATARSASSPAWNABLES = enabled;
		});
		featuresprotection.Logsettings("Block Pallet Completely", Color.cyan, ref BlockPalletCompletely, delegate(bool enabled)
		{
			BlockPalletCompletely = enabled;
		});
		featuresprotection.Logsettings("Block Author Of Spawnable", Color.cyan, ref BlockAuthorOfSpawnable, delegate(bool enabled)
		{
			BlockAuthorOfSpawnable = enabled;
		});
		featuresprotection.Logsettings("Block This Mod.IO Mod Completely", Color.cyan, ref ModIDBlocker, delegate(bool enabled)
		{
			ModIDBlocker = enabled;
		});
		featuresprotection.Logsettings("Block/UnBlock This Spawnable", Color.cyan, ref blockedspawnables, delegate(bool enabled)
		{
			blockedspawnables = enabled;
		});
		featuresprotection.Logsettings("Warn/UnWarn This Spawnable", Color.cyan, ref warnedspawnables, delegate(bool enabled)
		{
			warnedspawnables = enabled;
		});
		ProtectionLogs = protectionstuff.CreatePage("+ Logs", Color.green);
		protectionstuff.Logsettings("Force Nametags On", Color.cyan, ref forcenametagson, delegate(bool enabled)
		{
			forcenametagson = enabled;
		});
		protectionstuff.Logsettings("Remove Proximity Chat", Color.cyan, ref removeproxchat, delegate(bool enabled)
		{
			removeproxchat = enabled;
		});
		protectionstuff.Logsettings("Anti Decals", Color.cyan, ref antidecal, delegate(bool enabled)
		{
			antidecal = enabled;
		});
		protectionstuff.Logsettings("Block Exploits", Color.cyan, ref blockexploitscompletely, delegate(bool enabled)
		{
			blockexploitscompletely = enabled;
		});
		teleporters = NemesisAntiCheatPage.CreatePage("+ Teleporters", Color.green);
		teleporters.LogsettingsString("Teleporter Name", Color.yellow, ref teleportername, delegate(string stringy)
		{
			teleportername = stringy;
		});
		teleporters.CreateFunction("Add Teleporter", Color.yellow, delegate
		{
			bool flag = false;
			foreach (TeleporterManager item9 in TeleporterManager.Teleportersnowx)
			{
				if (string.Equals(item9.TitleOfTeleporter, teleportername, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				TeleporterManager.Teleportersnowx.Add(new TeleporterManager(StripColorTags(new LevelCrateReference(SceneStreamer.Session?.Level?.Barcode?.ID).Scannable.Title), new Vector3(Player.RigManager.physicsRig.m_head.transform.position.x, Player.RigManager.physicsRig.m_head.transform.position.y, Player.RigManager.physicsRig.m_head.transform.position.z), new Quaternion(Player.RigManager.physicsRig.m_head.transform.rotation.x, Player.RigManager.physicsRig.m_head.transform.rotation.y, Player.RigManager.physicsRig.m_head.transform.rotation.z, Player.RigManager.physicsRig.m_head.transform.rotation.w), teleportername, SceneStreamer.Session.Level.Barcode.ID));
				TeleporterManager.SaveTeleporters();
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "This Teleporter Exists Already!", NotificationType.WARNING, 2.5f);
			}
		});
		teleportersnow = teleporters.CreatePage("+ Active Teleporters", Color.yellow);
		cheatspreset = NemesisAntiCheatPage.CreatePage("+ Dev Tool Presets", Color.green);
		cheatspreset.Logsettings("Everything Is Fusion Local Only", Color.cyan, ref localonlydevtools, delegate(bool enabled)
		{
			localonlydevtools = enabled;
		});
		cheatspreset.LogsettingsString("Preset Name", Color.yellow, ref CHEATPRS, delegate(string stringy)
		{
			CHEATPRS = stringy;
		});
		cheatspreset.CreateFunction("Add Preset", Color.yellow, delegate
		{
			CreateDevToolPreset(CHEATPRS, new CreateCheatToolsPreset.Item
			{
				SpawnableName = StripColorTags(new SpawnableCrateReference("c1534c5a-5747-42a2-bd08-ab3b47616467").Crate.name),
				BarcodeId = "c1534c5a-5747-42a2-bd08-ab3b47616467",
				LocalSpawn = false,
				ModIoID = CrateFilterer.GetModID(new SpawnableCrateReference("c1534c5a-5747-42a2-bd08-ab3b47616467").Crate.Pallet)
			}, new CreateCheatToolsPreset.Item
			{
				SpawnableName = StripColorTags(new SpawnableCrateReference("c1534c5a-6b38-438a-a324-d7e147616467").Crate.name),
				BarcodeId = "c1534c5a-6b38-438a-a324-d7e147616467",
				LocalSpawn = false,
				ModIoID = CrateFilterer.GetModID(new SpawnableCrateReference("c1534c5a-6b38-438a-a324-d7e147616467").Crate.Pallet)
			}, new CreateCheatToolsPreset.Item
			{
				SpawnableName = "Empty",
				BarcodeId = "Empty",
				LocalSpawn = false,
				ModIoID = -1
			}, new CreateCheatToolsPreset.Item
			{
				SpawnableName = "Empty",
				BarcodeId = "Empty",
				LocalSpawn = false,
				ModIoID = -1
			}, new CreateCheatToolsPreset.Item
			{
				SpawnableName = "Empty",
				BarcodeId = "Empty",
				LocalSpawn = false,
				ModIoID = -1
			});
		});
		cheatspresetsnow = cheatspreset.CreatePage("+ Active Presets", Color.green);
		bodylognowpage = NemesisAntiCheatPage.CreatePage("+ Bodylog Pages", Color.green);
		bodylognowpage.LogsettingsString("Preset Name", Color.yellow, ref bodylogpagename, delegate(string stringy)
		{
			bodylogpagename = stringy;
		});
		bodylognowpage.CreateFunction("Add Preset", Color.yellow, delegate
		{
			CreateBodyLogPage(bodylogpagename);
		});
		bodylognowpagexx = bodylognowpage.CreatePage("+ Active Presets", Color.green);
		loadoutpages = NemesisAntiCheatPage.CreatePage("+ Loadouts", Color.green);
		loadoutpages.LogsettingsString("Loadout Name", Color.yellow, ref loadoutname, delegate(string stringy)
		{
			loadoutname = stringy;
		});
		loadoutpages.CreateFunction("Add Loadout", Color.yellow, delegate
		{
			InventoryPage.CaptureFromCurrentInventory(loadoutname);
		});
		loadoutpagesnow = loadoutpages.CreatePage("+ Active Loadouts", Color.green);
		loadoutpages.Logsettings("Keep Loadout", Color.cyan, ref KEEPLOADOUTINVENTORY, delegate(bool enabled)
		{
			KEEPLOADOUTINVENTORY = enabled;
		});
		colorpresets = NemesisAntiCheatPage.CreatePage("+ Bodylog & RadialMenu Color Presets", Color.green);
		colorpresets.Logsettings("Bodylog & Radial Menu Colors", Color.cyan, ref Bodylogradialcolors, delegate(bool enabled)
		{
			Bodylogradialcolors = enabled;
		});
		colorpresets.LogsettingsString("Preset Name", Color.yellow, ref colornamenowx, delegate(string stringy)
		{
			colornamenowx = stringy;
		});
		colorpresets.CreateFunction("Add Preset", Color.yellow, delegate
		{
			bool flag = false;
			foreach (BodyLogRadialMenuColorPreset colorPreset in BodyLogRadialMenuColorPreset.ColorPresets)
			{
				if (string.Equals(colorPreset.TitleOfPreset, colornamenowx, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				BodyLogRadialMenuColorPreset.ColorPresets.Add(new BodyLogRadialMenuColorPreset(colornamenowx, 0f, 255f, 0f, 255f, 0f, 255f, 0f, 255f, 0f, 255f, 0f, 255f, 0f, 255f, 0f, 255f));
				BodyLogRadialMenuColorPreset.SavePresets();
				NotificationNow("Nemesis Anti-Cheat", "Added Preset " + colornamenowx + "!", NotificationType.SUCCESS, 2.5f);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
			}
		});
		colorpresetsnow = colorpresets.CreatePage("+ Active Presets", Color.green);
		FusionProfiles = NemesisAntiCheatPage.CreatePage("+ Fusion Profile Presets", Color.green);
		FusionProfiles.LogsettingsString("Preset Name", Color.yellow, ref profilepresz, delegate(string stringy)
		{
			profilepresz = stringy;
		});
		FusionProfiles.CreateFunction("Add Preset", Color.yellow, delegate
		{
			if (string.IsNullOrWhiteSpace(profilepresz))
			{
				NotificationNow("Nemesis Anti-Cheat", "Preset name cannot be empty!", NotificationType.WARNING, 2.5f);
			}
			else if (!NetworkInfo.HasServer)
			{
				NotificationNow("Nemesis Anti-Cheat", "Start Or Join A Lobby To Store Your Preset!", NotificationType.ERROR, 2.5f);
			}
			else
			{
				NetworkPlayer networkPlayer = ND_YourNetworkPlayer();
				if (networkPlayer == null)
				{
					NotificationNow("Nemesis Anti-Cheat", "Player not found.", NotificationType.ERROR, 2.5f);
				}
				else
				{
					string presetName = profilepresz.Trim();
					if (FusionProfilePresets.ProfilePresets.Any((FusionProfilePresets t) => string.Equals(t.TitleOfPreset, presetName, StringComparison.OrdinalIgnoreCase)))
					{
						NotificationNow("Nemesis Anti-Cheat", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
					}
					else
					{
						string nickname = networkPlayer.ND_Nickname();
						string description = networkPlayer.ND_Description();
						string avatarAtTheTime = ND_YourAvatarBarcodeID();
						FusionProfilePresets item = new FusionProfilePresets(presetName, nickname, description, avatarAtTheTime, InternalServerHelpers.GetInitialEquippedItems());
						FusionProfilePresets.ProfilePresets.Add(item);
						FusionProfilePresets.SavePresets();
						NotificationNow("Nemesis Anti-Cheat", "Added Preset " + presetName + "!", NotificationType.SUCCESS, 2.5f);
					}
				}
			}
		});
		FusionProfilesnow = FusionProfiles.CreatePage("+ Active Presets", Color.green);
		NemesisAntiCheatPage.CreateFunction("Spawn BodyMall UI", Color.yellow, delegate
		{
			foreach (Poolee item10 in UnityEngine.Object.FindObjectsOfType<Poolee>())
			{
				if (item10.gameObject.name.Contains("BodyMallUI"))
				{
					item10.gameObject.DestroyNow();
				}
			}
			SpawnIt("doge15567.BodyMallUI.Spawnable.BodyMallUI", Player.RigManager.physicsRig.m_head.position + Player.RigManager.physicsRig.m_head.forward + Player.RigManager.physicsRig.m_head.up, Quaternion.identity, localonly: true);
		});
		NemesisAntiCheatPage.CreateFunction("Teleport To Spawn", Color.yellow, delegate
		{
			if (!GamemodeManager.IsGamemodeStarted)
			{
				LocalPlayer.TeleportToCheckpoint();
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Can't Do That Now!", NotificationType.ERROR, 2f);
			}
		});
		NemesisAntiCheatPage.CreateFunction("Give Ammo", Color.yellow, delegate
		{
			if (!GamemodeManager.IsGamemodeStarted)
			{
				LocalInventory.AddAmmo(99999999);
			}
			else
			{
				NotificationNow("Nemesis Anti-Cheat", "Can't Do That Now!", NotificationType.ERROR, 2f);
			}
		});
		NemesisAntiCheatPage.Logsettings("Auto Run", Color.cyan, ref autorunnow, delegate(bool enabled)
		{
			autorunnow = enabled;
		});
		levelsearcher.LogsettingsEnum("Search Method", Color.yellow, ref searchmethodlevelreal, delegate(SearchMethod enabled)
		{
			searchmethodlevelreal = enabled;
		});
		levelsearcher.LogsettingsString("Match", Color.yellow, ref levelsrch, delegate(string stringy)
		{
			levelsrch = stringy;
		});
		levelsearcher.CreateFunction("Find Results", Color.yellow, delegate
		{
			MelonCoroutines.Start(Search(levelsrch, levelresults, searchmethodlevelreal, SearchMethodType.Level, delegate(string barcode)
			{
				LevelCrateReference levelCrateReference = new LevelCrateReference(barcode);
				SceneStreamer.Load(levelCrateReference.Barcode);
			}));
		});
		levelresults = levelsearcher.CreatePage("+ Level Searcher Results", Color.green);
		levelhistory = levelsearcher.CreatePage("+ Search History", Color.green);
		Protectsettings.CreateFunction("Manually Save Settings", Color.green, delegate
		{
			ManuallySave();
		});
		Protectsettings.Logsettings("Auto Save Settings", Color.cyan, ref autosavenow, delegate(bool enabled)
		{
			autosavenow = enabled;
		});
		Protectsettings.Logsettingsint("Kick For Mins", Color.green, ref kicktime, 1, 1, 1000, delegate(int intnow)
		{
			kicktime = intnow;
		});
		Protectsettings.Logsettings("Hide FP From Lobbies", Color.green, ref HideNemesisAntiCheat, delegate(bool enabled)
		{
			if (HideNemesisAntiCheat && !enabled)
			{
				ModuleMessageManager.RegisterHandler<SendNotificationMessage>();
				ModuleMessageManager.RegisterHandler<ProtectorPingMessage>();
			}
			HideNemesisAntiCheat = enabled;
		}).SetTooltip("Disables \"Using Nemesis Anti-Cheat Message Ping\"\r\nDisables \"Player Messaging\"\r\nDisables \"Messaging To Players With Reasons Why They Was Kicked [If Using FP]\"\r\nDisables \"FP-000 Code From Your Lobby & Also Doesnt Show Your Lobby In Nemesis Anti-Cheat Lobbies\"\r\nThis Hides from mods that try to snipe you because your using a actual good mod.");
		Protectsettings.CreateFunction("Set Current Level As Home World", Color.yellow, delegate
		{
			File.WriteAllText(homeworldnow, SceneStreamer.Session?.Level?.Barcode?.ID);
			NotificationNow("Nemesis Anti-Cheat", "Made " + StripColorTags(new LevelCrateReference(SceneStreamer.Session?.Level?.Barcode?.ID).Crate.name) + " Your Home World!", NotificationType.SUCCESS, 3.5f);
		});
		Protectsettings.CreateFunction("Reload Website Data", Color.green, delegate
		{
			MelonCoroutines.Start(SiteStuff.UpdateSites());
		});
		Protectsettings.Logsettings("Keep Hidden Mods", Color.cyan, ref keephiddenmods, delegate(bool enabled)
		{
			keephiddenmods = enabled;
		});
		Protectsettings.Logsettings("Global FP Banlist", Color.cyan, ref globalFPBANLIST, delegate(bool enabled)
		{
			globalFPBANLIST = enabled;
		});
		Protectsettings.Logsettings("Global FP Blacklist", Color.cyan, ref globalblocklistnow, delegate(bool enabled)
		{
			globalblocklistnow = enabled;
		});
		Protectsettings.Logsettings("Spawn Gun UI Alway", Color.cyan, ref spawngunuialways, delegate(bool enabled)
		{
			spawngunuialways = enabled;
		});
		Protectsettings.Logsettingsfloat("Emergency Escape Seconds", Color.green, ref timerfoeesa, 1f, 1f, 170f, delegate(float intnow)
		{
			timerfoeesa = intnow;
			EmergencyEscapetimer.Refresh();
			EmergencyEscapetimer.Start(quicker: true, timerfoeesa);
		});
		Protectsettings.Logsettings("Emergency Escape Seconds Ago", Color.cyan, ref tpback10seconds, delegate(bool enabled)
		{
			tpback10seconds = enabled;
		});
		Protectsettings.Logsettings("Send Bodylog To Players", Color.cyan, ref bodylogsending, delegate(bool enabled)
		{
			bodylogsending = enabled;
		});
		Protectsettings.Logsettings("Send Mod.IO ID# To Players", Color.cyan, ref modidsending, delegate(bool enabled)
		{
			modidsending = enabled;
		});
		Protectsettings.Logsettings("Share Bodylog Pages To Players", Color.cyan, ref sharebodylogpagenow, delegate(bool enabled)
		{
			sharebodylogpagenow = enabled;
		});
		Protectsettings.Logsettings("Share DevToolPresets To Players", Color.cyan, ref sharedevtoolpresets, delegate(bool enabled)
		{
			sharedevtoolpresets = enabled;
		});
		Protectsettings.Logsettings("Send Code Mod Base64", Color.cyan, ref base64files, delegate(bool enabled)
		{
			base64files = enabled;
		});
		Protectsettings.Logsettings("Send Bits To Players", Color.cyan, ref bitsending, delegate(bool enabled)
		{
			bitsending = enabled;
		});
		Protectsettings.Logsettings("Mod O Mat On Level Load", Color.cyan, ref modomatonload, delegate(bool enabled)
		{
			modomatonload = enabled;
		});
		Protectsettings.Logsettings("Auto Kick Outdated FP Players", Color.cyan, ref autokickoldNemesisAntiCheatusers, delegate(bool enabled)
		{
			autokickoldNemesisAntiCheatusers = enabled;
		});
		Protectsettings.Logsettings("Remove Fusion Global Banlist", Color.white, ref REMOVEDGLOBALBANLIST, delegate(bool enabled)
		{
			REMOVEDGLOBALBANLIST = enabled;
		});
		Protectsettings.Logsettings("Player Messaging", Color.cyan, ref playermessaging, delegate(bool enabled)
		{
			playermessaging = enabled;
		});
		Protectsettings.Logsettings("Media Player Protection", Color.cyan, ref mediaplayerprotection, delegate(bool enabled)
		{
			mediaplayerprotection = enabled;
		});
		Protectsettings.Logsettings("Nemesis Anti-Cheat Lobby", Color.cyan, ref nemesisprotectedlobby, delegate(bool enabled)
		{
			nemesisprotectedlobby = enabled;
			NetworkHelper.RefreshServerCode();
		});
		Protectsettings.CreateFunction("Log Installed Mods MOD.IO ID'S", Color.yellow, delegate
		{
			Il2CppSystem.Collections.Generic.List<Pallet> pallets = AssetWarehouse.Instance.GetPallets();
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			Il2CppSystem.Collections.Generic.List<Pallet>.Enumerator enumerator = pallets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Pallet current = enumerator.Current;
				string item = $"[Name : {current.name} Author : {current.Author}]  => # ID {CrateFilterer.GetModID(current)}";
				list.Add(item);
			}
			string[] value = list.ToArray();
			string systemCopyBuffer = string.Join(Environment.NewLine, value);
			GUIUtility.systemCopyBuffer = systemCopyBuffer;
			NotificationNow("Nemesis Anti-Cheat", "All Installed Mods Mod.io ID'S Copied To Clipboard.", NotificationType.SUCCESS);
		});
		Protectsettings.Logsettings("Drop Stuff Before Despawn All", Color.cyan, ref dropallbefore, delegate(bool enabled)
		{
			dropallbefore = enabled;
		});
		Protectsettings.Logsettings("Randomizers Use SLZ Items Only", Color.cyan, ref randomizerslzonly, delegate(bool enabled)
		{
			if (!randomizerslzonly && enabled)
			{
				MelonCoroutines.Start(LoadAssetsEnum(randomizerslzonly));
				MelonLogger.Warning("Randomizer Use SLZ Items Only : " + randomizerslzonly);
			}
			randomizerslzonly = enabled;
		});
		Protectsettings.Logsettings("Save Bool Setting On Toggle", Color.cyan, ref togglesavesbool, delegate(bool enabled)
		{
			togglesavesbool = enabled;
		});
		Protectsettings.Logsettings("Clear Protection Logs On Join Server", Color.cyan, ref clientexploitclearonnewserver, delegate(bool enabled)
		{
			clientexploitclearonnewserver = enabled;
		});
		Protectsettings.Logsettings("Clear Spawn Logs On Join Server", Color.cyan, ref spawnlogexploitclearonnewserver, delegate(bool enabled)
		{
			spawnlogexploitclearonnewserver = enabled;
		});
		Protectsettings.Logsettings("Clear Avi Switch Logs On Join Server", Color.cyan, ref switchlogexploitclearonnewserver, delegate(bool enabled)
		{
			switchlogexploitclearonnewserver = enabled;
		});
		NemesisAntiCheatPage.CreateFunction("Rejoin Out Of Bounds Server", Color.yellow, delegate
		{
			if (!string.IsNullOrEmpty(outofboundslobbycode))
			{
				NetworkHelper.Disconnect();
				NetworkHelper.JoinServerByCode(outofboundslobbycode);
			}
		});
		NemesisAntiCheatPage.CreateFunction("Rejoin Last Server", Color.yellow, delegate
		{
			if (!string.IsNullOrEmpty(rejoinlastserver))
			{
				NetworkHelper.Disconnect();
				NetworkHelper.JoinServerByCode(rejoinlastserver);
			}
		});
		NemesisAntiCheatPage.CreateFunction("Show Item Information ID's In Hands", Color.yellow, delegate
		{
			Hand hand = ND_YourGetHand(WhichHand.Left);
			Hand hand2 = ND_YourGetHand(WhichHand.Right);
			string text = hand?.ND_GetMarrowEntity()?.ND_GetBarcodeID();
			string text2 = hand2?.ND_GetMarrowEntity()?.ND_GetBarcodeID();
			string text3 = ((text != null) ? ("Left Hand : [" + text.ND_BarcodeCrateName() + "] " + text) : "Left Hand : [Empty] Empty");
			string text4 = ((text2 != null) ? ("Right Hand : [" + text2.ND_BarcodeCrateName() + "] " + text2) : "Right Hand : [Empty] Empty");
			string message = text3 + "\n" + text4;
			Menu.DisplayDialog("Information In Hands", message);
		});
		NemesisAntiCheatPage.Logsettingsint("Bodylog Slot", Color.green, ref currentbodylogindex, 1, 1, 6, delegate(int intnow)
		{
			currentbodylogindex = intnow;
		});
		NemesisAntiCheatPage.CreateFunction("Set Current Avatar", Color.yellow, delegate
		{
			ChangeBodyLogAvatarSlot(currentbodylogindex, ND_YourAvatarBarcodeID());
		});
		Timersz.LogsettingsEnum("Despawn Timer Filter", Color.yellow, ref DespawnerTimerAllReal, delegate(DespawnerAll enabled)
		{
			DespawnerTimerAllReal = enabled;
		});
		Timersz.Logsettings("Despawn All", Color.cyan, ref DespawnAllTimer, delegate(bool enabled)
		{
			DespawnAllTimer = enabled;
		});
		Timersz.Logsettingsint("Despawn All Timer", Color.green, ref DespawnAllTimerMins, 1, 1, 1000, delegate(int intnow)
		{
			DespawnAllTimerMins = intnow;
			DespawnAllTimera.Refresh(null, DespawnAllTimerMins);
			DespawnAllTimera.Start(quicker: true, DespawnAllTimerMins);
		});
		avatarsearcher.LogsettingsEnum("Avatar Search Type", Color.yellow, ref AvatarSearchTypeReal, delegate(AvatarSearchType enabled)
		{
			AvatarSearchTypeReal = enabled;
		});
		avatarsearcher.LogsettingsEnum("Search Method", Color.yellow, ref searchmethodavatarreal, delegate(SearchMethod enabled)
		{
			searchmethodavatarreal = enabled;
		});
		avatarsearcher.Logsettingsint("SetBodylog Slot", Color.green, ref bodylogindex, 1, 1, 6, delegate(int intnow)
		{
			bodylogindex = intnow;
		});
		avatarsearcher.LogsettingsString("Match", Color.yellow, ref searchavi, delegate(string stringy)
		{
			searchavi = stringy;
		});
		avatarsearcher.CreateFunction("Find Results", Color.yellow, delegate
		{
			MelonCoroutines.Start(Search(searchavi, aviresults, searchmethodavatarreal, SearchMethodType.Avatar, delegate(string barcode)
			{
				AvatarsearcherLessCode(barcode);
			}));
		});
		aviresults = avatarsearcher.CreatePage("+ Avatar Searcher Results", Color.green);
		avisearchhistory = avatarsearcher.CreatePage("+ Search History", Color.green);
		spawnablesearch.LogsettingsEnum("Spawnable Search Type", Color.yellow, ref spawnablesrchtype, delegate(SpawnableSearchType enabled)
		{
			spawnablesrchtype = enabled;
		});
		spawnablesearch.LogsettingsEnum("Search Method", Color.yellow, ref searchspawnabletypereal, delegate(SearchMethod enabled)
		{
			searchspawnabletypereal = enabled;
		});
		spawnablesearch.LogsettingsString("Match", Color.yellow, ref spwnblesearch, delegate(string stringy)
		{
			spwnblesearch = stringy;
		});
		spawnablesearch.CreateFunction("Find Results", Color.yellow, delegate
		{
			MelonCoroutines.Start(Search(spwnblesearch, spawnableresults, searchspawnabletypereal, SearchMethodType.Spawnable, delegate(string barcode)
			{
				SpawnerFuncLessCode(barcode);
			}));
		});
		spawnableresults = spawnablesearch.CreatePage("+ Spawnable Searcher Results", Color.green);
		spawnablehistory = spawnablesearch.CreatePage("+ Search History", Color.green);
		NemesisAntiCheatPage.Logsettings("Show Ammo Always", Color.cyan, ref showammoalways, delegate(bool enabled)
		{
			showammoalways = enabled;
		});
		NemesisAntiCheatPage.Logsettings("Personal Space", Color.cyan, ref personalspace, delegate(bool enabled)
		{
			personalspace = enabled;
		});
		NemesisAntiCheatPage.Logsettingsfloat("Personal Space Distance", Color.green, ref personalspacevalue, 0.1f, 0.5f, 30f, delegate(float floatnow)
		{
			personalspacevalue = floatnow;
		});
		NemesisAntiCheatPage.Logsettings("Unlimited Ammo", Color.cyan, ref unlammo, delegate(bool enabled)
		{
			unlammo = enabled;
		});
		NemesisAntiCheatPage.CreateFunction("Dump Game Pallets", Color.yellow, delegate
		{
			MelonCoroutines.Start(DumpPalletsCoroutine());
		});
		Hostonlyoptions();
		OPERATORoptions();
		Owneroptionsonly();
		NotificationNow("Nemesis Anti-Cheat Built!", "Version 1.0.0", NotificationType.SUCCESS, 6f);
		MelonCoroutines.Start(RunAfterBuild());
		static void AvatarsearcherLessCode(string barcode)
		{
			switch (AvatarSearchTypeReal)
			{
			case AvatarSearchType.ChangeInto:
				ChangeIntoAvi(barcode);
				break;
			case AvatarSearchType.CopyDetailsToClipboard:
			{
				AvatarCrateReference avatarCrateReference = new AvatarCrateReference(barcode);
				GUIUtility.systemCopyBuffer = $"Barcode ID : {avatarCrateReference.Barcode.ID}\nPallet Name : {StripColorTags(avatarCrateReference.Crate.Pallet.name)}\nPallet Author : {avatarCrateReference.Crate.Pallet.Author}";
				break;
			}
			case AvatarSearchType.SetBodyLog:
				ChangeBodyLogAvatarSlot(bodylogindex, barcode);
				break;
			}
		}
		static void SpawnerFuncLessCode(string barcode)
		{
			switch (spawnablesrchtype)
			{
			case SpawnableSearchType.Spawn:
				SpawnIt(barcode, ND_YourGetHand(WhichHand.Left).transform.position + ND_YourGetHand(WhichHand.Left).transform.forward + ND_YourGetHand(WhichHand.Left).transform.up, Quaternion.identity);
				break;
			case SpawnableSearchType.CopyDetailsToClipboard:
				GUIUtility.systemCopyBuffer = barcode;
				NotificationNow("Nemesis Anti-Cheat", "Copied Barcode To Clipboard! " + barcode, NotificationType.WARNING, 3f);
				break;
			case SpawnableSearchType.UnFavoriteAndFavorite:
				if (!DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Contains(barcode))
				{
					DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Add(barcode);
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Added " + barcode + " To SaveGame Favorites!", NotificationType.SUCCESS);
				}
				else
				{
					DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Remove(barcode);
					DataManager.TrySaveActiveSave(SaveFlags.Progression);
					NotificationNow("Nemesis Anti-Cheat", "Removed " + barcode + " From SaveGame Favorites!", NotificationType.SUCCESS);
				}
				break;
			case SpawnableSearchType.DespawnAllOfThis:
				foreach (NetworkEntity item11 in NetworkEntities())
				{
					if (item11.ND_GetMarrowEntity().ND_GetBarcodeID() == barcode)
					{
						item11.ND_Despawn();
					}
				}
				NotificationNow("Nemesis Anti-Cheat", "Despawned Everything Matching " + StripColorTags(new SpawnableCrateReference(barcode).Crate.name), NotificationType.SUCCESS, 3.5f);
				break;
			case SpawnableSearchType.SetInSpawnGun:
				SetBarCodeToSpawnGun(barcode);
				break;
			}
		}
	}
}
