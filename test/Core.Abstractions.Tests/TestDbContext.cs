using System;
using Core.PersistentStore;
using Core.PersistentStore.Auditing;
using Core.Session;
using Microsoft.EntityFrameworkCore;

namespace Core.Abstractions.Tests
{
    public class TestDbContext : CorePersistentStoreDbContext
    {
        public virtual DbSet<TestEntityOne> TestEntityOnes { get; set; }

        public virtual DbSet<TestEntityTwo> TestEntityTwos { get; set; }

        public virtual DbSet<TestEntityHasCity> TestEntityHasCities { get; set; }


        public virtual DbSet<TestEntityHasCompany> TestEntityHasCompanies { get; set; }

        public virtual DbSet<TestEntityFullAudited> TestEntityFullAuditeds { get; set; }

        public virtual DbSet<TestPublishedMessageLog> TestPublishedMessageLogs { get; set; }
        public virtual DbSet<TestConsumedMessageLog> TestConsumedMessageLogs { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }

    public class TestPublishedMessageLog : Entity<Guid>, IHasCreationTime
    {
        public string Hash { get; set; }

        public string Group { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public string TypeName { get; set; }

        public DateTimeOffset CreationTime { get; set; }
    }

    public class TestConsumedMessageLog : Entity<Guid>, IHasCreationTime
    {
        public string Hash { get; set; }

        public string Group { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public string TypeName { get; set; }

        public DateTimeOffset CreationTime { get; set; }
    }

    public class TestEntityHasCity : Entity, IMayHaveCity
    {
        public string CityId { get; set; }

        public string Name { get; set; }
    }

    public class TestEntityOne : Entity
    {
        public int TestEntityTwoId { get; set; }
    }

    public class TestEntityTwo : Entity
    {

    }

    public class TestEntityHasCompany : Entity, IMayHaveCompany, IMayHaveCity
    {
        public string Name { get; set; }

        public string CityId { get; set; }
        
        public Guid? CompanyId { get; set; }

        public string CompanyName { get; set; }
    }

    public class TestEntityFullAudited : FullAuditedEntity, IMayHaveCompany, IMayHaveCity
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Guid? CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string CityId { get; set; }
    }
}