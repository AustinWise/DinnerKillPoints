#!/bin/sh
set -e
set -x

REMOTE_SERVER=10.5.2.106
REMOTE_USER=root
REMOTE_PATH=/root/publish

dotnet restore
dotnet publish -r ubuntu.16.04-x64 -c Release
ssh $REMOTE_USER@$REMOTE_SERVER systemctl stop dkp
#TODO: test for existence of REMOTE_PATH rather than '|| true'
ssh $REMOTE_USER@$REMOTE_SERVER "rm -r $REMOTE_PATH || true"
scp -r ./bin/Release/netcoreapp1.0/ubuntu.16.04-x64/publish/ $REMOTE_USER@$REMOTE_SERVER:$REMOTE_PATH
ssh $REMOTE_USER@$REMOTE_SERVER "chmod +x $REMOTE_PATH/DkpWeb && systemctl start dkp"
echo SUCCESS!
