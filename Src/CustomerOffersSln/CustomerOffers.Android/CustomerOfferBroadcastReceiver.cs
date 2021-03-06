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
using Newtonsoft.Json.Linq;
using WindowsAzure.Messaging;
using CustomerOffers.Android.Entities;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: Permission(Name = "@PACKAGE_NAME@.android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.android.permission.WAKE_LOCK")]
[assembly: Permission(Name = "@PACKAGE_NAME@.android.permission.WAKE_LOCK")]

//GET_ACCOUNTS is needed only for Android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]


namespace CustomerOffers.Android
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { "com.google.android.gcm.intent.RECEIVE" },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_TO_GCM_REGISTRATION },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_TO_GCM_UNREGISTRATION },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { "@PACKAGE_NAME@.com.google.firebase.INSTANCE_ID_EVENT" },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    public class CustomerOfferBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        // Set the Google app ID.
        public static string[] senderIDs = new string[] { "133298586621" };
    }

    // The ServiceAttribute must be applied to the class.
    [Service]
    public class PushHandlerService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }
        static NotificationHub hub;
        private static Context _context = null;

        public static void Initialize(Context context)
        {
            // Call this from our main activity
            var cs = ConnectionString.CreateUsingSharedAccessKeyWithListenAccess(
                new Java.Net.URI("https://CustomerOffersNS.servicebus.windows.net:443/"),
                "Endpoint=sb://customeroffersns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=/2qsB2EVJ7b66NNN4LoaL4AWV0MA002JQr0zKxY/kWU=");

            var hubName = "customeroffershub";

            hub = new NotificationHub(hubName, cs, context);
            _context = context;
        }

        public PushHandlerService() : base(CustomerOfferBroadcastReceiver.senderIDs) { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            System.Diagnostics.Debug.WriteLine("The device has been registered with GCM.", "Success!");
            if (hub != null)
                hub.Register(registrationId);
            // Get the MobileServiceClient from the current activity instance.
            MobileServiceClient client = CustomerOffersActivity.CurrentActivity.CurrentClient;
            var push = client.GetPush();
            // Define a message body for GCM.
            const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

            // Define the template registration as JSON.
            JObject templates = new JObject();
            templates["genericMessage"] = new JObject
    {
      {"body", templateBodyGCM }
    };

            try
            {
                // Make sure we run the registration on the same thread as the activity, 
                // to avoid threading errors.
                CustomerOffersActivity.CurrentActivity.RunOnUiThread(

                    // Register the template with Notification Hubs.
                    async () =>
                    {
                        try
                        {
                            //await push.RegisterTemplateAsync(registrationId, templates.ToString(), "testtemplate");
                        }
                        catch (Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException ex)
                        {
                            string exContent = await ex.Response.Content.ReadAsStringAsync();
                        }
                    });

                //System.Diagnostics.Debug.WriteLine(
                //    string.Format("Push Installation Id", push.InstallationId.ToString()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error with Azure push registration: {0}", ex.Message));
            }
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            string message = string.Empty;

            var keys = intent.Extras.KeySet();
            string notificationParamName = "message";
            // Extract the push notification message from the intent.
            if (intent.Extras.ContainsKey(notificationParamName))
            {
                message = intent.Extras.Get(notificationParamName).ToString();
                var title = "New item added:";
                string entityDataKEy = "entitydata";
                if (intent.Extras.ContainsKey(entityDataKEy))
                {
                    string entityJson = intent.Extras.Get(entityDataKEy).ToString();
                    Offer entity = 
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Offer>(entityJson);
                    CustomerOfferListAdapter.Items.Add(entity);
                }

                // Create a notification manager to send the notification.
                var notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Create a new intent to show the notification in the UI. 
                PendingIntent contentIntent =
                    PendingIntent.GetActivity(context, 0,
                    new Intent(this, typeof(CustomerOffersActivity)), 0);

                // Create the notification using the builder.
                var builder = new Notification.Builder(context);
                //builder.SetAutoCancel(true);
                builder.SetContentTitle(title);
                builder.SetContentText(message);
                builder.SetSmallIcon(Resource.Drawable.Icon);
                builder.SetContentIntent(contentIntent);
                builder.SetShowWhen(true);
                var notification = builder.Build();

                // Display the notification in the Notifications Area.
                notificationManager.Notify(1, notification);

            }
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            if (hub != null)
                hub.Unregister();
        }

        protected override void OnError(Context context, string errorId)
        {
            System.Diagnostics.Debug.WriteLine(
                string.Format("Error occurred in the notification: {0}.", errorId));
        }
    }
}