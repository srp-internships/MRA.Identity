﻿using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Queries;
public class GetAllUsersByFilters : PagedListQuery<UserResponse>
{ 
    public string Skills { get; set; }
}
