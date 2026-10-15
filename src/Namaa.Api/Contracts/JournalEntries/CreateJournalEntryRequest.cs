namespace Namaa.Api.Contracts.JournalEntries;public sealed record CreateJournalEntryRequest(string EntryNumber,DateTime EntryDate,string? Description,string? SourceType,long? SourceId);
