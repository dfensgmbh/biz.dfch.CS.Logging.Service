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
 */

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.ServiceProcess;
using biz.dfch.CS.Commons.Diagnostics;

namespace biz.dfch.CS.Logging.Service.Core
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            var programHelp = new ProgramHelp();

            Logger.Get(Constants.TRACE_SOURCE_NAME).TraceEvent
            (
                TraceEventType.Verbose, 
                ushort.MaxValue -1, 
                Messages.STARTUP_INFO, 
                programHelp.GetVersion(), 
                programHelp.GetArchitecture(), 
                Method.GetName(), 
                Environment.UserInteractive
            );

            if (Environment.UserInteractive)
            {
                if(null != args && 1 <= args.Length)
                {
                    const char trimChar0 = '-';
                    const char trimChar1 = '/';

                    var arg0 = args[0].TrimStart(trimChar0).TrimStart(trimChar1).ToUpper();
                    Contract.Assert(!string.IsNullOrWhiteSpace(arg0), args[0]);

                    if (arg0.Equals(Messages.CMDLINE_SWITCH_PIPENAME) && 2 <= args.Length)
                    {
                        var pipeName = args[1].Trim();
                        Contract.Assert(!string.IsNullOrWhiteSpace(pipeName));
                    }
                    else if 
                    (
                        arg0.Equals(Messages.CMDLINE_SWITCH_HELP) 
                        || 
                        arg0.Equals(Messages.CMDLINE_SWITCH_H)
                        || 
                        arg0.Equals(Messages.CMDLINE_SWITCH_QMARK)
                    )
                    {
                        Console.WriteLine(new ProgramHelp().GetHelpMessage());

                        return;
                    }
                }

                var service = new LoggingService();

                Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    service.TerminateInteractiveService();
                };

                service.OnStartInteractive(args);
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] 
                { 
                    new LoggingService() 
                });
            }
        }
    }
}
