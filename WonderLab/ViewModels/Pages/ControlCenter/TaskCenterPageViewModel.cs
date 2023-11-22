using WonderLab.Classes.Managers;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.ViewModels.Pages.ControlCenter {
    public class TaskCenterPageViewModel : ViewModelBase {
        private TaskManager _taskManager {  get; set; }

        public ObservableCollection<ITaskJob> TaskJobs => _taskManager.TaskJobs;

        public TaskCenterPageViewModel(TaskManager manager) {
            _taskManager = manager;
        }
    }
}
