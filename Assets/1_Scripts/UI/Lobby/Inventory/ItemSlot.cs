using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemCountText;

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
    }
}