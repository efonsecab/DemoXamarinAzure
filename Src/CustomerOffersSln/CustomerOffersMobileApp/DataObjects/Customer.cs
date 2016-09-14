using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerOffersMobileApp.DataObjects
{
    public class Customer : EntityData
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}