using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Presentation;

public static class UserRoutes
{
    public static void MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("User").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getuser",
                async Task<Results<Ok<Profile>, NotFound>> (
                    IIdentityUserService userService,
                    HttpContext context
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var user = await userService.GetUserByNameAsync(userName);
                    if (user is null)
                    {
                        return TypedResults.NotFound();
                    }

                    var profile = user.ToProfile();
                    return TypedResults.Ok(profile);
                }
            )
            .WithName("GetUser");

        group
            .MapGet(
                "/signin",
                async Task<Results<Ok<Profile>, BadRequest, UnauthorizedHttpResult>> (
                    IIdentityUserService userService,
                    string userName,
                    string password
                ) =>
                {
                    var user = await userService.GetUserByNameAsync(userName);
                    if (user == null)
                    {
                        return TypedResults.BadRequest();
                    }

                    var signInResult = await userService.PasswordSignInAsync(user, password);
                    return signInResult.Succeeded
                        ? TypedResults.Ok(user.ToProfile())
                        : TypedResults.Unauthorized();
                }
            )
            .AllowAnonymous()
            .WithName("SignIn");

        group
            .MapGet(
                "/signout",
                async Task<Ok> (IIdentityUserService userService) =>
                {
                    await userService.SignOutAsync();
                    return TypedResults.Ok();
                }
            )
            .WithName("SignOut");

        group
            .MapPut(
                "/createuser",
                async Task<Results<Ok<Profile>, BadRequest<IEnumerable<IdentityError>>>> (
                    IIdentityUserService userService,
                    string userName,
                    string password,
                    string firstName,
                    string lastName,
                    string email
                ) =>
                {
                    var newUser = new User
                    {
                        UserName = userName,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName
                    };

                    var result = await userService.CreateUserAsync(newUser, password);

                    if (!result.Succeeded)
                    {
                        return TypedResults.BadRequest(result.Errors);
                    }

                    await userService.SignInAsync(newUser);
                    return TypedResults.Ok(newUser.ToProfile());
                }
            )
            .AllowAnonymous()
            .WithName("CreateUser");

        group
            .MapDelete(
                "deleteuser",
                async Task<Results<Ok, BadRequest>> (
                    HttpContext context,
                    IIdentityUserService userService
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var targetUser = await userService.GetUserByNameAsync(userName);
                    if (targetUser is null)
                    {
                        return TypedResults.BadRequest();
                    }

                    //TODO: Confirm Auth flow
                    await userService.SignOutAsync();
                    await userService.DeleteUserAsync(targetUser);
                    //TODO: HERE - Delete StatusUser
                    //TODO: Also delete Friendships
                    return TypedResults.Ok();
                }
            )
            .WithName("DeleteUser");

        //TODO: Create separate route for updating Password
        group
            .MapPatch(
                "updateuser",
                async Task<Results<Ok<Profile>, BadRequest>> (
                    StatusContext db,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    HttpContext context,
                    IIdentityUserService userService,
                    IFriendshipService friendshipService,
                    string? firstName,
                    string? lastName,
                    string? status,
                    bool? online
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    //TODO: Update Friendships too
                    //TODO: HERE - create /updatestatususer
                    var targetUser = await userService.GetUserByNameAsync(userName);
                    if (targetUser is null)
                    {
                        return TypedResults.BadRequest();
                    }

                    targetUser.FirstName = firstName ?? targetUser.FirstName;
                    targetUser.LastName = lastName ?? targetUser.LastName;
                    targetUser.Status = status ?? targetUser.Status;
                    targetUser.Online = online ?? targetUser.Online;
                    // TODO: HERE
                    await userService.UpdateUserAsync(targetUser);

                    var updatedProfile = targetUser.ToProfile();

                    // Push changes to this user to any of their friends
                    var friendsUserNameList = friendshipService.GetFriendsUserNameList(userName);
                    var usersToNotify = db.Connections
                        .Where(s => friendsUserNameList.Contains(s.UserName))
                        .GroupBy(s => s.UserName)
                        .Select(s => s.First().UserName)
                        .ToList();

                    await hubContext.Clients
                        .Users(usersToNotify)
                        .ReceiveUpdatedUser(updatedProfile);
                    return TypedResults.Ok(updatedProfile);
                }
            )
            .WithName("UpdateUser");
    }
}
