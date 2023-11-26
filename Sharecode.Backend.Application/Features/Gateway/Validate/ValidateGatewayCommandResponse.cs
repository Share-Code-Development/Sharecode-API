using System.Net;
using System.Text.Json.Serialization;

namespace Sharecode.Backend.Application.Features.Gateway.Validate;

public record ValidateGatewayCommandResponse(
    [property: Newtonsoft.Json.JsonIgnore]
    [field: Newtonsoft.Json.JsonIgnore]
    [property: JsonIgnore]
    [field: JsonIgnore]
    HttpStatusCode Response,
    string Message)
{
    public static ValidateGatewayCommandResponse NotFound => new ValidateGatewayCommandResponse(HttpStatusCode.NotFound, "Unknown request");
    public static ValidateGatewayCommandResponse UserNotFound => new ValidateGatewayCommandResponse(HttpStatusCode.NotFound, "Unknown user");
    public static ValidateGatewayCommandResponse Expired => new ValidateGatewayCommandResponse(HttpStatusCode.BadRequest, "Request Expired");

    public static ValidateGatewayCommandResponse AlreadyVerified => new ValidateGatewayCommandResponse(HttpStatusCode.BadRequest, "Already Verified");
    public static ValidateGatewayCommandResponse Invalid(string message) => new ValidateGatewayCommandResponse(HttpStatusCode.BadRequest, message);

    public static ValidateGatewayCommandResponse Success => new ValidateGatewayCommandResponse(HttpStatusCode.OK, "Success");

    public static ValidateGatewayCommandResponse AccountSuspended =>
        new ValidateGatewayCommandResponse(HttpStatusCode.BadRequest, "Account has been suspended, Contact support!");
}
    
    