using Grpc.Core;
using GrpcServices;

namespace GrpcServer.Services
{
    public class EmployeeService : GrpcServices.EmployeeService.EmployeeServiceBase
    {
        private readonly ILogger<EmployeeService> _logger;

        private readonly List<Employee> Employees = new()
        {
            new() { Name = "Igor" },
            new() { Name = "Leo" },
            new() { Name = "João" },
            new() { Name = "Beatriz" },
            new() { Name = "Eros" },
            new() { Name = "Nicholas" },
            new() { Name = "Russio" }
        };

        public EmployeeService(ILogger<EmployeeService> logger)
        {
            _logger = logger;
        }

        public override async Task SearchEmployees(IAsyncStreamReader<EmployeeRequest> requestStream, IServerStreamWriter<EmployeeReply> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var filteredEmployees = Employees
                    .Where(e => e.Name.ToLower().Contains(requestStream.Current.Name.ToLower()))
                    .Select(e => new EmployeeReply { Employee = e.Name });

                Parallel.ForEach(filteredEmployees, employee =>
                {
                    responseStream.WriteAsync(employee);
                });
            }
        }
    }
}