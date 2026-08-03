using Support_System_API.Data;
using Support_System_API.Domain.Entities;

namespace Support_System_API.GraphQL;

public class Query 
{
    [UseProjection]
    [UseFiltering]
    [UseSorting] 
   public IQueryable<User> GetUsers([Service] AppDbContext context) => 
      context.Users;
}
