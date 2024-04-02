using MRA.Identity.Application.Contract.ApplicationRoles.Commands;
using MRA.Identity.Application.Contract.ApplicationRoles.Responses;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Application.Contract.UserRoles.Queries;
using MRA.Identity.Application.Contract.UserRoles.Response;

namespace MRA.Identity.Client.Services.Roles;

public interface IRoleService
{
    Task<bool> Post(CreateRoleCommand command);
    Task<bool> Put(UpdateRoleCommand command);
    Task<bool> Delete(string roleName);
    
    Task<List<RoleNameResponse>> GetRoles();
    Task<List<UserRolesResponse>> GetUserRoles(GetUserRolesQuery query);
    Task<bool> DeleteUserRole(string userRoleSlug);
    Task<bool> PostUserRole(CreateUserRolesCommand command);
}