using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string Target { get; set; }
    public string BuildConfiguration { get; set; }
    public string SolutionFolder { get; set; }
    public string ProjectFolder { get; set; }
    public string ArtifactsFolder { get; set; }
    public string OutputFolder { get; set; }

    // Demo of Project level Build
    public string CoreLibFolder { get; set; }
    public string CoreLibOutputFolder { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Run");
        BuildConfiguration = context.Argument("BuildConfiguration", "Release");
        SolutionFolder = "../";
        ProjectFolder = SolutionFolder + "CakeDemo/CakeDemo";
        ArtifactsFolder = SolutionFolder + "artifacts/";
        OutputFolder = ArtifactsFolder + "full/";

        CoreLibFolder = SolutionFolder + "CakeDemo/Services/CakeDemo.Services";
        CoreLibOutputFolder = ArtifactsFolder + "coreLib";
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.ArtifactsFolder);
    }
}

[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCoreRestore(context.SolutionFolder);
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCoreBuild(context.ProjectFolder, new DotNetCoreBuildSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration
        });
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCoreTest(context.SolutionFolder, new DotNetCoreTestSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration,
            NoBuild = true
        });
    }
}

[TaskName("Publish")]
[IsDependentOn(typeof(TestTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCorePublish(context.ProjectFolder, new DotNetCorePublishSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            OutputDirectory = context.OutputFolder
        });
    }
}

[TaskName("PublishCoreLib")]
[IsDependentOn(typeof(TestTask))]
public sealed class PublishCoreLibTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCorePublish(context.CoreLibFolder, new DotNetCorePublishSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            OutputDirectory = context.CoreLibOutputFolder
        });
    }
}

[IsDependentOn(typeof(PublishTask))]
[IsDependentOn(typeof(PublishCoreLibTask))]
public sealed class Default : FrostingTask
{
}