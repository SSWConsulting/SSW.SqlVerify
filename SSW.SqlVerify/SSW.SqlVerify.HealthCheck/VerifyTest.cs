namespace SSW.SqlVerify.HealthCheck
{
    using System;

    using SSW.HealthCheck;
    using SSW.HealthCheck.Tests;
    using SSW.SqlVerify.Core;

    /// <summary>
    /// Custom verification test for Health check UI
    /// </summary>
    public class SqlVerifyTest : FuncTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlVerifyTest"/> class.
        /// </summary>
        /// <param name="sqlVerify">The SQL verify.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        public SqlVerifyTest(ISqlVerify sqlVerify, bool isDefault = false) 
            : base(Strings.SqlVerifyTestTitle, Strings.SqlVerifyTestDescription, isDefault, null)
        {
            if (sqlVerify == null)
            {
                throw new ArgumentNullException("sqlVerify");
            }

            this.Method = testContext =>
                {
                    var result = sqlVerify.VerifyDb();
                    if (!result)
                    {
                        Assert.Fails(Strings.FailureMessage);
                    }
                };
        }
    }
}
