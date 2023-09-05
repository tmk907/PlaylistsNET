using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.DotNet;
using Cake.Frosting;

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var testSettings = new DotNetTestSettings
        {
            Configuration = context.MsBuildConfiguration,
            NoBuild = true,
            NoRestore = true,
            NoLogo = true
        };
        context.DotNetTest(context.ProjectNameToPath(context.TestsProjectName).FullPath, testSettings);
    }
}
