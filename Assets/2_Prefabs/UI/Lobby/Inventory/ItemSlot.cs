using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemCountText;

        

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