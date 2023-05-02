using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using wonderlab.control.Controls.Bar;
using wonderlab.control.Theme;

namespace wonderlab.control
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ColorHelper helper = new ColorHelper();
            helper.Load();
            filp.Source.Add(new Bitmap("C:\\Users\\w\\Desktop\\VIF1COK36DW43[TC@F[((V8.png"));
            filp.Source.Add(new Bitmap("C:\\Users\\w\\Desktop\\91523683_p8_master1200.jpg"));
            filp.Source.Add(new Bitmap("C:\\Users\\w\\Desktop\\QQ20230402133547.jpg"));
        }
    }
}
