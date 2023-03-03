namespace Art.Common.Management;

/// <summary>
/// Represents a wrapper around a <see cref="FileStream"/> that operates on a temporary file before moving the file into place.
/// </summary>
/// <remarks>
/// This class is intended to only be used for file write scenarios.
/// As such, creation of instances with <see cref="FileAccess"/> mode without <see cref="FileAccess.Write"/> will trigger <see cref="ArgumentException"/> upon construction.
/// </remarks>
public class CommittableFileStream : CommittableDelegatingStream
{
    /// <summary>
    /// Destination path.
    /// </summary>
    public string DestinationPath => _path;

    private readonly string _path;
    private readonly string _pathForStream;
    private readonly string? _tempPath;

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
        InnerStream = new FileStream(_pathForStream, mode);
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
        InnerStream = new FileStream(_pathForStream, mode, access);
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
        InnerStream = new FileStream(_pathForStream, mode, access, share);
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
        InnerStream = new FileStream(_pathForStream, mode, access, share, bufferSize);
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
        InnerStream = new FileStream(_pathForStream, mode, access, share, bufferSize, useAsync);
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
        InnerStream = new FileStream(_pathForStream, mode, access, share, bufferSize, options);
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
                InnerStream = new FileStream(_pathForStream, options);
                return;
            }
            catch (IOException)
            {
                options.PreallocationSize = 0;
            }
        }
        InnerStream = new FileStream(_pathForStream, options);
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
            tempPath = CreateRandomPathForSibling(path);
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
    protected override void Commit(bool shouldCommit)
    {
        CommitCore(shouldCommit);
    }

    /// <inheritdoc />
    protected override ValueTask CommitAsync(bool shouldCommit)
    {
        CommitCore(shouldCommit);
        return ValueTask.CompletedTask;
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

    /// <summary>
    /// Creates a random path for the specified sibling path.
    /// </summary>
    /// <param name="sibling">Sibling path.</param>
    /// <param name="attempts">Maximum number of attempts to generate path.</param>
    /// <returns>Random path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sibling"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="sibling"/> (e.g. drive root) or <paramref name="attempts"/> (&lt;=0) value.</exception>
    /// <exception cref="IOException">Thrown if failed to create sibling path with specified attempts.</exception>
    private static string CreateRandomPathForSibling(string sibling, int attempts = 10)
    {
        string dir = Path.GetDirectoryName(sibling) ?? throw new ArgumentException("Sibling path cannot be a drive root", nameof(sibling));
        return CreateRandomPath(dir, attempts);
    }

    /// <summary>
    /// Creates a random path directly under the specified base directory.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    /// <param name="attempts">Maximum number of attempts to generate path.</param>
    /// <returns>Random path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="attempts"/> (&lt;=0) value.</exception>
    /// <exception cref="IOException">Thrown if failed to create sibling path with specified attempts.</exception>
    internal static string CreateRandomPath(string baseDirectory, int attempts = 10)
    {
        if (baseDirectory == null) throw new ArgumentNullException(nameof(baseDirectory));
        if (attempts <= 0) throw new ArgumentException("Invalid max number of attempts", nameof(attempts));
        for (int i = 0; i < attempts; i++)
        {
            string path = Path.Combine(baseDirectory, $"{Guid.NewGuid():N}.tmp");
            if (!File.Exists(path) && !Directory.Exists(path)) return path;
        }
        throw new IOException("Failed to create random path");
    }
}
