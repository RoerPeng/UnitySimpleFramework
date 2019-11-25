using System;
using UnityEngine;

namespace ApplicationState
{
    public class MainMenuState : FiniteStateMachine<EApplicationState, EApplicationEvent, System.Object>.IState
    {
        
        public void OnEnter(EApplicationState? lastState, System.Object parameter)
        {
           
            //显示主界面
            UIContext.Instance.ShowPanel(EUIPanel.MainMenuPanel);
      
        }

        public void OnExit(EApplicationState? nextState)
        {          
            //关闭主界面
            UIContext.Instance.ClosePanel(EUIPanel.MainMenuPanel);

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        public void OnUpdate()
        {

        }

    }
}
