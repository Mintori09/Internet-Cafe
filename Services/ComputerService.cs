using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Models;

namespace InternetCafeManagementSystem.Services;

public class ComputerService : IComputerService
{
    private readonly ComputerDbContext _context;

    public ComputerService(ComputerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Computer> GetAll()
    {
        return _context.Computers.ToList();
    }

    public Computer GetById(int id)
    {
        return _context.Computers.Find(id);
    }

    public void Add(Computer computer)
    {
        _context.Computers.Add(computer);
        _context.SaveChanges();
    }

    public void Update(Computer computer)
    {
        _context.Computers.Update(computer);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var comp = _context.Computers.Find(id);
        if (comp != null)
        {
            _context.Computers.Remove(comp);
            _context.SaveChanges();
        }
    }
}
