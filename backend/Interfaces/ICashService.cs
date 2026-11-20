using backend.DTO.CashIn;
using backend.DTO.CashOut;
using backend.DTOs.Pagination;
using backend.Helper;

namespace backend.Interfaces;

public interface ICashService
{
    Task<CashInResponseDto> CashInAsync(CashInDto dto);

    Task<PagedResult<CashInListDto>> GetAllCashInAsync(PaginationRequest request, 
        int? year = null, int? month = null);

    Task<bool> CashOutAsync(CashOutDto dto);

    Task<PagedResult<CashOutDto>> GetAllCashOutAsync(PaginationRequest request, 
        int? year = null, int? month = null);

}
