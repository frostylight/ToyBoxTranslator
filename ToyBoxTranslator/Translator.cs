using System;
using System.Collections.Generic;
using System.IO;
using TinyJson;

namespace ToyBoxTranslator {
	public static class Translator {
		private static string ModDir;
		public static readonly Dictionary<string, HashSet<string>> original = new Dictionary<string, HashSet<string>>();
		public static Dictionary<string, Dictionary<string, string>> localization = null;

		public static string Translate(string name, string text) {
			if(!localization.ContainsKey(name)) {
				return text;
			}
			string result = localization[name].TryGet(text, text);
			if(result.Length == 0) {
				return text;
			}
			return result;
		}
		public static void Store(string name, string text) {
			if(original.ContainsKey(name)) {
				original[name].Add(text);
			}
			else {
				original.Add(name, new HashSet<string>() { text });
			}
		}
		public static void Load(string path) {
			ModDir = path;
			string LocalizationFile = Path.Combine(ModDir, "Localization.json");
			if(File.Exists(LocalizationFile)) {
				try {
					localization = File.ReadAllText(LocalizationFile).FromJson<Dictionary<string, Dictionary<string, string>>>();
				}
				catch(Exception e) {
					localization = null;
					Main.logger.Error($"Load localization failed!\n{e}");
				}
			}
		}
		public static void Save() {
			Dictionary<string, Dictionary<string, string>> oriDict = new Dictionary<string, Dictionary<string, string>>();
			foreach(string key in original.Keys) {
				oriDict.Add(key, new Dictionary<string, string>());
				foreach(string key1 in original[key]) {
					oriDict[key].Add(key1, "");
				}
			}
			string oriFile = Path.Combine(ModDir, "Original.json");
			File.WriteAllText(oriFile, oriDict.ToJson());
		}
	}

}
