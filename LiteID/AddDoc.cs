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
using Android.Util;

namespace LiteID
{
    [Activity(Label = "Add New Document")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] {
        Intent.CategoryDefault,
        Intent.CategoryBrowsable,
        Intent.CategoryAppGallery,
        Intent.CategoryOpenable,
    }, DataMimeTypes = new[] {
        "text/*",
        "image/*",
        "audio/*",
        "video/*",
        "application/*",
        "message/*"
    })]
    public class AddDoc : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddDoc);

            Log.Info("intent", Intent.HasExtra(Intent.ExtraText) ? Intent.GetStringExtra(Intent.ExtraText) : "fuck you");

        }
    }
}