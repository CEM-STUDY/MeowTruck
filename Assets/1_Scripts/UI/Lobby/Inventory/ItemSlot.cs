using MeowTruck.Manager;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MeowTruck.UI
{
	public class ItemSlot : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IDropHandler
	{
		[SerializeField] private Image bgImage;
		[SerializeField] private Image itemImage;
		[SerializeField] private TextMeshProUGUI itemCountText;

		public int MySlotId { get; private set; } = -1;

		public void Init(int slotId)
		{
			MySlotId = slotId;
		}

		public void OnSelected()
		{
            bgImage.color = Color.red;
		}

        public void OnUnselected()
        {
            bgImage.color = Color.white;
        }

		public void SetSprite(Sprite sprite)
        {
            itemImage.sprite = sprite;
        }

        public void SetCountText(int count)
        {
            itemCountText.text = count.ToString(); 
        }

		public void OnBeginDrag(PointerEventData eventData)
		{
			// TODO - 아이콘 생성
		}

		public void OnDrag(PointerEventData eventData)
		{
			// TODO - 마우스 따라다님
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			// TODO - 아이콘 제거
			var target = eventData.pointerCurrentRaycast.gameObject;

			if (target == null) // 바닥에 버리기
			{
				Managers.Inventory.DropItem(
					NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position,
					MySlotId,
					-1
					);
				return;
			}

			ItemSlot slot = target.GetComponentInParent<ItemSlot>();
			if (slot == null) return;	// 다른 UI

			Managers.Inventory.ChangeSlot(MySlotId, slot.MySlotId);
		}

		public void OnDrop(PointerEventData eventData)
		{
			// int to = eventData.pointerDrag.GetComponent<ItemSlot>().MySlotId;
			// Managers.Inventory.ChangeSlot(MySlotId, to);
		}
	}
}