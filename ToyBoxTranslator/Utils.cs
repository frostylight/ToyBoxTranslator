using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ToyBoxTranslator {
	public static class Utils {
		public static bool CanPatched(this Type type) {
			if(type == null) {
				return false;
			}
			if(!type.IsGenericType) {
				return true;
			}
			if(!type.IsGenericTypeDefinition || type.IsConstructedGenericType) {
				return true;
			}
			return false;
		}
		public static bool CanPatched(this MethodInfo method) {
			if(method == null) {
				return false;
			}
			if(method.IsAbstract) {
				return false;
			}
			if(method.IsGenericMethod) {
				return false;
			}
			if(method.DeclaringType.IsGenericType) {
				return !method.IsStatic && !method.IsGenericMethod;
			}
			return true;
		}
		public static TValue TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue) {
			if(dict == null || key == null) {
				return defaultValue;
			}
			if(dict.TryGetValue(key, out TValue val)) {
				return val;
			}
			else {
				return defaultValue;
			}
		}
		public static bool AnyCharacter(this string str) {
			if(str == null || str.Length == 0) {
				return false;
			}
			return Regex.IsMatch(str, @"[^\W\d]");
		}
		public static IEnumerable<string> SplitKeep(this string str) {
			foreach(string result in Regex.Split(str, @"([\n\r])")) {
				yield return result;
			}
		}
	}
}
