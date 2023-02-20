public record SimulationContext(
    CancellationToken Token
)
{
    public void VerifyIsActive()
    {
        Token.ThrowIfCancellationRequested();
    }
}
