using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;

namespace RaceElement.Graph;

public sealed record class LapTimeDataNode : AbstractNode
{
    /// <summary>
    /// Lap index (starting from 0). -1 means invalid and not set.
    /// </summary>
    public required int LapIndex { get; init; } = -1;

    /// <summary>
    /// The duration of the lap in milliseconds.
    /// -1 if invalid.
    /// </summary>
    public int LapTimeMs { get; init; } = -1;

    /// <summary>
    /// Sector Split times in milliseconds. Empty array if none exist.
    /// </summary>
    public int[] SectorTimesMs { get; init; } = [];


    /// <summary>
    /// Whether the lap is valid, true by default.
    /// </summary>
    public bool IsValid { get; init; } = true;
}

public sealed record class RacingDriverNode : AbstractNode
{
    /// <summary>
    /// Can be driver index or specific number if assigned.
    /// Should be used to identify the driver within the game.
    /// -1 is invalid.
    /// </summary>
    public required int DriverId { get; init; } = -1;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

public sealed record class RacingCarNode : AbstractNode
{
    public required int CarNumber { get; init; }

    /// <summary>
    /// Race Position
    /// </summary>
    public int Position { get; set; } = -1;
}

/// <summary>
/// Describes a <see cref="TrackStates"/> change for a <see cref="RacingCarNode"/>
/// </summary>
public record class TrackStateEdge : AbstractEdge
{
    /// <summary>
    /// Specifies the track state
    /// </summary>
    public TrackStates State { get; init; } = TrackStates.None;
}

[Flags]
public enum TrackStates : uint
{
    None = 0,
    PitLaneIn = 1,
    PitLaneOut = 2,
    Pitlane = 3,
    Track = 4,
    OffTrack = 5,
}


