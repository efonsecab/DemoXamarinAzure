using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerOffersMobileApp.DataObjects
{
    public class Offer : EntityData
    {
        public string OfferText { get; set; }
        public DateTime OfferStartDate { get; set; }
        public DateTime OfferEndDate { get; set; }
    }
}