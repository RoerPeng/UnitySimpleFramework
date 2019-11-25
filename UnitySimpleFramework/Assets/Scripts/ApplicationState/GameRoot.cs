using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ApplicationState
{

    /// <summary>
    /// 应用程序状态枚举
    /// </summary>
    public enum EApplicationState
    {
        Splash ,
        MainMenu ,
        Playing
    }

    /// <summary>
    /// 应用程序状态转换事件枚举
    /// </summary>
    public enum EApplicationEvent
    {
        EnterMain,
        EnterPlaying
    }


    /// <summary>
    /// 应用程序状态
    /// </summary>
    public class GameRoot : MonoSingleton<GameRoot>
    {

        private readonly FiniteStateMachine<EApplicationState , EApplicationEvent , System.Object> _stateMachine 
                            = new FiniteStateMachine<EApplicationState , EApplicationEvent , System.Object>();


        public FiniteStateMachine<EApplicationState, EApplicationEvent, System.Object> FSM
        { 
            get
            {
                return _stateMachine;
            }
        }

        private void Start()
        {
            InitFSM();
        }

        /// <summary>
        /// 初始化 有限状态机
        /// </summary>
        private void InitFSM()
        {
            //状态注册
            _stateMachine.Register(EApplicationState.Splash, new SplashState());
            _stateMachine.Register(EApplicationState.MainMenu, new MainMenuState());
            _stateMachine.Register(EApplicationState.Playing, new PlayingState());

            //设定状态起始点
            _stateMachine.SetEntryPoint(EApplicationState.Splash);

            //连接 状态转换 事件
            _stateMachine.State(EApplicationState.Splash)
                        .OnEvent(EApplicationEvent.EnterMain).Enter(EApplicationState.MainMenu);

            _stateMachine.State(EApplicationState.MainMenu)
                .OnFunc(EApplicationEvent.EnterPlaying, EnterPlaying);
            
            _stateMachine.State(EApplicationState.Playing)
                .OnEvent(EApplicationEvent.EnterMain).Enter(EApplicationState.MainMenu);
            
        }

        private bool EnterPlaying(System.Object ImageIndex)
        {
            _stateMachine.Enter(EApplicationState.Playing, ImageIndex);
            return true;
        }

        // Update is called once per frame
        void Update()
        {
            _stateMachine.Update();
        }

    }
}
