using Unity.Netcode;

namespace MeowTruck.Manager
{
	/// <summary>
	/// MonoBehaviour 오브젝트에서 전역적인 네트워크 통신 접근 필요시 사용합니다.
	/// 
	/// ex. 채팅, vfx, ...
	/// </summary>
	public class NetworkTransmission : NetworkBehaviour
	{
		private static NetworkTransmission instance;
		public static NetworkTransmission Instance => instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

	}
}