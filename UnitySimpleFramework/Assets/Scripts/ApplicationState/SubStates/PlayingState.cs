using HudModule;
using System;
using System.Collections;
using UnityEngine;

namespace ApplicationState
{
    public class PlayingState : FiniteStateMachine<EApplicationState, EApplicationEvent, System.Object>.IState
    {
        public void OnEnter(EApplicationState? lastState, System.Object parameter)
        {
            //Debug.Log("  painting state Enter   ");

            GameObject LoadingPanelPanelObj = UIContext.Instance.ShowPanel(EUIPanel.LoadingPanel);

            LoadingPanel loadingPanel = LoadingPanelPanelObj.GetComponent<LoadingPanel>();

            GameRoot.Instance.StartCoroutine(InitPlayer(loadingPanel));

        }

        public IEnumerator InitPlayer(LoadingPanel loadingPanel)
        {

            yield return new WaitForSeconds(0.8f); //暂停loading界面 ， 测试代码

            //Debug.Log(" Loading completed ! ");

            GameObject playerPanelObj = UIContext.Instance.ShowPanel(EUIPanel.PlayingPanel);

            UIContext.Instance.ClosePanel(EUIPanel.LoadingPanel);
           
        }


        public void OnExit(EApplicationState? nextState)
        {
            UIContext.Instance.ClosePanel(EUIPanel.PlayingPanel);

            Resources.UnloadUnusedAssets();
            GC.Collect();

        }

        public void OnUpdate()
        {
          
        }
    }
}
