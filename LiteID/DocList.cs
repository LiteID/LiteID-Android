using Android.App;
using Android.Widget;
using Android.OS;
using static Android.Widget.AdapterView;
using Android.Content;

namespace LiteID
{
    [Activity(Label = "LiteID", MainLauncher = true)]
    public class DocList : Activity
    {
        private DocListAdapter doclistAdapter;
        private LiteIDContext Context;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Context = new LiteIDContext();

            SetContentView (Resource.Layout.Main);
            ImageButton bOpt = FindViewById<ImageButton>(Resource.Id.buttonOptions);
            ImageButton bAdd = FindViewById<ImageButton>(Resource.Id.buttonAdd);

            ListView doclistView = FindViewById<ListView>(Resource.Id.docList);
            doclistAdapter = new DocListAdapter(this, Context.DocStore);
            doclistView.Adapter = doclistAdapter;
            doclistView.ItemClick += delegate (object sender, ItemClickEventArgs e)
            {
                Intent docView = new Intent(this, typeof(ViewDoc));
                docView.PutExtra("TargetID", Context.DocStore.Documents[e.Position].ID);
                StartActivity(docView);
            };
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
            Context.DocStore.LoadList(Context.DocStoreFile);
            Context.Config = LiteIDConfig.Load(Context.ConfigFile);
            doclistAdapter.NotifyDataSetChanged();
        }

        private void UpdateList()
        {
            Context.DocStore.SaveList(Context.DocStoreFile);
            doclistAdapter.NotifyDataSetChanged();
        }
    }
}

