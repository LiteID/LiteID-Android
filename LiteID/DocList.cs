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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);
            ImageButton bOpt = FindViewById<ImageButton>(Resource.Id.buttonOptions);
            ImageButton bAdd = FindViewById<ImageButton>(Resource.Id.buttonAdd);

            DocumentList mainlist = new DocumentList("documents.lxm");
            ListView doclistView = FindViewById<ListView>(Resource.Id.docList);
            doclistAdapter = new DocListAdapter(this, mainlist);
            doclistView.Adapter = doclistAdapter;
            UpdateList(mainlist);

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
            doclistAdapter.NotifyDataSetChanged();
        }
    }
}

