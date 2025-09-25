using log4net;
using SimpleInjector;
using System.Diagnostics;
using System.Reflection;

namespace FBSSOM010Notification.CompositionRoot
{
    using WBBBusinessLayer;

    public sealed class DebugLogger : ILogger
    {
        private readonly Container container;
        private readonly ILog _log;

        public DebugLogger(Container container)
        {
            this.container = container;
            _log = Log4Net();
        }

        private ILog Log4Net(StackFrame frame)
        {
            return LogManager.GetLogger(frame.GetMethod().DeclaringType);
        }

        private ILog Log4Net()
        {
            return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        #region ILog Members

        public void Info(object message)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(message);
            }
        }

        public void Info(object message, System.Exception exception)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(message, exception);
            }
        }

        public void Debug(object message, System.Exception exception)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(message, exception);
            }
        }

        public void Debug(object message)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(message);
            }
        }

        public void DebugFormat(System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public void DebugFormat(string format, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public void DebugFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void Error(object message, System.Exception exception)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(message, exception);
            }
        }

        public void Error(object message)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(message);
            }
        }

        public void ErrorFormat(System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public void ErrorFormat(string format, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public void ErrorFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void Fatal(object message, System.Exception exception)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(message, exception);
            }
        }

        public void Fatal(object message)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(message);
            }
        }

        public void FatalFormat(System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public void FatalFormat(string format, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public void FatalFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void InfoFormat(System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public void InfoFormat(string format, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public void InfoFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _log.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Warn(object message, System.Exception exception)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(message, exception);
            }
        }

        public void Warn(object message)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(message);
            }
        }

        public void WarnFormat(System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public void WarnFormat(string format, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ILoggerWrapper Members

        public log4net.Core.ILogger Logger
        {
            get { return _log.Logger; }
        }

        #endregion
    }
}
