@startuml

namespace Meta {
  enum PlayerMetaState {
    WAITING
    READY
    STARTING
    PLAYING
  }

  enum RoomState {
    WAITING
    STARTING
    PLAYING
  }

  class PlayerNameFactory {
    + string MakePlayerName ()
  }

  class PlayerMeta {
    ~ ReactiveProperty<PlayerMetaState> State
    ~ ReactiveProperty<int> Score
    ~ ReactiveProperty<bool> IsVotedStopGame
    ~ string Name
    + void Init (string name)
    + void Ready ()
    + void CancelReady ()
    + void SetStartState ()
  }

  class Room {
    ~ ReactiveProperty<RoomState> State
    ~ ReactiveCollection<PlayerMeta> PlayerMetaList
    ~ ReactiveProperty<PlayerMeta> MyPlayerMeta
    ~ ReactiveProperty<bool> IsAllReady
    + void AddPlayer (PlayerMeta playerMeta)
    + void RemovePlayer (PlayerMeta playerMeta)
    + void StartGame ()
  }

  class ReadyTimer {
    .. serializable ..
    + int MaxTimeSecond
    ....
    ~ ReactiveProperty<bool> IsRunning
    ~ ReactiveProperty<int> Time
    ~ IObservable<bool> OnTimeUp
    ____
    + void StartTimer ()
    + void StopTimer ()
  }

  class StopVote {
    ~ ReactiveProperty<int> VotedCount
    ~ IObservable<bool> VotedAll
  }

  PlayerMeta --> PlayerMetaState
  PlayerMeta --o Room
  Room --> RoomState
  ReadyTimer ..> Room
  StopVote ..> PlayerMeta
}

@enduml
