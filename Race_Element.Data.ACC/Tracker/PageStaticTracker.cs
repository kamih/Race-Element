using RaceElement.Core.Jobs.Loop;
using RaceElement.Data.ACC.Core;
using RaceElement.Data.Games;
using System;
using static RaceElement.ACCSharedMemory;

namespace RaceElement.Data.ACC.Tracker;

public sealed class PageStaticTracker : AbstractLoopJob
{
    private static PageStaticTracker _instance;
    public static PageStaticTracker Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PageStaticTracker();

            return _instance;
        }
    }

    public PageStaticTracker()
    {
        IntervalMillis = 100;
    }

    public static event EventHandler<SPageFileStatic> Tracker;

    public override void RunAction()
    {
        if (GameManager.IsGameRunning && GameManager.CurrentGame == Game.AssettoCorsaCompetizione)
            Tracker?.Invoke(null, ACCSharedMemory.Instance.ReadStaticPageFile());
    }
}
