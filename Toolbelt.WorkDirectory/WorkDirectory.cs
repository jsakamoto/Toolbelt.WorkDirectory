using System.Numerics;

namespace Toolbelt;

/// <summary>
/// Represents a temporary working directory that is automatically cleaned up when disposed.
/// </summary>
public class WorkDirectory : IDisposable
{
    /// <summary>
    /// Gets the path of the working directory.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkDirectory"/> class using the base directory of the current application domain.
    /// </summary>
    public WorkDirectory() : this(AppDomain.CurrentDomain.BaseDirectory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkDirectory"/> class using the specified base directory.
    /// </summary>
    /// <param name="baseDir">The base directory to create the working directory in.</param>
    public WorkDirectory(string baseDir)
    {
        this.Path = this.EnumFolderCandidates(baseDir).First(path => !Directory.Exists(path) && !File.Exists(path));
        Directory.CreateDirectory(this.Path);
    }

    /// <summary>
    /// Copies files from the specified source directory to the working directory.
    /// </summary>
    /// <param name="srcDir">The source directory to copy files from.</param>
    /// <param name="predicate">A function to filter which files or folders to copy.</param>
    private void CopyFrom(string srcDir, Func<(string Dir, string Name), bool>? predicate)
    {
        FileIO.XcopyDir(srcDir, dstDir: this.Path, predicate);
    }

    /// <summary>
    /// Deletes the working directory and its contents.
    /// </summary>
    public void Dispose()
    {
        try { FileIO.DeleteDir(this.Path); } catch { }
    }

    /// <summary>
    /// Returns the path of the working directory.
    /// </summary>
    /// <returns>The path of the working directory.</returns>
    public override string ToString() => this.Path;

    /// <summary>
    /// Implicitly converts a <see cref="WorkDirectory"/> to a string representing the path of the working directory.
    /// </summary>
    /// <param name="workDirectory">The <see cref="WorkDirectory"/> to convert.</param>
    public static implicit operator string(WorkDirectory workDirectory) => workDirectory.Path;

    /// <summary>
    /// Creates a new <see cref="WorkDirectory"/> and copies files from the specified source directory.
    /// </summary>
    /// <param name="srcDir">The source directory to copy files from.</param>
    /// <param name="predicate">A function to filter which files or folders to copy.</param>
    /// <returns>A new <see cref="WorkDirectory"/> with files copied from the source directory.</returns>
    public static WorkDirectory CreateCopyFrom(string srcDir, Func<(string Dir, string Name), bool>? predicate)
    {
        var workDir = new WorkDirectory();
        workDir.CopyFrom(srcDir, predicate);
        return workDir;
    }

    /// <summary>
    /// Enumerates candidate folder names for the working directory.
    /// </summary>
    /// <param name="baseDir">The base directory to create the working directory in.</param>
    /// <returns>An enumerable of candidate folder names.</returns>
    private IEnumerable<string> EnumFolderCandidates(string baseDir)
    {
        for (; ; ) yield return System.IO.Path.Combine(baseDir, ToBase36(Guid.NewGuid().ToByteArray()));
    }

    /// <summary>
    /// Converts a byte array to a Base36 string.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>A Base36 string representation of the byte array.</returns>
    private static string ToBase36(byte[] bytes)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

        var result = new char[10];
        var dividend = BigInteger.Abs(new BigInteger(bytes.Take(9).ToArray()));
        for (var i = 0; i < 10; i++)
        {
            dividend = BigInteger.DivRem(dividend, 36, out var remainder);
            result[i] = chars[(int)remainder];
        }

        return new string(result);
    }
}
