using API.DAL.Models;

namespace API.DAL.Repositories.Abstraction;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetRecipesAsync();
    
}