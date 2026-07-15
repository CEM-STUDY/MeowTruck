using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class PlayerController : NetworkBehaviour
    {
		[Header("Movement Params")]
		[SerializeField] private float moveSpeed;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (!IsOwner) return;

			if (Input.GetKey(KeyCode.W)) transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
			if (Input.GetKey(KeyCode.A)) transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
			if (Input.GetKey(KeyCode.S)) transform.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
			if (Input.GetKey(KeyCode.D)) transform.position += new Vector3(moveSpeed * Time.deltaTime, 0,0);
		}
	}
}