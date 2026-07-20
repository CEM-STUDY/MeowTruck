using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class PlayerCameraController : MonoBehaviour
    {
		[SerializeField] private Vector3 offset;
		[SerializeField] private float followSpeed;
		private Transform targetTransform = null; 

		private void Update()
		{
			if (targetTransform == null)
			{
				FindPlayerObject();
			}
		}
		private void LateUpdate()
		{
			if (targetTransform == null)
				return;

			Vector3 targetPos = targetTransform.position + offset;
			transform.position = Vector3.Lerp(
				transform.position,
				targetPos,
				followSpeed * Time.deltaTime);
		}

		private void FindPlayerObject()
		{
			if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;

			targetTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
			if (targetTransform != null) transform.position = targetTransform.position + offset;
		}
	}
}