using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services.DTO;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public Task<IActionResult> Pay(long id);

        public Task<IActionResult> Cancel(long id);

        public IActionResult Return(VnPayCallbackDTO callback);
        // API
        public Task<IActionResult> NextStage(long id);
    }
}
