public sealed partial class SimulationWorker
{
    private DriverInstanceUpdater UpdateDriver()
        => new DriverInstanceUpdater(ModelContext);

}
