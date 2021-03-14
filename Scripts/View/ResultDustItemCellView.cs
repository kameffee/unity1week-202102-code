using Domain;
using TMPro;
using UnityEngine;

namespace View
{
    /// <summary>
    /// リザルトのスクロールビューの中
    /// </summary>
    public class ResultDustItemCellView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI itemNameText;

        [SerializeField]
        private TextMeshProUGUI countText;

        public void Prepare(IDustData data, int count)
        {
            itemNameText.text = data.DustName;
            countText.text = count.ToString();
        }
    }
}