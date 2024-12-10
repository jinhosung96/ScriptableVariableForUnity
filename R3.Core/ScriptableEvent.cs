#if R3_SUPPORT
using System;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JHS.ScriptableVariable.R3
{
    public class ScriptableEvent<T> : ScriptableObject
    {
        [SerializeField] ResetOn resetOn;
        [SerializeField] T debugValue;
        [field: NonSerialized] public Subject<T> Observable { get; private set; } = new();

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
            Observable = new();
        }
    }
}
#endif