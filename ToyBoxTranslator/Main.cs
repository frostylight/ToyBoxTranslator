using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
#if DEBUG
using System;
#endif

namespace ToyBoxTranslator {
	public static class Main {
		public static bool Enabled = false;
		public static Harmony harmony = null;
		public static UnityModManager.ModEntry.ModLogger logger;
		public static int originalCount = 0;
		public static bool Load(UnityModManager.ModEntry modEntry) {
#if DEBUG
			try {
#endif
				modEntry.OnGUI = OnGUI;
				logger = modEntry.Logger;
				Translator.Load(modEntry.Path);
				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				Translator.Save();
				foreach(HashSet<string> st in Translator.original.Values) {
					originalCount += st.Count;
				}
#if DEBUG
			}
			catch(Exception e) {
				if(e is HarmonyException exception) {
					modEntry.Logger.Log($"{exception}");
				}
				else {
					modEntry.Logger.Log($"{e}");
				}
				return false;
			}
#endif
			return true;
		}
		public static void OnGUI(UnityModManager.ModEntry modEntry) {
			GUILayout.Label($"original : {originalCount} in total for {Translator.original.Count} categories");
			if(GUILayout.Button("Save Original", GUILayout.ExpandWidth(false))) {
				Translator.Save();
			}
		}
	}
}
