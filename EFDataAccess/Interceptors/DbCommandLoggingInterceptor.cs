using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using EFDataAccess.Logging;

namespace EFDataAccess.Interceptors
{
    public class DbCommandLoggingInterceptor : DbCommandInterceptor
    {
        private ILogger _logger = new Logger();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

            base.ScalarExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

                _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQL Database", "SchoolInterceptor.ScalarExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
            }
            base.ScalarExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

            base.NonQueryExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQL Database", "SchoolInterceptor.NonQueryExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
            }
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

            base.ReaderExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("False"))
            { return; }

            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQL Database", "SchoolInterceptor.ReaderExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
            }
            base.ReaderExecuted(command, interceptionContext);
        }
    }
}