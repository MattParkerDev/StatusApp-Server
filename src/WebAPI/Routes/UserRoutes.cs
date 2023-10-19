using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Application.DTOs;
using Application.Mappers;
using Application.Services.Contracts;
using Domain.Entities;
//TODO: Remove
using Infrastructure.Persistence;
using WebAPI.SignalR;

namespace WebAPI.Routes;

public static class UserRoutes
{
    public static void MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("User").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getuser",
                async Task<Results<Ok<StatusUserDto>, NotFound>> (
                    IStatusUserService statusUserService,
                    HttpContext context
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var statusUser = await statusUserService.GetUserByNameAsync(userName);
                    if (statusUser is null)
                    {
                        return TypedResults.NotFound();
                    }

                    var statusUserDto = statusUser.ToDto();
                    return TypedResults.Ok(statusUserDto);
                }
            )
            .WithName("GetUser");

        group
            .MapGet(
                "/signin",
                async Task<Results<Ok<StatusUserDto>, BadRequest, UnauthorizedHttpResult>> (
                    IIdentityUserService userService,
                    IStatusUserService statusUserService,
                    // TODO: Basic auth
                    string userName,
                    string password
                ) =>
                {
                    var identityUser = await userService.GetUserByNameAsync(userName);
                    var statusUser = await statusUserService.GetUserByNameAsync(userName);
                    if (identityUser is null || statusUser is null)
                    {
                        return TypedResults.BadRequest();
                    }

                    var signInResult = await userService.PasswordSignInAsync(
                        identityUser,
                        password
                    );
                    return signInResult.Succeeded
                        ? TypedResults.Ok(statusUser.ToDto())
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
                async Task<
                    Results<Ok<StatusUserDto>, BadRequest, BadRequest<IEnumerable<IdentityError>>>
                > (
                    IIdentityUserService identityUserService,
                    IStatusUserService statusUserService,
                    CreateUserDto createUserDto
                ) =>
                {
                    var newIdentityUser = new User
                    {
                        UserName = createUserDto.UserName,
                        Email = createUserDto.Email,
                    };

                    var newStatusUser = new StatusUser
                    {
                        UserName = createUserDto.UserName,
                        FirstName = createUserDto.FirstName,
                        LastName = createUserDto.LastName
                    };

                    var identityResult = await identityUserService.CreateUserAsync(
                        newIdentityUser,
                        createUserDto.Password
                    );

                    if (!identityResult.Succeeded)
                    {
                        return TypedResults.BadRequest(identityResult.Errors);
                    }

                    var statusUserCreationSuccess = await statusUserService.CreateUserAsync(
                        newStatusUser
                    );

                    if (statusUserCreationSuccess is false)
                    {
                        return TypedResults.BadRequest();
                    }

                    await identityUserService.SignInAsync(newIdentityUser);
                    return TypedResults.Ok(newStatusUser.ToDto());
                }
            )
            .AllowAnonymous()
            .WithName("CreateUser");

        group
            .MapDelete(
                "deleteuser",
                async Task<Results<Ok, BadRequest>> (
                    HttpContext context,
                    IIdentityUserService userService,
                    IStatusUserService statusUserService
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var identityUser = await userService.GetUserByNameAsync(userName);
                    var statusUser = await statusUserService.GetUserByNameAsync(userName);
                    if (identityUser is null)
                    {
                        return TypedResults.BadRequest();
                    }

                    //TODO: Confirm Auth flow
                    await userService.SignOutAsync();
                    await userService.DeleteUserAsync(identityUser);
                    if (statusUser is not null)
                    {
                        await statusUserService.DeleteUserAsync(statusUser);
                    }

                    //TODO: Also delete Friendships
                    return TypedResults.Ok();
                }
            )
            .WithName("DeleteUser");

        //TODO: Create separate route for updating Password
        group
            .MapPatch(
                "updateuser",
                async Task<Results<Ok<StatusUserDto>, BadRequest>> (
                    StatusContext db,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    HttpContext context,
                    IStatusUserService statusUserService,
                    IFriendshipService friendshipService,
                    StatusUserDto statusUserDto
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    //TODO: Update Friendships too
                    var statusUser = statusUserDto.FromDto();
                    var success = await statusUserService.UpdateUserAsync(statusUser);
                    if (success is false)
                    {
                        // TODO: Review status code
                        return TypedResults.BadRequest();
                    }

                    // Push changes to this user to any of their friends
                    var friendsUserNameList = friendshipService.GetFriendsUserNameList(userName);
                    var usersToNotify = db.SignalRConnections
                        .Where(s => friendsUserNameList.Contains(s.UserName))
                        .GroupBy(s => s.UserName)
                        .Select(s => s.First().UserName)
                        .ToList();

                    await hubContext.Clients.Users(usersToNotify).ReceiveUpdatedUser(statusUserDto);
                    return TypedResults.Ok(statusUserDto);
                }
            )
            .WithName("UpdateUser");
    }
}
