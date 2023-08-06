using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Interface
{
    /// <summary>
    /// 标准通用信息对话框接口
    /// </summary>
    public interface IMessageDialog : IDialog
    {
        /// <summary>
        /// 标题
        /// </summary>
        string? Title { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        string? Message { get; set; }
    }
}
