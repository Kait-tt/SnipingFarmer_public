@startuml

namespace Game {
  class GameManager {
    ~ ReactiveProperty<bool> IsPlaying
    ~ ReactiveCollection<Player> PlayerList
    ~ Player MyPlayer
    + void StartGame ()
  }

  class Player {
    .. serializable ..
    ~ int WalkSpeed
    ~ int CameraSpeed
    ~ int ScopingWalkSpeed
    ~ int ScopingCameraSpeed
    ....
    ~ ReactiveProperty<bool> CanMove
    ~ ReactiveProperty<bool> IsWarping
    + SnipingArea CurrentArea
    ____
    + void Init (PlayerMeta player)
  }

  class PlayerShotPenalty {
    .. serializable ..
    ~ int DurationSecond
    ....
    ~ ReactiveProperty<bool> IsEnabled
    ____
    + void EnablePenalty ()
  }

  class Weapon {
    .. serializable ..
    ~ float ScopeSensitivityRate
    ~ float ReloadDurationSecond
    ~ GameObject Bullet
    ....
    ~ ReactiveProperty<bool> IsScoping
    ~ ReactiveProperty<bool> IsReloading
    ~ ReactiveProperty<bool> CanShoot
    + void Shoot ()
    + void Scope ()
    + void Unscope ()
    - void Reload ()
  }

  class Bullet {
    .. serializable ..
    ~ int Speed
    ~ int Weight
    ....
    ~ Player ShotPlayer
    ~ SnipingArea ShotArea
    ____
    + void Init (Player shotPlayer, SnipingArea shotArea)
    - GameObject ComputeLandingObject ()
    - void Land ()
  }

  class SnipingArea {
    .. serializable ..
    + int WindSpeed
    + int WindDirection
    ....
    ____
  }

  class WarpPoint {
    .. serializable ..
    ~ Vector3 DistPosition
    ~ SnipingArea DistArea
    ....
    ____
    + void Warp (Player player)
  }

  interface ICapturable {
    + IObservable<ICapturable> OnCaptured
    + void Capture (Bullet bullet)
  }

  namespace Item {
    abstract AbstractItem
    class SmokeItem
    class StrongItem
    class DecoyItem

    class ItemSpawner {
      .. serializable ..
      ~ GameObject[] ItemList
      ~ int SpawnIntervalSeconds
      ~ int PoolSize
      ....
      - GameObject[] itemPool
      ____
      + void StartSpawnLoop ()
      + void StopSpawnLoop ()
      - void SpawnNext ()
      - Vector3 NextSpawnPosition ()
    }

    class Smoke {
      .. serializable ..
      ~ int DurationSecond
      ....
      ____
    }

    AbstractItem --o ItemSpawner
    AbstractItem ..|> Game.ICapturable
    SmokeItem --|> AbstractItem
    SmokeItem --> Smoke : create
    StrongItem --|> AbstractItem
    DecoyItem --|> AbstractItem
    StrongItem --> GameMeta.FruitStrong
  }

  namespace Fruit {
    abstract AbstractFruit {
      .. serializable ..
      ~ float SpawnProbability
      ~ int DefaultScorePoint
      ~ string DisplayName
      ....
      ~ ReactiveProperty<bool> IsStrong
      ~ ReactiveProperty<bool> IsFever
      ~ int ScorePoint
      ____
    }

    class Apple
    class Banana
    class Melon
    class Orange
    class Grape

    class FruitSpawner {
      .. serializable ..
      ~  GameObject[] FruitList
      ~ int SpawnInterval
      ~ int SpawnSize
      ~ int SpawnLimitSize
      ....
      ____
      + void StartSpawnLoop ()
      + void StopSpawnLoop ()
      - void SpawnNext ()
      - GameObject NextSpawnFruit ()
      - Vector3 NextSpawnPos ()
    }

    Apple --|> AbstractFruit
    Banana --|> AbstractFruit
    Melon --|> AbstractFruit
    Orange --|> AbstractFruit
    Grape --|> AbstractFruit
    AbstractFruit --o FruitSpawner
    AbstractFruit ..|> Game.ICapturable
    AbstractFruit ..> GameMeta.FruitStrong
    AbstractFruit ..> GameMeta.FruitFever

    namespace MovePoint {
      class BasicPoint

      class Decoy {
        .. serializable ..
        ~ int VisibleDistance
        ....
        ____
      }

      Decoy --|> BasicPoint
    }

    namespace MoveController {
      class BaseMove {
        .. serializable ..
        ~ int DefaultSpeed
        ....
        + int Speed
        - Vector3 targetMovePoint
        ____
        + void Move ()
        - Vector3 NextMovePoint ()
      }

      BaseMove ..> GameMeta.FruitStrong
      BaseMove --> Game.Fruit.MovePoint.BasicPoint
    }

    AbstractFruit --> Game.Fruit.MoveController.BaseMove

    class CaptureEffect {
      + {static} void exec (GameObject fruitObject, SnipingArea area)
    }

    CaptureEffect <-- AbstractFruit
    CaptureEffect --> Game.SnipingArea
  }

  Game.Item.DecoyItem --> Game.Fruit.MovePoint.Decoy : create

  WarpPoint --> SnipingArea

  Player --o GameManager
  Player --> Meta.PlayerMeta
  Player --> SnipingArea
  Player ..|> ICapturable
  Player --> PlayerShotPenalty
  WarpPoint --> Player

  Weapon --> Bullet : create
  Bullet --> Player
  Bullet --> SnipingArea

  ICapturable --> Bullet
}

@enduml
