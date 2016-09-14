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

using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;

namespace CustomerOffers.Android
{
    [Activity(Label = "CustomerOffersActivity")]
    public class CustomerOffersActivity : Activity
    {
        // Create a new instance field for this activity.
        static CustomerOffersActivity instance = new CustomerOffersActivity();
        private static string[] senderIDs;
        private MobileServiceClient client;

        // Return the current activity instance.
        public static CustomerOffersActivity CurrentActivity
        {
            get
            {
                return instance;
            }
        }
        // Return the Mobile Services client.
        public MobileServiceClient CurrentClient
        {
            get
            {
                return client;
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            // Set the current instance of TodoActivity.
            instance = this;

            // Make sure the GCM client is set up correctly.
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register the app for push notifications.
            GcmClient.Register(this, CustomerOffersActivity.senderIDs);
        }
    }
}