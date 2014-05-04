namespace SSW.SqlVerify.DbUp
{
    using global::DbUp.Engine;

    using SSW.SqlVerify.Core;

    public static class UpgradeEngineExtensions
    {
        /// <summary>
        /// Performs the upgrade.
        /// </summary>
        /// <param name="upgradeEngine">The upgrade engine.</param>
        /// <param name="sqlVerify">The SQL verify.</param>
        public static void PerformUpgrade(this UpgradeEngine upgradeEngine, ISqlVerify sqlVerify)
        {
            var result = upgradeEngine.PerformUpgrade();
            if (result.Successful)
            {
                sqlVerify.UpdateDb();
            }
        }
    }
}
