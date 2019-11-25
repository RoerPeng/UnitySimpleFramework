using UnityEngine;
using ApplicationState;

namespace HudModule
{
    /// <summary>
    /// 主菜单
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
      
        public void OnClickEnterPlay()
        {
            ApplicationState.GameRoot.Instance.FSM.Trigger(EApplicationEvent.EnterPlaying);
        }
        
    }
}
