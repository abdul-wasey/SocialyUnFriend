using SocialyUnFriend.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PropertyChanged;
using Prism.Mvvm;

namespace SocialyUnFriend.Model
{
   
    public class ImageButtonItem
    {
       
        public string ImageSource { get; set; }
        
        public string Text { get; set; }

        
        public Color Color { get; set; }

        public bool IsEnabled { get; set; }

      
        public SocialMediaPlatform Platform { get; set; }

    }
}
