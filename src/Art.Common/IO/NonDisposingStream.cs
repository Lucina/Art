namespace Art.Common.IO;

internal class NonDisposingStream : DelegatingStream
{
    public NonDisposingStream(Stream innerStream) : base(innerStream)
    {
    }

    protected override void Dispose(bool disposing)
    {
    }

    public override ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
