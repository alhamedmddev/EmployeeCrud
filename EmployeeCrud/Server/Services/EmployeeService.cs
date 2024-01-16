using EmployeeCrud.Client.Pages;
using EmployeeCrud.Server.Data;
using EmployeeCrud.Server.Iservices;
using EmployeeCrud.Shared.Model;
using EmployeeCrud.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Server.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DatabaseContext _dbContext;
        public EmployeeService(DatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        private Employee EmpVMtoModelWraper(EmployeeVM employeeVM)
        {
            return new Employee
            {
                Id = employeeVM.Id,
                FirstName = employeeVM.FirstName,
                LastName = employeeVM.LastName,
                Email = employeeVM.Email,
                PhoneNo = employeeVM.PhoneNo,
                Designation = employeeVM.Designation,
            };
        }
        private EmployeeVM EmpModeltoVMWraper(Employee emp) {

            return new EmployeeVM
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                PhoneNo = emp.PhoneNo,
                Designation = emp.Designation,
            };
        }


        public async Task<(bool IsGetSuccess, string message)> DeleteEMployeeAsync(int Id)
        {
            try
            {
                var employee = _dbContext.Employee.Find(Id);
                if (employee != null)
                {
                    _dbContext.Employee.Remove(employee);
                    _dbContext.SaveChanges();
                    return (true, "Employee Deleted");
                }
                else
                {
                    return (false, "Employee not found");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public async Task<(bool IsGetSuccess, List<EmployeeVM> employeeVM)> GetEmployeeAsync()
        {
            try
            {
                
               
                var employee = await _dbContext.Employee.ToListAsync();
                

                if (employee != null)
                {
                   var empVM = employee.Select(e => new EmployeeVM
                   {
                       Id = e.Id,
                       FirstName = e.FirstName,
                       LastName = e.LastName,
                       Email = e.Email,
                       PhoneNo = e.PhoneNo,
                       Designation = e.Designation,
                   }).ToList();
                    return (true, empVM);
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {

                return (false, null);
            }
        }

        public async Task<(bool IsGetSuccess, EmployeeVM employeeVM)> GetEmployeeByIDAsync(int Id)
        {
            try
            {
                var employee = await _dbContext.Employee.FindAsync(Id);
                if (employee != null)
                {
                    
                    return (true, EmpModeltoVMWraper(employee));
                }
                else
                {
                    return (false, EmpModeltoVMWraper(employee));
                }
            }
            catch (Exception )
            {

                throw;
            }
        }

        public async Task<(bool IsEmpSaved, string message)> SaveNewEmpAsync(EmployeeVM employeeVM)
        {
            try
            {
                var isEmployeeExist = _dbContext.Employee.Any(u => u.Email.ToLower() == employeeVM.Email.ToLower());
                if (isEmployeeExist)
                {
                    return (false, "Email Address Already Registered");
                }
                await _dbContext.Employee.AddAsync(EmpVMtoModelWraper(employeeVM));
                _dbContext.SaveChanges();
                return (true, "Success");
            }
            catch (Exception ex)
            {

                return (false, ex.Message);
            }
        }

        public async Task<(bool IsEmpSaved, string message)> UpdateEmpAsync(EmployeeVM employeeVM)
        {
            try
            {
                _dbContext.Entry(EmpVMtoModelWraper(employeeVM)).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return (true, "Success");
            }
            catch (Exception ex) 
            {
                return (false, ex.Message);
            }
        }
    }
}
