using RaceElement.Core.Jobs.Loop;
using RaceElement.Data.Common;

namespace RaceElement.Data.Games;

public static class GameManager
{
    public static Game CurrentGame { get; private set; } = Game.Any;
    public static bool IsGameRunning { get => _isGameRunning; set => _isGameRunning = value; }

    private static bool _isGameRunning = false;

    public static event EventHandler<(Game previous, Game next)>? OnGameChanged;

    private static SimpleLoopJob? _dataUpdaterJob;

    private static SimpleLoopJob? _gameMonitorJob;

    public static void SetCurrentGame(Game nextGame)
    {
        ExitGameData(CurrentGame);

        SimDataProvider.Clear();
        Game previousGame = CurrentGame;
        CurrentGame = nextGame;
        OnGameChanged?.Invoke(null, (previousGame, nextGame));

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

        _gameMonitorJob = new()
        {
            Action = () =>
            {
                _isGameRunning = CurrentGame == GameExtensions.GetRunningGame();
            },
            IntervalMillis = 500
        };
        _gameMonitorJob.Run();
    }

    /// <summary>
    /// Gracefully disposes and stops all mechanisms that are required for a game so they do not interfere with other games.
    /// </summary>
    /// <param name="game"></param>
    private static void ExitGameData(Game game)
    {
        _gameMonitorJob?.CancelJoin();
        _isGameRunning = false;

        SimDataProvider.Stop();
        _dataUpdaterJob?.CancelJoin();
    }
}
