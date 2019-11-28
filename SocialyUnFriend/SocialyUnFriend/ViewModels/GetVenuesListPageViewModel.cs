using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials;
using SocialyUnFriend.Repositories;
using Microsoft.AppCenter.Crashes;
using PropertyChanged;

namespace SocialyUnFriend.ViewModels
{
   
    public class GetVenuesListPageViewModel : BindableBase, INavigatedAware
    {
        private readonly IFourSquareService _fourSquareService;
        private readonly IGeoLocatorService _geoLocatorService;
        private readonly IPageDialogService _pageDialogService;
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IVenuesRepository _venuesRepository;

        public GetVenuesListPageViewModel(IFourSquareService fourSquareService, IGeoLocatorService geoLocatorService,
                                          IPageDialogService pageDialogService, INavigationService navigationService,
                                          IConnectivity connectivity, IVenuesRepository venuesRepository)
        {
            _fourSquareService = fourSquareService;
            _geoLocatorService = geoLocatorService;
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;
            _connectivity = connectivity;
            _venuesRepository = venuesRepository;



            CheckInCommand = new DelegateCommand<string>(CheckInCommandExecuted);

            _connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
           
        }


        ~GetVenuesListPageViewModel()
        {
            _connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }


        [DoNotNotify]
        public DelegateCommand<string> CheckInCommand { get; }

        public ObservableCollection<Venue> Venues { get; set; }
        public bool IsRunning { get; set; }


        public async void CheckInCommandExecuted(string venueId)
        {
            try
            {
                IsRunning = true;

                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {

                }

                var checkIn = await _fourSquareService.CreateCheckIn(
                                                  Constants.FSCreateCheckInURL, Application.Current.Properties["acces_token"].ToString(),
                                                  venueId, DateTime.Now.ToString("yyyyMMdd")
                                                  );

                if (checkIn.IsSuccess)
                {
                    var checkInAt = checkIn.ResultData.response.checkin.venue.name;
                    var checkInId = checkIn.ResultData.response.checkin.id;

                    if (!string.IsNullOrEmpty(checkInAt))
                        await _pageDialogService.DisplayAlertAsync("Hurrah", $"You create a Check-in at {checkInAt}", "Ok");

                    await _navigationService.NavigateAsync("LinkedInPostPage", new NavigationParameters { { "checkInId", checkInId } });
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", checkIn.ErrorMessage, "Ok");
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                IsRunning = false;
            }


        }

        private async void GetNearestVenues()
        {

            IsRunning = true;

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {

                try
                {
                    var venuesList = await _venuesRepository.GetVenuesAsync();

                    if (venuesList.Count() > 0)
                    {
                        Venues = new ObservableCollection<Venue>(venuesList);
                        await _pageDialogService.DisplayAlertAsync("Message", "Please Turn On your internet connection, to fetch latest Venues", "Ok");
                    }
                    else
                        await _pageDialogService.DisplayAlertAsync("Message", "No record found", "Ok");

                    return;
                }
                catch (Exception ex)
                {

                    var properties = new Dictionary<string, string>
                    {
                        { "Category", "Venues" },
                        { "Wifi", "On"}
                    };

                    Crashes.TrackError(ex, properties);
                }

                finally
                {
                    IsRunning = false;
                }
            }

            try
            {
                await _geoLocatorService.GetLocationAsync();

                var venues = await _fourSquareService.GetVenueList(
                                                Constants.FSVenueSearchURL, Application.Current.Properties["acces_token"].ToString(),
                                                _geoLocatorService.Latitude.ToString(), _geoLocatorService.Longitude.ToString(),
                                                DateTime.Now.ToString("yyyyMMdd")
                                                );
                if (venues.IsSuccess)
                {
                    var venueList = venues.ResultData.response.venues;

                    if (venueList.Count > 0)
                    {
                        foreach (var venue in venueList)
                        {

                            venue.City = venue.location.city;

                            foreach (var category in venue.categories)
                            {
                                venue.Image = category.icon.prefix + "88" + category.icon.suffix;
                            }


                        }

                        Venues = new ObservableCollection<Venue>(venueList);

                        var localVenues = await _venuesRepository.GetVenuesAsync();

                        if (localVenues.Count() > 0)
                            await _venuesRepository.DeleteAllVenuesAsync(localVenues);


                        await _venuesRepository.AddAllVenuesAsync(venueList);

                    }
                }
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string>
                    {
                        { "Category", "Venues" },
                        { "Wifi", "On"}
                    };
                Crashes.TrackError(exception, properties);
            }
            finally
            {
                IsRunning = false;
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            GetNearestVenues();
        }


        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                await _pageDialogService.DisplayAlertAsync("Connection Error", "Internet is not available.", "Ok");
            else
                GetNearestVenues();
        }

     
    }
}
