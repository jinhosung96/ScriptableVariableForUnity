# ScriptableVariable 라이브러리

ScriptableVariable은 ScriptableObject의 불편한 점을 개선하여 전역 상태와 이벤트를 관리하는 유연하고 타입 안전한 유니티 라이브러리입니다.
이 라이브러리는 SOAP(Scriptable Objects Architecture Pattern)에서 영감을 받았으며, R3 무료 라이브러리를 활용하여 SOAP의 일부 기능을 제공합니다.

- ScriptableVariable
- ScriptableReactiveVariable(R3 사용 시)
- ScriptableEvent(R3 사용 시)

## 주요 기능

Scriptable Variable은 다음과 같은 공통 스파게티 코드 문제에 대한 노코드 솔루션을 제공하여 다음과 같은 작업을 할 수 있게 해줍니다

- 씬과 컴포넌트 간에 변수 공유
- 간단하고 강력한 방식으로 이벤트를 보내고 받기

## 설치 방법

1. 다음과 같은 방법으로 패키지를 유니티 프로젝트에 추가하세요:
   - `ScriptableVariable` 라이브러리를 프로젝트의 Assets 폴더에 복사

2. 리액티브 기능을 사용하려면 다음 사항을 확인하세요:
   - R3 패키지 설치
   - 프로젝트에 `R3_SUPPORT` 스크립팅 정의 심볼 추가

## 핵심 컴포넌트

### ScriptableVariable<T>

ScriptableObject에서 값을 저장하고 관리하는 기본 구현입니다.

```csharp
[CreateAssetMenu(fileName = "NewIntVariable", menuName = "Variables/Int Variable")]
public class IntVariable : ScriptableVariable<int> { }
```

#### 초기화 옵션

- `ResetOn.None`: 씬 로드와 애플리케이션 재시작 시에도 값 유지(R3 환경에서는 지원하지 않음)
- `ResetOn.SceneLoad`: 새로운 씬이 로드될 때 값 초기화
- `ResetOn.ApplicationStart`: 애플리케이션 시작 시 값 초기화

### ScriptableReactiveVariable<T>

R3를 사용한 리액티브 프로그래밍을 지원하는 확장 구현입니다.

```csharp
[CreateAssetMenu(fileName = "NewReactiveIntVariable", menuName = "Variables/Reactive/Int Variable")]
public class ReactiveIntVariable : ScriptableReactiveVariable<int> { }
```

### ScriptableEvent<T>

R3의 Subject 시스템을 사용한 타입 안전한 이벤트 시스템 구현입니다.

```csharp
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Events/Game Event")]
public class GameEvent : ScriptableEvent<string> { }
```

라이브러리는 매개변수가 없는 이벤트를 위한 `UnitEvent` 클래스도 포함합니다:

```csharp
[CreateAssetMenu(fileName = "NewUnitEvent", menuName = "Events/Unit Event")]
public class MyUnitEvent : UnitEvent { }
```

## 사용 예제

### ScriptableVariable 생성

1. Project 창에서 우클릭
2. Create > Variables > [원하는 변수 타입] 선택
3. 변수 에셋 이름 지정

### 기본 사용법

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

### 리액티브 사용법

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

### 이벤트 사용법

```csharp
public class GameSystem : MonoBehaviour
{
    [SerializeField] ScriptableEvent<string> gameEvent;
    [SerializeField] UnitEvent levelCompleteEvent;

    void Start()
    {
        // 이벤트 구독
        gameEvent.Observable
            .Subscribe(message => Debug.Log(message))
            .AddTo(this);

        levelCompleteEvent.Observable
            .Subscribe(_ => HandleLevelComplete())
            .AddTo(this);
    }

    void TriggerEvent()
    {
        // 이벤트 발생
        gameEvent.Observable.OnNext("Game Started!");
        levelCompleteEvent.Observable.OnNext(Unit.Default);
    }
}
```

## 에디터 통합

라이브러리는 다음을 제공하는 커스텀 프로퍼티 드로어를 포함합니다:
- 인스펙터에서 값 편집
- 쉬운 인스턴스 생성

## 참고 사항

- ResetOn 설정값에 따라 씬 로딩 및 애플리케이션 시작 시 값이 적절히 초기화됨
- 깊은 복사로 참조 타입 값의 적절한 격리 보장
- 에디터 수정사항이 올바르게 직렬화됨
- 런타임 변경사항은 설정에 따라 유지 또는 초기화 가능
- 씬 언로드 시 이벤트가 자동으로 정리됨