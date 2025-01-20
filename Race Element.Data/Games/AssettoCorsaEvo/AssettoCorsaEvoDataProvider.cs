using RaceElement.Data.Common;
using RaceElement.Data.Common.SimulatorData;
using RaceElement.Data.Common.SimulatorData.LocalCar;
using RaceElement.Data.Games.AssettoCorsaEvo.DataMapper;
using RaceElement.Data.Games.AssettoCorsaEvo.SharedMemory;
using System.Drawing;

namespace RaceElement.Data.Games.AssettoCorsaEvo;

internal sealed class AssettoCorsaEvoDataProvider : AbstractSimDataProvider
{
    static int lastPhysicsPacketId = -1;

    // AC1 seems to have only one class. Or at least no race class info in the telemetry.
    static string dummyCarClass = "Race";
    List<string> classes = [dummyCarClass];

    internal override int PollingRate() => 200;

    private static string GameName => Game.AssettoCorsaEvo.ToShortName();

    public sealed override void Update(ref LocalCarData localCar, ref SessionData sessionData, ref GameData gameData)
    {
        var physicsPage = AcEvoSharedMemory.Instance.ReadPhysicsPageFile();
        if (lastPhysicsPacketId == physicsPage.PacketId) // no need to remap the physics page if packet is the same
        {
            SimDataProvider.GameData.IsGamePaused = true;
            return;
        }
        else
        {
            SimDataProvider.GameData.IsGamePaused = false;
        }

        LocalCarMapper.AddPhysics(ref physicsPage, ref localCar, ref sessionData);

        gameData.Name = GameName;


        // For now only physics page works, so no need to map other pages.

        //var graphicsPage = AcEvoSharedMemory.Instance.ReadGraphicsPageFile();
        //var staticPage = AcEvoSharedMemory.Instance.ReadStaticPageFile();
        //LocalCarMapper.AddGraphics(ref graphicsPage, ref localCar, ref sessionData);

        //SessionData.Instance.PlayerCarIndex = graphicsPage.PlayerCarID;
        //SimDataProvider.LocalCar.CarModel.CarClass = dummyCarClass;

    }

    public override List<string> GetCarClasses()
    {
        return classes;
    }

    public override bool HasTelemetry()
    {
        return lastPhysicsPacketId > 0;
    }

    internal override void Stop()
    {
        // No-op
    }

    public override bool IsSpectating(int playerCarIndex, int focusedIndex)
    {
        // TODO: Can we spectate other cars in the pits in AC1?
        return false;
    }

    public override Color GetColorForCategory(string category)
    {
        return Color.White;
    }

    internal sealed override void Start()
    {
    }
}
