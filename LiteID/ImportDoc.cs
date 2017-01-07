using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using System.IO;
using Android.Net;

namespace LiteID
{
    [Activity(Label = "LiteID Document")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] {
        Intent.CategoryDefault,
        Intent.CategoryOpenable
    }, DataMimeTypes = new[] {
        "application/octet-stream"
    }, DataPathPattern = "*.lxe")]
    public class ImportDoc : Activity
    {
        private LiteIDContext Context;
        private Document CurrentDoc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Context = new LiteIDContext();

            Uri PackedDocURI;
            if (Intent.HasExtra("PackedDocURI"))
            {
               PackedDocURI = (Uri)Intent.GetParcelableExtra("PackedDocURI");
            }
            else if (Intent.Data != null)
            {
                PackedDocURI = Intent.Data;
            }
            else
            {
                Android.Util.Log.Error("liteid-import", "No valid URI in intent!");
                Finish();
                return;
            }

            Stream PackedDocStream = ContentResolver.OpenInputStream(PackedDocURI);
            CurrentDoc = Document.ImportDocument(PackedDocStream);


            SetContentView(Resource.Layout.ImportDoc);
            TextView docTitle = FindViewById<TextView>(Resource.Id.docTitle);
            docTitle.Text = CurrentDoc.Name;
            TextView docDate = FindViewById<TextView>(Resource.Id.docDate);
            docDate.Text = "Added to blockchain on: " + CurrentDoc.IngestionTime.ToLongDateString();

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
                    string filename = Path.Combine(path, "import/" + CurrentDoc.ID);
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
            
            TextView textOriginID = FindViewById<TextView>(Resource.Id.textOriginID);
            if (CurrentDoc.OriginID == Context.Config.BlockchainID)
            {
                textOriginID.Text = "0x" + LiteIDContext.BytesToHex(CurrentDoc.OriginID) + " (You)";
            }
            else
            {
                textOriginID.Text = "0x" + LiteIDContext.BytesToHex(CurrentDoc.OriginID);
            }

            Button buttonVerify = FindViewById<Button>(Resource.Id.buttonVerify);
            Button buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
            Button buttonDiscard = FindViewById<Button>(Resource.Id.buttonDiscard);

            buttonVerify.Click += delegate
            {
                //TODO: Implement this
                Toast.MakeText(this.ApplicationContext, "Verified", ToastLength.Long).Show();
            };

            buttonSave.Click += delegate
            {
                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string docImported = Path.Combine(path, "import/" + CurrentDoc.ID);
                string docFinal = Path.Combine(path, CurrentDoc.ID);
                if (File.Exists(docFinal))
                {
                    Toast.MakeText(this.ApplicationContext, "You already have this document.", ToastLength.Long).Show();
                }
                else
                {
                    File.Move(docImported, docFinal);
                    Context.DocStore.Documents.Add(CurrentDoc);
                    Context.DocStore.SaveList(Context.DocStoreFile);
                }
                Finish();
            };

            buttonDiscard.Click += delegate
            {
                new AlertDialog.Builder(this)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetTitle("Delete File")
                    .SetMessage("Are you sure you want to discard this document? You can import it again later.")
                    .SetPositiveButton("Yes", delegate
                    {
                        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        string filename = Path.Combine(path, "import/" + CurrentDoc.ID);
                        File.Delete(filename);
                        Finish();
                    })
                   .SetNegativeButton("No", delegate { })
                   .Show();
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (CurrentDoc != null)
            {
                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string filename = Path.Combine(path, "import/" + CurrentDoc.ID);
                File.Delete(filename);
            }
        }
    }
}