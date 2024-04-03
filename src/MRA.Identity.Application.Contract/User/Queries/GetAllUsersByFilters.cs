﻿using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Queries;

public class GetAllUsersByFilters : PagedListQuery<UserResponse>
{
    public string Skills { get; set; }
    public Guid? ApplicationId { get; set; } = null;
    public string ApplicationClientSecret { get; set; } = null;
}