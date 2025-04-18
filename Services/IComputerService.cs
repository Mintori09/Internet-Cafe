using InternetCafeManagementSystem.Models;

namespace InternetCafeManagementSystem.Services;

interface IComputerService
{
    IEnumerable<Computer> GetAll();
    Computer GetById(int id);
    void Add(Computer computer);
    void Update(Computer computer);
    void Delete(int id);
}