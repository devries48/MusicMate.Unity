using System;

    [Serializable]
    public class ImportReleaseResult
    {
        /// <summary>True for virtual (no audio files yet), e.g., Last.fm scrobbles.</summary>
        public bool IsVirtual { get; set; }

        /// <summary>Best-known title (from tags; else folder name).</summary>
        public string Title { get; set; }

        /// <summary>Primary artist(s). Use DataResult so later you can populate Id if matched to DB.</summary>
        public string Artist { get; set; }

        /// <summary>Optional year if available.</summary>
        public int? Year { get; set; }

        // ---------- Artwork ----------
        /// <summary>Small display image for the list; keep it lightweight.</summary>
        public ImageModel Artwork { get; set; }

        /// <summary>Where the artwork came from (helps with debugging/UX hints).</summary>
        public string ArtworkHint { get; set; } // "embedded", "folder.jpg", "api:lastfm", etc.

        // ---------- Dedup / matching ----------
        /// <summary>RoadieId if your API looked up and found an existing release.</summary>
        public Guid? ExistingReleaseId { get; set; }

        // ---------- Local-folder specific ----------
        /// <summary>Absolute path of the release folder (for local imports).</summary>
        public string FolderPath { get; set; }
    }