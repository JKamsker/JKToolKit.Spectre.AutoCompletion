<p align="center">
  <a>
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/JKamsker/JKToolKit.Spectre.AutoCompletion/master/assets/logo/logo_small_128x128.png">
      <img src="https://raw.githubusercontent.com/JKamsker/JKToolKit.Spectre.AutoCompletion/master/assets/logo/logo_small_128x128.png" height="128">
    </picture>
    <h1 align="center">Spectre.Console AutoCompletion</h1>
  </a>
</p>

<!-- 
    Badges: Nuget, GitHub Actions, CodeFactor, License
 -->

[![NuGet](https://img.shields.io/nuget/v/JKToolKit.Spectre.AutoCompletion)](https://www.nuget.org/packages/JKToolKit.Spectre.AutoCompletion/) [![Nuget](https://img.shields.io/nuget/dt/JKToolKit.Spectre.AutoCompletion)](https://www.nuget.org/packages/JKToolKit.Spectre.AutoCompletion)
[![License](https://img.shields.io/github/license/JKamsker/JKToolKit.Spectre.AutoCompletion)](LICENSE) [![PR](https://img.shields.io/badge/PR-Welcome-blue)](https://github.com/JKamsker/JKToolKit.Spectre.AutoCompletion/pulls)
<!-- [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/JKamsker/JKToolKit.Spectre.AutoCompletion/.NET)]( -->
<!-- [![CodeFactor](https://www.codefactor.io/repository/github/jkamsker/jktoolkit.spectre.autocompletion/badge)](https://www.codefactor.io/repository/github/jkamsker/jktoolkit.spectre.autocompletion) -->


``JKToolKit.Spectre.AutoCompletion`` is an extension package for adding auto completion to your [Spectre.Console](https://github.com/spectreconsole/spectre.console) powered applications. </br>
It comes with suggestions for Options and Branches out of the box, but you can also add your own suggestions for option and argument values.

- [Shell integrations](#shell-integrations)
  - [PowerShell](#powershell)
- [How integrations get the suggestions](#how-integrations-get-the-suggestions)
- [Customizations](#customizations)
  - [Static Autocomplete](#static-autocomplete)
  - [Dynamic Autocomplete](#dynamic-autocomplete)


## Enabling Module

The extension can be enabled using the `AddAutoCompletion` method on the `Configurator` object.

```csharp
public static void Main(string[] args)
{
    var app = new CommandApp();
    app.Configure
    (
        config =>
        {
            config
                .AddAutoCompletion(x => x.AddPowershell())
                .AddCommand<LionCommand>("lion");
        }
    ) ;
}
```

## Shell integrations
1. [PowerShell](#powershell)
3. More to come...


### PowerShell

You can add autocomplete to PowerShell by running your application with the `completion powershell` command, as shown below:


```powershell
.\AutoCompletion.exe completion powershell | Out-String | Invoke-Expression
```

To add autocomplete to PowerShell permanently, use the `--install` flag:

```powershell
.\AutoCompletion.exe completion powershell --install | Out-String | Invoke-Expression
```

## How integrations get the suggestions

The shell integration uses the `completion complete` command to get the suggestions for the current command line like this:

```powershell
.\AutoCompletion.exe completion complete "AutoCompletion.exe Li"
```

## Customizations
1. [Static Autocomplete](#static-autocomplete) 
2. [Dynamic Autocomplete](#dynamic-autocomplete)

### Static Autocomplete

Spectre.Console auto completion allows you to specify static autocomplete suggestions for your command arguments and options. This can be done using the `CompletionSuggestions` attribute in your command settings class.

Here's an example of how to add static autocomplete suggestions:

```csharp
public class LionSettings : CommandSettings
{
    [CommandArgument(0, "<TEETH>")]
    [Description("The number of teeth the lion has.")]
    [CompletionSuggestions("10", "15", "20", "30")]
    public int Teeth { get; set; }

    [CommandOption("-a|--age <AGE>")]
    public int Age { get; set; }

    [CommandOption("-n|--name <NAME>")]
    public string Name { get; set; }
}
```

### Dynamic Autocomplete

In addition to static autocomplete suggestions, you can also provide dynamic autocomplete suggestions based on the user's input. This can be done by implementing the `IAsyncCommandCompletable` interface in your command class and overriding the `GetSuggestionsAsync` method.

Here's an example of how to add dynamic autocomplete suggestions:

```csharp
[Description("The lion command.")]
public class LionCommand : Command<LionSettings>, IAsyncCommandCompletable
{
    public override int Execute(CommandContext context, LionSettings settings)
    {
        return 0;
    }

    public async Task<CompletionResult> GetSuggestionsAsync(ICommandParameterInfo parameter, string? prefix)
    {
        if(string.IsNullOrEmpty(prefix))
        {
            return CompletionResult.None();
        }

        return await this.MatchAsync()
            .Add(x => x.Age, (prefix) =>
            {
                if (prefix.Length != 0)
                {
                    return FindNextEvenNumber(prefix);
                }

                return "16";
            })
            .Add(x => x.Name, prefix =>
            {
                var names = new List<string>
                {
                    "angel", "angelika", "robert",
                    "jennifer", "michael", "lucy",
                    "david", "sarah", "john", "katherine",
                    "mark"
                };

                var bestMatches = names
                    .Where(name => name.StartsWith(prefix))
                    .ToList();

                return new CompletionResult(bestMatches, bestMatches.Any());
            })
            .MatchAsync(parameter, prefix)
            .WithPreventDefault();
    }
}
```


There is a working [example of the AutoCompletion feature](src/samples/AutoCompletionExample/Program.cs) demonstrating this.

</br>
<p align="center">
Made with <span style="color: #e25555;">&hearts;</span> in Austria <img src="https://images.emojiterra.com/google/noto-emoji/v2.034/128px/1f1e6-1f1f9.png" width="20" height="20"/> 
</p>