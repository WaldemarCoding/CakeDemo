var target = Argument("target", "Run");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var projectFolder = solutionFolder + "CakeDemo/CakeDemo";
var artifactsFolder = solutionFolder + "artifacts/";
var outputFolder = artifactsFolder + "full/";

// Demo of Projectlevel Build
var coreLibFolder = "./CakeDemo/Services/CakeDemo.Services";
var coreLibOutputfolder = artifactsFolder + "coreLib";

Task("Clean")
    .Does(() => {
        CleanDirectory(artifactsFolder);
        // CleanDirectory(outputFolder);
        // CleanDirectory(coreLibOutputfolder);
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionFolder);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(projectFolder, new DotNetCoreBuildSettings{
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true
        });
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projectFolder, new DotNetCorePublishSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = outputFolder
        });
    });

Task("PublishCoreLib")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(coreLibFolder, new DotNetCorePublishSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = coreLibOutputfolder
        });
    });

Task("Run")
    .IsDependentOn("Publish")
    .IsDependentOn("PublishCoreLib");

RunTarget(target);