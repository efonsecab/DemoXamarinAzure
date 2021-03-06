﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using CustomerOffersMobileApp.DataObjects;
using CustomerOffersMobileApp.Models;
using System;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;

namespace CustomerOffersMobileApp.Controllers
{
    public class OfferController : TableController<Offer>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Offer>(context, Request);
        }

        // GET tables/Offer
        public IQueryable<Offer> GetAllOffer()
        {
            return Query();
        }

        // GET tables/Offer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Offer> GetOffer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Offer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Offer> PatchOffer(string id, Delta<Offer> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Offer
        public async Task<IHttpActionResult> PostOffer(Offer item)
        {
            try
            {
                item.Id = Guid.NewGuid().ToString();
                item.CreatedAt = DateTimeOffset.UtcNow;
                Offer current = await InsertAsync(item);
                await SendAndroidNotification(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                return StatusCode(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task SendAndroidNotification(Offer item)
        {
            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            try
            {
                // Send the push notification and log the results.
                //var result = await hub.SendTemplateNotificationAsync(templateParams);
                CustomerOffers.SharedEntities.NotificationHubMessage<Offer>
                    objPayload = new CustomerOffers.SharedEntities.NotificationHubMessage<Offer>()
                    {
                        Data = new CustomerOffers.SharedEntities.NotificationHubMessageData<Offer>()
                        {
                            Message = string.Format("New Offer: {0}", item.OfferText),
                            EntiyData = item
                        }
                    };
                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(objPayload);
                templateParams["data"] = jsonPayload;
                var result = await hub.SendGcmNativeNotificationAsync(jsonPayload);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }
        }

        // DELETE tables/Offer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteOffer(string id)
        {
            return DeleteAsync(id);
        }
    }
}
