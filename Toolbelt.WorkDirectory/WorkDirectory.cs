using System.Numerics;

namespace Toolbelt;

public class WorkDirectory : IDisposable
{
    public string Path { get; }

    public WorkDirectory() : this(AppDomain.CurrentDomain.BaseDirectory)
    {
    }

    public WorkDirectory(string baseDir)
    {
        this.Path = this.EnumFolderCandidates(baseDir).First(path => !Directory.Exists(path) && !File.Exists(path));
        Directory.CreateDirectory(this.Path);
    }

    private void CopyFrom(string srcDir, Func<(string Dir, string NAme), bool>? predicate)
    {
        FileIO.XcopyDir(srcDir, dstDir: this.Path, predicate);
    }

    public void Dispose()
    {
        try { FileIO.DeleteDir(this.Path); } catch { }
    }

    public override string ToString() => this.Path;

    public static implicit operator string(WorkDirectory workDirectory) => workDirectory.Path;

    public static WorkDirectory CreateCopyFrom(string srcDir, Func<(string Dir, string Name), bool>? predicate)
    {
        var workDir = new WorkDirectory();
        workDir.CopyFrom(srcDir, predicate);
        return workDir;
    }

    private IEnumerable<string> EnumFolderCandidates(string baseDir)
    {
        for (; ; ) yield return System.IO.Path.Combine(baseDir, ToBase36(Guid.NewGuid().ToByteArray()));
    }

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
