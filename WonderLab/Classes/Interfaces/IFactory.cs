namespace WonderLab.Classes.Interfaces;

public interface IFactory<out T> where T : class
{
    T Create();
}
