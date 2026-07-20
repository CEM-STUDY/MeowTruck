using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Entity Data", menuName = "Data/Entity Data")]
	public class EntityData : ScriptableObject
	{
		[SerializeField] private GameObject entityPrefab;
		[SerializeField] private int entityID;
		[SerializeField] private string entityName;
		[SerializeField] private EntityType entityType;

		[SerializeField] private float maxHp;
		[SerializeField] private float attack;

		public GameObject EntityPrefab => entityPrefab;
		public int EntityID => entityID;
		public string EntityName => entityName;
		public EntityType EntityType => entityType;
		public float MaxHp => maxHp;
		public float Attack => attack;
	}
}
