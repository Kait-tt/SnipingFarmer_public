@startuml

namespace UI {
  class FruitFever {
    - void ShowEffect ()
  }

  class Time

  class InfoPanel
  class ScorePanel

  class MiniMap {
    .. serializable ..
    ~ int ShowDuration
    ~ int ShowInterval
    ....
    ____
  }

  class WindPanel {
    - int WindSpeed ()
    - int WindDirection ()
  }

  class StopVote

  FruitFever ...> GameMeta.FruitFever
  Time ...> GameMeta.GameTimer
  InfoPanel ...> GameMeta.FruitFever
  InfoPanel ...> GameMeta.FruitStrong
  InfoPanel ...> GameMeta.GameTimer
  InfoPanel ...> Game.GameManager
  ScorePanel ...> Game.GameManager
  MiniMap ---> Game.GameManager
  MiniMap ---> Game.Fruit.FruitSpawner
  WindPanel ---> Game.Player
  StopVote ...> Meta.StopVote
}

@enduml
