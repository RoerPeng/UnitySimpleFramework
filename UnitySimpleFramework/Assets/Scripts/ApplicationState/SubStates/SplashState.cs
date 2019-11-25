using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ApplicationState
{
    public class SplashState : FiniteStateMachine<EApplicationState, EApplicationEvent, System.Object>.IState
    {

        public void OnEnter(EApplicationState? lastState, System.Object parameter)
        {

            Application.targetFrameRate = 60;
            //初始化  UI系统
            GameObject uiroot = GameObject.Instantiate(Resources.Load<GameObject>("UIPanel/UIRoot"));
            uiroot.gameObject.SetActive(true);
            //初始化  各系统Manager
            InitSystemManager();
            //直接 进入主菜单 状态
            GameRoot.Instance.FSM.Trigger(EApplicationEvent.EnterMain);
           
        }

     
        private void InitSystemManager()
        {
            //TODO
        }
           

        public void OnExit(EApplicationState? nextState)
        {

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        
        public void OnUpdate()
        {
          
        }
    }
}
