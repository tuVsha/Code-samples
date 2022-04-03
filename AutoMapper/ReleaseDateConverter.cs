using AutoMapper;

namespace Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper
{
    /// <summary>
    /// Provides converting release date of movie from string to dateonly.
    /// </summary>
    internal class ReleaseDateConverter : AutoMapper.ITypeConverter<string, DateTime?>
    {
        /// <summary>
        /// OMDb tag means information not available.
        /// </summary>
        private string NotAvailable;

        public ReleaseDateConverter(string omdbNotAvailableString)
        {
            this.NotAvailable = omdbNotAvailableString;
        }

        /// <summary>
        /// Main method to execute converting.
        /// </summary>
        /// <param name="source">String to take date from.</param>
        /// <param name="destination">Instance to place release date.</param>
        /// <returns>Ready release date in dateonly format.</returns>
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (source == NotAvailable)
            {
                destination = null;
            }
            else
            {
                destination = DateTime.Parse(source);
            }
            return destination;
        }
    }
}
