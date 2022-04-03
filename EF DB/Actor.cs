using System.ComponentModel.DataAnnotations;

namespace Saritasa.OMDbScrubber.Domain
{
    /// <summary>
    /// Actor who played in movie.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Id in database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of actor.
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Time of adding to database.
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.Now;

        /// <summary>
        /// Movies in which the actor played.
        /// </summary>
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
