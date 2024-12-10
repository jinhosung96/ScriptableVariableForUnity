using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JHS.ScriptableVariable
{
    public enum ResetOn
    {
        SceneLoad,
        ApplicationStart,
        None
    }

    public abstract class ScriptableVariable<T> : ScriptableObject
    {
        [SerializeField] T value;
        [SerializeField] ResetOn resetOn;
        [NonSerialized] T runtimeValue;

        public T Value
        {
            get => resetOn == ResetOn.None ? value : runtimeValue;
            set
            {
                if (resetOn == ResetOn.None) this.value = value;
                else runtimeValue = value;
            }
        }

        void OnEnable()
        {
            switch (resetOn)
            {
                case ResetOn.ApplicationStart:
                    Initialize();
                    break;
                case ResetOn.SceneLoad:
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    break;
            }
        }

        void OnDisable()
        {
            if (resetOn == ResetOn.SceneLoad)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Initialize();

        void Initialize()
        {
            if (typeof(T).IsValueType)
            {
                // 값 타입은 자체가 깊은 복사
                Value = value;
            }
            else if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                // Unity Object 타입은 참조를 유지
                Value = value;
            }
            else if (value is ICloneable cloneable)
            {
                Value = (T)cloneable.Clone();
            }
            else
            {
                // 직렬화 가능한 참조 타입은 JSON을 통한 깊은 복사
                string json = JsonUtility.ToJson(value);
                Value = JsonUtility.FromJson<T>(json);
            }
        }
    }
}