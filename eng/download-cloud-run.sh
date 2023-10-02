#!/usr/bin/env bash

set -e


gcloud run services describe \
    --format=yaml \
    --project=dinnerkillpoints \
    --region us-central1 \
    dkpweb \
    > cloudrun.yaml
