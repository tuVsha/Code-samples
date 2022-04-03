using AutoMapper;

namespace Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper
{
    /// <summary>
    /// Provides converting runtime of movie from string to int.
    /// </summary>
    internal class RuntimeConverter : AutoMapper.ITypeConverter<string, int?>
    {
        /// <summary>
        /// Length of the end of runtime string from OMDb response.
        /// </summary>
        private const int LengthOfMin = 4;

        /// <summary>
        /// OMDb tag means information not available.
        /// </summary>
        private string NotAvailable;

        public RuntimeConverter(string omdbNotAvailableString)
        {
            this.NotAvailable = omdbNotAvailableString;
        }

        /// <summary>
        /// Main method to execute converting.
        /// </summary>
        /// <param name="source">String to take runtime from.</param>
        /// <param name="destination">Instance to place runtime.</param>
        /// <returns>Ready runtime in int format.</returns>
        public int? Convert(string source, int? destination, ResolutionContext context)
        {
            if (source == NotAvailable)
            {
                destination = null;
            }
            else
            {
                var mins = source.Remove(source.Length - LengthOfMin);
                destination = int.Parse(mins);
            }
            return destination;
        }
    }
}
