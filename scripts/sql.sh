#!/usr/bin/env bash

set -eou pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
BIN=$SCRIPT_DIR/cloud-sql-proxy

if [[ ! -f $BIN ]] ; then
  OS=$(uname | tr '[:upper:]' '[:lower:]')
  ARCH=$(uname -m)
  case $ARCH in
    x86_64)
        ARCH=amd64
        ;;
    aarch64)
        ARCH=arm64
        ;;
    amd64)
        ;;
    arm64)
        ;;
    *)
        echo "Unsupported architecture: $ARCH"
        exit 1
        ;;
  esac
  echo "Downloading Cloud SQL Proxy for $OS $ARCH..."
  curl -f -o $BIN.tmp "https://storage.googleapis.com/cloud-sql-connectors/cloud-sql-proxy/v2.21.1/cloud-sql-proxy.${OS}.${ARCH}"
  mv $BIN.tmp $BIN
  chmod +x $BIN
fi

$BIN --address 127.0.0.1 --port 5432 --http-address 127.0.0.1 --http-port 9191 --health-check \
  "austinsql:us-central1:austinsql" &
BGPID=$!

# prepare to clean up the proxy when the script exits
cleanup() {
    echo "Terminating SQL proxy (PID: $BGPID)..."
    kill "$BGPID"
    wait "$BGPID" > /dev/null 2>&1 || true
    echo "SQL proxy has been killed."
}
trap cleanup EXIT

# wait for the proxy to start up
$BIN wait --http-address 127.0.0.1 --http-port 9191

psql "host=127.0.0.1 sslmode=disable dbname=dkp user=postgres"
