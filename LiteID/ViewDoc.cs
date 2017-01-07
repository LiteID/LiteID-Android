using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using Android.Net;
using System.IO;
using System.Linq;
using Android.Content.PM;
using System.Collections.Generic;

namespace LiteID
{
    [Activity(Label = "LiteID Document")]
    public class ViewDoc : Activity
    {
        private LiteIDContext Context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Context = new LiteIDContext();

            Document CurrentDoc;
            if (Intent.HasExtra("TargetID"))
            {
                string TargetID = Intent.GetStringExtra("TargetID");
                try
                {
                    CurrentDoc = Context.DocStore.GetDocumentById(TargetID);
                }
                catch
                {
                    Finish();
                    return;
                }
            }
            else
            {
                Finish();
                return;
            }

            SetContentView(Resource.Layout.ViewDoc);
            TextView docTitle = FindViewById<TextView>(Resource.Id.docTitle);
            docTitle.Text = CurrentDoc.Name;
            TextView docDate = FindViewById<TextView>(Resource.Id.docDate);
            docDate.Text = "Added on: " + CurrentDoc.IngestionTime.ToLongDateString();
            if (CurrentDoc.TextDoc)
            {
                LinearLayout modeText = FindViewById<LinearLayout>(Resource.Id.modeText);
                modeText.Visibility = ViewStates.Visible;
                TextView docContent = FindViewById<TextView>(Resource.Id.docContent);
                docContent.Text = CurrentDoc.GetTextContent();
            }
            else
            {
                LinearLayout modeFile = FindViewById<LinearLayout>(Resource.Id.modeFile);
                modeFile.Visibility = ViewStates.Visible;
                TextView docType = FindViewById<TextView>(Resource.Id.docType);
                docType.Text = "Type: " + CurrentDoc.MimeType;
                Button buttonOpen = FindViewById<Button>(Resource.Id.buttonOpen);

                buttonOpen.Click += delegate
                {
                    string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string filename = Path.Combine(path, CurrentDoc.ID);
                    Java.IO.File outfile = new Java.IO.File(filename);
                    Uri extURI = FileProvider.GetUriForFile(ApplicationContext, "org.LiteID.fileprovider", outfile);
                    Intent viewIntent = new Intent(Intent.ActionView);
                    viewIntent.SetDataAndType(extURI, CurrentDoc.MimeType);
                    viewIntent.AddFlags(ActivityFlags.NewTask);
                    viewIntent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    
                    if (viewIntent.ResolveActivity(ApplicationContext.PackageManager) != null)
                    {
                        StartActivity(viewIntent);
                    }
                    else
                    {
                        Toast.MakeText(this.ApplicationContext, "You don't have any apps that can open this.", ToastLength.Long).Show();
                    }
                };
            }

            Button buttonExport = FindViewById<Button>(Resource.Id.buttonExport);
            Button buttonDelete = FindViewById<Button>(Resource.Id.buttonDelete);

            buttonExport.Click += delegate
            {
                System.Uri PackedDoc = CurrentDoc.ExportDocument();
                Java.IO.File outfile = new Java.IO.File(PackedDoc.AbsolutePath);
                Uri extURI = FileProvider.GetUriForFile(ApplicationContext, "org.LiteID.fileprovider", outfile);
                Intent emailIntent = new Intent(Intent.ActionSend);
                emailIntent.SetType("application/octet-stream");
                emailIntent.PutExtra(Intent.ExtraSubject, "LiteID Document");
                emailIntent.PutExtra(Intent.ExtraText, "Attached is a verifiable LiteID document.");
                emailIntent.PutExtra(Intent.ExtraStream, extURI);
                emailIntent.AddFlags(ActivityFlags.NewTask);
                emailIntent.SetFlags(ActivityFlags.GrantReadUriPermission);
                
                if (emailIntent.ResolveActivity(ApplicationContext.PackageManager) != null)
                {
                    IList<ResolveInfo> resInfoList = ApplicationContext.PackageManager.QueryIntentActivities(emailIntent, PackageInfoFlags.MatchDefaultOnly);
                    foreach (ResolveInfo resolveInfo in resInfoList)
                    {
                        string packageName = resolveInfo.ActivityInfo.PackageName;
                        ApplicationContext.GrantUriPermission(packageName, extURI, ActivityFlags.GrantReadUriPermission);
                    }
                    StartActivity(Intent.CreateChooser(emailIntent, "Share Document"));
                }
                else
                {
                    Toast.MakeText(this.ApplicationContext, "You don't have any apps that can share this.", ToastLength.Long).Show();
                }
            };

            buttonDelete.Click += delegate
            {
                new AlertDialog.Builder(this)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetTitle("Delete File")
                    .SetMessage("Are you sure you want to permanently delete this document?")
                    .SetPositiveButton("Yes", delegate
                    {
                        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        string filename = Path.Combine(path, CurrentDoc.ID);
                        File.Delete(filename);
                        Context.DocStore.Documents.Remove(CurrentDoc);
                        Context.DocStore.SaveList(Context.DocStoreFile);
                        Finish();
                    })
                   .SetNegativeButton("No", delegate { })
                   .Show();
            };

            if (CurrentDoc.OriginID == null || !CurrentDoc.OriginID.SequenceEqual(Context.Config.BlockchainID))
            {
                LinearLayout modeRemote = FindViewById<LinearLayout>(Resource.Id.modeRemote);
                TextView textOriginID = FindViewById<TextView>(Resource.Id.textOriginID);
                Button buttonVerify = FindViewById<Button>(Resource.Id.buttonVerify);

                modeRemote.Visibility = ViewStates.Visible;
                if (CurrentDoc.OriginID != null)
                {
                    textOriginID.Text = "0x" + LiteIDContext.BytesToHex(CurrentDoc.OriginID);
                }
                else
                {
                    textOriginID.Text = "Local Document";
                    buttonVerify.Visibility = ViewStates.Gone;
                }

                buttonVerify.Click += delegate
                {
                    //TODO: Implement this
                    Toast.MakeText(this.ApplicationContext, "Verified", ToastLength.Long).Show();
                };
            }
        }
    }
}