#!/bin/sh
set -e
set -x

REMOTE_SERVER=10.5.2.106
REMOTE_USER=root
REMOTE_PATH=/root/

dotnet restore
dotnet publish -r ubuntu.16.04-x64 -c Release
tar cf publish.tar -C bin/Release/netcoreapp1.0/ubuntu.16.04-x64 publish
scp publish.tar $REMOTE_USER@$REMOTE_SERVER:$REMOTE_PATH
scp remoteSetup.sh $REMOTE_USER@$REMOTE_SERVER:$REMOTE_PATH

ssh $REMOTE_USER@$REMOTE_SERVER bash remoteSetup.sh

echo SUCCESS!
