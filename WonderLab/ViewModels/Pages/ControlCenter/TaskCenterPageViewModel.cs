using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Managers;

namespace WonderLab.ViewModels.Pages.ControlCenter {
    public class TaskCenterPageViewModel : ViewModelBase {
        [Reactive]
        public TaskManager _taskManager {  get; set; }

        public ObservableCollection<ITaskJob> TaskJobs => _taskManager.TaskJobs;

        public TaskCenterPageViewModel(TaskManager manager) {
            _taskManager = manager;
        }
    }
}
