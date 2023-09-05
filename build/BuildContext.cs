using Cake.Common.Build;
using Cake.Common.Tools.DotNet;
using Cake.Common;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using System.Collections.Generic;

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration => this.ArgumentOrEnvironment("configuration", Constants.DefaultBuildConfiguration);
    public string GitHubSecretToken => this.ArgumentOrEnvironment<string>("GITHUB_TOKEN");
    public string NugetApiKey => this.ArgumentOrEnvironment<string>("NUGET_API_KEY");
    public string LocalNugetRepo = @"C:\Source\NugetRepo";

    public DirectoryPath RootDirectory { get; }
    public FilePath SolutionFile { get; }
    public List<string> ProjectNames { get; } = new List<string>()
    {
        "PlaylistsNET",
    };
    public string TestsProjectName { get; } = "PlaylistsNET.Tests";
    public DirectoryPath ArtifactsDirectory { get; }

    public DotNetVerbosity Verbosity => DotNetVerbosity.Normal;

    public bool IsRunningInCI
        => this.GitHubActions()?.IsRunningOnGitHubActions ?? false;

    public BuildContext(ICakeContext context)
        : base(context)
    {
        if (this.IsRunningInCI)
        {
            RootDirectory = this.GitHubActions().Environment.Workflow.Workspace;
        }
        else
        {
            RootDirectory = this.Environment.WorkingDirectory.GetParent();
        }

        SolutionFile = RootDirectory.CombineWithFilePath("PlaylistsNET.sln");
        ArtifactsDirectory = RootDirectory.Combine("artifacts");

        Log.Information($"{nameof(RootDirectory)} {RootDirectory}");
        Log.Information($"{nameof(SolutionFile)} {SolutionFile}");
        ProjectNames.ForEach(p => Log.Information($"{ProjectNameToPath(p).FullPath}"));
        Log.Information($"{nameof(TestsProjectName)} {ProjectNameToPath(TestsProjectName)}");
        Log.Information($"{nameof(ArtifactsDirectory)} {ArtifactsDirectory}");

        Log.Information($"{nameof(GitHubSecretToken)} {GitHubSecretToken?.Length ?? 0}");
        Log.Information($"{nameof(NugetApiKey)} {NugetApiKey?.Length ?? 0}");
    }

    public FilePath ProjectNameToPath(string projectName)
    {
        return RootDirectory.Combine(projectName).CombineWithFilePath($"{projectName}.csproj");
    }

    public T ArgumentOrEnvironment<T>(string name, T defaultValue = default)
        => this.HasArgument(name) ? this.Argument<T>(name) : this.EnvironmentVariable<T>(name, defaultValue);
}
