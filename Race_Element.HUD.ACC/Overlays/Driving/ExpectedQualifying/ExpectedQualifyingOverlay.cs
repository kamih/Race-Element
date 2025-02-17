using RaceElement.Data.ACC.EntryList;
using RaceElement.Data.ACC.Session;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static RaceElement.Data.ACC.EntryList.EntryListTracker;

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

    private SolidBrush _whiteBrush;
    private SolidBrush _purpleBrush;
    private SolidBrush _greenBrush;
    private SolidBrush _redBrush;

    public sealed override void BeforeStart()
    {
        _panel = new(11, 220);
        Width = 220;
        Height = _panel.FontHeight * 2;
        RefreshRateHz = 2;

        _whiteBrush = new SolidBrush(Color.White);
        _purpleBrush = new SolidBrush(Color.Purple);
        _greenBrush = new SolidBrush(Color.LimeGreen);
        _redBrush = new SolidBrush(Color.Red);
    }

    public sealed override void BeforeStop()
    {
        _panel?.Dispose();

        _whiteBrush?.Dispose();
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

        return isCorrectSession && (base.ShouldRender() || RaceSessionState.IsSpectating(pageGraphics.PlayerCarID, broadCastRealTime.FocusedCarIndex));
    }

    public sealed override void Render(Graphics g)
    {
        CarData localCar = GetLocalCar();

        if (broadCastRealTime.BestSessionLap != null && localCar != null && localCar.RealtimeCarUpdate.BestSessionLap != null && localCar.RealtimeCarUpdate.BestSessionLap.LaptimeMS != null)
        {
            int purpleLapMs = broadCastRealTime.BestSessionLap.GetLapTimeMS();
            int localBestLapMs = localCar.RealtimeCarUpdate.BestSessionLap.GetLapTimeMS();
            int localExpectedMs = localBestLapMs + localCar.RealtimeCarUpdate.Delta;
            int deltaToPurple = purpleLapMs - localExpectedMs;

            _panel.AddLine("Purple Delta", $"{(deltaToPurple > 0 ? "-" : "+")}{TimeSpan.FromMilliseconds(deltaToPurple):s\\.fff}");
            HashSet<int> fasterThanPositions = [];
            foreach (var car in EntryListTracker.Instance.Cars)
            {
                if (car.Value.CarInfo == null) continue;
                if (car.Value.RealtimeCarUpdate.BestSessionLap != null)
                {
                    int otherCarLapMs = car.Value.RealtimeCarUpdate.BestSessionLap.GetLapTimeMS();
                    if (localExpectedMs < otherCarLapMs)
                        fasterThanPositions.Add(car.Value.RealtimeCarUpdate.Position);
                }
            }
            int expectedPosition = localCar.RealtimeCarUpdate.Position;
            if (fasterThanPositions.Count > 0) expectedPosition = fasterThanPositions.Min();
            int positionGain = localCar.RealtimeCarUpdate.Position - expectedPosition;
            string positionString = $"{expectedPosition}";
            if (positionGain > 0) positionString += $" (+{positionGain})";
            SolidBrush positionBrush = expectedPosition switch
            {
                1 => _purpleBrush,
                var p when p < localCar.RealtimeCarUpdate.Position => _greenBrush,
                var p when p == localCar.RealtimeCarUpdate.Position => _whiteBrush,
                _ => _redBrush,
            };
            _panel.AddLine("Position?", $"{positionString}", positionBrush);
        }
        else
        {
            _panel.AddLine("Purple Delta", "?");
            _panel.AddLine("Position", "?");
        }
        _panel.Draw(g);
    }

    private CarData GetLocalCar()
    {
        int focussedIndex = broadCastRealTime.FocusedCarIndex;
        if (focussedIndex < 0)
            return null;

        CarData localCar = null;
        foreach (var car in EntryListTracker.Instance.Cars)
        {
            if (car.Value.CarInfo == null) continue;
            if (car.Key == focussedIndex)
            {
                localCar = car.Value;
                break;
            }
        }

        return localCar;
    }
}
