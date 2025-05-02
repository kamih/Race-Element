using RaceElement.Core.Jobs.Loop;
using RaceElement.Data.Common;

namespace RaceElement.Data.Games;

public static class GameManager
{
    public static Game CurrentGame { get; private set; } = Game.Any;
    private static Game _runningGame = Game.Any;
    public static bool IsGameRunning { get => _isGameRunning; set => _isGameRunning = value; }

    private static bool _isGameRunning = false;

    public static event EventHandler<(Game previous, Game next)>? OnGameChanged;
    public static event EventHandler<Game>? OnAutoGameRequest;

    private static SimpleLoopJob? _dataUpdaterJob;

    private static SimpleLoopJob? _gameMonitorJob;

    private static SimpleLoopJob? _autoSwitchJob;

    public static bool AutoSwitch = true;

    public static void SetCurrentGame(Game nextGame)
    {
        ExitGameData(CurrentGame);

        SimDataProvider.Clear();
        Game previousGame = CurrentGame;
        CurrentGame = nextGame;
        OnGameChanged?.Invoke(null, (previousGame, nextGame));

        _gameMonitorJob = new()
        {
            Action = () =>
            {
                _runningGame = GameExtensions.GetRunningGame();
                _isGameRunning = CurrentGame == _runningGame;
            },
            IntervalMillis = 500
        };
        _gameMonitorJob.Run();

        _autoSwitchJob = new()
        {
            Action = () =>
            {
                if (AutoSwitch && !_isGameRunning && _runningGame != Game.Any)
                    OnAutoGameRequest?.Invoke(null, _runningGame);
            },
            IntervalMillis = 2000,
        };
        _autoSwitchJob.Run();

        SimDataProvider.Update(true);
        if (SimDataProvider.Instance != null)
        {
            SimDataProvider.Instance.Start();

            _dataUpdaterJob = new()
            {
                Action = () => SimDataProvider.Update(),
                IntervalMillis = 1000 / SimDataProvider.Instance.PollingRate()
            };
            _dataUpdaterJob.Run();
        }
    }

    /// <summary>
    /// Gracefully disposes and stops all mechanisms that are required for a game so they do not interfere with other games.
    /// </summary>
    /// <param name="game"></param>
    private static void ExitGameData(Game game)
    {
        _autoSwitchJob?.CancelJoin();
        _gameMonitorJob?.CancelJoin();
        _isGameRunning = false;

        SimDataProvider.Stop();
        _dataUpdaterJob?.CancelJoin();
    }
}
