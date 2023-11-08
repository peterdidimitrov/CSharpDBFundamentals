namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class StartUp
{
    public static void Main()
    {
        using var db = new BookShopContext();
        //DbInitializer.ResetDatabase(db);

        string input = Console.ReadLine();
        //int input = int.Parse(Console.ReadLine());

        Console.WriteLine(GetAuthorNamesEndingIn(db, input));

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
}

