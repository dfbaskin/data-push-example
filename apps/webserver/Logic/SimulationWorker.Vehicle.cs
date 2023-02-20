public sealed partial class SimulationWorker
{
    private VehicleInstanceUpdater UpdateVehicle()
        => new VehicleInstanceUpdater(ModelContext);

}
