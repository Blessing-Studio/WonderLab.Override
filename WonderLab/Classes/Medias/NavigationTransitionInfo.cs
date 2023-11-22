using Avalonia.Animation;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Medias {
    public abstract class NavigationTransitionInfo : AvaloniaObject {
        public abstract void RunAnimation(Animatable ctrl, CancellationToken cancellationToken);
    }
}
