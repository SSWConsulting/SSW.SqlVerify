namespace SSW.SqlVerify.EF
{
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using SSW.SqlVerify.Core;

    /// <summary>
    /// An implementation of <see cref="T:System.Data.Entity.IDatabaseInitializer`1"/> that will use Code First Migrations
    ///     to update the database to the latest version.
    ///     This replaces the initializer provided by EF so that we can inject the configuration via constructor
    /// </summary>
    /// <typeparam name="TContext">
    /// Database context
    /// </typeparam>
    public class UpdateMigrationsInitializer<TContext> : IDatabaseInitializer<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// The migration configuration.
        /// </summary>
        private readonly DbMigrationsConfiguration<TContext> _config;

        /// <summary>
        /// The sql verify manager.
        /// </summary>
        private readonly ISqlVerify _sqlVerify;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMigrationsInitializer{TContext}"/> class. 
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public UpdateMigrationsInitializer(DbMigrationsConfiguration<TContext> config)
        {
            this._config = config;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMigrationsInitializer{TContext}" /> class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="sqlVerify">The SQL verify.</param>
        public UpdateMigrationsInitializer(DbMigrationsConfiguration<TContext> config, ISqlVerify sqlVerify)
        {
            this._config = config;
            this._sqlVerify = sqlVerify;
        }

        /// <inheritdoc />
        public void InitializeDatabase(TContext context)
        {
            var migrator = new DbMigrator(this._config);
            var pendingMigrations = migrator.GetPendingMigrations();
            if (!pendingMigrations.Any())
            {
                return;
            }

            // Run update
            migrator.Update();
            if (this._sqlVerify != null)
            {
                this._sqlVerify.UpdateDb();
            }
        }
    }
}