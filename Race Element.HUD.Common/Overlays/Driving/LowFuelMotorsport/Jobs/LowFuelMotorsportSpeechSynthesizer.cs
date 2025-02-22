using System.Speech.Synthesis;
using RaceElement.Core.Jobs;
using RaceElement.Core.Jobs.Timer;

namespace RaceElement.HUD.Common.Overlays.Driving.LowFuelMotorsport.Jobs;

internal sealed class LowFuelMotorsportSpeechSynthesizer(DateTime RaceStartTimeUtc, LowFuelMotorsportOverlay LfmOverlay) : IJob
{
    public bool IsRunning { get; private set; } = false;

    private readonly Guid _id = Guid.NewGuid();
    public Guid JobId => _id;

    public void Cancel() { }

    public void Run()
    {
        var timeDiff = (RaceStartTimeUtc - DateTime.UtcNow);
        string message = $"{timeDiff.Minutes} minutes until race starts";
        int minTimeRaceStartSec = 5;

        if (timeDiff.Hours > 0)
        {
            var h = timeDiff.Hours > 1 ? "hours" : "hour";
            message = $"{timeDiff.Hours} {h} and {message}";
        }

        if (timeDiff.TotalDays > 1)
        {
            var d = timeDiff.TotalDays > 2 ? "days" : "day";
            message = $"{timeDiff.Days} {d} and {message}";
        }

        if (timeDiff.TotalSeconds <= minTimeRaceStartSec)
        {
            message = "Race has started";
        }
        else if (timeDiff.TotalSeconds < 60)
        {
            message = "Race starts in less than 1 minute";
        }

        var speech = new SpeechSynthesizer();
        speech.Speak(message);
        speech.Dispose();

        if (timeDiff.TotalSeconds > minTimeRaceStartSec)
        {
            NextMessage((int)timeDiff.TotalSeconds);
        }
    }

    private void NextMessage(int remainingTimeSeconds)
    {
        DateTime time = remainingTimeSeconds switch
        {
            > (5 * 60) => RaceStartTimeUtc.Subtract(TimeSpan.FromMinutes(5)),
            > (3 * 60) => RaceStartTimeUtc.Subtract(TimeSpan.FromMinutes(3)),
            > (1 * 60) => RaceStartTimeUtc.Subtract(TimeSpan.FromMinutes(1)),
            _ => RaceStartTimeUtc,
        };

        LowFuelMotorsportSpeechSynthesizer speech = new(RaceStartTimeUtc, LfmOverlay);
        if (JobTimerExecutor.Instance().Add(speech, time, out Guid jobId))
            LfmOverlay._speechJobIds.Add(jobId);
    }
}
