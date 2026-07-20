using MeowTruck.Data;
using System.Collections.Generic;
using UnityEngine;

namespace MeowTruck.Manager
{
	public class ResourceManager
	{
		private readonly string PATH_ITEMDATA = "Datas/ItemData/";
		private readonly string PATH_ENTITYDATA = "Datas/EntityData/";

		private Dictionary<int, ItemData> itemDataDictionary = new();
		private Dictionary<int, EntityData> entityDataDictionary = new();

		public void Init()
		{
			ItemData[] itemDatas = Resources.LoadAll<ItemData>(PATH_ITEMDATA);
			EntityData[] entityDatas = Resources.LoadAll<EntityData>(PATH_ENTITYDATA);

			for (int i = 0; i < itemDatas.Length; i++)
				itemDataDictionary.Add(itemDatas[i].ItemID, itemDatas[i]);
			for (int i = 0; i < entityDatas.Length; i++)
				entityDataDictionary.Add(entityDatas[i].EntityID, entityDatas[i]);
		}

		public ItemData GetItemData(int itemId)
		{
			if (!itemDataDictionary.TryGetValue(itemId, out var itemData)) return null;
			return itemData;
		}

		public EntityData GetEntityData(int entityId)
		{
			if (!entityDataDictionary.TryGetValue(entityId, out var entityData)) return null;
			return entityData;
		}
	}
}
