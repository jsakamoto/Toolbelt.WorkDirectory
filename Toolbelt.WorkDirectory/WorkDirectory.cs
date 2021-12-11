namespace Toolbelt;

public class WorkDirectory : IDisposable
{
    public string Path { get; }

    public WorkDirectory() : this(AppDomain.CurrentDomain.BaseDirectory)
    {
    }

    public WorkDirectory(string baseDir)
    {
        this.Path = System.IO.Path.Combine(baseDir, Guid.NewGuid().ToString("N"));
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
}
