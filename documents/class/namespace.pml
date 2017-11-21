@startuml

namespace Scene {
}
namespace Meta {
}
namespace Game {
}
namespace GameMeta {
}
namespace UI {
}

namespace Sound {
}

Scene --+ Meta
Scene --+ GameMeta
Scene --+ Game

UI --+ Meta
UI --+ GameMeta
UI --+ Game

Game --+ Meta
Game --+ GameMeta

@enduml
