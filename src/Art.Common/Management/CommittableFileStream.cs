namespace Art.Common.Management;

/// <summary>
/// Represents a wrapper around a <see cref="FileStream"/> that operates on a temporary file before moving the file into place.
/// </summary>
/// <remarks>
/// This class is intended to only be used for file write scenarios.
/// As such, creation of instances with <see cref="FileAccess"/> mode without <see cref="FileAccess.Write"/> will trigger <see cref="ArgumentException"/> upon construction.
/// </remarks>
public class CommittableFileStream : CommittableWrappingStream
{
    /// <summary>
    /// Destination path.
    /// </summary>
    public string DestinationPath => _path;

    private readonly string _path;
    private readonly string _pathForStream;
    private readonly string? _tempPath;
    private bool _committed;

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <seealso cref="FileStream(string,FileMode)"/>
    public CommittableFileStream(string path, FileMode mode)
    {
        // Implicit write access
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <param name="access">File access mode.</param>
    /// <seealso cref="FileStream(string,FileMode,FileAccess)"/>
    public CommittableFileStream(string path, FileMode mode, FileAccess access)
    {
        EnsureWriting(access, nameof(access));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode, access);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <param name="access">File access mode.</param>
    /// <param name="share">File share mode.</param>
    /// <seealso cref="FileStream(string,FileMode,FileAccess,FileShare)"/>
    public CommittableFileStream(string path, FileMode mode, FileAccess access, FileShare share)
    {
        EnsureWriting(access, nameof(access));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode, access, share);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <param name="access">File access mode.</param>
    /// <param name="share">File share mode.</param>
    /// <param name="bufferSize">Buffer size.</param>
    /// <seealso cref="FileStream(string,FileMode,FileAccess,FileShare,int)"/>
    public CommittableFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
    {
        EnsureWriting(access, nameof(access));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode, access, share, bufferSize);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <param name="access">File access mode.</param>
    /// <param name="share">File share mode.</param>
    /// <param name="bufferSize">Buffer size.</param>
    /// <param name="useAsync">If true, try to enable async I/O.</param>
    /// <seealso cref="FileStream(string,FileMode,FileAccess,FileShare,int,bool)"/>
    public CommittableFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
    {
        EnsureWriting(access, nameof(access));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode, access, share, bufferSize, useAsync);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="mode">File opening mode.</param>
    /// <param name="access">File access mode.</param>
    /// <param name="share">File share mode.</param>
    /// <param name="bufferSize">Buffer size.</param>
    /// <param name="options">Advanced file options.</param>
    /// <seealso cref="FileStream(string,FileMode,FileAccess,FileShare,int,FileOptions)"/>
    public CommittableFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
    {
        EnsureWriting(access, nameof(access));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        BaseStream = new FileStream(_pathForStream, mode, access, share, bufferSize, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableFileStream"/>.
    /// </summary>
    /// <param name="path">Destination path.</param>
    /// <param name="options">File stream options.</param>
    /// <seealso cref="FileStream(string,FileStreamOptions)"/>
    public CommittableFileStream(string path, FileStreamOptions options)
    {
        EnsureWriting(options.Access, nameof(options));
        EnsureAccess(_path = path, nameof(path), out _pathForStream, out _tempPath);
        if (options.PreallocationSize != 0 && options.Mode is FileMode.Create or FileMode.CreateNew)
        {
            // Try to initialize with the provided preallocation size.
            // Failing that, retry with size 0.
            try
            {
                BaseStream = new FileStream(_pathForStream, options);
                return;
            }
            catch (IOException)
            {
                options.PreallocationSize = 0;
            }
        }
        BaseStream = new FileStream(_pathForStream, options);
    }

    private static void EnsureWriting(FileAccess access, string arg)
    {
        if ((access & FileAccess.Write) == 0)
            throw new ArgumentException($"Cannot create an instance of {nameof(CommittableFileStream)} that does not write", arg);
    }

    private static void EnsureAccess(string path, string arg, out string pathForStream, out string? tempPath)
    {
        FileInfo fi = new(path);
        if (fi.Exists)
        {
            if (fi.IsReadOnly) throw new IOException("File exists and is read-only");
            tempPath = ArtUtils.CreateRandomPathForSibling(path);
            pathForStream = tempPath;
        }
        else
        {
            string? dir = Path.GetDirectoryName(path);
            if (dir == null) throw new ArgumentException("Target path is not a valid file path", arg);
            DirectoryInfo di = new(dir);
            if (!di.Exists) throw new ArgumentException("Directory for file does not exist");
            tempPath = null;
            pathForStream = path;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        DisposeStream();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await DisposeStreamAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Commit(bool shouldCommit)
    {
        if (_committed) return;
        _committed = true;
        DisposeStream();
        CommitCore(shouldCommit);
    }

    /// <inheritdoc />
    protected override async ValueTask CommitAsync(bool shouldCommit)
    {
        if (_committed) return;
        _committed = true;
        await DisposeStreamAsync().ConfigureAwait(false);
        CommitCore(shouldCommit);
    }

    private void CommitCore(bool shouldCommit)
    {

        if (shouldCommit)
        {
            if (_tempPath != null) File.Replace(_tempPath, _path, null, true);
        }
        else
        {
            if (File.Exists(_pathForStream))
                File.Delete(_pathForStream);
        }
    }
}
