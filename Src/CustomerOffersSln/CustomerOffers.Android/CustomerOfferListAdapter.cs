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
using Com.Syncfusion.Rating;
using Java.Lang;
using System.Collections.ObjectModel;

namespace CustomerOffers.Android
{
    class CustomerOfferListAdapter : ArrayAdapter<Offer>
    {
        internal static ObservableCollection<Offer> Items { get; set; } = null;
        private Activity _context { get; set; } = null;
        private int? ResourceId { get; set; } = null;
        public CustomerOfferListAdapter(Activity context, int resourceId, ObservableCollection<Offer> items) : base(context, resourceId)
        {
            Items = items;
            this.ResourceId = resourceId;
            _context = context;
        }

        public override int Count
        {
            get
            {
                return Items.Count();
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = Items[position];
            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Android.Resource.Layout.CustomerOfferRow, null);
            }
            TextView txtOfferId = convertView.FindViewById<TextView>(Android.Resource.Id.txtOfferId);
            txtOfferId.Text = item.Id;
            TextView txtOfferText = convertView.FindViewById<TextView>(Android.Resource.Id.txtOfferText);
            txtOfferText.Text = item.OfferText;
            TextView txtOfferStartDate = convertView.FindViewById<TextView>(Android.Resource.Id.txtOfferStartDate);
            txtOfferStartDate.Text = item.OfferStartDate.ToString();
            return convertView;
        }
    }

    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }
}