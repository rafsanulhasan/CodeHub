﻿using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewmodel ViewModel { get; set; }
        public Frame AppFrame { get { return this.mainFrame; } }
        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainViewmodel();
            this.DataContext = ViewModel;

            Loaded += Page_Loaded;
           
            //SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(this).Compositor);

            Messenger.Default.Register<NoInternetMessageType>(this, ViewModel.RecieveNoInternetMessage); //Listening for No Internet message
            Messenger.Default.Register<HasInternetMessageType>(this, ViewModel.RecieveInternetMessage); //Listening Internet available message
           
            Messenger.Default.Register(this, delegate(SetHeaderTextMessageType m)
            {
                ViewModel.setHeadertext(m.PageName);
            });  //Setting Header Text to the current page name

            SimpleIoc.Default.Register<INavigationService>(() =>
            { return new NavigationService(mainFrame); });

            SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(HomeView));
            NavigationCacheMode = NavigationCacheMode.Enabled;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            bool handled = e.Handled;
            this.BackRequested(ref handled);
            e.Handled = handled;
        }
        private void HamButton_Click(object sender, RoutedEventArgs e)
        {
            HamSplitView.IsPaneOpen = !HamSplitView.IsPaneOpen;
        }
        private void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.HamItemClicked(e.ClickedItem as HamItem);
            HamSplitView.IsPaneOpen = false;
        }
        private void SettingsItem_ItemClick(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.NavigateToSettings();
            HamSplitView.IsPaneOpen = false;
        }
        private void UpdateVisualState(VisualState currentState)
        {
            ViewModel.CurrentState = currentState;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualState(VisualStateGroup.CurrentState);
        }
        private void BackRequested(ref bool handled)
        {
            if (this.AppFrame == null)
                return;

            if (this.AppFrame.CanGoBack && !handled)
            {
                handled = true;
                this.AppFrame.GoBack();
            }
        }
        private void AppBarTrending_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.HamItemClicked(ViewModel.HamItems[0]);
        }
        private void AppBarProfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.HamItemClicked(ViewModel.HamItems[1]);
        }
        private void AppBarMyRepos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.HamItemClicked(ViewModel.HamItems[2]);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.isLoggedin = (bool)e.Parameter;
        }
    }
}
