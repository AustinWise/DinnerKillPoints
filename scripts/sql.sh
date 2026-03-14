#!/usr/bin/env bash

set -eou pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
BIN=$SCRIPT_DIR/cloud-sql-proxy

if [[ ! -f $BIN ]] ; then
  echo "Downloading Cloud SQL Proxy..."
  curl -o $BIN https://storage.googleapis.com/cloud-sql-connectors/cloud-sql-proxy/v2.21.1/cloud-sql-proxy.darwin.arm64
  chmod +x $BIN
fi

$BIN --address 127.0.0.1 --port 5432 "austinsql:us-central1:austinsql"
