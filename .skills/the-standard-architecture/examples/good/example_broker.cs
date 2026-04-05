// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Storage Broker
// Demonstrates: local interface, no flow control, no exception handling,
// infrastructure language, async methods, owns configuration.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Brokers.Storages
{
    // arch-001: Implements a local interface
    public partial class StorageBroker : DbContext, IStorageBroker
    {
        // arch-004: Broker owns its configuration
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            Database.Migrate();
        }

        // arch-006: Infrastructure language (Insert, not Add)
        // arch-009: Async method
        // arch-002: No flow control
        // arch-003: No exception handling — raw exceptions propagate to service
        public async ValueTask<Student> InsertStudentAsync(Student student) =>
            await this.InsertAsync(student);

        // arch-006: Infrastructure language (Select, not Retrieve)
        public IQueryable<Student> SelectAllStudents() =>
            this.SelectAll<Student>();

        // arch-006: Infrastructure language (Select, not Retrieve)
        // arch-009: Async
        public async ValueTask<Student> SelectStudentByIdAsync(Guid studentId) =>
            await this.SelectAsync<Student>(studentId);

        // arch-006: Infrastructure language (Update, not Modify)
        public async ValueTask<Student> UpdateStudentAsync(Student student) =>
            await this.UpdateAsync(student);

        // arch-006: Infrastructure language (Delete, not Remove)
        public async ValueTask<Student> DeleteStudentAsync(Student student) =>
            await this.DeleteAsync(student);
    }
}

// ---------------------------------------------------------------
// Interface (also good)
// ---------------------------------------------------------------

namespace MyProject.Brokers.Storages
{
    // arch-001: Local interface pattern I{Resource}Broker
    public interface IStorageBroker
    {
        ValueTask<Student> InsertStudentAsync(Student student);
        IQueryable<Student> SelectAllStudents();
        ValueTask<Student> SelectStudentByIdAsync(Guid studentId);
        ValueTask<Student> UpdateStudentAsync(Student student);
        ValueTask<Student> DeleteStudentAsync(Student student);
    }
}
