#!/bin/sh
set -e
set -x

REMOTE_SERVER=10.5.2.106
REMOTE_USER=root
REMOTE_PATH=/root/
PUBLISH_OS=linux-x64
PUBLISH_FOLDER=bin/Release/netcoreapp2.0/${PUBLISH_OS}


dotnet restore
dotnet publish -r ${PUBLISH_OS} -c Release
tar cf publish.tar -C $PUBLISH_FOLDER publish
scp publish.tar $REMOTE_USER@$REMOTE_SERVER:$REMOTE_PATH
scp remoteSetup.sh $REMOTE_USER@$REMOTE_SERVER:$REMOTE_PATH

ssh $REMOTE_USER@$REMOTE_SERVER bash remoteSetup.sh

echo SUCCESS!
