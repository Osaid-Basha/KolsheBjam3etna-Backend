using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IServiceRequestService
    {
        Task<ApiResponse<int>> CreateAsync(string userId, CreateServiceRequestRequest request);

    }
}
