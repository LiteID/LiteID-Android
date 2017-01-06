using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content.PM;
using Android.Database;
using Android;
using static Android.Manifest;

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
        "message/*",
        "file/*"
    })]
    public class AddDoc : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M
                && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new String[] { Manifest.Permission.ReadExternalStorage }, 1);
            }

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
                radioFile.Checked = true;
                radioText.Checked = false;
                PackageManager pm = ApplicationContext.PackageManager;
                Intent getFileIntent;
                getFileIntent = new Intent(Intent.ActionGetContent);
                getFileIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                getFileIntent.SetType("*/*");
                getFileIntent.PutExtra(Intent.ExtraLocalOnly, true);
                if (getFileIntent.ResolveActivity(pm) != null)
                {
                    StartActivityForResult(getFileIntent, 1);
                }
                else
                {
                    Toast.MakeText(this.ApplicationContext, "There are no file browsers!", ToastLength.Long).Show();
                }
            };

            radioText.Click += delegate
            {
                radioFile.Checked = false;
            };

            textContent.Click += delegate
            {
                radioFile.Checked = false;
                radioText.Checked = true;
            };

            buttonCreate.Click += delegate
            {
                if (textTitle.Text != "")
                {
                    if (radioFile.Checked && newFileUri != null)
                    {
                        Toast.MakeText(this.ApplicationContext, "Adding a file isn't implemented", ToastLength.Long).Show();
                        //Finish();
                    }
                    else if (radioText.Checked && textContent.Text != "")
                    {
                        Document newDoc = new Document();
                        newDoc.RandomDocument(new Random());
                        newDoc.Name = textTitle.Text;
                        newDoc.IngestionTime = DateTime.Now;
                        DocumentList docList = new DocumentList("documents.lxm");
                        docList.Documents.Add(newDoc);
                        docList.SaveList("documents.lxm");
                        Finish();
                    }
                }
            };

            buttonDiscard.Click += delegate
            {
                Finish();
            };

            if (Intent.HasExtra(Intent.ExtraText))
            {
                radioFile.Checked = false;
                radioText.Checked = true;
                textContent.Text = Intent.GetStringExtra(Intent.ExtraText);
            }
            else if (Intent.DataString != null)
            {
                Uri file = new Uri(Intent.DataString);
                if (file.IsFile)
                {
                    newFileUri = file;
                    buttonFile.Text = Path.GetFileName(file.AbsolutePath);
                    radioFile.Checked = true;
                    radioText.Checked = false;
                }
            }
        }

        private Uri newFileUri;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                Uri file = new Uri(data.DataString);
                Android.Net.Uri aURI = Android.Net.Uri.Parse(file.ToString());
                Button buttonFile = FindViewById<Button>(Resource.Id.buttonFile);
                if (file.Scheme == "file")
                {
                    newFileUri = file;
                    buttonFile.Text = Path.GetFileName(file.AbsolutePath);
                }
                else if (file.Scheme == "content")
                {
                    newFileUri = file;
                    string[] columns = { Android.Provider.MediaStore.Files.FileColumns.DisplayName };
                    ICursor cursor = ContentResolver.Query(data.Data, columns, null, null, null);
                    cursor.MoveToFirst();
                    buttonFile.Text = cursor.GetString(0);
                }
                else
                {
                    Toast toast = Toast.MakeText(this.ApplicationContext, "You can only select a file on your device", ToastLength.Long);
                    toast.Show();
                }
            }
        }
    }
}