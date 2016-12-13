/**
 * Copyright 2011-2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using System.Threading;
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Commons.Diagnostics.NamedPipeServer;
using log4net;

namespace biz.dfch.CS.Logging.Service.Core
{
    public partial class LoggingService : ServiceBase
    {
        private const int ADDITIONAL_TIME_MS = 10 * 10000;
        private const int DEQUEUE_WAIT_TIME_MS = 500;

        private readonly ManualResetEventSlim abortEvent = 
            new ManualResetEventSlim(false);

        private NamedPipeServerTraceWriter server;

        public LoggingService()
        {
            CanPauseAndContinue = false;
            CanShutdown = true;
            InitializeComponent();

            // as we are using "configSource" in the app.config
            // we cannot use the standard assembly directive for log4net
            // but we have to use the explicit "Configure" statement
            log4net.Config.XmlConfigurator.Configure();
        }

        public void TerminateInteractiveService()
        {
            abortEvent.Set();
        }

        public void OnStartInteractive(string[] args)
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            try
            {
                Console.WriteLine(new ProgramHelp().GetInteractiveMessage());

                OnStart(args);
                abortEvent.Wait();
                Console.WriteLine(Messages.LoggingServiceOnStartInteractiveCancelKeyPress);
            }
            catch (Exception ex)
            {
                var message = string.Format(Messages.LoggingServiceOnStartInteractiveException, ex.GetType().Name, ex.Source, Messages.LoggingService_OnStartInteractive_Stopping_interactive_mode, ex.Message, ex.StackTrace);
                Console.WriteLine(message);
            }
            finally
            {
                OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnStart(args);

            var pipeName = 2 <= args.Length
                ? args[1].Trim()
                : string.Empty;

            server = string.IsNullOrWhiteSpace(pipeName) 
                ? new NamedPipeServerTraceWriter() 
                : new NamedPipeServerTraceWriter(pipeName);

            ThreadPool.QueueUserWorkItem(ServerProc);

            if (!Program.IsInteractiveStartup)
            {
                this.RequestAdditionalTime(ADDITIONAL_TIME_MS);
            }
        }

        public void ServerProc(object stateInfo)
        {
            Contract.Requires(null == stateInfo);

            for (;;)
            {
                if (abortEvent.IsSet)
                {
                    break;
                }
                    
                string composedMessage;
                var result = server.Messages.TryDequeue(out composedMessage);
                if (!result)
                {
                    Thread.Sleep(DEQUEUE_WAIT_TIME_MS);

                    continue;
                }

                var pipeMessage = new PipeMessage(composedMessage);
                if (!pipeMessage.IsValid())
                {
                    continue;
                }

                var logger = LogManager.GetLogger(pipeMessage.Source);

                switch (pipeMessage.TraceEventType)
                {
                    case TraceEventType.Critical:
                        logger.Fatal(pipeMessage.Message);
                        break;
                    case TraceEventType.Error:
                        logger.Error(pipeMessage.Message);
                        break;
                    case TraceEventType.Warning:
                        logger.Warn(pipeMessage.Message);
                        break;
                    case TraceEventType.Information:
                        logger.Info(pipeMessage.Message);
                        break;
                    default:
                        logger.Debug(pipeMessage.Message);
                        break;
                }
            }

        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnStop();

            server.Dispose();

            if (!Program.IsInteractiveStartup)
            {
                this.RequestAdditionalTime(ADDITIONAL_TIME_MS);
            }
        }
        
        protected override void OnPause()
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnPause();
        }
        
        protected override void OnContinue()
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnContinue();
        }
        
        protected override void OnCustomCommand(int command)
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnCustomCommand(command);
        }
        
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnSessionChange(changeDescription);
        }
        
        protected override void OnShutdown()
        {
            EventLog.WriteEntry(string.Format(Messages.MESSAGE_TYPE_NAMESPACE_FULLNAME, GetType().FullName, Method.GetName()), EventLogEntryType.Information);

            base.OnShutdown();

            if (!Program.IsInteractiveStartup)
            {
                this.RequestAdditionalTime(ADDITIONAL_TIME_MS);
            }
        }
    }
}
