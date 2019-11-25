using ApplicationState;
using UnityEngine;

namespace HudModule
{
    public class PlayingPanel : MonoBehaviour
    {
        public void OnClickReturnBtn()
        {
            GameRoot.Instance.FSM.Trigger(EApplicationEvent.EnterMain);
        }
    }
}
