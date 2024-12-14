# ScriptableVariable Library

ScriptableVariable is a flexible and type-safe Unity library for managing global state and events by improving upon ScriptableObjects.
This library is inspired by SOAP (Scriptable Objects Architecture Pattern) and provides some of SOAP's functionality using the free R3 library.

- ScriptableVariable
- ScriptableReactiveVariable (when using R3)
- ScriptableEvent (when using R3)

## Key Features

Scriptable Variable provides no-code solutions for common spaghetti code problems, enabling you to:

- Share variables between scenes and components
- Send and receive events in a simple yet powerful way

## Installation

### Via Package Manager

For Unity 2019.3.4f1 or higher, you can install the package directly through the Package Manager using a Git URL.

1. Open Package Manager (Window > Package Manager)
2. Click '+' button and select "Add package from git URL"
3. Enter the following URL:
```
https://github.com/jinhosung96/ScriptableVariableForUnity.git
```

Alternatively, you can add it directly to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/ScriptableVariableForUnity.git"
  }
}
```

To install a specific version, add the #{version} tag to the URL:
```
https://github.com/jinhosung96/ScriptableVariableForUnity.git#1.0.0
```

## Core Components

### ScriptableVariable<T>

Basic implementation for storing and managing values in ScriptableObjects.

```csharp
[CreateAssetMenu(fileName = "NewIntVariable", menuName = "Variables/Int Variable")]
public class IntVariable : ScriptableVariable<int> { }
```

#### Reset Options

- `ResetOn.None`: Value persists across scene loads and application restarts (not supported in R3 environment)
- `ResetOn.SceneLoad`: Value resets when a new scene is loaded
- `ResetOn.ApplicationStart`: Value resets when the application starts

### ScriptableReactiveVariable<T>

Extended implementation supporting reactive programming using R3.

```csharp
[CreateAssetMenu(fileName = "NewReactiveIntVariable", menuName = "Variables/Reactive/Int Variable")]
public class ReactiveIntVariable : ScriptableReactiveVariable<int> { }
```

### ScriptableEvent<T>

Type-safe event system implementation using R3's Subject system.

```csharp
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Events/Game Event")]
public class GameEvent : ScriptableEvent<string> { }
```

The library also includes a `UnitEvent` class for parameterless events:

```csharp
[CreateAssetMenu(fileName = "NewUnitEvent", menuName = "Events/Unit Event")]
public class MyUnitEvent : UnitEvent { }
```

## Usage Examples

### Creating ScriptableVariable

1. Right-click in the Project window
2. Select Create > Variables > [Your Variable Type]
3. Name your variable asset

### Basic Usage

```csharp
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] IntVariable healthVariable;

    void TakeDamage(int damage)
    {
        healthVariable.Value -= damage;
    }
}
```

### Reactive Usage

```csharp
public class HealthUI : MonoBehaviour
{
    [SerializeField] ReactiveIntVariable healthVariable;
    [SerializeField] Text healthText;

    void Start()
    {
        healthVariable.OnValueChanged
            .Subscribe(health => healthText.text = $"HP: {health}")
            .AddTo(this);
    }
}
```

### Event Usage

```csharp
public class GameSystem : MonoBehaviour
{
    [SerializeField] ScriptableEvent<string> gameEvent;
    [SerializeField] UnitEvent levelCompleteEvent;

    void Start()
    {
        // Subscribe to events
        gameEvent.Observable
            .Subscribe(message => Debug.Log(message))
            .AddTo(this);

        levelCompleteEvent.Observable
            .Subscribe(_ => HandleLevelComplete())
            .AddTo(this);
    }

    void TriggerEvent()
    {
        // Trigger events
        gameEvent.Observable.OnNext("Game Started!");
        levelCompleteEvent.Observable.OnNext(Unit.Default);
    }
}
```

## Editor Integration

The library includes custom property drawers that provide:
- Value editing in inspector
- Easy instance creation

## Notes

- Values are properly initialized on scene loading and application start based on ResetOn settings
- Deep copying ensures proper isolation of reference type values
- Editor modifications are properly serialized
- Runtime changes can be configured to persist or reset based on settings
- Events are automatically cleaned up when scenes are unloaded