using RaceElement.Data.ACC.EntryList;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RaceElement.HUD.ACC.Overlays.Driving.ExpectedQualifying;

[Overlay(
    Name = "Expected Qualifying",
Description = "Works after setting a valid lap, shows expected position and delta to purple lap.\nWill not display in racing sessions, by default only qualifying with optionally practice."
)]
internal sealed class ExpectedQualifyingOverlay(Rectangle rectangle) : AbstractOverlay(rectangle, "Expected Qualifying")
{
    private readonly ExpectedQualifyingConfiguration _config = new();
    private sealed class ExpectedQualifyingConfiguration : OverlayConfiguration
    {
        [ConfigGrouping("Visibility", "Adjust visibility for other session types than qualifying.")]
        public VisibilityGrouping Visibility { get; init; } = new();
        public sealed class VisibilityGrouping
        {
            [ToolTip("If enabled the HUD will also show during practice sessions.")]
            public bool ShowInPractice { get; init; } = true;
        }
    }

    private InfoPanel _panel;

    private SolidBrush _purpleBrush;
    private SolidBrush _greenBrush;
    private SolidBrush _redBrush;

    public sealed override void BeforeStart()
    {
        _panel = new(11, 300);

        _purpleBrush = new SolidBrush(Color.Purple);
        _greenBrush = new SolidBrush(Color.LimeGreen);
        _redBrush = new SolidBrush(Color.Red);
    }

    public sealed override void BeforeStop()
    {
        _panel?.Dispose();

        _purpleBrush?.Dispose();
        _greenBrush?.Dispose();
        _redBrush?.Dispose();
    }

    public sealed override bool ShouldRender()
    {
        bool isCorrectSession = false;
        switch (broadCastRealTime.SessionType)
        {
            case Broadcast.RaceSessionType.Practice:
                {
                    if (_config.Visibility.ShowInPractice)
                        isCorrectSession = true;
                    break;
                }
            case Broadcast.RaceSessionType.Qualifying: { isCorrectSession = true; break; }

            default: break;
        }

        return isCorrectSession && base.ShouldRender();
    }

    public sealed override void Render(Graphics g)
    {
        if (pageGraphics.BestTimeMs > 0 && broadCastRealTime.BestSessionLap != null)
        {
            int purpleLapMs = broadCastRealTime.BestSessionLap.GetLapTimeMS();
            int localExpectedMs = pageGraphics.EstimatedLapTimeMillis;
            int deltaToPurple = purpleLapMs - localExpectedMs;

            _panel.AddLine("Purple Delta", $"{TimeSpan.FromMilliseconds(deltaToPurple):SS\\.fff}", deltaToPurple < 0 ? _purpleBrush : _greenBrush);


            HashSet<int> fasterThanPositions = [];
            foreach (var car in EntryListTracker.Instance.Cars)
            {
                if (car.Value.CarInfo == null) continue;
                if (car.Value.RealtimeCarUpdate.BestSessionLap != null)
                {
                    int otherCarLapMs = car.Value.RealtimeCarUpdate.BestSessionLap.GetLapTimeMS();
                    if (localExpectedMs < otherCarLapMs)
                        fasterThanPositions.Add(car.Value.RealtimeCarUpdate.Position);

                    break;
                }
            }
            int expectedPosition = pageGraphics.Position;
            if (fasterThanPositions.Count > 0) expectedPosition = fasterThanPositions.Min();
            _panel.AddLine("Position?", $"{expectedPosition}", expectedPosition <= pageGraphics.Position ? (deltaToPurple < 0 ? _purpleBrush : _greenBrush) : _redBrush);
        }
        else
        {
            _panel.AddLine("Purple Delta", "?");
            _panel.AddLine("Position", "?");
        }
    }
}
