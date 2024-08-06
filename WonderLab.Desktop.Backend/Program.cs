using System.CommandLine;
using System.CommandLine.Invocation;

namespace WonderLab.Desktop.Backend;

public class Program {
    public static Task Main(string[] args) {
        return Build().InvokeAsync(args);
    }

    private static RootCommand Build() {
        var idOption = new Option<string>("-id", "game id.");
        var threadOption = new Option<int>("-thread", "Maximum number of threads.");
        var pathOption = new Option<string>("-path", "The path to the game directory.");
        var sourceOption = new Option<string>("-source", "Default download source for resources.");

        var root = new RootCommand("Backend time-consuming event handler.");

        root.AddGlobalOption(idOption);
        root.AddGlobalOption(pathOption);
        root.AddGlobalOption(threadOption);
        root.AddGlobalOption(sourceOption);

        var subCommand = new Command("--install", "install games.");
        subCommand.SetHandler(() => {
            Console.WriteLine("Subcommand 1!");
        });

        var completion = new Command("--completion", "Complete game assets and libraries files.");
        completion.SetHandler(async (id, path, source, thread) => {
            await ResourceDownloader.CompleteResourceAsync(path, id, source, thread);
        },
        idOption, pathOption, sourceOption, threadOption);

        root.Add(subCommand);
        root.Add(completion);
        return root;
    }
}