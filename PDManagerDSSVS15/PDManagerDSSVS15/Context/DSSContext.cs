
using PDManagerDSSVS15.Entities;
using System.Data.Entity;

namespace PDManagerDSSVS15.Context
{
    /// <summary>
    /// DSS Context
    /// A small Context in order to store dss models
    /// This implementationj is a inmemory database only for testing
    /// </summary>
    public class DSSContext : DbContext
    {

        /// <summary>
        /// Constructor
        /// </summary>        
        public DSSContext()      
        { }


        /// <summary>
        /// DSS Models
        /// </summary>
        public DbSet<DSSModel> DSSModels { get; set; }

        /// <summary>
        /// Aggregation Models
        /// </summary>
        public DbSet<AggrModel> AggrModels { get; set; }

        /// <summary>
        /// Alert Models
        /// </summary>
        public DbSet<AlertModel> AlertModels { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DSSModel>().HasKey(m => m.Id);
            modelBuilder.Entity<AggrModel>().HasKey(m => m.Id);
            modelBuilder.Entity<AlertModel>().HasKey(m => m.Id);
            base.OnModelCreating(modelBuilder);
        }

        
    }
}