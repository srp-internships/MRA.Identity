using MRA.BlazorComponents.HttpClient.Responses;
using MRA.Identity.Application.Contract.User.Commands.ChangePassword;
using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;
using MRA.Identity.Application.Contract.User.Commands.ResetPassword;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNymber;

namespace MRA.Identity.Client.Services.Auth;

public interface IAuthService
{
    Task<string> RegisterUserAsync(RegisterUserCommand command);
    Task<string> LoginUserAsync(LoginUserCommand command);
    Task<ApiResponse> ChangePassword(ChangePasswordUserCommand command);
    Task<ApiResponse<bool>> IsAvailableUserPhoneNumber(IsAvailableUserPhoneNumberQuery query);
    Task<ApiResponse> ResetPassword(ResetPasswordCommand command);
    Task<ApiResponse> CheckUserName(string userName);
    Task<ApiResponse> CheckUserDetails(CheckUserDetailsQuery checkUserDetailsQuery);
    Task<ApiResponse> ResendVerificationEmail();
    Task SendVerificationEmailToken(string token, string userId);
}