using Microsoft.Extensions.Logging;

namespace WonderLab.Services.Download;

public sealed class UpdateService {
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(ILogger<UpdateService> logger) {
        _logger = logger;
    }
}