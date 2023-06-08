using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;

namespace wonderlab.ViewModels.Pages {
    public class ModConfigPageViewModel : ViewModelBase {
        public ModConfigPageViewModel(GameCore core) {
            PropertyChanged += OnPropertyChanged;

            Current = core;
        }

        private async void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Current)) {
                Toolkit = new(Current, true);
                var result = await Task.Run(async () => await Toolkit.LoadAllAsync());

                if (result.Any()) {
                    await Task.Run(() => ModPacks = result.Select(x => x.CreateViewData<ModPack, ModPackViewData>()).ToObservableCollection());
                } else HasModPack = 1;

                Trace.WriteLine($"[信息] 共有 {ModPacks.Count} 个模组");
            }
        }

        [Reactive]
        public GameCore Current { get; set; }

        [Reactive]
        public int HasModPack { get; set; } = 0;

        [Reactive]
        public ObservableCollection<ModPackViewData> ModPacks { get; set; } = new();

        public static ModPackToolkit Toolkit { get; set; }
    }
}
