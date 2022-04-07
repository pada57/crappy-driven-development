using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Password.Test;

public class Challenge
{
    private readonly string _input = GetResourceAsString(typeof(Challenge)
        .Assembly, "passwords.txt");

    // Exercise from AOC 2020 : https://adventofcode.com/2020/day/2
    [Fact]
    public void lnkdfjsflsdnjsdnjlfdnj() // this is doing some
    {
        CountValidPasswords(Split(_input))
            .Should()
            .Be(622);
    }

    public IEnumerable<string> Split(string s)
    {
        return s.Split(Environment.NewLine);
    }

    public bool IsValid(PasswordWithPolicy passwordWithPolicy) =>
        passwordWithPolicy?.Range
            .Contains(passwordWithPolicy
                .Password
                .Count(p => p == passwordWithPolicy.Letter)
            ) ?? false;

    public int CountValidPasswords(IEnumerable<string> lines)
    {
        var type = typeof(Challenge).GetMethod("IsValid");
        return lines
               .Select(line => ToPasswordWithPolicy(line))
               .Count(p => type.Invoke(this, new object[] { p }) as bool? ?? false);
    }
    private static readonly Regex PasswordRegex = new(@"(\d+)-(\d+) ([a-z]): ([a-z]+)");

    public static PasswordWithPolicy ToPasswordWithPolicy(string input) =>
        PasswordRegex.Matches(input)
            .ToList()
            .Select(ToPasswordWithPolicy)
            .Single();

    private static IEnumerable<int> ToRange(Match match)
    {
        var start = int.Parse(match.Groups[1].Value);
        var end = int.Parse(match.Groups[2].Value);

        return Enumerable.Range(start, end - start + 1);
    }

    private static PasswordWithPolicy ToPasswordWithPolicy(Match match) =>
        new PasswordWithPolicy(
            Password: match.Groups[4].Value,
            Range: ToRange(match),
            Letter: match.Groups[3].Value.First());

    public record PasswordWithPolicy(string Password, IEnumerable<int> Range, char Letter);
    public static string GetResourceAsString(Assembly assembly, string relativeResourcePath)
    {
        ArgumentNullException.ThrowIfNull(relativeResourcePath);
        var resourcePath = $"{Regex.Replace(assembly.ManifestModule.Name, @"\.(exe|dll)$", string.Empty, RegexOptions.IgnoreCase)}.{relativeResourcePath}";

        var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null)
            throw new ArgumentException($"The specified embedded resource {relativeResourcePath} is not found.");

        return new StreamReader(stream).ReadToEnd();
    }
}