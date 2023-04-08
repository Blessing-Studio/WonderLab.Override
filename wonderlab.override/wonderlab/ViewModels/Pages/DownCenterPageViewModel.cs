using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;

namespace wonderlab.ViewModels.Pages
{
    public class DownCenterPageViewModel : ReactiveObject {
        public DownCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       
            
        }

        public CurseForgeToolkit Toolkit { get; } = new("$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6");

        [Reactive]
        public ObservableCollection<WebModpackViewData> Modpacks { get; set; } = new();

        [Reactive]
        public bool IsLoading { get; set; }

        public async ValueTask GetModrinthModpackAsync() {
            Modpacks.Clear();

            var modpacks = await Task.Run(async () => await ModrinthToolkit.GetFeaturedModpacksAsync());
            foreach (var i in modpacks.Hits.AsParallel()) {
                await Task.Run(async () => {
                    var infos = await ModrinthToolkit.GetProjectInfos(i.ProjectId);
                    Modpacks.Add(new WebModpackModel(i, infos).CreateViewData<WebModpackModel, WebModpackViewData>());
                });
            }

            IsLoading = false;
        }

        public async ValueTask GetCurseforgeModpacksAsync() {
            Modpacks.Clear();

            var modpacks = await Task.Run(async () => await Toolkit.GetFeaturedModpacksAsync());
            foreach (var x in modpacks) {
                Modpacks.Add(new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>());
                await Task.Delay(10);
            }

            IsLoading = false;
        }

        public void OpenGameInstallDialogAction() {
            MainWindow.Instance.Install.InstallDialog.ShowDialog();
        }

        public async void GetModrinthModpackAction() {
            IsLoading = true;
            await GetModrinthModpackAsync();
        }

        public async void GetCurseforgeModpackAction() {       
            IsLoading = true;
            await GetCurseforgeModpacksAsync();
        }
    }
}
