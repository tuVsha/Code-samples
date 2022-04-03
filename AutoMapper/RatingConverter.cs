using AutoMapper;

namespace Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper
{
    /// <summary>
    /// Provides converting rating of movie from string to decimal.
    /// </summary>
    internal class RatingConverter : AutoMapper.ITypeConverter<string, decimal?>
    {
        /// <summary>
        /// OMDb tag means information not available.
        /// </summary>
        private string NotAvailable;

        public RatingConverter(string omdbNotAvailableString)
        {
            this.NotAvailable = omdbNotAvailableString;
        }

        /// <summary>
        /// Main method to execute converting.
        /// </summary>
        /// <param name="source">String to take rating from.</param>
        /// <param name="destination">Instance to place rating.</param>
        /// <returns>Ready rating in decimal format.</returns>
        public decimal? Convert(string source, decimal? destination, ResolutionContext context)
        {
            if (source == NotAvailable)
            {
                destination = null;
            }
            else
            {
                destination = System.Convert.ToDecimal(source);
            }
            return destination;
        }
    }
}
