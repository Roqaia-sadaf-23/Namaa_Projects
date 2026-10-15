namespace Namaa.Api.Contracts.Expenses;public sealed record UpdateExpenseRequest(DateTime ExpenseDate,string Category,string Description,decimal Amount,long? ProjectId);
