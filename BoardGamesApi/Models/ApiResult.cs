using System.Collections.Generic;
using System.Linq;

namespace BoardGamesApi.Models
{
    public class ApiResult<T>
    {
        public string Error => string.Join("; ", Errors);
        public IList<string> Errors { get; set; } = new List<string>();
        public bool IsSuccess => !Errors.Any();

        public T Data { get; set; }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}
