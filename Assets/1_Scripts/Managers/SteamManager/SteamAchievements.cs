using Steamworks;
using Steamworks.Data;

namespace MeowTruck 
{

	/// <summary>
	/// 
	/// - Steam 도전과제 관련 API 제공
	/// 
	/// </summary>
	public class SteamAchievement
	{
		public void UnlockAchievement(string achievementId)
		{
			if (!SteamClient.IsValid) return;

			var achievement = new Achievement(achievementId);
			if (!achievement.State)
			{
				achievement.Trigger();
				SteamUserStats.StoreStats();
			}
		}

		public void IndicateProgress(string statName, int count)
		{
			if (SteamClient.IsValid) return;
			SteamUserStats.SetStat(statName, count);
			SteamUserStats.StoreStats();
		}
	}
}