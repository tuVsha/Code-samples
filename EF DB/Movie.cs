using System.ComponentModel.DataAnnotations;

namespace Saritasa.OMDbScrubber.Domain
{
    /// <summary>
    /// Information about OMDb Movie.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Id in database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the movie on imdb https://www.imdb.com.
        /// </summary>
        [MaxLength(10)]
        public string ImdbId { get; set; }

        /// <summary>
        /// Title of the movie.
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Genres of the movie.
        /// </summary>
        [MaxLength(50)]
        public string? Genre { get; set; }

        /// <summary>
        /// Year of release of the movie.
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Duration of the movie.
        /// </summary>
        public int? RunTimeMins { get; set; }

        /// <summary>
        /// Rating of the movie on https://www.imdb.com.
        /// </summary>
        public decimal? Rating { get; set; }

        /// <summary>
        /// Time of adding to database.
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.Now;

        /// <summary>
        /// Actors who played in the movie.
        /// </summary>
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    }
}
