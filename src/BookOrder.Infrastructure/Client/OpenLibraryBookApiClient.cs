using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookOrder.Infrastructure.Client
{
    public class OpenLibraryBookApiClient : IBookApiClient
    {
        private HttpClient _httpClient;
        private const string OpenLibraryUrl = "https://openlibrary.org/";

        public OpenLibraryBookApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookApiResponse.AuthorInfo> GetAuthorInfo(string authorKey)
        {
            var authorResponse = await GetOpenLibraryResponse(authorKey);

            return JsonSerializer.Deserialize<BookApiResponse.AuthorInfo>(authorResponse);
        }

        public async Task<BookApiResponse> GetBookInfo(string bookKey)
        {
            var bookResponse = await GetOpenLibraryResponse(bookKey);
            var bookInfo = JsonSerializer.Deserialize<BookApiResponse>(bookResponse);
            foreach (var authorPayload in bookInfo.AuthorPayloads)
            {
                var authorKey = authorPayload?.AuthorKey?.Key;
                var authorInfo = await GetAuthorInfo(authorKey);
                bookInfo.Authors.Add(authorInfo);
            }
            return bookInfo;
        }

        private Task<string> GetOpenLibraryResponse(string key)
        {
            return _httpClient.GetStringAsync(OpenLibraryUrl + key + ".json");
        }
    }
}
