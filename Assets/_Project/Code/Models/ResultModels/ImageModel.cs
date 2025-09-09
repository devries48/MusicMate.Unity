using System;

[Serializable]
public class ImageModel
{
    public Guid? Id { get; set; }
    public byte[] Bytes { get; set; }
    public string Caption { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Url { get; set; }

    public string Source { get; set; }
    public string SourcePath { get; set; } // e.g. file path if Source == Disk
}