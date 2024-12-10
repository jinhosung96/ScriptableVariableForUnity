#if R3_SUPPORT
using System;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JHS.ScriptableVariable.R3
{
    public enum ResetOn
    {
        SceneLoad,
        ApplicationStart
    }
    
    public abstract class ScriptableReactiveVariable<T> : ScriptableObject
    {
        [SerializeField] T value;
        [SerializeField] ResetOn resetOn;
        [field: NonSerialized] public SerializableReactiveProperty<T> Observable { get; private set; } = new();

        public T Value
        {
            get => Observable.Value;
            set => Observable.Value = value;
        }

        public Observable<T> OnValueChanged => Observable;

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

            Observable?.Dispose();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Initialize();

        void Initialize()
        {
            Observable?.Dispose();

            if (typeof(T).IsValueType)
            {
                // 값 타입은 자체가 깊은 복사
                Observable = new(value);
            }
            else if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                // Unity Object 타입은 참조를 유지
                Observable = new(value);
            }
            else if (value is ICloneable cloneable)
            {
                Observable = new((T)cloneable.Clone());
            }
            else
            {
                // 직렬화 가능한 참조 타입은 JSON을 통한 깊은 복사
                string json = JsonUtility.ToJson(value);
                Observable = new(JsonUtility.FromJson<T>(json));
            }
        }
    }
}
#endif