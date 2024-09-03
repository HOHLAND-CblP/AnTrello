using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AnTrello.Backend.Controllers;

public class BaseController : ControllerBase
{
    internal long UserId => long.Parse(User?.FindFirst("sub").Value);
    internal string UserEmail => User?.FindFirst(JwtRegisteredClaimNames.Email).Value;
}