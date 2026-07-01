using AxiPlus.Application.DTOs.Tracks;
using AxiPlus.API.Filters;
using AxiPlus.Domain.Entities;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AxiPlus.API.Endpoints;

public static class TrackEndpoints
{
    public static void MapTrackEndpoints(
        this WebApplication app)
   {
        var group = app.MapGroup("/api/tracks");
        group.AddEndpointFilter<FunctionTraceEndpointFilter>();

        // CREATE TRACK
        group.MapPost("/", async (
            CreateTrackDto dto,
            AppDbContext context) =>
       {
            var track = new Track
           {        
                Name = dto.Name,

                Level = dto.Level,

                IsActive = true,

                CreatedAt = DateTime.UtcNow,

                BatchPrefix = dto.BatchPrefix

            };

            context.Tracks.Add(track);

            await context.SaveChangesAsync();

            return Results.Ok();
        });

        // GET TRACKS
        group.MapGet("/", async (
            AppDbContext context) =>
       {
            var tracks = await context.Tracks
                .Select(x => new TrackDto
               {        
                    Id = x.Id,

                    Name = x.Name,

                    Level = x.Level,

                    IsActive = x.IsActive,

                    BatchPrefix = x.BatchPrefix
                })
                .ToListAsync();

            return Results.Ok(tracks);
        });
    }
}
