using System.Collections.Generic;
using System;

/// <summary>
/// 有限状态机  
/// </summary>
/// <typeparam name="T">自定义 【状态】 枚举</typeparam>
/// <typeparam name="E">自定义 【事件】 枚举</typeparam>
/// <typeparam name="P">自定义 【转换事件】 参数</typeparam>
public class FiniteStateMachine<T,E,P>  where T : struct , IConvertible 
                                      where E : struct , IConvertible
{
    /// <summary>
    /// 进入状态  委托类型 
    /// </summary>
    /// <param name="stateName"></param>
    public delegate void EnterStateHandle(T stateName , P parameter);

    /// <summary>
    /// 加入状态 委托类型
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="lastStateName"></param>
    public delegate void PushStateHandle(T stateName, T? lastStateName, P parameter);

    /// <summary>
    /// 弹出状态 委托类型
    /// </summary>
    public delegate void PopStateHandle();

#region 状态事件外壳
    /// <summary>
    /// 状态事件外壳
    /// </summary>
    public class FSEvent
    {
        protected EnterStateHandle mEnterDelegate;
        protected PushStateHandle mPushDelegate;
        protected PopStateHandle mPopDelegate;

        /// <summary>
        /// 自定义 触发事件
        /// </summary>
        public Func<P, bool> mAction = null;

        /// <summary>
        /// 单个事件类型
        /// </summary>
        protected enum EventType { NONE, ENTER, PUSH, POP };

        /// <summary>
        /// 事件状态， 
        /// </summary>
        protected EventType eType;

        //事件名
        protected E eventEnumName;

        protected FSState mStateOwner;
        protected T? mTargetState;
        protected FiniteStateMachine<T, E, P> mOwner;

        public FSEvent(E eventEnum, T? target, FSState state,
            FiniteStateMachine<T, E, P> owner,
            EnterStateHandle e,
            PushStateHandle pu,
            PopStateHandle po)
        {
            mStateOwner = state;
            eventEnumName = eventEnum;
            mTargetState = target;
            mOwner = owner;
            eType = EventType.NONE;
            mEnterDelegate = e;
            mPushDelegate = pu;
            mPopDelegate = po;
        }

        /// <summary>
        /// Enter状态 ，会推出前一状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public FSState Enter(T stateName)
        {
            mTargetState = stateName;
            eType = EventType.ENTER;
            return mStateOwner;
        }

        public FSState Push(T stateName)
        {
            mTargetState = stateName;
            eType = EventType.PUSH;
            return mStateOwner;
        }

        public void Pop()
        {
            eType = EventType.POP;
        }

        public void Execute(P o1) 
        {
            if (eType == EventType.POP)
                mPopDelegate();
            else if (eType == EventType.PUSH)
                mPushDelegate((T)mTargetState, mOwner.CurrentState.StateName , o1); 
            else if (eType == EventType.ENTER)
                mEnterDelegate((T)mTargetState , o1);
            else if (mAction != null)
                mAction(o1);
        }

    }

#endregion

#region 状态外壳
    /// <summary>
    /// 状态机 组件 , 状态实体的包装外壳
    /// </summary>
    public class FSState
    {
        protected EnterStateHandle mEnterDelegate;
        protected PushStateHandle mPushDelegate;
        protected PopStateHandle mPopDelegate;

        //实际使用的状态
        protected IState mStateObject;
        protected T mStateName;
        protected FiniteStateMachine<T, E , P> mOwner;

        //单个状态实体 事件池
        protected Dictionary<E, FSEvent> mFSEventsPool;

        public FSState(IState obj, FiniteStateMachine<T, E, P> owner, T name,
                         EnterStateHandle e,
                         PushStateHandle pu,
                         PopStateHandle po)
        {
            mStateObject = obj;
            mStateName = name;
            mOwner = owner;
            mEnterDelegate = e;
            mPushDelegate = pu;
            mPopDelegate = po;
            mFSEventsPool = new Dictionary<E, FSEvent>();
        }

        public IState StateObject
        {
            get
            {
                return mStateObject;
            }
        }

        public T StateName
        {
            get
            {
                return mStateName;
            }
        }

        public FSEvent OnEvent(E eventEnum)
        {

            FSEvent newEvent = new FSEvent(eventEnum, null, this, mOwner, mEnterDelegate, mPushDelegate, mPopDelegate);

            if (mFSEventsPool.ContainsKey(eventEnum))
            {
                UnityEngine.Debug.LogError(string.Format(
                    " This state [ {0} ] , have this Event [ {1} ]!  "
                    , StateName, eventEnum));
                return null;
            }

            mFSEventsPool.Add(eventEnum, newEvent);
            return newEvent;
        }

        public void Trigger(E eventEnum)
        {
            if(!mFSEventsPool.ContainsKey(eventEnum))
            {
                UnityEngine.Debug.LogError(string.Format(
                    " This state [ {0} ] , Don't have this Event [ {1} ]!  "
                    , StateName , eventEnum ) );
                return;           
            }

            mFSEventsPool[eventEnum].Execute(default(P));
        }

        public void Trigger(E eventEnum, P param1)
        {
            if (!mFSEventsPool.ContainsKey(eventEnum))
            {
                UnityEngine.Debug.LogError(string.Format(
                   " This state [ {0} ] , Don't have this Event [ {1} ]!  "
                    , StateName, eventEnum));
                return;
            }

            mFSEventsPool[eventEnum].Execute(param1);
        }
        //public void Trigger(E eventEnum, P param1)
        //{
        //    mFSEventsPool[eventEnum].Execute(param1, param2, null);
        //}
        //public void Trigger(E eventEnum, P param1, object param2, object param3)
        //{
        //    mFSEventsPool[eventEnum].Execute(param1, param2, param3);
        //}


        public FSState OnFunc(E eventEnum, Func<P, bool> action)
        {
            FSEvent newEvent = new FSEvent(eventEnum, null, this, mOwner, mEnterDelegate, mPushDelegate, mPopDelegate);
            newEvent.mAction = action;
            mFSEventsPool.Add(eventEnum, newEvent);
            return this;
        }

        /// <summary>
        /// 自定义事件注册
        /// </summary>
        /// <typeparam name="M"> </typeparam>
        /// <param name="eventEnum"> </param>
        /// <param name="action"></param>
        /// <returns></returns>
        //public FSState OnAction(E eventEnum, Action<P> action)
        //{
        //    //TODO 匿名委托 优化 
        //    FSEvent newEvent = new FSEvent(eventEnum, null, this, mOwner, mEnterDelegate, mPushDelegate, mPopDelegate);
        //    newEvent.mAction = delegate (P o1)
        //    {
        //        action(o1);
        //        return true;
        //    };

        //    mFSEventsPool.Add(eventEnum, newEvent);
        //    return this;
        //}
    }
#endregion

#region FSM IState 接口
    public interface IState
    {
        void OnEnter(T? lastState , P parameter);  //parameter
        void OnExit(T? nextState);
        void OnUpdate();      
    }
#endregion

    /// <summary>
    /// 状态池
    /// </summary>
    protected Dictionary<T, FSState> mStatePool;
        
    protected T mEntryPoint;

    /// <summary>
    /// 状态栈
    /// </summary>
    protected Stack<FSState> mStateStack;

    public FiniteStateMachine()
    {
        mStatePool = new Dictionary< T, FSState >();
        mStateStack = new Stack<FSState>();
        mEntryPoint = default(T);
    }

    /// <summary>
    /// 当前状态
    /// </summary>
    public FSState CurrentState
    {
        get
        {
            //从状态栈中 查看
            if (mStateStack.Count == 0)
                return null;
            return mStateStack.Peek();
        }
    }

    public void Update()
    {
        TryToEnterFirstState();

        if (CurrentState == null)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(" FSM Update ERROR ! The Current State is NULL ! " );
#endif
            return;
        }

        CurrentState.StateObject.OnUpdate();
    }

    /// <summary>
    /// 如果当前状态为空,尝试进入起始状态
    /// </summary>
    private void TryToEnterFirstState()
    {
        if (CurrentState != null)
        {
            //已经入过状态
            return;
        }

        if (!mStatePool.ContainsKey(mEntryPoint))
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(" StatePool No Contains Key - EntryPoint  : " + mEntryPoint);
#endif
            return;
        }

        mStateStack.Push(mStatePool[mEntryPoint]);
        CurrentState.StateObject.OnEnter(null, default(P));
    }

    /// <summary>
    /// 状态注册，将状态实体加壳后加入状态池
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="stateObject"></param>
    public void Register(T stateName, IState stateObject)
    {
        if (mStatePool.Count == 0)
            mEntryPoint = stateName;
        mStatePool.Add(stateName, new FSState(stateObject, this, stateName, Enter, Push, Pop));
    }

    /// <summary>
    /// 通过状态枚举，获取状态实体
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public FSState State(T stateName)
    {
        if (!mStatePool.ContainsKey(stateName))
        {
            return null;
        }
        return mStatePool[stateName];
    }

    /// <summary>
    /// 设置状态初始点
    /// </summary>
    /// <param name="startName"></param>
    public void SetEntryPoint(T startName)
    {
        mEntryPoint = startName;
    }

    public void Enter(T stateName, P parameter)
    {
        Push(stateName, ExcutePop( (T?)stateName ) , parameter);
    }
    
    public void Push(T newState)
    {
        T? lastName = null ;
        if (mStateStack.Count > 1)
        {
            lastName = mStateStack.Peek().StateName;
        }
        Push(newState, lastName ,default(P));
    }

    protected void Push(T stateName, T? lastStateName, P parameter)
    {
        mStateStack.Push(mStatePool[stateName]);
        mStateStack.Peek().StateObject.OnEnter(lastStateName , parameter);
    }

    public void Pop()
    {
        ExcutePop(null);
    }
    
    /// <summary>
    /// 执行出栈,出栈事件会传出准备入栈的状态名
    /// </summary>
    /// <param name="nextState">下一个状态，可以为空</param>
    /// <returns></returns>
    protected T? ExcutePop(T? nextState)
    {
        //Fixing
        FSState lastState = mStateStack.Peek();
        T? newState = null;
        if (nextState == null && mStateStack.Count > 1)
        {
            int index = 0;
            foreach (FSState item in mStateStack)
            {
                if (index++ == mStateStack.Count - 2)
                {
                    newState = item.StateName;
                }
            }
        }
        else
        {
            newState = nextState;
        }

        T? lastStateName = null;
        if (lastState != null)
        {
            lastStateName = lastState.StateName;
            lastState.StateObject.OnExit(newState);
        }
        mStateStack.Pop();
        return lastStateName;
    }

    /// <summary>
    /// 触发当前状态事件
    /// </summary>
    /// <param name="eventName"></param>
    public void Trigger(E eventEnum)
    {
        CurrentState.Trigger(eventEnum);
    }

    public void Trigger(E eventEnum, P param1)
    {
        CurrentState.Trigger(eventEnum, param1);
    }

    //public void Trigger(E eventEnum, P param1, object param2)
    //{
    //    CurrentState.Trigger(eventEnum, param1, param2);
    //}
}
