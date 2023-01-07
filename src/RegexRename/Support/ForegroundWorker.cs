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
        registrations = new[]
        {
            PosixSignalRegistration.Create(PosixSignal.SIGINT, HandlePosixSignal),
            PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandlePosixSignal),
        };
    }

    /// <summary>
    /// Gets the cancellation token that can be used by derived classes to detect application shutdown.
    /// </summary>
    /// <value>The cancellation token that can be used by derived classes to detect application shutdown.</value>
    protected CancellationToken StoppingToken => cancellationTokenSource.Token;

    public virtual Task<int> ExecuteAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the current instance.
    /// </summary>
    /// <param name="disposing">Indicates whether this was called by a Dispose method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var registration in registrations)
            {
                registration.Dispose();
            }

            cancellationTokenSource.Dispose();
        }

        disposed = true;
    }

    /// <summary>
    /// POSIX signal handler.
    /// </summary>
    /// <param name="context">The <see cref="PosixSignalContext"/>.</param>
    private void HandlePosixSignal(PosixSignalContext context)
    {
        cancellationTokenSource.Cancel();
        context.Cancel = true;
    }
}
