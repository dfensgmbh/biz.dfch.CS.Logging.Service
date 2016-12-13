/**
 * Copyright 2016 d-fens GmbH
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
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Logging.Service.Core
{
    public class ProgramHelp
    {
        public string GetHelpMessage()
        {
            return string.Format(Messages.HELP_HELP_MESSAGE, GetVersion(), GetArchitecture());
        }

        public string GetInteractiveMessage()
        {
            return string.Format(Messages.HELP_INTERACTIVE_MESSAGE, GetVersion(), GetArchitecture());
        }

        public Version GetVersion()
        {
            Contract.Ensures(null != Contract.Result<Version>());

            var version = this.GetType().Assembly.GetName().Version;
            return version;
        }

        public string GetArchitecture()
        {
            var application = Environment.Is64BitProcess ? Messages.ARCHITECTURE_X64 : Messages.ARCHITECTURE_X86;
            var operatingSystem = Environment.Is64BitOperatingSystem ? Messages.ARCHITECTURE_X64 : Messages.ARCHITECTURE_X86;

            var result = string.Format(Messages.ARCHITECTURE_FORMATTER, application, operatingSystem);
            return result;
        }

    }
}
