using System;

namespace Clinicks.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        int? GetCurrentUserId();
    }
}
