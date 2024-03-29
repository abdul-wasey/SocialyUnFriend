﻿using Prism.Commands;
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
using SocialyUnFriend.LocalDB;
using System.Threading.Tasks;

namespace SocialyUnFriend.ViewModels
{

    public class GetVenuesListPageViewModel : BindableBase, INavigatedAware
    {
        private readonly IFourSquareService _fourSquareService;
        private readonly IGeoLocatorService _geoLocatorService;
        private readonly IPageDialogService _pageDialogService;
        private readonly INavigationService _navigationService;
        //private readonly IConnectivity _connectivity;
        private readonly IVenuesRepository _venuesRepository;
        private readonly ISqliteDb _sqliteDb;

        public GetVenuesListPageViewModel(IFourSquareService fourSquareService, IGeoLocatorService geoLocatorService,
                                          IPageDialogService pageDialogService, INavigationService navigationService,
                                          IVenuesRepository venuesRepository,
                                          ISqliteDb sqliteDb)
        {
            _fourSquareService = fourSquareService;
            _geoLocatorService = geoLocatorService;
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;
            // _connectivity = connectivity;
            _venuesRepository = venuesRepository;
            _sqliteDb = sqliteDb;

            //CheckInCommand = new DelegateCommand<string>(CheckInCommandExecuted);

            //_connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

        }


        //~GetVenuesListPageViewModel()
        //{
        //    //_connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        //}


        [DoNotNotify]
        public DelegateCommand<string> CheckInCommand { get; }

        public ObservableCollection<RecentPost> RecentPosts { get; set; }


        public bool IsRunning { get; set; }
        public bool IsBusy { get; set; }


        //public async void CheckInCommandExecuted(string venueId)
        //{
        //    try
        //    {
        //        IsRunning = true;

        //        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        //        {

        //        }

        //        var checkIn = await _fourSquareService.CreateCheckIn(
        //                                          Constants.FSCreateCheckInURL, Application.Current.Properties["acces_token"].ToString(),
        //                                          venueId, DateTime.Now.ToString("yyyyMMdd")
        //                                          );

        //        if (checkIn.IsSuccess)
        //        {
        //            var checkInAt = checkIn.ResultData.response.checkin.venue.name;
        //            var checkInId = checkIn.ResultData.response.checkin.id;

        //            if (!string.IsNullOrEmpty(checkInAt))
        //                await _pageDialogService.DisplayAlertAsync("Hurrah", $"You create a Check-in at {checkInAt}", "Ok");

        //            await _navigationService.NavigateAsync("LinkedInPostPage", new NavigationParameters { { "checkInId", checkInId } });
        //        }
        //        else
        //        {
        //            await _pageDialogService.DisplayAlertAsync("Error", checkIn.ErrorMessage, "Ok");
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        IsRunning = false;
        //    }


        //}

        //private async void GetNearestVenues()
        //{

        //    IsRunning = true;

        //    if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        //    {

        //        try
        //        {
        //            var venuesList = await _venuesRepository.GetVenuesAsync();

        //            if (venuesList.Count() > 0)
        //            {
        //                Venues = new ObservableCollection<Venue>(venuesList);
        //                await _pageDialogService.DisplayAlertAsync("Message", "Please Turn On your internet connection, to fetch latest Venues", "Ok");
        //            }
        //            else
        //                await _pageDialogService.DisplayAlertAsync("Message", "No record found", "Ok");

        //            return;
        //        }
        //        catch (Exception ex)
        //        {

        //            var properties = new Dictionary<string, string>
        //            {
        //                { "Category", "Venues" },
        //                { "Wifi", "On"}
        //            };

        //            Crashes.TrackError(ex, properties);
        //        }

        //        finally
        //        {
        //            IsRunning = false;
        //        }
        //    }

        //    try
        //    {
        //        await _geoLocatorService.GetLocationAsync();

        //        var venues = await _fourSquareService.GetVenueList(
        //                                        Constants.FSVenueSearchURL, Application.Current.Properties["acces_token"].ToString(),
        //                                        _geoLocatorService.Latitude.ToString(), _geoLocatorService.Longitude.ToString(),
        //                                        DateTime.Now.ToString("yyyyMMdd")
        //                                        );
        //        if (venues.IsSuccess)
        //        {
        //            var venueList = venues.ResultData.response.venues;

        //            if (venueList.Count > 0)
        //            {
        //                foreach (var venue in venueList)
        //                {

        //                    venue.City = venue.location.city;

        //                    foreach (var category in venue.categories)
        //                    {
        //                        venue.Image = category.icon.prefix + "88" + category.icon.suffix;
        //                    }


        //                }

        //                Venues = new ObservableCollection<Venue>(venueList);

        //                var localVenues = await _venuesRepository.GetVenuesAsync();

        //                if (localVenues.Count() > 0)
        //                    await _venuesRepository.DeleteAllVenuesAsync(localVenues);


        //                await _venuesRepository.AddAllVenuesAsync(venueList);

        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        var properties = new Dictionary<string, string>
        //            {
        //                { "Category", "Venues" },
        //                { "Wifi", "On"}
        //            };
        //        Crashes.TrackError(exception, properties);
        //    }
        //    finally
        //    {
        //        IsRunning = false;
        //    }
        //}


        private async Task GetRecentPosts()
        {
            try
            {
               
                RecentPosts = new ObservableCollection<RecentPost>
                {
                    new RecentPost
                    {
                         IsBusy = true,
                         Text = "ashashalsjla",
                         DateTime = DateTime.Now,
                         ImageUri = "sasasa",
                         Platform = SocialMediaPlatform.LinkedIn.ToString()
                    },
                    new RecentPost
                    {
                         IsBusy = true,
                         Text = "ashashalsjla",
                         DateTime = DateTime.Now,
                         ImageUri = "sasasa",
                         Platform = SocialMediaPlatform.LinkedIn.ToString()
                    },
                    new RecentPost
                    {
                         IsBusy = true,
                         Text = "ashashalsjla",
                         DateTime = DateTime.Now,
                         ImageUri = "sasasa",
                         Platform = SocialMediaPlatform.LinkedIn.ToString()
                    }

                };

                IsBusy = true;

                await Task.Delay(2500);

                var recentPosts = await _sqliteDb.GetAllDataAsync<RecentPost>();

                if (recentPosts.Count > 0)
                    RecentPosts = new ObservableCollection<RecentPost>(recentPosts);
                else
                {
                    RecentPosts = new ObservableCollection<RecentPost>
                {
                    new RecentPost
                    {
                         IsBusy = false,
                         DateTime = null
                    },
                    new RecentPost
                    {
                         IsBusy = false,
                         DateTime = null
                    },
                    new RecentPost
                    {
                         IsBusy = false,
                         DateTime = null
                    }

                };
                    await _pageDialogService.DisplayAlertAsync("Message", "No record found", "Ok");
                }
            }
            catch (Exception ex)
            {

                var properties = new Dictionary<string, string>
                    {
                        { "Category", "Venues" },
                        { "Offline", "On"}
                    };

                Crashes.TrackError(ex, properties);
            }

            finally
            {
                IsBusy = false;
            }

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await GetRecentPosts();
        }


        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            //    await _pageDialogService.DisplayAlertAsync("Connection Error", "Internet is not available.", "Ok");
            //else
            //    GetNearestVenues();
        }


    }
}
