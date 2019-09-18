# SSW.SqlVerify

This library can help protect your application's database schema in the following scenario:
- You have developed a database-backed Application for a client - and have handed that application over to that client.
- You have an automated process to publish your DB schema changes (eg. EF Migrations or DbUP)
- You want to verify that no SQL schema changes have been made outside of the automated process.

In this scenario, SqlVerify can detect that schema changes have been made.

# How It Works

1. Add SqlVerify to your schema deployment process. Examples are included for EF6 migrations and DbUP
1. After pushlishing, SqlVerify will build a hash value based on your db schema - and write to a table in your DB
1. Run the Verify operation to recalculate the hash for the current schemna and compare with the stored hash.
1. SqlVerify can tell you that unexpected changes were made, but it can't tell you what was changed.
