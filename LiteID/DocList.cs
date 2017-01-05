using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;

namespace LiteID
{
    [Activity(Label = "LiteID", MainLauncher = true)]
    public class DocList : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);
            Button randomize = FindViewById<Button>(Resource.Id.button1);
            Button clearlist = FindViewById<Button>(Resource.Id.button4);
            ImageButton bOpt = FindViewById<ImageButton>(Resource.Id.buttonOptions);
            ImageButton bAdd = FindViewById<ImageButton>(Resource.Id.buttonAdd);

            DocumentList mainlist = new DocumentList("documents.lxm");
            UpdateList(mainlist);

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

            bOpt.Click += delegate
            {
                StartActivity(typeof(Options));
            };

            bAdd.Click += delegate
            {
                StartActivity(typeof(AddDoc));
            };
        }

        private void UpdateList(DocumentList list)
        {
            list.SaveList("documents.lxm");

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

