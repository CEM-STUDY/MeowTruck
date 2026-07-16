using Steamworks;
using System.IO;
using System.Text;
using UnityEngine;

namespace MeowTruck.Manager
{
	/// <summary>
	/// 
	/// - Steam Cloud 관련 API 제공
	/// 
	/// </summary>
	public class SteamCloud
	{
		private const string SAVE_FILE_NAME = "temp.json";

		public void Save<T>(T data)
		{
			string json = JsonUtility.ToJson(data, true);

			string localPath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
			File.WriteAllText(localPath, json);

			if (SteamClient.IsValid)
			{
				SteamRemoteStorage.FileWrite(SAVE_FILE_NAME, Encoding.UTF8.GetBytes(json));
			}
		}

		public T Load<T>()
		{
			string json = "";
			if (SteamClient.IsValid && SteamRemoteStorage.FileExists(SAVE_FILE_NAME))
			{
				json = Encoding.UTF8.GetString(SteamRemoteStorage.FileRead(SAVE_FILE_NAME));
			}
			else
			{
				string localPath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
				if (File.Exists(localPath)) json = File.ReadAllText(localPath);
			}

			return string.IsNullOrEmpty(json) ? default(T) : JsonUtility.FromJson<T>(json);
		}
	}
}
