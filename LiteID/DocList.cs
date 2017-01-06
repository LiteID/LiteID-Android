﻿using Android.App;
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
            doclistView.ItemClick += delegate (object sender, ItemClickEventArgs e)
            {
                Intent docView = new Intent(this, typeof(DocView));
                docView.PutExtra("TargetID", docList.Documents[e.Position].ID);
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

