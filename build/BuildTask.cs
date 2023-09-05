using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Clean;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;

[TaskName("RestoreWorkloads")]
public sealed class RestoreWorkloadsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        foreach (var project in context.ProjectNames)
        {
            context.DotNetWorkloadRestore(context.ProjectNameToPath(project).FullPath);
        }
    }
}

[TaskName("Clean")]
[IsDependentOn(typeof(RestoreWorkloadsTask))]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetCleanSettings
        {
            NoLogo = true,
            Verbosity = context.Verbosity,
        };

        foreach (var project in context.ProjectNames)
        {
            context.CleanDirectories(context.RootDirectory
                .Combine(project).Combine("obj").FullPath);
            context.CleanDirectories(context.RootDirectory
                .Combine(project).Combine("bin").Combine(context.MsBuildConfiguration).FullPath);
            context.DotNetClean(context.ProjectNameToPath(project).FullPath, settings);
        }

        context.CleanDirectories(context.RootDirectory
                .Combine(context.TestsProjectName).Combine("obj").FullPath);
        context.CleanDirectories(context.RootDirectory
                .Combine(context.TestsProjectName).Combine("bin").Combine(context.MsBuildConfiguration).FullPath);
        context.DotNetClean(context.ProjectNameToPath(context.TestsProjectName).FullPath, settings);
    }
}

[TaskName("Restore")]
[IsDependentOn(typeof(CleanTask))]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetRestoreSettings
        {
            Verbosity = context.Verbosity
        };
        foreach (var project in context.ProjectNames)
        {
            context.DotNetRestore(context.ProjectNameToPath(project).FullPath, settings);
        }
        context.DotNetRestore(context.ProjectNameToPath(context.TestsProjectName).FullPath, settings);
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = context.MsBuildConfiguration,
            NoLogo = true,
            Verbosity = context.Verbosity
        };
        foreach (var project in context.ProjectNames)
        {
            context.DotNetBuild(context.ProjectNameToPath(project).FullPath, settings);
        }
        context.DotNetBuild(context.ProjectNameToPath(context.TestsProjectName).FullPath, settings);
    }
}
