using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MeowTruck.Manager
{
	public class Managers : MonoBehaviour
	{
		private static Managers instance = null;
		public static Managers Instance { get { return instance; } }

		private void Awake()
		{
			Init();
		}

		private void Init()
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

			Input.Init();
			Resource.Init();
			Inventory.Init(10);
		}

		public static SceneManagerEx Scene { get; private set; } = new SceneManagerEx();
		public static InputManager Input { get; private set; } = new InputManager();
		public static InventoryManager Inventory { get; private set; } = new InventoryManager();
		public static ResourceManager Resource { get; private set; } = new ResourceManager();

	}
}