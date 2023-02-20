public sealed partial class SimulationWorker
{
    private TransportInstanceUpdater UpdateTransport()
        => new TransportInstanceUpdater(ModelContext);


}
