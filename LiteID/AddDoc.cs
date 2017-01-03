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
    [Activity(Label = "Add New Document", WindowSoftInputMode = SoftInput.AdjustPan)]
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
            EditText textTitle = FindViewById<EditText>(Resource.Id.textTitle);
            RadioButton radioFile = FindViewById<RadioButton>(Resource.Id.radioFile);
            Button buttonFile = FindViewById<Button>(Resource.Id.buttonFile);
            RadioButton radioText = FindViewById<RadioButton>(Resource.Id.radioText);
            EditText textContent = FindViewById<EditText>(Resource.Id.textContent);
            Button buttonCreate = FindViewById<Button>(Resource.Id.buttonCreate);
            Button buttonDiscard = FindViewById<Button>(Resource.Id.buttonDiscard);

            radioFile.Click += delegate
            {
                radioText.Checked = false;
            };

            buttonFile.Click += delegate
            {
                Intent getFileIntent = new Intent(Intent.ActionGetContent);
                getFileIntent.SetType("*/*");
                StartActivityForResult(getFileIntent, 0);
            };

            radioText.Click += delegate
            {
                radioFile.Checked = false;
            };

            buttonCreate.Click += delegate
            {
                Finish();
            };

            buttonDiscard.Click += delegate
            {
                Finish();
            };
        }
    }
}