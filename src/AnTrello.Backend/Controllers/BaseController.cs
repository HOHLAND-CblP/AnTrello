using Microsoft.AspNetCore.Mvc;

namespace MoinBackend.Controllers;

public class BaseController : ControllerBase
{
    internal long UserId => long.Parse(User.FindFirst("sid").Value);
    //internal string Username => User.FindFirst("Username").Value;
}