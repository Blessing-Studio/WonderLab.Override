using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.Class.ViewData
{
    public class ModLoaderViewData : ViewDataBase<ModLoaderModel> { 
        public ModLoaderViewData(ModLoaderModel data) : base(data) {     
            Id = data.Id;

            try {
                Icon = (Bitmap) BitmapUtils.GetIconBitmap($"{data.ModLoader}.png");
            }
            catch (Exception) {
            }
        }

        [Reactive]
        public Bitmap Icon { get; set; }

        [Reactive]
        public string Type { get; set; }

        [Reactive]
        public string Id { get; set; }
    }
}

