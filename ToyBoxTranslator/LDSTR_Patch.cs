using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ToyBoxTranslator {
	[HarmonyPatch]
	public static class LDSTR_Patch {
		public static IEnumerable<MethodBase> TargetMethods() {
			foreach(Type type in Assembly.GetAssembly(typeof(ToyBox.BagOfTricks)).GetTypes()) {
				if(type.Namespace == null || !type.Namespace.StartsWith("ToyBox")) {
					continue;
				}
				if(!type.CanPatched()) {
					continue;
				}
				foreach(MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) {
					if(!method.CanPatched()) {
						continue;
					}
					if(method.Name == "OnGUI") {
						yield return method;
					}
				}
				foreach(Type ntype in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) {
					if(ntype.IsVisible || !ntype.IsClass || !ntype.CanPatched()) {
						continue;
					}
					foreach(MethodInfo method in ntype.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) {
						if(!method.IsAssembly || !method.CanPatched()) {
							continue;
						}
						yield return method;
					}
				}
			}
		}
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod) {
			string name;
			if(__originalMethod.IsAssembly) {
				name = __originalMethod.DeclaringType.DeclaringType.FullName;
			}
			else {
				name = __originalMethod.DeclaringType.FullName;
			}
			foreach(CodeInstruction ins in instructions) {
				if(ins.opcode != OpCodes.Ldstr) {
					yield return ins;
					continue;
				}
				string text = ins.operand.ToString();
				if(!text.AnyCharacter()) {
					yield return ins;
					continue;
				}
				string result = "";
				foreach(string str in text.SplitKeep()) {
					if(str.AnyCharacter()) {
						Translator.Store(name, str);
					}
					result += Translator.Translate(name, str);
				}
				ins.operand = result;
				yield return ins;
			}
		}
	}
}
