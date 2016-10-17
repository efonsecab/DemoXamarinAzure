using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CustomerOffers.Android.Entities;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;

namespace CustomerOffers.Android
{
    [Activity(Label = "CustomerOffersActivity")]
    public class CustomerOffersActivity : Activity
    {
        // Create a new instance field for this activity.
        static CustomerOffersActivity instance = new CustomerOffersActivity();
        private static string[] senderIDs;
        private MobileServiceClient client = null;

        // Return the current activity instance.
        public static CustomerOffersActivity CurrentActivity
        {
            get
            {
                return instance;
            }
        }

        public class ZumoVersionHeaderHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("ZUMO-API-VERSION", "2.0.0");

                return base.SendAsync(request, cancellationToken);
            }
        }
        // Return the Mobile Services client.
        public MobileServiceClient CurrentClient
        {
            get
            {
                if (client == null)
                {
                    ZumoVersionHeaderHandler handler = new ZumoVersionHeaderHandler();
                    client = new MobileServiceClient("http://customeroffers.azurewebsites.net", string.Empty, handler );
                }
                return client;
            }
        }

        public IMobileServiceTable<Offer> OffersTable { get; set; }
        ListView lstOffers = null;
        CustomerOfferListAdapter adapter = null;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            base.OnCreate(savedInstanceState);

            // Create your application here
            // Set the current instance of TodoActivity.
            SetContentView(Android.Resource.Layout.CustomerOffers);
            instance = this;
            try
            {
                this.OffersTable = CurrentClient.GetTable<Entities.Offer>();
                var offersList = await this.OffersTable.ToListAsync();
                lstOffers = FindViewById<ListView>(Android.Resource.Id.lstOffers);
                ObservableCollection<Offer> data = new ObservableCollection<Offer>(offersList);
                data.CollectionChanged += Data_CollectionChanged;
                adapter = new CustomerOfferListAdapter(this, Android.Resource.Layout.CustomerOfferRow, data);
                lstOffers.Adapter = adapter;
                adapter.SetNotifyOnChange(true);
                adapter.NotifyDataSetChanged();
            }
            catch (Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException iEx)
            {
                var response = iEx.Response;
                var exceptionContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

            }
            ConfigurePushNotifications();
        }

        private void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() => 
            {
                adapter.Add(e.NewItems[0] as Offer);
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        private void ConfigurePushNotifications()
        {
            // Make sure the GCM client is set up correctly.
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);
            try
            {
                PushHandlerService.Initialize(this);
                // Register the app for push notifications.
                GcmClient.Register(this, CustomerOfferBroadcastReceiver.senderIDs);
            }
            catch (AggregateException aggreEx)
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}