
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CinemaBookingFinal;

public sealed record BookingDto(
    int Id,
    int SessionId,
    int Seat,
    string CustomerType,
    long PaidCents,
    string Status,
    DateTime At);

public sealed record CinemaDto(
    List<Movie> Movies,
    List<Session> Sessions,
    List<BookingDto> Bookings);

public static class JsonStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private static Result<CinemaState> Validate(CinemaDto? dto)
    {
        if (dto?.Movies is null ||
            dto.Sessions is null ||
            dto.Bookings is null)
        {
            return Result.Fail<CinemaState>(
                "Missing required JSON collections");
        }

        if (dto.Movies.Any(m =>
                m is null ||
                m.Id <= 0 ||
                string.IsNullOrWhiteSpace(m.Title) ||
                string.IsNullOrWhiteSpace(m.Genre) ||
                m.DurationMinutes <= 0) ||
            dto.Movies.Where(m => m is not null)
                .GroupBy(m => m.Id)
                .Any(g => g.Count() > 1))
        {
            return Result.Fail<CinemaState>(
                "Invalid movies data");
        }

        if (dto.Sessions.Any(s =>
                s is null ||
                s.Id <= 0 ||
                s.MovieId <= 0 ||
                s.Seats <= 0 ||
                s.BasePriceCents < 0 ||
                s.StartsAt == default ||
                !dto.Movies.Any(m =>
                    m.Id == s.MovieId)) ||
            dto.Sessions.Where(s => s is not null)
                .GroupBy(s => s.Id)
                .Any(g => g.Count() > 1))
        {
            return Result.Fail<CinemaState>(
                "Invalid sessions data");
        }

        if (dto.Bookings.Any(b =>
                b is null ||
                b.Id <= 0 ||
                b.Seat <= 0 ||
                b.PaidCents < 0 ||
                b.At == default ||
                b.Status is not ("Confirmed" or "Cancelled") ||
                b.CustomerType is not ("adult" or "student" or "child") ||
                !dto.Sessions.Any(s =>
                    s.Id == b.SessionId &&
                    b.Seat <= s.Seats)) ||
            dto.Bookings.Where(b => b is not null)
                .GroupBy(b => b.Id)
                .Any(g => g.Count() > 1) ||
            dto.Bookings.Where(b =>
                    b is not null &&
                    b.Status == "Confirmed")
                .GroupBy(b => (b.SessionId, b.Seat))
                .Any(g => g.Count() > 1))
        {
            return Result.Fail<CinemaState>(
                "Invalid bookings data");
        }

        var bookings = dto.Bookings.Select(b =>
            new Booking(
                b.Id,
                b.SessionId,
                b.Seat,
                b.CustomerType,
                b.PaidCents,
                b.Status == "Confirmed"
                    ? (BookingStatus)new BookingStatus.Confirmed(b.At)
                    : new BookingStatus.Cancelled(b.At)))
            .ToImmutableArray();

        return Result.Ok(new CinemaState(
            dto.Movies.ToImmutableArray(),
            dto.Sessions.ToImmutableArray(),
            bookings));
    }

    public static Result<CinemaState> Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Result.Fail<CinemaState>(
                "Data file path missing");

        try
        {
            using var stream = File.OpenRead(path);

            var dto = JsonSerializer.Deserialize<CinemaDto>(
                stream,
                Options);

            return Validate(dto);
        }
        catch (FileNotFoundException e)
        {
            return Result.Fail<CinemaState>(
                "Data file missing: " + e.FileName);
        }
        catch (DirectoryNotFoundException e)
        {
            return Result.Fail<CinemaState>(
                "Data directory missing: " + e.Message);
        }
        catch (JsonException e)
        {
            return Result.Fail<CinemaState>(
                "Malformed JSON: " + e.Message);
        }
        catch (IOException e)
        {
            return Result.Fail<CinemaState>(
                "Read error: " + e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Result.Fail<CinemaState>(
                "Access denied: " + e.Message);
        }
    }

    public static Result<string> Save(
        string path,
        CinemaState state)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Result.Fail<string>(
                "Data file path missing");

        if (state is null)
            return Result.Fail<string>(
                "Cinema state missing");

        string? temp = null;

        try
        {
            var dto = new CinemaDto(
                state.Movies.ToList(),
                state.Sessions.ToList(),
                state.Bookings.Select(b =>
                    b.Status switch
                    {
                        BookingStatus.Confirmed c =>
                            new BookingDto(
                                b.Id,
                                b.SessionId,
                                b.Seat,
                                b.CustomerType,
                                b.PaidCents,
                                "Confirmed",
                                c.At),

                        BookingStatus.Cancelled c =>
                            new BookingDto(
                                b.Id,
                                b.SessionId,
                                b.Seat,
                                b.CustomerType,
                                b.PaidCents,
                                "Cancelled",
                                c.At),

                        _ => throw new InvalidOperationException(
                            "Unknown booking status")
                    }).ToList());

            var validation = Validate(dto);

            if (validation is Result<CinemaState>.Fail failure)
                return Result.Fail<string>(failure.Error);

            var full = Path.GetFullPath(path);

            var directory = Path.GetDirectoryName(full);

            if (string.IsNullOrWhiteSpace(directory))
                return Result.Fail<string>(
                    "Invalid data directory");

            Directory.CreateDirectory(directory);

            temp = full + "." +
                Guid.NewGuid().ToString("N") + ".tmp";

            using (var stream = new FileStream(
                temp,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                JsonSerializer.Serialize(
                    stream,
                    dto,
                    Options);

                stream.Flush(true);
            }

            File.Move(temp, full, true);

            temp = null;

            return Result.Ok(full);
        }
        catch (Exception e) when (
            e is IOException or
            UnauthorizedAccessException or
            JsonException or
            ArgumentException or
            NotSupportedException or
            InvalidOperationException)
        {
            return Result.Fail<string>(
                "Save failed: " + e.Message);
        }
        finally
        {
            if (temp is not null)
            {
                try
                {
                    if (File.Exists(temp))
                        File.Delete(temp);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }
}
