classDiagram
    %% Base Classes
    class Form {
        <<abstract>>
    }
    
    class Player {
        #int score
        #string name
        +UpdateScore(int score)
        +GetScore() int
        +ResetScore()
    }

    %% Forms
    class ModeSelectionScreen {
        +OnPvPClick()
        +OnPvEClick()
        +OnFormClosed()
    }

    class GameController {
        -Panel gamePanel
        -Label scoreLbP1
        -Label scoreLbP2
        -Label timeLb
        +SetGameMode(string)
        +UpdateScore(int, Player)
        +UpdateTime(int)
        +HandleGameFinished(bool)
        -RegisterEvents()
    }

    %% Managers
    class FormManager {
        <<singleton>>
        -static FormManager instance
        +Instance FormManager$
        +NavigateToGame(string)
        +GoBack()
        +NavigateToWinScreen()
        +NavigateToGameOver()
    }

    class AudioManager {
        <<singleton>>
        -static AudioManager instance
        +Instance AudioManager$
        +PlaySound(string, float)
    }

    class GameManager {
        <<singleton>>
        -Timer timer
        -Player player1
        -Player player2
        -Player currentPlayer
        -string gameMode
        -int timeRemaining
        +OnScoreUpdated event~int,Player~
        +OnTimeUpdated event~int~
        +OnGameFinished event~bool~
        +Instance GameManager$
        +Initialize(string)
        +StartGame(Panel)
        +AddScore(int, Player)
        +SwitchTurn()
        +CheckIfTheGameIsFinished()
    }

    class GridManager {
        <<singleton>>
        -Grid grid
        -PictureBox[,] pictureGrid
        -Dictionary~PictureBox,Image~ originalImages
        -bool firstGuess
        -bool secondGuess
        +Instance GridManager$
        +GenerateGrid(Panel)
        +GetUnmatchedBoxes() List~PictureBox~
        +AreImagesMatching(PictureBox, PictureBox) bool
        +BotClickCell(PictureBox) Task
        +GetOriginalImage(PictureBox) Image
    }

    class Grid {
        -int rows
        -int cols
        -PictureBox[,] pictureGrid
        -Node[,] nodes
        -List~Image~ imagesList
        -Dictionary~Image,ScoreGroup~ imageScoreGroups
        +GenerateGrid()
        +HasPath(Node, Node) bool
        +GetNodeFromPictureBox(PictureBox) Node
        +GetScoreForImage(Image) ScoreGroup
        +DrawPath(List~Node~, Panel) Task
    }

    class Bot {
        -Random random
        -float IQ
        +Bot(float)
        +MakeMove(GridManager) Task
        -FindMatchingPairs(List~PictureBox~) List~Tuple~
    }

    %% Inheritance
    Bot --|> Player
    ModeSelectionScreen --|> Form
    GameController --|> Form

    %% Composition
    GameManager "1" *-- "2" Player
    GameManager "1" *-- "1" GridManager
    GridManager "1" *-- "1" Grid

    %% Dependencies & Associations
    FormManager ..> Form : manages
    GameController --> GameManager : uses
    GameController --> GridManager : uses
    Bot ..> GridManager : uses
    GridManager ..> GameManager : uses
    Grid ..> AudioManager : uses
    GameManager ..> AudioManager : uses