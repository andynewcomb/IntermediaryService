﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediaryService.Tests.HelperMockClasses
{
    class MockLogger : ILogger
    {
        private IList<string> logs;

        public MockLogger()
        {
            logs = new List<string>();            
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            logs.Add(message);
        }

        public IList<string> GetLogs()
        {
            return logs;
        }
    }
}







    
        
        
        

        