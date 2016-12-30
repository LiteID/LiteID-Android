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

namespace LiteID
{
    [Activity(Label = "LiteID Options", Icon = "@drawable/icon4sc_256", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class Options : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Options);
        }
    }
}