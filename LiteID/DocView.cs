using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using System.IO;

namespace LiteID
{
    [Activity(Label = "LiteID Document")]
    public class DocView : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Document CurrentDoc;
            DocumentList DocList = new DocumentList("documents.lxm");
            if (Intent.HasExtra("TargetID"))
            {
                string TargetID = Intent.GetStringExtra("TargetID");
                try
                {
                    CurrentDoc = DocList.GetDocumentById(TargetID);
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

            SetContentView(Resource.Layout.DocView);
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
                    Android.Net.Uri extURI = FileProvider.GetUriForFile(
                        ApplicationContext, "org.LiteID.fileprovider", outfile);
                    Intent viewIntent = new Intent(Intent.ActionView);
                    viewIntent.SetDataAndType(extURI, CurrentDoc.MimeType);
                    viewIntent.AddFlags(ActivityFlags.NewTask);
                    viewIntent.SetFlags(ActivityFlags.GrantReadUriPermission);

                    Android.Content.PM.PackageManager pm = ApplicationContext.PackageManager;
                    if (viewIntent.ResolveActivity(pm) != null)
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
                Toast.MakeText(this.ApplicationContext, "Not Yet Implemented", ToastLength.Long).Show();
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
                        DocList.Documents.Remove(CurrentDoc);
                        DocList.SaveList("documents.lxm");
                        Finish();
                    })
                   .SetNegativeButton("No", delegate { })
                   .Show();
            };
        }
    }
}