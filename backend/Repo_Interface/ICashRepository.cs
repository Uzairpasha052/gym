using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;

namespace backend.Repo_Interface;

public interface ICashRepository
{
    Task<Player?> GetPlayerByIdAsync(int playerId);
    Task<CashIn> CreateCashInAsync(CashIn cashIn);
    Task<CashOut> CreateCashOutAsync(CashOut cashOut);

    Task<PagedResult<CashIn>> GetCashInsAsync(PaginationRequest request, DateTime start, DateTime end);
    Task<PagedResult<CashOut>> GetCashOutsAsync(PaginationRequest request, DateTime start, DateTime end);
    //    Task<List<CashIn>> GetCashInsAsync(DateTime start, DateTime end);
    //    Task<List<CashOut>> GetCashOutsAsync(DateTime start, DateTime end);
}
