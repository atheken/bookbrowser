namespace BookBrowser.Models.ViewModels;

public record SimpleBookView(long Id, string Title, IReadOnlyCollection<SimpleAuthorView> Authors);