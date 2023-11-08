namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text;

public class StartUp
{
    public static void Main()
    {
        using var db = new BookShopContext();
        DbInitializer.ResetDatabase(db);

        //string input = Console.ReadLine();
        //int input = int.Parse(Console.ReadLine());

        Console.WriteLine(RemoveBooks(db));

    }
    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
        {
            return $"{command} is not a valid restriction";
        }

        string[] bookTitles = context.Books
            .Where(b => b.AgeRestriction == ageRestriction)
            .OrderBy(b => b.Title)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    public static string GetGoldenBooks(BookShopContext context)
    {
        string edition = "gold";

        Enum.TryParse<EditionType>(edition, true, out var editionType);

        string[] bookTitles = context.Books
            .Where(b => b.EditionType == editionType &&
            b.Copies < 5000)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    public static string GetBooksByPrice(BookShopContext context)
    {
        var bookTitles = context.Books
            .Where(b => b.Price > 40)
            .OrderByDescending(b => b.Price)
            .Select(b => new
            {
                b.Title,
                b.Price
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var bookTitle in bookTitles)
        {
            sb.AppendLine($"{bookTitle.Title} - ${bookTitle.Price:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        string[] bookTitles = context.Books
            .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    public static string GetBooksByCategory(BookShopContext context, string input)
    {

        string[] categories = input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.ToLower())
            .ToArray();

        string[] bookTitles = context.Books
            .Where(b => b.BookCategories
                .Any(bc => categories
                .Contains(bc.Category.Name.ToLower())))
            .OrderBy(b => b.Title)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        DateTime inputDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var bookTitles = context.Books
            .Where(b => b.ReleaseDate < inputDate)
            .OrderByDescending(b => b.ReleaseDate)
            .Select(b => new
            {
                b.Title,
                b.EditionType,
                b.Price
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var bookTitle in bookTitles)
        {
            sb.AppendLine($"{bookTitle.Title} - {bookTitle.EditionType} - ${bookTitle.Price:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var authors = context.Authors
            .Where(a => a.FirstName.EndsWith(input))
            .Select(a => new
            {
                Name = $"{a.FirstName} {a.LastName}"
            })
            .OrderBy(a => a.Name)
            .ToArray();

        return string.Join(Environment.NewLine, authors.Select(a => a.Name));
    }

    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        string[] bookTitles = context.Books
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .OrderBy(b => b.Title)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var bookTitles = context.Books
            .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .OrderBy(b => b.BookId)
            .Select(b => new
            {
                b.Title,
                b.Author.FirstName,
                b.Author.LastName
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var bookTitle in bookTitles)
        {
            sb.AppendLine($"{bookTitle.Title} ({bookTitle.FirstName} {bookTitle.LastName})");
        }

        return sb.ToString().TrimEnd();
    }

    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        return context.Books
            .Where(b => b.Title.Length > lengthCheck)
            .Count();
    }

    public static string CountCopiesByAuthor(BookShopContext context)
    {
        var authors = context.Authors
            .Select(a => new
            {
                a.FirstName,
                a.LastName,
                TotalCopies = a.Books.Sum(b => b.Copies)
            })
            .OrderByDescending(a => a.TotalCopies)
            .ToArray();

        return string.Join(Environment.NewLine, authors.Select(a => $"{a.FirstName} {a.LastName} - {a.TotalCopies}"));
    }

    public static string GetTotalProfitByCategory(BookShopContext context)
    {
        var categoties = context.Categories
            .Select(c => new
            {
                c.Name,
                TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
            })
            .OrderByDescending(cb => cb.TotalProfit)
            .ThenBy(cb => cb.Name)
            .ToArray();

        return string.Join(Environment.NewLine, categoties.Select(c => $"{c.Name} ${c.TotalProfit:f2}"));
    }

    public static string GetMostRecentBooks(BookShopContext context)
    {
        var categories = context.Categories
            .Select(c => new
            {
                c.Name,
                Books = c.CategoryBooks
                    .Select(cb => new
                    {
                        BookTitle = cb.Book.Title,
                        BookReleaseDate = cb.Book.ReleaseDate
                    })
                    .OrderByDescending(b => b.BookReleaseDate)
                    .Take(3)
                    .ToList()
            })
            .OrderBy(cb => cb.Name)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var category in categories)
        {
            sb.AppendLine($"--{category.Name}");

            foreach (var book in category.Books)
            {
                sb.AppendLine($"{book.BookTitle} ({book.BookReleaseDate.Value.Year})");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static void IncreasePrices(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate.Value.Year < 2010)
            .ToArray();
        foreach (var book in books)
        {
            book.Price += 5;
        }
    }

    public static int RemoveBooks(BookShopContext context)
    {
        int lengthCheck = 4200;

        var deletedBooks = context.Books
            .Where(b => b.Copies < lengthCheck);

        int count = deletedBooks.Count();

        context.RemoveRange(deletedBooks);

        context.SaveChanges();

        return count;
    }
}