using AxiPlus.Application.DTOs.Modules;
using AxiPlus.API.Filters;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AxiPlus.API.Endpoints;

public static class ModuleEndpoints
{        
    public static void MapModuleEndpoints(
        this WebApplication app)
   {        
        var group =
            app.MapGroup("/api/modules");
        group.AddEndpointFilter<FunctionTraceEndpointFilter>();

        group.MapGet(
            "/",
            async (AppDbContext context) =>
       {        
            var modules =
                await context.Modules
                    .OrderBy(x => x.Order)
                    .Select(x =>
                        new ModuleDto
                       {        
                            Id = x.Id,

                            Title = x.Title,

                            Description =
                                x.Description,

                            Order = x.Order,

                            IsPublished =
                                x.IsPublished,

                            LessonCount =
                                x.Lessons.Count
                        })
                    .ToListAsync();

            return Results.Ok(modules);
        });
    }
}
