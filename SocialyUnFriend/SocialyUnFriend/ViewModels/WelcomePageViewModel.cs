using Prism.Commands;
using Prism.Mvvm;
using SocialyUnFriend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
using Prism.Navigation;
using Xamarin.Forms;
using SocialyUnFriend.Common;

namespace SocialyUnFriend.ViewModels
{
    public class WelcomePageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public WelcomePageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            WelcomePages = new ObservableCollection<WelcomeModel>(GetListOfViews());

            GetStartedCommand = new DelegateCommand(GetStartedCommandExecuted);
           
        }

        [DoNotNotify]
        public DelegateCommand GetStartedCommand { get;}
        [DoNotNotify]
        public ObservableCollection<WelcomeModel> WelcomePages { get; set; }

        public async void GetStartedCommandExecuted()
        {

            await _navigationService.NavigateAsync("/NavigationPage/LoginPage");  //reseting the navigation stack

            Application.Current.Properties[Constants.IsWelcomePageVisible] = true;
            await Application.Current.SavePropertiesAsync();
        }

        private List<WelcomeModel> GetListOfViews()
        {

            return new List<WelcomeModel>
            {
                 new WelcomeModel
                 {
                     Image = "announce_icon.png",
                     Title = "Welcome!",
                     Description= "Welcome! A new and improved Social Post App is a few clicks away. Swipe and learn more!"
                 },
                 new WelcomeModel
                 {
                     Image = "linkedin_white_icon.jpg",
                     Title = "Publish like a Pro!",
                     Description= "Publish content like a Pro to Linkedin, Foursquare, Facebook, Instagaram and much more!"
                 },
                 new WelcomeModel
                 {
                     Image = "foursquare_icon.png",
                     Title = "Register Yourself!",
                     Description= "Please register to your desired social media platforms and enjoy the features!"
                 }
            };
        }
    }
}
