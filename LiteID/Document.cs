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

    //Returns the content of a text document
    public string GetTextContent()
    {
        if (TextDoc)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, ID);

            if (!File.Exists(filename))
                filename = Path.Combine(path, "import/" + ID);

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

    //Export document and metadata in portable format
    public Uri ExportDocument()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string tempfolder = Path.Combine(path, "temp-export");
        string docin = Path.Combine(path, ID);
        string docout = Path.Combine(tempfolder, "payload");
        string metaout = Path.Combine(tempfolder, "metadata.lxm");

        Directory.CreateDirectory(tempfolder);
        File.Copy(docin, docout);

        XmlSerializer docser = new XmlSerializer(typeof(Document));
        TextWriter writer = new StreamWriter(metaout, false);
        docser.Serialize(writer, this);
        writer.Close();

        string exportDir = Path.Combine(path, "export");
        string output = Path.Combine(exportDir, Name.Replace(' ', '_') + ".lxe");
        if (!Directory.Exists(exportDir))
            Directory.CreateDirectory(exportDir);
        GZUtils.CompressDirectory(tempfolder, output);
        Directory.Delete(tempfolder, true);
        return new Uri(output);
    }

    //Import 
    public static Document ImportDocument(Stream LXE)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string input = Path.Combine(path, "import.temp");
        string tempfolder = Path.Combine(path, "temp-import");
        string docin = Path.Combine(tempfolder, "payload");
        string metain = Path.Combine(tempfolder, "metadata.lxm");

        Directory.CreateDirectory(tempfolder);
        Stream tempfile = File.OpenWrite(input);
        LXE.CopyTo(tempfile);
        LXE.Close();
        tempfile.Close();
        GZUtils.DecompressToDirectory(input, tempfolder);
        File.Delete(input);

        XmlSerializer docser = new XmlSerializer(typeof(Document));
        TextReader reader = new StreamReader(metain);
        Document newDoc = (Document)docser.Deserialize(reader);

        string importDir = Path.Combine(path, "import");
        string docout = Path.Combine(importDir, newDoc.ID);
        if (!Directory.Exists(importDir))
            Directory.CreateDirectory(importDir);
        if (File.Exists(docout))
            File.Delete(docout);
        File.Move(docin, docout);
        Directory.Delete(tempfolder, true);
        
        return newDoc;
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
            reader.Close();
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