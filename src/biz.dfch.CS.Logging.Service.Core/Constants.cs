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

namespace biz.dfch.CS.Logging.Service.Core
{
    public static class Constants
    {
        public const string TRACE_SOURCE_NAME = "biz.dfch.CS.Logging.Service";

        public enum EventIds
        {
            Stop = ushort.MaxValue - 2,
            Start = ushort.MaxValue - 1,
        }
    }
}
