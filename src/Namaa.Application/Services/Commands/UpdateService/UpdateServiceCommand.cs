using MediatR;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Commands.UpdateService;

public sealed record UpdateServiceCommand(long Id, string Name, decimal DefaultPrice, string? Category, string? Description) : IRequest<Result<ServiceDto>>;

