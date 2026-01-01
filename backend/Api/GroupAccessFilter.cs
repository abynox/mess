using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Mess.Api;

public class GroupAccessFilter : IAsyncActionFilter
{
    private readonly ICurrentUserService _currentUser;
    private readonly AppDatabaseContext _db;

    public GroupAccessFilter(ICurrentUserService currentUser, AppDatabaseContext db)
    {
        _currentUser = currentUser;
        _db = db;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        User? user = _currentUser.GetCurrentUser();
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!context.ActionArguments.TryGetValue("groupId", out var groupIdObj)
            || groupIdObj is not Guid groupId)
        {
            context.Result = new BadRequestResult();
            return;
        }

        var group = await _db.Groups
            .Include(g => g.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            context.Result = new NotFoundResult();
            return;
        }
        

        if (!group.CanBeAccessedBy(user))
        {
            context.Result = new ForbidResult();
            return;
        }
        Member member = group.Members.First(m => m.UserId == user.Id);

        // Make the group available to the controller
        context.HttpContext.Items["Group"] = group;
        context.HttpContext.Items["Member"] = member;
        context.HttpContext.Items["CurrentUser"] = user;
        await next();
    }
}
