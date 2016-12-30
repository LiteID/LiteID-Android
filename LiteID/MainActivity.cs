using Android.App;
using Android.Widget;
using Android.OS;

namespace LiteID
{
    [Activity(Label = "LiteID", MainLauncher = true, Icon = "@drawable/icon4sc_256", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);
            Button randomize = FindViewById<Button>(Resource.Id.button1);
            Button clearlist = FindViewById<Button>(Resource.Id.button4);
            Button writeout = FindViewById<Button>(Resource.Id.button2);
            Button readin = FindViewById<Button>(Resource.Id.button3);

            DocumentList mainlist = new DocumentList();

            randomize.Click += delegate
            {
                mainlist.Randomize();
                UpdateList(mainlist);
            };

            clearlist.Click += delegate
            {
                mainlist.Documents.Clear();
                UpdateList(mainlist);
            };

            writeout.Click += delegate
            {
                mainlist.SaveList("documents.lxm");
                UpdateList(mainlist);
            };

            readin.Click += delegate
            {
                mainlist = new DocumentList("documents.lxm");
                UpdateList(mainlist);
            };
        }

        private void UpdateList(DocumentList list)
        {
            LinearLayout listview = FindViewById<LinearLayout>(Resource.Id.listView);
            listview.RemoveAllViews();

            foreach (Document doc in list.Documents)
            {
                TextView label = new TextView(this.ApplicationContext);
                label.Text = doc.Name;
                label.SetTextColor(Android.Graphics.Color.Black);
                listview.AddView(label);
            }
        }
    }
}

