namespace BookBrowser.Models.ViewModels;

public record ResultPageView<T>(int Count, IEnumerable<T> Records);