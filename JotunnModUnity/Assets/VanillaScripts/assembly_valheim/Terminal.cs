using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Terminal : MonoBehaviour
{
	public class ConsoleEventArgs
	{
		public string[] Args;

		public string FullLine;

		public Terminal Context;

		public int Length => Args.Length;

		public string this[int i] => Args[i];

		public ConsoleEventArgs(string line, Terminal context)
		{
			Context = context;
			FullLine = line;
			Args = line.Split(' ');
		}
	}

	public class ConsoleCommand
	{
		public string Command;

		public string Description;

		public bool IsCheat;

		public bool IsNetwork;

		public bool OnlyServer;

		public bool IsSecret;

		public bool AllowInDevBuild;

		private ConsoleEvent action;

		private ConsoleOptionsFetcher m_tabOptionsFetcher;

		private List<string> m_tabOptions;

		public ConsoleCommand(string command, string description, ConsoleEvent action, bool isCheat = false, bool isNetwork = false, bool onlyServer = false, bool isSecret = false, bool allowInDevBuild = false, ConsoleOptionsFetcher optionsFetcher = null)
		{
			commands[command.ToLower()] = this;
			Command = command;
			Description = description;
			this.action = action;
			IsCheat = isCheat;
			OnlyServer = onlyServer;
			IsSecret = isSecret;
			IsNetwork = isNetwork;
			AllowInDevBuild = allowInDevBuild;
			m_tabOptionsFetcher = optionsFetcher;
		}

		public List<string> GetTabOptions()
		{
			if (m_tabOptions == null && m_tabOptionsFetcher != null)
			{
				m_tabOptions = m_tabOptionsFetcher();
			}
			return m_tabOptions;
		}

		public void RunAction(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				List<string> tabOptions = GetTabOptions();
				if (tabOptions != null)
				{
					foreach (string item in tabOptions)
					{
						if (args[1].ToLower() == item.ToLower())
						{
							args.Args[1] = item;
							break;
						}
					}
				}
			}
			action(args);
		}

		public bool IsValid(Terminal context, bool skipAllowedCheck = false)
		{
			if ((!IsCheat || context.IsCheatsEnabled()) && (context.isAllowedCommand(this) || skipAllowedCheck) && (!IsNetwork || (bool)ZNet.instance))
			{
				if (OnlyServer)
				{
					if ((bool)ZNet.instance && ZNet.instance.IsServer())
					{
						return Player.m_localPlayer;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public delegate void ConsoleEvent(ConsoleEventArgs args);

	public delegate List<string> ConsoleOptionsFetcher();

	private static bool m_terminalInitialized;

	protected static SyncedList m_bindList;

	protected static Dictionary<KeyCode, List<string>> m_binds = new Dictionary<KeyCode, List<string>>();

	private static bool m_cheat = false;

	protected static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();

	protected char m_tabPrefix;

	protected bool m_autoCompleteSecrets;

	private List<string> m_history = new List<string>();

	private List<string> m_tabOptions = new List<string>();

	private int m_historyPosition;

	private int m_tabCaretPosition = -1;

	private int m_tabCaretPositionEnd;

	private int m_tabLength;

	private int m_tabIndex;

	private List<string> m_commandList = new List<string>();

	protected bool m_focused;

	public RectTransform m_chatWindow;

	public Text m_output;

	public InputField m_input;

	protected List<string> m_chatBuffer = new List<string>();

	protected const int m_maxBufferLength = 300;

	protected int m_maxVisibleBufferLength = 60;

	private const int m_maxScrollHeight = 5;

	private int m_scrollHeight;

	protected abstract Terminal m_terminalInstance { get; }

	private static void InitTerminal()
	{
		if (m_terminalInitialized)
		{
			return;
		}
		m_terminalInitialized = true;
		new ConsoleCommand("help", "Shows a list of console commands (optional: help 2 4 shows the second quarter)", delegate(ConsoleEventArgs args)
		{
			if ((bool)ZNet.instance && ZNet.instance.IsServer())
			{
				_ = (bool)Player.m_localPlayer;
			}
			else
				_ = 0;
			args.Context.IsCheatsEnabled();
			List<string> list10 = new List<string>();
			foreach (KeyValuePair<string, ConsoleCommand> command in commands)
			{
				if (!command.Value.IsSecret && command.Value.IsValid(args.Context))
				{
					list10.Add(command.Value.Command + " - " + command.Value.Description);
				}
			}
			list10.Sort();
			if (args.Context != null)
			{
				int num10 = 5;
				if (args.Length >= 3 && int.TryParse(args[2], out var result12) && result12 > 1)
				{
					num10 = result12;
				}
				if (args.Length >= 2 && int.TryParse(args[1], out var result13))
				{
					int num11 = list10.Count / num10;
					for (int l = num11 * (result13 - 1); l < Mathf.Min(list10.Count, num11 * (result13 - 1) + num11); l++)
					{
						args.Context.AddString(list10[l]);
					}
				}
				else
				{
					foreach (string item in list10)
					{
						args.Context.AddString(item);
					}
				}
			}
		});
		new ConsoleCommand("devcommands", "enables cheats", delegate(ConsoleEventArgs args)
		{
			m_cheat = !m_cheat;
			args.Context?.AddString("Dev commands: " + m_cheat);
			args.Context?.AddString("WARNING: using any dev commands is not recommended and is done at your own risk.");
			Gogan.LogEvent("Cheat", "CheatsEnabled", m_cheat.ToString(), 0L);
			args.Context.updateCommandList();
		}, isCheat: false, isNetwork: false, onlyServer: false, isSecret: true);
		new ConsoleCommand("hidebetatext", "", delegate
		{
			if ((bool)Hud.instance)
			{
				Hud.instance.ToggleBetaTextVisible();
			}
		}, isCheat: false, isNetwork: false, onlyServer: false, isSecret: true);
		new ConsoleCommand("ping", "ping server", delegate
		{
			if ((bool)Game.instance)
			{
				Game.instance.Ping();
			}
		});
		new ConsoleCommand("dpsdebug", "toggle dps debug print", delegate(ConsoleEventArgs args)
		{
			Character.SetDPSDebug(!Character.IsDPSDebugEnabled());
			args.Context?.AddString("DPS debug " + Character.IsDPSDebugEnabled());
		}, isCheat: true);
		new ConsoleCommand("lodbias", "set distance lod bias", delegate(ConsoleEventArgs args)
		{
			float result11;
			if (args.Length == 1)
			{
				args.Context.AddString("Lod bias:" + QualitySettings.lodBias);
			}
			else if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result11))
			{
				args.Context.AddString("Setting lod bias:" + result11);
				QualitySettings.lodBias = result11;
			}
		});
		new ConsoleCommand("info", "print system info", delegate(ConsoleEventArgs args)
		{
			args.Context.AddString("Render threading mode:" + SystemInfo.renderingThreadingMode);
			long totalMemory3 = GC.GetTotalMemory(forceFullCollection: false);
			args.Context.AddString("Total allocated mem: " + (totalMemory3 / 1048576).ToString("0") + "mb");
		});
		new ConsoleCommand("gc", "shows garbage collector information", delegate(ConsoleEventArgs args)
		{
			long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
			GC.Collect();
			long totalMemory2 = GC.GetTotalMemory(forceFullCollection: true);
			long num9 = totalMemory2 - totalMemory;
			args.Context.AddString("GC collect, Delta: " + (num9 / 1048576).ToString("0") + "mb   Total left:" + (totalMemory2 / 1048576).ToString("0") + "mb");
		}, isCheat: true);
		new ConsoleCommand("fov", "changes camera field of view", delegate(ConsoleEventArgs args)
		{
			Camera mainCamera = Utils.GetMainCamera();
			if ((bool)mainCamera)
			{
				float result10;
				if (args.Length == 1)
				{
					args.Context.AddString("Fov:" + mainCamera.fieldOfView);
				}
				else if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result10) && result10 > 5f)
				{
					args.Context.AddString("Setting fov to " + result10);
					Camera[] componentsInChildren = mainCamera.GetComponentsInChildren<Camera>();
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						componentsInChildren[k].fieldOfView = result10;
					}
				}
			}
		});
		new ConsoleCommand("kick", "[name/ip/userID] - kick user", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				string user3 = args[1];
				ZNet.instance.Kick(user3);
			}
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("ban", "[name/ip/userID] - ban user", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				string user2 = args[1];
				ZNet.instance.Ban(user2);
			}
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("unban", "[ip/userID] - unban user", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				string user = args[1];
				ZNet.instance.Unban(user);
			}
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("banned", "list banned users", delegate
		{
			ZNet.instance.PrintBanned();
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("save", "force saving of world", delegate
		{
			ZNet.instance.ConsoleSave();
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("optterrain", "optimize old terrain modifications", delegate
		{
			TerrainComp.UpgradeTerrain();
		}, isCheat: false, isNetwork: true);
		new ConsoleCommand("genloc", "regenerate all locations.", delegate
		{
			ZoneSystem.instance.GenerateLocations();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("players", "[nr] - force diffuculty scale ( 0 = reset)", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2 && int.TryParse(args[1], out var result9))
			{
				Game.instance.SetForcePlayerDifficulty(result9);
				args.Context.AddString("Setting players to " + result9);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("setkey", "[name]", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				ZoneSystem.instance.SetGlobalKey(args[1]);
				args.Context.AddString("Setting global key " + args[1]);
			}
			else
			{
				args.Context.AddString("Syntax: setkey [key]");
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("resetkeys", "[name]", delegate(ConsoleEventArgs args)
		{
			ZoneSystem.instance.ResetGlobalKeys();
			args.Context.AddString("Global keys cleared");
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("listkeys", "", delegate(ConsoleEventArgs args)
		{
			List<string> globalKeys = ZoneSystem.instance.GetGlobalKeys();
			args.Context.AddString("Keys " + globalKeys.Count);
			foreach (string item2 in globalKeys)
			{
				args.Context.AddString(item2);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("debugmode", "fly mode", delegate(ConsoleEventArgs args)
		{
			Player.m_debugMode = !Player.m_debugMode;
			args.Context.AddString("Debugmode " + Player.m_debugMode);
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("fly", "fly mode", delegate
		{
			Player.m_localPlayer.ToggleDebugFly();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("nocost", "no build cost", delegate
		{
			Player.m_localPlayer.ToggleNoPlacementCost();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("raiseskill", "[skill] [amount]", delegate(ConsoleEventArgs args)
		{
			if (args.Length > 2)
			{
				string text5 = args[1];
				int num8 = int.Parse(args[2]);
				Player.m_localPlayer.GetSkills().CheatRaiseSkill(text5, num8);
			}
			else
			{
				args.Context.AddString("Syntax: raiseskill [skill] [amount]");
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<string> list9 = Enum.GetNames(typeof(Skills.SkillType)).ToList();
			list9.Remove(Skills.SkillType.All.ToString());
			list9.Remove(Skills.SkillType.None.ToString());
			list9.Remove(Skills.SkillType.FireMagic.ToString());
			list9.Remove(Skills.SkillType.FrostMagic.ToString());
			return list9;
		});
		new ConsoleCommand("resetskill", "[skill]", delegate(ConsoleEventArgs args)
		{
			if (args.Length > 1)
			{
				string text4 = args[1];
				Player.m_localPlayer.GetSkills().CheatResetSkill(text4);
			}
			else
			{
				args.Context.AddString("Syntax: resetskill [skill]");
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<string> list8 = Enum.GetNames(typeof(Skills.SkillType)).ToList();
			list8.Remove(Skills.SkillType.All.ToString());
			list8.Remove(Skills.SkillType.None.ToString());
			list8.Remove(Skills.SkillType.FireMagic.ToString());
			list8.Remove(Skills.SkillType.FrostMagic.ToString());
			return list8;
		});
		new ConsoleCommand("sleep", "skips to next morning", delegate
		{
			EnvMan.instance.SkipToMorning();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("skiptime", "[gameseconds] skips head in seconds", delegate(ConsoleEventArgs args)
		{
			double timeSeconds2 = ZNet.instance.GetTimeSeconds();
			float num7 = 240f;
			if (args.Length > 1)
			{
				num7 = float.Parse(args[1]);
			}
			timeSeconds2 += (double)num7;
			ZNet.instance.SetNetTime(timeSeconds2);
			args.Context.AddString("Skipping " + num7.ToString("0") + "s , Day:" + EnvMan.instance.GetDay(timeSeconds2));
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("time", "shows current time", delegate(ConsoleEventArgs args)
		{
			double timeSeconds = ZNet.instance.GetTimeSeconds();
			bool flag2 = EnvMan.instance.CanSleep();
			args.Context.AddString(string.Format("{0} sec, Day: {1} ({2}), {3}", timeSeconds.ToString("0.00"), EnvMan.instance.GetDay(timeSeconds), EnvMan.instance.GetDayFraction().ToString("0.00"), flag2 ? "Can sleep" : "Can NOT sleep"));
		}, isCheat: true);
		new ConsoleCommand("resetcharacter", "reset character data", delegate(ConsoleEventArgs args)
		{
			args.Context?.AddString("Reseting character");
			Player.m_localPlayer.ResetCharacter();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("tutorialreset", "reset tutorial data", delegate(ConsoleEventArgs args)
		{
			args.Context?.AddString("Reseting tutorials");
			Player.ResetSeenTutorials();
			Raven.m_tutorialsEnabled = true;
		});
		new ConsoleCommand("tutorialtoggle", "toggles hugin hints", delegate(ConsoleEventArgs args)
		{
			if (Raven.IsInstantiated() && Tutorial.instance != null)
			{
				args.Context?.AddString("tutorials " + (Raven.m_tutorialsEnabled ? "disabled" : "enabled"));
				Raven.m_tutorialsEnabled = !Raven.m_tutorialsEnabled;
			}
		});
		new ConsoleCommand("randomevent", "start a random event", delegate
		{
			RandEventSystem.instance.StartRandomEvent();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("event", "[name] - start event", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				string text3 = args[1];
				if (!RandEventSystem.instance.HaveEvent(text3))
				{
					args.Context.AddString("Random event not found:" + text3);
				}
				else
				{
					RandEventSystem.instance.SetRandomEventByName(text3, Player.m_localPlayer.transform.position);
				}
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<string> list7 = new List<string>();
			foreach (RandomEvent @event in RandEventSystem.instance.m_events)
			{
				list7.Add(@event.m_name);
			}
			return list7;
		});
		new ConsoleCommand("stopevent", "stop current event", delegate
		{
			RandEventSystem.instance.ResetRandomEvent();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("removedrops", "remove all item-drops in area", delegate
		{
			int num6 = 0;
			ItemDrop[] array = UnityEngine.Object.FindObjectsOfType<ItemDrop>();
			for (int j = 0; j < array.Length; j++)
			{
				ZNetView component3 = array[j].GetComponent<ZNetView>();
				if ((bool)component3 && component3.IsValid() && component3.IsOwner())
				{
					component3.Destroy();
					num6++;
				}
			}
			Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Removed item drops: " + num6);
		}, isCheat: true);
		new ConsoleCommand("freefly", "freefly photo mode", delegate(ConsoleEventArgs args)
		{
			args.Context.AddString("Toggling free fly camera");
			GameCamera.instance.ToggleFreeFly();
		}, isCheat: true);
		new ConsoleCommand("ffsmooth", "freefly smoothness", delegate(ConsoleEventArgs args)
		{
			float result8;
			if (args.Length <= 1)
			{
				args.Context.AddString(GameCamera.instance.GetFreeFlySmoothness().ToString());
			}
			else if (!float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result8))
			{
				args.Context.AddString("syntax error");
			}
			else
			{
				args.Context.AddString("Setting free fly camera smoothing:" + result8);
				GameCamera.instance.SetFreeFlySmoothness(result8);
			}
		}, isCheat: true);
		new ConsoleCommand("location", "spawn location (CAUTION: saving permanently disabled)", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				string text2 = args[1];
				Vector3 pos2 = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 10f;
				ZoneSystem.instance.TestSpawnLocation(text2, pos2);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<string> list6 = new List<string>();
			foreach (ZoneSystem.ZoneLocation location in ZoneSystem.instance.m_locations)
			{
				list6.Add(location.m_prefabName);
			}
			return list6;
		});
		new ConsoleCommand("spawn", "[amount] [level] - spawn something", delegate(ConsoleEventArgs args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				int num4 = ((args.Length < 3) ? 1 : int.Parse(args[2]));
				int num5 = ((args.Length < 4) ? 1 : int.Parse(args[3]));
				GameObject prefab = ZNetScene.instance.GetPrefab(text);
				if (!prefab)
				{
					Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Missing object " + text);
				}
				else
				{
					DateTime now = DateTime.Now;
					if (num4 == 1)
					{
						Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + text);
						Character component = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
						if ((bool)component && num5 > 1)
						{
							component.SetLevel(num5);
						}
					}
					else
					{
						for (int i = 0; i < num4; i++)
						{
							Vector3 vector = UnityEngine.Random.insideUnitSphere * 0.5f;
							Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + text);
							Character component2 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up + vector, Quaternion.identity).GetComponent<Character>();
							if ((bool)component2 && num5 > 1)
							{
								component2.SetLevel(num5);
							}
						}
					}
					ZLog.Log("Spawn time :" + (DateTime.Now - now).TotalMilliseconds + " ms");
					Gogan.LogEvent("Cheat", "Spawn", text, num4);
				}
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, () => ZNetScene.instance.GetPrefabNames());
		new ConsoleCommand("pos", "print current player position", delegate(ConsoleEventArgs args)
		{
			Player localPlayer2 = Player.m_localPlayer;
			if ((bool)localPlayer2)
			{
				args.Context?.AddString("Player position (X,Y,Z):" + localPlayer2.transform.position.ToString("F0"));
			}
		}, isCheat: true);
		new ConsoleCommand("goto", "[x,z]- teleport", delegate(ConsoleEventArgs args)
		{
			if (args.Length < 3)
			{
				args.Context?.AddString("Syntax /goto x y");
			}
			else
			{
				if (float.TryParse(args[1], out var result6) && float.TryParse(args[2], out var result7))
				{
					Player localPlayer = Player.m_localPlayer;
					if ((bool)localPlayer)
					{
						Vector3 pos = new Vector3(result6, localPlayer.transform.position.y, result7);
						localPlayer.TeleportTo(pos, localPlayer.transform.rotation, distantTeleport: true);
					}
				}
				else
				{
					ZLog.Log("parse error: x and y must be numeric. Syntax: /goto x y");
				}
				Gogan.LogEvent("Cheat", "Goto", "", 0L);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("exploremap", "explore entire map", delegate
		{
			Minimap.instance.ExploreAll();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("resetmap", "reset map exploration", delegate
		{
			Minimap.instance.Reset();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("puke", "empties your stomach of food", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.ClearFood();
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("tame", "tame all nearby tameable creatures", delegate
		{
			Tameable.TameAllInArea(Player.m_localPlayer.transform.position, 20f);
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("killall", "kill nearby enemies", delegate
		{
			List<Character> allCharacters = Character.GetAllCharacters();
			int num3 = 0;
			foreach (Character item3 in allCharacters)
			{
				if (!item3.IsPlayer())
				{
					item3.Damage(new HitData
					{
						m_damage = 
						{
							m_damage = 1E+10f
						}
					});
					num3++;
				}
			}
			Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Killing all the monsters:" + num3);
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("heal", "heal to full health & stamina", delegate
		{
			Player.m_localPlayer.Heal(Player.m_localPlayer.GetMaxHealth());
			Player.m_localPlayer.AddStamina(Player.m_localPlayer.GetMaxStamina());
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("god", "invincible mode", delegate(ConsoleEventArgs args)
		{
			Player.m_localPlayer.SetGodMode(!Player.m_localPlayer.InGodMode());
			args.Context.AddString("God mode:" + Player.m_localPlayer.InGodMode());
			Gogan.LogEvent("Cheat", "God", Player.m_localPlayer.InGodMode().ToString(), 0L);
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("ghost", "", delegate(ConsoleEventArgs args)
		{
			Player.m_localPlayer.SetGhostMode(!Player.m_localPlayer.InGhostMode());
			args.Context.AddString("Ghost mode:" + Player.m_localPlayer.InGhostMode());
			Gogan.LogEvent("Cheat", "Ghost", Player.m_localPlayer.InGhostMode().ToString(), 0L);
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("beard", "change beard", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2 && (bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.SetBeard(args[1]);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("hair", "change hair", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2 && (bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.SetHair(args[1]);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("model", "change player model", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2 && (bool)Player.m_localPlayer && int.TryParse(args[1], out var result5))
			{
				Player.m_localPlayer.SetPlayerModel(result5);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("tod", "-1 OR [0-1]", delegate(ConsoleEventArgs args)
		{
			if (!(EnvMan.instance == null) && args.Length >= 2 && float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var result4))
			{
				args.Context.AddString("Setting time of day:" + result4);
				if (result4 < 0f)
				{
					EnvMan.instance.m_debugTimeOfDay = false;
				}
				else
				{
					EnvMan.instance.m_debugTimeOfDay = true;
					EnvMan.instance.m_debugTime = Mathf.Clamp01(result4);
				}
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: true);
		new ConsoleCommand("env", "[env] override environment", delegate(ConsoleEventArgs args)
		{
			if (!(EnvMan.instance == null) && args.Length >= 2)
			{
				args.Context.AddString("Setting debug enviornment:" + args[1]);
				EnvMan.instance.m_debugEnv = args[1];
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: true, delegate
		{
			List<string> list5 = new List<string>();
			foreach (EnvSetup environment in EnvMan.instance.m_environments)
			{
				list5.Add(environment.m_name);
			}
			return list5;
		});
		new ConsoleCommand("resetenv", "disables environment override", delegate(ConsoleEventArgs args)
		{
			if (!(EnvMan.instance == null))
			{
				args.Context.AddString("Reseting debug enviornment");
				EnvMan.instance.m_debugEnv = "";
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: true);
		new ConsoleCommand("wind", "[angle] [intensity]", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 3 && float.TryParse(args[1], out var result2) && float.TryParse(args[2], out var result3))
			{
				EnvMan.instance.SetDebugWind(result2, result3);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("resetwind", "", delegate
		{
			EnvMan.instance.ResetDebugWind();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("clear", "clear the console window", delegate(ConsoleEventArgs args)
		{
			args.Context.m_chatBuffer.Clear();
			args.Context.UpdateChat();
		});
		new ConsoleCommand("clearstatus", "clear any status modifiers", delegate
		{
			Player.m_localPlayer.ClearHardDeath();
			Player.m_localPlayer.GetSEMan().RemoveAllStatusEffects();
		}, isCheat: true, isNetwork: false, onlyServer: true);
		new ConsoleCommand("addstatus", "[name] adds a status effect (ex: Rested, Burning, SoftDeath, Wet, etc)", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				Player.m_localPlayer.GetSEMan().AddStatusEffect(args[1], resetTime: true);
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<StatusEffect> statusEffects2 = ObjectDB.instance.m_StatusEffects;
			List<string> list4 = new List<string>();
			foreach (StatusEffect item4 in statusEffects2)
			{
				list4.Add(item4.name);
			}
			return list4;
		});
		new ConsoleCommand("setpower", "[name] sets your current guardian power and resets cooldown (ex: GP_Eikthyr, GP_TheElder, etc)", delegate(ConsoleEventArgs args)
		{
			if (args.Length >= 2)
			{
				Player.m_localPlayer.SetGuardianPower(args[1]);
				Player.m_localPlayer.m_guardianPowerCooldown = 0f;
			}
		}, isCheat: true, isNetwork: false, onlyServer: true, isSecret: false, allowInDevBuild: false, delegate
		{
			List<StatusEffect> statusEffects = ObjectDB.instance.m_StatusEffects;
			List<string> list3 = new List<string>();
			foreach (StatusEffect item5 in statusEffects)
			{
				list3.Add(item5.name);
			}
			return list3;
		});
		new ConsoleCommand("bind", "[keycode] [command and parameters] bind a key to a console command. note: may cause conflicts with game controls", delegate(ConsoleEventArgs args)
		{
			if (!Enum.TryParse<KeyCode>(args[1], ignoreCase: true, out var _))
			{
				args.Context.AddString("'" + args[1] + "' is not a valid UnityEngine.KeyCode.");
			}
			else
			{
				string s = string.Join(" ", args.Args, 1, args.Length - 1);
				m_bindList.Add(s);
				updateBinds();
			}
		});
		new ConsoleCommand("unbind", "[keycode] clears all binds connected to keycode", delegate(ConsoleEventArgs args)
		{
			List<string> list2 = m_bindList.GetList();
			for (int num2 = list2.Count - 1; num2 >= 0; num2--)
			{
				if (list2[num2].Split(' ')[0].ToLower() == args[1])
				{
					list2.RemoveAt(num2);
				}
			}
			updateBinds();
		});
		new ConsoleCommand("printbinds", "prints current binds", delegate(ConsoleEventArgs args)
		{
			foreach (string item6 in m_bindList.GetList())
			{
				args.Context.AddString(item6);
			}
		});
		new ConsoleCommand("resetbinds", "resets all custom binds to default dev commands", delegate
		{
			List<string> list = m_bindList.GetList();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				m_bindList.Remove(list[num]);
			}
			updateBinds();
		});
		new ConsoleCommand("nomap", "disables map for this character", delegate(ConsoleEventArgs args)
		{
			if (Player.m_localPlayer != null)
			{
				string key = "mapenabled_" + Player.m_localPlayer.GetPlayerName();
				bool flag = PlayerPrefs.GetFloat(key, 1f) == 1f;
				PlayerPrefs.SetFloat(key, (!flag) ? 1 : 0);
				Minimap.instance.SetMapMode(Minimap.MapMode.None);
				args.Context?.AddString("Map " + (flag ? "disabled" : "enabled"));
			}
		}, isCheat: false, isNetwork: false, onlyServer: false, isSecret: true);
		new ConsoleCommand("resetspawn", "resets spawn location", delegate(ConsoleEventArgs args)
		{
			Game.instance.GetPlayerProfile()?.ClearCustomSpawnPoint();
			args.Context?.AddString("Reseting spawn point");
		});
		new ConsoleCommand("respawn", "quick trip to valhalla", delegate
		{
			HitData hit = new HitData
			{
				m_damage = 
				{
					m_damage = 99999f
				}
			};
			Player.m_localPlayer.Damage(hit);
		});
		new ConsoleCommand("say", "chat message", delegate(ConsoleEventArgs args)
		{
			if (args.FullLine.Length >= 5 && !(Chat.instance == null))
			{
				Chat.instance.SendText(Talker.Type.Normal, args.FullLine.Substring(4));
			}
		});
		new ConsoleCommand("s", "shout message", delegate(ConsoleEventArgs args)
		{
			if (args.FullLine.Length >= 3 && !(Chat.instance == null))
			{
				Chat.instance.SendText(Talker.Type.Shout, args.FullLine.Substring(2));
			}
		});
		new ConsoleCommand("w", "[playername] whispers a private message to a player", delegate(ConsoleEventArgs args)
		{
			if (args.FullLine.Length >= 3 && !(Chat.instance == null))
			{
				Chat.instance.SendText(Talker.Type.Whisper, args.FullLine.Substring(2));
			}
		});
		new ConsoleCommand("wave", "emote: wave", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("wave");
			}
		});
		new ConsoleCommand("sit", "sit down", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("sit", oneshot: false);
			}
		});
		new ConsoleCommand("challenge", "emote: challenge", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("challenge");
			}
		});
		new ConsoleCommand("cheer", "emote: cheer", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("cheer");
			}
		});
		new ConsoleCommand("nonono", "emote: nonono", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("nonono");
			}
		});
		new ConsoleCommand("thumbsup", "emote: thumbsup", delegate
		{
			if ((bool)Player.m_localPlayer)
			{
				Player.m_localPlayer.StartEmote("thumbsup");
			}
		});
		new ConsoleCommand("point", "emote: point", delegate
		{
			if ((bool)Player.m_localPlayer && Player.m_localPlayer.StartEmote("point"))
			{
				Player.m_localPlayer.FaceLookDirection();
			}
		});
	}

	protected static void updateBinds()
	{
		m_binds.Clear();
		List<string> value = m_bindList.GetList();
		foreach (string item2 in value)
		{
			string[] array = item2.Split(' ');
			string item = string.Join(" ", array, 1, array.Length - 1);
			if (Enum.TryParse<KeyCode>(array[0], ignoreCase: true, out var result))
			{
				if (m_binds.TryGetValue(result, out value))
				{
					value.Add(item);
					continue;
				}
				m_binds[result] = new List<string> { item };
			}
		}
	}

	private void updateCommandList()
	{
		m_commandList.Clear();
		foreach (KeyValuePair<string, ConsoleCommand> command in commands)
		{
			if (command.Value.IsValid(this) && (m_autoCompleteSecrets || !command.Value.IsSecret))
			{
				m_commandList.Add(command.Key);
			}
		}
	}

	public bool IsCheatsEnabled()
	{
		if (m_cheat)
		{
			if ((bool)ZNet.instance)
			{
				return ZNet.instance.IsServer();
			}
			return false;
		}
		return false;
	}

	public void TryRunCommand(string text, bool silentFail = false, bool skipAllowedCheck = false)
	{
		string[] array = text.Split(' ');
		if (commands.TryGetValue(array[0].ToLower(), out var value))
		{
			if (value.IsValid(this, skipAllowedCheck))
			{
				value.RunAction(new ConsoleEventArgs(text, this));
			}
			else if (!silentFail)
			{
				AddString("'" + text.Split(' ')[0] + "' is not valid in the current context.");
			}
		}
		else if (!silentFail)
		{
			AddString("'" + array[0] + "' is not a recognized command. Type 'help' to see a list of valid commands.");
		}
	}

	public virtual void Awake()
	{
		InitTerminal();
	}

	public virtual void Update()
	{
		if (m_focused)
		{
			UpdateInput();
		}
	}

	private void UpdateInput()
	{
		if (ZInput.GetButtonDown("ChatUp"))
		{
			if (m_historyPosition > 0)
			{
				m_historyPosition--;
			}
			m_input.text = ((m_history.Count > 0) ? m_history[m_historyPosition] : "");
			m_input.caretPosition = m_input.text.Length;
		}
		if (ZInput.GetButtonDown("ChatDown"))
		{
			if (m_historyPosition < m_history.Count)
			{
				m_historyPosition++;
			}
			m_input.text = ((m_historyPosition < m_history.Count) ? m_history[m_historyPosition] : "");
			m_input.caretPosition = m_input.text.Length;
		}
		if (ZInput.GetButtonDown("ScrollChatUp") && m_scrollHeight < m_chatBuffer.Count - 5)
		{
			m_scrollHeight++;
			UpdateChat();
		}
		if (ZInput.GetButtonDown("ScrollChatDown") && m_scrollHeight > 0)
		{
			m_scrollHeight--;
			UpdateChat();
		}
		if (m_input.caretPosition != m_tabCaretPositionEnd)
		{
			m_tabCaretPosition = -1;
		}
		if (Input.GetKeyDown(KeyCode.Tab) && ZNetScene.instance != null)
		{
			if (m_commandList.Count == 0)
			{
				updateCommandList();
			}
			string[] array = m_input.text.Split(' ');
			if (array.Length == 1)
			{
				tabCycle(array[0], m_commandList, usePrefix: true);
			}
			else
			{
				string key = ((m_tabPrefix == '\0') ? array[0] : array[0].Substring(1));
				if (commands.TryGetValue(key, out var value))
				{
					tabCycle(array[1], value.GetTabOptions(), usePrefix: false);
				}
			}
		}
		m_input.gameObject.SetActive(value: true);
		m_input.ActivateInputField();
		if (!Input.GetKeyDown(KeyCode.Return))
		{
			return;
		}
		if (!string.IsNullOrEmpty(m_input.text))
		{
			InputText();
			if (m_history.Count == 0 || m_history[m_history.Count - 1] != m_input.text)
			{
				m_history.Add(m_input.text);
			}
			m_historyPosition = m_history.Count;
			m_input.text = "";
			m_scrollHeight = 0;
			UpdateChat();
		}
		EventSystem.current.SetSelectedGameObject(null);
		m_input.gameObject.SetActive(value: false);
	}

	protected virtual void InputText()
	{
		string text = m_input.text;
		AddString(text);
		TryRunCommand(text);
	}

	protected virtual bool isAllowedCommand(ConsoleCommand cmd)
	{
		return true;
	}

	public void AddString(string text)
	{
		m_chatBuffer.Add(text);
		while (m_chatBuffer.Count > 300)
		{
			m_chatBuffer.RemoveAt(0);
		}
		UpdateChat();
	}

	private void UpdateChat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = Mathf.Min(m_chatBuffer.Count, Mathf.Max(5, m_chatBuffer.Count - m_scrollHeight));
		for (int i = Mathf.Max(0, num - m_maxVisibleBufferLength); i < num; i++)
		{
			stringBuilder.Append(m_chatBuffer[i]);
			stringBuilder.Append("\n");
		}
		m_output.text = stringBuilder.ToString();
	}

	private void tabCycle(string word, List<string> options, bool usePrefix)
	{
		if (options == null || options.Count == 0)
		{
			return;
		}
		usePrefix = usePrefix && m_tabPrefix != '\0';
		if (usePrefix)
		{
			if (word.Length < 1 || word[0] != m_tabPrefix)
			{
				return;
			}
			word = word.Substring(1);
		}
		if (m_tabCaretPosition == -1)
		{
			m_tabOptions.Clear();
			m_tabCaretPosition = m_input.caretPosition;
			word = word.ToLower();
			m_tabLength = word.Length;
			if (m_tabLength == 0)
			{
				m_tabOptions.AddRange(options);
			}
			else
			{
				foreach (string option in options)
				{
					if (option.Length > m_tabLength && safeSubstring(option, 0, m_tabLength).ToLower() == word)
					{
						m_tabOptions.Add(option);
					}
				}
			}
			m_tabOptions.Sort();
			m_tabIndex = -1;
		}
		if (m_tabOptions.Count != 0)
		{
			if (++m_tabIndex >= m_tabOptions.Count)
			{
				m_tabIndex = 0;
			}
			if (m_tabCaretPosition - m_tabLength >= 0)
			{
				m_input.text = safeSubstring(m_input.text, 0, m_tabCaretPosition - m_tabLength) + m_tabOptions[m_tabIndex];
			}
			int num = (m_tabCaretPositionEnd = (m_input.caretPosition = m_input.text.Length));
		}
	}

	private string safeSubstring(string text, int start, int length = -1)
	{
		if (text.Length == 0)
		{
			return text;
		}
		if (start < 0)
		{
			start = 0;
		}
		if (start + length >= text.Length)
		{
			length = text.Length - start;
		}
		if (length < 0)
		{
			return text;
		}
		if (length != -1)
		{
			return text.Substring(start, length);
		}
		return text.Substring(start);
	}
}
