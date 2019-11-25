using UnityEngine;
using System.Collections.Generic;
using System;

namespace ApplicationState
{
    public sealed class SystemManagerContex
    {

        #region 单例

        private static volatile SystemManagerContex instance;
        private static object syncRoot = new System.Object();

        public static SystemManagerContex Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SystemManagerContex();
                    }
                }

                return instance;
            }
        }

        private SystemManagerContex() { }

        #endregion

        private readonly Dictionary<Type, MonoBehaviour> _systemManagerDic = new Dictionary<Type, MonoBehaviour>();

        public T AddSystemManager<T>() where T : MonoBehaviour
        {
            Type type = typeof(T);

            if (_systemManagerDic.ContainsKey(type))
            {
                return _systemManagerDic[type] as T;
            }

            T manager = GameRoot.Instance.gameObject.AddComponent<T>();
            _systemManagerDic.Add(type, manager);

            return manager;
        }

        public T GetSystemManager<T>() where T : MonoBehaviour
        {
            T result = null;
            Type type = typeof(T);
            if (_systemManagerDic.ContainsKey(type))
            {
                result = _systemManagerDic[type] as T;
            }
            return result;
        }

        public bool RemoveSystemManager<T>() where T : MonoBehaviour
        {
            return DestoryManager<T>(true);
        }

        public T LoadSystemManagerFromResource<T>() where T : MonoBehaviour
        {

            Type type = typeof(T);
            if (_systemManagerDic.ContainsKey(type))
            {
                return _systemManagerDic[type] as T;
            }

            T manager = null;

            do
            {

                string managerPathName = string.Format("Managers/{0}", type.Name);

                GameObject managerObj = GameObject.Instantiate(Resources.Load<GameObject>(managerPathName));

                if (managerObj == null)
                {
#if UNITY_EDITOR
                    Debug.LogError(" Load Resources Error !  " + type.Name);
#endif
                    break;
                }
                managerObj.transform.SetParent(GameRoot.Instance.transform, false);

                manager = managerObj.GetComponent<T>();

                if (manager == null)
                {
#if UNITY_EDITOR
                    Debug.LogError(" Not Found " + type.Name);
#endif
                    break;
                }

                _systemManagerDic.Add(type, manager);

            } while (false);


            return manager;
        }


        public bool DestoryManagerObj<T>() where T : MonoBehaviour
        {
            return DestoryManager<T>(false);
        }


        /// <summary>
        /// 销毁系统控制中心
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IsCompnent"> 是销毁单个组件， 还是销毁整个GameObject</param>
        private bool DestoryManager<T>(bool IsCompnent) where T : MonoBehaviour
        {
            bool result = false;
            Type type = typeof(T);
            if (!_systemManagerDic.ContainsKey(type))
            {
                //Error 未发现该系统
#if UNITY_EDITOR
                Debug.LogWarning(" 未发现该系统! Not Fount SystemManager :"
                                + type.Name);
#endif

                return result;
            }

            MonoBehaviour manager = _systemManagerDic[type];

            result = _systemManagerDic.Remove(type);

            if (IsCompnent)
            {
                UnityEngine.Object.Destroy(manager);
            }
            else
            {
                UnityEngine.Object.Destroy(manager.gameObject);
            }

            return result;
        }


    }

}