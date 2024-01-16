using EmployeeCrud.Shared.ViewModel;

namespace EmployeeCrud.Server.Iservices
{
    public interface IEmployeeService
    {
        Task<(bool IsEmpSaved, string message)> SaveNewEmpAsync(EmployeeVM employeeVM);
        Task<(bool IsEmpSaved, string message)> UpdateEmpAsync(EmployeeVM employeeVM);
        Task<(bool IsGetSuccess, List<EmployeeVM> employeeVM)> GetEmployeeAsync();
        Task<(bool IsGetSuccess, EmployeeVM employeeVM)> GetEmployeeByIDAsync(int Id);

        Task<(bool IsGetSuccess, string message)> DeleteEMployeeAsync(int Id);
    }
}
