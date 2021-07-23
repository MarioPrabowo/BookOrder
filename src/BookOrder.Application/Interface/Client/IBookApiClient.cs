using BookOrder.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookOrder.Application.Interface.Client
{
    public interface IBookApiClient
    {
        public Task<BookApiResponse> GetBookInfo(string bookKey);
        public Task<BookApiResponse.AuthorInfo> GetAuthorInfo(string authorKey);
    }
}
