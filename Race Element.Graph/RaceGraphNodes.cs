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

