using RaceElement.Data.Common.SimulatorData.LocalCar;
using Riok.Mapperly.Abstractions;
using System.Numerics;
using static RaceElement.Data.Games.AssettoCorsa.SharedMemory.AcSharedMemory;

namespace RaceElement.Data.Games.AssettoCorsaEvo.DataMapper.LocalCar;

[Mapper]
internal static partial class PhysicsDataMapper
{
    internal static void InsertPhysicsPage(ref PageFilePhysics pagePhysics, PhysicsData commonData)
    {
        commonData.Velocity = pagePhysics.SpeedKmh;
        commonData.Acceleration = new(pagePhysics.AccG[0], pagePhysics.AccG[2], pagePhysics.AccG[1]);
        commonData.Rotation = Quaternion.CreateFromYawPitchRoll(pagePhysics.Heading, pagePhysics.Pitch, pagePhysics.Roll);
    }
}
