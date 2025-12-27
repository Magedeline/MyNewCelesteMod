namespace DesoloZantas.Core.Metadata
{
    /// <summary>
    /// Defines the chapter structure and metadata for DesoloZantas campaign.
    /// This includes special chapters that need specific handling.
    /// </summary>
    public static class ChapterMetadata
    {
        /// <summary>
        /// Defines a chapter with its ID, name, and type.
        /// </summary>
        public class ChapterInfo
        {
            public int ChapterNumber { get; set; }
            public string InternalName { get; set; }
            public string DisplayName { get; set; }
            public ChapterType Type { get; set; }
            public bool RequiresSpecialHandling { get; set; }

            public ChapterInfo(int chapterNumber, string internalName, string displayName, ChapterType type, bool requiresSpecialHandling = false)
            {
                ChapterNumber = chapterNumber;
                InternalName = internalName;
                DisplayName = displayName;
                Type = type;
                RequiresSpecialHandling = requiresSpecialHandling;
            }
        }

        public enum ChapterType
        {
            Prologue,
            MainChapter,
            Epilogue,
            PostEpilogue
        }

        /// <summary>
        /// All chapters in the correct order, including special chapters.
        /// </summary>
        public static readonly List<ChapterInfo> AllChapters = new List<ChapterInfo>
        {
            // Chapter 0 - Prologue (Special Handling Required)
            new ChapterInfo(0, "ch0_awakening_dream", "Awakening Dream", ChapterType.Prologue, true),
            
            // Main Chapters (1-16)
            new ChapterInfo(1, "ch1", "Chapter 1", ChapterType.MainChapter),
            new ChapterInfo(2, "ch2", "Chapter 2", ChapterType.MainChapter),
            new ChapterInfo(3, "ch3", "Chapter 3", ChapterType.MainChapter),
            new ChapterInfo(4, "ch4", "Chapter 4", ChapterType.MainChapter),
            new ChapterInfo(5, "ch5", "Chapter 5", ChapterType.MainChapter),
            new ChapterInfo(6, "ch6", "Chapter 6", ChapterType.MainChapter),
            new ChapterInfo(7, "ch7", "Chapter 7", ChapterType.MainChapter),
            new ChapterInfo(8, "ch8", "Chapter 8", ChapterType.MainChapter),
            new ChapterInfo(9, "ch9", "Chapter 9", ChapterType.MainChapter),
            new ChapterInfo(10, "ch10", "Chapter 10", ChapterType.MainChapter),
            new ChapterInfo(11, "ch11", "Chapter 11", ChapterType.MainChapter),
            new ChapterInfo(12, "ch12", "Chapter 12", ChapterType.MainChapter),
            new ChapterInfo(13, "ch13", "Chapter 13", ChapterType.MainChapter),
            new ChapterInfo(14, "ch14", "Chapter 14", ChapterType.MainChapter),
            new ChapterInfo(15, "ch15", "Chapter 15", ChapterType.MainChapter),
            new ChapterInfo(16, "ch16", "Chapter 16", ChapterType.MainChapter),
            
            // Chapter 17 - Epilogue (Special Handling Required)
            new ChapterInfo(17, "ch17_final_resonance", "Final Resonance", ChapterType.Epilogue, true),
            
            // Additional Chapters (18-20)
            new ChapterInfo(18, "ch18", "Chapter 18", ChapterType.MainChapter),
            new ChapterInfo(19, "ch19", "Chapter 19", ChapterType.MainChapter),
            new ChapterInfo(20, "ch20", "Chapter 20", ChapterType.MainChapter),
            
            // Chapter 21 - Post-Epilogue (Special Handling Required)
            new ChapterInfo(21, "ch21_post_respite", "Post Respite", ChapterType.PostEpilogue, true)
        };

        /// <summary>
        /// Get chapters that require special handling.
        /// </summary>
        public static List<ChapterInfo> SpecialChapters => AllChapters.Where(c => c.RequiresSpecialHandling).ToList();

        /// <summary>
        /// Get the prologue chapter (Chapter 0).
        /// </summary>
        public static ChapterInfo Prologue => AllChapters.FirstOrDefault(c => c.Type == ChapterType.Prologue);

        /// <summary>
        /// Get the epilogue chapter (Chapter 17).
        /// </summary>
        public static ChapterInfo Epilogue => AllChapters.FirstOrDefault(c => c.Type == ChapterType.Epilogue);

        /// <summary>
        /// Get the post-epilogue chapter (Chapter 21).
        /// </summary>
        public static ChapterInfo PostEpilogue => AllChapters.FirstOrDefault(c => c.Type == ChapterType.PostEpilogue);

        /// <summary>
        /// Get main story chapters (1-16, 18-20).
        /// </summary>
        public static List<ChapterInfo> MainChapters => AllChapters.Where(c => c.Type == ChapterType.MainChapter).ToList();

        /// <summary>
        /// Get chapter info by number.
        /// </summary>
        public static ChapterInfo GetChapter(int chapterNumber)
        {
            return AllChapters.FirstOrDefault(c => c.ChapterNumber == chapterNumber);
        }

        /// <summary>
        /// Get chapter info by internal name.
        /// </summary>
        public static ChapterInfo GetChapterByName(string internalName)
        {
            return AllChapters.FirstOrDefault(c => c.InternalName == internalName);
        }

        /// <summary>
        /// Check if a chapter requires special handling.
        /// </summary>
        public static bool RequiresSpecialHandling(int chapterNumber)
        {
            var chapter = GetChapter(chapterNumber);
            return chapter?.RequiresSpecialHandling ?? false;
        }

        /// <summary>
        /// Get the total number of chapters.
        /// </summary>
        public static int TotalChapterCount => AllChapters.Count;

        /// <summary>
        /// Get the total number of main chapters (excluding prologue, epilogue, post-epilogue).
        /// </summary>
        public static int MainChapterCount => MainChapters.Count;
    }
}




