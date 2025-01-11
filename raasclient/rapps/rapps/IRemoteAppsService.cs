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
﻿// Copyright (c) Elbitin
using System.ServiceModel;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    [ServiceContract(Namespace = "http://applications.elbitin.com/WCF/RAASClient/1.0.0/RAASClient.RemoteApps.IRemoteAppsService",
        SessionMode = SessionMode.Required, CallbackContract = typeof(IRemoteAppsChange))]
    public interface IRemoteAppsService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Unsubscribe();

        [OperationContract]
        void StartRemoteApplication(string application, string arguments);

        [OperationContract]
        void Connect();

        [OperationContract]
        void Disconnect();

        [OperationContract]
        int GetStatus();
    }
}
