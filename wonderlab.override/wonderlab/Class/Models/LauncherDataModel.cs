using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    /// <summary>
    /// 启动器设置数据模型
    /// </summary>
    public class LauncherDataModel {
        public List<UserModel> Users { get; set; }

        public UserModel CurrentUser { get; set; }
    }
}
