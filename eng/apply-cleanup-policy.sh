#!/usr/bin/env bash

# documentation:
# https://cloud.google.com/artifact-registry/docs/repositories/cleanup-policy

set -e

gcloud artifacts repositories set-cleanup-policies cloud-run-source-deploy \
    --project=dinnerkillpoints \
    --location=us-central1 \
    --policy=artifact-registry-cleanup-policy.json \
    --no-dry-run
