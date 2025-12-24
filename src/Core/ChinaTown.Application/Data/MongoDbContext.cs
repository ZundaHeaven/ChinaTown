using ChinaTown.Application.Models;
using ChinaTown.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ChinaTown.Application.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFs;
    private readonly MongoDbConfig _config;

    public MongoDbContext(IOptions<MongoDbConfig> config)
    {
        _config = config.Value;
        var client = new MongoClient(_config.ConnectionString);
        _database = client.GetDatabase(_config.DatabaseName);
        _gridFs = new GridFSBucket(_database);
    }

    public async Task UploadFileAsync(Guid fileId, string fileName, Stream stream)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "fileId", fileId.ToString() },
                { "uploadedAt", DateTime.UtcNow }
            }
        };
        await _gridFs.UploadFromStreamAsync(fileName, stream, options);
    }

    public async Task<byte[]> DownloadFileAsync(Guid fileId)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("metadata.fileId", fileId.ToString());
        var fileInfo = await _gridFs.Find(filter).FirstOrDefaultAsync();
        
        if (fileInfo == null)
            throw new NotFoundException("File not found");

        var stream = new MemoryStream();
        await _gridFs.DownloadToStreamAsync(fileInfo.Id, stream);
        return stream.ToArray();
    }

    public async Task DeleteFileAsync(Guid fileId)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("metadata.fileId", fileId.ToString());
        var fileInfo = await _gridFs.Find(filter).FirstOrDefaultAsync();
        
        if (fileInfo != null)
        {
            await _gridFs.DeleteAsync(fileInfo.Id);
        }
    }
}