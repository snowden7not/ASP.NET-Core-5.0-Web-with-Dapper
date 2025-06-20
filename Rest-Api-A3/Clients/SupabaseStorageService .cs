using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using FileOptions = Supabase.Storage.FileOptions;

namespace Rest_Api_A3.Clients
{
    public class SupabaseStorageService : IStorageService
    {
        private readonly string _bucketName;
        private readonly Supabase.Client _client;

        public SupabaseStorageService(
            Supabase.Client client,
            IOptions<SupabaseOptions> supabaseOptions)
        {
            _client = client;
            _bucketName = supabaseOptions.Value.BucketName
                          ?? throw new InvalidOperationException("BucketName must be configured in SupabaseOptions.");
        }

        // Insert Poster image in supabase bucket. if poster image exists, then delete and upload new.
        public async Task<string> UploadMoviePosterAsync(int movieId, IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var uniqueId = Guid.NewGuid().ToString("N");
            var key = $"movies/{movieId}/poster_{uniqueId}{ext}";

            // read into memory
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            // upload 
            await _client
                .Storage
                .From(_bucketName)
                .Upload(ms.ToArray(), key, new FileOptions { CacheControl = "3600", Upsert = true });

            return _client
                   .Storage
                   .From(_bucketName)
                   .GetPublicUrl(key);
        }

        // Delete Poster image in supabase bucket using public URL.
        public async Task DeleteMoviePosterByUrlAsync(string publicUrl)
        {
            if (string.IsNullOrEmpty(publicUrl))
                return;

            var marker = $"/{_bucketName}/";
            var idx = publicUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return;

            var key = publicUrl.Substring(idx + marker.Length);
            await _client
                .Storage
                .From(_bucketName)
                .Remove(new List<string> { key });
        }
    }
}

