using UnityEngine;
using UnityEngine.UI;

namespace HudModule
{
    /// <summary>
    /// Loading界面
    /// </summary>
    public class LoadingPanel : MonoBehaviour
    {
        [SerializeField]
        private Slider _loadingBar;
        public Text m_Text;

        public void SetFillAmount(float amount)
        {
            _loadingBar.value = amount;

            if (m_Text != null)
            {
               m_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
            }
        }
    }
}