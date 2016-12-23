using Android.App;
using Android.Widget;
using Android.OS;

namespace LiteID
{
    [Activity(Label = "LiteID", MainLauncher = true, Icon = "@drawable/icon4sc_256")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);
        }
    }
}

