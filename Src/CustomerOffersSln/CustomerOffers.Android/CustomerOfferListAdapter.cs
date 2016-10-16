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

namespace CustomerOffers.Android
{
    class CustomerOfferListAdapter: ArrayAdapter<Offer>
    {
        private IList<Offer> Items { get; set; } = null;
        private Activity _context { get; set; } = null;
        private int? ResourceId { get; set; } = null;
        public CustomerOfferListAdapter(Activity context, int resourceId, IList<Offer> items):base(context,resourceId)
        {
            this.Items = items;
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
            var item = this.Items[position];
            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Android.Resource.Layout.CustomerOfferRow, null);
            }
            TextView txtOfferId = convertView.FindViewById<TextView>(Android.Resource.Id.txtOfferId);
            txtOfferId.Text = item.Id;
            return convertView;
        }
    }
}