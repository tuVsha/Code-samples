using Microsoft.EntityFrameworkCore;
using Saritasa.OMDbScrubber.Domain;
using Saritasa.OMDbScrubber.Domain.Interfaces;

namespace Saritasa.OMDbScrubber.DataAccess
{
    /// <summary>
    /// Database context for saving movies and actors.
    /// </summary>
    public class AppDbContext : DbContext, IMovieDbContext
    {
        /// <inheritdoc/>
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        /// <inheritdoc/>
        public DbSet<Movie> Movies { get; private set; }

        /// <inheritdoc/>
        public DbSet<Actor> Actors { get; private set; }
    }
}