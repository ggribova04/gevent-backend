public interface ITenderService
{
  System.Threading.Tasks.Task<int> CreateTenderAsync(CreateTenderRequest request);
  System.Threading.Tasks.Task CreateResponseAsync(int tenderId, int employeeId, CreateTenderResponseRequest request);
  System.Threading.Tasks.Task<IEnumerable<TenderResponseDto>> GetResponsesAsync(int tenderId);
  System.Threading.Tasks.Task DeleteResponseAsync(int responseId);
  System.Threading.Tasks.Task CloseTenderAsync(int tenderId, int? winnerResponseId);
  System.Threading.Tasks.Task<TenderResponseDto?> GetResponseForEmployeeAsync(int tenderId, int employeeId);
  System.Threading.Tasks.Task UpdateResponseAsync(int tenderId, int employeeId, CreateTenderResponseRequest request);
  System.Threading.Tasks.Task UpdateTenderAsync(int tenderId, CreateTenderRequest request);
  System.Threading.Tasks.Task DeleteTenderAsync(int tenderId);
  System.Threading.Tasks.Task<TenderDto?> GetTenderByIdAsync(int tenderId);
  System.Threading.Tasks.Task ToggleResponseSelectedAsync(int responseId);
  System.Threading.Tasks.Task<IEnumerable<TenderDto>> GetTendersByUserAsync(int userId);
}
