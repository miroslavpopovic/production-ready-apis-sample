using System.Collections.Generic;
using BoardGamesApi.Models;

namespace BoardGamesApi.Data
{
    public interface IGamesRepository
    {
        IEnumerable<Game> GetAll();
        void Create(Game game);
        void Delete(string id);
        void Update(Game game);
    }
}