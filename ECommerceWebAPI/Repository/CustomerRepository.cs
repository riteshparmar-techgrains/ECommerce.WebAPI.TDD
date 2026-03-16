using ECommerceWebAPI.Data;
using ECommerceWebAPI.Entities;
using Microsoft.EntityFrameworkCore;
namespace ECommerceWebAPI.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers
                                 .AnyAsync(x => x.Id == id);
        }


    }
}
