using RaceElement.Data.ACC.Core;
using RaceElement.Data.Games;
using System;
using System.Threading;
using System.Threading.Tasks;
using static RaceElement.ACCSharedMemory;

namespace RaceElement.Data.ACC.Tracker;

public sealed class PagePhysicsTracker : IDisposable
{
    private static PagePhysicsTracker _instance;
    public static PagePhysicsTracker Instance
    {
        get
        {
            _instance ??= new PagePhysicsTracker();

            return _instance;
        }
    }

    public event EventHandler<SPageFilePhysics> Tracker;

    private bool isTracking = false;

    private readonly Task trackingTask;

    private PagePhysicsTracker()
    {
        trackingTask = Task.Run(() =>
        {
            isTracking = true;
            while (isTracking)
            {
                if (GameManager.IsGameRunning && GameManager.CurrentGame == Game.AssettoCorsaCompetizione)
                {
                    Thread.Sleep(2);
                    Tracker?.Invoke(this, ACCSharedMemory.Instance.ReadPhysicsPageFile());
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        });

        _instance = this;
    }

    public void Dispose()
    {
        isTracking = false;
        trackingTask.Wait();
        trackingTask.Dispose();
    }
}
