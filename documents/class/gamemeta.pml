@startuml

namespace GameMeta {
  class GameTimer {
    .. serializable ..
    ~ int MaxTimeSecond
    ~ int HarryTimeSecond
    ....
    ~ IObservable<bool> OnTimeUp
    ~ IObservable<bool> OnHarryUp
    ~ ReactiveProperty<int> TimeMilliSecond
    ____
    + void StartTimer ()
  }

  class FruitFever {
    .. serializable ..
    ~ int DurationSecond
    ~ int[] RaiseTimeSecondList
    ....
    ~ ReactiveProperty<bool> IsEnabled
    ~ ReactiveProperty<bool> RemainedTimeSecond
    ~ ReactiveProperty<bool> GoodFruit
    ____
    - void Raise ()
  }

  class FruitStrong {
    .. serializable ..
    ~ int DurationSecond
    ....
    ~ ReactiveProperty<bool> IsEnabled
    ~ ReactiveProperty<bool> RemainedTimeSecond
    ____
    - void Raise ()
  }
}

@enduml
