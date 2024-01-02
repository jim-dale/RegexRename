namespace RegexRename.Support;

using System.Runtime.InteropServices;

/// <summary>
/// Foreground worker class.
/// </summary>
public class ForegroundWorker : IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly PosixSignalRegistration[] registrations;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForegroundWorker"/> class.
    /// </summary>
    public ForegroundWorker()
    {
        this.registrations =
        [
            PosixSignalRegistration.Create(PosixSignal.SIGINT, this.HandlePosixSignal),
            PosixSignalRegistration.Create(PosixSignal.SIGTERM, this.HandlePosixSignal),
        ];
    }

    /// <summary>
    /// Gets the cancellation token that can be used by derived classes to detect application shutdown.
    /// </summary>
    /// <value>The cancellation token that can be used by derived classes to detect application shutdown.</value>
    protected CancellationToken StoppingToken => this.cancellationTokenSource.Token;

    public virtual Task<int> ExecuteAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the current instance.
    /// </summary>
    /// <param name="disposing">Indicates whether this was called by a Dispose method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var registration in this.registrations)
            {
                registration.Dispose();
            }

            this.cancellationTokenSource.Dispose();
        }

        this.disposed = true;
    }

    /// <summary>
    /// POSIX signal handler.
    /// </summary>
    /// <param name="context">The <see cref="PosixSignalContext"/>.</param>
    private void HandlePosixSignal(PosixSignalContext context)
    {
        this.cancellationTokenSource.Cancel();
        context.Cancel = true;
    }
}
