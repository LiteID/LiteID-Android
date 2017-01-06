using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

//Placeholder class for the hash of a document
//Constructor initializes 32 bytes of random numbers
//ToString returns as hex string
public class BHash
{
    [XmlIgnore]
    public Byte[] HashData = new Byte[32];
    public BHash()
    {
        Random random = new Random();
        random.NextBytes(HashData);
    }
    public override string ToString()
    {
        StringBuilder hex = new StringBuilder(HashData.Length * 2);
        foreach (byte b in HashData) hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }
}

[Serializable]
public class Document
{
    [XmlAttribute]
    public string ID;
    private byte[] Hash;
    private SHA256 Hasher = SHA256.Create();
    public string Name;
    public string MimeType;
    public bool TextDoc; //True is yes
    public DateTime IngestionTime;

    //Ingest file as document
    public void IngestDocument(Stream FileDoc, string MimeType)
    {
        GIngestDocument(FileDoc);
    }

    //Ingest string as document
    public void IngestDocument(string TextDoc)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(TextDoc);
        writer.Flush();
        stream.Position = 0;
        MimeType = "text/plain";
        GIngestDocument(stream);
    }

    //General ingestment tasks
    private void GIngestDocument(Stream DocStream)
    {
        IngestionTime = DateTime.Now;

        Hash = Hasher.ComputeHash(DocStream);
        ID = BytesToHex(Hash);

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string filename = Path.Combine(path, ID);
        StreamWriter writer = new StreamWriter(filename, false);
        DocStream.CopyTo(writer.BaseStream);
        writer.Close();
    }

    //Convert byte array to hex string
    private string BytesToHex(byte[] Bytes)
    {
        StringBuilder hex = new StringBuilder(Bytes.Length * 2);
        foreach (byte b in Bytes) hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    //Initialize document with random data
    public void RandomDocument(Random gen)
    {
        ID = Hash.ToString();

        string[] names = { "Document", "Work Thing", "Name", "ID", "Video", "Picture", "Evidence", "Incriminating Record", "Stolen SSN", "Unencrypted Password", "Unfinished App", "Joke Text", "Contract", "Loan" };
        Name = names.PickRandom();
        Name += " #" + (gen.Next(99) + 1);

        DateTime start = new DateTime(2016, 1, 1);
        //int range = (DateTime.Today - start).Days;
        int range = 365;
        IngestionTime = start.AddDays(gen.Next(range));
    }
}

public class DocumentList
{
    public List<Document> Documents = new List<Document>();
    private XmlSerializer dlser = new XmlSerializer(typeof(List<Document>), "Documents");

    public DocumentList()
    {

    }

    public DocumentList(string filepath)
    {
        LoadList(filepath);
    }

    public void LoadList(string filepath)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string filename = Path.Combine(path, filepath);
        TextReader reader;
        try
        {
            reader = new StreamReader(filename);
        }
        catch
        {
            return;
        }
        Documents = (List<Document>)dlser.Deserialize(reader);
        reader.Close();
    }

    public void SaveList(string filepath)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string filename = Path.Combine(path, filepath);
        TextWriter writer = new StreamWriter(filename, false);
        dlser.Serialize(writer, Documents);
        writer.Close();
    }

    public void Randomize()
    {
        Documents.Clear();
        Random random = new Random();
        int count = random.Next(30);
        for (int i = 0; i < count; i++)
        {
            Document inter = new Document();
            inter.RandomDocument(random);
            Documents.Add(inter);
        }
    }
}