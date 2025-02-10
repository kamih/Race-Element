using RaceElement.Data.Common.Graph;
using RaceElement.Data.Games.RaceRoom.SharedMemory;
using RaceElement.Graph;
using RaceElement.Graph.Edge;
using System.Diagnostics;

namespace RaceElement.Data.Games.RaceRoom.DataMapper;
internal static class R3EDataGraphMapper
{
    public static void AddSharedMemory(DataGraph graph, Shared shared)
    {
        if (shared.SessionType == (int)Constants.Session.Unavailable)
            graph.ClearGraph();

        var existingCars = graph.Where(x => x is RacingCarNode).Select(x => x as RacingCarNode);
        var existingDrivers = graph.Where(x => x is RacingDriverNode).Select(x => x as RacingDriverNode);
        var existingLaps = graph.Where(x => x is LapTimeDataNode).Select(x => x as LapTimeDataNode);

        for (int i = 0; i < shared.DriverData.Length; i++)
        {
            var driverData = shared.DriverData[i];
            if (driverData.Place == -1) continue;

            var matchingCars = existingCars.Where(x => x?.CarNumber == driverData.DriverInfo.CarNumber);
            if (!matchingCars.Any())
            {
                var raceCarNode = new RacingCarNode()
                {
                    CarNumber = driverData.DriverInfo.CarNumber,
                    CarModelGameID = $"{driverData.DriverInfo.ModelId}",
                    Position = driverData.Place,
                    Laps = driverData.CompletedLaps,
                };
                var raceDriverNode = new RacingDriverNode()
                {
                    DriverId = driverData.DriverInfo.UserId,
                    Name = System.Text.Encoding.UTF8.GetString(driverData.DriverInfo.Name).TrimEnd('\0'),
                };
                graph.Add(raceCarNode);
                graph.Add(raceDriverNode);
                graph.TryAddEdge(new OwnsEdge() { ParentId = raceCarNode.Id, ChildId = raceDriverNode.Id });

                Debug.WriteLine($"Added new driver + car:\n- {raceCarNode}\n- {raceDriverNode}");
            }
            else
            {
                var car = matchingCars.FirstOrDefault();
                if (car != null)
                {
                    // Update data
                    if (car.Position != driverData.Place)
                        car.Position = driverData.Place;


                    if (car.Laps < driverData.CompletedLaps && graph.Edges.Any())
                    {
                        car.Laps = driverData.CompletedLaps;

                        int[] sectors = [
                            (int)(driverData.SectorTimePreviousSelf.Sector1 * 1000f),
                            (int)(driverData.SectorTimePreviousSelf.Sector2 * 1000f),
                            (int)(driverData.SectorTimePreviousSelf.Sector3 * 1000f),
                        ];

                        if (sectors[2] < 0)
                        {
                            // sector[2] is the laptime, it's not a sector in this case, if it's lower than 0 we will skip it.
                            continue;
                        }

                        LapTimeDataNode lapNode = new() { SectorTimesMs = sectors, LapIndex = car.Laps, LapTimeMs = (int)(driverData.SectorTimePreviousSelf.Sector3 * 1000f) };
                        Debug.WriteLine($"Added new lap for:\n- {car}\n- {lapNode}");
                        graph.Add(lapNode);

                        graph.TryGetEdgesFrom(car, out var carEdgesFrom);

                        RacingDriverNode driverNode = (RacingDriverNode)existingDrivers.First(x => carEdgesFrom.Select(x => x.ChildId).Contains(x.Id));


                        graph.TryAddEdge(new OwnsEdge() { ParentId = driverNode.Id, ChildId = lapNode.Id });
                    }
                }

            }
        }
    }
}
