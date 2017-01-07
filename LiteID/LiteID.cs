using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace LiteID
{
    public class LiteIDContext
    {
        public string DocStoreFile = "documents.lxm";
        public string ConfigFile = "config.lxm";
        public DocumentList DocStore;
        public LiteIDConfig Config;

        public LiteIDContext()
        {
            Initialize();
        }

        public LiteIDContext(string DocStoreFile, string ConfigFile)
        {
            this.DocStoreFile = DocStoreFile;
            this.ConfigFile = ConfigFile;
            Initialize();
        }

        private void Initialize()
        {
            DocStore = new DocumentList(DocStoreFile);
            Config = LiteIDConfig.Load(ConfigFile);
        }

        public void SaveAll()
        {
            DocStore.SaveList(DocStoreFile);
            Config.Save(ConfigFile);
        }

        //Convert byte array to hex string
        public static string BytesToHex(byte[] Bytes)
        {
            StringBuilder hex = new StringBuilder(Bytes.Length * 2);
            foreach (byte b in Bytes) hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }

    [Serializable]
    public class LiteIDConfig
    {
        public byte[] BlockchainID;
        public bool Configured = false;
        public string RPCAddress = "www.example.com:4444";

        public static LiteIDConfig Load(string filepath)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, filepath);
            XmlSerializer configLoader = new XmlSerializer(typeof(LiteIDConfig));
            TextReader reader;
            LiteIDConfig Config;
            try
            {
                reader = new StreamReader(filename);
            }
            catch
            {
                Android.Util.Log.Info("liteid-config", "Config file missing!");
                return new LiteIDConfig();
            }
            try
            {
                Config = (LiteIDConfig)configLoader.Deserialize(reader);
            }
            catch
            {
                string alternate = Path.Combine(path, filename + ".old");
                File.Move(filename, alternate);
                return new LiteIDConfig();
            }
            reader.Close();
            return Config;
        }

        public void Save(string filepath)
        {
            XmlSerializer configWriter = new XmlSerializer(typeof(LiteIDConfig));
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, filepath);
            TextWriter writer = new StreamWriter(filename, false);
            configWriter.Serialize(writer, this);
            writer.Close();
        }
    }
}