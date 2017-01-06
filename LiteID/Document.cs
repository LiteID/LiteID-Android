using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using CmprDir;

[Serializable]
public class Document
{
    [XmlAttribute]
    public string ID;
    public byte[] Hash;
    public byte[] Salt;
    private SHA256 Hasher;
    public string Name;
    public string MimeType;
    public bool TextDoc; //True is yes
    public DateTime IngestionTime;

    //Ingest file as document
    public void IngestDocument(Stream FileDoc, string MimeType)
    {
        this.MimeType = MimeType;
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

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string tempname = Path.Combine(path, "ingest.temp");
        FileStream Writer = File.OpenWrite(tempname);
        Hasher = SHA256.Create();
        byte[] ingest = new byte[32768];
        int read = 0;
        while((read = DocStream.Read(ingest, 0, 32768)) > 0)
        {
            Hasher.TransformBlock(ingest, 0, read, ingest, 0);
            Writer.Write(ingest, 0, read);
        }
        Writer.Close();

        Salt = new byte[32];
        Random random = new Random();
        random.NextBytes(Salt);
        Hasher.TransformFinalBlock(Salt, 0, Salt.Length);
        Hash = Hasher.Hash;
        ID = BytesToHex(Hash);
        Hasher.Dispose();

        string filename = Path.Combine(path, ID);
        File.Move(tempname, filename);
        File.Delete(tempname);
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