using AxiPlus.Domain.Entities;
using AxiPlus.Infrastructure.Data;
using AxiCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.API.Services;

public class BatchAllocationService
{       
    private readonly AppDbContext _context;
    private readonly ILogger<BatchAllocationService> _logger;

    public BatchAllocationService(
        AppDbContext context,
        ILogger<BatchAllocationService> logger)
   {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Allocates a student to an available batch for the selected track or creates a new batch when capacity is full.
    /// Returns the allocated batch so student registration can persist the correct batch relationship.
    /// </summary>
    public async Task<Batch> AllocateBatchAsync(
        Track track)
   {
        using var trace = FunctionTrace.Enter(_logger, nameof(BatchAllocationService), nameof(AllocateBatchAsync));
        try
        {
            var batch = await _context.Batches
                .Where(x =>
                    x.TrackId == track.Id &&
                    x.IsActive &&
                    x.CurrentStrength < x.Capacity)
                .OrderBy(x => x.BatchNumber)
                .FirstOrDefaultAsync();

            if (batch != null)
           {
                batch.CurrentStrength++;

                await _context.SaveChangesAsync();

                return batch;
            }

            var lastBatchNumber = await _context.Batches
                .Where(x => x.TrackId == track.Id)
                .MaxAsync(x =>
                    (int?)x.BatchNumber) ?? 0;

            var nextBatchNumber =
                lastBatchNumber + 1;

            var newBatch = new Batch
           {       
                Id = Guid.NewGuid(),

                TrackId = track.Id,

                BatchNumber = nextBatchNumber,

                Name =
                    $"{track.BatchPrefix}{nextBatchNumber}",

                Level = track.Level,

                Capacity = 15,

                CurrentStrength = 1,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            _context.Batches.Add(newBatch);

            await _context.SaveChangesAsync();

            return newBatch;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
