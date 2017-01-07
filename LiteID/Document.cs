using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using CmprDir;
using LiteID;

[Serializable]
public class Document
{
    [XmlAttribute]
    public string ID;
    public byte[] Hash;
    public byte[] Salt;
    public string Name;
    public string MimeType;
    public bool TextDoc; //True is yes
    public DateTime IngestionTime;
    public byte[] OriginID;
    
    private SHA256 Hasher;
    private string path;

    public Document()
    {
        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }

    //Ingest file as document
    public static Document IngestDocument(Stream FileDoc, string MimeType)
    {
        Document newDoc = new Document();
        newDoc.MimeType = MimeType;
        newDoc.TextDoc = false;
        GIngestDocument(newDoc, FileDoc);
        return newDoc;
    }

    //Ingest string as document
    public static Document IngestDocument(string TextDoc)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(TextDoc);
        writer.Flush();
        stream.Position = 0;

        Document newDoc = new Document();
        newDoc.MimeType = "text/plain";
        newDoc.TextDoc = true;
        GIngestDocument(newDoc, stream);
        return newDoc;
    }

    //General ingestment tasks
    private static void GIngestDocument(Document newDoc, Stream DocStream)
    {
        newDoc.IngestionTime = DateTime.Now;

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string tempname = Path.Combine(path, "ingest.temp");
        FileStream Writer = File.OpenWrite(tempname);
        newDoc.Hasher = SHA256.Create();
        byte[] ingest = new byte[32768];
        int read = 0;
        while((read = DocStream.Read(ingest, 0, 32768)) > 0)
        {
            newDoc.Hasher.TransformBlock(ingest, 0, read, ingest, 0);
            Writer.Write(ingest, 0, read);
        }
        Writer.Close();

        newDoc.Salt = new byte[32];
        Random random = new Random();
        random.NextBytes(newDoc.Salt);
        newDoc.Hasher.TransformFinalBlock(newDoc.Salt, 0, newDoc.Salt.Length);
        newDoc.Hash = newDoc.Hasher.Hash;
        newDoc.ID = LiteIDContext.BytesToHex(newDoc.Hash);
        newDoc.Hasher.Dispose();

        string filename = Path.Combine(path, newDoc.ID);
        File.Move(tempname, filename);
        File.Delete(tempname);
    }

    public string GetTextContent()
    {
        if (TextDoc)
        {
    //Returns the content of a text document
            string filename = Path.Combine(path, ID);
            StreamReader reader = new StreamReader(filename);
            string textContent = reader.ReadToEnd();
            reader.Close();
            return textContent;
        }
        else
        {
            return "";
        }
    }
    //Convert byte array to hex string
    private static string BytesToHex(byte[] Bytes)
    {
        StringBuilder hex = new StringBuilder(Bytes.Length * 2);
        foreach (byte b in Bytes) hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

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
    private XmlSerializer dlser = new XmlSerializer(typeof(List<Document>));

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
        try
        {
            Documents = (List<Document>)dlser.Deserialize(reader);
        }
        catch
        {
            string alternate = Path.Combine(path, filepath + ".old");
            File.Move(filename, alternate);
            return;
        }
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

    public Document GetDocumentById(string ID)
    {
        return Documents.Find(x => x.ID == ID);
    }
}