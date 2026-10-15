using MediatR;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Commands.CreateService;

public sealed record CreateServiceCommand(string Name, decimal DefaultPrice, string? Category, string? Description) : IRequest<Result<ServiceDto>>;

