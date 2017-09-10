using System.Collections.Generic;
using System.Linq;

namespace BoardGamesApi.Models
{
    /// <summary>
    /// Encapsulates an API result with success flag, errors (if any) and result object.
    /// </summary>
    /// <typeparam name="T">Type of the result.</typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// Gets the API error string or empty string if there are no errors.
        /// </summary>
        public string Error => string.Join("; ", Errors);

        /// <summary>
        /// Gets the error list.
        /// </summary>
        public IList<string> Errors { get; } = new List<string>();

        /// <summary>
        /// Gets the value indicating whether the API call was successful or not.
        /// </summary>
        public bool IsSuccess => !Errors.Any();

        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Add the error to the error list.
        /// </summary>
        /// <param name="error">An error to add to errors list.</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}
