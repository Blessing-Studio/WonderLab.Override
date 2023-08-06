using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Interface
{
    /// <summary>
    /// 对话框通用基础接口
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// 打开对话框
        /// </summary>
        /// <returns></returns>
        void ShowDialog();

        /// <summary>
        /// 关闭对话框
        /// </summary>
        void HideDialog();
    }
}
