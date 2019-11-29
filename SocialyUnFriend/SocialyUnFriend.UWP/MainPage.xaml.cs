using CarouselView.FormsPlugin.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SocialyUnFriend.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            List<Assembly> assembliesToInclude = new List<Assembly>
            {
                typeof(CarouselViewRenderer).GetTypeInfo().Assembly
            };

            LoadApplication(new SocialyUnFriend.App());
        }
    }
}
