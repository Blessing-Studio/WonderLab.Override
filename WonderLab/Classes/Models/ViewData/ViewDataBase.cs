using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.Classes.Models.ViewData;

public partial class ViewDataBase<T> : ObservableObject
{
    [ObservableProperty]
    public T data;

    public ViewDataBase(T data) : base()
    {
        this.Data = data;
    }
}
