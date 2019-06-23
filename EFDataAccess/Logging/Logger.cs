using System;
using System.Diagnostics;
using System.Text;

namespace EFDataAccess.Logging
{
    public class Logger : ILogger
    {

        public void Information(string message)
        {
            Trace.TraceInformation(DateTime.Now + " : " + message);
        }

        public void Information(string fmt, params object[] vars)
        {
            Trace.TraceInformation(DateTime.Now + " : " + fmt, vars);
        }

        public void Information(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceInformation(DateTime.Now + " : " + FormatExceptionMessage(exception, fmt, vars));
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(DateTime.Now + " : " + message);
        }

        public void Warning(string fmt, params object[] vars)
        {
            Trace.TraceWarning(DateTime.Now + " : " + fmt, vars);
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceWarning(DateTime.Now + " : " + FormatExceptionMessage(exception, fmt, vars));
        }

        public void Error(string message)
        {
            Trace.TraceError(DateTime.Now + " : " + message);
        }

        public void Error(string fmt, params object[] vars)
        {
            Trace.TraceError(DateTime.Now + " : " + fmt, vars);
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceError(DateTime.Now + " : " + FormatExceptionMessage(exception, fmt, vars));
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, "");
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            string message = String.Concat("Component:", componentName, ";Method:", method, ";Timespan:", timespan.ToString(), ";Properties:", properties);
            Trace.TraceInformation(DateTime.Now + " : " + message);
        }

        private static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
        {
            // Simple exception formatting: for a more comprehensive version see 
            // http://code.msdn.microsoft.com/windowsazure/Fix-It-app-for-Building-cdd80df4
            var sb = new StringBuilder();
            if (vars != null)
            {
                sb.Append(string.Format(fmt, vars));
            }
            sb.Append(" Exception: ");
            sb.Append(exception.ToString());
            return sb.ToString();
        }
    }
}