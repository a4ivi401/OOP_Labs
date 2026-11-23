using System.Diagnostics;
using Lab03.BLL.Mappers;
using Lab03.BLL.Models;
using Lab03.DAL.Entities;
using Lab03.DAL.Providers;

namespace Lab03.BLL.Services;

public class SerializationComparisonService
{
    public static BookModel[] GetInitialSampleBooks()
    {
        return new BookModel[]
        {
            new("978-0132350884", "Clean Code", 2008, 450.00m, 1500),
            new("978-0201633610", "Design Patterns", 1994, 620.00m, 1200),
            new("978-0134494166", "Clean Architecture", 2017, 510.00m, 2000),
            new("978-1491987650", "C# in a Nutshell", 2023, 780.00m, 800)
        };
    }

    public SerializationExperimentResult RunArrayExperiment<TProvider>(string baseDir, BookModel[] books)
        where TProvider : IDataProvider<BookEntity[]>, new()
    {
        TProvider provider = new();
        string filePath = Path.Combine(baseDir, $"books_array{provider.DefaultExtension}");
        BookEntity[] entities = BookMapper.ToEntities(books);

        Stopwatch sw = Stopwatch.StartNew();
        provider.Serialize(filePath, entities);
        sw.Stop();
        double serTime = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        BookEntity[] restoredEntities = provider.Deserialize(filePath);
        sw.Stop();
        double deserTime = sw.Elapsed.TotalMilliseconds;

        BookModel[] restored = BookMapper.ToModels(restoredEntities);
        bool verified = AreBooksEqual(books, restored);
        long fileSize = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;

        return new SerializationExperimentResult
        {
            FormatName = provider.FormatName,
            TargetType = "Array (Book[])",
            FilePath = filePath,
            FileSizeBytes = fileSize,
            SerializationTimeMs = Math.Round(serTime, 3),
            DeserializationTimeMs = Math.Round(deserTime, 3),
            DataIntegrityVerified = verified,
            ItemCount = restored.Length
        };
    }

    public SerializationExperimentResult RunListExperiment<TProvider>(string baseDir, List<BookModel> books)
        where TProvider : IDataProvider<List<BookEntity>>, new()
    {
        TProvider provider = new();
        string filePath = Path.Combine(baseDir, $"books_list{provider.DefaultExtension}");
        List<BookEntity> entities = books.Select(BookMapper.ToEntity).ToList();

        Stopwatch sw = Stopwatch.StartNew();
        provider.Serialize(filePath, entities);
        sw.Stop();
        double serTime = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        List<BookEntity> restoredEntities = provider.Deserialize(filePath);
        sw.Stop();
        double deserTime = sw.Elapsed.TotalMilliseconds;

        List<BookModel> restored = restoredEntities.Select(BookMapper.ToModel).ToList();
        bool verified = AreBooksEqual(books.ToArray(), restored.ToArray());
        long fileSize = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;

        return new SerializationExperimentResult
        {
            FormatName = provider.FormatName,
            TargetType = "List (List<Book>)",
            FilePath = filePath,
            FileSizeBytes = fileSize,
            SerializationTimeMs = Math.Round(serTime, 3),
            DeserializationTimeMs = Math.Round(deserTime, 3),
            DataIntegrityVerified = verified,
            ItemCount = restored.Count
        };
    }

    public SerializationExperimentResult[] RunFullComparison(string outputDirectory)
    {
        BookModel[] sampleBooks = GetInitialSampleBooks();
        List<BookModel> sampleList = sampleBooks.ToList();

        List<SerializationExperimentResult> results = new()
        {
            RunArrayExperiment<JsonDataProvider<BookEntity[]>>(outputDirectory, sampleBooks),
            RunListExperiment<JsonDataProvider<List<BookEntity>>>(outputDirectory, sampleList),

            RunArrayExperiment<XmlDataProvider<BookEntity[]>>(outputDirectory, sampleBooks),
            RunListExperiment<XmlDataProvider<List<BookEntity>>>(outputDirectory, sampleList),

            RunArrayExperiment<BinaryDataProvider<BookEntity[]>>(outputDirectory, sampleBooks),
            RunListExperiment<BinaryDataProvider<List<BookEntity>>>(outputDirectory, sampleList),

            RunArrayExperiment<CustomDataProvider<BookEntity[]>>(outputDirectory, sampleBooks),
            RunListExperiment<CustomDataProvider<List<BookEntity>>>(outputDirectory, sampleList)
        };

        return results.ToArray();
    }

    private static bool AreBooksEqual(BookModel[] first, BookModel[] second)
    {
        if (first.Length != second.Length) return false;
        for (int i = 0; i < first.Length; i++)
        {
            if (first[i].SerialNumber != second[i].SerialNumber ||
                first[i].Title != second[i].Title ||
                first[i].PublicationYear != second[i].PublicationYear ||
                first[i].Price != second[i].Price ||
                first[i].CopiesCount != second[i].CopiesCount)
            {
                return false;
            }
        }
        return true;
    }
}
