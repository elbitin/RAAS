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
﻿using System.ServiceModel;

namespace Elbitin.Applications.RAAS.RAASClient.RAASClient
{
    [ServiceContract(Namespace = "http://applications.elbitin.com/WCF/RAASClient/1.0.0/RAASClient.RAASClient.IRAASClientService",
        SessionMode = SessionMode.Required)]
    public interface IRAASClientService
    {
        [OperationContract]
        void StartRemoteApplication(string application, string arguments, string serverName);

        [OperationContract]
        void ConnectServer(string serverName);

        [OperationContract]
        void DisconnectServer(string serverName);

        [OperationContract]
        int GetServerStatus(string serverName);

        [OperationContract]
        string GetServerVersion(string serverName);

        [OperationContract]
        void StartAutostart(string arguments, string serverName);

        [OperationContract]
        void StartShortcutsServer(string arguments, string serverName);

        [OperationContract]
        void StartKeepAlive(string arguments, string serverName);
    }
}
