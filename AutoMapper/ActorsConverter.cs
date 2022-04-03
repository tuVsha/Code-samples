using AutoMapper;
using Saritasa.OMDbScrubber.Domain;

namespace Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper
{
    /// <summary>
    /// Provides converting string of actors name to list <see cref="Actor"/>.
    /// </summary>
    internal class ActorsConverter : AutoMapper.ITypeConverter<string, ICollection<Actor>>
    {
        /// <summary>
        /// String that separate the actors in <see cref="OmdbMovie.Actors"/>.
        /// </summary>
        private const string ActorsSeparator = ", ";

        /// <summary>
        /// Main method to execute converting.
        /// </summary>
        /// <param name="source">String to take actors from.</param>
        /// <param name="destination">List to place actors.</param>
        /// <returns>Ready list of actors.</returns>
        public ICollection<Actor> Convert(string source, ICollection<Actor> destination, ResolutionContext context)
        {
            if (destination == null)
            {
                destination = new List<Actor>();
            }
            var actorsString = source.Split(ActorsSeparator).ToList();
            foreach (var actor in actorsString)
            {
                destination.Add(new Actor() { Name = actor });
            }
            return destination;
        }
    }
}
