        /// <summary>
        /// Checks if application can connect to QB
        /// </summary>
        /// <returns>Status</returns>
        public QBCanConnectStatus CanConnect()
        {
            QBCanConnectStatus canConnectStatus = QBCanConnectStatus.ErrorUnknown;
            if (isConnectionOpen && currentAppInfo != null)
            {
                QBVersion currentVersion = currentAppInfo.CurrentVersion;
                if(currentVersion.MajorVersion == 0)
                {
                    canConnectStatus = QBCanConnectStatus.ErrorQBNotInstalled;
                    return canConnectStatus;
                }

                if (  (currentVersion.MajorVersion >= MinSupportedQBVersion.MajorVersion) && (currentVersion.MinorVersion >= MinSupportedQBVersion.MinorVersion) )
                {
                    if ((currentVersion.MajorVersion <= MaxSupportedQBVersion.MajorVersion) && (currentVersion.MinorVersion <= MaxSupportedQBVersion.MinorVersion))
                    {
                        canConnectStatus = QBCanConnectStatus.Success;
                    }
                    else
                    {
                        canConnectStatus = QBCanConnectStatus.ErrorCurrentQBVersionHigherThanMaximum;
                    }
                }
                else 
                {
                    canConnectStatus = QBCanConnectStatus.ErrorCurrentQBVersionLowerThanMinimum;
                }
            }

            return canConnectStatus;
        }