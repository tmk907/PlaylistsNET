using Cake.Frosting;

[TaskName("Default")]
[IsDependentOn(typeof(BuildTask))]
public sealed class DefaultTarget : FrostingTask<BuildContext> { }

[TaskName("Publish")]
[IsDependentOn(typeof(PublishGithubTask))]
[IsDependentOn(typeof(PublishNugetTask))]
[IsDependentOn(typeof(PublishLocalTask))]
public sealed class PublishTarget : FrostingTask<BuildContext> { }
