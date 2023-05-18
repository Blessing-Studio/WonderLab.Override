using System.Reflection;
using wonderlab.PluginLoader.Interfaces;
using EventHandler = wonderlab.PluginLoader.Attributes.EventHandler;

namespace wonderlab.PluginLoader.Events
{
    /// <summary>
    /// 事件基类
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// 所有插件监听器
        /// </summary>
        public static List<IListener> Listeners = new List<IListener>();
        public abstract string Name { get; }
        public abstract bool Do();
        public string GetEventName()
        {
            return Name;
        }
        /// <summary>
        /// 注册监听器
        /// </summary>
        /// <param name="listener">
        /// 监听器
        /// </param>
        public static void RegListener(IListener listener)
        {
            Listeners.Add(listener);
        }
        /// <summary>
        /// 发送全局事件
        /// </summary>
        /// <param name="event">
        /// 事件
        /// </param>
        public static bool CallEvent(Event @event)
        {
            bool cancel = false;
            for (int i = 0; i < Listeners.Count; i++)
            {
                Listeners[i].GetEvent(@event);
                if (@event is ICancellable)
                {
                    cancel = cancel || ((ICancellable)@event).IsCanceled;
                }
                MethodInfo[] methods = Listeners[i].GetType().GetMethods();
                foreach (MethodInfo method in methods)
                {
                    bool IsEventMethod = false;
                    if (IsEventMethod && method.GetParameters().Length == 1)
                    {
                        Type? baseType = method.GetParameters()[0].ParameterType.BaseType;
                        if (baseType != null)
                        {
                            while (baseType.BaseType != null && baseType != typeof(Event))
                            {
                                baseType = baseType.BaseType;
                            }
                        }
                        if (!(method.GetCustomAttribute<EventHandler>() != null) && baseType == typeof(Event) && method.GetParameters()[0].ParameterType == @event.GetType())
                        {
                            method.Invoke(Listeners[i], new object[] { @event });
                            if (@event is ICancellable)
                            {
                                cancel = cancel || ((ICancellable)@event).IsCanceled;
                            }
                        }
                    }
                }
            }
            return @event.Do();
        }
    }
}
