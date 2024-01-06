using WonderLab.Services;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.ViewModels.Pages.ControlCenter {
    public class TaskCenterPageViewModel : ViewModelBase {
        private TaskService _taskService;

        public ObservableCollection<ITaskJob> TaskJobs => _taskService.TaskJobs;

        public TaskCenterPageViewModel(TaskService taskService) {
            _taskService = taskService;
        }
    }
}
