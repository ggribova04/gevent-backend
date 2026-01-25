using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public interface IServiceService
{
    Task<PerformerDto> AddByLoginAsync(AddPerformerByLoginDto dto);
    Task<List<User>> SearchPerformersAsync(PerformerSearchDto dto);
    Task<PerformerDto> AddFoundPerformerAsync(PerformerChoiceDto dto);
    Task<List<PerformerDto>> GetPerformersByEventIdAsync(int eventId);
    Task<PerformerStatusUpdateResultDto> UpdatePerformerStatusAsync(UpdatePerformerStatusDto dto);
    Task<List<PerformerDto>> GetAllUserPerformersAsync(int userId, int userRoleId);
}