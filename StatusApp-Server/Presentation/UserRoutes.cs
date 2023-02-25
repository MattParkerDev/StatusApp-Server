using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StatusApp_Server.Application;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Presentation;

public static class UserRoutes
{
    public static void MapUserRoutes(this WebApplication app)
    {
        app.MapGet(
                "/getUser",
                async Task<Results<Ok<Profile>, NotFound>> (
                    UserService userService,
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
            .RequireAuthorization()
            .WithName("GetUser")
            .WithOpenApi();

        app.MapGet(
                "/signin",
                async Task<Results<Ok<Profile>, BadRequest, UnauthorizedHttpResult>> (
                    UserService userService,
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
            .WithName("SignIn")
            .WithOpenApi();

        app.MapGet(
                "/signOut",
                async Task<Ok> (UserService userService) =>
                {
                    await userService.SignOutAsync();
                    return TypedResults.Ok();
                }
            )
            .RequireAuthorization()
            .WithName("SignOut")
            .WithOpenApi();

        app.MapPut(
                "/createUser",
                async Task<Results<Ok<Profile>, BadRequest<IEnumerable<IdentityError>>>> (
                    UserService userService,
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
            .WithName("CreateUser")
            .WithOpenApi();

        app.MapDelete(
                "deleteUser",
                async Task<Results<Ok, BadRequest>> (
                    HttpContext context,
                    UserService userService
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
                    //TODO: Also delete Friendships
                    return TypedResults.Ok();
                }
            )
            .RequireAuthorization()
            .WithName("DeleteUser")
            .WithOpenApi();

        //TODO: Create separate route for updating Password
        app.MapPatch(
                "updateUser",
                async Task<Results<Ok<Profile>, BadRequest>> (
                    ChatContext db,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    HttpContext context,
                    UserService userService,
                    FriendshipService friendshipService,
                    string? firstName,
                    string? lastName,
                    string? status,
                    bool? online
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    //TODO: Update Friendships too
                    var targetUser = await userService.GetUserByNameAsync(userName);
                    if (targetUser is null)
                    {
                        return TypedResults.BadRequest();
                    }

                    targetUser.FirstName = firstName ?? targetUser.FirstName;
                    targetUser.LastName = lastName ?? targetUser.LastName;
                    targetUser.Status = status ?? targetUser.Status;
                    targetUser.Online = online ?? targetUser.Online;
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
            .RequireAuthorization()
            .WithName("UpdateUser")
            .WithOpenApi();
    }
}
