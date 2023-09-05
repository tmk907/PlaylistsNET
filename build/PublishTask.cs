using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet;
using Cake.Frosting;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Core.Diagnostics;
using System;

[TaskName("Pack")]
[IsDependentOn(typeof(TestTask))]
public sealed class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetPackSettings
        {
            Configuration = context.MsBuildConfiguration,
            OutputDirectory = context.ArtifactsDirectory,
            NoBuild = true,
            NoRestore = true,
            NoLogo = true,
        };
        foreach (var project in context.ProjectNames)
        {
            context.DotNetPack(context.ProjectNameToPath(project).FullPath, settings);
        }
    }
}

[TaskName("PublishGithub")]
[IsDependentOn(typeof(PackTask))]
public sealed class PublishGithubTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.BuildSystem().GitHubActions.IsRunningOnGitHubActions)
        {
            if (string.IsNullOrEmpty(context.GitHubSecretToken))
            {
                throw new ArgumentException("GitHubSecretToken is null or empty");
            }

            var pattern = context.ArtifactsDirectory.FullPath + "/*.nupkg";
            foreach (var file in context.GetFiles(pattern))
            {
                context.Log.Information("Publishing {0}...", file.FullPath);
                context.DotNetNuGetPush(file, new DotNetNuGetPushSettings
                {
                    ApiKey = context.GitHubSecretToken,
                    Source = "https://nuget.pkg.github.com/tmk907/index.json",
                    SkipDuplicate = true,
                });
            }
        }
        else
        {
            context.Log.Warning("Not running on GitHub Actions. Can't publish package to Github.");
        }
    }
}

[TaskName("PublishNuget")]
[IsDependentOn(typeof(PackTask))]
public sealed class PublishNugetTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.BuildSystem().GitHubActions.IsRunningOnGitHubActions)
        {
            if (string.IsNullOrEmpty(context.NugetApiKey))
            {
                throw new ArgumentException("NugetApiKey is null or empty");
            }

            var pattern = context.ArtifactsDirectory.FullPath + "/*.nupkg";
            foreach (var file in context.GetFiles(pattern))
            {
                context.Log.Information("Publishing {0}", file.FullPath);
                context.DotNetNuGetPush(file, new DotNetNuGetPushSettings
                {
                    ApiKey = context.NugetApiKey,
                    Source = "https://api.nuget.org/v3/index.json",
                    SkipDuplicate = true,
                });
            }
        }
        else
        {
            context.Log.Warning("Not running on GitHub Actions. Can't publish package to nuget.");
        }
    }
}

[TaskName("PublishLocal")]
[IsDependentOn(typeof(PackTask))]
public sealed class PublishLocalTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.BuildSystem().IsLocalBuild)
        {
            var pattern = context.ArtifactsDirectory.FullPath + "/*.nupkg";
            foreach (var file in context.GetFiles(pattern))
            {
                context.Log.Information("Publishing {0}", file.FullPath);
                context.DotNetNuGetPush(file, new DotNetNuGetPushSettings
                {
                    Source = context.LocalNugetRepo,
                    SkipDuplicate = true,
                });
            }
        }
        else
        {
            context.Log.Warning("Not local build. Can't publish package.");
        }
    }
}
