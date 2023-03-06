using Avalonia.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Interface
{
    /// <summary>
    /// 动画类基础接口
    /// </summary>
    public interface IAnimation
    {
        /// <summary>
        /// 是否反转动画
        /// </summary>
        bool IsReversed { get; set; }

        /// <summary>
        /// 运行动画！
        /// </summary>
        /// <param name="ctrl"></param>
        void RunAnimation(Animatable ctrl);

        /// <summary>
        /// 动画完成事件
        /// </summary>
        event EventHandler<EventArgs> AnimationCompleted;
    }
}
