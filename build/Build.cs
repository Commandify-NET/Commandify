using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions("dev", GitHubActionsImage.Ubuntu2204, AutoGenerate = true, OnPushBranches = new[] { "dev", "feature/**" }, InvokedTargets = new [] { nameof(Compile) })]
[GitHubActions("main", GitHubActionsImage.Ubuntu2204, AutoGenerate = true, OnPushBranches = new[] { "main" }, PublishArtifacts = true, InvokedTargets = new [] { nameof(Publish) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    
    public AbsolutePath SourcesDirectory => RootDirectory / "src";
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourcesDirectory.GlobDirectories("**/bin", "**/obj").ForEach(_ => _.DeleteDirectory());
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _.SetProjectFile(Solution));
        });
    
    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            
        });
    
    Target Publish => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            
        });
}