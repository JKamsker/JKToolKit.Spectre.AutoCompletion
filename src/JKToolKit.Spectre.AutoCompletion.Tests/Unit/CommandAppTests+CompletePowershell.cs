using Spectre.Console.Cli;
using Spectre.Console.Cli.Tests.Data.Commands;
using Spectre.Console.Testing;
using Spectre.Console.Tests.Data;

namespace Spectre.Console.Tests.Unit.Cli;

public sealed partial class CommandAppTests
{
    [UsesVerify]
    [ExpectationPath("Cli/Complete/Powershell")]
    public sealed class CompletePowershellIntegration
    {
        [Fact]
        public void TestParseStartArgs_WithDllCommand_ShouldSetDotnetRuntime()
        {
            var startArgs = StartArgs.ParseStartArgs("myapp.dll");
            Assert.Equal("dotnet", startArgs.Runtime);
            Assert.Equal("myapp.dll", startArgs.Command);
        }

        [Fact]
        public void TestParseStartArgs_WithDotnetCommand_ShouldSetCommandToSecondArg()
        {
            var startArgs = StartArgs.ParseStartArgs("dotnet", "myapp.dll");
            Assert.Equal("dotnet", startArgs.Runtime);
            Assert.Equal("myapp.dll", startArgs.Command);
        }

        [Fact]
        public void TestParseStartArgs_WithNonDotnetCommand_ShouldSetCommandToFirstArg()
        {
            var startArgs = StartArgs.ParseStartArgs("myapp.exe");
            Assert.Equal(string.Empty, startArgs.Runtime);
            Assert.Equal("myapp.exe", startArgs.Command);
        }


        // Assert that the integration builder does not throw for now

        [Fact]
        public void TestBuildIntegration_WithNullStartArgs_ShouldUseSelfStartCommand()
        {
            var settings = new PowershellIntegrationBuilderSettings { Install = false, StartArgs = null };
            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
        }

        [Fact]
        public void TestBuildIntegration_WithStartArgs_ShouldUseProvidedStartArgs()
        {
            var startArgs = new StartArgs("dotnet", "myapp.dll");
            var settings = new PowershellIntegrationBuilderSettings { Install = false, StartArgs = startArgs };
            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
        }

    }
}
