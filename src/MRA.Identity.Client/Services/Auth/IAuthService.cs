using MRA.BlazorComponents.HttpClient.Responses;
using MRA.Identity.Application.Contract.User.Commands.ChangePassword;
using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;
using MRA.Identity.Application.Contract.User.Commands.ResetPassword;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNymber;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Client.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterUserAsync(RegisterUserCommand command);
    Task<bool> LoginUserAsync(LoginUserCommand command);
    Task<bool> ChangePassword(ChangePasswordUserCommand command);
    Task<bool> IsAvailableUserPhoneNumber(IsAvailableUserPhoneNumberQuery query);
    Task<bool> ResetPassword(ResetPasswordCommand command);
    Task<UserDetailsResponse> CheckUserDetails(CheckUserDetailsQuery checkUserDetailsQuery);
    Task ResendVerificationEmail();
    Task SendVerificationEmailToken(string token, string userId);
}