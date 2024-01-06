using WonderLab.Services;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.ViewModels.Pages.ControlCenter;

public class TaskCenterPageViewModel(TaskService taskService) : ViewModelBase {
    public ObservableCollection<ITaskJob> TaskJobs => taskService.TaskJobs;
}