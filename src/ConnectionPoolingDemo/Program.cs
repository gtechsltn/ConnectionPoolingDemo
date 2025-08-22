namespace ConnectionPoolingDemo;

//
// Filename: Program.cs
// Description: A C# Console Application demonstrating programmatic control over
//              SQL Server connection pooling.
//
// This program opens and closes multiple connections to show the effect of
// pooling, and then uses SqlConnection.ClearPool and SqlConnection.ClearAllPools
// to programmatically empty the connection pool.
//

/*
 * SELECT
 *     c.session_id,
 *     s.login_name,
 *     s.host_name,
 *     c.connect_time,
 *     s.program_name
 * FROM sys.dm_exec_connections AS c
 * JOIN sys.dm_exec_sessions AS s
 *     ON c.session_id = s.session_id
 * --WHERE s.program_name LIKE '%ConnectionPoolingDemo%';
 */

using System;
using System.Threading;

using Microsoft.Data.SqlClient;

class Program
{
    // IMPORTANT: Replace this with your actual connection string.
    // The "Pooling=true" and "Min/Max Pool Size" parameters are crucial
    // for enabling and configuring connection pooling.
    private const string ConnectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Pooling=True;Max Pool Size=20;Min Pool Size=5;";

    static void Main(string[] args)
    {
        Console.WriteLine("--- SQL Connection Pooling Demonstration ---");
        Console.WriteLine();
        Console.WriteLine($"Connection String: {ConnectionString}");
        Console.WriteLine();

        // Part 1: Demonstrate Connection Pooling in action
        // We'll open and close several connections.
        // Because pooling is enabled, the connections are not physically
        // closed and destroyed. Instead, they are returned to the pool
        // for reuse.
        Console.WriteLine("Part 1: Opening and closing 10 connections to utilize the pool.");
        for (int i = 0; i < 10; i++)
        {
            // The 'using' statement ensures the connection is closed and
            // returned to the pool, even if an exception occurs.
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    Console.WriteLine($"  -> Opening connection #{i + 1}");
                    connection.Open();
                    Console.WriteLine($"  <- Connection #{i + 1} opened. State: {connection.State}");
                    // The connection is automatically closed and returned
                    // to the pool when the 'using' block ends.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return; // Exit if a connection fails
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("All connections have been returned to the connection pool.");
        Console.WriteLine("The pool now holds the connections ready for reuse.");
        Thread.Sleep(2000); // Wait for a moment to show the state change

        Console.WriteLine("\n-------------------------------------------");

        // Part 2: Programmatically clear the connection pool
        // This is how you can force the pool to be emptied.
        // This is useful in scenarios where you need to discard all connections,
        // for example, after a database maintenance operation or connection error.

        Console.WriteLine("\nPart 2: Clearing the connection pool programmatically.");
        try
        {
            // ClearPool is used to clear a specific connection pool identified by
            // its connection string. This is the most common use case.
            Console.WriteLine("  -> Using SqlConnection.ClearPool...");
            SqlConnection.ClearPool(new SqlConnection(ConnectionString));
            Console.WriteLine("  <- Connection pool cleared for the specified connection string.");

            // To clear all pools managed by the application, you can use ClearAllPools.
            // This will remove all connections from all pools. Use this with caution.
            Console.WriteLine("\n  -> Using SqlConnection.ClearAllPools...");
            SqlConnection.ClearAllPools();
            Console.WriteLine("  <- All connection pools cleared for this application.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while clearing the pool: {ex.Message}");
        }

        Console.WriteLine("\n-------------------------------------------");

        // Part 3: Show that the pool is now empty
        // The next time we try to open a connection, a new physical connection
        // will need to be established with the database, which can be slower.
        Console.WriteLine("\nPart 3: Opening a new connection after the pool was cleared.");
        using (var connection = new SqlConnection(ConnectionString))
        {
            try
            {
                Console.WriteLine("  -> Opening a new connection (should be a new physical connection).");
                connection.Open();
                Console.WriteLine("  <- New connection opened successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        Console.WriteLine("\n--- Demonstration Complete ---");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}

