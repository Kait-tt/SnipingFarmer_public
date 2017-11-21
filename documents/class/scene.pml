@startuml

namespace Scene {
  class SceneMaster {
    ~ ReactiveProperty<AbstractScene> CurrentScene
  }

  abstract AbstractScene

  class StartScene {
    + void GoToRoby ()
    - void LeaveRoby ()
  }

  class RobyScene {
    + void GoToStart ()
    + void GoToGame ()
    - Room EnterRoom (PlayerMeta player)
    - PlayerMeta CreatePlayer ()
  }

  class GameScene {
    + void GoToRoby ()
    + void GoToResult ()
    + void StopGame ()
  }

  class ResultScene {
    + void GoToStart ()
    + void GoToRoby ()
  }

  StartScene --|> AbstractScene
  RobyScene --|> AbstractScene
  GameScene --|> AbstractScene
  ResultScene --|> AbstractScene
  AbstractScene --o SceneMaster
}

Scene.AbstractScene ---> Meta.Room
Scene.RobyScene ---> Meta.PlayerMeta : create
Scene.RobyScene ---> Meta.PlayerNameFactory
Scene.RobyScene ...> Meta.ReadyTimer
Scene.GameScene ...> Meta.StopVote
Scene.GameScene ---> Game.GameManager
Scene.GameScene ...> GameMeta.GameTimer

@enduml
