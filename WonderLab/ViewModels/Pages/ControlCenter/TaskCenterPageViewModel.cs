using System.Collections.ObjectModel;
using WonderLab.Classes.Interfaces;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.ControlCenter;

public sealed class TaskCenterPageViewModel(TaskService taskService) : ViewModelBase
{
    public ObservableCollection<ITaskJob> TaskJobs => taskService.TaskJobs;
}