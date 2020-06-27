using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

///<summary>
/// This class faciliates converting a Dictionary into a JSON string and converting
///a JSON string into a Dictionary.
///</summary>
///<remarks> You will need to make your own functions for each specific type of Dictionary you wish to convert.</remarks>
public static class DictionarySerializeUtil {

	///<summary>
	/// Converts a Dictionary(string, bool) into a JSON string.
	///</summary>
	///<param name="dict">A Dictionary(string,bool) to convert into a JSON string.</param>
	///<returns>Returns a string formatted in JSON.</returns>
	public static string RewardDictionaryToJson(Dictionary<string, bool> dict){
		string json = "{";
		foreach (var entry in dict){
			string formattedEntry = "\"" + entry.Key + "\":" + entry.Value + ",";
			json = string.Concat(json, formattedEntry);
		}
		json = json.Substring(0, json.Length - 1);
		json = string.Concat(json, "}");

		return json;
	}

	///<summary>
	/// Converts a JSON string into a Dictionary(string, bool).
	///</summary>
	///<param name="json">A string representing the JSON string to convert into a Dictionary.</param>
	///<returns>Returns a Dictionary(string, bool)</returns>
	public static Dictionary<string, bool> JsonToRewardDictionary(string json){
		Dictionary<string, bool> dict = new Dictionary<string, bool>();
		
		Regex rx = new Regex(@"\x22\w+\x22:\w+",RegexOptions.IgnoreCase);
		MatchCollection matches = rx.Matches(json);

		foreach (Match match in matches){
			string rewardInfo = match.Value.Replace("\"", "");
			string[] pair = rewardInfo.Split(':');
			bool value = false;
			if (pair[1].ToLower().Equals("true")){
				value = true;
			}
			dict.Add(pair[0], value);
		}
	
		return dict;
	}
}
