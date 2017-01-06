using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;

namespace LiteID
{
    [Activity(Label = "LiteID", MainLauncher = true)]
    public class DocList : Activity
    {
        private DocListAdapter doclistAdapter;
        private DocumentList docList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);
            ImageButton bOpt = FindViewById<ImageButton>(Resource.Id.buttonOptions);
            ImageButton bAdd = FindViewById<ImageButton>(Resource.Id.buttonAdd);

            docList = new DocumentList("documents.lxm");
            ListView doclistView = FindViewById<ListView>(Resource.Id.docList);
            doclistAdapter = new DocListAdapter(this, docList);
            doclistView.Adapter = doclistAdapter;
            UpdateList();

            bOpt.Click += delegate
            {
                StartActivity(typeof(Options));
            };

            bAdd.Click += delegate
            {
                StartActivity(typeof(AddDoc));
            };
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            docList.LoadList("documents.lxm");
            doclistAdapter.NotifyDataSetChanged();
        }

        private void UpdateList()
        {
            docList.SaveList("documents.lxm");
            doclistAdapter.NotifyDataSetChanged();
        }
    }
}

