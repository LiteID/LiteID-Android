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

namespace LiteID
{
    [Activity(Label = "LiteID Options")]
    public class Options : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string ConfigFile = "config.lxm";
            LiteIDConfig Config = LiteIDConfig.Load(ConfigFile);

            SetContentView(Resource.Layout.Options);
            Button buttonGenerateAccount = FindViewById<Button>(Resource.Id.buttonGenerateAccount);
            Button buttonResetID = FindViewById<Button>(Resource.Id.buttonResetID);
            TextView textCurrentID = FindViewById<TextView>(Resource.Id.textCurrentID);
            if (Config.Configured)
            {
                textCurrentID.Text = "0x" + LiteIDContext.BytesToHex(Config.BlockchainID);
            }
            else
            {
                textCurrentID.Text = "No ID - You must deploy a new contract.";
            }
            EditText textRPCAddress = FindViewById<EditText>(Resource.Id.textRPCAddress);
            Button buttonSaveRPC = FindViewById<Button>(Resource.Id.buttonSaveRPC);

            buttonGenerateAccount.Click += delegate
            {
                Toast.MakeText(this.ApplicationContext, "No connection; demo mode", ToastLength.Long).Show();
            };

            buttonResetID.Click += delegate
            {
                Config.BlockchainID = new byte[32];
                Random random = new Random();
                random.NextBytes(Config.BlockchainID);
                textCurrentID.Text = "0x" + LiteIDContext.BytesToHex(Config.BlockchainID);
                Config.Save(ConfigFile);
            };

            buttonSaveRPC.Click += delegate
            {
                Config.RPCAddress = textRPCAddress.Text;
                Config.Save(ConfigFile);
            };
        }
    }
}