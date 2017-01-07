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
using Android;
using Android.Database;

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
        private LiteIDContext Context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            Context = new LiteIDContext();
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
                        Document newDoc = Document.IngestDocument(ContentResolver.OpenInputStream(newFileUri), newMimeType);
                        newDoc.Name = textTitle.Text;
                        newDoc.OriginID = Context.Config.BlockchainID;
                        Context.DocStore.Documents.Add(newDoc);
                        Context.DocStore.SaveList("documents.lxm");
                        Finish();
                    }
                    else if (radioText.Checked && textContent.Text != "")
                    {
                        Document newDoc = Document.IngestDocument(textContent.Text);
                        newDoc.Name = textTitle.Text;
                        newDoc.OriginID = Context.Config.BlockchainID;
                        Context.DocStore.Documents.Add(newDoc);
                        Context.DocStore.SaveList("documents.lxm");
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this.ApplicationContext, "You must set the content", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this.ApplicationContext, "You must set a title", ToastLength.Long).Show();
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
                OnActivityResult(1, Result.Ok, Intent);
            }
        }

        private Android.Net.Uri newFileUri;
        private string newMimeType;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                Uri file = new Uri(data.DataString);
                Button buttonFile = FindViewById<Button>(Resource.Id.buttonFile);
                if (file.Scheme == "file")
                {
                    newFileUri = data.Data;
                    buttonFile.Text = Path.GetFileName(file.AbsolutePath);
                    newMimeType = MimeTypes.GetMimeType(Path.GetExtension(file.AbsolutePath));
                }
                else if (file.Scheme == "content")
                {
                    newFileUri = data.Data;
                    string[] columns = {
                        Android.Provider.MediaStore.Files.FileColumns.DisplayName,
                        Android.Provider.MediaStore.Files.FileColumns.MimeType
                    };
                    ICursor cursor = ContentResolver.Query(data.Data, columns, null, null, null);
                    cursor.MoveToFirst();
                    buttonFile.Text = cursor.GetString(0);
                    newMimeType = cursor.GetString(1);
                    cursor.Close();
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