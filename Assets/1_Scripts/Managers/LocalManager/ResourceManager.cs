using MeowTruck.Data;
using System.Collections.Generic;
using UnityEngine;

namespace MeowTruck.Manager
{
	public class ResourceManager
	{
		private readonly string PATH_ITEMDATA = "ItemData/";

		private Dictionary<int, ItemData> itemDataDictionary = new();

		public void Init()
		{
			ItemData[] itemDatas = Resources.LoadAll<ItemData>(PATH_ITEMDATA);
		
			for(int i=0; i<itemDatas.Length; i++)
			{
				itemDataDictionary.Add(itemDatas[i].ItemID, itemDatas[i]);
			}
		}

		public ItemData GetItemData(int itemId)
		{
			return itemDataDictionary[itemId];	
		}
	}
}
