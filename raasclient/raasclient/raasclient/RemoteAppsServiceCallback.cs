/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Client.
 *
 * RAAS Client is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Client is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Client. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using Elbitin.Applications.RAAS.RAASClient.RAASClient.RemoteAppsServiceRef;
using System.ServiceModel;

namespace Elbitin.Applications.RAAS.RAASClient.RAASClient
{
    public delegate void ConnectedDelegate();
    [CallbackBehaviorAttribute(UseSynchronizationContext = false)]
    public class RemoteAppsServiceCallback : IRemoteAppsServiceCallback
    {
        public event ConnectedDelegate ConnectedEvent;

        public void Connected()
        {
            ConnectedEvent?.BeginInvoke(null, null);
        }
    }
}