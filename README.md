# Toolbelt.WorkDirectory [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.WorkDirectory.svg)](https://www.nuget.org/packages/Toolbelt.WorkDirectory/)

## Summary

Create a temporary working directory, copy files from a template, and finally, delete the working directory altogether when exiting a scope.

## Examples

```csharp
using Toolbelt;

// Create a temporary working directory under the application domain's base directory.
using var workDir = new WorkDirectory();

// The working directory name is a base36 string, like "ua6i0t8k6n".
Console.WriteLine($"Working directory created at: {workDir.Path}");

// Use the working directory
// ...

// The working directory object can be used as a string.
File.WriteAllText(Path.Combine(workDir, "test.txt"), "Hello, World!");

// The working directory is automatically deleted here
```

```csharp
using Toolbelt;

// Create a temporary working directory and copy files from a source directory,
// excluding "bin" and "obj" directories.
using var workDir = WorkDirectory.CreateCopyFrom("path/to/source", (file) => file.Name is not "bin" and not "obj");

// Use the working directory
// ...

// The working directory is automatically deleted here
```

## Release Notes

[Release Notes](https://github.com/jsakamoto/Toolbelt.WorkDirectory/blob/master/RELEASE-NOTES.txt)

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.WorkDirectory/blob/master/LICENSE)
